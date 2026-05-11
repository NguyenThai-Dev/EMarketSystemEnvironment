using EMarket.Helpers;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Enums;
using EMarket.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EMarket.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _service;
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        private readonly string _secretLoginKey = System.Configuration.ConfigurationManager.AppSettings["SecretLoginKey"];
        private readonly string _secretKeyForJwt = System.Configuration.ConfigurationManager.AppSettings["SecretKeyForJwt"];

        public LoginController(ILoginService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Forgot_Password()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error_404()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCurrentUser()
        {
            var currentUser = Session["CurrentUser"] as CurrentUserDTO;

            if (currentUser == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    currentUser.UserId,
                    currentUser.FullName,
                    currentUser.SupplierId,
                    currentUser.BranchId,
                    currentUser.Permissions,
                    currentUser.IsAdmin,
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(LoginRequestDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.EmailOrUsername) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng nhập đầy đủ thông tin."
                });
            }

            var result = await _service.LoginAsync(
                model.EmailOrUsername.Trim(),
                model.Password
            );

            switch (result.Status)
            {
                case LoginStatus.Success:
                    Session["CurrentUser"] = result.User;

                    string secretKey = _secretLoginKey;
                    string rawData = $"{result.User.UserId}|{result.User.Email}|{secretKey}";

                    using (var sha256 = System.Security.Cryptography.SHA256.Create())
                    {
                        byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                        Session["SessionSignature"] = Convert.ToBase64String(bytes);
                    }

                    // 1. Khởi tạo Claims (Thông tin định danh)
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, result.User.UserId.ToString()),
                            new Claim(ClaimTypes.Name, result.User.FullName),
                            new Claim(ClaimTypes.Email, result.User.Email),
                            new Claim(ClaimTypes.Role, "Admin")
                        };

                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                    // 2. Sign in bằng OWIN
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignIn(new AuthenticationProperties()
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    }, identity);

                    string apiToken = GenerateJwtToken(result.User);

                    return Json(new
                    {
                        success = true,
                        message = "Đăng nhập thành công.",
                        token = apiToken,
                        data = new { result.User.UserId, result.User.FullName }
                    });
                case LoginStatus.Locked:
                    return Json(new
                    {
                        success = false,
                        message = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị hệ thống."
                    });

                default:
                    return Json(new
                    {
                        success = false,
                        message = "Sai tài khoản hoặc mật khẩu."
                    });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RequestOtp(string email)
        {
            try
            {
                await _service.RequestOtpAsync(email);
                return Json(new { success = true, message = "OTP đã được gửi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPassword(ResetPasswordRequestDTO model)
        {
            try
            {
                await _service.ResetPasswordAsync(model.Email, model.Otp, model.NewPassword);
                return Json(new { success = true, message = "Mật khẩu đã được cập nhật." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            // 1. Xóa Session cũ
            Session.Clear();
            Session.Abandon();

            // 2. Đăng xuất OWIN 
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // 3. Xóa cookie chứa JWT token (nếu có sinh ra từ ExternalLogin)
            if (Request.Cookies["access_token"] != null)
            {
                var jwtCookie = new HttpCookie("access_token");
                jwtCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(jwtCookie);
            }

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Login", "Login", new { area = "Admin" })
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Login", new { area = "Admin", returnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            if (loginInfo == null) return RedirectToAction("Login");

            string email = loginInfo.Email;
            var result = await _service.LoginByEmailAsync(email);

            if (result.Status == LoginStatus.Success)
            {
                Session["CurrentUser"] = result.User;

                string secretKey = _secretLoginKey;
                string rawData = $"{result.User.UserId}|{result.User.Email}|{secretKey}";

                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                    Session["SessionSignature"] = Convert.ToBase64String(bytes);
                }

                // 1. Khởi tạo Claims & Đăng nhập bằng OWIN
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.User.UserId.ToString()),
            new Claim(ClaimTypes.Name, result.User.FullName),
            new Claim(ClaimTypes.Email, result.User.Email)
        };
                var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                }, identity);

                // 2. Generate JWT Token
                // Trong ExternalLoginCallback của bạn
                var apiToken = GenerateJwtToken(result.User);

                // Sử dụng OwinContext để ghi Cookie đồng bộ với AuthenticationManager
                var options = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(1),
                    HttpOnly = false, 
                    Path = "/",
                    Secure = Request.IsSecureConnection
                };

                // Ghi đè trực tiếp vào OWIN Context thay vì Response.Cookies
                HttpContext.GetOwinContext().Response.Cookies.Append("access_token", apiToken, options);

                return Redirect(returnUrl ?? "/Admin/Admin/Index");
            }
            else if (result.Status == LoginStatus.Locked)
            {
                TempData["ErrorMessage"] = "Tài khoản đã bị khóa.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Email Google này chưa được cấp quyền truy cập EMarket.";
            return RedirectToAction("Login");
        }

        private string GenerateJwtToken(CurrentUserDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Khóa bí mật (Secret Key) - Đảm bảo chuỗi này dài tối thiểu 256 bits (32 ký tự)
            var key = Encoding.ASCII.GetBytes(_secretKeyForJwt);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                // Token có hạn trong 1 ngày
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "eMarketServer",
                Audience = "eMarketClient"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
