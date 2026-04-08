namespace EMarket.Modules.DashboardModule.DTOs
{
    public class DashboardSummaryDTO
    {
        public decimal TotalSales { get; set; }
        public decimal TotalPurchase { get; set; }
        public int TotalOrders { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalProducts { get; set; }
        public int TotalWarehouses { get; set; }
        public decimal TotalInventoryQuantity { get; set; }

        // So sánh kỳ trước (%)
        public decimal SalesGrowthPercent { get; set; }
        public decimal PurchaseGrowthPercent { get; set; }

        // BỔ SUNG: Giá trị hàng hóa thất thoát (Hết hạn)
        public decimal InventoryLossValue { get; set; }

        // Tính toán Lợi nhuận gộp và Lợi nhuận thực tế ngay 
        public decimal GrossProfit => TotalSales - TotalPurchase;
        public decimal ActualProfit => GrossProfit - InventoryLossValue;
    }
}