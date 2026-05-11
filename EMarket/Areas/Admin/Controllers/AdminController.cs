using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.DashboardModule.Servcie.Interfaces;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IBranchService _branchService;
        private readonly IWarehouseService _warehouseService;
        private readonly ILoginService _loginService;
        private readonly ISupplierService _supplierService;
        private readonly IUserContext _userContext;

        public AdminController(IDashboardService dashboardService, IBranchService branchService, IWarehouseService warehouseService, ILoginService loginService, ISupplierService supplierService, IUserContext userContext)
        {
            _dashboardService = dashboardService;
            _branchService = branchService;
            _warehouseService = warehouseService;
            _loginService = loginService;
            _supplierService = supplierService;
            _userContext = userContext;
        }
        [EMarketAuthorize(Module = "DashboardModule")]

        public ActionResult Index()
        {
            if (!_userContext.IsAuthenticated)
                return RedirectToAction("Login", "Login");

            switch (_userContext.PrimaryRoleId)
            {
                case 1: // Admin
                    return View("ExecutiveDashboard");

                case 2: // HR
                    return View("PeopleOperationsDashboard");

                case 3: // Warehouse
                    return View("WarehouseManagementDashboard");

                case 4: // Sales (Bán hàng)
                    return View("FinanceManagementDashboard");

                case 5: // Debt (Công nợ)
                case 6: // Supplier (Nhà cung cấp)
                    return View("PayablesOverviewDashboard"); // Màn hình công nợ

                default:
                    return View("AccessDenied");
            }
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            Session.Abandon();
            Session.Clear();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
            Response.StatusCode = 403;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error_404()
        {
            Response.StatusCode = 404;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error_500()
        {
            Response.StatusCode = 500;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Server_Maintenance()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("Error_404");
        }

        #region Admin View
        [EMarketAuthorize(RequireAdmin = true)]
        public ActionResult AdminDashboard()
        {
            return View();
        }

        [EMarketAuthorize(RequireAdmin = true)]
        public ActionResult ExecutiveDashboard()
        {
            ViewBag.Title = "Super Admin Command Center";
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetSummary(int? branchId)
        {
            var data = await _dashboardService.GetSummaryAsync(branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetBranchPerformance(int? branchId, DateTime fromDate, DateTime toDate)
        {
            var data = await _dashboardService.GetBranchPerformanceAsync(branchId, fromDate, toDate);
            return Json(new
            {
                data,
                recordsTotal = data.Count,
                recordsFiltered = data.Count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetStockChart(int? branchId)
        {
            var data = await _dashboardService.GetStockChartAsync(branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Overview(
        int? branchId,
        DateTime? fromDate,
        DateTime? toDate,
        string groupBy = "day")
        {
            var from = fromDate ?? DateTime.Today.AddDays(-7);
            var to = toDate ?? DateTime.Today;

            var data = await _dashboardService.GetOverviewAsync(
                branchId, from, to, groupBy);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region People Operation Views
        [EMarketAuthorize(Module = "DashboardModule")]
        public ActionResult PeopleOperationsDashboard()
        {
            return View();
        }

        [HttpGet]
        [EMarketAuthorize(Module = "DashboardModule")]
        public async Task<JsonResult> GetPeopleDashboardData()
        {
            // Gọi Service tổng hợp dữ liệu
            var data = await _dashboardService.GetPeopleDashboardAsync();
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Warehouse Manage Views
        [EMarketAuthorize(Module = "DashboardModule")]
        public ActionResult WarehouseManagementDashboard()
        {
            return View();
        }


        [HttpGet]
        public async Task<JsonResult> LoadDropdowns(int? branchId)
        {
            var branches = await _branchService.GetAllBranchesAsync();
            var warehouses = await _warehouseService.GetAllWarehouseByBranchId(branchId);


            return Json(new
            {
                branches,
                warehouses
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [EMarketAuthorize(Module = "DashboardModule")]
        public async Task<JsonResult> GetWarehouseDashboard(int dayBacks, int? branchId, int? warehouseId)
        {
            var data = await _dashboardService.GetWarehouseDashboardAsync(dayBacks, branchId, warehouseId);

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Finance Views
        [EMarketAuthorize(Module = "DashboardModule")]
        public ActionResult FinanceManagementDashboard()
        {
            return View();
        }

        [HttpGet]
        [EMarketAuthorize(Module = "DashboardModule")]
        public async Task<JsonResult> GetFinanceDashboardData(int? branchId, int daysBack)
        {
            var data = await _dashboardService.GetFinanceDashboardAsync(daysBack, branchId);
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Payables Overview
        [EMarketAuthorize(Module = "DashboardModule")]
        public ActionResult PayablesOverviewDashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> LoadSuppliers()
        {
            var suppliers = await _supplierService.GetAllSupplierAsync();

            return Json(new
            {
                suppliers
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [EMarketAuthorize(Module = "DashboardModule")]
        public async Task<JsonResult> GetPayablesOverviewDashboardData(int? branchId,
   int? supplierId,
   DateTime? fromDate,
   DateTime? toDate)
        {
            var data = await _dashboardService.GetDebtDashboardAsync(branchId, supplierId, fromDate, toDate);
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        [EMarketAuthorize(Module = "DashboardModule")]
        public async Task<JsonResult> GetSuperAdminHubData()
        {
            var data = await _dashboardService.GetSuperAdminHubData();
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}