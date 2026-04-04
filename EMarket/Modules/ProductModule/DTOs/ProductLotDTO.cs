using System;

namespace EMarket.Modules.ProductModule.DTOs
{
    public class ProductLotDTO
    {
        public int LotId { get; set; }
        public int ProductId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string BatchCode { get; set; }
        public decimal? CostPrice { get; set; }
        public DateTime? ManufacturingDate { get; set; }
    }
}