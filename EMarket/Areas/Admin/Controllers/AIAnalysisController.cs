using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Forecast.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class AIAnalysisController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IUserContext _userContext;

        // Constructor Injection (Đảm bảo bạn đã cấu hình Unity/Autofac)
        public AIAnalysisController(IAIService aiService, IUserContext userContext)
        {
            _aiService = aiService;
            _userContext = userContext;
        }

        // GET: Admin/AIAnalysis
        public ActionResult Index()
        {
            return View();
        }

        // 1. TRIGGER PHÂN TÍCH (SYSTEM vs AI)
        [HttpPost]
        public async Task<JsonResult> RunAnalysis(string mode, int branchId)
        {
            bool success = false;
            string message = "";

            try
            {
                if (mode == "system")
                {
                    success = await _aiService.RunAnalysisAsync(branchId);
                    message = "Hệ thống (Rule-based) đã cập nhật báo cáo!";
                }
                else
                {
                    success = await _aiService.RunAIPipelineAsync();
                    message = "AI XGBOOST đã hoàn tất training và dự báo!";
                }
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // 2. GET DATA: SYSTEM MODE (Thêm Insights)
        [HttpGet]
        public async Task<JsonResult> GetSystemData(int branchId)
        {

            // Gọi song song các task để tối ưu tốc độ
            var taskRec = _aiService.GetRecommendationsAsync(branchId);
            var taskAno = _aiService.GetAnomaliesAsync(branchId);
            var taskIns = _aiService.GetProductInsightsAsync(branchId);

            await Task.WhenAll(taskRec, taskAno, taskIns);

            return Json(new
            {
                success = true,
                type = "system",
                data = new
                {
                    Recommendations = taskRec.Result,
                    Anomalies = taskAno.Result,
                    Insights = taskIns.Result // Trả về thêm Insights
                }
            }, JsonRequestBehavior.AllowGet);
        }

        // 3. GET DATA: AI MODE
        [HttpGet]
        public async Task<JsonResult> GetAIData(int branchId)
        {
            var data = await _aiService.GetReplenishmentAdviceAsync(branchId);

            return Json(new
            {
                success = true,
                type = "ai",
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetDeadstockAnalysis(int branchId)
        {
            var data = await _aiService.GetDeadstockAnalysisAsync(branchId);

            return Json(new
            {
                success = true,
                type = "ai",
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesForecast(int productId, int branchId)
        {
            var data = await _aiService.GetSalesForecastAsync(productId, branchId);
            return Json(new
            {
                success = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetFullChartData(int productId, int branchId)
        {
            var history = await _aiService.GetProductHistoryAsync(productId, branchId, DateTime.Now.AddDays(-30), DateTime.Now);
            var forecast = await _aiService.GetSalesForecastAsync(productId, branchId);

            return Json(new
            {
                success = true,
                history = history,
                forecast = forecast
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetTopForecast(int branchId = 1)
        {
            try
            {
                var topProducts = await _aiService.GetTopPredictedProductsAsync(branchId, 50);

                return Json(new
                {
                    success = true,
                    data = topProducts,
                    generatedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetInventoryForecast(int branchId)
        {
            var data = await _aiService.GetInventoryForecastAsync(branchId);

            return Json(new
            {
                success = true,
                type = "ai",
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        // 4. GET HISTORY: Dành cho ApexCharts
        [HttpGet]
        public async Task<JsonResult> GetProductHistory(int productId, int branchId, string startDate, string endDate)
        {
            if (!DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate) ||
                !DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate))
            {
                return Json(new { success = false, message = "Ngày không hợp lệ" }, JsonRequestBehavior.AllowGet);
            }

            var data = await _aiService.GetProductHistoryAsync(productId, branchId, fromDate, toDate);

            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }
    }
}