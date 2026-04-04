using System.Collections.Generic;

namespace EMarket.Modules.UserModule.DTOs
{
    public class CurrentUserDTO
    {
        // Thông tin cơ bản
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }

        // Ngữ cảnh làm việc
        public int? SupplierId { get; set; }
        public int? BranchId { get; set; }

        // Phân quyền
        public int RoleId { get; set; }
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
        public List<PermissionDTO> Permissions { get; set; } = new List<PermissionDTO>();

        // Phân loại kỹ thuật (không dùng để check quyền)
        public bool IsAdmin { get; set; }
        public bool IsSupplier { get; set; }
    }

}