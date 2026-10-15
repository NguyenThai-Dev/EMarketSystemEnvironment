using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// REST Web API for Inventory Management: Stock Movements, Stock Adjustments, Purchases, Debts, and Warehouses.
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/admin/inventory")]
    public class InventoryAdminApiController : ApiController
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPurchaseService _purchaseService;
        private readonly IStockMovementService _stockMovementService;
        private readonly ISupplierServiceDebtAndPaymentService _debtService;
        private readonly IWarehouseService _warehouseService;

        public InventoryAdminApiController(
            IInventoryService inventoryService,
            IPurchaseService purchaseService,
            IStockMovementService stockMovementService,
            ISupplierServiceDebtAndPaymentService debtService,
            IWarehouseService warehouseService)
        {
            _inventoryService = inventoryService;
            _purchaseService = purchaseService;
            _stockMovementService = stockMovementService;
            _debtService = debtService;
            _warehouseService = warehouseService;
        }

        #region Stock Movements & Adjustments

        /// <summary>
        /// Lấy nhật ký biến động kho (Phân trang DataTable &amp; Mobile).
        /// </summary>
        [HttpGet, HttpPost]
        [Route("stock/movements")]
        [Route("stock-movements")]
        public async Task<IHttpActionResult> GetStockMovements(
            int start = 0,
            int length = 10,
            int? warehouseId = null,
            int? branchId = null,
            string type = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string keyword = null)
        {
            if (!warehouseId.HasValue && branchId.HasValue && branchId.Value > 0)
            {
                var whList = await _warehouseService.GetAllWarehouseByBranchId(branchId.Value);
                if (whList != null && whList.Any())
                {
                    warehouseId = whList.First().WarehouseId;
                }
            }

            var pageSize = Math.Min(length > 0 ? length : 10, 100);
            var res = await _stockMovementService.GetStockMovementsDataTableAsync(
                start, pageSize, warehouseId, type, fromDate, toDate, keyword);

            return Ok(new
            {
                draw = start / pageSize + 1,
                recordsTotal = res.total,
                recordsFiltered = res.filtered,
                total = res.total,
                filtered = res.filtered,
                data = res.data
            });
        }

        /// <summary>
        /// Lấy tổng tồn kho thực tế của 1 sản phẩm tại 1 kho.
        /// </summary>
        [HttpGet, Route("stock/total-actual")]
        public async Task<IHttpActionResult> GetTotalActualStock(int productId, int warehouseId)
        {
            var total = await _stockMovementService.GetTotalStockAsync(productId, warehouseId);
            return Ok(total);
        }

        /// <summary>
        /// Thực hiện điều chỉnh kho.
        /// </summary>
        [HttpPost, Route("stock/adjust")]
        public async Task<IHttpActionResult> AdjustStock([FromBody] StockAdjustmentDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu điều chỉnh kho không hợp lệ.");
            var success = await _stockMovementService.AdjustStockAsync(dto);
            return Ok(new { success });
        }

        #endregion

        #region Inventory Items

        /// <summary>
        /// Lấy danh sách tồn kho theo bộ lọc (productId, warehouseId, branchId).
        /// </summary>
        [HttpGet, Route("items")]
        public async Task<IHttpActionResult> GetInventoryItems(int? productId = null, int? warehouseId = null, int? branchId = null)
        {
            if (productId.HasValue || warehouseId.HasValue)
            {
                var filtered = await _inventoryService.GetFilteredInventoryAsync(productId, warehouseId);
                return Ok(filtered);
            }
            if (branchId.HasValue)
            {
                var branchItems = await _inventoryService.GetAllAsync(branchId);
                return Ok(branchItems);
            }
            var all = await _inventoryService.GetAllInventoryAsync();
            return Ok(all);
        }

        /// <summary>
        /// Lấy chi tiết bản ghi tồn kho theo ID.
        /// </summary>
        [HttpGet, Route("items/{id:int}")]
        public async Task<IHttpActionResult> GetInventoryById(int id)
        {
            var item = await _inventoryService.GetInventoryByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Tạo bản ghi tồn kho mới.
        /// </summary>
        [HttpPost, Route("items")]
        public async Task<IHttpActionResult> CreateInventory([FromBody] InventoryDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu tồn kho không hợp lệ.");
            var success = await _inventoryService.CreateInventoryAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Cập nhật bản ghi tồn kho.
        /// </summary>
        [HttpPut, Route("items")]
        [HttpPost, Route("items/update")]
        public async Task<IHttpActionResult> UpdateInventory([FromBody] InventoryDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu tồn kho không hợp lệ.");
            var success = await _inventoryService.UpdateInventoryAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Xóa bản ghi tồn kho.
        /// </summary>
        [HttpDelete, Route("items/{id:int}")]
        [HttpPost, Route("items/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteInventory(int id)
        {
            var success = await _inventoryService.DeleteInventoryAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Purchase Orders (Phiếu nhập hàng)

        /// <summary>
        /// Lấy tất cả đơn nhập hàng.
        /// </summary>
        [HttpGet, Route("purchase/all")]
        public async Task<IHttpActionResult> GetAllPurchases()
        {
            var result = await _purchaseService.GetAllPurchaseAsync();
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm đơn nhập hàng với phân trang và nhiều tiêu chí lọc.
        /// </summary>
        [HttpGet]
        [Route("purchase/paged")]
        [Route("purchase/search")]
        public async Task<IHttpActionResult> SearchPurchases(
            int start = 0,
            int length = 10,
            string keyword = null,
            int? supplierId = null,
            int? branchId = null,
            int? warehouseId = null,
            string status = null,
            string paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var pageSize = Math.Min(length > 0 ? length : 10, 100);
            var res = await _purchaseService.GetPurchaseOrdersDataTableAsync(
                start, pageSize, keyword, supplierId, branchId, warehouseId, status, paymentStatus, fromDate, toDate);

            return Ok(new { total = res.total, filtered = res.filtered, data = res.data });
        }

        /// <summary>
        /// Lấy đơn nhập hàng theo ID.
        /// </summary>
        [HttpGet, Route("purchase/{id:int}")]
        public async Task<IHttpActionResult> GetPurchaseById(int id)
        {
            var po = await _purchaseService.GetPurchaseByIdAsync(id);
            if (po == null) return NotFound();
            return Ok(po);
        }

        /// <summary>
        /// Tạo / Lưu đơn nhập hàng mới.
        /// </summary>
        [HttpPost, Route("purchase")]
        public async Task<IHttpActionResult> CreatePurchase([FromBody] PurchaseOrderDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu phiếu nhập không hợp lệ.");
            var id = await _purchaseService.CreatePurchaseAsync(dto);
            return Ok(new { success = true, purchaseOrderId = id });
        }

        /// <summary>
        /// Cập nhật đơn nhập hàng.
        /// </summary>
        [HttpPut, Route("purchase")]
        [HttpPost, Route("purchase/update")]
        public async Task<IHttpActionResult> UpdatePurchase([FromBody] PurchaseOrderDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu phiếu nhập không hợp lệ.");
            var success = await _purchaseService.UpdatePurchaseAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Xóa đơn nhập hàng.
        /// </summary>
        [HttpDelete, Route("purchase/{id:int}")]
        [HttpPost, Route("purchase/delete/{id:int}")]
        public async Task<IHttpActionResult> DeletePurchase(int id)
        {
            var success = await _purchaseService.DeletePurchaseAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Supplier Debts & Payments (Công nợ NCC)

        /// <summary>
        /// Lấy tất cả công nợ nhà cung cấp.
        /// </summary>
        [HttpGet, Route("debt/all")]
        public async Task<IHttpActionResult> GetAllDebts()
        {
            var debts = await _debtService.GetAllSupplierDebtsAsync();
            return Ok(debts);
        }

        /// <summary>
        /// Thống kê tổng hợp công nợ nhà cung cấp (4 Thẻ Widget).
        /// </summary>
        [HttpGet, Route("debt/summary")]
        public async Task<IHttpActionResult> GetDebtSummary(
            int? supplierId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var debts = await _debtService.GetAllSupplierDebtsAsync(null, supplierId, null, fromDate, toDate);
            decimal totalDebt = debts.Sum(d => d.TotalAmount ?? 0);
            decimal totalPaid = debts.Sum(d => d.PaidAmount ?? 0);
            decimal totalRemaining = debts.Sum(d => d.UnpaidAmount ?? 0);
            double payRate = totalDebt > 0 ? (double)Math.Round((totalPaid / totalDebt) * 100, 1) : 0;
            var nearDue = await _debtService.GetDebtsNearDueDateAsync(7);
            var overdue = await _debtService.GetOverdueDebtsAsync();

            return Ok(new
            {
                TotalDebt = totalDebt,
                TotalPaid = totalPaid,
                TotalRemaining = totalRemaining,
                PayRate = payRate,
                NearDueCount = nearDue.Count,
                OverdueCount = overdue.Count,
                TotalRecords = debts.Count
            });
        }

        /// <summary>
        /// Tìm kiếm công nợ nhà cung cấp với phân trang &amp; bộ lọc.
        /// </summary>
        [HttpGet]
        [Route("debt/paged")]
        [Route("debt/list")]
        public async Task<IHttpActionResult> GetFilteredDebts(
            int start = 0,
            int length = 10,
            string keyword = null,
            int? supplierId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var pageSize = Math.Min(length > 0 ? length : 10, 100);
            var allDebts = await _debtService.GetAllSupplierDebtsAsync(keyword, supplierId, status, fromDate, toDate);
            var totalCount = allDebts.Count;
            var pagedData = allDebts.Skip(start).Take(pageSize).ToList();
            return Ok(new { total = totalCount, filtered = totalCount, data = pagedData });
        }

        /// <summary>
        /// Lấy chi tiết công nợ theo ID.
        /// </summary>
        [HttpGet, Route("debt/{id:int}")]
        public async Task<IHttpActionResult> GetDebtById(int id)
        {
            var debt = await _debtService.GetSupplierDebtByIdAsync(id);
            if (debt == null) return NotFound();
            return Ok(debt);
        }

        /// <summary>
        /// Lấy công nợ theo mã Đơn nhập hàng (PurchaseOrderId).
        /// </summary>
        [HttpGet, Route("debt/by-purchase-order/{purchaseOrderId:int}")]
        public async Task<IHttpActionResult> GetSupplierDebtByPurchaseOrder(int purchaseOrderId)
        {
            var debt = await _debtService.GetSupplierDebtByPurchaseOrderIdAsync(purchaseOrderId);
            if (debt == null) return NotFound();
            return Ok(debt);
        }

        /// <summary>
        /// Tạo mới công nợ nhà cung cấp.
        /// </summary>
        [HttpPost]
        [Route("debt/create")]
        [Route("debt")]
        public async Task<IHttpActionResult> CreateSupplierDebt([FromBody] SupplierDebtDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu công nợ không hợp lệ.");
            var ok = await _debtService.CreateSupplierDebtAsync(dto);
            return Ok(new { success = ok });
        }

        /// <summary>
        /// Cập nhật công nợ nhà cung cấp.
        /// </summary>
        [HttpPost, Route("debt/update")]
        [HttpPut, Route("debt/{id:int}")]
        public async Task<IHttpActionResult> UpdateSupplierDebt([FromBody] SupplierDebtDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Dữ liệu công nợ không hợp lệ.");
            if (id.HasValue && dto.DebtId <= 0) dto.DebtId = id.Value;
            var ok = await _debtService.UpdateSupplierDebtAsync(dto);
            return Ok(new { success = ok });
        }

        /// <summary>
        /// Lấy danh sách công nợ sắp đến hạn (trong số ngày truyền vào).
        /// </summary>
        [HttpGet, Route("debt/near-due")]
        public async Task<IHttpActionResult> GetDebtsNearDueDate(int daysBefore = 7)
        {
            var debts = await _debtService.GetDebtsNearDueDateAsync(daysBefore);
            return Ok(debts);
        }

        /// <summary>
        /// Lấy danh sách công nợ quá hạn.
        /// </summary>
        [HttpGet, Route("debt/overdue")]
        public async Task<IHttpActionResult> GetOverdueDebts()
        {
            var debts = await _debtService.GetOverdueDebtsAsync();
            return Ok(debts);
        }

        /// <summary>
        /// Lấy danh sách thanh toán của một khoản nợ.
        /// </summary>
        [HttpGet, Route("debt/{debtId:int}/payments")]
        public async Task<IHttpActionResult> GetPaymentsByDebtId(int debtId)
        {
            var payments = await _debtService.GetPaymentsByDebtIdAsync(debtId);
            return Ok(payments);
        }

        /// <summary>
        /// Tạo thanh toán công nợ cho NCC.
        /// </summary>
        [HttpPost, Route("debt/payment")]
        public async Task<IHttpActionResult> CreateSupplierPayment([FromBody] SupplierPaymentDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu thanh toán không hợp lệ.");
            var success = await _debtService.CreateSupplierPaymentAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Xóa phiếu thanh toán công nợ.
        /// </summary>
        [HttpDelete, Route("debt/payment/{id:int}")]
        [HttpPost, Route("debt/payment/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteSupplierPayment(int id)
        {
            var success = await _debtService.DeleteSupplierPaymentAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Warehouses (Kho hàng)

        /// <summary>
        /// Lấy tất cả kho hàng.
        /// </summary>
        [HttpGet, Route("warehouses/all")]
        public async Task<IHttpActionResult> GetAllWarehouses(string name = null, int? branchId = null)
        {
            if (!string.IsNullOrEmpty(name) || branchId.HasValue)
            {
                var filtered = await _warehouseService.GetFilteredWarehouseAsync(name, branchId);
                return Ok(filtered);
            }
            var warehouses = await _warehouseService.GetAllWarehousesByBranchIdAsync();
            return Ok(warehouses);
        }

        /// <summary>
        /// Lấy danh sách kho theo Branch ID.
        /// </summary>
        [HttpGet, Route("warehouses/by-branch/{branchId:int}")]
        public async Task<IHttpActionResult> GetWarehousesByBranch(int branchId)
        {
            var warehouses = await _warehouseService.GetAllWarehouseByBranchId(branchId);
            return Ok(warehouses);
        }

        /// <summary>
        /// Lấy chi tiết kho hàng theo ID.
        /// </summary>
        [HttpGet, Route("warehouses/{id:int}")]
        public async Task<IHttpActionResult> GetWarehouseById(int id)
        {
            var wh = await _warehouseService.GetWarehouseByIdAsync(id);
            if (wh == null) return NotFound();
            return Ok(wh);
        }

        /// <summary>
        /// Tạo kho hàng mới.
        /// </summary>
        [HttpPost, Route("warehouses")]
        public async Task<IHttpActionResult> CreateWarehouse([FromBody] WarehouseDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu kho hàng không hợp lệ.");
            var id = await _warehouseService.CreateWarehouseAsync(dto);
            return Ok(new { success = true, warehouseId = id });
        }

        /// <summary>
        /// Cập nhật kho hàng.
        /// </summary>
        [HttpPut, Route("warehouses")]
        [HttpPost, Route("warehouses/update")]
        public async Task<IHttpActionResult> UpdateWarehouse([FromBody] WarehouseDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu kho hàng không hợp lệ.");
            var success = await _warehouseService.UpdateWarehouseAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Xóa kho hàng.
        /// </summary>
        [HttpDelete, Route("warehouses/{id:int}")]
        [HttpPost, Route("warehouses/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteWarehouse(int id)
        {
            var success = await _warehouseService.DeleteWarehouseAsync(id);
            return Ok(new { success });
        }

        #endregion
    }
}
