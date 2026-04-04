using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;


namespace EMarket.Modules.InventoryModule.Services.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly EMarket_DBEntities _db;
        DateTime defaultDate = new DateTime(2000, 1, 1);

        public WarehouseService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<WarehouseDTO>> GetAllWarehousesByBranchIdAsync()
        {
            try
            {
                return await Task.Run(() =>
                {
                    return _db.Warehouses
                        .Select(w => new WarehouseDTO
                        {
                            WarehouseId = w.warehouse_id,
                            BranchId = w.branch_id,
                            Name = w.name,
                            Address = w.address,
                            AddressUrl = w.address_url,
                            Latitude = w.latitude,
                            Longitude = w.longitude,
                            CreatedAt = w.created_at ?? defaultDate,
                            UpdatedAt = w.updated_at,
                            BranchName = w.Branch.name
                        })
                        .ToList();
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAllWarehousesAsync: " + ex.Message);
            }
        }

        public async Task<List<WarehouseDTO>> GetWarehouseByIdsAsync(List<int> ids)
        {
            return await _db.Warehouses
                .AsNoTracking()
                .Where(w => ids.Contains(w.warehouse_id))
                .Select(w => new WarehouseDTO
                {
                    WarehouseId = w.warehouse_id,
                    BranchId = w.branch_id,
                    Name = w.name,
                    Address = w.address,
                    AddressUrl = w.address_url,
                    Latitude = w.latitude,
                    Longitude = w.longitude,
                    CreatedAt = w.created_at ?? defaultDate,
                    UpdatedAt = w.updated_at,
                    BranchName = w.Branch.name
                })
                .ToListAsync();
        }


        public async Task<List<WarehouseDTO>> GetFilteredWarehouseAsync(string name, int? branchId)
        {
            // 1. Lấy dữ liệu Warehouse thô (Flat Data)
            var query = _db.Warehouses.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(w => w.name.Contains(name.Trim()));
            }

            if (branchId.HasValue && branchId.Value > 0)
            {
                query = query.Where(w => w.branch_id == branchId.Value);
            }

            var warehouses = await query.ToListAsync();

            if (!warehouses.Any()) return new List<WarehouseDTO>();

            // 2. Lấy danh sách ID để truy vấn các bảng liên quan một lần duy nhất
            var warehouseIds = warehouses.Select(w => w.warehouse_id).ToList();
            var distinctBranchIds = warehouses.Select(w => w.branch_id).Distinct().ToList();

            // Lấy tên chi nhánh (Dùng Dictionary để map nhanh)
            var branchDict = await _db.Branches
                .AsNoTracking()
                .Where(b => distinctBranchIds.Contains(b.branch_id))
                .ToDictionaryAsync(b => b.branch_id, b => b.name);

            // Kiểm tra kho có chứa hàng không (Check bảng Inventories)
            // Nếu ID kho xuất hiện trong bảng Inventory thì không cho xóa
            var warehousesWithStock = await _db.Inventories
                .AsNoTracking()
                .Where(i => warehouseIds.Contains(i.warehouse_id))
                .Select(i => i.warehouse_id)
                .Distinct()
                .ToListAsync();

            // 3. Map thủ công sang DTO (Thực hiện trên RAM để tránh EF loop)
            var result = warehouses.Select(w => new WarehouseDTO
            {
                WarehouseId = w.warehouse_id,
                BranchId = w.branch_id,
                Name = w.name,
                Address = w.address,
                AddressUrl = w.address_url,
                Latitude = w.latitude,
                Longitude = w.longitude,
                CreatedAt = w.created_at ?? DateTime.Now,
                UpdatedAt = w.updated_at,
                // Gán tên chi nhánh từ Dictionary
                BranchName = branchDict.ContainsKey(w.branch_id) ? branchDict[w.branch_id] : "N/A",
                // Nếu kho không nằm trong danh sách có tồn kho thì cho phép xóa
                CanBeDeleted = !warehousesWithStock.Contains(w.warehouse_id)
            }).ToList();

            return result;
        }

        public async Task<WarehouseDTO> GetWarehouseByIdAsync(int warehouseId)
        {
            try
            {
                return await Task.Run(() =>
                {
                    return _db.Warehouses
                        .Where(w => w.warehouse_id == warehouseId)
                        .Select(w => new WarehouseDTO
                        {
                            WarehouseId = w.warehouse_id,
                            BranchId = w.branch_id,
                            Name = w.name,
                            Address = w.address,
                            AddressUrl = w.address_url,
                            Latitude = w.latitude,
                            Longitude = w.longitude,
                            CreatedAt = w.created_at ?? defaultDate,
                            UpdatedAt = w.updated_at,
                            BranchName = w.Branch.name
                        })
                        .FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetWarehouseByIdAsync: " + ex.Message);
            }
        }

        public async Task<int> CreateWarehouseAsync(WarehouseDTO dto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var entity = new Warehouse
                    {
                        branch_id = dto.BranchId,
                        name = dto.Name,
                        address = dto.Address,
                        address_url = dto.AddressUrl,
                        latitude = dto.Latitude,
                        longitude = dto.Longitude,
                        created_at = DateTime.Now
                    };

                    _db.Warehouses.Add(entity);
                    _db.SaveChanges();

                    return entity.warehouse_id;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in CreateWarehouseAsync: " + ex.Message);
            }
        }

        public async Task<bool> UpdateWarehouseAsync(WarehouseDTO dto)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var entity = _db.Warehouses.Find(dto.WarehouseId);
                    if (entity == null) return false;

                    entity.branch_id = dto.BranchId;
                    entity.name = dto.Name;
                    entity.address = dto.Address;
                    entity.address_url = dto.AddressUrl;
                    entity.latitude = dto.Latitude;
                    entity.longitude = dto.Longitude;
                    entity.updated_at = DateTime.Now;

                    _db.SaveChanges();
                    return true;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateWarehouseAsync: " + ex.Message);
            }
        }

        public async Task<bool> DeleteWarehouseAsync(int warehouseId)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var entity = _db.Warehouses.Find(warehouseId);
                    if (entity == null) return false;

                    _db.Warehouses.Remove(entity);
                    _db.SaveChanges();
                    return true;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteWarehouseAsync: " + ex.Message);
            }
        }

        public async Task<List<WarehouseDTO>> GetAllWarehouseByBranchId(int? branchId)
        {
            IQueryable<Warehouse> query = _db.Warehouses;

            // Chỉ lọc khi branchId có giá trị và > 0.
            if (branchId.GetValueOrDefault() > 0)
            {
                query = query.Where(w => w.branch_id == branchId.Value);
            }

            return await query
                .Select(w => new WarehouseDTO
                {
                    WarehouseId = w.warehouse_id,
                    Name = w.name,
                    Address = w.address,
                    BranchId = w.branch_id
                })
                .ToListAsync();
        }

        public async Task<Dictionary<int, WarehouseDTO>> GetWarehouseDictAsync()
        {
            return await _db.Warehouses
                .AsNoTracking()
                .ToDictionaryAsync(
                    w => w.warehouse_id,
                    w => new WarehouseDTO
                    {
                        WarehouseId = w.warehouse_id,
                        BranchId = w.branch_id,
                        Name = w.name,
                    });
        }
    }
}