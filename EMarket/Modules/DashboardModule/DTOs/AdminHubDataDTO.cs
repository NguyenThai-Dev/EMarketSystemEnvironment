using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class AdminHubDataDTO
    {
        public HubMetricsDTO Metrics { get; set; }
        public List<HubAlertDTO> Alerts { get; set; }
    }

    public class HubMetricsDTO
    {
        public decimal RevenueCurrent { get; set; }
        public decimal RevenueGrowth { get; set; } // % tăng trưởng
        public decimal TotalDebt { get; set; }
        public decimal TotalStockValue { get; set; }
        public int ActiveStaffCount { get; set; }
        public int CriticalAlerts { get; set; }
    }

    public class HubAlertDTO
    {
        public string Type { get; set; } // "DEBT" hoặc "STOCK"
        public string Title { get; set; }
        public string Message { get; set; }
    }
}