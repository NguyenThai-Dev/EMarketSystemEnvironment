using System;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class SupplierDebtDTO
    {
        public int DebtId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? UnpaidAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string SupplierEmail { get; set; }
    }
}