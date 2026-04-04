using System;

namespace EMarket.Modules.UserModule.DTOs
{
    public class BranchDTO
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AddressUrl { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool CanBeDeleted { get; set; }
        public double? Distance { get; set; }
    }
}