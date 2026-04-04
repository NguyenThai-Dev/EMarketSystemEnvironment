using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.InventoryModule.Services.Implementations
{
    public class StockMovementService : IStockMovementService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IProductLotService _productLotService;

        public StockMovementService(EMarket_DBEntities db, IProductService productService, IUserService userService, IProductLotService productLotService)
        {
            _db = db;
            _productService = productService;
            _userService = userService;
            _productLotService = productLotService;
        }

        public async Task<(int total, int filtered, List<StockMovementDTO> data)>
     GetStockMovementsDataTableAsync(
         int start,
         int length,
         int? warehouseId,
         string type,
         DateTime? fromDate,
         DateTime? toDate,
         string keyword)
        {
            // ======================================================
            // 0. BASE QUERY (NO KEYWORD)
            // ======================================================
            var baseQuery = _db.StockMovements
                .AsNoTracking()
                .AsQueryable();

            // ======================================================
            // 1. FILTER CƠ BẢN (WAREHOUSE / TYPE / DATE)
            // ======================================================
            if (warehouseId.HasValue)
                baseQuery = baseQuery.Where(x => x.warehouse_id == warehouseId.Value);

            if (!string.IsNullOrWhiteSpace(type))
                baseQuery = baseQuery.Where(x => x.movement_type == type);

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                baseQuery = baseQuery.Where(x => x.movement_date >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.Date.AddDays(1);
                baseQuery = baseQuery.Where(x => x.movement_date < to);
            }

            // ======================================================
            // 2. RECORDS TOTAL (DT REQUIREMENT)
            // ======================================================
            var total = await baseQuery.CountAsync();

            // ======================================================
            // 3. KEYWORD SEARCH (CROSS-MODULE VIA SERVICE)
            // ======================================================
            var query = baseQuery;

            List<int> matchedProductIds = null;
            List<int> matchedUserIds = null;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                // --- Product Module ---
                var products = await _productService.GetFilteredProductAsync(keyword, null);
                matchedProductIds = products.Select(x => x.ProductId).Where(id => id.HasValue).Select(id => id.Value).ToList();

                // --- User Module ---
                var users = await _userService.GetFilteredUsersAsync(keyword);
                matchedUserIds = users.Select(x => x.UserId).ToList();

                query = query.Where(x =>
                    (x.order_id.HasValue && x.order_id.Value.ToString().Contains(keyword))

                    // 2. Tìm theo Product
                    || (x.product_id.HasValue && matchedProductIds.Contains(x.product_id.Value))

                    // 3. Tìm theo User
                    || (x.user_id.HasValue && matchedUserIds.Contains(x.user_id.Value))

                     // 4. (Gợi ý thêm) Tìm theo Reason/Note nếu có
                     || (x.reason != null && x.reason.Contains(keyword))
                );
            }

            // ======================================================
            // 4. RECORDS FILTERED
            // ======================================================
            var filtered = await query.CountAsync();

            if (filtered == 0)
                return (total, 0, new List<StockMovementDTO>());

            // ======================================================
            // 5. PAGING + RAW FETCH
            // ======================================================
            var rawData = await query
                .OrderByDescending(x => x.movement_date)
                .Skip(start)
                .Take(length)
                .ToListAsync();

            // ======================================================
            // 6. DATA ENRICHMENT (BATCH – NO N+1)
            // ======================================================
            var productIds = rawData
                .Where(x => x.product_id.HasValue)
                .Select(x => x.product_id.Value)
                .Distinct()
                .ToList();

            var userIds = rawData
                .Where(x => x.user_id.HasValue)
                .Select(x => x.user_id.Value)
                .Distinct()
                .ToList();

            var warehouseIds = rawData
                .Where(x => x.warehouse_id.HasValue)
                .Select(x => x.warehouse_id.Value)
                .Distinct()
                .ToList();

            // --- Product ---
            var productDict = (await _productService.GetProductsByIdsAsync(productIds))
                .ToDictionary(k => k.ProductId, v => v);

            // --- User ---
            var userDict = (await _userService.GetUsersByUserIdsAsync(userIds))
                .ToDictionary(k => k.UserId, v => v.FullName);

            // --- Warehouse (same module → DB OK) ---
            var warehouseDict = await _db.Warehouses
                .Where(w => warehouseIds.Contains(w.warehouse_id))
                .ToDictionaryAsync(k => k.warehouse_id, v => v.name);

            // ======================================================
            // 7. FINAL MAPPING DTO
            // ======================================================
            var result = rawData.Select(x => new StockMovementDTO
            {
                MovementId = x.movement_id,
                OrderId = x.order_id,

                ProductId = x.product_id ?? 0,
                ProductName =
                    x.product_id.HasValue && productDict.TryGetValue(x.product_id.Value, out var p)
                        ? p.Name
                        : "Unknown Product",
                ProductImage =
                    x.product_id.HasValue && productDict.TryGetValue(x.product_id.Value, out p)
                        ? p.Image
                        : null,
                Reason = x.reason,
                Barcode =
                    x.product_id.HasValue && productDict.TryGetValue(x.product_id.Value, out p)
                        ? p.Barcode
                        : "N/A",
                WarehouseId = x.warehouse_id,
                WarehouseName =
                    x.warehouse_id.HasValue && warehouseDict.TryGetValue(x.warehouse_id.Value, out var w)
                        ? w
                        : "Unknown Warehouse",

                UserId = x.user_id,
                UserName =
                    x.user_id.HasValue && userDict.TryGetValue(x.user_id.Value, out var u)
                        ? u
                        : "System",

                MovementType = x.movement_type,
                Quantity = x.quantity ?? 0,
                MovementDate = x.movement_date ?? DateTime.MinValue,
                lotId = x.lot_id ?? 0
            }).ToList();

            return (total, filtered, result);
        }

        public async Task<decimal> GetTotalStockAsync(int productId, int warehouseId)
        {
            var lotIds = await _productLotService.GetLotIdsByProductIdAsync(productId);

            if (lotIds == null || !lotIds.Any()) return 0;

            var totalQty = await _db.Inventories
                .Where(inv => inv.warehouse_id == warehouseId && lotIds.Contains(inv.lot_id))
                .SumAsync(inv => inv.quantity);

            return totalQty ?? 0;
        }

        // =========================================================================
        // 2. ĐIỀU CHỈNH KHO (Logic phức tạp: FIFO)
        // =========================================================================
        public async Task<bool> AdjustStockAsync(StockAdjustmentDTO dto)
        {
            using (var tx = _db.Database.BeginTransaction())
            {
                try
                {
                    // --- BƯỚC 1: LẤY DANH SÁCH LOT ID CỦA SẢN PHẨM ---
                    var productLotIds = await _productLotService.GetLotIdsByProductIdAsync(dto.ProductId);

                    if (!productLotIds.Any())
                    {
                        // Nếu là trừ kho mà không tìm thấy lô nào của SP này -> Lỗi
                        if (dto.QuantityChange < 0) throw new Exception("Sản phẩm chưa có lịch sử lô hàng nào, không thể xuất kho.");

                        // Nếu là cộng kho -> Cũng chặn luôn vì Adjustment chỉ sửa cái có sẵn. 
                        // Muốn tạo mới phải dùng Purchase Order.
                        throw new Exception("Không tìm thấy lô hàng nào liên kết với sản phẩm này.");
                    }

                    // --- BƯỚC 2: LẤY INVENTORY ITEMS TỪ DB (Chưa sắp xếp Expiry) ---
                    var inventoryEntities = await _db.Inventories
                        .Where(inv => inv.warehouse_id == dto.WarehouseId
                                   && productLotIds.Contains(inv.lot_id)
                                   && inv.quantity > 0)
                        .ToListAsync();

                    if (!inventoryEntities.Any() && dto.QuantityChange < 0)
                    {
                        throw new Exception("Sản phẩm đã hết hàng trong kho này.");
                    }

                    // --- BƯỚC 3: LẤY THÔNG TIN CHI TIẾT LOT (ĐỂ LẤY HẠN SỬ DỤNG) ---
                    // Chỉ lấy thông tin của những lô đang tồn tại trong kho (để tối ưu)
                    var activeLotIds = inventoryEntities.Select(x => x.lot_id).ToList();
                    var lotDetails = await _productLotService.GetLotsByIdsAsync(activeLotIds);

                    // Tạo Dictionary để tra cứu nhanh ExpiryDate: [LotId] -> [LotDTO]
                    var lotMap = lotDetails.ToDictionary(k => k.LotId, v => v);

                    // --- BƯỚC 4: GHÉP DỮ LIỆU VÀ SẮP XẾP TRÊN RAM (IN-MEMORY JOIN) ---
                    var mergedItems = inventoryEntities.Select(inv => new
                    {
                        Entity = inv,
                        LotId = inv.lot_id,
                        // Nếu không tìm thấy thông tin lot (lỗi dữ liệu) thì cho hạn xa tít để trừ sau cùng
                        ExpiryDate = lotMap.ContainsKey(inv.lot_id) ? lotMap[inv.lot_id].ExpiryDate : DateTime.MaxValue,
                        CurrentQty = inv.quantity ?? 0
                    })
                    .OrderBy(x => x.ExpiryDate) // Sắp xếp FIFO: Hết hạn trước -> Trừ trước
                    .ToList();

                    // --- BƯỚC 5: XỬ LÝ LOGIC CỘNG/TRỪ ---
                    decimal remainingQty = Math.Abs(dto.QuantityChange);
                    bool isDeduction = dto.QuantityChange < 0;

                    if (isDeduction)
                    {
                        // >>> LOGIC TRỪ KHO (FIFO) <<<
                        decimal totalAvailable = mergedItems.Sum(x => x.CurrentQty);
                        if (totalAvailable < remainingQty)
                        {
                            throw new Exception($"Không đủ tồn kho. (Có: {totalAvailable}, Cần trừ: {remainingQty})");
                        }

                        foreach (var item in mergedItems)
                        {
                            if (remainingQty <= 0) break;

                            decimal qtyToDeduct = Math.Min(item.CurrentQty, remainingQty);

                            // Update Entity (Entity Framework tự track change)
                            item.Entity.quantity -= (int?)qtyToDeduct;
                            item.Entity.last_update = DateTime.Now;

                            // Ghi Log StockMovements
                            var movement = new StockMovement
                            {
                                product_id = dto.ProductId, // Lưu ProductId để query báo cáo dễ hơn
                                warehouse_id = dto.WarehouseId,
                                lot_id = item.LotId,
                                movement_type = dto.MovementType,
                                quantity = (int?)-qtyToDeduct,
                                movement_date = DateTime.Now,
                                user_id = dto.UserId,
                                reason = dto.Reason,
                            };
                            _db.StockMovements.Add(movement);

                            remainingQty -= qtyToDeduct;
                        }
                    }
                    else
                    {
                        // >>> LOGIC CỘNG KHO (ADJUSTMENT TĂNG) <<<
                        // Cộng vào lô nhập GẦN NHẤT (dựa trên LastUpdate của Inventory) 
                        // hoặc lô có hạn sử dụng XA NHẤT (tùy nghiệp vụ, ở đây dùng LastUpdate cho an toàn)

                        var targetItem = inventoryEntities
                            .OrderByDescending(x => x.last_update)
                            .FirstOrDefault();

                        if (targetItem == null)
                        {
                            // Trường hợp kho rỗng tuếch (Quantity = 0 hết), ta lấy đại lô có hạn xa nhất để cộng
                            targetItem = mergedItems.OrderByDescending(x => x.ExpiryDate).FirstOrDefault()?.Entity;

                            if (targetItem == null)
                                throw new Exception("Không xác định được lô hàng để điều chỉnh tăng.");
                        }

                        targetItem.quantity += (int?)remainingQty;
                        targetItem.last_update = DateTime.Now;

                        var movement = new StockMovement
                        {
                            product_id = dto.ProductId,
                            warehouse_id = dto.WarehouseId,
                            lot_id = targetItem.lot_id,
                            movement_type = dto.MovementType,
                            quantity = (int?)remainingQty,
                            movement_date = DateTime.Now,
                            user_id = dto.UserId
                        };
                        _db.StockMovements.Add(movement);
                    }

                    await _db.SaveChangesAsync();
                    tx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw new Exception("Lỗi xử lý kho: " + ex.Message, ex);
                }
            }
        }
    }
}