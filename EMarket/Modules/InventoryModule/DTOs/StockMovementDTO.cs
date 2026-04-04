using System;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class StockMovementDTO
    {
        public int MovementId { get; set; }

        // Thông tin Sản phẩm (Lấy từ ProductService)
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Barcode { get; set; } // Nếu có
        public string Reason { get; set; }

        // Thông tin Kho (Lấy từ WarehouseService hoặc Dictionary)
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        // Thông tin Giao dịch
        public string MovementType { get; set; } // Sale, Import, Return...
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }

        // Tham chiếu
        public int? OrderId { get; set; }

        // Thông tin User (Lấy từ UserService)
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int lotId { get; set; }
    }

    public class StockAdjustmentDTO
    {
        public string MovementType { get; set; } // "ADJUSTMENT" hoặc "ISSUE"
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public decimal QuantityChange { get; set; } // Số lượng thay đổi (có thể âm hoặc dương)
        public string Reason { get; set; }
        public int UserId { get; set; } // Lấy từ Session đăng nhập
    }
}