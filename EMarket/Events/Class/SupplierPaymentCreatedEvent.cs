using EMarket.Events.Interfaces;

namespace EMarket.Events.Class
{
    public class SupplierPaymentCreatedEvent : IEvent
    {
        public int PaymentId { get; set; }
        public SupplierPaymentCreatedEvent(int id) { PaymentId = id; }
    }
}