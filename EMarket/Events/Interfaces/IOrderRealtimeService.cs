using System.Threading.Tasks;

namespace EMarket.Events.Interfaces
{
    public interface IOrderRealtimeService
    {
        Task NotifyOrderCreatedAsync(int orderId, string status, int? branchId, string excludedId = null);
        Task NotifyOrderStatusChangedAsync(int orderId, string status, int? branchId, string excludedId = null);
    }

}
