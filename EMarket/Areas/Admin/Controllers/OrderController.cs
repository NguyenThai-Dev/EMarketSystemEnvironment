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
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;

namespace EMarket.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _service;
        private readonly IUserContext _userContext;
        private readonly ISystemConfigService _systemConfigService;
        private readonly IPaymentService _paymentService;

        public OrderController(IOrderService service, IUserContext userContext, ISystemConfigService systemConfigService, IPaymentService paymentService)
        {
            _service = service;
            _userContext = userContext;
            _systemConfigService = systemConfigService;
            _paymentService = paymentService;
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

        [HttpPost]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<JsonResult> CreatePayOSLink(CreateQrRequestDTO model) // Đưa model lên làm tham số
        {
            try
            {
                if (model == null || model.Amount <= 0)
                {
                    return Json(new { Success = false, Message = "Dữ liệu không hợp lệ!" });
                }

                var result = await _paymentService.CreatePayOSLinkAsync(model);
                
                // Nếu tạo link thành công, khởi chạy một Background Task ngầm để tự động kiểm tra trạng thái
                // Điều này giúp loại bỏ sự phụ thuộc vào Webhook nếu đang dev trên Localhost
                if (result != null && result.Success && result.OrderCode > 0)
                {
                    long orderCode = result.OrderCode;
                    decimal amount = model.Amount;

                    Task.Run(async () =>
                    {
                        for (int i = 0; i < 60; i++) // Liên tục kiểm tra trong vòng 3 phút (60 lần * 3s)
                        {
                            try
                            {
                                // Khởi tạo Client riêng vì DbContext/Service hiện tại sẽ bị dispose khi request kết thúc
                                string clientId = System.Configuration.ConfigurationManager.AppSettings["PayOSClientId"];
                                string apiKey = System.Configuration.ConfigurationManager.AppSettings["PayOSApiKey"];
                                string checksumKey = System.Configuration.ConfigurationManager.AppSettings["ChecksumKey"];
                                
                                var payOSClient = new PayOSClient(clientId, apiKey, checksumKey);
                                var paymentInfo = await payOSClient.PaymentRequests.GetAsync(orderCode);
                                
                                if (paymentInfo.Status == PaymentLinkStatus.Paid)
                                {
                                    // Thông báo thành công qua SignalR
                                    var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<EMarket.Hubs.OrderHub>();
                                    hubContext.Clients.Group("PAYMENT_" + orderCode).orderChanged(new
                                    {
                                        status = "PAID",
                                        orderCode = orderCode,
                                        amount = amount
                                    });
                                    break; // Thoát vòng lặp khi đã thanh toán thành công
                                }
                            }
                            catch
                            {
                                // Bỏ qua lỗi tạm thời (ví dụ: mất mạng lúc gọi API)
                            }
                            
                            // Đợi 3 giây trước khi gọi lại
                            await Task.Delay(3000);
                        }
                    });
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Lỗi Server: " + ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> PayOSWebhook() 
        {
            try
            {
                // ASP.NET MVC 5 mặc định không bind được JSON nested phức tạp của SDK PayOS vào tham số hàm.
                // Do đó BẮT BUỘC phải đọc thủ công từ InputStream.
                string json;
                Request.InputStream.Position = 0;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    json = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrEmpty(json))
                {
                    return Json(new { success = false, message = "Empty request body" });
                }

                var webhookBody = JsonConvert.DeserializeObject<Webhook>(json);

                if (webhookBody == null)
                {
                    return Json(new { success = false, message = "Webhook data is null after deserialize" });
                }

                // Thực hiện verify dữ liệu từ webhook
                var verifiedData = await _paymentService.VerifyPayOSWebhookAsync(webhookBody);

                // Notify via SignalR đến Client
                var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<EMarket.Hubs.OrderHub>();
                hubContext.Clients.Group("PAYMENT_" + verifiedData.OrderCode).orderChanged(new
                {
                    status = "PAID",
                    orderCode = verifiedData.OrderCode,
                    amount = verifiedData.Amount
                });

                // PayOS yêu cầu phản hồi lại kết quả để họ biết đã gửi thành công
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Nên dùng một thư viện Log (như NLog, Serilog) thay vì chỉ Debug.WriteLine 
                // để khi deploy lên Server vẫn xem được lỗi nếu có sự cố.
                Debug.WriteLine($"[PAYOS WEBHOOK ERROR] {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}