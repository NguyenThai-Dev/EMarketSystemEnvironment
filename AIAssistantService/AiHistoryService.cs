using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIAssistantService
{
    public interface IAiHistoryService
    {
        Task SaveLogAsync(string sessionId, string role, string content);
        Task<List<ChatMessageContent>> GetRecentHistoryAsync(string sessionId);

        Task SaveLearningErrorAsync(string prompt, string invalidSql, string errorMessage, string correctedSql = null);
        Task<string> GetRelevantLessonsAsync(string prompt);
    }

    public class AiHistoryService : IAiHistoryService
    {
        private readonly string _connectionString;
        public AiHistoryService(string connStr) => _connectionString = connStr;

        public async Task<List<ChatMessageContent>> GetRecentHistoryAsync(string sessionId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                var history = await conn.QueryAsync<dynamic>(
                    @"SELECT TOP 10 [Role], [Content] 
                  FROM AI_ChatLog 
                  WHERE SessionId = @sid 
                  ORDER BY CreatedAt DESC",
                    new { sid = sessionId });

                var chatHistory = new List<ChatMessageContent>();
                foreach (var log in history.Reverse())
                {
                    var role = log.Role == "user" ? AuthorRole.User : AuthorRole.Assistant;
                    chatHistory.Add(new ChatMessageContent(role, log.Content));
                }
                return chatHistory;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[DB HISTORY ERROR]: {ex.Message}");
                return new List<ChatMessageContent>();
            }
        }

        public async Task SaveLogAsync(string sessionId, string role, string content)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.ExecuteAsync(
                    "INSERT INTO AI_ChatLog (SessionId, [Role], [Content]) VALUES (@sid, @r, @c)",
                    new { sid = sessionId, r = role, c = content });
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[DB LOG ERROR]: {ex.Message}");
            }
        }

        #region [Self-Learning Mechanism]

        // Ghi lại sai lầm để lần sau AI né
        public async Task SaveLearningErrorAsync(string prompt, string invalidSql, string errorMessage, string correctedSql = null)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.ExecuteAsync(@"
                    INSERT INTO AI_Query_Learning (UserPrompt, InvalidSQL, ErrorMessage, CorrectedSQL, IsSuccess)
                    VALUES (@p, @isql, @err, @csql, @status)",
                    new
                    {
                        p = prompt,
                        isql = invalidSql,
                        err = errorMessage,
                        csql = correctedSql,
                        status = (correctedSql != null)
                    });
                Console.WriteLine("" +
                    "[Learnig from Error]." +
                    "");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[LEARNING SAVE ERROR]: {ex.Message}");
            }
        }

        // Lấy ra các bài học cũ để nạp vào Prompt làm "Lưu ý" cho AI
        public async Task<string> GetRelevantLessonsAsync(string prompt)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                // Dùng LIKE đơn giản hoặc sau này nâng cấp lên Vector Search
                var lessons = await conn.QueryAsync<string>(@"
                    SELECT TOP 10 
                    ('Lỗi cũ: ' + ErrorMessage + ' -> Giải pháp: ' + CorrectedSQL)
                    FROM AI_Query_Learning
                    WHERE @p LIKE '%' + UserPrompt + '%' AND IsSuccess = 1
                    ORDER BY CreatedAt DESC", new { p = prompt });

                return lessons.Any()
                    ? "\n[BÀI HỌC KINH NGHIỆM TỪ LỖI CŨ]:\n" + string.Join("\n", lessons)
                    : string.Empty;
            }
            catch (SqlException)
            {
                return string.Empty;
            }
        }
        #endregion
    }
}