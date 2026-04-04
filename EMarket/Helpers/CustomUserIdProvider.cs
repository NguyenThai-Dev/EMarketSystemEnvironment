using Microsoft.AspNet.SignalR;

namespace EMarket.Helpers
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            if (request.Cookies.ContainsKey("UserId"))
            {
                return request.Cookies["UserId"].Value;
            }
            return null;
        }
    }
}