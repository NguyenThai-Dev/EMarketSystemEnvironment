using System.Web.Mvc;
using System.Web.Routing;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Filters
{
    public class RequireLoginFilter : IAuthorizationFilter
    {
        private readonly IUserContext _userContext;

        public RequireLoginFilter(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            bool skipCheck =
                filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipCheck) return;

            var currentUser = filterContext.HttpContext.Session["CurrentUser"] as CurrentUserDTO;

            if (!_userContext.IsAuthenticated || currentUser == null)
            {
                string returnUrl = filterContext.HttpContext.Request.RawUrl;

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                { "controller", "Login" },
                { "action", "Login" },
                { "returnUrl", returnUrl }
                    }
                );
            }
        }

    }
}