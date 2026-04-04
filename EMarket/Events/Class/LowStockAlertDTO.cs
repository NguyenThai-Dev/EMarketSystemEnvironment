namespace EMarket.Events.Class
{
    public class LowStockAlertDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }

        public int CurrentStock { get; set; }
        public int MinStock { get; set; }

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string RecipientEmail { get; set; }

        // Helper để FE dùng nếu cần
        public decimal StockRatio => MinStock == 0 ? 0 : (decimal)CurrentStock / MinStock;
    }

}