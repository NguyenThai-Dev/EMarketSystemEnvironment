using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.InventoryModule.Services.Implementations
{
    public class PurchaseService : IPurchaseService
    {
        private readonly EMarket_DBEntities _db;

        private readonly ISupplierService _supplierService;
        private readonly IUserContext _userContext;
        private readonly ISupplierServiceDebtAndPaymentService _supplierServiceDebtAndPaymentService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;
        private readonly ILoginService _loginService;
        private readonly IWarehouseService _warehouseService;
        private readonly IBranchService _branchService;
        private readonly IProductLotService _productLotService;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public PurchaseService(
            EMarket_DBEntities db,
            ISupplierService supplierService,
            ISupplierServiceDebtAndPaymentService debtAndPaymentService,
            IProductService productService,
            IProductCategoryService categoryService,
            ILoginService userService,
            IWarehouseService warehouseService,
            IBranchService branchService,
            IProductLotService productLotService,
            IUserContext userContext
        )
        {
            _db = db;

            _supplierService = supplierService;
            _supplierServiceDebtAndPaymentService = debtAndPaymentService;
            _productService = productService;
            _categoryService = categoryService;
            _loginService = userService;
            _warehouseService = warehouseService;
            _branchService = branchService;
            _productLotService = productLotService;
            _userContext = userContext;
        }

        // ------------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------------
        public async Task<List<PurchaseOrderDTO>> GetAllPurchaseAsync()
        {
            var orders = await _db.PurchaseOrders.AsNoTracking().ToListAsync();
            return await MapPurchaseAsync(orders);
        }

        public async Task<PurchaseOrderDTO> GetPurchaseByIdAsync(int id)
        {
            var orders = await _db.PurchaseOrders
                .Where(x => x.purchase_order_id == id)
                .ToListAsync();

            if (!orders.Any())
            {
                throw new Exception("Purchase order not found.");
            }

            var resultDTOs = await MapPurchaseAsync(orders);

            return resultDTOs.FirstOrDefault();
        }


        public async Task<int> CreatePurchaseAsync(PurchaseOrderDTO dto)
        {
            var currentUserId = _loginService.GetCurrentUserId();

            using (var tx = _db.Database.BeginTransaction())
            {
                try
                {
                    var orderDate = dto.OrderDate?.ToLocalTime() ?? DateTime.Now;
                    decimal calculatedTotal = dto.Details.Sum(d => d.TotalPrice ?? 0);

                    // 1. TẠO HEADER
                    var order = new PurchaseOrder
                    {
                        supplier_id = dto.SupplierId,
                        warehouse_id = dto.WarehouseId,
                        user_id = currentUserId ?? -1,
                        order_date = orderDate,
                        status = "Pending",
                        total_amount = calculatedTotal,
                        payment_status = dto.PaymentStatus ?? "Unpaid",
                        notes = dto.Notes
                    };

                    _db.PurchaseOrders.Add(order);
                    // Cần lưu để lấy order.purchase_order_id cho các bước sau
                    await _db.SaveChangesAsync();

                    // Danh sách các lô mới tạo trong đơn này để cập nhật BatchCode sau
                    var newLotsInOrder = new List<ProductLot>();

                    // 2. XỬ LÝ CHI TIẾT
                    foreach (var detailDto in dto.Details)
                    {
                        int? finalLotId = null;

                        if (detailDto.ProductLots != null && detailDto.ProductLots.Any())
                        {
                            var lotDto = detailDto.ProductLots.First();

                            // Kiểm tra tồn tại
                            var existingId = await _productLotService.FindExistingLotIdAsync(
                                detailDto.ProductId,
                                lotDto.ManufacturingDate,
                                lotDto.ExpiryDate
                            );

                            if (existingId.HasValue)
                            {
                                finalLotId = existingId.Value;
                            }
                            else
                            {
                                // CASE: TẠO MỚI ENTITY (Không gọi service để tránh SaveChanges vụn)
                                var newLot = new ProductLot
                                {
                                    product_id = detailDto.ProductId,
                                    manufacturing_date = lotDto.ManufacturingDate,
                                    expiry_date = lotDto.ExpiryDate,
                                    cost_price = detailDto.UnitPrice,
                                };

                                _db.ProductLots.Add(newLot);
                                newLotsInOrder.Add(newLot);
                                // Tạm thời chưa có ID, chúng ta sẽ gán detail.ProductLot sau
                            }
                        }

                        var detail = new PurchaseOrderDetail
                        {
                            purchase_order_id = order.purchase_order_id,
                            product_id = detailDto.ProductId,
                            category_id = detailDto.CategoryId,
                            quantity = detailDto.Quantity,
                            unit_price = detailDto.UnitPrice,
                            total_price = detailDto.Quantity * detailDto.UnitPrice,
                            // Nếu là lô cũ thì gán ngay, lô mới thì dùng Navigation Property bên dưới
                        };

                        if (finalLotId.HasValue)
                            detail.lot_id = finalLotId.Value;
                        else
                            detail.ProductLot = newLotsInOrder.LastOrDefault(); // Gán liên kết entity

                        _db.PurchaseOrderDetails.Add(detail);
                    }

                    // LƯU LẦN 2: Lưu tất cả Lots mới và Details mới cùng lúc
                    await _db.SaveChangesAsync();

                    // 3. CẬP NHẬT BATCH CODE CHO CÁC LÔ MỚI (Lúc này đã có lot_id)
                    if (newLotsInOrder.Any())
                    {
                        foreach (var lot in newLotsInOrder)
                        {
                            DateTime dateForCode = (lot.manufacturing_date == null || lot.manufacturing_date == new DateTime(2000, 1, 1))
                                                    ? (lot.expiry_date)
                                                    : lot.manufacturing_date.Value;

                            lot.batch_code = $"B-{lot.product_id}-{dateForCode:yyyyMMdd}-{lot.lot_id}";
                        }
                        // Lưu BatchCode - Lần này nhanh vì chỉ cập nhật chuỗi
                        await _db.SaveChangesAsync();
                    }

                    // 4. TẠO CÔNG NỢ
                    var debt = new SupplierDebt
                    {
                        purchase_order_id = order.purchase_order_id,
                        supplier_id = dto.SupplierId,
                        total_amount = calculatedTotal,
                        paid_amount = 0,
                        unpaid_amount = calculatedTotal,
                        due_date = DateTime.Now.AddDays(30),
                        status = "Unpaid",
                        updated_at = DateTime.Now
                    };
                    _db.SupplierDebts.Add(debt);

                    // 5. CHỐT STATUS
                    if (dto.Status == "Completed")
                    {
                        order.status = "Completed";
                    }

                    await _db.SaveChangesAsync(); // Lưu công nợ và trạng thái cuối cùng

                    tx.Commit();
                    return order.purchase_order_id;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> UpdatePurchaseAsync(PurchaseOrderDTO dto)
        {
            var curUser = _userContext.UserId;

            // Dùng Transaction để đảm bảo tính toàn vẹn (ACID)
            using (var tx = _db.Database.BeginTransaction())
            {
                try
                {
                    // =================================================================
                    // 1. GET ENTITY HEADER & VALIDATE
                    // =================================================================
                    var entity = await _db.PurchaseOrders
                        .Include(p => p.PurchaseOrderDetails) // Load chi tiết cũ
                        .FirstOrDefaultAsync(x => x.purchase_order_id == dto.PurchaseOrderId);

                    if (entity == null)
                        throw new Exception("Không tìm thấy đơn hàng.");

                    // TƯỜNG LỬA: Chặn sửa nếu đơn đã hoàn tất (để bảo vệ dữ liệu Kho)
                    if (entity.status == "Completed")
                        throw new Exception("Đơn hàng đã nhập kho (Completed), không được phép chỉnh sửa.");

                    // =================================================================
                    // 2. UPDATE HEADER
                    // =================================================================
                    entity.supplier_id = dto.SupplierId;
                    entity.warehouse_id = dto.WarehouseId;
                    entity.user_id = curUser;
                    entity.order_date = dto.OrderDate;
                    entity.notes = dto.Notes;

                    // Tính lại tổng tiền dựa trên chi tiết mới gửi lên
                    decimal recalTotal = dto.Details.Sum(x => x.UnitPrice * x.Quantity ?? 0);
                    entity.total_amount = recalTotal;

                    // =================================================================
                    // 3. XỬ LÝ CHI TIẾT (SMART UPDATE: DELETE -> SCAN LOT -> UPSERT)
                    // =================================================================
                    var existingDetails = entity.PurchaseOrderDetails.ToList();
                    var incomingDetails = dto.Details;

                    // A. DELETE: Xóa những dòng có trong DB mà KHÔNG có trong DTO gửi lên
                    var detailsToDelete = existingDetails
                        .Where(e => !incomingDetails.Any(i => i.PurchaseOrderDetailId == e.purchase_order_detail_id && i.PurchaseOrderDetailId != 0))
                        .ToList();

                    if (detailsToDelete.Any())
                    {
                        _db.PurchaseOrderDetails.RemoveRange(detailsToDelete);
                    }

                    // B. LOOP XỬ LÝ TỪNG DÒNG (THÊM HOẶC SỬA)
                    foreach (var d in incomingDetails)
                    {
                        int finalLotId = 0;

                        // --- LOGIC XỬ LÝ LOT: "DÒ TÌM TRƯỚC - HÀNH ĐỘNG SAU" ---
                        if (d.ProductLots != null && d.ProductLots.Any())
                        {
                            var lotDto = d.ProductLots.First(); // Lấy thông tin lô từ UI

                            // BƯỚC 1: Quét DB xem có lô nào khớp (ProductId + Ngày SX + HSD) chưa?
                            var foundId = await _productLotService.FindExistingLotIdAsync(
                                d.ProductId,
                                lotDto.ManufacturingDate,
                                lotDto.ExpiryDate
                            );

                            Debug.WriteLine($"[PurchaseService][UpdatePurchaseAsync] Found Lot ID: {foundId}");

                            if (foundId.HasValue)
                            {
                                // >>> CASE 1: TÌM THẤY -> DÙNG LẠI & CẬP NHẬT GIÁ VỐN
                                // Logic: Đã khớp ngày tháng thì đó chính là lô cũ, chỉ cần update lại giá nhập mới nhất
                                lotDto.LotId = foundId.Value;
                                lotDto.CostPrice = d.UnitPrice;

                                await _productLotService.UpdateProductLotCostAsync(lotDto); // Chỉ update giá

                                finalLotId = foundId.Value;
                            }
                            else
                            {
                                // >>> CASE 2: KHÔNG TÌM THẤY -> TẠO MỚI (INSERT)
                                // Logic: Lô này có ngày tháng khác biệt -> Là lô mới hoàn toàn
                                lotDto.ProductId = d.ProductId;
                                lotDto.CostPrice = d.UnitPrice;

                                finalLotId = await _productLotService.CreateProductLotAsync(lotDto);
                            }
                        }
                        else
                        {
                            // Bắt buộc phải có thông tin lô để đảm bảo quy trình kho
                            throw new Exception($"Sản phẩm {d.ProductName ?? d.ProductId.ToString()} thiếu thông tin Lô hàng.");
                        }

                        // --- XỬ LÝ DETAIL (UPSERT) ---
                        if (d.PurchaseOrderDetailId == 0)
                        {
                            // >>> INSERT (Dòng mới)
                            var newDetail = new PurchaseOrderDetail
                            {
                                purchase_order_id = dto.PurchaseOrderId,
                                product_id = d.ProductId,
                                category_id = d.CategoryId,
                                quantity = d.Quantity,
                                unit_price = d.UnitPrice,
                                total_price = d.Quantity * d.UnitPrice,
                                lot_id = finalLotId // <--- Luôn gán ID chuẩn (Cũ hoặc Mới)
                            };
                            _db.PurchaseOrderDetails.Add(newDetail);
                        }
                        else
                        {
                            // >>> UPDATE (Dòng cũ)
                            var existingDetail = existingDetails.FirstOrDefault(e => e.purchase_order_detail_id == d.PurchaseOrderDetailId);
                            if (existingDetail != null)
                            {
                                existingDetail.product_id = d.ProductId;
                                existingDetail.quantity = d.Quantity;
                                existingDetail.unit_price = d.UnitPrice;
                                existingDetail.total_price = d.Quantity * d.UnitPrice;
                                existingDetail.lot_id = finalLotId; // Cập nhật lại Lot ID nếu logic dò tìm thay đổi kết quả
                            }
                        }
                    }

                    // =================================================================
                    // 4. UPDATE CÔNG NỢ (SUPPLIER DEBT)
                    // =================================================================
                    var debt = await _db.SupplierDebts
                        .FirstOrDefaultAsync(x => x.purchase_order_id == dto.PurchaseOrderId);

                    if (debt != null)
                    {
                        debt.total_amount = recalTotal;
                        debt.unpaid_amount = debt.total_amount - (debt.paid_amount ?? 0);
                        debt.updated_at = DateTime.Now;

                        // Logic cập nhật trạng thái nợ
                        if (entity.payment_status == "Paid")
                        {
                            debt.status = "Paid";
                        }
                        else
                        {
                            // Tự động tính toán dựa trên số đã trả
                            if (debt.paid_amount >= debt.total_amount) debt.status = "Paid";
                            else if (debt.paid_amount > 0) debt.status = "Partial";
                            else debt.status = "Unpaid";
                        }
                    }

                    // =================================================================
                    // 5. COMMIT & TRIGGER KÍCH HOẠT
                    // =================================================================
                    entity.status = dto.Status;

                    // Lệnh này sẽ đẩy tất cả thay đổi (Lot, Detail, Header, Debt) xuống DB cùng lúc
                    // Nếu Status = 'Completed', Trigger SQL 'trg_PurchaseOrder_Complete_Inventory' sẽ chạy ngay sau lệnh này
                    await _db.SaveChangesAsync();

                    tx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { /* ignore rollback error */ }

                    var inner = ex.InnerException;
                    while (inner != null)
                    {
                        Debug.WriteLine("Inner Error: " + inner.Message);
                        inner = inner.InnerException;
                    }

                    throw;
                }
            }
        }

        // ------------------------------------------------------------
        // DELETE (DELETE DETAILS ALSO)
        // ------------------------------------------------------------
        public async Task<bool> DeletePurchaseAsync(int id)
        {
            using (var tx = _db.Database.BeginTransaction())
            {
                try
                {
                    var order = await _db.PurchaseOrders.FindAsync(id);
                    if (order == null)
                        return false;

                    // LOAD DETAILS
                    var details = await _db.PurchaseOrderDetails
                        .Where(x => x.purchase_order_id == id)
                        .ToListAsync();

                    var productIds = details.Select(d => d.product_id).Distinct().ToList();
                    var lotIds = details.Where(d => d.lot_id.HasValue)
                                        .Select(d => d.lot_id.Value)
                                        .ToList();

                    // LẤY LOT IDs ĐÚNG MODULE ProductLot
                    var realLotIds = await _productLotService
                        .GetLotIdsByProductAndLotAsync(productIds, lotIds);

                    // XÓA BẰNG SERVICE (CHUẨN MODULE)
                    await _productLotService.DeleteProductLotsByIdsAsync(realLotIds);

                    // XÓA DETAILS
                    _db.PurchaseOrderDetails.RemoveRange(details);

                    // XÓA DEBT
                    var debt = await _db.SupplierDebts
                        .FirstOrDefaultAsync(x => x.purchase_order_id == id);

                    if (debt != null)
                        _db.SupplierDebts.Remove(debt);

                    // XÓA ORDER
                    _db.PurchaseOrders.Remove(order);

                    await _db.SaveChangesAsync();
                    tx.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw new Exception("Error while delete purchase: " + ex.Message, ex);
                }
            }
        }

        // ------------------------------------------------------------
        // FILTER SEARCH
        // ------------------------------------------------------------
        public async Task<List<PurchaseOrderDTO>> GetFilteredPurchasesAsync(
            string keyword,
            int? supplierId,
            int? branchId,
            int? warehouseId,
            string status,
            string paymentStatus,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _db.PurchaseOrders.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.notes.Contains(keyword));

            if (supplierId.HasValue)
                query = query.Where(x => x.supplier_id == supplierId.Value);

            if (branchId.HasValue)
            {
                var warehouses = await _db.Warehouses
                    .Where(w => w.branch_id == branchId.Value)
                    .Select(w => w.warehouse_id)
                    .ToListAsync();

                query = query.Where(x => warehouses.Contains(x.warehouse_id));
            }

            if (warehouseId.HasValue)
            {
                var warehouses = await _db.Warehouses
                    .Where(w => w.warehouse_id == warehouseId.Value)
                    .Select(w => w.warehouse_id)
                    .ToListAsync();

                query = query.Where(x => warehouses.Contains(x.warehouse_id));
            }

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.status == status);

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(x => x.payment_status == paymentStatus);

            if (fromDate.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.order_date) >= DbFunctions.TruncateTime(fromDate));

            if (toDate.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.order_date) <= DbFunctions.TruncateTime(toDate));

            var list = await query.OrderByDescending(x => x.purchase_order_id).ToListAsync();
            return await MapPurchaseAsync(list);
        }

        private async Task<List<PurchaseOrderDTO>> MapPurchaseAsync(List<PurchaseOrder> orders)
        {
            if (orders == null || !orders.Any())
                return new List<PurchaseOrderDTO>();

            var orderIds = orders.Select(o => o.purchase_order_id).ToList();

            // 1. LOAD DETAILS
            var details = await _db.PurchaseOrderDetails
                .Where(d => orderIds.Contains(d.purchase_order_id))
                .ToListAsync();

            // 2. LOAD PRODUCT & LOTS
            var productIds = details.Select(d => d.product_id).Distinct().ToList();
            var products = await _productService.GetProductsByIdsAsync(productIds);
            var productMap = products.ToDictionary(p => p.ProductId, p => p);

            // --- [TỐI ƯU] ---
            // Thay vì lấy tất cả Lot của Product, ta chỉ nên lấy những Lot ID có trong Detail
            // Tuy nhiên, nếu bạn muốn giữ logic cũ (lấy theo ProductId) thì sửa ở đoạn mapping bên dưới.
            // Ở đây tôi giữ nguyên cách lấy data của bạn nhưng sẽ LỌC KỸ ở bước Map.
            var productLots = await _productLotService.GetAllProductLotsByIdsAsync(productIds);
            var productLotMap = productLots
                .GroupBy(pl => pl.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3. LOAD LOOKUPS (Supplier, Warehouse, User, Branch...)
            // ... (Giữ nguyên đoạn code Load Lookups của bạn) ...
            var supplierIds = orders.Select(o => o.supplier_id).Distinct().ToList();
            var warehouseIds = orders.Select(o => o.warehouse_id).Distinct().ToList();
            var userIds = orders.Select(o => o.user_id).Distinct().ToList();

            var supplierDTOs = await _supplierService.GetAllSupplierByIdAsync(supplierIds);
            var supplierMap = supplierDTOs.ToDictionary(s => s.SupplierId, s => s.Name);

            var warehouseDTOs = await _warehouseService.GetWarehouseByIdsAsync(warehouseIds);
            var warehouseMap = warehouseDTOs.ToDictionary(w => w.WarehouseId, w => w);

            var userDTOs = await _loginService.GetAllUsersByIdsAsync(userIds);
            var userMap = userDTOs.ToDictionary(u => u.UserId, u => u.FullName);

            var branchIds = warehouseDTOs.Select(w => w.BranchId).Distinct().ToList();
            var branchDTOs = await _branchService.GetBranchByIdsAsync(branchIds);
            var branchMap = branchDTOs.ToDictionary(b => b.BranchId, b => b.Name);

            // 4. LOAD DEBTS
            var supplierDebtDTOs = await _supplierServiceDebtAndPaymentService.GetSupplierDebtsByIdsAsync(orderIds);
            var debtLookup = supplierDebtDTOs
                .GroupBy(d => d.PurchaseOrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 5. GROUP DETAILS
            var detailLookup = details
                .GroupBy(d => d.purchase_order_id)
                .ToDictionary(g => g.Key, g => g.ToList());

            // ========================= FINAL MAPPING =========================
            var result = orders.Select(o =>
            {
                supplierMap.TryGetValue(o.supplier_id, out string supplierName);
                warehouseMap.TryGetValue(o.warehouse_id, out var warehouseDTO);
                userMap.TryGetValue(o.user_id, out string userName);

                string branchName = null;
                if (warehouseDTO != null)
                    branchMap.TryGetValue(warehouseDTO.BranchId, out branchName);

                var nestedDetails = new List<PurchaseOrderDetailDTO>();
                if (detailLookup.TryGetValue(o.purchase_order_id, out var orderDetails))
                {
                    nestedDetails = orderDetails.Select(d =>
                    {
                        productMap.TryGetValue(d.product_id, out var p);

                        // Lấy danh sách tất cả Lot của sản phẩm này
                        productLotMap.TryGetValue(d.product_id, out var allLotsOfProduct);

                        // [FIX QUAN TRỌNG] : Chỉ lấy đúng cái LotId được lưu trong Detail
                        // Nếu d.lot_id null thì trả về list rỗng
                        var specificLot = allLotsOfProduct?
                            .FirstOrDefault(l => l.LotId == d.lot_id);

                        var finalLotList = new List<ProductLotDTO>();
                        if (specificLot != null)
                        {
                            finalLotList.Add(new ProductLotDTO
                            {
                                LotId = specificLot.LotId,
                                ProductId = specificLot.ProductId,
                                ExpiryDate = specificLot.ExpiryDate,
                                CostPrice = specificLot.CostPrice,
                                ManufacturingDate = specificLot.ManufacturingDate,
                            });
                        }

                        return new PurchaseOrderDetailDTO
                        {
                            PurchaseOrderDetailId = d.purchase_order_detail_id,
                            PurchaseOrderId = d.purchase_order_id,
                            ProductId = d.product_id,

                            ProductName = p?.Name,
                            CategoryName = p?.CategoryName,
                            Unit = p?.Unit,
                            Quantity = d.quantity,
                            UnitPrice = d.unit_price,
                            TotalPrice = d.total_price,

                            // Gán danh sách đã lọc (Chỉ chứa 1 phần tử hoặc rỗng)
                            ProductLots = finalLotList
                        };
                    }).OrderByDescending(po => po.PurchaseOrderId).ToList();
                }

                var supplierDetail = debtLookup.ContainsKey(o.purchase_order_id)
                    ? debtLookup[o.purchase_order_id]
                    : new List<SupplierDebtDTO>();

                return new PurchaseOrderDTO
                {
                    PurchaseOrderId = o.purchase_order_id,
                    SupplierId = o.supplier_id,
                    WarehouseId = o.warehouse_id,
                    UserId = o.user_id,
                    BranchId = warehouseDTO?.BranchId ?? 0,
                    OrderDate = o.order_date ?? DateTime.Now,
                    Status = o.status,
                    PaymentStatus = o.payment_status,
                    TotalAmount = o.total_amount,
                    Notes = o.notes,

                    SupplierName = supplierName,
                    WarehouseName = warehouseDTO?.Name,
                    UserName = userName,
                    BranchName = branchName,

                    Details = nestedDetails,
                    SupplierDetail = supplierDetail
                };
            }).ToList();

            return result;
        }
        public async Task<List<PurchaseOrderDTO>> GetPurchaseByBranchIdAsync(int? branchId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                // 1. Khởi tạo query (Chưa thực thi xuống DB)
                var query = from po in _db.PurchaseOrders.AsNoTracking()
                            join w in _db.Warehouses.AsNoTracking() on po.warehouse_id equals w.warehouse_id
                            select new
                            {
                                po.purchase_order_id,
                                po.warehouse_id,
                                po.order_date,
                                po.total_amount,
                                w.branch_id // Chỉ lấy thêm cột này để lọc
                            };

                // 2. Lọc theo Chi nhánh
                if (branchId.HasValue && branchId.Value > 0)
                {
                    query = query.Where(x => x.branch_id == branchId.Value);
                }

                // 3. Lọc Theo ngày (Sử dụng cách tiếp cận Half-Open Interval: [fromDate, toDate + 1))
                if (fromDate.HasValue)
                {
                    var start = fromDate.Value.Date;
                    query = query.Where(x => x.order_date >= start);
                }

                if (toDate.HasValue)
                {
                    var end = toDate.Value.Date.AddDays(1);
                    query = query.Where(x => x.order_date < end);
                }

                // 4. Sắp xếp và Map sang DTO (Thực thi duy nhất 1 lần tại ToListAsync)
                return await query
                    .OrderByDescending(x => x.order_date)
                    .Select(x => new PurchaseOrderDTO
                    {
                        PurchaseOrderId = x.purchase_order_id,
                        WarehouseId = x.warehouse_id,
                        OrderDate = x.order_date,
                        TotalAmount = x.total_amount ?? 0,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây nếu có hệ thống Logging (Serilog, NLog...)
                throw new Exception("Lỗi khi truy vấn danh sách nhập hàng. Vui lòng kiểm tra lại tham số.", ex);
            }
        }
    }
}
