using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace EMarket.ApiControllers.Admin
{
    [AllowAnonymous]
    [RoutePrefix("api/admin/user-management")]
    public class UserAdminApiController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly ILoginService _loginService;
        private readonly string _secretKeyForJwt = System.Configuration.ConfigurationManager.AppSettings["SecretKeyForJwt"];

        public UserAdminApiController(
            IUserService userService, IBranchService branchService,
            IRoleService roleService, IPermissionService permissionService,
            ILoginService loginService)
        {
            _userService = userService;
            _branchService = branchService;
            _roleService = roleService;
            _permissionService = permissionService;
            _loginService = loginService;
        }

        private string GenerateJwtToken(CurrentUserDTO user)
        {
            if (string.IsNullOrEmpty(_secretKeyForJwt))
            {
                return Guid.NewGuid().ToString("N");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKeyForJwt);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? user.Username ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                }),
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

        #region Authentication

        [HttpPost, Route("auth/login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.EmailOrUsername) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Vui lòng nhập đầy đủ thông tin đăng nhập.");

            var result = await _loginService.LoginAsync(model.EmailOrUsername.Trim(), model.Password);
            if (result.Status == EMarket.Modules.UserModule.Enums.LoginStatus.Success)
            {
                string token = GenerateJwtToken(result.User);
                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập thành công.",
                    token = token,
                    user = result.User
                });
            }
            if (result.Status == EMarket.Modules.UserModule.Enums.LoginStatus.Locked)
            {
                return Ok(new { success = false, message = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị." });
            }
            return Ok(new { success = false, message = "Sai tài khoản hoặc mật khẩu." });
        }

        [HttpPost] 
        [Route("auth/external-login")]
        [Route("auth/google-login")]
        public async Task<IHttpActionResult> ExternalLogin([FromBody] ExternalLoginRequestDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Email không được để trống.");

            var result = await _loginService.LoginByEmailAsync(model.Email.Trim());
            if (result.Status == EMarket.Modules.UserModule.Enums.LoginStatus.Success)
            {
                string token = GenerateJwtToken(result.User);
                return Ok(new
                {
                    success = true,
                    message = "Đăng nhập Google thành công.",
                    token = token,
                    user = result.User
                });
            }
            if (result.Status == EMarket.Modules.UserModule.Enums.LoginStatus.Locked)
            {
                return Ok(new { success = false, message = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị." });
            }
            return Ok(new { success = false, message = "Email Google này chưa được cấp quyền truy cập EMarket." });
        }

        [HttpPost, Route("auth/request-otp")]
        public async Task<IHttpActionResult> RequestOtp([FromBody] RequestOtpModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Email không được để trống.");
            await _loginService.RequestOtpAsync(model.Email);
            return Ok(new { success = true, message = "OTP đã được gửi về email." });
        }

        public class RequestOtpModel
        {
            public string Email { get; set; }
        }

        [HttpPost, Route("auth/reset-password")]
        public async Task<IHttpActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO model)
        {
            if (model == null) return BadRequest("Dữ liệu reset password không hợp lệ.");
            await _loginService.ResetPasswordAsync(model.Email, model.Otp, model.NewPassword);
            return Ok(new { success = true, message = "Mật khẩu đã được cập nhật thành công." });
        }

        #endregion

        #region Users

        [HttpGet, Route("users")]
        public async Task<IHttpActionResult> GetAllUsers()
        { return Ok(await _userService.GetAllUsersAsync()); }

        [HttpGet]
        [Route("users/search")]
        [Route("users/paged")]
        public async Task<IHttpActionResult> SearchUsers(int start = 0, int length = 10, string keyword = null)
        {
            var pageSize = Math.Min(length > 0 ? length : 10, 100);
            var all = await _userService.GetFilteredUsersAsync(keyword ?? "");
            var totalCount = all.Count;
            var pagedData = all.Skip(start).Take(pageSize).ToList();
            return Ok(new { total = totalCount, filtered = totalCount, data = pagedData });
        }

        [HttpGet, Route("users/{id:int}")]
        public async Task<IHttpActionResult> GetUserById(int id)
        {
            var d = await _userService.GetUserByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("users/by-ids")]
        public async Task<IHttpActionResult> GetUsersByIds([FromBody] List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0) return BadRequest("userIds required.");
            return Ok(await _userService.GetUsersByUserIdsAsync(userIds));
        }

        [HttpGet, Route("users/dict")]
        public async Task<IHttpActionResult> GetUserDict()
        { return Ok(await _userService.GetUserDictAsync()); }

        [HttpGet, Route("users/stats")]
        public async Task<IHttpActionResult> GetUserStats()
        {
            var total = await _userService.CountAllAsync();
            var active = await _userService.CountActiveUsersAsync();
            var roleStats = await _userService.GetRoleStatisticsAsync();
            var growth = await _userService.GetUsersCreatedByMonthAsync();
            return Ok(new { Total = total, Active = active, RoleDistribution = roleStats, MonthlyGrowth = growth });
        }

        [HttpGet, Route("users/count-new")]
        public async Task<IHttpActionResult> CountNewUsers(DateTime? fromDate = null)
        {
            var from = fromDate ?? DateTime.Today.AddDays(-30);
            return Ok(await _userService.CountCreatedFromAsync(from));
        }

        [HttpGet, Route("users/recent-avatars")]
        public async Task<IHttpActionResult> GetRecentAvatars(int top = 5)
        { return Ok(await _userService.GetRecentActiveUserAvatarsAsync(top)); }

        [HttpGet, Route("users/warehouse-managers-emails")]
        public async Task<IHttpActionResult> GetWarehouseManagerEmails()
        { return Ok(await _userService.GetWarehouseManagerEmailsAsync()); }

        [HttpPost, Route("users/create")]
        public async Task<IHttpActionResult> CreateUser([FromBody] CurrentUserDTO dto)
        {
            if (dto == null) return BadRequest("User data required.");
            var id = await _userService.CreateUserAsync(dto, null);
            return Ok(new { success = id > 0, userId = id });
        }

        [HttpPost, Route("users/update")]
        [HttpPut, Route("users/{id:int}")]
        public async Task<IHttpActionResult> UpdateUser([FromBody] CurrentUserDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("User data required.");
            if (id.HasValue && dto.UserId <= 0) dto.UserId = id.Value;
            var success = await _userService.UpdateUserAsync(dto, null);
            return Ok(new { success });
        }

        [HttpPost]
        [Route("users/upload-avatar")]
        [Route("users/{id:int}/avatar")]
        public async Task<IHttpActionResult> UploadAvatar(int? id = null)
        {
            var httpContext = System.Web.HttpContext.Current;
            if (httpContext == null || httpContext.Request.Files.Count == 0)
                return BadRequest("Vui lòng chọn file ảnh để tải lên.");

            var file = httpContext.Request.Files[0];
            if (file == null || file.ContentLength == 0)
                return BadRequest("File không hợp lệ.");

            int targetUserId = id ?? (int.TryParse(httpContext.Request.Form["userId"], out int uId) ? uId : 0);
            string subFolder = targetUserId > 0 ? targetUserId.ToString() : "temp";
            string folderRelativePath = $"/Uploads/Users/{subFolder}/";
            string folderMapPath = httpContext.Server.MapPath("~" + folderRelativePath);

            if (!System.IO.Directory.Exists(folderMapPath))
                System.IO.Directory.CreateDirectory(folderMapPath);

            string ext = System.IO.Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext)) ext = ".jpg";
            string fileName = $"{Guid.NewGuid()}{ext}";
            string fullPath = System.IO.Path.Combine(folderMapPath, fileName);
            file.SaveAs(fullPath);

            string relativeUrl = folderRelativePath + fileName;

            if (targetUserId > 0)
            {
                var user = await _userService.GetUserByIdAsync(targetUserId);
                if (user != null)
                {
                    user.Image = relativeUrl;
                    await _userService.UpdateUserAsync(user, null);
                }
            }

            return Ok(new { success = true, url = relativeUrl, Image = relativeUrl });
        }

        [HttpPost, Route("users/delete/{id:int}")]
        [HttpDelete, Route("users/{id:int}")]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            return Ok(new { success });
        }

        public class UpdateEmailModel
        {
            public int UserId { get; set; }
            public string NewEmail { get; set; }
        }

        [HttpPost, Route("users/update-email")]
        public async Task<IHttpActionResult> UpdateUserEmail([FromBody] UpdateEmailModel model)
        {
            if (model == null) return BadRequest("Email update data required.");
            var success = await _userService.UpdateUserEmailAsync(model.UserId, model.NewEmail);
            return Ok(new { success });
        }

        public class VerifyChangeEmailModel
        {
            public int UserId { get; set; }
            public string CurrentEmail { get; set; }
            public string Otp { get; set; }
            public string NewEmail { get; set; }
        }

        [HttpPost, Route("users/verify-change-email")]
        public async Task<IHttpActionResult> VerifyAndChangeEmail([FromBody] VerifyChangeEmailModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.NewEmail) || string.IsNullOrWhiteSpace(model.Otp))
                return BadRequest("Thiếu thông tin OTP hoặc Email mới.");

            var currentEmail = model.CurrentEmail;
            if (string.IsNullOrEmpty(currentEmail))
            {
                var user = await _userService.GetUserByIdAsync(model.UserId);
                if (user == null) return NotFound();
                currentEmail = user.Email;
            }

            bool isValid = await _loginService.VerifyOtpAsync(currentEmail, model.Otp);
            if (!isValid)
                return Ok(new { success = false, message = "Mã OTP không hợp lệ hoặc đã hết hạn." });

            var updated = await _userService.UpdateUserEmailAsync(model.UserId, model.NewEmail);
            return Ok(new { success = updated, message = updated ? "Cập nhật Email thành công!" : "Không thể cập nhật Email." });
        }

        #endregion

        #region Branches

        [HttpGet, Route("branches")]
        public async Task<IHttpActionResult> GetAllBranches()
        { return Ok(await _branchService.GetAllBranchesAsync()); }

        [HttpGet, Route("branches/search")]
        public async Task<IHttpActionResult> SearchBranches(string name = null, double? lat = null, double? lng = null, double maxDist = 10)
        {
            if (lat.HasValue && lng.HasValue && lat > 0 && lng > 0)
                return Ok(await _branchService.GetNearestBranchAsync(lat.Value, lng.Value, maxDist));
            return Ok(await _branchService.GetFilteredBranchesAsync(name ?? ""));
        }

        [HttpGet, Route("branches/nearest")]
        public async Task<IHttpActionResult> GetNearestBranch(double lat, double lng, double maxDist = 100)
        {
            var branch = await _branchService.GetNearestBranchAsync(lat, lng, maxDist);
            if (branch == null) return NotFound();
            return Ok(branch);
        }

        [HttpGet, Route("branches/{id:int}")]
        public async Task<IHttpActionResult> GetBranchById(int id)
        {
            var d = await _branchService.GetBranchByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("branches/by-ids")]
        public async Task<IHttpActionResult> GetBranchesByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("ids required.");
            return Ok(await _branchService.GetBranchByIdsAsync(ids));
        }

        [HttpGet, Route("branches/dict")]
        public async Task<IHttpActionResult> GetBranchDict()
        { return Ok(await _branchService.GetBranchDictAsync()); }

        [HttpPost, Route("branches/create")]
        public async Task<IHttpActionResult> CreateBranch([FromBody] BranchDTO dto)
        {
            if (dto == null) return BadRequest("Branch data required.");
            var id = await _branchService.CreateBranchAsync(dto);
            return Ok(new { success = id > 0, branchId = id });
        }

        [HttpPost, Route("branches/update")]
        [HttpPut, Route("branches/{id:int}")]
        public async Task<IHttpActionResult> UpdateBranch([FromBody] BranchDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Branch data required.");
            if (id.HasValue && dto.BranchId <= 0) dto.BranchId = id.Value;
            var success = await _branchService.UpdateBranchAsync(dto);
            return Ok(new { success });
        }

        [HttpPost, Route("branches/delete/{id:int}")]
        [HttpDelete, Route("branches/{id:int}")]
        public async Task<IHttpActionResult> DeleteBranch(int id)
        {
            var success = await _branchService.DeleteBranchAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Roles & Permissions

        [HttpGet, Route("roles")]
        public async Task<IHttpActionResult> GetAllRoles()
        { return Ok(await _roleService.GetAllRolesAsync()); }

        [HttpGet, Route("roles/{id:int}")]
        public async Task<IHttpActionResult> GetRoleById(int id)
        {
            var d = await _roleService.GetRoleByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpGet, Route("roles/{id:int}/permissions")]
        public async Task<IHttpActionResult> GetRolePermissions(int id)
        { return Ok(await _roleService.GetRolePermissionByRoleId(id)); }

        [HttpPost, Route("roles/create")]
        public async Task<IHttpActionResult> CreateRole([FromBody] RoleDTO dto)
        {
            if (dto == null) return BadRequest("Role data required.");
            var id = await _roleService.CreateRoleAsync(dto);
            return Ok(new { success = id > 0, roleId = id });
        }

        [HttpPost]
        [Route("roles/update-permissions")]          
        [Route("roles/{roleId:int?}/permissions")]
        public async Task<IHttpActionResult> UpdateRolePermissions([FromBody] RolePermissionUpdateDTO model, int? roleId = null)
        {
            if (model == null) return BadRequest("Role permission data required.");
            if (roleId.HasValue && model.RoleId <= 0) model.RoleId = roleId.Value;
            var success = await _roleService.UpdateRolePermissionsAsync(model);
            return Ok(new { success });
        }

        [HttpPost, Route("roles/delete/{id:int}")]
        [HttpDelete, Route("roles/{id:int}")]
        public async Task<IHttpActionResult> DeleteRole(int id)
        {
            var success = await _roleService.DeleteRoleAsync(id);
            return Ok(new { success });
        }

        [HttpGet, Route("permissions")]
        public async Task<IHttpActionResult> GetAllPermissions()
        { return Ok(await _permissionService.GetAllPermissionsAsync()); }

        [HttpGet, Route("permissions/{id:int}")]
        public async Task<IHttpActionResult> GetPermissionById(int id)
        {
            var d = await _permissionService.GetPermissionByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        #endregion
    }
}