using System.Collections.Generic;
using EMarket.Modules.UserModule.DTOs;

namespace EMarket.Modules.UserModule.Services.Interfaces
{
    public interface IUserContext
    {
        // Raw object (khi thật sự cần)
        CurrentUserDTO CurrentUser { get; }

        // Identity
        int UserId { get; }
        string Username { get; }
        string FullName { get; }
        string Email { get; }

        string Image { get; }

        // Context
        int? BranchId { get; }
        int? CurrentBranchId { get; }
        string CurrentBranchName { get; }
        int? SupplierId { get; }

        // Role
        int PrimaryRoleId { get; }
        IReadOnlyList<RoleDTO> Roles { get; }

        // Permission
        IReadOnlyList<PermissionDTO> Permissions { get; }
        bool HasPermission(string moduleName);
        bool HasPermission(string moduleName, string permissionName);

        // Technical flags (UI / routing only)
        bool IsAdmin { get; }
        bool IsSupplier { get; }

        // State
        bool IsAuthenticated { get; }
    }
}
