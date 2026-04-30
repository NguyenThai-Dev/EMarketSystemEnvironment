using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.InventoryModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [RoutePrefix("api/admin/inventory")]
    public class InventoryAdminApiController : ApiController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPurchaseService _purchaseService;
        private readonly IStockMovementService _movementService;
        private readonly ISupplierServiceDebtAndPaymentService _debtService;
        private readonly IWarehouseService _warehouseService;

        public InventoryAdminApiController(
            IInventoryService inventoryService, IPurchaseService purchaseService,
            IStockMovementService movementService, ISupplierServiceDebtAndPaymentService debtService,
            IWarehouseService warehouseService)
        {
            _inventoryService = inventoryService;
            _purchaseService = purchaseService;
            _movementService = movementService;
            _debtService = debtService;
            _warehouseService = warehouseService;
        }

        #region Inventory (Stock)

        [HttpGet, Route("stock/all")]
        public async Task<IHttpActionResult> GetAllInventory()
        { return Ok(await _inventoryService.GetAllInventoryAsync()); }

        [HttpGet, Route("stock/filter")]
        public async Task<IHttpActionResult> GetFilteredInventory(int? productId = null, int? warehouseId = null)
        { return Ok(await _inventoryService.GetFilteredInventoryAsync(productId, warehouseId)); }

        [HttpGet, Route("stock/by-branch")]
        public async Task<IHttpActionResult> GetInventoryByBranch(int? branchId = null)
        { return Ok(await _inventoryService.GetAllAsync(branchId)); }

        [HttpGet, Route("stock/{id:int}")]
        public async Task<IHttpActionResult> GetInventoryById(int id)
        {
            var d = await _inventoryService.GetInventoryByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("stock/by-product-ids")]
        public async Task<IHttpActionResult> GetInventoryByProductIds([FromBody] List<int> productIds, int? warehouseId = null, int? branchId = null)
        {
            if (productIds == null || productIds.Count == 0) return BadRequest("productIds required.");
            return Ok(await _inventoryService.GetInventoryByProductIdsAsync(productIds, warehouseId, branchId));
        }

        #endregion

        #region Stock Movements

        [HttpGet, Route("stock/movements")]
        public async Task<IHttpActionResult> GetStockMovements(int start = 0, int length = 10, int? warehouseId = null, string type = null, DateTime? fromDate = null, DateTime? toDate = null, string keyword = null)
        {
            var r = await _movementService.GetStockMovementsDataTableAsync(start, length, warehouseId, type, fromDate, toDate, keyword);
            return Ok(new { r.total, r.filtered, r.data });
        }

        [HttpGet, Route("stock/total-actual")]
        public async Task<IHttpActionResult> GetTotalStock(int productId, int warehouseId)
        { return Ok(await _movementService.GetTotalStockAsync(productId, warehouseId)); }

        #endregion

        #region Purchase Orders

        [HttpGet, Route("purchase/all")]
        public async Task<IHttpActionResult> GetAllPurchases()
        { return Ok(await _purchaseService.GetAllPurchaseAsync()); }

        [HttpGet, Route("purchase/search")]
        public async Task<IHttpActionResult> SearchPurchases(string keyword = null, int? supplierId = null, int? branchId = null, int? warehouseId = null, string status = null, string paymentStatus = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _purchaseService.GetFilteredPurchasesAsync(keyword, supplierId, branchId, warehouseId, status, paymentStatus, fromDate, toDate)); }

        [HttpGet, Route("purchase/{id:int}")]
        public async Task<IHttpActionResult> GetPurchaseById(int id)
        {
            var d = await _purchaseService.GetPurchaseByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpGet, Route("purchase/by-branch")]
        public async Task<IHttpActionResult> GetPurchasesByBranch(int? branchId = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _purchaseService.GetPurchaseByBranchIdAsync(branchId, fromDate, toDate)); }

        #endregion

        #region Supplier Debt & Payment

        [HttpGet, Route("debt/all")]
        public async Task<IHttpActionResult> GetAllDebts()
        { return Ok(await _debtService.GetAllSupplierDebtsAsync()); }

        [HttpGet, Route("debt/list")]
        public async Task<IHttpActionResult> GetFilteredDebts(string keyword = null, int? supplierId = null, string status = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _debtService.GetAllSupplierDebtsAsync(keyword, supplierId, status, fromDate, toDate)); }

        [HttpGet, Route("debt/{id:int}")]
        public async Task<IHttpActionResult> GetDebtById(int id)
        {
            var d = await _debtService.GetSupplierDebtByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpGet, Route("debt/by-purchase/{purchaseOrderId:int}")]
        public async Task<IHttpActionResult> GetDebtByPurchaseOrder(int purchaseOrderId)
        {
            var d = await _debtService.GetSupplierDebtByPurchaseOrderIdAsync(purchaseOrderId);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("debt/by-ids")]
        public async Task<IHttpActionResult> GetDebtsByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("ids required.");
            return Ok(await _debtService.GetSupplierDebtsByIdsAsync(ids));
        }

        [HttpGet, Route("debt/overdue")]
        public async Task<IHttpActionResult> GetOverdueDebts()
        { return Ok(await _debtService.GetOverdueDebtsAsync()); }

        [HttpGet, Route("debt/near-due")]
        public async Task<IHttpActionResult> GetNearDueDebts(int days = 7)
        { return Ok(await _debtService.GetDebtsNearDueDateAsync(days)); }

        [HttpGet, Route("debt/{debtId:int}/payments")]
        public async Task<IHttpActionResult> GetPaymentsByDebt(int debtId)
        { return Ok(await _debtService.GetPaymentsByDebtIdAsync(debtId)); }

        [HttpGet, Route("debt/payment-mail-info/{paymentId:int}")]
        public async Task<IHttpActionResult> GetPaymentMailInfo(int paymentId)
        {
            var d = await _debtService.GetPaymentMailInfoAsync(paymentId);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("debt/internal-detail")]
        public async Task<IHttpActionResult> GetInternalDebtDetail([FromBody] List<int> debtIds)
        {
            if (debtIds == null || debtIds.Count == 0) return BadRequest("debtIds required.");
            return Ok(await _debtService.GetInternalDebtDetailAsync(debtIds));
        }

        #endregion

        #region Warehouses

        [HttpGet, Route("warehouses/all")]
        public async Task<IHttpActionResult> GetAllWarehouses()
        { return Ok(await _warehouseService.GetAllWarehousesByBranchIdAsync()); }

        [HttpGet, Route("warehouses")]
        public async Task<IHttpActionResult> GetWarehousesByBranch(int? branchId = null)
        { return Ok(await _warehouseService.GetAllWarehouseByBranchId(branchId)); }

        [HttpGet, Route("warehouses/search")]
        public async Task<IHttpActionResult> SearchWarehouses(string name = null, int? branchId = null)
        { return Ok(await _warehouseService.GetFilteredWarehouseAsync(name, branchId)); }

        [HttpGet, Route("warehouses/{id:int}")]
        public async Task<IHttpActionResult> GetWarehouseDetail(int id)
        {
            var d = await _warehouseService.GetWarehouseByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpGet, Route("warehouses/dict")]
        public async Task<IHttpActionResult> GetWarehouseDict()
        { return Ok(await _warehouseService.GetWarehouseDictAsync()); }

        [HttpPost, Route("warehouses/by-ids")]
        public async Task<IHttpActionResult> GetWarehousesByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("ids required.");
            return Ok(await _warehouseService.GetWarehouseByIdsAsync(ids));
        }

        #endregion
    }
}