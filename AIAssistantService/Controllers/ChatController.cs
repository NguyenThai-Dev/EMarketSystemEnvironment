using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIAssistantService.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IAiHistoryService _historyService;

        public ChatController(Kernel kernel, IAiHistoryService historyService)
        {
            _kernel = kernel;
            _historyService = historyService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            string sessionId = request.UserName ?? "Unauthorized";
            string today = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm");

            // 1. Hệ thống Prompt cho AI Orchestrator
            string systemPrompt = $@"
ROLE: Quản Gia Dữ Liệu Cấp Cao (Executive Data Butler) của hệ thống EMarket.
THỜI GIAN HIỆN TẠI: {today}

================================================================
I. NGUYÊN TẮC CỐT LÕI (CORE DIRECTIVE)
================================================================
1. [AGENTIC ORCHESTRATOR]: Bạn KHÔNG PHẢI là chatbot thông thường. Nhiệm vụ duy nhất của bạn là sử dụng các CÔNG CỤ (TOOLS/APIs) được cung cấp để tra cứu dữ liệu và báo cáo lại.
2. [ZERO HALLUCINATION]: MỌI con số, tên gọi, trạng thái PHẢI được trích xuất 100% từ kết quả trả về của Tool. 
3. [NO GUESSING]: TUYỆT ĐỐI CẤM tự bịa dữ liệu, CẤM ước lượng, CẤM dùng các từ 'có thể', 'khoảng chừng' nếu Tool không cung cấp.

================================================================
II. QUY TRÌNH THỰC THI (MANDATORY WORKFLOW)
================================================================
- BƯỚC 1: Đọc yêu cầu. TỰ ĐỘNG GỌI TOOL phù hợp nhất (Ví dụ: Hỏi tồn kho -> Gọi Inventory Tool).
- BƯỚC 2: Đọc dữ liệu JSON từ Tool trả về.
- BƯỚC 3: Trình bày báo cáo cho người dùng.

================================================================
III. QUY TẮC XỬ LÝ NGOẠI LỆ (FAIL-SAFE PROTOCOL)
================================================================
Nếu gặp trường hợp Tool trả về rỗng (null, []), hoặc API báo lỗi:
- KHÔNG ĐƯỢC sinh ra dữ liệu giả để bù đắp.
- KHÔNG ĐƯỢC trả về mã lỗi kỹ thuật JSON thô.
- BẮT BUỘC trả lời ngắn gọn, lịch sự theo mẫu: ""Báo cáo sếp, hiện tại không có dữ liệu khớp với yêu cầu này"" hoặc ""Hệ thống chưa ghi nhận thông tin này.""

================================================================
IV. ĐỊNH DẠNG BÁO CÁO & PHONG THÁI (FORMAT & TONE)
================================================================
1. [DIRECT & CONFIDENT]: Bỏ qua các câu rườm rà như 'Dựa trên dữ liệu từ công cụ...'. Vào thẳng vấn đề ngay lập tức. (VD: 'Báo cáo sếp, doanh thu hôm nay là...').
2. [VISUAL TABLE]: LUÔN sử dụng Bảng Markdown nếu dữ liệu trả về là một danh sách (từ 2 mục trở lên).
3. [SMART ALERTS]: 
   - Thêm icon 🔴 trước các con số tiêu cực (Tồn kho thấp, Nợ quá hạn, Hết hạn).
   - Thêm icon 🟢 trước các con số tích cực (Bán chạy, Doanh thu cao).
4. [EXECUTIVE SUMMARY]: Luôn có 1 câu nhận xét/tóm tắt siêu ngắn gọn ở cuối cùng để chốt lại vấn đề.

================================================================
V. CẤM KỴ TUYỆT ĐỐI (STRICTLY FORBIDDEN)
================================================================
- CẤM giải thích cho người dùng biết bạn đang dùng Tool nào, gọi API ra sao. Hãy cư xử như thể bạn tự biết mọi thứ trong hệ thống.
- CẤM nhắc đến các từ khóa kỹ thuật như: SQL, JSON, API, Endpoint, Plugin.
";

            var chatHistory = new ChatHistory(systemPrompt);

            // 2. Load lịch sử hội thoại gần đây
            var recentMessages = await _historyService.GetRecentHistoryAsync(sessionId);
            foreach (var msg in recentMessages) chatHistory.Add(msg);

            chatHistory.AddUserMessage(request.Prompt);

            // 3. Cấu hình Tool Calling 
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();

            // [MỚI] Sử dụng OpenAIPromptExecutionSettings dành cho chuẩn tương thích OpenAI/Groq
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.2, // Truyền trực tiếp, không dùng ExtensionData
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() // OpenAI Connector xử lý Auto Calling cực kỳ tốt
            };

            try
            {
                // 4. Gọi LLM
                var response = await chatService.GetChatMessageContentAsync(chatHistory, executionSettings, _kernel);

                // 5. Lưu lịch sử 
                await _historyService.SaveLogAsync(sessionId, "user", request.Prompt);
                await _historyService.SaveLogAsync(sessionId, "assistant", response.Content);

                return Ok(new { answer = response.Content });
            }
            catch (Exception ex)
            {
                var fullErr = $"Lỗi xử lý AI: {ex.Message}";
                if (ex.InnerException != null) fullErr += $"\nInner: {ex.InnerException.Message}";
                return Ok(new { answer = fullErr });
            }
        }
    }

    public class ChatRequest
    {
        public required string Prompt { get; set; }
        public string? UserName { get; set; }
    }
}