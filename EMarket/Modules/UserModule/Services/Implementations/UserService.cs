using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EMarket.Hubs;
using EMarket.Models;
using EMarket.Modules.DashboardModule.DTOs;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNet.SignalR;

namespace EMarket.Modules.UserModule.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly EMarket_DBEntities _db;

        public UserService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<int> CountActiveUsersAsync()
        {
            return await _db.Users
                .CountAsync(x => x.status == "Active");
        }

        public async Task<int> CountCreatedFromAsync(DateTime fromDate)
        {
            return await _db.Users
                .CountAsync(x => x.created_at >= fromDate);
        }

        public async Task<int> CountAllAsync()
        {
            return await _db.Users.CountAsync();
        }

        public async Task<List<string>> GetRecentActiveUserAvatarsAsync(int top)
        {
            return await _db.Users
                .Where(x => x.status == "Active" && x.user_img != null)
                .OrderByDescending(x => x.created_at)
                .Take(top)
                .Select(x => x.user_img)
                .ToListAsync();
        }

        public async Task<List<(int Month, int Count)>> GetUsersCreatedByMonthAsync()
        {
            // Use an anonymous type in the query, then project to tuple in memory
            var result = await _db.Users
                .GroupBy(x => x.created_at.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return result.Select(x => (x.Month, x.Count)).ToList();
        }

        public async Task<List<RoleStatItemDTO>> GetRoleStatisticsAsync()
        {
            var totalUsers = await _db.Users.CountAsync();

            return await (
                from ur in _db.UserRoles
                join r in _db.Roles on ur.role_id equals r.role_id
                group ur by r.name into g
                select new RoleStatItemDTO
                {
                    RoleName = g.Key,
                    Count = g.Count(),
                    TotalUsers = totalUsers
                }
            ).ToListAsync();
        }

        public async Task<List<string>> GetWarehouseManagerEmailsAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .Join(_db.UserRoles,
                      u => u.user_id,
                      ur => ur.user_id,
                      (u, ur) => new { u, ur }) // Kết hợp 2 bảng
                .Where(x => x.ur.role_id == 3 && !string.IsNullOrEmpty(x.u.email))
                .Select(x => x.u.email)
                .Distinct() // Tránh trùng nếu 1 user có nhiều record role (tùy DB)
                .ToListAsync();
        }

        public async Task<List<CurrentUserDTO>> GetAllUsersAsync()
        {
            // =========================
            // 1. Load Users
            // =========================
            var users = await _db.Users
                .AsNoTracking()
                .OrderBy(x => x.full_name)
                .ToListAsync();

            if (!users.Any())
                return new List<CurrentUserDTO>();

            var userIds = users.Select(x => x.user_id).ToList();

            // =========================
            // 2. Load UserRoles + Roles
            // =========================
            var userRoleRows = await (
                from ur in _db.UserRoles.AsNoTracking()
                join r in _db.Roles.AsNoTracking()
                    on ur.role_id equals r.role_id
                where userIds.Contains(ur.user_id)
                      && ur.status == "Active"
                select new UserRoleRowDTO
                {
                    UserId = ur.user_id,
                    RoleId = ur.role_id,
                    BranchId = ur.branch_id,
                    RoleName = r.name
                }
            ).ToListAsync();

            var rolesByUser = userRoleRows
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // =========================
            // 3. Load Permissions theo RoleIds
            // =========================
            var roleIds = userRoleRows
                .Select(x => x.RoleId)
                .Distinct()
                .ToList();

            var permissionRows = await (
                 from rp in _db.RolePermissions.AsNoTracking()
                 join p in _db.Permissions.AsNoTracking()
                     on rp.permission_id equals p.permission_id
                 where roleIds.Contains(rp.role_id)
                 select new RolePermissionRowDTO
                 {
                     RoleId = rp.role_id,
                     PermissionId = p.permission_id,
                     PermissionName = p.name,
                     Module = p.module
                 }
             ).ToListAsync();

            var permissionsByRole = permissionRows
                 .GroupBy(x => x.RoleId)
                 .ToDictionary(g => g.Key, g => g.ToList());


            // =========================
            // 4. Build DTOs (IN-MEMORY)
            // =========================
            var result = new List<CurrentUserDTO>(users.Count);

            foreach (var user in users)
            {
                result.Add(
                    BuildCurrentUserDTOOptimized(
                        user,
                        rolesByUser,
                        permissionsByRole
                    )
                );
            }

            return result;
        }


        private CurrentUserDTO BuildCurrentUserDTOOptimized(
     User user,
     Dictionary<int, List<UserRoleRowDTO>> rolesByUser,
     Dictionary<int, List<RolePermissionRowDTO>> permissionsByRole)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            List<UserRoleRowDTO> userRoleRows;
            rolesByUser.TryGetValue(user.user_id, out userRoleRows);

            // =========================
            // Không có role
            // =========================
            if (userRoleRows == null || userRoleRows.Count == 0)
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

                    IsAdmin = false,
                    IsSupplier = user.supplier_id != null
                };
            }

            // =========================
            // Build Roles + Permissions
            // =========================
            var roles = new List<RoleDTO>();
            var allPermissions = new Dictionary<int, PermissionDTO>();

            foreach (var roleRow in userRoleRows
                .GroupBy(x => new { x.RoleId, x.RoleName }))
            {
                var roleDto = new RoleDTO
                {
                    RoleId = roleRow.Key.RoleId,
                    Name = roleRow.Key.RoleName
                };

                List<RolePermissionRowDTO> rolePerms;
                if (permissionsByRole.TryGetValue(roleDto.RoleId, out rolePerms))
                {
                    foreach (var p in rolePerms)
                    {
                        var permDto = new PermissionDTO
                        {
                            PermissionId = p.PermissionId,
                            Name = p.PermissionName,
                            Module = p.Module
                        };

                        roleDto.Permissions.Add(permDto);

                        // gom permission unique cho user
                        if (!allPermissions.ContainsKey(p.PermissionId))
                        {
                            allPermissions.Add(p.PermissionId, permDto);
                        }
                    }
                }

                roles.Add(roleDto);
            }

            // =========================
            // Branch
            // =========================
            int? branchId = userRoleRows
                .Select(x => x.BranchId)
                .FirstOrDefault(x => x.HasValue);

            // =========================
            // Build DTO
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
                Permissions = allPermissions.Values.ToList(),

                IsAdmin = roles.Any(r =>
                    r.Name.Equals("System Admin", StringComparison.OrdinalIgnoreCase)),

                IsSupplier = user.supplier_id != null
            };
        }


        public async Task<List<CurrentUserDTO>> GetFilteredUsersAsync(string keyword)
        {
            keyword = keyword?.Trim();

            // =========================
            // 1. Load Users
            // =========================
            var usersQuery = _db.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                usersQuery = usersQuery.Where(x =>
                    x.username.Contains(keyword) ||
                    x.full_name.Contains(keyword) ||
                    x.email.Contains(keyword) ||
                    x.phone.Contains(keyword)
                );
            }

            var users = await usersQuery
                .OrderBy(x => x.full_name)
                .ToListAsync();

            if (users.Count == 0)
                return new List<CurrentUserDTO>();

            var userIds = users.Select(x => x.user_id).ToList();

            // =========================
            // 2. Load Roles
            // =========================
            var userRoleRows = await (
                from ur in _db.UserRoles.AsNoTracking()
                join r in _db.Roles.AsNoTracking()
                    on ur.role_id equals r.role_id
                where userIds.Contains(ur.user_id)
                      && ur.status == "Active"
                select new UserRoleRowDTO
                {
                    UserId = ur.user_id,
                    RoleId = ur.role_id,
                    BranchId = ur.branch_id,
                    RoleName = r.name
                }
            ).ToListAsync();

            var rolesByUser = userRoleRows
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var roleIds = userRoleRows
                .Select(x => x.RoleId)
                .Distinct()
                .ToList();

            // =========================
            // 3. Load Permissions
            // =========================
            var permissionRows = await (
                from rp in _db.RolePermissions.AsNoTracking()
                join p in _db.Permissions.AsNoTracking()
                    on rp.permission_id equals p.permission_id
                where roleIds.Contains(rp.role_id)
                select new RolePermissionRowDTO
                {
                    RoleId = rp.role_id,
                    PermissionId = p.permission_id,
                    PermissionName = p.name,
                    Module = p.module
                }
            ).ToListAsync();

            var permissionsByRole = permissionRows
                .GroupBy(x => x.RoleId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // =========================
            // 4. Build DTOs
            // =========================
            var result = new List<CurrentUserDTO>(users.Count);

            foreach (var user in users)
            {
                result.Add(
                    BuildCurrentUserDTOOptimized(
                        user,
                        rolesByUser,
                        permissionsByRole
                    )
                );
            }

            return result;
        }


        #region Helpper DTO
        private class UserRoleRowDTO
        {
            public int UserId { get; set; }
            public int RoleId { get; set; }
            public int? BranchId { get; set; }
            public string RoleName { get; set; }
        }

        private class RolePermissionRowDTO
        {
            public int RoleId { get; set; }
            public int PermissionId { get; set; }
            public string PermissionName { get; set; }
            public string Module { get; set; }
        }

        #endregion

        public async Task<int> CreateUserAsync(CurrentUserDTO dto, HttpPostedFileBase file)
        {
            using (var tran = _db.Database.BeginTransaction())
            {
                try
                {
                    // =========================
                    // 1. Create User
                    // =========================
                    var user = new User
                    {
                        username = dto.Username,
                        full_name = dto.FullName,
                        email = dto.Email,
                        phone = dto.Phone,
                        status = dto.Status ?? "Active",
                        supplier_id = dto.SupplierId,
                        password_hash = HashPassword("systempass"),
                        created_at = DateTime.Now
                    };

                    _db.Users.Add(user);
                    await _db.SaveChangesAsync(); // lấy user_id

                    // =========================
                    // 2. Create UserRole
                    // =========================
                    var userRole = new UserRole
                    {
                        user_id = user.user_id,
                        role_id = dto.RoleId,
                        branch_id = dto.RoleId == 6 ? null : dto.BranchId, // Partner không có branch
                        status = "Active",
                    };

                    _db.UserRoles.Add(userRole);

                    // =========================
                    // 3. Save Image (nếu có)
                    // =========================
                    if (file != null && file.ContentLength > 0)
                    {
                        string folder = HttpContext.Current.Server.MapPath($"~/Uploads/Users/{user.user_id}");
                        Directory.CreateDirectory(folder);

                        string ext = Path.GetExtension(file.FileName);
                        string fileName = Guid.NewGuid() + ext;
                        string path = Path.Combine(folder, fileName);

                        file.SaveAs(path);

                        user.user_img = $"/Uploads/Users/{user.user_id}/{fileName}";
                    }

                    await _db.SaveChangesAsync();
                    tran.Commit();

                    return user.user_id;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception("CreateUserAsync failed.", ex);
                }
            }
        }

        private string HashPassword(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("myPassword");
            return passwordHash;
        }

        public async Task<bool> UpdateUserAsync(CurrentUserDTO dto, HttpPostedFileBase file)
        {
            using (var tran = _db.Database.BeginTransaction())
            {
                try
                {
                    var user = await _db.Users
                        .FirstOrDefaultAsync(x => x.user_id == dto.UserId);

                    if (user == null) return false;

                    // ============================================================
                    // 1. Update User Information (Full Fields)
                    // ============================================================
                    user.username = !string.IsNullOrWhiteSpace(dto.Username) ? dto.Username : user.username;
                    user.full_name = dto.FullName;
                    user.email = dto.Email;
                    user.phone = dto.Phone;

                    user.status = !string.IsNullOrWhiteSpace(dto.Status) ? dto.Status : user.status;

                    user.supplier_id = dto.SupplierId;

                    user.updated_at = DateTime.Now;

                    // ============================================================
                    // 2. Update UserRole & Branch Context
                    // ============================================================
                    if (dto.RoleId > 0)
                    {
                        var oldRoles = await _db.UserRoles
                            .Where(x => x.user_id == user.user_id)
                            .ToListAsync();

                        _db.UserRoles.RemoveRange(oldRoles);

                        _db.UserRoles.Add(new UserRole
                        {
                            user_id = user.user_id,
                            role_id = dto.RoleId,
                            branch_id = dto.IsAdmin ? null : dto.BranchId,
                            status = "Active",
                        });
                    }

                    // ===========================================================
                    // 3. Update Image & Cleanup
                    // ============================================================
                    if (file != null && file.ContentLength > 0)
                    {
                        string folderRelativePath = $"/Uploads/Users/{user.user_id}/";
                        string folderMapPath = HttpContext.Current.Server.MapPath("~" + folderRelativePath);

                        if (!Directory.Exists(folderMapPath)) Directory.CreateDirectory(folderMapPath);

                        // Xóa file cũ để tiết kiệm bộ nhớ server
                        if (!string.IsNullOrEmpty(user.user_img))
                        {
                            string oldImagePath = HttpContext.Current.Server.MapPath("~" + user.user_img);
                            if (File.Exists(oldImagePath)) File.Delete(oldImagePath);
                        }

                        string ext = Path.GetExtension(file.FileName);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string fullPath = Path.Combine(folderMapPath, fileName);

                        file.SaveAs(fullPath);
                        user.user_img = folderRelativePath + fileName;
                    }

                    await _db.SaveChangesAsync();
                    tran.Commit();

                    if (user.status == "Locked" || user.status == "Inactive")
                    {
                        System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(ct =>
                        {
                            try
                            {
                                var hubContext = GlobalHost.ConnectionManager.GetHubContext<SystemLogHub>();
                                string targetId = user.user_id.ToString();

                                hubContext.Clients.User(targetId).forceLogout("Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động.");

                                System.Diagnostics.Debug.WriteLine($">>> [SignalR] Đã gửi lệnh Kick tới UserID: {targetId}");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(">>> [SignalR Error]: " + ex.Message);
                            }
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception($"UpdateUserAsync failed for UserId={dto.UserId}", ex);
                }
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using (var tran = _db.Database.BeginTransaction())
            {
                try
                {
                    var user = await _db.Users
                        .FirstOrDefaultAsync(x => x.user_id == id);

                    if (user == null)
                        return false;

                    // =========================
                    // 1. Delete UserRoles (nếu có)
                    // =========================
                    var userRoles = await _db.UserRoles
                        .Where(x => x.user_id == id)
                        .ToListAsync();

                    if (userRoles.Count > 0)
                    {
                        _db.UserRoles.RemoveRange(userRoles);
                    }

                    // =========================
                    // 2. Delete User
                    // =========================
                    _db.Users.Remove(user);
                    await _db.SaveChangesAsync();

                    tran.Commit();

                    // =========================
                    // 3. Delete Avatar Folder (OUTSIDE TRANSACTION)
                    // =========================
                    string userFolder = HttpContext.Current.Server
                        .MapPath($"~/Uploads/Users/{id}");

                    if (Directory.Exists(userFolder))
                    {
                        Directory.Delete(userFolder, true);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception($"DeleteUserAsync failed for UserId={id}", ex);
                }
            }
        }

        public async Task<List<CurrentUserDTO>> GetUsersByUserIdsAsync(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
                return new List<CurrentUserDTO>();

            // =========================
            // 1. Load Users
            // =========================
            var users = await _db.Users
                .AsNoTracking()
                .Where(x => userIds.Contains(x.user_id))
                .OrderBy(x => x.full_name)
                .ToListAsync();

            if (!users.Any())
                return new List<CurrentUserDTO>();

            var validUserIds = users.Select(x => x.user_id).ToList();

            // =========================
            // 2. Load UserRoles + Roles
            // =========================
            var userRoleRows = await (
                from ur in _db.UserRoles.AsNoTracking()
                join r in _db.Roles.AsNoTracking()
                    on ur.role_id equals r.role_id
                where validUserIds.Contains(ur.user_id)
                      && ur.status == "Active"
                select new UserRoleRowDTO
                {
                    UserId = ur.user_id,
                    RoleId = ur.role_id,
                    BranchId = ur.branch_id,
                    RoleName = r.name
                }
            ).ToListAsync();

            var rolesByUser = userRoleRows
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // =========================
            // 3. Load Permissions theo RoleIds
            // =========================
            var roleIds = userRoleRows
                .Select(x => x.RoleId)
                .Distinct()
                .ToList();

            var permissionRows = await (
                from rp in _db.RolePermissions.AsNoTracking()
                join p in _db.Permissions.AsNoTracking()
                    on rp.permission_id equals p.permission_id
                where roleIds.Contains(rp.role_id)
                select new RolePermissionRowDTO
                {
                    RoleId = rp.role_id,
                    PermissionId = p.permission_id,
                    PermissionName = p.name,
                    Module = p.module
                }
            ).ToListAsync();

            var permissionsByRole = permissionRows
                .GroupBy(x => x.RoleId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // =========================
            // 4. Build DTOs (REUSE LOGIC)
            // =========================
            var result = new List<CurrentUserDTO>(users.Count);

            foreach (var user in users)
            {
                result.Add(
                    BuildCurrentUserDTOOptimized(
                        user,
                        rolesByUser,
                        permissionsByRole
                    )
                );
            }

            return result;
        }

        public async Task<Dictionary<int, CurrentUserDTO>> GetUserDictAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .OrderBy(x => x.full_name)
                .ToDictionaryAsync(
                    u => u.user_id,
                    u => new CurrentUserDTO
                    {
                        UserId = u.user_id,
                        Username = u.username,
                        FullName = u.full_name,
                        Email = u.email,
                        Phone = u.phone,
                        Image = u.user_img,
                        Status = u.status,
                        SupplierId = u.supplier_id,
                        IsSupplier = u.supplier_id != null
                    }
                );
        }

        public async Task<CurrentUserDTO> GetUserByIdAsync(int id)
        {
            // =========================
            // 1. Load User
            // =========================
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.user_id == id);

            if (user == null) return null;

            var userIdList = new List<int> { id };

            // =========================
            // 2. Load UserRoles + Roles (Giống logic GetAllUsers)
            // =========================
            var userRoleRows = await (
                from ur in _db.UserRoles.AsNoTracking()
                join r in _db.Roles.AsNoTracking() on ur.role_id equals r.role_id
                where ur.user_id == id && ur.status == "Active"
                select new UserRoleRowDTO
                {
                    UserId = ur.user_id,
                    RoleId = ur.role_id,
                    BranchId = ur.branch_id,
                    RoleName = r.name
                }
            ).ToListAsync();

            var rolesByUser = userRoleRows
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // =========================
            // 3. Load Permissions theo RoleIds
            // =========================
            var roleIds = userRoleRows.Select(x => x.RoleId).Distinct().ToList();
            var permissionRows = new List<RolePermissionRowDTO>();

            if (roleIds.Any())
            {
                permissionRows = await (
                    from rp in _db.RolePermissions.AsNoTracking()
                    join p in _db.Permissions.AsNoTracking() on rp.permission_id equals p.permission_id
                    where roleIds.Contains(rp.role_id)
                    select new RolePermissionRowDTO
                    {
                        RoleId = rp.role_id,
                        PermissionId = p.permission_id,
                        PermissionName = p.name,
                        Module = p.module
                    }
                ).ToListAsync();
            }

            var permissionsByRole = permissionRows
                .GroupBy(x => x.RoleId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // =========================
            // 4. Reuse Helper Logic (Build DTO full Roles/Permissions)
            // =========================
            return BuildCurrentUserDTOOptimized(user, rolesByUser, permissionsByRole);
        }

        public async Task<bool> UpdateUserEmailAsync(int userId, string newEmail)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.user_id == userId);
            if (user == null)
                return false;
            user.email = newEmail;
            user.updated_at = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<int, CurrentUserDTO>> GetUserDictAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new Dictionary<int, CurrentUserDTO>();
            return await _db.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.user_id))
                .ToDictionaryAsync(
                    u => u.user_id,
                    u => new CurrentUserDTO
                    {
                        UserId = u.user_id,
                        Username = u.username,
                        FullName = u.full_name,
                        Email = u.email,
                        Phone = u.phone,
                        Image = u.user_img,
                        Status = u.status,
                        SupplierId = u.supplier_id,
                        IsSupplier = u.supplier_id != null
                    }
                );
        }
    }
}