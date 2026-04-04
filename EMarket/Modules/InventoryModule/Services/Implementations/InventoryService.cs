using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;














// sau này có làm eventory hãy thêm send sms thông báo hết hàng.
















namespace EMarket.Modules.InventoryModule.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IProductLotService _productLotService;
        private readonly IWarehouseService _warehouseService;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public InventoryService(EMarket_DBEntities db, IProductLotService productLotService, IWarehouseService warehouseService)
        {
            _db = db;
            _productLotService = productLotService;
            _warehouseService = warehouseService;
        }

        public async Task<List<InventoryDTO>> GetAllInventoryAsync()
        {
            try
            {
                return await _db.Inventories
                    .OrderByDescending(i => i.inventory_id)
                    .Select(i => new InventoryDTO
                    {
                        InventoryId = i.inventory_id,
                        LotId = i.lot_id,
                        WarehouseId = i.warehouse_id,
                        Quantity = i.quantity ?? 0,
                        LastUpdate = i.last_update ?? defaultDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy danh sách tồn kho: " + ex.Message, ex);
            }
        }

        public async Task<List<InventoryDTO>> GetInventoryByProductIdsAsync(
    List<int> productIds,
    int? warehouseId = null)
        {
            var query = _db.Inventories
                .AsNoTracking()
                .Include(i => i.Warehouse)
                .Include(i => i.Warehouse.Branch)
                .Include(i => i.ProductLot)
                .AsQueryable();

            // Filter theo product
            query = query.Where(i => productIds.Contains(i.ProductLot.product_id));

            // Filter theo warehouse (nếu có)
            if (warehouseId.HasValue)
            {
                query = query.Where(i => i.warehouse_id == warehouseId.Value);
            }

            var inventoryList = await query
                .Select(i => new InventoryDTO
                {
                    InventoryId = i.inventory_id,
                    LotId = i.lot_id,
                    WarehouseId = i.warehouse_id,
                    Quantity = i.quantity ?? 0,

                    WarehouseName = i.Warehouse != null
                        ? i.Warehouse.name
                        : null,

                    BranchName = i.Warehouse != null && i.Warehouse.Branch != null
                        ? i.Warehouse.Branch.name
                        : null
                })
                .ToListAsync();

            return inventoryList;
        }

        public async Task<List<InventoryDTO>> GetInventoryByProductIdsAsync(
    List<int> productIds,
    int? warehouseId = null,
    int? branchId = null)
        {
            var query = _db.Inventories
                .AsNoTracking()
                .Include(i => i.Warehouse)
                .Include(i => i.Warehouse.Branch)
                .Include(i => i.ProductLot)
                .AsQueryable();

            // 1. Filter theo Product (bắt buộc)
            query = query.Where(i => productIds.Contains(i.ProductLot.product_id));

            // 2. Filter theo Warehouse (nếu có)
            if (warehouseId.HasValue)
            {
                query = query.Where(i => i.warehouse_id == warehouseId.Value);
            }

            // 3. Filter theo Branch (nếu có)
            if (branchId.HasValue)
            {
                query = query.Where(i =>
                    i.Warehouse != null &&
                    i.Warehouse.branch_id == branchId.Value
                );
            }

            var inventoryList = await query
                .Select(i => new InventoryDTO
                {
                    InventoryId = i.inventory_id,
                    LotId = i.lot_id,
                    WarehouseId = i.warehouse_id,
                    Quantity = i.quantity ?? 0,

                    WarehouseName = i.Warehouse != null
                        ? i.Warehouse.name
                        : null,

                    BranchName = i.Warehouse != null && i.Warehouse.Branch != null
                        ? i.Warehouse.Branch.name
                        : null,
                    BatchCode = i.ProductLot != null
                        ? i.ProductLot.batch_code
                        : null
                })
                .ToListAsync();

            return inventoryList;
        }



        public async Task<List<InventoryDTO>> GetFilteredInventoryAsync(
     int? productId,
     int? warehouseId)
        {
            try
            {
                var query = _db.Inventories
                    .AsNoTracking()
                    .AsQueryable();

                if (warehouseId.HasValue)
                    query = query.Where(i => i.warehouse_id == warehouseId.Value);

                // ===== FILTER BY PRODUCT (THROUGH LOT SERVICE) =====
                if (productId.HasValue)
                {
                    var lots = await _productLotService
                        .GetProductLotsByProductIdAsync(productId.Value);

                    if (lots == null || lots.Count == 0)
                        return new List<InventoryDTO>();

                    var lotIds = lots.Select(x => x.LotId).ToList();
                    query = query.Where(i => lotIds.Contains(i.lot_id));
                }

                // ===== LOAD INVENTORY DATA FIRST =====
                var inventories = await query
                    .OrderByDescending(i => i.inventory_id)
                    .Select(i => new InventoryDTO
                    {
                        InventoryId = i.inventory_id,
                        LotId = i.lot_id,
                        WarehouseId = i.warehouse_id,
                        Quantity = i.quantity ?? 0,
                        LastUpdate = i.last_update ?? defaultDate
                    })
                    .ToListAsync();

                if (!inventories.Any())
                    return inventories;

                // ===== LOAD WAREHOUSES + BRANCH =====
                var warehouseIds = inventories
                    .Select(x => x.WarehouseId)
                    .Distinct()
                    .ToList();

                var warehouses = await _warehouseService.GetWarehouseByIdsAsync(warehouseIds);

                var warehouseMap = warehouses.ToDictionary(
                    w => w.WarehouseId,
                    w => w
                );

                // ===== MAP UI FIELDS =====
                foreach (var inv in inventories)
                {
                    if (warehouseMap.TryGetValue(inv.WarehouseId, out var wh))
                    {
                        inv.WarehouseName = wh.Name;
                        inv.BranchName = wh.BranchName;
                    }
                }

                return inventories;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lọc tồn kho: " + ex.Message, ex);
            }
        }



        public async Task<InventoryDTO> GetInventoryByIdAsync(int id)
        {
            try
            {
                return await _db.Inventories
                    .Where(i => i.inventory_id == id)
                    .Select(i => new InventoryDTO
                    {
                        InventoryId = i.inventory_id,
                        LotId = i.lot_id,
                        WarehouseId = i.warehouse_id,
                        Quantity = i.quantity ?? 0,
                        LastUpdate = i.last_update ?? defaultDate
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi lấy tồn kho theo ID: " + ex.Message, ex);
            }
        }

        public async Task<bool> CreateInventoryAsync(InventoryDTO dto)
        {
            try
            {
                var entity = new Inventory
                {
                    lot_id = dto.LotId,
                    warehouse_id = dto.WarehouseId,
                    quantity = dto.Quantity,
                    last_update = DateTime.Now
                };

                _db.Inventories.Add(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi tạo tồn kho: " + ex.Message, ex);
            }
        }

        public async Task<List<InventoryDTO>> GetAllAsync(int? branchId)
        {
            try
            {
                var query =
                    from i in _db.Inventories.AsNoTracking()
                    join w in _db.Warehouses.AsNoTracking()
                        on i.warehouse_id equals w.warehouse_id
                    where !branchId.HasValue || w.branch_id == branchId.Value
                    select new InventoryDTO
                    {
                        InventoryId = i.inventory_id,
                        WarehouseId = i.warehouse_id,
                        LotId = i.lot_id,
                        Quantity = i.quantity ?? -1,
                        WarehouseName = w.name

                    };

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllAsync (Inventory) failed", ex);
            }
        }

        public async Task<bool> UpdateInventoryAsync(InventoryDTO dto)
        {
            try
            {
                var entity = await _db.Inventories.FindAsync(dto.InventoryId);
                if (entity == null)
                    return false;

                entity.lot_id = dto.LotId;
                entity.warehouse_id = dto.WarehouseId;
                entity.quantity = dto.Quantity;
                entity.last_update = DateTime.Now;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi cập nhật tồn kho: " + ex.Message, ex);
            }
        }

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            try
            {
                var entity = await _db.Inventories.FindAsync(id);
                if (entity == null)
                    return false;

                _db.Inventories.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi xóa tồn kho: " + ex.Message, ex);
            }
        }
    }
}
