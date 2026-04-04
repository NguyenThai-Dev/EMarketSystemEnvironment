using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIAssistantService.Plugins
{
    public class ForecastPlugin
    {
        private readonly IHttpClientFactory _factory;
        public ForecastPlugin(IHttpClientFactory f) => _factory = f;

        // ------------------------------------------------------------------------------------------------
        // 1. ACTION: HUẤN LUYỆN LẠI (NẶNG - HẠN CHẾ GỌI)
        // ------------------------------------------------------------------------------------------------
        [KernelFunction]
        [Description("CẢNH BÁO: Hàm này kích hoạt quy trình Training Python rất nặng. CHỈ GỌI khi người dùng yêu cầu rõ ràng: 'Hãy học lại dữ liệu mới', 'Train lại model'. TUYỆT ĐỐI KHÔNG GỌI nếu chỉ muốn xem kết quả dự báo.")]
        public async Task<string> RunAIForecast()
        {
            var client = _factory.CreateClient("EMarketClient");

            // Chạy ngầm hoàn toàn và tự bắt lỗi nếu có
            _ = Task.Run(async () =>
            {
                try
                {
                    // Vẫn dùng await bên trong Task.Run để đảm bảo request gửi đi thành công
                    await client.PostAsync("api/admin/ai-analysis/run-prophet", null);
                    Console.WriteLine("[SYSTEM]: Huấn luyện Prophet hoàn tất.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SYSTEM ERROR]: Kích hoạt Prophet thất bại: {ex.Message}");
                }
            });

            return "Lệnh huấn luyện đã được gửi đi thành công. Hệ thống sẽ xử lý ngầm, sếp cứ thong thả làm việc khác ạ.";
        }

        // ------------------------------------------------------------------------------------------------
        // 2. QUERY: LẤY KẾ HOẠCH NHẬP HÀNG (QUAN TRỌNG NHẤT)
        // ------------------------------------------------------------------------------------------------
        [KernelFunction]
        [Description("Lấy bảng tư vấn nhập hàng chi tiết (Replenishment Advice). " +
                     "Dữ liệu trả về bao gồm: Tồn kho hiện tại (CurrentStock), Nhu cầu dự báo (ExpectedDemand), " +
                     "Số lượng Đề xuất nhập (SuggestedQty), và LÝ DO (Reason - ví dụ: 'Cao điểm Mùa vụ/Tết'). " +
                     "Luôn dùng hàm này để lập bảng kế hoạch nhập hàng.")]
        public async Task<string> GetReplenishmentAdvice([Description("ID của chi nhánh (Mặc định là 1)")] int branchId = 1)
        {
            var client = _factory.CreateClient("EMarketClient");
            return await client.GetStringAsync($"api/admin/ai-analysis/replenishment-advice/{branchId}");
        }

        // ------------------------------------------------------------------------------------------------
        // 3. QUERY: LẤY XU HƯỚNG LỊCH SỬ (DÙNG ĐỂ VẼ BIỂU ĐỒ)
        // ------------------------------------------------------------------------------------------------
        [KernelFunction]
        [Description("Lấy dữ liệu chuỗi thời gian (Time-Series) lịch sử bán hàng theo ngày của MỘT sản phẩm cụ thể. " +
                     "Trả về danh sách dạng {Date, Qty}. " +
                     "Dùng hàm này khi cần phân tích sâu biến động, tìm ngày bán chạy nhất, hoặc vẽ biểu đồ xu hướng.")]
        public async Task<string> GetProductTrend(
            [Description("ID sản phẩm cần soi")] int productId,
            [Description("ID chi nhánh")] int branchId,
            [Description("Ngày bắt đầu (yyyy-MM-dd)")] string start,
            [Description("Ngày kết thúc (yyyy-MM-dd)")] string end)
        {
            var client = _factory.CreateClient("EMarketClient");
            string url = $"api/admin/ai-analysis/product-history/{productId}/{branchId}?start={start}&end={end}";
            return await client.GetStringAsync(url);
        }

        // ------------------------------------------------------------------------------------------------
        // 4. COMPOSITE: TỔNG HỢP (DÙNG CHO BÁO CÁO NHANH)
        // ------------------------------------------------------------------------------------------------
        [KernelFunction]
        [Description("Hàm tổng hợp nhanh: Lấy cùng lúc Dự báo nhập hàng (Forecast) và Danh sách cảnh báo tồn kho thấp (LowStock). " +
                     "Dùng hàm này khi người dùng hỏi chung chung như: 'Tình hình hàng hóa thế nào?', 'Có cần nhập hàng không?'.")]
        public async Task<string> GetIntegratedAnalysis(int branchId)
        {
            var client = _factory.CreateClient("EMarketClient");

            // Gọi song song 2 API để tiết kiệm thời gian chờ
            var forecastTask = client.GetStringAsync($"api/admin/ai-analysis/replenishment-advice/{branchId}");
            var lowStockTask = client.GetStringAsync("api/admin/product-management/products/low-stock-alerts");

            await Task.WhenAll(forecastTask, lowStockTask);

            // Gói gọn vào 1 JSON để AI đọc 1 lần
            return $"{{ \"Forecast_Advice\": {forecastTask.Result}, \"Actual_Low_Stock_Alerts\": {lowStockTask.Result} }}";
        }
    }
}