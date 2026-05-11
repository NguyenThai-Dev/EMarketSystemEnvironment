using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.SalesModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [Authorize]
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

        /// <summary>
        /// Lấy toàn bộ đơn hàng trong hệ thống.
        /// </summary>
        [HttpGet, Route("orders/all")]
        public async Task<IHttpActionResult> GetAllOrders()
        { return Ok(await _orderService.GetAllOrdersAsync()); }

        /// <summary>
        /// Lấy danh sách đơn hàng với bộ lọc nâng cao (DataTable).
        /// </summary>
        [HttpGet, Route("orders")]
        public async Task<IHttpActionResult> GetOrders(int start = 0, int length = 10, int? userId = null, int? branchId = null, string status = null, DateTime? fromDate = null, DateTime? toDate = null, string keyword = null)
        {
            var r = await _orderService.GetOrdersDataTableAsync(1, start, length, userId, branchId, status, fromDate, toDate, keyword);
            return Ok(new { r.total, r.filtered, r.data });
        }

        /// <summary>
        /// Lấy chi tiết một đơn hàng kèm danh sách sản phẩm (OrderDetails).
        /// </summary>
        [HttpGet, Route("orders/{id:int}")]
        public async Task<IHttpActionResult> GetOrderDetail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            var details = await _orderService.GetOrderDetailsByOrderIdAsync(id);
            return Ok(new { Order = order, Details = details });
        }

        /// <summary>
        /// Lấy đơn hàng theo chi nhánh và thời gian (phiên bản đầy đủ - Full Join).
        /// </summary>
        [HttpGet, Route("orders/full-by-branch")]
        public async Task<IHttpActionResult> GetFullOrdersByBranch(int? branchId = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _orderService.GetFullOrdersByBranchIdAsync(branchId, fromDate, toDate)); }

        /// <summary>
        /// Lấy đơn hàng theo chi nhánh và thời gian (phiên bản nhẹ).
        /// </summary>
        [HttpGet, Route("orders/by-branch")]
        public async Task<IHttpActionResult> GetOrdersByBranch(int? branchId = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _orderService.GetOrdersByBranchIdAsync(branchId, fromDate, toDate)); }

        /// <summary>
        /// Lấy danh sách chi tiết sản phẩm trong đơn hàng (OrderDetails).
        /// </summary>
        [HttpGet, Route("orders/{orderId:int}/details")]
        public async Task<IHttpActionResult> GetOrderDetails(int orderId)
        { return Ok(await _orderService.GetOrderDetailsByOrderIdAsync(orderId)); }

        #endregion

        #region Payment

        /// <summary>
        /// Lấy lịch sử thanh toán của một đơn hàng.
        /// </summary>
        [HttpGet, Route("orders/{orderId:int}/payments")]
        public async Task<IHttpActionResult> GetPaymentsByOrder(int orderId)
        { return Ok(await _paymentService.GetPaymentsByOrderIdAsync(orderId)); }

        #endregion

        #region Promotions

        /// <summary>
        /// Lấy toàn bộ chương trình khuyến mãi.
        /// </summary>
        [HttpGet, Route("promotions")]
        public async Task<IHttpActionResult> GetAllPromotions()
        { return Ok(await _promotionService.GetAllPromotionsAsync()); }

        /// <summary>
        /// Tìm kiếm khuyến mãi với bộ lọc nâng cao.
        /// </summary>
        [HttpGet, Route("promotions/search")]
        public async Task<IHttpActionResult> SearchPromotions(string keyword = null, int? categoryId = null, string discountType = null, string cusType = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _promotionService.GetFilteredPromotionAsync(keyword, categoryId, discountType, cusType, fromDate, toDate)); }

        /// <summary>
        /// Lấy chi tiết một chương trình khuyến mãi theo ID.
        /// </summary>
        [HttpGet, Route("promotions/{id:int}")]
        public async Task<IHttpActionResult> GetPromotionById(int id)
        {
            var d = await _promotionService.GetPromotionByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        /// <summary>
        /// Lấy danh sách khuyến mãi đang có hiệu lực.
        /// </summary>
        [HttpGet, Route("promotions/active")]
        public async Task<IHttpActionResult> GetActivePromotions()
        { return Ok(await _promotionService.GetActivePromotionsAsync()); }

        #endregion
    }
}