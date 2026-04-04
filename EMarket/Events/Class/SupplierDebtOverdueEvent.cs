using System.Collections.Generic;
using EMarket.Events.Interfaces;

namespace EMarket.Events.Class
{
    public class SupplierDebtOverdueEvent : IEvent
    {
        public List<int> DebtIds { get; set; }
        public SupplierDebtOverdueEvent(List<int> debtIds)
        {
            DebtIds = debtIds;
        }
    }
}