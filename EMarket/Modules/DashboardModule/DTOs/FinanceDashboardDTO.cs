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

        //Thất thoát hàng tồn kho
        public decimal InventoryLossValue { get; set; }



        // Thuộc tính tính toán: Lợi nhuận sau khi trừ hàng hỏng
        public decimal ActualProfit => GrossProfit - InventoryLossValue;

        // Tỷ lệ thất thoát trên lợi nhuận (%)
        public double LossRate => GrossProfit > 0
            ? (double)(InventoryLossValue / GrossProfit) * 100
            : 0;
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