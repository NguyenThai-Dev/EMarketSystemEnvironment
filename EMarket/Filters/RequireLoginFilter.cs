using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Filters
{
    public class RequireLoginFilter : IAuthorizationFilter
    {
        private readonly IUserContext _userContext;
        private readonly string _secretLoginKey = System.Configuration.ConfigurationManager.AppSettings["SecretLoginKey"];

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
            var sessionSignature = filterContext.HttpContext.Session["SessionSignature"] as string;

            bool isSessionValid = false;

            // 1. Kiểm tra xem User có tồn tại và Session Signature có khớp không
            if (currentUser != null && sessionSignature != null)
            {
                string secretKey = _secretLoginKey;
                string rawData = $"{currentUser.UserId}|{currentUser.Email}|{secretKey}";

                using (var sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                    string computedSignature = Convert.ToBase64String(bytes);

                    if (computedSignature == sessionSignature)
                    {
                        isSessionValid = true;
                    }
                }
            }

            // 2. KIỂM TRA CHÉO VỚI ACCESS TOKEN (Nếu có gửi kèm)
            if (isSessionValid)
            {
                var authHeader = filterContext.HttpContext.Request.Headers["Authorization"];

                // Chỉ check khi request có mang theo Token (VD: gọi Ajax)
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    try
                    {
                        var tokenString = authHeader.Substring("Bearer ".Length).Trim();
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(tokenString);

                        // Lấy UserId từ bên trong cấu trúc của Token
                        var tokenUserIdStr = jwtToken.Claims.FirstOrDefault(c =>
                            c.Type == "nameid" || c.Type == ClaimTypes.NameIdentifier)?.Value;

                        // Nếu ID trong Token KHÁC với ID của Session hiện tại -> Thoát ngay lập tức.
                        if (tokenUserIdStr != currentUser.UserId.ToString())
                        {
                            System.Diagnostics.Debug.WriteLine($"[Security Alert] Phát hiện Token lạ! Session User: {currentUser.UserId}, Token User: {tokenUserIdStr}");
                            isSessionValid = false;
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("[Security Alert] Phát hiện Token không hợp lệ hoặc bị chỉnh sửa cấu trúc.");
                        isSessionValid = false;
                    }
                }
            }

            if (!_userContext.IsAuthenticated || !isSessionValid)
            {
                // Hủy session triệt để nếu phát hiện bất thường
                filterContext.HttpContext.Session.Clear();
                filterContext.HttpContext.Session.Abandon();

                filterContext.HttpContext.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);

                string returnUrl = filterContext.HttpContext.Request.RawUrl;

                // Nếu là request Ajax (có header Token), trả về HTTP 401 để JS tự handle
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new HttpStatusCodeResult(401, "Phiên đăng nhập không hợp lệ hoặc Token bị từ chối.");
                }
                else
                {
                    // Chuyển hướng về trang đăng nhập nếu là chuyển trang bình thường
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
}