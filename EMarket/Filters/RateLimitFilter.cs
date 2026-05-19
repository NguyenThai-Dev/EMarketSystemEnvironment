using System;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Caching;

namespace EMarket.Filters
{
    public class RateLimitFilter : IActionFilter
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;
        private const int RequestLimit = 30; // 30 requests
        private const int TimeWindowSeconds = 10; // trong 10 giây

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            // Bỏ qua nếu là các file tĩnh (CSS, JS)
            string path = request.Url.AbsolutePath.ToLower();
            if (path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".ico"))
            {
                return;
            }

            string ip = request.UserHostAddress;
            string actionName = filterContext.ActionDescriptor.ActionName;

            // Bỏ qua API lấy log hệ thống vì SignalR có thể trigger liên tục
            if (actionName.Equals("GetLatestSystemEvents", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            string cacheKey = $"GlobalRate_{ip}_{actionName}";

            var requestCount = Cache[cacheKey] as int?;

            if (requestCount == null)
            {
                Cache.Set(cacheKey, 1, DateTimeOffset.UtcNow.AddSeconds(TimeWindowSeconds));
            }
            else
            {
                if (requestCount >= RequestLimit)
                {
                    filterContext.HttpContext.Response.StatusCode = 429; // Quá tải
                    filterContext.Result = new JsonResult
                    {
                        Data = new { success = false, message = "Hệ thống đang bận, vui lòng chờ chút nhé!" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    return;
                }

                Cache[cacheKey] = requestCount.Value + 1;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) { }
    }
}