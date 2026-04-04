using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [RoutePrefix("api/admin/user-management")]
    public class UserAdminApiController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        private readonly IRoleService _roleService;

        public UserAdminApiController(
            IUserService userService,
            IBranchService branchService,
            IRoleService roleService)
        {
            _userService = userService;
            _branchService = branchService;
            _roleService = roleService;
        }

        // ============================================================
        #region Branch APIs (Quản lý chi nhánh)
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ danh sách chi nhánh.
        /// </summary>
        [HttpGet]
        [Route("branches")]
        public async Task<IHttpActionResult> GetAllBranches()
        {
            var data = await _branchService.GetAllBranchesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Tìm kiếm chi nhánh theo tên hoặc vị trí (GPS).
        /// Giúp AI trả lời: "Chi nhánh nào gần khách hàng này nhất?".
        /// </summary>
        [HttpGet]
        [Route("branches/search")]
        public async Task<IHttpActionResult> SearchBranches(string name = null, double? lat = null, double? lng = null, double maxDist = 10)
        {
            if (lat.HasValue && lng.HasValue && lat > 0 && lng > 0)
            {
                var nearest = await _branchService.GetNearestBranchAsync(lat.Value, lng.Value, maxDist);
                return Ok(nearest);
            }
            var filtered = await _branchService.GetFilteredBranchesAsync(name ?? "");
            return Ok(filtered);
        }

        #endregion

        // ============================================================
        #region User APIs (Quản lý người dùng & Nhân sự)
        // ============================================================

        /// <summary>
        /// Lấy danh sách nhân viên/người dùng kèm bộ lọc.
        /// </summary>
        [HttpGet]
        [Route("users")]
        public async Task<IHttpActionResult> GetUsers(string keyword = null)
        {
            var data = await _userService.GetFilteredUsersAsync(keyword ?? "");
            return Ok(data);
        }

        /// <summary>
        /// Thống kê nhân sự: Tổng số, số lượng tạo mới, tỷ lệ theo vai trò.
        /// Rất hữu ích khi quản lý hỏi: "Tình hình nhân sự tháng này thế nào?".
        /// </summary>
        [HttpGet]
        [Route("users/stats")]
        public async Task<IHttpActionResult> GetUserStats()
        {
            var total = await _userService.CountAllAsync();
            var active = await _userService.CountActiveUsersAsync();
            var roleStats = await _userService.GetRoleStatisticsAsync();
            var growth = await _userService.GetUsersCreatedByMonthAsync();

            return Ok(new
            {
                Total = total,
                Active = active,
                RoleDistribution = roleStats,
                MonthlyGrowth = growth
            });
        }

        /// <summary>
        /// Lấy danh sách Email của các quản lý kho.
        /// AI có thể dùng để: "Gửi thông báo nợ cho các quản lý kho giúp tôi".
        /// </summary>
        [HttpGet]
        [Route("users/warehouse-managers-emails")]
        public async Task<IHttpActionResult> GetWarehouseManagerEmails()
        {
            var emails = await _userService.GetWarehouseManagerEmailsAsync();
            return Ok(emails);
        }

        #endregion

        // ============================================================
        #region Role & Permission APIs
        // ============================================================

        /// <summary>
        /// Lấy danh sách các vai trò (Roles) trong hệ thống.
        /// </summary>
        [HttpGet]
        [Route("roles")]
        public async Task<IHttpActionResult> GetAllRoles()
        {
            var data = await _roleService.GetAllRolesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Kiểm tra các quyền (Permission ID) của một vai trò cụ thể.
        /// </summary>
        [HttpGet]
        [Route("roles/{id:int}/permissions")]
        public async Task<IHttpActionResult> GetRolePermissions(int id)
        {
            var permissions = await _roleService.GetRolePermissionByRoleId(id);
            return Ok(permissions);
        }

        #endregion
    }
}