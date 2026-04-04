using System;
using System.Web;
using System.Web.Mvc;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Filters
{
    public class EMarketAuthorizeAttribute : AuthorizeAttribute
    {
        public string Module { get; set; } // Đổi tên thành số nhiều cho rõ nghĩa
        public bool RequireAdmin { get; set; } = false;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userContext = DependencyResolver.Current.GetService<IUserContext>();
            var sessionUser = httpContext.Session["CurrentUser"];

            // Lấy tên Controller/Action để log cho dễ nhìn
            var routeData = httpContext.Request.RequestContext.RouteData;
            string controller = routeData.Values["controller"]?.ToString();
            string action = routeData.Values["action"]?.ToString();
            string logPrefix = $"[AuthLog - {controller}/{action}] ";

            System.Diagnostics.Debug.WriteLine($"{logPrefix} Bắt đầu check quyền cho Module: {Module ?? "N/A"}, RequireAdmin: {RequireAdmin}");

            // 1. Kiểm tra Login
            if (userContext == null || !userContext.IsAuthenticated || sessionUser == null)
            {
                System.Diagnostics.Debug.WriteLine($"{logPrefix} FAIL: Chưa đăng nhập (ContextNull={userContext == null}, IsAuth={userContext?.IsAuthenticated}, SessionNull={sessionUser == null})");
                return false;
            }

            // 2. Đặc quyền Admin
            if (userContext.IsAdmin)
            {
                System.Diagnostics.Debug.WriteLine($"{logPrefix} PASS: User là Admin (Full quyền)");
                return true;
            }

            // 3. Kiểm tra nếu trang bắt buộc Admin mà User không phải Admin
            if (RequireAdmin)
            {
                System.Diagnostics.Debug.WriteLine($"{logPrefix} FAIL: Trang yêu cầu Admin nhưng User chỉ là Member");
                return false;
            }

            // 4. Kiểm tra Module
            if (!string.IsNullOrEmpty(Module))
            {
                var moduleList = Module.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                System.Diagnostics.Debug.WriteLine($"{logPrefix} Đang check danh sách Module: {Module}");

                foreach (var mod in moduleList)
                {
                    string cleanMod = mod.Trim();
                    if (userContext.HasPermission(cleanMod))
                    {
                        System.Diagnostics.Debug.WriteLine($"{logPrefix} PASS: Khớp quyền tại Module [{cleanMod}]");
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"{logPrefix} Check: Không có quyền [{cleanMod}]");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"{logPrefix} FAIL: Không khớp bất kỳ Module nào trong danh sách");
                return false;
            }

            // 5. Mặc định cho qua nếu chỉ yêu cầu Login
            System.Diagnostics.Debug.WriteLine($"{logPrefix} PASS: Chỉ yêu cầu đăng nhập");
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var userContext = DependencyResolver.Current.GetService<IUserContext>();
            var sessionUser = filterContext.HttpContext.Session["CurrentUser"];

            // Log thêm một lần nữa ở đây để xem tại sao nó nhảy vào else
            System.Diagnostics.Debug.WriteLine($"Handle Request: IsAuth={userContext?.IsAuthenticated}, SessionNull={sessionUser == null}");

            if ((userContext != null && userContext.IsAuthenticated) || sessionUser != null)
            {
                // Ép buộc đá về AccessDenied nếu có bất kỳ dấu hiệu nào là đã login
                filterContext.Result = new RedirectResult("/Admin/Admin/AccessDenied");
            }
            else
            {
                // Chỉ đá về Login khi thực sự không tìm thấy dấu vết đăng nhập nào
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}