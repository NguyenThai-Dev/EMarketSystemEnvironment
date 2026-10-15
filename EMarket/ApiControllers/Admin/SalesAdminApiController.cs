using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [AllowAnonymous]
    [RoutePrefix("api/admin/sales")]
    public class SalesAdminApiController : ApiController
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPromotionService _promotionService;

        public SalesAdminApiController(
            IOrderService orderService, IPaymentService paymentService,
            IPromotionService promotionService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _promotionService = promotionService;
        }

        #region Orders

        [HttpGet, Route("orders/all")]
        public async Task<IHttpActionResult> GetAllOrders()
        { return Ok(await _orderService.GetAllOrdersAsync()); }

        [HttpGet, Route("orders")]
        public async Task<IHttpActionResult> GetOrders(int start = 0, int length = 10, int? userId = null, int? branchId = null, string status = null, DateTime? fromDate = null, DateTime? toDate = null, string keyword = null)
        {
            var pageSize = Math.Min(length > 0 ? length : 10, 100);
            var pageIndex = (start / pageSize) + 1;
            var r = await _orderService.GetOrdersDataTableAsync(pageIndex, start, pageSize, userId, branchId, status, fromDate, toDate, keyword);
            return Ok(new { total = r.total, filtered = r.filtered, data = r.data });
        }

        [HttpGet, Route("orders/{id:int}")]
        public async Task<IHttpActionResult> GetOrderDetail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            var details = await _orderService.GetOrderDetailsByOrderIdAsync(id);
            return Ok(new { Order = order, Details = details });
        }

        [HttpGet, Route("orders/full-by-branch")]
        public async Task<IHttpActionResult> GetFullOrdersByBranch(int? branchId = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _orderService.GetFullOrdersByBranchIdAsync(branchId, fromDate, toDate)); }

        [HttpGet, Route("orders/by-branch")]
        public async Task<IHttpActionResult> GetOrdersByBranch(int? branchId = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _orderService.GetOrdersByBranchIdAsync(branchId, fromDate, toDate)); }

        [HttpGet, Route("orders/{orderId:int}/details")]
        public async Task<IHttpActionResult> GetOrderDetails(int orderId)
        { return Ok(await _orderService.GetOrderDetailsByOrderIdAsync(orderId)); }

        public class UpdateStatusModel
        {
            public string Status { get; set; }
            public string ConnectionId { get; set; }
        }

        [HttpPost, Route("orders/{orderId:int}/status")]
        public async Task<IHttpActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateStatusModel model)
        {
            var status = model?.Status ?? "Completed";
            var connId = model?.ConnectionId ?? "";
            var success = await _orderService.UpdateOrderStatusAsync(orderId, status, connId);
            return Ok(new { success });
        }

        [HttpPost, Route("orders/create")]
        public async Task<IHttpActionResult> CreateOrder([FromBody] OrderDTO dto)
        {
            if (dto == null) return BadRequest("Order data required.");
            var id = await _orderService.CreateOrderAsync(dto);
            return Ok(new { success = id > 0, orderId = id });
        }

        [HttpPost, Route("orders/checkout")]
        public async Task<IHttpActionResult> Checkout([FromBody] CheckoutRequestDTO request)
        {
            if (request == null) return BadRequest("Checkout data required.");
            var result = await _orderService.CheckoutAsync(request);
            return Ok(result);
        }

        [HttpPost, Route("orders/details/create")]
        public async Task<IHttpActionResult> CreateOrderDetail([FromBody] OrderDetailDTO dto)
        {
            if (dto == null) return BadRequest("OrderDetail data required.");
            var id = await _orderService.CreateOrderDetailAsync(dto);
            return Ok(new { success = id > 0, orderDetailId = id });
        }

        [HttpPost, Route("orders/details/update")]
        [HttpPut, Route("orders/details/{id:int}")]
        public async Task<IHttpActionResult> UpdateOrderDetail([FromBody] OrderDetailDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("OrderDetail data required.");
            if (id.HasValue && dto.OrderDetailId <= 0) dto.OrderDetailId = id.Value;
            var ok = await _orderService.UpdateOrderDetailAsync(dto);
            return Ok(new { success = ok });
        }

        [HttpPost, Route("orders/details/delete/{id:int}")]
        [HttpDelete, Route("orders/details/{id:int}")]
        public async Task<IHttpActionResult> DeleteOrderDetail(int id)
        {
            var ok = await _orderService.DeleteOrderDetailAsync(id);
            return Ok(new { success = ok });
        }

        [HttpPost, Route("orders/delete/{orderId:int}")]
        [HttpDelete, Route("orders/{orderId:int}")]
        public async Task<IHttpActionResult> DeleteOrder(int orderId)
        {
            var success = await _orderService.DeleteOrderAsync(orderId);
            return Ok(new { success });
        }

        #endregion

        #region Payment

        [HttpGet, Route("orders/{orderId:int}/payments")]
        public async Task<IHttpActionResult> GetPaymentsByOrder(int orderId)
        { return Ok(await _paymentService.GetPaymentsByOrderIdAsync(orderId)); }

        [HttpPost, Route("orders/create-payos-link")]
        public async Task<IHttpActionResult> CreatePayOSLink([FromBody] CreateQrRequestDTO model)
        {
            if (model == null || model.Amount <= 0)
                return BadRequest("Dữ liệu thanh toán PayOS không hợp lệ.");
            var result = await _paymentService.CreatePayOSLinkAsync(model);
            return Ok(result);
        }

        [HttpGet, Route("orders/check-payos-status/{orderCode:long}")]
        public async Task<IHttpActionResult> CheckPayOSStatus(long orderCode)
        {
            var isPaid = await _paymentService.CheckPayOSPaymentStatusAsync(orderCode);
            return Ok(new { success = true, isPaid = isPaid, orderCode = orderCode });
        }

        #endregion

        #region Promotions

        [HttpGet, Route("promotions")]
        public async Task<IHttpActionResult> GetAllPromotions()
        { return Ok(await _promotionService.GetAllPromotionsAsync()); }

        [HttpGet, Route("promotions/search")]
        public async Task<IHttpActionResult> SearchPromotions(string keyword = null, int? categoryId = null, string discountType = null, string cusType = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _promotionService.GetFilteredPromotionAsync(keyword, categoryId, discountType, cusType, fromDate, toDate)); }

        [HttpGet, Route("promotions/{id:int}")]
        public async Task<IHttpActionResult> GetPromotionById(int id)
        {
            var d = await _promotionService.GetPromotionByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpGet, Route("promotions/active")]
        public async Task<IHttpActionResult> GetActivePromotions()
        { return Ok(await _promotionService.GetActivePromotionsAsync()); }

        [HttpPost, Route("promotions/create")]
        public async Task<IHttpActionResult> CreatePromotion([FromBody] PromotionDTO dto)
        {
            if (dto == null) return BadRequest("Promotion data required.");
            var id = await _promotionService.CreatePromotionAsync(dto);
            return Ok(new { success = id > 0, promotionId = id });
        }

        [HttpPost, Route("promotions/update")]
        [HttpPut, Route("promotions/{id:int}")]
        public async Task<IHttpActionResult> UpdatePromotion([FromBody] PromotionDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Promotion data required.");
            if (id.HasValue && dto.PromotionId <= 0) dto.PromotionId = id.Value;
            var success = await _promotionService.UpdatePromotionAsync(dto);
            return Ok(new { success });
        }

        [HttpPost, Route("promotions/delete/{id:int}")]
        [HttpDelete, Route("promotions/{id:int}")]
        public async Task<IHttpActionResult> DeletePromotion(int id)
        {
            var success = await _promotionService.DeletePromotionAsync(id);
            return Ok(new { success });
        }

        #endregion
    }
}