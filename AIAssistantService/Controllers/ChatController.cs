using AIAssistantService.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;

namespace AIAssistantService.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly DatabasePlugin _dbService;
        private readonly Kernel _kernel;
        private readonly IAiHistoryService _historyService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPromptService _promptService;

        public ChatController(Kernel kernel,
            IAiHistoryService historyService,
            DatabasePlugin databasePlugin, 
            IHttpClientFactory httpClientFactory,
            IPromptService promptService)
        {
            _kernel = kernel;
            _historyService = historyService;
            _dbService = databasePlugin;
            _httpClientFactory = httpClientFactory;
            _promptService = promptService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            string SqlGenSystemMessage_V2 = _promptService.GetPrompt("SqlGenerator"); ;
            string ReporterSystemMessage_V2 = _promptService.GetPrompt("Reporter");

            string baseUrl = await _dbService.GetAppBaseUrl();

            var client = _httpClientFactory.CreateClient("EMarketClient");
            client.BaseAddress = new Uri(baseUrl);

            string sessionId = request.UserName ?? "Unauthorized";
            string today = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm");

            // Config vòng lặp sửa lỗi
            int maxRetries = 3;
            int currentAttempt = 0;
            string lastError = "";

            // ==================================================================================
            // PHASE 1: KHỞI TẠO ARCHITECT (TƯ DUY BAN ĐẦU)
            // ==================================================================================
            string dynamicSchema = GetDynamicSchema(request.Prompt);
            string pastLessons = await _historyService.GetRelevantLessonsAsync(request.Prompt);

            var sqlSystemMessage = SqlGenSystemMessage_V2
                .Replace("{today}", today)
                .Replace("{dynamicSchema}", dynamicSchema)
                .Replace("{dynamicLessons}", string.IsNullOrEmpty(pastLessons) ? "Chưa có bài học nào." : pastLessons);

            var sqlHistory = new ChatHistory(sqlSystemMessage);
            var recentMessages = await _historyService.GetRecentHistoryAsync(sessionId);
            foreach (var msg in recentMessages) sqlHistory.Add(msg);

            sqlHistory.AddUserMessage(request.Prompt);

            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var jsonSettings = new OpenAIPromptExecutionSettings { Temperature = 0, ResponseFormat = "json_object" };

            // Biến lưu trạng thái xuyên suốt vòng lặp
            AiThoughtProcess currentThoughtData = null;
            string finalResponseText = "";

            // ==================================================================================
            // PHASE 2 & 3: VÒNG LẶP THỰC THI & TỰ SỬA LỖI (THE SELF-HEALING LOOP)
            // ==================================================================================
            while (currentAttempt < maxRetries)
            {
                currentAttempt++;

                // BƯỚC A: LẤY SQL TỪ AI
                // (Lần 1: Lấy từ câu hỏi User. Lần 2+: Lấy từ yêu cầu sửa lỗi)
                var aiResponse = await chatService.GetChatMessageContentAsync(sqlHistory, jsonSettings, _kernel);
                currentThoughtData = ParseAiJson(aiResponse.Content);

                // CASE: AI từ chối viết SQL (Chat xã giao) -> Thoát vòng lặp luôn
                if (string.IsNullOrEmpty(currentThoughtData.Sql) || currentThoughtData.Sql.Trim().ToUpper() == "NONE")
                {
                    finalResponseText = currentThoughtData.Thought;
                    goto SaveAndReturn;
                }

                // BƯỚC B: CHẠY THỬ SQL
                try
                {
                    // [Engine] Thực thi
                    var rawData = await _dbService.ExecuteQueryAsync(currentThoughtData.Sql);

                    // NẾU CHẠY THÀNH CÔNG (Không Exception) -> XỬ LÝ DỮ LIỆU
                    if (rawData == null || !rawData.Any())
                    {
                        finalResponseText = $"Thưa sếp, tôi đã rà soát kỹ hệ thống theo yêu cầu '{request.Prompt}', nhưng hiện tại chưa ghi nhận dữ liệu nào phù hợp.";
                    }
                    else
                    {
                        // [Reporter] Báo cáo kết quả
                        string dataJson = JsonConvert.SerializeObject(rawData);
                        if (dataJson.Length > 10000) dataJson = dataJson.Substring(0, 10000) + "...";

                        var reportSystemMessage = ReporterSystemMessage_V2
                            .Replace("{today}", today)
                            .Replace("{databaseJson}", dataJson);

                        var reportHistory = new ChatHistory(reportSystemMessage);
                        reportHistory.AddUserMessage($"Câu hỏi gốc: {request.Prompt}. Hãy báo cáo kết quả.");

                        var reportResponse = await chatService.GetChatMessageContentAsync(reportHistory);
                        finalResponseText = reportResponse.Content;
                    }

                    // Nếu thành công thì thoát vòng lặp ngay
                    goto SaveAndReturn;
                }
                catch (Exception ex)
                {
                    // BƯỚC C: GẶP LỖI -> KÍCH HOẠT CƠ CHẾ TỰ SỬA (FIXING MODE)
                    lastError = ex.Message;
                    Console.WriteLine($"[ATTEMPT {currentAttempt} FAILED]: {lastError}");

                    // 1. Ghi lại lỗi vào DB để học lâu dài
                    await _historyService.SaveLearningErrorAsync(request.Prompt, currentThoughtData.Sql, lastError);

                    // 2. Nếu đã hết lượt thử -> Báo lỗi cho User
                    if (currentAttempt >= maxRetries)
                    {
                        finalResponseText = $"⚠️ **Thất bại sau {maxRetries} lần thử:** Tôi gặp khó khăn kỹ thuật với câu hỏi này.\n- **Lỗi cuối cùng:** {lastError}";
                        break;
                    }

                    // 3. CHƯA HẾT LƯỢT -> NẠP LỖI VÀO CONTEXT ĐỂ AI SỬA
                    // Mẹo: Đóng vai Assistant (SQL cũ) và User (Thông báo lỗi)
                    sqlHistory.AddAssistantMessage(aiResponse.Content); // Nhét lại câu JSON cũ vào mồm nó

                    // Thay vì viết string trực tiếp, bro gọi Service
                    string fixTemplate = _promptService.GetPrompt("SqlFixer");

                    string fixPrompt = fixTemplate.Replace("{lastError}", lastError);

                    sqlHistory.AddUserMessage(fixPrompt);

                    
                }
            }

        // ==================================================================================
        // PHASE 5: CLOSING
        // ==================================================================================
        SaveAndReturn:;

            if (!string.IsNullOrEmpty(finalResponseText))
            {
                await _historyService.SaveLogAsync(sessionId, "user", request.Prompt);
                await _historyService.SaveLogAsync(sessionId, "assistant", finalResponseText);
            }

            return Ok(new { answer = finalResponseText });
        }
        // ---------------------------------------------------------
        // HELPER METHODS (Giữ nguyên như cũ)
        // ---------------------------------------------------------
        public class AiThoughtProcess
        {
            public string Thought { get; set; } // Suy nghĩ của AI
            public string Sql { get; set; }     // Câu lệnh SQL
        }
        private AiThoughtProcess ParseAiJson(string aiContent)
        {
            try
            {
                var cleanJson = aiContent.Replace("```json", "").Replace("```", "").Trim();
                return JsonConvert.DeserializeObject<AiThoughtProcess>(cleanJson);
            }
            catch
            {
                // Fallback an toàn nếu AI trả về lỗi định dạng
                return new AiThoughtProcess
                {
                    Thought = aiContent, // Coi toàn bộ nội dung là câu trả lời text
                    Sql = "NONE"
                };
            }
        }

        private string GetDynamicSchema(string userPrompt)
        {
            var normalizedPrompt = userPrompt.ToLower();
            var schemaParts = new List<string>();

            // =========================================================================================
            // 1. CORE SYSTEM (Giữ nguyên)
            // =========================================================================================
            string coreSchema = _promptService.GetPrompt("Domains/Core");
            schemaParts.Add(coreSchema);

            // =========================================================================================
            // 2. INVENTORY DOMAIN (Bổ sung nhắc nhở về tên cột)
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"tồn|kho|lô|hết hạn|nhập|hàng|stock|inventory|còn|bao nhiêu|hiện tại|sắp hết|expiry|sản phẩm"))
            {
                string inventorySchema = _promptService.GetPrompt("Domains/Inventory");
            }

            // =========================================================================================
            // 3. SALES DOMAIN (Giữ nguyên)
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"bán|doanh thu|đơn|tiền|lợi nhuận|top|chạy nhất|mua|sales|order|bill|revenue"))
            {
                schemaParts.Add("Domains/Sales");
            }

            // =========================================================================================
            // 4. CUSTOMER DOMAIN (FIX LỖI LEVEL 4: QUÊN TÊN CỘT)
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"khách|vip|điểm|thành viên|ai mua|customer|loyalty|người dùng|người mua"))
            {
                schemaParts.Add("Domains/Customer");
            }

            // =========================================================================================
            // 5. FINANCE & PARTNERS 
            // =========================================================================================
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"nợ|chi phí|trả|lỗ|lãi|nhà cung cấp|expense|debt|supplier|nhập hàng|đối tác"))
            {
                schemaParts.Add("");
            }

            // =========================================================================================
            // 6. AI & ANALYTICS (FIX LỖI LEVEL 5: TỰ TÍNH TAY)
            // =========================================================================================
            // Bổ sung keyword: "ngày tới", "days", "nguy cơ", "cháy hàng"
            if (System.Text.RegularExpressions.Regex.IsMatch(normalizedPrompt, @"dự báo|tương lai|cảnh báo|gợi ý|thông minh|forecast|predict|warn|risk|xu hướng|ngày tới|days|nguy cơ|cháy hàng"))
            {
                schemaParts.Add("Domains/AIAndAnalytics");
            }

            // Fallback
            if (schemaParts.Count == 1)
            {
                schemaParts.Add("Domains/Fallbacks");
            }

            return string.Join("\n", schemaParts);
        }
    }

    public class ChatRequest
    {
        public required string Prompt { get; set; }
        public string? UserName { get; set; }
    }
}