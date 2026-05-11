using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Forecast.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// Read-only API for AI Forecast & Analytics data.
    /// Exposes all prediction, recommendation, anomaly, insight, and risk analysis results.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/admin/ai-analysis")]
    public class AIAnalysisApiController : ApiController
    {
        private readonly IAIService _aiService;

        public AIAnalysisApiController(IAIService aiService)
        {
            _aiService = aiService;
        }

        // ============================================================
        #region Replenishment & Recommendations
        // ============================================================

        /// <summary>
        /// Lấy danh sách gợi ý nhập hàng chi tiết do AI tính toán (Replenishment Advice).
        /// Bao gồm: Tồn kho hiện tại, Nhu cầu dự báo, Số lượng đề xuất nhập, Lý do.
        /// </summary>
        [HttpGet]
        [Route("replenishment-advice/{branchId:int}")]
        public async Task<IHttpActionResult> GetReplenishmentAdvice(int branchId)
        {
            var data = await _aiService.GetReplenishmentAdviceAsync(branchId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách gợi ý nhập hàng dạng Recommendation (Join tên sản phẩm + danh mục).
        /// </summary>
        [HttpGet]
        [Route("recommendations/{branchId:int}")]
        public async Task<IHttpActionResult> GetRecommendations(int branchId)
        {
            var data = await _aiService.GetRecommendationsAsync(branchId);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Anomaly Detection
        // ============================================================

        /// <summary>
        /// Lấy danh sách bất thường ngành hàng (Spike/Dip) do AI phát hiện.
        /// </summary>
        [HttpGet]
        [Route("anomalies/{branchId:int}")]
        public async Task<IHttpActionResult> GetAnomalies(int branchId)
        {
            var data = await _aiService.GetAnomaliesAsync(branchId);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Product Insights
        // ============================================================

        /// <summary>
        /// Lấy insight chi tiết sản phẩm: Star, Trending, Warning.
        /// </summary>
        [HttpGet]
        [Route("insights/{branchId:int}")]
        public async Task<IHttpActionResult> GetProductInsights(int branchId)
        {
            var data = await _aiService.GetProductInsightsAsync(branchId);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Sales & Inventory Forecast
        // ============================================================

        /// <summary>
        /// Lấy dữ liệu dự báo tồn kho (Inventory Forecast) cho toàn bộ sản phẩm tại chi nhánh.
        /// </summary>
        [HttpGet]
        [Route("inventory-forecast/{branchId:int}")]
        public async Task<IHttpActionResult> GetInventoryForecast(int branchId)
        {
            var data = await _aiService.GetInventoryForecastAsync(branchId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy dữ liệu dự báo bán hàng (Sales Forecast) theo chuỗi thời gian cho một sản phẩm cụ thể.
        /// </summary>
        [HttpGet]
        [Route("sales-forecast/{productId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetSalesForecast(int productId, int branchId)
        {
            var data = await _aiService.GetSalesForecastAsync(productId, branchId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy Top N sản phẩm có sản lượng dự báo cao nhất tại chi nhánh.
        /// </summary>
        [HttpGet]
        [Route("top-predicted/{branchId:int}")]
        public async Task<IHttpActionResult> GetTopPredictedProducts(int branchId, int topCount = 10)
        {
            var data = await _aiService.GetTopPredictedProductsAsync(branchId, topCount);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Deadstock Analysis
        // ============================================================

        /// <summary>
        /// Lấy danh sách hàng tồn kho chậm luân chuyển (Deadstock) có nguy cơ hết hạn hoặc ứ đọng.
        /// </summary>
        [HttpGet]
        [Route("deadstock/{branchId:int}")]
        public async Task<IHttpActionResult> GetDeadstockAnalysis(int branchId)
        {
            var data = await _aiService.GetDeadstockAnalysisAsync(branchId);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Product History (Time-Series)
        // ============================================================

        /// <summary>
        /// Lấy chuỗi thời gian lịch sử bán hàng theo ngày cho một sản phẩm cụ thể.
        /// Trả về danh sách {Date, Qty} dùng để vẽ biểu đồ xu hướng.
        /// </summary>
        [HttpGet]
        [Route("product-history/{productId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetProductHistory(int productId, int branchId, string start, string end)
        {
            DateTime fromDate = DateTime.Parse(start);
            DateTime toDate = DateTime.Parse(end);
            var data = await _aiService.GetProductHistoryAsync(productId, branchId, fromDate, toDate);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Financial Risk (FEFO Lot-Level)
        // ============================================================

        /// <summary>
        /// Lấy phân tích rủi ro tài chính theo lô hàng (FEFO - First Expired First Out).
        /// Bao gồm: Tổng giá trị dự phòng, Số lô nguy hiểm, Chi tiết từng lô.
        /// </summary>
        [HttpGet]
        [Route("lot-financial-risk/{branchId:int}")]
        public async Task<IHttpActionResult> GetLotFinancialRisk(int branchId)
        {
            var data = await _aiService.GetLotFinancialRiskAsync(branchId);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Pipeline Trigger (Action - POST)
        // ============================================================

        /// <summary>
        /// Kích hoạt quy trình huấn luyện AI Python Pipeline. CHỈ dùng khi có yêu cầu rõ ràng.
        /// </summary>
        [HttpPost]
        [Route("run-prophet")]
        public async Task<IHttpActionResult> RunProphet()
        {
            try
            {
                var success = await _aiService.RunAIPipelineAsync();
                return Ok(new { success, message = "AI Prophet Pipeline triggered" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
