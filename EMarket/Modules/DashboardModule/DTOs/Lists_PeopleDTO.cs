using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class Lists_PeopleDTO
    {
        public List<CustomerRowDTO> TopCustomers { get; set; }
        public List<RoleStatItemDTO> RoleStats { get; set; }
    }

    public class CustomerRowDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public string Avatar { get; set; }
    }

    public class RoleStatItemDTO
    {
        public string RoleName { get; set; }
        public int Count { get; set; }
        public int TotalUsers { get; set; }
    }
}