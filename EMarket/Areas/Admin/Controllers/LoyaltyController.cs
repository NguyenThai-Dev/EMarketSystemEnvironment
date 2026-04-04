using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Modules.CustomerModule.DTOs;
using EMarket.Modules.CustomerModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class LoyaltyController : Controller
    {
        private readonly ILoyaltyProgramService _loyaltyProgramService;

        public LoyaltyController(ILoyaltyProgramService loyaltyProgramService)
        {
            _loyaltyProgramService = loyaltyProgramService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllLoyalty()
        {
            var data = await _loyaltyProgramService.GetAllLoyaltyAsync();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetLoyaltyById(int id)
        {
            var item = await _loyaltyProgramService.GetLoyaltyByIdAsync(id);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateLoyalty(LoyaltyProgramDTO dto)
        {
            var ok = await _loyaltyProgramService.CreateLoyaltyAsync(dto);
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateLoyalty(LoyaltyProgramDTO dto)
        {
            var ok = await _loyaltyProgramService.UpdateLoyaltyAsync(dto);
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteLoyalty(int id)
        {
            var ok = await _loyaltyProgramService.DeleteLoyaltyAsync(id);
            return Json(new { success = ok });
        }

    }
}