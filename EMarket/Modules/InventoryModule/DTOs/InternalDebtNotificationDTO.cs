using System;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class InternalDebtNotificationDTO
    {
        public int DebtId { get; set; }
        public int PurchaseOrderId { get; set; }
        public string SupplierName { get; set; }
        public decimal? UnpaidAmount { get; set; }
        public DateTime? DueDate { get; set; }

        public string RecipientEmail { get; set; } // Email nhân viên (Role 5)
        public string EmployeeName { get; set; }   // Tên nhân viên (hoặc "Bộ phận Kế toán")
        public int OverdueDays { get; set; }
    }
}