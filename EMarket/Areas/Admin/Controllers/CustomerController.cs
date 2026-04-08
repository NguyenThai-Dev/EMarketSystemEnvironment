using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.CustomerModule.DTOs;
using EMarket.Modules.CustomerModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerAddressService _customerAddressService;

        public CustomerController(ICustomerService customerService, ICustomerAddressService customerAddressService)
        {
            _customerService = customerService;
            _customerAddressService = customerAddressService;
        }

        [EMarketAuthorize(Module = "CustomerModule")]
        public ActionResult CustomerList()
        {
            return View();
        }

        //Customer Controller
        #region

        [HttpGet]
        public async Task<ActionResult> GetAllCustomer()
        {
            var data = await _customerService.GetAllCustomerAsync();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCustomerFiltered(string keyword)
        {
            var data = await _customerService.GetAllCustomerFilteredAsync(keyword);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetCustomerById(int id)
        {
            var data = await _customerService.GetCustomerByIdAsync(id);
            return Json(new { customers = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "CustomerModule")]
        public async Task<ActionResult> CreateCustomer(CustomerCreateDTO dto, HttpPostedFileBase file)
        {
            var newId = await _customerService.CreateCustomerAsync(dto, file);

            return Json(new { success = newId > 0, newId = newId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "CustomerModule")]
        public async Task<ActionResult> UpdateCustomer(CustomerUpdateDTO dto, HttpPostedFileBase file)
        {
            var result = await _customerService.UpdateCustomerAsync(dto, file);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "CustomerModule")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            return Json(new { success = result });
        }

        #endregion

        //Customer Address Controller
        #region

        [HttpGet]
        public async Task<ActionResult> GetCustomerAddress(int customerId)
        {
            var data = await _customerAddressService.GetCustomerAddressAsync(customerId);
            return Json(new { addresses = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetCustomerAddressById(int addressId)
        {
            var data = await _customerAddressService.GetCustomerAddressByIdAsync(addressId);
            return Json(new { addresses = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "CustomerModule")]
        public async Task<ActionResult> CreateCustomerAddress(CustomerAddressCreateDTO dto)
        {
            return Json(new { success = await _customerAddressService.CreateCustomerAddressAsync(dto) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "CustomerModule")]
        public async Task<ActionResult> UpdateCustomerAddress(CustomerAddressUpdateDTO dto)
        {
            return Json(new { success = await _customerAddressService.UpdateCustomerAddressAsync(dto) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "CustomerModule")]
        public async Task<ActionResult> DeleteCustomerAddress(int id)
        {
            return Json(new { success = await _customerAddressService.DeleteCustomerAddressAsync(id) });
        }

        #endregion

    }
}