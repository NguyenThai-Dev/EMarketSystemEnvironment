using System;

namespace EMarket.Modules.ProductModule.DTOs
{
    public class ProductImageDTO
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}