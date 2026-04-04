using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.UserModule.Services.Implementations
{
    public class BranchService : IBranchService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IWarehouseService _warehouseService;

        public BranchService(EMarket_DBEntities db, IWarehouseService warehouseService)
        {
            _db = db;
            _warehouseService = warehouseService;
        }

        public async Task<List<BranchDTO>> GetAllBranchesAsync()
        {
            return await _db.Branches
                .Select(x => new BranchDTO
                {
                    BranchId = x.branch_id,
                    Name = x.name,
                    Address = x.address,
                    AddressUrl = x.address_url,
                    Latitude = x.latitude,
                    Longitude = x.longitude,
                    CreatedAt = x.created_at,
                    UpdatedAt = x.updated_at
                })
                .ToListAsync();
        }

        public async Task<BranchDTO> GetBranchByIdAsync(int id)
        {
            return await _db.Branches
                .Where(x => x.branch_id == id)
                .Select(x => new BranchDTO
                {
                    BranchId = x.branch_id,
                    Name = x.name,
                    Address = x.address,
                    AddressUrl = x.address_url,
                    Latitude = x.latitude,
                    Longitude = x.longitude,
                    CreatedAt = x.created_at,
                    UpdatedAt = x.updated_at
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<BranchDTO>> GetFilteredBranchesAsync(string branchName)
        {
            var query = _db.Branches.AsQueryable();

            if (!string.IsNullOrWhiteSpace(branchName))
            {
                query = query.Where(x => x.name.Contains(branchName));
            }

            var branches = await query
                .Select(x => new BranchDTO
                {
                    BranchId = x.branch_id,
                    Name = x.name,
                    Address = x.address,
                    AddressUrl = x.address_url,
                    Latitude = x.latitude,
                    Longitude = x.longitude,
                    CreatedAt = x.created_at,
                    UpdatedAt = x.updated_at
                })
                .ToListAsync();

            foreach (var branch in branches)
            {
                var warehouses = await _warehouseService.GetAllWarehouseByBranchId(branch.BranchId);
                branch.CanBeDeleted = warehouses == null || !warehouses.Any();
            }

            return branches;
        }

        public async Task<List<BranchDTO>> GetBranchByIdsAsync(List<int> ids)
        {
            return await _db.Branches
                .Where(x => ids.Contains(x.branch_id))
                .Select(x => new BranchDTO
                {
                    BranchId = x.branch_id,
                    Name = x.name,
                    Address = x.address,
                    AddressUrl = x.address_url,
                    Latitude = x.latitude,
                    Longitude = x.longitude,
                    CreatedAt = x.created_at,
                    UpdatedAt = x.updated_at
                })
                .ToListAsync();
        }

        public async Task<int> CreateBranchAsync(BranchDTO dto)
        {
            try
            {
                var entity = new Branch
                {
                    name = dto.Name,
                    address = dto.Address,
                    address_url = dto.AddressUrl,
                    latitude = dto.Latitude,
                    longitude = dto.Longitude,
                    created_at = DateTime.Now
                };

                _db.Branches.Add(entity);
                await _db.SaveChangesAsync();
                return entity.branch_id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdateBranchAsync(BranchDTO dto)
        {
            try
            {
                var entity = await _db.Branches.FindAsync(dto.BranchId);
                if (entity == null) return false;

                entity.name = dto.Name;
                entity.address = dto.Address;
                entity.address_url = dto.AddressUrl;
                entity.latitude = dto.Latitude;
                entity.longitude = dto.Longitude;
                entity.updated_at = DateTime.Now;

                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            try
            {
                var entity = await _db.Branches.FindAsync(id);
                if (entity == null) return false;

                _db.Branches.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<BranchDTO>> GetNearestBranchAsync(double lat, double lng, double maxDistanceKm = 50)
        {
            // Dùng chính câu lệnh SQL ông vừa test chạy đúng
            string sql = @"
        SELECT TOP 5 
              branch_id AS BranchId,
            Name, 
            Address, 
            Latitude, 
            Longitude,
            geography::Point(Latitude, Longitude, 4326).STDistance(geography::Point(@lat, @lng, 4326)) / 1000 AS Distance
        FROM Branches
        WHERE Latitude IS NOT NULL AND Longitude IS NOT NULL
        AND geography::Point(Latitude, Longitude, 4326).STDistance(geography::Point(@lat, @lng, 4326)) <= (@maxDistance * 1000)
        ORDER BY geography::Point(Latitude, Longitude, 4326).STDistance(geography::Point(@lat, @lng, 4326)) ASC";

            // Thực thi qua EF và map thẳng vào DTO
            var result = await _db.Database.SqlQuery<BranchDTO>(sql,
                new SqlParameter("@lat", lat),
                new SqlParameter("@lng", lng),
                new SqlParameter("@maxDistance", maxDistanceKm)
            ).ToListAsync();

            return result;
        }

        public async Task<Dictionary<int, BranchDTO>> GetBranchDictAsync()
        {
            var branches = await GetAllBranchesAsync();
            return branches.ToDictionary(b => b.BranchId, b => b);
        }
    }
}