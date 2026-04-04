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
    }
}