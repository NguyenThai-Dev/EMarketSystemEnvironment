using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ISupplierService _supplierService;
        private readonly IWarehouseService _warehouseService;
        private readonly IBranchService _branchService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;

        public PurchaseOrderController(
            IPurchaseService purchaseService,
            ISupplierService supplierService,
            IWarehouseService warehouseService,
            IBranchService branchService,
            IProductService productService,
            IProductCategoryService categoryService)
        {
            _purchaseService = purchaseService;
            _supplierService = supplierService;
            _warehouseService = warehouseService;
            _branchService = branchService;
            _productService = productService;
            _categoryService = categoryService;
        }

        [EMarketAuthorize(Module = "InventoryModule")]
        public async Task<ActionResult> PurchaseOrderList(int? id)
        {
            var suppliers = await _supplierService.GetAllSupplierAsync();
            var branches = await _branchService.GetAllBranchesAsync();

            ViewData["SupplierList"] = new SelectList(suppliers, "SupplierId", "Name");
            ViewData["BranchList"] = new SelectList(branches, "BranchId", "Name");

            // dùng để JS bắt và focus
            ViewBag.FocusPurchaseOrderId = id;

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetWarehousesByBranch(int? branchId)
        {
            // Tái sử dụng service hiện có
            var warehouses = await _warehouseService.GetAllWarehouseByBranchId(branchId);

            // Ánh xạ sang cấu trúc đơn giản nếu cần, hoặc trả về trực tiếp DTO
            var data = warehouses.Select(w => new { id = w.WarehouseId, name = w.Name }).ToList();

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        // [Thêm vào PurchaseOrderController hoặc một Controller chung]

        [HttpGet]
        public async Task<JsonResult> GetSuppliersForDropdown()
        {
            var suppliers = await _supplierService.GetAllSupplierAsync();
            var data = suppliers.Select(s => new { id = s.SupplierId, name = s.Name }).ToList();
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetBranchesForDropdown()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            var data = branches.Select(b => new { id = b.BranchId, name = b.Name }).ToList();
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }

        // ===================== GET FILTERED LIST =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetAllPurchaseOrder(string keyword, int? supplierId, int? branchId, int? warehouseId,
            string status, string paymentStatus, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _purchaseService.GetFilteredPurchasesAsync(
                keyword, supplierId, branchId, warehouseId, status, paymentStatus, fromDate, toDate
            );

            return Json(new { data });
        }

        [HttpGet]
        public async Task<ActionResult> GetPurchaseOrder(int id)
        {
            try
            {
                var result = id > 0
                    ? await _purchaseService.GetPurchaseByIdAsync(id)
                    : new PurchaseOrderDTO
                    {
                        OrderDate = DateTime.Now,
                        Details = new List<PurchaseOrderDetailDTO>()
                    };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "InventoryModule")]
        public async Task<ActionResult> SavePurchaseOrder(PurchaseOrderDTO model)
        {
            try
            {
                Debug.WriteLine("id: " + model.PurchaseOrderId);
                if (model.PurchaseOrderId > 0)
                {
                    await _purchaseService.UpdatePurchaseAsync(model);
                }

                else
                {
                    await _purchaseService.CreatePurchaseAsync(model);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "InventoryModule")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _purchaseService.DeletePurchaseAsync(id);
            return Json(new { success = ok });
        }

    }
}