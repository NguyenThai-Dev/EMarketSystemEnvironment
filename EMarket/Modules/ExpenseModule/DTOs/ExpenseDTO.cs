using System;
using System.ComponentModel.DataAnnotations;

namespace EMarket.Modules.ExpenseModule.DTOs
{
    public class ExpenseCategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }

    public class ExpenseDTO
    {
        public int ExpenseId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chi nhánh")]
        public int BranchId { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại chi phí")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

        [Required]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền không hợp lệ")]
        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.Now;
        public string Note { get; set; }

        public string RefImage { get; set; } // Đường dẫn ảnh hóa đơn
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Transfer
        public string Status { get; set; } = "Pending"; // Pending, Approved

        public int ApprovedBy { get; set; }
        public string ApproverName { get; set; }
        public DateTime ApprovedAt { get; set; }
        public int RejectedBy { get; set; }
        public string RejectorName { get; set; }
        public DateTime RejectedAt { get; set; }
        public string RejectionReason { get; set; }
    }

    public class UpdateExpenseStatusRequest
    {
        public int ExpenseId { get; set; }
        public string Status { get; set; }      // Approved | Rejected
        public string PaymentMethod { get; set; } // chỉ dùng khi Approved
        public string RejectReason { get; set; } // chỉ dùng khi Rejected
    }


    public static class ExpenseStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }

}