using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.DashboardModule.Servcie.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [RoutePrefix("api/admin/dashboard")]
    public class DashboardAdminApiController : ApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardAdminApiController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ============================================================
        #region Summary
        // ============================================================

        /// <summary>
        /// Lấy tổng quan dashboard: doanh thu, khách hàng, sản phẩm, tồn kho, ...
        /// </summary>
        [HttpGet]
        [Route("summary")]
        public async Task<IHttpActionResult> GetSummary(int? branchId = null)
        {
            var data = await _dashboardService.GetSummaryAsync(branchId);
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region Branch Performance
        // ============================================================

        /// <summary>
        /// Lấy hiệu suất chi nhánh theo khoảng thời gian.
        /// </summary>
        [HttpGet]
        [Route("branch-performance")]
        public async Task<IHttpActionResult> GetBranchPerformance(
            int? branchId,
            DateTime fromDate,
            DateTime toDate)
        {
            var data = await _dashboardService.GetBranchPerformanceAsync(branchId, fromDate, toDate);
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region Stock Chart
        // ============================================================

        /// <summary>
        /// Lấy dữ liệu biểu đồ tồn kho.
        /// </summary>
        [HttpGet]
        [Route("stock-chart")]
        public async Task<IHttpActionResult> GetStockChart(int? branchId)
        {
            var data = await _dashboardService.GetStockChartAsync(branchId);
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region Overview
        // ============================================================

        /// <summary>
        /// Lấy dữ liệu tổng quan theo ngày hoặc tháng.
        /// </summary>
        /// <param name="branchId">ID chi nhánh.</param>
        /// <param name="fromDate">Ngày bắt đầu.</param>
        /// <param name="toDate">Ngày kết thúc.</param>
        /// <param name="groupBy">"day" hoặc "month".</param>
        [HttpGet]
        [Route("overview")]
        public async Task<IHttpActionResult> GetOverview(
            int? branchId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string groupBy = "day")
        {
            var f = fromDate ?? DateTime.Today.AddDays(-7);
            var t = toDate ?? DateTime.Today;

            var data = await _dashboardService.GetOverviewAsync(branchId, f, t, groupBy);
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region People Dashboard
        // ============================================================

        /// <summary>
        /// Lấy dashboard nhân sự.
        /// </summary>
        [HttpGet]
        [Route("people")]
        public async Task<IHttpActionResult> GetPeopleDashboard()
        {
            var data = await _dashboardService.GetPeopleDashboardAsync();
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region Warehouse Dashboard
        // ============================================================

        /// <summary>
        /// Lấy dashboard kho (nhập - xuất - tồn).
        /// </summary>
        [HttpGet]
        [Route("warehouse")]
        public async Task<IHttpActionResult> GetWarehouseDashboard(
            int dayBacks,
            int? branchId = null,
            int? warehouseId = null)
        {
            var data = await _dashboardService.GetWarehouseDashboardAsync(dayBacks, branchId, warehouseId);
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region Finance Dashboard
        // ============================================================

        /// <summary>
        /// Lấy dashboard tài chính: doanh thu, lợi nhuận, dòng tiền.
        /// </summary>
        [HttpGet]
        [Route("finance")]
        public async Task<IHttpActionResult> GetFinanceDashboard(
            int daysBack,
            int? branchId = null)
        {
            var data = await _dashboardService.GetFinanceDashboardAsync(daysBack, branchId);
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region Debt / Payables Dashboard
        // ============================================================

        /// <summary>
        /// Lấy dashboard công nợ nhà cung cấp.
        /// </summary>
        [HttpGet]
        [Route("debt")]
        public async Task<IHttpActionResult> GetDebtDashboard(
            int? branchId = null,
            int? supplierId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var data = await _dashboardService.GetDebtDashboardAsync(branchId, supplierId, fromDate, toDate);
            return Ok(data);
        }

        #endregion


        // ============================================================
        #region Super Admin Hub
        // ============================================================

        /// <summary>
        /// Lấy dữ liệu tổng hợp dành cho Super Admin.
        /// </summary>
        [HttpGet]
        [Route("super-admin")]
        public async Task<IHttpActionResult> GetSuperAdminHub()
        {
            var data = await _dashboardService.GetSuperAdminHubData();
            return Ok(data);
        }

        #endregion
    }
}
