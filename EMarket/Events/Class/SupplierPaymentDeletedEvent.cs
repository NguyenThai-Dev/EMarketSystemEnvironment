using EMarket.Events.Interfaces;

namespace EMarket.Events.Class
{
    public class SupplierPaymentDeletedEvent : IEvent
    {
        // Event Deleted cần giữ lại thông tin vì DB đã bị xóa
        public string SupplierEmail { get; set; }
        public string SupplierName { get; set; }
        public decimal Amount { get; set; }
        public int PaymentId { get; set; }
    }
}