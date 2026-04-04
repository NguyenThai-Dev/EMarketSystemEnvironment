using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.UserModule.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly EMarket_DBEntities _db;

        public RoleService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            return await _db.Roles
                .Select(r => new RoleDTO
                {
                    RoleId = r.role_id,
                    Name = r.name,
                    Permissions = r.RolePermissions
                        .Select(p => new PermissionDTO
                        {
                            PermissionId = p.permission_id,
                            Name = p.Permission.name,
                            Module = p.Permission.module
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            return await _db.Roles
                .Where(r => r.role_id == id)
                .Select(r => new RoleDTO
                {
                    RoleId = r.role_id,
                    Name = r.name,
                    Permissions = r.RolePermissions
                        .Select(p => new PermissionDTO
                        {
                            PermissionId = p.permission_id,
                            Name = p.Permission.name,
                            Module = p.Permission.module
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetRolePermissionByRoleId(int roleId)
        {
            bool roleExists = await _db.Roles.AnyAsync(r => r.role_id == roleId);
            if (!roleExists)
                throw new ArgumentException("Role not found");

            return await _db.RolePermissions
                .Where(rp => rp.role_id == roleId)
                .Select(rp => rp.permission_id)
                .ToListAsync();
        }


        public async Task<int> CreateRoleAsync(RoleDTO dto)
        {
            try
            {
                var role = new Role
                {
                    name = dto.Name
                };

                _db.Roles.Add(role);
                await _db.SaveChangesAsync();

                // Insert permissions
                foreach (var perm in dto.Permissions)
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        role_id = role.role_id,
                        permission_id = perm.PermissionId
                    });
                }

                await _db.SaveChangesAsync();
                return role.role_id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdateRolePermissionsAsync(RolePermissionUpdateDTO model)
        {
            try
            {
                var existingPerms = _db.RolePermissions.Where(rp => rp.role_id == model.RoleId);
                _db.RolePermissions.RemoveRange(existingPerms);
                foreach (var permId in model.PermissionIds)
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        role_id = model.RoleId,
                        permission_id = permId
                    });
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            try
            {
                var role = await _db.Roles.FindAsync(id);
                if (role == null) return false;

                var relPerms = _db.RolePermissions.Where(x => x.role_id == id);
                _db.RolePermissions.RemoveRange(relPerms);

                _db.Roles.Remove(role);
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