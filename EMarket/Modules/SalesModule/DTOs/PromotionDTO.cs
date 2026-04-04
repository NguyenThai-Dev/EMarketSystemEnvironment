using System;

namespace EMarket.Modules.SalesModule.DTOs
{
    public class PromotionDTO
    {
        public int PromotionId { get; set; }
        public string Name { get; set; }
        public string DiscountType { get; set; } // 'Percent' hoặc 'Amount'
        public decimal DiscountValue { get; set; }

        public int? CategoryId { get; set; }      // Null = Áp dụng tất cả (nếu logic cho phép)
        public string CustomerType { get; set; }  // Null = Áp dụng mọi khách

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }

        public string CategoryName { get; set; }
    }
}