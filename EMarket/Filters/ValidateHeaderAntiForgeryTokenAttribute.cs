using System;
using System.Web.Helpers;
using System.Web.Mvc;

namespace EMarket.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidateHeaderAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");

            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];

            // Lấy token từ Header thay vì Form
            var headerToken = httpContext.Request.Headers["RequestVerificationToken"];

            AntiForgery.Validate(cookie != null ? cookie.Value : null, headerToken);
        }
    }
}