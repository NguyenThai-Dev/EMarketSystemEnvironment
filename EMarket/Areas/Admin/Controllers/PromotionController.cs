using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class PromotionController : Controller
    {
        private readonly IPromotionService _service;

        public PromotionController(IPromotionService service)
        {
            _service = service;
        }

        public ActionResult PromotionManagement()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPromotion()
        {
            var data = await _service.GetAllPromotionsAsync();
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetPromotionById(int id)
        {
            var result = await _service.GetPromotionByIdAsync(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePromotion(PromotionDTO dto)
        {
            var id = await _service.CreatePromotionAsync(dto);
            return Json(new { success = true, id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePromotion(PromotionDTO dto)
        {
            var ok = await _service.UpdatePromotionAsync(dto);
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePromotion(int id)
        {
            var ok = await _service.DeletePromotionAsync(id);
            return Json(new { success = ok });
        }

    }
}