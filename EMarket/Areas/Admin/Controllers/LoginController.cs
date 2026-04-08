using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMarket.Helpers;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Enums;
using EMarket.Modules.UserModule.Services.Interfaces;
using Microsoft.Owin.Security;

namespace EMarket.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _service;
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

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
                    Response.Cookies["UserId"].Value = result.User.UserId.ToString();
                    Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(1);

                    return Json(new
                    {
                        success = true,
                        message = "Đăng nhập thành công.",
                        data = new
                        {
                            result.User.UserId,
                            result.User.FullName,
                            result.User.Roles,
                            result.User.Permissions,
                        }
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
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies[".EMarket.Auth"] != null)
            {
                var cookie = new HttpCookie(".EMarket.Auth");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
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

                //Response.Cookies["UserId"].Value = result.User.UserId.ToString();
                //Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(1);

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
    }
}
