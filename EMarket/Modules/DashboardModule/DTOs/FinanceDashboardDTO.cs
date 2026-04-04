using System;
using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class FinanceDashboardDTO
    {
        public FinanceKpiDTO Kpi { get; set; }
        public List<FinanceDailyTrendDTO> Trends { get; set; }
        public List<FinanceExpensePieDTO> ExpensePie { get; set; }
        public List<RecentOrderDTO> RecentOrders { get; set; }
    }

    public class FinanceKpiDTO
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalPurchase { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal SupplierDebt { get; set; }
    }

    public class FinanceDailyTrendDTO
    {
        public string DateLabel { get; set; }
        public decimal Revenue { get; set; }
        public decimal PurchaseCost { get; set; }
    }

    public class RecentOrderDTO
    {
        public string OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class FinanceExpensePieDTO
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
    }

}