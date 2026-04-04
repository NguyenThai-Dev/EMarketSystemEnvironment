using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace EMarket.Hubs
{
    [HubName("orderHub")]
    public class OrderHub : Hub
    {
        public override Task OnReconnected()
        {
            JoinGroups();
            return base.OnReconnected();
        }

        public override Task OnConnected()
        {
            JoinGroups();
            return base.OnConnected();
        }

        private void JoinGroups()
        {
            var branchIdStr = Context.QueryString["branchId"];
            var isAdminStr = Context.QueryString["isAdmin"];

            if (!string.IsNullOrEmpty(isAdminStr) &&
                isAdminStr.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine($"=== ADD TO ADMIN_ALL GROUP ===");
                Groups.Add(Context.ConnectionId, "ADMIN_ALL");
            }

            if (int.TryParse(branchIdStr, out int branchId) && branchId > 0)
            {
                Debug.WriteLine($"=== ADD TO BRANCH_{branchId} GROUP ===");
                Groups.Add(Context.ConnectionId, $"BRANCH_{branchId}");
            }
        }

        public void CheckMyGroups(string branchId, bool isAdmin)
        {
            var connectionId = Context.ConnectionId;
            var groupName = "BRANCH_" + branchId;

            System.Diagnostics.Debug.WriteLine($"=== CHECK GROUP ===");
            System.Diagnostics.Debug.WriteLine($"Connection: {connectionId}");
            System.Diagnostics.Debug.WriteLine($"Target Group: {groupName}");
            System.Diagnostics.Debug.WriteLine($"Is Admin: {isAdmin}");

            Clients.Group(groupName).orderChanged(new
            {
                message = $"Xác nhận: Connection {connectionId} ĐÃ nằm trong group {groupName}",
                isTest = true
            });

            if (isAdmin)
            {
                Clients.Group("ADMIN_ALL").orderChanged(new
                {
                    message = "Xác nhận: Bạn cũng nằm trong group ADMIN_ALL",
                    isTest = true
                });
            }
        }

        public void SendTest()
        {
            Clients.All.orderChanged(new { status = "From Hub" });
        }

        public void WhoAmI()
        {
            var branchId = Context.QueryString["branchId"];
            var isAdmin = Context.QueryString["isAdmin"];
            Clients.Caller.orderChanged(new { message = $"Bạn là: {isAdmin}, chi nhánh: {branchId}" });
        }

        public void TriggerTestSignalR(int branchId)
        {
            var branchGroupName = $"BRANCH_{branchId}";
            // Giả lập một payload giống hệt lúc checkout
            var payload = new
            {
                orderId = 9999,
                status = "TEST_SIGNALR",
                branchId = branchId,
                serverTime = DateTime.Now.ToString("HH:mm:ss")
            };

            // Bắn cho Group chi nhánh
            Clients.Group($"BRANCH_{branchId}").orderChanged(payload);

            // Bắn cho Admin
            Clients.Group("ADMIN_ALL").orderChanged(payload);

            Debug.WriteLine($"---> Console Test: Đã bắn tín hiệu test cho Branch {branchId} và Admin");
            Clients.Caller.orderChanged(new { message = $"Đã trigger tới group {branchGroupName}" });
        }
    }
}