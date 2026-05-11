using AIAssistantService.Plugins;
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
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatController(Kernel kernel,
            IAiHistoryService historyService,
            IHttpClientFactory httpClientFactory)
        {
            _kernel = kernel;
            _historyService = historyService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ") || authHeader.StartsWith("Bearer null"))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn." });
            }

            string sessionId = request.UserName ?? "Unauthorized";
            string today = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm");

            // 1. Hệ thống Prompt cho AI Orchestrator (Zero Hallucination)
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

            // Gắn câu hỏi của người dùng
            chatHistory.AddUserMessage(request.Prompt);

            var chatService = _kernel.GetRequiredService<IChatCompletionService>();

            // Cấu hình Semantic Kernel để Auto Invoke Tool
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                Temperature = 0.2,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            string finalResponseText = "";

            try
            {
                // Gọi API Semantic Kernel. Nếu cần gọi tool, Kernel sẽ tự động gọi EMarketApiPlugin.
                var aiResponse = await chatService.GetChatMessageContentAsync(chatHistory, executionSettings, _kernel);
                finalResponseText = aiResponse.Content ?? "Tôi không thể xử lý yêu cầu này vào lúc này.";
            }
            catch (Exception ex)
            {
                finalResponseText = $"Xin lỗi, có lỗi trong quá trình phân tích: {ex.Message}";
                Console.WriteLine($"[ERROR]: {ex.Message}");
            }

            // Lưu log
            if (!string.IsNullOrEmpty(finalResponseText))
            {
                await _historyService.SaveLogAsync(sessionId, "user", request.Prompt);
                await _historyService.SaveLogAsync(sessionId, "assistant", finalResponseText);
            }

            return Ok(new { answer = finalResponseText });
        }
    }

    public class ChatRequest
    {
        public required string Prompt { get; set; }
        public string? UserName { get; set; }
    }
}