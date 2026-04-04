using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace EMarket.Hubs
{
    [HubName("systemLogHub")]
    public class SystemLogHub : Hub
    {
        // Admin sẽ join vào group này khi mở trang SystemLog
        public void JoinLogGroup()
        {
            Groups.Add(Context.ConnectionId, "ADMIN_LOG_GROUP");
        }
    }
}