using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Modules.UserModule.Services.Implementations
{
    public class UserContext : IUserContext
    {
        private readonly HttpContextBase _httpContext;

        public UserContext(HttpContextBase httpContext)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        // Đừng dùng biến toàn cục lưu HttpContext, hãy dùng get trực tiếp
        public CurrentUserDTO CurrentUser
            => System.Web.HttpContext.Current?.Session?["CurrentUser"] as CurrentUserDTO;

        public int? CurrentBranchId
            => System.Web.HttpContext.Current?.Session?["CurrentBranchId"] as int?;
        public string CurrentBranchName
            => System.Web.HttpContext.Current?.Session?["CurrentBranchName"] as string;

        public bool IsAuthenticated => CurrentUser != null;

        // ===== Identity =====
        public int UserId => CurrentUser?.UserId ?? 0;
        public string Username => CurrentUser?.Username;
        public string FullName => CurrentUser?.FullName;
        public string Email => CurrentUser?.Email;

        public string Image => CurrentUser?.Image;

        // ===== Context =====
        public int? BranchId => CurrentUser?.BranchId;
        public int? SupplierId => CurrentUser?.SupplierId;

        // ===== Role =====
        public int PrimaryRoleId
        {
            get
            {
                if (CurrentUser == null) return 0;

                // 1. Ưu tiên lấy từ danh sách Roles nếu có
                if (CurrentUser.Roles != null && CurrentUser.Roles.Any())
                {
                    return CurrentUser.Roles.First().RoleId;
                }

                return CurrentUser.RoleId;
            }
        }

        public IReadOnlyList<RoleDTO> Roles
      => (CurrentUser?.Roles ?? new List<RoleDTO>()).AsReadOnly();

        public IReadOnlyList<PermissionDTO> Permissions
            => (CurrentUser?.Permissions ?? new List<PermissionDTO>()).AsReadOnly();


        public bool HasPermission(string moduleName)
        {
            // 1. Nếu là Admin, cho qua luôn (God mode)
            if (IsAdmin) return true;

            // 2. Nếu không phải Admin, mới đi check danh sách quyền
            return Permissions.Any(p =>
                p.Module.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
        }

        public bool HasPermission(string moduleName, string permissionName)
        {
            // 1. Nếu là Admin, cho qua luôn
            if (IsAdmin) return true;

            // 2. Check chi tiết
            return Permissions.Any(p =>
                p.Module.Equals(moduleName, StringComparison.OrdinalIgnoreCase)
                && p.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase));
        }

        // ===== Technical flags =====
        public bool IsAdmin => CurrentUser?.IsAdmin == true;
        public bool IsSupplier => CurrentUser?.IsSupplier == true;
    }
}