using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public RoleController(IRoleService roleService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        [EMarketAuthorize(RequireAdmin = true)]
        public ActionResult RoleManagement()
        {
            return View();
        }

        public async Task<JsonResult> GetAllRole()
        {
            var data = await _roleService.GetAllRolesAsync();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetRolePermissions(int roleId)
        {
            var data = await _roleService.GetRolePermissionByRoleId(roleId);
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetRoleById(int roleId)
        {
            var item = await _roleService.GetRoleByIdAsync(roleId);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<JsonResult> CreateRole(RoleDTO dto)
        {
            var id = await _roleService.CreateRoleAsync(dto);
            return Json(new { success = id > 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<JsonResult> UpdateRolePermissions(RolePermissionUpdateDTO model)
        {
            var success = await _roleService.UpdateRolePermissionsAsync(model);
            return Json(new { success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<JsonResult> DeleteRole(int id)
        {
            var success = await _roleService.DeleteRoleAsync(id);
            return Json(new { success });
        }

        public async Task<JsonResult> GetAllPermission()
        {
            var data = await _permissionService.GetAllPermissionsAsync();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetByIdGetAllPermission(int id)
        {
            var item = await _permissionService.GetPermissionByIdAsync(id);
            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}