using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IBranchService _branchService;
        private readonly ISupplierService _supplierService;
        private readonly IUserContext _userContext;
        private readonly ILoginService _loginService;
        public UserController(IUserService userService, IRoleService roleService, IPermissionService permissionService, IBranchService branchService, ISupplierService supplierService, IUserContext userContext, ILoginService loginService)
        {
            _userService = userService;
            _roleService = roleService;
            _permissionService = permissionService;
            _branchService = branchService;
            _supplierService = supplierService;
            _userContext = userContext;
            _loginService = loginService;
        }
        // GET: Admin/User
        [EMarketAuthorize(Module = "UserModule")]
        public ActionResult UserList()
        {
            return View();
        }

        public ActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAllUsers()
        {
            var data = await _userService.GetAllUsersAsync();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetFilteredUsers(string keyword)
        {
            var data = await _userService.GetFilteredUsersAsync(keyword);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateUser(CurrentUserDTO dto, HttpPostedFileBase file)
        {
            var newId = await _userService.CreateUserAsync(dto, file);

            return Json(new { success = newId > 0, newId = newId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "UserModule")]
        public async Task<ActionResult> UpdateUser(CurrentUserDTO dto, HttpPostedFileBase file)
        {
            var result = await _userService.UpdateUserAsync(dto, file);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "UserModule")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateMyProfile(CurrentUserDTO dto, HttpPostedFileBase file)
        {
            try
            {
                var currentUserId = _userContext.UserId;
                if (dto.UserId != currentUserId)
                {
                    return Json(new { success = false, message = "Bạn không có quyền sửa thông tin người khác." });
                }


                var oldUser = await _userService.GetUserByIdAsync(currentUserId);

                oldUser.FullName = dto.FullName;
                oldUser.Phone = dto.Phone;
                oldUser.Username = dto.Username;

                var result = await _userService.UpdateUserAsync(oldUser, file);

                if (result)
                {
                    var updatedUser = await _userService.GetUserByIdAsync(currentUserId);

                    Session["CurrentUser"] = updatedUser;
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại. Vui lòng thử lại." });
                }
            }
            catch (Exception ex)
            {
                // Log ex
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> VerifyAndChangeEmail(string otp, string newEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newEmail))
                    return Json(new { success = false, message = "Email mới không được để trống." });

                var currentEmail = _userContext.Email;
                bool isValid = await _loginService.VerifyOtpAsync(currentEmail, otp);

                if (!isValid)
                    return Json(new { success = false, message = "Mã OTP không hợp lệ hoặc đã hết hạn." });

                var userId = _userContext.UserId;
                var isUpdated = await _userService.UpdateUserEmailAsync(userId, newEmail);

                if (!isUpdated)
                    return Json(new { success = false, message = "Không thể cập nhật Email vào hệ thống. Vui lòng thử lại." });

                var updatedUser = await _userService.GetUserByIdAsync(userId);
                Session["CurrentUser"] = updatedUser;

                return Json(new { success = true, message = "Cập nhật Email thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}