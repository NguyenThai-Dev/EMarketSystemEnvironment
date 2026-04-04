using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.UserModule.DTOs;

namespace EMarket.Modules.UserModule.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<PermissionDTO>> GetAllPermissionsAsync();
        Task<PermissionDTO> GetPermissionByIdAsync(int id);
        Task<int> CreatePermissionAsync(PermissionDTO dto);
        Task<bool> UpdatePermissionAsync(PermissionDTO dto);
        Task<bool> DeletePermissionAsync(int id);
    }
}
