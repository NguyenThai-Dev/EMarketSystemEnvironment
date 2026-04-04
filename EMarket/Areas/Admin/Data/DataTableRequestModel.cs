using System;

namespace EMarket.Areas.Admin.Data
{
    public class OrderDataTableRequestDTO
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; } // Chuyển sang string
        public DateTime? ToDate { get; set; }   // Chuyển sang string
        public string Keyword { get; set; }
    }
}