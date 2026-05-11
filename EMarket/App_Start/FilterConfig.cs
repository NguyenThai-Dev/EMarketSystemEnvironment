using System.Web.Mvc;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;
using EMarket.Filters;
using EMarket.Models;
using SimpleInjector;

namespace EMarket
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(
            GlobalFilterCollection filters,
            Container container)
        {
            filters.Add(new GlobalErrorFilter());
            filters.Add(container.GetInstance<RequireLoginFilter>());

            filters.Add(new RateLimitFilter());

            filters.Add(new HandleErrorAttribute());
        }
    }

    public class GlobalErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled) return;

            var ex = filterContext.Exception;

            // Nếu Interceptor đã log rồi thì mình bỏ qua hoặc chỉ log nhẹ
            if (ex.Data.Contains("IsLogged")) return;

            // Lấy dispatcher từ Container (vì Filter thường khởi tạo ở Global.asax)
            var dispatcher = GlobalContainer.Container.GetInstance<IEventDispatcher>();

            _ = dispatcher.DispatchAsync(new AppLogEvent
            {
                LogLevel = "CRITICAL",
                Logger = filterContext.Controller.GetType().Name,
                Message = $"https://www.merriam-webster.com/dictionary/error 500 tại: {filterContext.HttpContext.Request.RawUrl}",
                Exception = ex.ToString()
            });
        }
    }
}