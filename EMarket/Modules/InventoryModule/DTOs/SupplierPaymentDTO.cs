using System;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class SupplierPaymentDTO
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int DebtId { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentProof { get; set; }

        public string SupplierName { get; set; }
        public string SupplierEmail { get; set; }
        public decimal? UnpaidAmountAfterPayment { get; set; }
    }
}