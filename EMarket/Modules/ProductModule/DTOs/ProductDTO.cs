using System;

namespace EMarket.Modules.ProductModule.DTOs
{
    public class ProductDTO
    {
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public string Barcode { get; set; }
        public decimal? Price { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public string Image { get; set; }


        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpiredAt { get; set; }

        public decimal OriginalPrice { get; set; } // Giá gốc (để gạch ngang)
        public decimal FinalPrice { get; set; }    // Giá sau giảm (để tính tiền)
        public string PromotionName { get; set; }  // Tên chương trình KM (để hiện tag)
        public decimal DiscountAmount { get; set; } // Số tiền được giảm
    }

}