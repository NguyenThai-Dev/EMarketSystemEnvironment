using System;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class InventoryDTO
    {
        public int InventoryId { get; set; }
        public int LotId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdate { get; set; }

        public string BranchName { get; set; }
        public string WarehouseName { get; set; }
        public string BatchCode { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public decimal? Price { get; set; }
        public string ProductImage { get; set; }
        public string Unit { get; set; }
    }
}