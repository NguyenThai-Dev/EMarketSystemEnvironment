using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMarket.Modules.SalesModule.DTOs
{
    // Class này dùng để nhận Request từ Frontend (JS gửi lên)
    public class CheckoutRequestDTO
    {
        public int? CustomerId { get; set; } // Nullable vì có thể là khách vãng lai

        // Thông tin điểm
        public int PointsUsed { get; set; } = 0;   // Điểm khách muốn trừ
        public int PointsEarned { get; set; } = 0; // Điểm hệ thống tính toán cho khách

        public decimal TotalAmount { get; set; } // Tổng tiền sau khi trừ hết khuyến mãi

        public decimal ManualDiscount { get; set; } // Giảm giá thủ công
        public string ManualDiscountReason { get; set; } // Lý do
        public int? ParentOrderId { get; set; }
        public string Status { get; set; } // Trạng thái đơn hàng khi tạo (Mặc định: "Completed")
        public string PaymentMethod { get; set; } // Phương thức thanh toán
        public string ConnectionId { get; set; } // ConnectionId hiện tại của client

        // Danh sách giỏ hàng
        [Required]
        public List<CartItemDTO> Items { get; set; }
    }

    // Class con cho từng món hàng trong giỏ
    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }   // Giá bán tại thời điểm đó
        public decimal Discount { get; set; } // Chiết khấu trên từng món (nếu có)
    }

    // Class trả về kết quả
    public class CheckoutResultDTO
    {
        public bool Success { get; set; }
        public int OrderId { get; set; }
        public string Message { get; set; }
    }
}