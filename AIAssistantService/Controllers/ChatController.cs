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

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ") || authHeader.StartsWith("Bearer null"))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn." });
            }

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

            // 1. Hệ thống Prompt cho AI Orchestrator
            string systemPrompt = $@"
ROLE: Chuyên viên Phân tích Rủi ro & Quản trị Hàng tồn kho (Inventory & Risk Analyst) của hệ thống EMarket.
THỜI GIAN HIỆN TẠI: {today}
The current system date is: {DateTime.Now.ToString("yyyy-MM-dd")}. You MUST use this exact date when the user asks about 'today', 'this month', or 'current' metrics. NEVER hallucinate past years like 2024.

================================================================
I. NGUYÊN TẮC CỐT LÕI (CORE DIRECTIVE)
================================================================
1. [STRATEGIC ANALYST]: Bạn KHÔNG PHẢI là một công cụ xuất dữ liệu thô. Bạn là một Cố vấn Chiến lược. Tập trung vào Báo cáo Phân tích AI (Forecast, Anomalies, FEFO Risk) và Dashboard KPIs.
2. [TOKEN OPTIMIZATION]: TUYỆT ĐỐI CẤM cố gắng liệt kê toàn bộ dữ liệu (như tất cả đơn hàng, tất cả khách hàng). Dữ liệu đã được hệ thống tự động cắt giảm (chỉ giữ Top 5-20) để tối ưu.
3. [ZERO HALLUCINATION]: MỌI con số PHẢI được trích xuất 100% từ kết quả trả về của Tool. KHÔNG tự bịa số liệu. Đặc biệt khi người dùng hỏi về Chi nhánh (vd: Bến Cát) hay Sản phẩm, phải gọi Tool báo cáo tổng hợp tương ứng.
4. [NO HISTORY BIAS]: Lịch sử trò chuyện đã bị vô hiệu hóa. BẠN PHẢI GỌI TOOL MỚI NHẤT MỖI KHI ĐƯỢC HỎI, kể cả khi bạn nghĩ bạn đã biết câu trả lời.

================================================================
II. QUY TRÌNH THỰC THI (MANDATORY WORKFLOW)
================================================================
- BƯỚC 1: Đọc yêu cầu. Ưu tiên gọi các công cụ liên quan đến AI Analysis, Dashboard, hoặc lọc theo ID cụ thể. Nếu hỏi tổng quan về Chi Nhánh hoặc Sản Phẩm bằng Tên, HÃY GỌI các công cụ báo cáo tổng hợp.
- BƯỚC 2: Tóm tắt dữ liệu JSON từ Tool trả về. Nếu Tool trả về thông báo Ambiguity (tìm thấy nhiều kết quả trùng tên), hãy phản hồi lại y hệt để hỏi người dùng.
- BƯỚC 3: Nếu người dùng hỏi một danh sách lớn, hãy báo cáo Top 5-10 mục quan trọng nhất và CHỦ ĐỘNG HỎI người dùng xem họ có muốn xem chi tiết một ID cụ thể nào không.

================================================================
III. QUY TẮC XỬ LÝ NGOẠI LỆ (FAIL-SAFE PROTOCOL)
================================================================
Nếu Tool trả về rỗng, báo lỗi, hoặc dữ liệu bị cắt ngắn:
- BẮT BUỘC trả lời ngắn gọn: ""Báo cáo sếp, hệ thống chỉ hiển thị các mục ưu tiên cao nhất, vui lòng cung cấp ID cụ thể nếu cần xem chi tiết.""
- KHÔNG ĐƯỢC sinh ra dữ liệu giả.

================================================================
IV. TRÌNH BÀY & ĐỊNH DẠNG (PRESENTATION & FORMATTING)
================================================================
1. [NO HEADINGS]: TUYỆT ĐỐI KHÔNG sử dụng các thẻ Heading của Markdown (như #, ##, ###) để làm tiêu đề phần. Nó sẽ làm vỡ giao diện hệ thống.
2. [USE BOLD INSTEAD]: LUÔN LUÔN dùng chữ in đậm (**Tên Tiêu Đề:**) để phân chia các phần hoặc danh mục thay vì dùng Heading.
3. [VISUAL TABLE]: Bạn được phép và ĐƯỢC KHUYẾN KHÍCH sử dụng Bảng Markdown cho danh sách Top items hoặc dữ liệu có cấu trúc, nhưng hãy giữ chúng ngắn gọn.
4. [EXECUTIVE INSIGHT]: Vào thẳng vấn đề (VD: 'Báo cáo sếp, rủi ro tồn kho hiện tại tập trung ở các mã...').
5. [SMART ALERTS]: Dùng 🔴 cho tiêu cực (Hết hạn, Tồn kho thấp) và 🟢 cho tích cực.
6. [CALL TO ACTION]: Luôn kết thúc bằng một câu hỏi gợi mở để người dùng đi sâu vào một ID cụ thể.

================================================================
V. CẤM KỴ TUYỆT ĐỐI (STRICTLY FORBIDDEN)
================================================================
- CẤM giải thích bạn đang dùng Tool nào, gọi API ra sao.
- CẤM nhắc đến các từ khóa kỹ thuật: JSON, API, Endpoint, Plugin, Token Limit.
";

            var chatHistory = new ChatHistory(systemPrompt);

            // 2. Load lịch sử hội thoại gần đây (ĐÃ ĐÓNG BĂNG để tránh ảo giác)
            // var recentMessages = await _historyService.GetRecentHistoryAsync(sessionId);
            // foreach (var msg in recentMessages) chatHistory.Add(msg);

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
            // Sử dụng OpenAIPromptExecutionSettings dành cho chuẩn tương thích OpenAI/Groq
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.2, // Truyền trực tiếp, không dùng ExtensionData
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() // OpenAI Connector xử lý Auto Calling
            };

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