using System;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class WarehouseDTO
    {
        public int WarehouseId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AddressUrl { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string BranchName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool CanBeDeleted { get; set; }
    }
}