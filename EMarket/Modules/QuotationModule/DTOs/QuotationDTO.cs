using System;
using System.Collections.Generic;

namespace EMarket.Modules.QuotationModule.DTOs
{
    public class QuotationDTO
    {
        public int QuotationId { get; set; }
        public string QuotationCode { get; set; } // BG-2024...

        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }

        public int UserId { get; set; }
        public string CreatorName { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(7); // Mặc định 7 ngày

        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ManualDiscount { get; set; } // Giảm giá chung
        public string DiscountReason { get; set; }

        // Thuộc tính tính toán
        public decimal FinalAmount => TotalAmount - DiscountAmount;

        public string Status { get; set; } // Draft, Sent, Accepted, Converted
        public string Note { get; set; }
        public int? ConvertedOrderId { get; set; }

        // Chi tiết báo giá
        public List<QuotationDetailDTO> Details { get; set; } = new List<QuotationDetailDTO>();
    }

    public class QuotationDetailDTO
    {
        public int DetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Unit { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Giá báo
        public decimal Discount { get; set; }  // Giảm giá từng món
        public decimal TotalPrice { get; set; } // Thành tiền line này
        public string Note { get; set; }
    }

    public class QuotationConvertResult
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
    }

    public static class QuotationStatus
    {
        public const string Draft = "Draft";
        public const string Sent = "Sent";
        public const string Accepted = "Accepted";
        public const string Converted = "Converted";
        public const string Cancelled = "Cancelled";
    }

}