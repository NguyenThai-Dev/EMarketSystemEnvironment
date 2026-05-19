using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EMarket.Events.Interfaces;
using EMarket.Models;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;
using SimpleInjector.Lifestyles;

namespace EMarket.Modules.UserModule.Services.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IEmailService _emailService;
        private readonly HttpContext _context;


        public LoginService(EMarket_DBEntities db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
            _context = HttpContext.Current;
        }



        private async Task<CurrentUserDTO> BuildCurrentUserDTO(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // =========================
            // 1. Load Roles của User
            // =========================
            var roleRows = await (
                from ur in _db.UserRoles.AsNoTracking()
                join r in _db.Roles.AsNoTracking()
                    on ur.role_id equals r.role_id
                where ur.user_id == user.user_id
                      && ur.status == "Active"
                select new
                {
                    ur.role_id,
                    ur.branch_id,
                    RoleName = r.name
                }
            ).ToListAsync();

            // =========================
            // Không có role
            // =========================
            if (!roleRows.Any())
            {
                return new CurrentUserDTO
                {
                    UserId = user.user_id,
                    Username = user.username,
                    FullName = user.full_name,
                    Email = user.email,
                    Phone = user.phone,
                    Image = user.user_img,
                    Status = user.status,

                    SupplierId = user.supplier_id,
                    BranchId = null,

                    Roles = new List<RoleDTO>(),
                    Permissions = new List<PermissionDTO>(),

                    IsAdmin = false,
                    IsSupplier = user.supplier_id != null
                };
            }

            var roleIds = roleRows
                .Select(x => x.role_id)
                .Distinct()
                .ToList();

            // =========================
            // 2. Load Permissions theo Role
            // =========================
            var permissionRows = await (
                from rp in _db.RolePermissions.AsNoTracking()
                join p in _db.Permissions.AsNoTracking()
                    on rp.permission_id equals p.permission_id
                where roleIds.Contains(rp.role_id)
                select new
                {
                    rp.role_id,
                    PermissionId = p.permission_id,
                    PermissionName = p.name,
                    Module = p.module
                }
            ).ToListAsync();

            // =========================
            // 3. Build Roles + Permissions
            // =========================
            var roles = new List<RoleDTO>();
            var permissionMap = new Dictionary<int, PermissionDTO>();

            foreach (var roleGroup in roleRows
                .GroupBy(x => new { x.role_id, x.RoleName }))
            {
                var roleDto = new RoleDTO
                {
                    RoleId = roleGroup.Key.role_id,
                    Name = roleGroup.Key.RoleName
                };

                foreach (var perm in permissionRows.Where(p => p.role_id == roleDto.RoleId))
                {
                    var permDto = new PermissionDTO
                    {
                        PermissionId = perm.PermissionId,
                        Name = perm.PermissionName,
                        Module = perm.Module
                    };

                    roleDto.Permissions.Add(permDto);

                    // Gom permission unique cho user
                    if (!permissionMap.ContainsKey(perm.PermissionId))
                    {
                        permissionMap.Add(perm.PermissionId, permDto);
                    }
                }

                roles.Add(roleDto);
            }

            // =========================
            // 4. Branch
            // =========================
            int? branchId = roleRows
                .Select(x => x.branch_id)
                .FirstOrDefault(x => x.HasValue);

            // =========================
            // 5. Build CurrentUserDTO
            // =========================
            return new CurrentUserDTO
            {
                UserId = user.user_id,
                Username = user.username,
                FullName = user.full_name,
                Email = user.email,
                Phone = user.phone,
                Image = user.user_img,
                Status = user.status,

                BranchId = branchId,
                SupplierId = user.supplier_id,

                Roles = roles,
                Permissions = permissionMap.Values.ToList(),

                IsAdmin = roles.Any(r =>
                    r.Name.Equals("System Admin", StringComparison.OrdinalIgnoreCase)),

                IsSupplier = user.supplier_id != null
            };
        }



        public async Task<LoginResponseDTO> LoginAsync(string emailOrUsername, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.email == emailOrUsername || x.username == emailOrUsername);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.password_hash))
            {
                return new LoginResponseDTO
                {
                    Status = Enums.LoginStatus.InvalidCredential
                };
            }

            if (user.status != "Active")
            {
                return new LoginResponseDTO
                {
                    Status = Enums.LoginStatus.Locked
                };
            }

            var currentUser = await BuildCurrentUserDTO(user);

            return new LoginResponseDTO
            {
                Status = Enums.LoginStatus.Success,
                User = currentUser
            };
        }

        public async Task<CurrentUserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.user_id == userId);

            if (user == null)
                return null;

            return await BuildCurrentUserDTO(user);
        }

        public async Task<List<CurrentUserDTO>> GetAllUsersByIdsAsync(List<int> ids)
        {
            var users = await _db.Users
                .Where(x => ids.Contains(x.user_id))
                .ToListAsync();

            var result = new List<CurrentUserDTO>();

            foreach (var user in users)
            {
                result.Add(await BuildCurrentUserDTO(user));
            }

            return result;
        }

        public int? GetCurrentUserId()
        {
            var currentUser = _context?.Session?["CurrentUser"] as CurrentUserDTO;
            return currentUser?.UserId;
        }

        public async Task<int> GetCurrentUserBranch()
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
                throw new Exception("Chưa đăng nhập");
            var userRoles = await _db.UserRoles
                .Where(ur => ur.user_id == currentUserId.Value && ur.status == "Active" && ur.branch_id.HasValue)
                .ToListAsync();
            if (!userRoles.Any())
                throw new Exception("Người dùng không có chi nhánh làm việc");
            // Lấy chi nhánh đầu tiên
            return userRoles.First().branch_id.Value;
        }

        public async Task<List<string>> GetEmailsByRoleAsync(int roleId)
        {
            var emails = await (
                from ur in _db.UserRoles.AsNoTracking()
                join u in _db.Users.AsNoTracking()
                    on ur.user_id equals u.user_id
                where ur.role_id == roleId
                      && ur.status == "Active"
                      && u.status == "Active"
                select u.email
            ).Distinct().ToListAsync();
            return emails;
        }

        private async Task SaveOtpAsync(string email, string otp)
        {
            var record = new ForgotPasswordOtp
            {
                Email = email,
                Otp = otp,
                ExpireAt = DateTime.Now.AddMinutes(2)
            };

            _db.ForgotPasswordOtps.Add(record);
            await _db.SaveChangesAsync();
        }

        public async Task RequestOtpAsync(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.email == email);
            if (user == null)
                throw new Exception("Email không tồn tại");

            // Tạo OTP ngẫu nhiên 6 số
            var otp = new Random().Next(100000, 999999).ToString();

            // Lưu DB trước (nhanh) — không block bởi SMTP
            await SaveOtpAsync(email, otp);

            // Gửi email qua SmtpEmailService (fire-and-forget)
            // Tạo AsyncScope mới để tránh DbContext disposed sau khi request kết thúc
            var body = $@"
    <p>Xin chào,</p>
    <p>Mã OTP của bạn là: <strong>{otp}</strong></p>
    <p>Mã sẽ hết hạn sau <strong>2 phút</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
";

            _ = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(GlobalContainer.Container))
                {
                    try
                    {
                        var emailService = GlobalContainer.Container.GetInstance<IEmailService>();
                        await emailService.SendAsync(email, "[EMarket] Mã OTP xác thực", body);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoginService] Gửi OTP email thất bại: {ex.Message}");
                    }
                }
            });
        }

        public async Task ResetPasswordAsync(string email, string otp, string newPassword)
        {
            // Lấy OTP mới nhất trong DB
            var otpRecord = await _db.ForgotPasswordOtps
                .Where(x => x.Email == email)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
                throw new Exception("Không tìm thấy OTP");

            if (otpRecord.Otp != otp)
                throw new Exception("OTP không đúng");

            if (DateTime.Now > otpRecord.ExpireAt)
                throw new Exception("OTP đã hết hạn");

            // Lấy user
            var user = await _db.Users.FirstOrDefaultAsync(x => x.email == email);
            if (user == null)
                throw new Exception("User not found");

            // Cập nhật mật khẩu
            user.password_hash = HashPassword(newPassword);
            user.updated_at = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task<LoginResponseDTO> LoginByEmailAsync(string email)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.email == email);
            if (user == null)
            {
                return new LoginResponseDTO
                {
                    Status = Enums.LoginStatus.InvalidCredential
                };
            }
            if (user.status != "Active")
            {
                return new LoginResponseDTO
                {
                    Status = Enums.LoginStatus.Locked
                };
            }
            return new LoginResponseDTO
            {
                Status = Enums.LoginStatus.Success,
                User = await BuildCurrentUserDTO(user)
            };
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var otpRecord = await _db.ForgotPasswordOtps
                .Where(x => x.Email == email)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            if (otpRecord == null)
                return false;
            if (otpRecord.Otp != otp)
                return false;
            if (DateTime.Now > otpRecord.ExpireAt)
                return false;
            return true;
        }

        private string HashPassword(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            return passwordHash;
        }
    }
}
