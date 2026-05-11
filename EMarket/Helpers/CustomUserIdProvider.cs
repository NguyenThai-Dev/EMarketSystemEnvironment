using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System.Security.Claims;

namespace EMarket.Helpers
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            // Lấy trực tiếp từ Identity của User đã qua Middleware xác thực
            if (request.User?.Identity is ClaimsIdentity identity)
            {
                // Tìm đúng cái Claim chứa ID (thường là NameIdentifier bạn set lúc Login)
                var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
            }
            return null;
        }
    }
}