using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.UserModule.Services.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly EMarket_DBEntities _db;

        public PermissionService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<PermissionDTO>> GetAllPermissionsAsync()
        {
            return await _db.Permissions
                .Select(x => new PermissionDTO
                {
                    PermissionId = x.permission_id,
                    Name = x.name,
                    Module = x.module
                })
                .ToListAsync();
        }

        public async Task<PermissionDTO> GetPermissionByIdAsync(int id)
        {
            return await _db.Permissions
                .Where(x => x.permission_id == id)
                .Select(x => new PermissionDTO
                {
                    PermissionId = x.permission_id,
                    Name = x.name,
                    Module = x.module
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreatePermissionAsync(PermissionDTO dto)
        {
            try
            {
                var entity = new Permission
                {
                    name = dto.Name,
                    module = dto.Module
                };

                _db.Permissions.Add(entity);
                await _db.SaveChangesAsync();
                return entity.permission_id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdatePermissionAsync(PermissionDTO dto)
        {
            try
            {
                var entity = await _db.Permissions.FindAsync(dto.PermissionId);
                if (entity == null) return false;

                entity.name = dto.Name;
                entity.module = dto.Module;

                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            try
            {
                var entity = await _db.Permissions.FindAsync(id);
                if (entity == null) return false;

                _db.Permissions.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}