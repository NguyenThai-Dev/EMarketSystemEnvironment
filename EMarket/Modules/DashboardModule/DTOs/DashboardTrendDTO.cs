using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class DashboardTrendDTO
    {
        public List<string> Labels { get; set; }
        public List<decimal> Sales { get; set; }
        public List<decimal> Purchases { get; set; }
    }
}