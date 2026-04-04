using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.DashboardModule.DTOs;

namespace EMarket.Modules.DashboardModule.Servcie.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDTO> GetSummaryAsync(int? branchId);
        Task<List<BranchDashboardDTO>> GetBranchPerformanceAsync(int? branchId,
            DateTime fromDate,
            DateTime toDate);
        Task<List<ChartItemDTO>> GetStockChartAsync(int? branchId);
        Task<DashboardOverviewDTO> GetOverviewAsync(
            int? branchId,
            DateTime fromDate,
            DateTime toDate,
            string groupBy // "day" | "month"
        );

        Task<PeopleDashboardDTO> GetPeopleDashboardAsync();
        Task<WarehouseDashboardViewModel> GetWarehouseDashboardAsync(int dayBacks, int? branchId, int? warehouseId);
        Task<FinanceDashboardDTO> GetFinanceDashboardAsync(int daysBack, int? branchId);
        Task<DebtDashboardDto> GetDebtDashboardAsync(
   int? branchId,
   int? supplierId,
   DateTime? fromDate,
   DateTime? toDate);

        Task<AdminHubDataDTO> GetSuperAdminHubData();
    }
}
