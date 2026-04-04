using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Areas.Admin.Data;
using EMarket.Filters;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;
using Newtonsoft.Json;

namespace EMarket.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _service;
        private readonly IUserContext _userContext;
        private readonly ISystemConfigService _systemConfigService;

        public OrderController(IOrderService service, IUserContext userContext, ISystemConfigService systemConfigService)
        {
            _service = service;
            _userContext = userContext;
            _systemConfigService = systemConfigService;
        }

        [EMarketAuthorize(Module = "SalesModule")]
        public ActionResult OrderList()
        {
            return View();
        }

        [EMarketAuthorize(Module = "SalesModule")]
        public ActionResult POSEMarket()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetAllOrders(OrderDataTableRequestDTO request)
        {
            Debug.WriteLine("GetAllOrders called");
            if (request == null) return Json(new { error = "Request mapping failed" });

            try
            {
                var result = await _service.GetOrdersDataTableAsync(
                    request.draw, request.start, request.length,
                    request.UserId, request.BranchId, request.Status,
                    request.FromDate,
                    request.ToDate,
                    request.Keyword
                );

                return Json(new
                {
                    draw = request.draw,
                    recordsTotal = result.total,
                    recordsFiltered = result.filtered,
                    data = result.data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        // ===============================
        // GET ORDER DETAIL
        // ===============================
        [HttpGet]
        public async Task<ActionResult> GetOrder(int id)
        {
            var data = await _service.GetOrderByIdAsync(id);
            if (data == null)
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" },
                    JsonRequestBehavior.AllowGet);

            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        // ===============================
        // UPDATE STATUS
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> UpdateStatus(int orderId, string status, string connectionId)
        {
            var ok = await _service.UpdateOrderStatusAsync(orderId, status, connectionId);
            if (!ok) return Json(new { success = false });

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<ActionResult> GetEMarketVAT()
        {
            var data = await _systemConfigService.GetEMarketVAT();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetEMarketBankID()
        {
            var data = await _systemConfigService.GetEMarketBankID();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetEMarketBankNum()
        {
            var data = await _systemConfigService.GetEMarketBankNum();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetVIPDiscount()
        {
            var data = await _systemConfigService.GetVIPDiscount();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetMemberDiscount()
        {
            var data = await _systemConfigService.GetMemberDiscount();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetPointExchangeRate()
        {
            var data = await _systemConfigService.GetEMarketPointExchnageRate();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetPointEarnedRate()
        {
            var data = await _systemConfigService.GetEMarketPointEarnedRate();
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderById(int id)
        {
            var result = await _service.GetOrderByIdAsync(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> CreateOrder(OrderDTO dto)
        {
            var id = await _service.CreateOrderAsync(dto);
            return Json(new { success = true, id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> UpdateOrder(OrderDTO dto)
        {
            var ok = await _service.UpdateOrderAsync(dto);
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var ok = await _service.DeleteOrderAsync(id);
            return Json(new { success = ok });
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderDetail(int orderId)
        {
            var data = await _service.GetOrderDetailsByOrderIdAsync(orderId);
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> AddOrderDetail(OrderDetailDTO dto)
        {
            var id = await _service.CreateOrderDetailAsync(dto);
            return Json(new { success = true, id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> UpdateOrderDetail(OrderDetailDTO dto)
        {
            var ok = await _service.UpdateOrderDetailAsync(dto);
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> DeleteOrderDetail(int id)
        {
            var ok = await _service.DeleteOrderDetailAsync(id);
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<JsonResult> Checkout() // Bỏ tham số model ở đây
        {
            try
            {
                // 1. TỰ ĐỌC JSON BODY
                string json;
                Request.InputStream.Position = 0;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    json = await reader.ReadToEndAsync();
                }

                // 2. Convert JSON chuỗi thành Object C#
                var model = JsonConvert.DeserializeObject<CheckoutRequestDTO>(json);

                // 3. Validate thủ công sau khi convert
                if (model == null || model.Items == null || model.Items.Count == 0)
                {
                    return Json(new { success = false, message = "Giỏ hàng trống (Lỗi nhận dữ liệu)!" });
                }

                // 4. Gọi Service như bình thường
                var result = await _service.CheckoutAsync(model);

                if (result.Success)
                {
                    return Json(new { success = true, data = new { orderId = result.OrderId }, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi Server: " + ex.Message });
            }
        }
    }
}