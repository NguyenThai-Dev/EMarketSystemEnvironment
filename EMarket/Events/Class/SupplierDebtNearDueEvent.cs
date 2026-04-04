using System.Collections.Generic;
using EMarket.Events.Interfaces;

namespace EMarket.Events.Class
{
    public class SupplierDebtNearDueEvent : IEvent
    {
        public List<int> DebtIds { get; set; }
        public SupplierDebtNearDueEvent(List<int> debtIds) { DebtIds = debtIds; }
    }
}