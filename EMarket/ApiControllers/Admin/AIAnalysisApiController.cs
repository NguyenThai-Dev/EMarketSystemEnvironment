using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Forecast.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [RoutePrefix("api/admin/ai-analysis")]
    public class AIAnalysisApiController : ApiController
    {
        private readonly IAIService _aiService;

        public AIAnalysisApiController(IAIService aiService)
        {
            _aiService = aiService;
        }

        // AI gọi vào đây để kích hoạt chạy Prophet Python
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

        // AI gọi vào đây để lấy kết quả dự báo nhập hàng
        [HttpGet]
        [Route("replenishment-advice/{branchId}")]
        public async Task<IHttpActionResult> GetAdvice(int branchId)
        {
            var data = await _aiService.GetReplenishmentAdviceAsync(branchId);
            return Ok(data);
        }

        // Lấy lịch sử bán hàng để AI phân tích biểu đồ
        [HttpGet]
        [Route("product-history/{productId}/{branchId}")]
        public async Task<IHttpActionResult> GetHistory(int productId, int branchId, string start, string end)
        {
            DateTime fromDate = DateTime.Parse(start);
            DateTime toDate = DateTime.Parse(end);
            var data = await _aiService.GetProductHistoryAsync(productId, branchId, fromDate, toDate);
            return Ok(data);
        }
    }
}
