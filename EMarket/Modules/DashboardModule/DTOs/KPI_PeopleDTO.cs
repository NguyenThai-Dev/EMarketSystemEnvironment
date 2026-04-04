using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class KPI_PeopleDTO
    {
        public int TotalCustomers { get; set; }
        public double CustomerGrowth { get; set; }
        public int VipCount { get; set; }
        public int ActiveEmployees { get; set; }
        public int NewRegistrations { get; set; }
        public List<string> RecentUserAvatars { get; set; }
    }

    public class KpiRawHelper
    {
        public int TotalCustomers { get; set; }
        public int ThisMonthCustomers { get; set; }
        public int LastMonthCustomers { get; set; }
        public int VipCount { get; set; }
        public int NewCustomers { get; set; }
        public int ActiveEmployees { get; set; }
        public int NewUsers { get; set; }
    }

}