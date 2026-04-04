using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EMarket.Events.Interfaces;
using EMarket.Hubs;
using Microsoft.AspNet.SignalR;

namespace EMarket.Events.Implementations
{
    public class OrderRealtimeService : IOrderRealtimeService
    {
        private readonly IHubContext _hub;

        public OrderRealtimeService()
        {
            _hub = GlobalHost.ConnectionManager.GetHubContext<OrderHub>();
        }

        public async Task NotifyOrderCreatedAsync(int orderId, string status, int? branchId, string excludedId = null)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext("orderHub");

            var payload = new
            {
                orderId = orderId,
                status = status,
                branchId = branchId,
                serverTime = DateTime.Now.ToString("HH:mm:ss")
            };

            await Task.Run(() =>
            {
                if (branchId.HasValue && branchId > 0)
                {
                    var branchGroupName = $"BRANCH_{branchId}";
                    Debug.WriteLine($"---> SignalR: Gửi tới {branchGroupName}. Loại trừ: {excludedId ?? "None"}");

                    context.Clients.Group(branchGroupName, excludedId).orderChanged(payload);
                }

                Debug.WriteLine($"---> SignalR: Gửi tới ADMIN_ALL. Loại trừ: {excludedId ?? "None"}");
                context.Clients.Group("ADMIN_ALL", excludedId).orderChanged(payload);
            });
        }

        public Task NotifyOrderStatusChangedAsync(int orderId, string status, int? branchId, string excludedId = null)
        {
            return NotifyOrderCreatedAsync(orderId, status, branchId, excludedId);
        }

        private string GetBranchGroup(int branchId)
        {
            return $"BRANCH_{branchId}";
        }
    }
}