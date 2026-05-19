using Dapper;
using Npgsql;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.RegularExpressions;

namespace AIAssistantService
{
    /// <summary>
    /// Kết quả trả về từ Pre-Processing Gate.
    /// </summary>
    public class CacheCheckResult
    {
        /// <summary>Prompt chứa dấu hiệu Prompt Injection — chặn lại, không gọi AI.</summary>
        public bool IsInjectionDetected { get; set; }

        /// <summary>
        /// Câu trả lời đã có trong lịch sử (khớp mờ ≥ ngưỡng).
        /// Null = chưa có cache, cần gọi AI bình thường.
        /// </summary>
        public string? CachedAnswer { get; set; }

        public bool HasCache => CachedAnswer != null;
    }

    public interface IAiHistoryService
    {
        Task SaveLogAsync(string sessionId, string role, string content, string? modelName = null, int tokensUsed = 0, string? toolCalls = null, string? toolCallId = null);
        Task<List<ChatMessageContent>> GetRecentHistoryAsync(string sessionId);

        Task SaveLearningErrorAsync(string prompt, string invalidSql, string errorMessage, string correctedSql = null);
        Task<string> GetRelevantLessonsAsync(string prompt);

        /// <summary>
        /// Pre-Processing Gate: chạy TRƯỚC khi AI được gọi.
        /// Làm 2 việc: (1) phát hiện Prompt Injection, (2) tìm cache mờ trong lịch sử 2 giờ.
        /// </summary>
        Task<CacheCheckResult> CheckCacheAndSafetyAsync(string sessionId, string prompt);
    }

    public class AiHistoryService : IAiHistoryService
    {
        private readonly string _connectionString;
        public AiHistoryService(string connStr) => _connectionString = connStr;

        // ──────────────────────────────────────────────
        // PRIVATE HELPERS — Pre-Processing Gate Logic
        // ──────────────────────────────────────────────

        /// <summary>
        /// Phát hiện Prompt Injection: kiểm tra các mẫu nguy hiểm phổ biến.
        /// Kết hợp cả tiếng Anh lẫn tiếng Việt.
        /// </summary>
        private static bool IsPromptInjection(string prompt)
        {
            var lower = prompt.ToLowerInvariant();

            // Tiếng Anh — instruction override patterns
            var enPatterns = new[]
            {
                @"ignore\s+(previous|all|your)\s+(instructions?|prompt|rules?)",
                @"forget\s+(your|all|previous)\s+(instructions?|rules?|context)",
                @"you\s+are\s+now\s+",
                @"act\s+as\s+(a\s+)?(?!an?\s+analyst)",   // "act as" nhưng không phải analyst
                @"disregard\s+(the\s+)?(system|previous)",
                @"new\s+persona",
                @"jailbreak",
                @"\bdan\b",                                 // Do Anything Now
                @"developer\s+mode",
            };

            // Tiếng Việt — các mẫu tương đương
            var viPatterns = new[]
            {
                @"bỏ\s+qua\s+(hướng\s+dẫn|lệnh|vai\s+trò|quy\s+tắc)",
                @"quên\s+(vai\s+trò|hướng\s+dẫn|hệ\s+thống|tất\s+cả)",
                @"đóng\s+vai\s+(là\s+)?(?!chuyên\s+viên)",  // không phải chuyên viên
                @"giả\s+vờ\s+(là|bạn\s+là)",
                @"hãy\s+là\s+(một\s+)?(?!cố\s+vấn|chuyên)",
                @"hệ\s+thống\s+mới",
                @"lệnh\s+mới",
            };

            // SQL/XSS injection
            var techPatterns = new[]
            {
                @";\s*drop\s+table",
                @"union\s+select",
                @"--\s*$",
                @"<\s*script",
                @"javascript\s*:",
                @"onerror\s*=",
            };

            foreach (var p in enPatterns.Concat(viPatterns).Concat(techPatterns))
            {
                if (Regex.IsMatch(lower, p, RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Chuẩn hóa văn bản: lowercase, bỏ dấu câu thừa, thu gọn khoảng trắng.
        /// Giữ nguyên dấu tiếng Việt để so sánh chính xác hơn.
        /// </summary>
        private static string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            // Bỏ dấu câu không cần thiết, giữ chữ và số và khoảng trắng
            var cleaned = Regex.Replace(text.ToLowerInvariant().Trim(), @"[^\p{L}\p{N}\s]", " ");

            // Thu gọn nhiều khoảng trắng thành một
            return Regex.Replace(cleaned, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Tính Jaccard Similarity dựa trên tập từ (word-level).
        /// Phù hợp với tiếng Việt (tách từ bằng khoảng trắng).
        /// Kết quả: [0.0 – 1.0], trong đó 1.0 = giống hoàn toàn.
        /// </summary>
        private static double ComputeJaccardSimilarity(string a, string b)
        {
            var setA = new HashSet<string>(a.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            var setB = new HashSet<string>(b.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (setA.Count == 0 && setB.Count == 0) return 1.0;
            if (setA.Count == 0 || setB.Count == 0) return 0.0;

            int intersection = setA.Count(w => setB.Contains(w));
            int union = setA.Union(setB).Count();

            return (double)intersection / union;
        }

        // ──────────────────────────────────────────────
        // PRE-PROCESSING GATE (Public)
        // ──────────────────────────────────────────────

        /// <summary>
        /// Cổng kiểm tra an toàn + cache — chạy TRƯỚC khi AI được gọi.
        ///
        /// Luồng xử lý:
        ///   [1] Phát hiện Prompt Injection → chặn ngay, trả IsInjectionDetected = true
        ///   [2] Lấy 15 cặp Q&A gần nhất từ DB (ROW_NUMBER để ghép đúng cặp)
        ///   [3] Tính Jaccard Similarity giữa prompt hiện tại với từng câu hỏi lịch sử
        ///   [4] Nếu similarity ≥ 0.75 (≥75% từ trùng nhau) → trả về câu trả lời cache
        ///   [5] Không tìm thấy → trả null, để controller tiếp tục gọi AI
        /// </summary>
        public async Task<CacheCheckResult> CheckCacheAndSafetyAsync(string sessionId, string prompt)
        {
            var result = new CacheCheckResult();

            // ── BƯỚC 1: Kiểm tra Prompt Injection ──────────────────────────────
            if (IsPromptInjection(prompt))
            {
                result.IsInjectionDetected = true;
                Console.WriteLine($"[SECURITY] Prompt Injection detected for user '{sessionId}': {prompt[..Math.Min(80, prompt.Length)]}...");
                return result;
            }

            // ── BƯỚC 2-4: Tìm cache mờ trong lịch sử ──────────────────────────
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);

                // Dùng ROW_NUMBER() để ghép chính xác mỗi câu hỏi (q) với câu trả lời ngay sau (a).
                // Tránh cross-join sai cặp như cách JOIN đơn giản cũ.
                // Dùng dynamic thay vì ValueTuple vì Npgsql trả tên cột dưới dạng lowercase.
                // QueryAsync<(string UserContent, ...)> sẽ map sai vì field "UserContent" ≠ column "usercontent".
                var pairs = await conn.QueryAsync<dynamic>(@"
                    WITH ranked_msgs AS (
                        SELECT
                            m.role,
                            m.content,
                            m.created_at,
                            ROW_NUMBER() OVER (PARTITION BY m.conversation_id ORDER BY m.created_at ASC) AS rn,
                            m.conversation_id
                        FROM messages m
                        JOIN conversations c ON m.conversation_id = c.id
                        WHERE c.user_id = @sid
                          AND m.created_at >= NOW() - INTERVAL '2 hours'
                          AND LOWER(m.role) IN ('user', 'assistant')
                          AND m.content IS NOT NULL
                          AND m.content <> ''
                    )
                    SELECT q.content AS user_content, a.content AS assistant_content
                    FROM ranked_msgs q
                    JOIN ranked_msgs a
                        ON a.conversation_id = q.conversation_id
                       AND a.rn = q.rn + 1
                       AND LOWER(a.role) = 'assistant'
                    WHERE LOWER(q.role) = 'user'
                      AND LENGTH(a.content) >= 80
                      AND a.content NOT ILIKE '%tôi cần thêm thông tin%'
                      AND a.content NOT ILIKE '%bạn có thể cung cấp%'
                      AND a.content NOT ILIKE '%cung cấp%id%'
                    ORDER BY q.created_at DESC
                    LIMIT 15",
                    new { sid = sessionId });

                string normalizedPrompt = NormalizeText(prompt);

                // Duyệt các cặp theo thứ tự từ mới nhất đến cũ nhất
                foreach (var pair in pairs)
                {
                    string userContent      = (string)pair.user_content;
                    string assistantContent = (string)pair.assistant_content;

                    string normalizedHistory = NormalizeText(userContent);
                    double similarity = ComputeJaccardSimilarity(normalizedPrompt, normalizedHistory);

                    Console.WriteLine($"[CACHE CHECK] Similarity={similarity:F2} | '{prompt[..Math.Min(40, prompt.Length)]}' vs '{userContent[..Math.Min(40, userContent.Length)]}'");

                    // Ngưỡng 0.75: ≥75% từ trùng nhau → coi là "cùng câu hỏi", trả cache ngay
                    if (similarity >= 0.75)
                    {
                        result.CachedAnswer = assistantContent;
                        Console.WriteLine($"[CACHE HIT] Similarity={similarity:F2} — trả cache, bỏ qua AI.");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                // Cache miss do lỗi DB → để AI xử lý bình thường, không block
                Console.WriteLine($"[CACHE CHECK ERROR]: {ex.Message}");
            }

            return result; // CachedAnswer = null, IsInjectionDetected = false → tiếp tục gọi AI
        }

        // ──────────────────────────────────────────────
        // HISTORY — Inject vào ChatHistory của SK
        // ──────────────────────────────────────────────

        public async Task<List<ChatMessageContent>> GetRecentHistoryAsync(string sessionId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                // LOWER(m.role) để xử lý trường hợp SK lưu 'Assistant' (capital A) thay vì 'assistant'.
                // PostgreSQL so sánh chuỗi phân biệt hoa thường — đây là nguyên nhân gốc rễ khiến
                // history luôn trả về rỗng và AI không có bộ nhớ ngắn hạn.
                var history = await conn.QueryAsync<dynamic>(
                    @"SELECT m.role, m.content 
                      FROM messages m
                      JOIN conversations c ON m.conversation_id = c.id
                      WHERE c.user_id = @sid 
                        AND LOWER(m.role) IN ('user', 'assistant')
                        AND m.content IS NOT NULL
                        AND m.content <> ''
                        AND m.created_at >= NOW() - INTERVAL '2 hours'
                      ORDER BY m.created_at DESC
                      LIMIT 20",
                    new { sid = sessionId });

                var chatHistory = new List<ChatMessageContent>();
                foreach (var log in history.Reverse())
                {
                    var roleStr = (string)log.role;
                    // OrdinalIgnoreCase để match cả 'user' lẫn 'User', 'assistant' lẫn 'Assistant'
                    var role = roleStr.Equals("user", StringComparison.OrdinalIgnoreCase)
                        ? AuthorRole.User
                        : AuthorRole.Assistant;
                    chatHistory.Add(new ChatMessageContent(role, (string)log.content));
                }
                return chatHistory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB HISTORY ERROR]: {ex.Message}");
                return new List<ChatMessageContent>();
            }
        }

        // ──────────────────────────────────────────────
        // SAVE LOG
        // ──────────────────────────────────────────────

        public async Task SaveLogAsync(string sessionId, string role, string content, string? modelName = null, int tokensUsed = 0, string? toolCalls = null, string? toolCallId = null)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                var convId = await GetOrCreateConversationAsync(conn, sessionId);
                
                await conn.ExecuteAsync(
                    @"INSERT INTO messages (conversation_id, role, content, model_name, tokens_used, tool_calls, tool_call_id) 
                      VALUES (@convId, @r, @c, @m, @t, CAST(@tc AS jsonb), @tcid)",
                    new { convId, r = role, c = content, m = modelName, t = tokensUsed, tc = toolCalls, tcid = toolCallId });
                    
                await conn.ExecuteAsync("UPDATE conversations SET last_message_at = NOW() WHERE id = @convId", new { convId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB LOG ERROR]: {ex.Message}");
            }
        }

        private async Task<Guid> GetOrCreateConversationAsync(NpgsqlConnection conn, string userId)
        {
            var conversationId = await conn.QueryFirstOrDefaultAsync<Guid?>(
                "SELECT id FROM conversations WHERE user_id = @userId AND is_active = true ORDER BY created_at DESC LIMIT 1",
                new { userId });

            if (!conversationId.HasValue || conversationId.Value == Guid.Empty)
            {
                conversationId = await conn.QuerySingleAsync<Guid>(
                    "INSERT INTO conversations (user_id, title) VALUES (@userId, 'New Chat') RETURNING id",
                    new { userId });
            }
            return conversationId.Value;
        }

        #region [Self-Learning Mechanism]
        public async Task SaveLearningErrorAsync(string prompt, string invalidSql, string errorMessage, string correctedSql = null)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.ExecuteAsync(@"
                    INSERT INTO ai_query_learning (user_prompt, invalid_sql, error_message, corrected_sql, is_success)
                    VALUES (@p, @isql, @err, @csql, @status)",
                    new
                    {
                        p = prompt,
                        isql = invalidSql,
                        err = errorMessage,
                        csql = correctedSql,
                        status = (correctedSql != null)
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LEARNING SAVE ERROR]: {ex.Message}");
            }
        }

        public async Task<string> GetRelevantLessonsAsync(string prompt)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                var lessons = await conn.QueryAsync<string>(@"
                    SELECT ('Lỗi cũ: ' || error_message || ' -> Giải pháp: ' || corrected_sql)
                    FROM ai_query_learning
                    WHERE @p ILIKE '%' || user_prompt || '%' AND is_success = true
                    ORDER BY created_at DESC
                    LIMIT 10", new { p = prompt });

                return lessons.Any()
                    ? "\n[BÀI HỌC KINH NGHIỆM TỪ LỖI CŨ]:\n" + string.Join("\n", lessons)
                    : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion
    }
}