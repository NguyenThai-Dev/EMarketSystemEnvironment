using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.UserModule.DTOs;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [EMarketAuthorize(Module = "InventoryModule")]
        public ActionResult BranchList()
        {
            return View();
        }

        public async Task<JsonResult> GetAllBranch()
        {
            var data = await _branchService.GetAllBranchesAsync();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetBranchSession(int id, string name)
        {
            // Lưu thẳng vào bộ nhớ Session của Server
            Session["CurrentBranchId"] = id;
            Session["CurrentBranchName"] = name;

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<JsonResult> GetNearest(double lat, double lng)
        {
            var branch = await _branchService.GetNearestBranchAsync(lat, lng, 100);

            if (branch != null)
            {

                return Json(new { success = true, data = branch }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Không tìm thấy chi nhánh nào gần đây." }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetFilteredBranch(string branchName)
        {
            var data = await _branchService.GetFilteredBranchesAsync(branchName);
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetBranchById(int id)
        {
            var item = await _branchService.GetBranchByIdAsync(id);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [EMarketAuthorize(Module = "InventoryModule")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateBranch(BranchDTO dto)
        {
            var id = await _branchService.CreateBranchAsync(dto);
            return Json(new { success = id > 0 });
        }

        [HttpPost]
        [EMarketAuthorize(Module = "InventoryModule")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateBranch(BranchDTO dto)
        {
            var success = await _branchService.UpdateBranchAsync(dto);
            return Json(new { success });
        }

        [HttpPost]
        [EMarketAuthorize(Module = "InventoryModule")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteBranch(int id)
        {
            var success = await _branchService.DeleteBranchAsync(id);
            return Json(new { success });
        }
    }
}