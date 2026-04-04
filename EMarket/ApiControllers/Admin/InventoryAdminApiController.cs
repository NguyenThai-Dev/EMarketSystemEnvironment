using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;

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
        private readonly IProductLotService _productLotService;

        public InventoryAdminApiController(
            IInventoryService inventoryService,
            IPurchaseService purchaseService,
            IStockMovementService movementService,
            ISupplierServiceDebtAndPaymentService debtService,
            IProductLotService productLotService,
            IWarehouseService warehouseService)
        {
            _inventoryService = inventoryService;
            _purchaseService = purchaseService;
            _movementService = movementService;
            _debtService = debtService;
            _warehouseService = warehouseService;
            _productLotService = productLotService;
        }

        // ============================================================
        #region Stock & Inventory APIs
        // ============================================================

        /// <summary>
        /// Lấy danh sách tồn kho tổng thể.
        /// </summary>
        [HttpGet]
        [Route("stock/all")]
        public async Task<IHttpActionResult> GetAllInventory()
        {
            var data = await _inventoryService.GetAllInventoryAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy tồn kho theo sản phẩm và kho hàng.
        /// </summary>
        [HttpGet]
        [Route("stock/filter")]
        public async Task<IHttpActionResult> GetFilteredInventory(int? productId = null, int? warehouseId = null)
        {
            var data = await _inventoryService.GetFilteredInventoryAsync(productId, warehouseId);
            return Ok(data);
        }

        [HttpGet]
        [Route("stock/filterTime")]
        public async Task<IHttpActionResult> GetFilteredInventory(int productId, DateTime? manufacturingDate, DateTime? expiryDate)
        {
            var data = await _productLotService.FindExistingLotIdAsync(productId, manufacturingDate, expiryDate);
            return Ok(data);
        }

        /// <summary>
        /// Lấy tổng số lượng tồn kho thực tế của 1 sản phẩm tại 1 kho (Cộng dồn các lô).
        /// </summary>
        [HttpGet]
        [Route("stock/total-actual")]
        public async Task<IHttpActionResult> GetTotalStock(int productId, int warehouseId)
        {
            var total = await _movementService.GetTotalStockAsync(productId, warehouseId);
            return Ok(total);
        }

        /// <summary>
        /// Truy vấn biến động kho (Stock Movements) theo lịch sử.
        /// </summary>
        [HttpGet]
        [Route("stock/movements")]
        public async Task<IHttpActionResult> GetStockMovements(
            int start = 0, int length = 10, int? warehouseId = null,
            string type = null, DateTime? fromDate = null, DateTime? toDate = null, string keyword = null)
        {
            var data = await _movementService.GetStockMovementsDataTableAsync(start, length, warehouseId, type, fromDate, toDate, keyword);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Purchase Order APIs
        // ============================================================

        /// <summary>
        /// Tra cứu danh sách đơn nhập hàng với bộ lọc nâng cao.
        /// </summary>
        [HttpGet]
        [Route("purchase/search")]
        public async Task<IHttpActionResult> SearchPurchases(
            string keyword = null, int? supplierId = null, int? branchId = null,
            int? warehouseId = null, string status = null, string paymentStatus = null,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var data = await _purchaseService.GetFilteredPurchasesAsync(keyword, supplierId, branchId, warehouseId, status, paymentStatus, fromDate, toDate);
            return Ok(data);
        }

        /// <summary>
        /// Lấy chi tiết đơn nhập hàng theo ID.
        /// </summary>
        [HttpGet]
        [Route("purchase/{id:int}")]
        public async Task<IHttpActionResult> GetPurchaseById(int id)
        {
            var data = await _purchaseService.GetPurchaseByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Supplier Debt & Payment APIs
        // ============================================================

        /// <summary>
        /// Lấy danh sách công nợ nhà cung cấp.
        /// </summary>
        [HttpGet]
        [Route("debt/list")]
        public async Task<IHttpActionResult> GetSupplierDebts(
            string keyword = null, int? supplierId = null, string status = null,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var data = await _debtService.GetAllSupplierDebtsAsync(keyword, supplierId, status, fromDate, toDate);
            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách các khoản nợ quá hạn.
        /// </summary>
        [HttpGet]
        [Route("debt/overdue")]
        public async Task<IHttpActionResult> GetOverdueDebts()
        {
            var data = await _debtService.GetOverdueDebtsAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy các khoản nợ sắp đến hạn trong N ngày tới.
        /// </summary>
        [HttpGet]
        [Route("debt/near-due")]
        public async Task<IHttpActionResult> GetNearDueDebts(int days = 7)
        {
            var data = await _debtService.GetDebtsNearDueDateAsync(days);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Warehouse APIs
        // ============================================================

        /// <summary>
        /// Lấy danh sách kho hàng theo chi nhánh.
        /// </summary>
        [HttpGet]
        [Route("warehouses")]
        public async Task<IHttpActionResult> GetWarehouses(int? branchId = null)
        {
            var data = await _warehouseService.GetAllWarehouseByBranchId(branchId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy thông tin chi tiết một kho hàng.
        /// </summary>
        [HttpGet]
        [Route("warehouses/{id:int}")]
        public async Task<IHttpActionResult> GetWarehouseDetail(int id)
        {
            var data = await _warehouseService.GetWarehouseByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion
    }
}