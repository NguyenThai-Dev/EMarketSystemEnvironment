using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.SalesModule.Services.Interfaces;

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
    }
}