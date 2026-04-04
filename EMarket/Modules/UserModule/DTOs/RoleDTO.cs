using System.Collections.Generic;

namespace EMarket.Modules.UserModule.DTOs
{
    public class RoleDTO
    {
        public int RoleId { get; set; }
        public string Name { get; set; }

        public List<PermissionDTO> Permissions { get; set; } = new List<PermissionDTO>();
    }

    public class RolePermissionUpdateDTO
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}