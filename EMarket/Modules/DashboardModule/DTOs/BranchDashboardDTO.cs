namespace EMarket.Modules.DashboardModule.DTOs
{
    public class BranchDashboardDTO
    {
        public decimal SalesAmount { get; set; }
        public decimal PurchaseAmount { get; set; }
        public int LowStockCount { get; set; }
        public int BranchId { get; internal set; }
        public string Name { get; internal set; }
        public decimal Profit { get; internal set; }
    }
}