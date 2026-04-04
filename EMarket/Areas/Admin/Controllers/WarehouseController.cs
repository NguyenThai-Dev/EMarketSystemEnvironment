using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IStockMovementService _stockMovementService;
        private readonly IUserContext _userContext;

        public WarehouseController(IWarehouseService warehouseService, IStockMovementService stockMovementService, IUserContext userContext)
        {
            _warehouseService = warehouseService;
            _stockMovementService = stockMovementService;
            _userContext = userContext;
        }

        [EMarketAuthorize(Module = "InventoryModule")]
        public ActionResult WarehouseList()
        {
            return View();
        }

        [EMarketAuthorize(Module = "InventoryModule")]
        public ActionResult StockHistory()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAllWarehouses()
        {
            var data = await _warehouseService.GetAllWarehousesByBranchIdAsync();
            return Json(new { success = true, warehouses = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetWarehouse(int id)
        {
            var data = await _warehouseService.GetWarehouseByIdAsync(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetFilteredWarehouse(string warehouseName, int? branchId)
        {
            var data = await _warehouseService.GetFilteredWarehouseAsync(warehouseName, branchId);
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [EMarketAuthorize(Module = "InventoryModule")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateWarehouse(WarehouseDTO dto)
        {
            var newId = await _warehouseService.CreateWarehouseAsync(dto);
            return Json(new { success = true, id = newId });
        }

        [HttpPost]
        [EMarketAuthorize(Module = "InventoryModule")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateWarehouse(WarehouseDTO dto)
        {
            var result = await _warehouseService.UpdateWarehouseAsync(dto);
            return Json(new { success = result });
        }

        [HttpPost]
        [EMarketAuthorize(Module = "InventoryModule")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteWarehouse(int id)
        {
            var result = await _warehouseService.DeleteWarehouseAsync(id);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetStockMovements(
             int draw, int start, int length,
             int? WarehouseId, string Type,
             DateTime? FromDate, DateTime? ToDate,
             string Keyword)
        {
            try
            {
                var (total, filtered, data) = await _stockMovementService.GetStockMovementsDataTableAsync(
                    start, length, WarehouseId, Type, FromDate, ToDate, Keyword);

                return Json(new
                {
                    draw = draw,
                    recordsTotal = total,
                    recordsFiltered = filtered,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log error here
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetStockQuantity(int productId, int warehouseId)
        {
            var qty = await _stockMovementService.GetTotalStockAsync(productId, warehouseId);
            return Json(qty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AdjustStock(StockAdjustmentDTO model)
        {
            try
            {
                // Lấy User ID từ session/cookie
                model.UserId = _userContext.UserId;

                var result = await _stockMovementService.AdjustStockAsync(model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}