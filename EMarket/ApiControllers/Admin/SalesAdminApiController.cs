using EMarket.Hubs;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;
using Microsoft.AspNet.SignalR;
using PayOS.Models.Webhooks;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;

namespace EMarket.ApiControllers.Admin
{
    [RoutePrefix("api/admin/sales")]
    public class SalesAdminApiController : ApiController
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPromotionService _promotionService;

        public SalesAdminApiController(
            IOrderService orderService,
            IPaymentService paymentService,
            IPromotionService promotionService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _promotionService = promotionService;
        }

        // ============================================================
        #region Order Management APIs (Dành cho AI Assistant)
        // ============================================================

        /// <summary>
        /// Lấy danh sách đơn hàng với bộ lọc nâng cao (DataTable).
        /// Hỗ trợ AI trả lời về số lượng đơn, trạng thái và lọc theo ngày.
        /// </summary>
        [HttpGet]
        [Route("orders")]
        public async Task<IHttpActionResult> GetOrders(
            int start = 0, int length = 10, int? userId = null,
            int? branchId = null, string status = null,
            DateTime? fromDate = null, DateTime? toDate = null, string keyword = null)
        {
            // Draw mặc định là 1 cho các cuộc gọi API lẻ
            var result = await _orderService.GetOrdersDataTableAsync(1, start, length, userId, branchId, status, fromDate, toDate, keyword);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết một đơn hàng kèm theo danh sách sản phẩm (OrderDetails).
        /// </summary>
        [HttpGet]
        [Route("orders/{id:int}")]
        public async Task<IHttpActionResult> GetOrderDetail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var details = await _orderService.GetOrderDetailsByOrderIdAsync(id);
            return Ok(new { Order = order, Details = details });
        }

        /// <summary>
        /// Tra cứu nhanh danh sách đơn hàng theo chi nhánh và thời gian.
        /// </summary>
        [HttpGet]
        [Route("orders/by-branch")]
        public async Task<IHttpActionResult> GetOrdersByBranch(int? branchId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var data = await _orderService.GetOrdersByBranchIdAsync(branchId, fromDate, toDate);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Payment & Financial APIs
        // ============================================================

        /// <summary>
        /// Lấy lịch sử thanh toán của một đơn hàng.
        /// Giúp AI kiểm tra xem đơn hàng đã trả đủ tiền chưa.
        /// </summary>
        [HttpGet]
        [Route("orders/{orderId:int}/payments")]
        public async Task<IHttpActionResult> GetPaymentsByOrder(int orderId)
        {
            var data = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Promotion APIs
        // ============================================================

        /// <summary>
        /// Lấy danh sách các chương trình khuyến mãi đang có hiệu lực.
        /// </summary>
        [HttpGet]
        [Route("promotions/active")]
        public async Task<IHttpActionResult> GetActivePromotions()
        {
            var data = await _promotionService.GetActivePromotionsAsync();
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Sales Actions (Giao tiếp với AI)
        // ============================================================

        /// <summary>
        /// Cập nhật trạng thái đơn hàng (Duyệt đơn, Đã giao, Hủy...).
        /// AI có thể dùng để thực thi lệnh: "Xác nhận đơn hàng #555 cho khách nhé".
        /// </summary>
        [HttpPut]
        [Route("orders/{id:int}/status")]
        public async Task<IHttpActionResult> UpdateStatus(int id, [FromBody] string status, string connectionId = "")
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status, connectionId);
            if (result) return Ok(new { Message = "Cập nhật trạng thái thành công" });
            return BadRequest("Không thể cập nhật trạng thái");
        }

        #endregion


        [HttpPost]
        [Route("payment/create-qr")]
        public async Task<IHttpActionResult> CreateQR([FromBody] CreateQrRequestDTO request)
        {
            if (request == null || request.Amount <= 0)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await _paymentService.CreatePayOSLinkAsync(request);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost]
        [Route("payment/webhook")]
        public async Task<IHttpActionResult> WebhookHandler([FromBody] Newtonsoft.Json.Linq.JObject rawJson)
        {
            try
            {
                var serializer = new Newtonsoft.Json.JsonSerializer
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };

                // Convert JObject sang class Webhook của thư viện với định dạng chuẩn
                var webhookBody = rawJson.ToObject<PayOS.Models.Webhooks.Webhook>(serializer);

                var verifiedData = await _paymentService.VerifyPayOSWebhookAsync(webhookBody);
                
                if (verifiedData.Code == "00")
                {
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<OrderHub>();
                    var groupName = "PAYMENT_" + verifiedData.OrderCode;

                    // Payload phải khớp với những gì frontend file JS đang lắng nghe
                    var payload = new
                    {
                        orderId = verifiedData.OrderCode,
                        status = "PAID",
                        serverTime = DateTime.Now.ToString("HH:mm:ss"),
                        isTest = false,
                        message = "Khách hàng đã thanh toán thành công!"
                    };

                    hubContext.Clients.Group(groupName).orderChanged(payload);
                    System.Diagnostics.Debug.WriteLine($"[WEBHOOK] Đã bắn SignalR tới group {groupName}");

                    return Ok(new { success = true });
                }

                return Ok(new { success = false, message = "Giao dịch không thành công" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WEBHOOK ERROR] {ex.Message}");

                return Ok(new { success = false, message = "Xác thực Webhook thất bại" });
            }
        }


        //[HttpPost]
        //[Route("payment/webhook")]
        //public async Task<IHttpActionResult> WebhookHandler([FromBody] Webhook webhookBody)
        //{
        //    try
        //    {
        //        // 1. Lấy dữ liệu (Service đã được sửa ở trên để không quăng Exception nữa)
        //        var verifiedData = await _paymentService.VerifyPayOSWebhookAsync(webhookBody);

        //        // 2. Kiểm tra mã thành công (PayOS trả về "00" trong data hoặc code tổng)
        //        if (webhookBody.Code == "00" || (verifiedData != null && verifiedData.Code == "00"))
        //        {
        //            var hubContext = GlobalHost.ConnectionManager.GetHubContext<OrderHub>();

        //            // QUAN TRỌNG: Phải dùng đúng OrderCode để bắn vào Group
        //            string groupName = "PAYMENT_" + verifiedData.OrderCode.ToString();

        //            var payload = new
        //            {
        //                orderId = verifiedData.OrderCode,
        //                status = "PAID",
        //                message = "Thanh toán thành công!",
        //                serverTime = DateTime.Now.ToString("HH:mm:ss")
        //            };

        //            hubContext.Clients.Group(groupName).orderChanged(payload);

        //            Debug.WriteLine($"[SIGNALR] Đã nổ Ting Ting cho Group: {groupName}");
        //            return Ok(new { success = true });
        //        }

        //        return Ok(new { success = false });
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"[FATAL ERROR] {ex.Message}");
        //        return Ok(new { success = false }); // Luôn trả về 200 để PayOS không bắn lại liên tục
        //    }
        //}



    }
}