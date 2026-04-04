using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.UserModule.DTOs;

namespace EMarket.Modules.UserModule.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(int id);
        Task<List<int>> GetRolePermissionByRoleId(int id);
        Task<int> CreateRoleAsync(RoleDTO dto);
        Task<bool> UpdateRolePermissionsAsync(RolePermissionUpdateDTO model);
        Task<bool> DeleteRoleAsync(int id);
    }
}
