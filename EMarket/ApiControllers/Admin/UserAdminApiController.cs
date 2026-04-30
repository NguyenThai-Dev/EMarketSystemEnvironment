using System;
using System.Collections.Generic;
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
        private readonly IPermissionService _permissionService;

        public UserAdminApiController(
            IUserService userService, IBranchService branchService,
            IRoleService roleService, IPermissionService permissionService)
        {
            _userService = userService;
            _branchService = branchService;
            _roleService = roleService;
            _permissionService = permissionService;
        }

        #region Users

        [HttpGet, Route("users")]
        public async Task<IHttpActionResult> GetAllUsers()
        { return Ok(await _userService.GetAllUsersAsync()); }

        [HttpGet, Route("users/search")]
        public async Task<IHttpActionResult> SearchUsers(string keyword = null)
        { return Ok(await _userService.GetFilteredUsersAsync(keyword ?? "")); }

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