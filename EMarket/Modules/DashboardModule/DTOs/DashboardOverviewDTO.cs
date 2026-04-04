using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class DashboardOverviewDTO
    {
        public DashboardSummaryDTO Summary { get; set; }
        public DashboardTrendDTO Trend { get; set; }
        public List<BranchDashboardDTO> BranchPerformance { get; set; }
        public List<ChartItemDTO> StockChart { get; set; }
    }
}