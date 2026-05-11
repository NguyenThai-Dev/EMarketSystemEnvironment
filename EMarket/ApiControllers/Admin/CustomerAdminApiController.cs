using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.CustomerModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// Read-only API for Customer, Address, and Loyalty Program data.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/admin/customer")]
    public class CustomerAdminApiController : ApiController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerAddressService _addressService;
        private readonly ILoyaltyProgramService _loyaltyService;

        public CustomerAdminApiController(
            ICustomerService customerService,
            ICustomerAddressService addressService,
            ILoyaltyProgramService loyaltyService)
        {
            _customerService = customerService;
            _addressService = addressService;
            _loyaltyService = loyaltyService;
        }

        // ============================================================
        #region Customer Core APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ danh sách khách hàng.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            var data = await _customerService.GetAllCustomerAsync();
            return Ok(data);
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo từ khóa (Tên, SĐT, Email).
        /// </summary>
        [HttpGet]
        [Route("search")]
        public async Task<IHttpActionResult> SearchCustomers(string keyword = null)
        {
            var data = await _customerService.GetAllCustomerFilteredAsync(keyword ?? "");
            return Ok(data);
        }

        /// <summary>
        /// Lấy thông tin chi tiết khách hàng theo ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetCustomerById(int id)
        {
            var data = await _customerService.GetCustomerByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        /// <summary>
        /// Lấy email của khách hàng theo ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}/email")]
        public async Task<IHttpActionResult> GetCustomerEmail(int id)
        {
            var email = await _customerService.GetCustomerEmailAsync(id);
            return Ok(email);
        }

        #endregion

        // ============================================================
        #region Customer Dashboard & Analytics
        // ============================================================

        /// <summary>
        /// Lấy thống kê tổng hợp khách hàng: Tổng số, VIP, khách mới trong khoảng thời gian.
        /// </summary>
        [HttpGet]
        [Route("stats")]
        public async Task<IHttpActionResult> GetCustomerStats(DateTime? fromDate = null)
        {
            var from = fromDate ?? DateTime.Today.AddDays(-30);
            var total = await _customerService.CountAllAsync();
            var vip = await _customerService.CountVipAsync();
            var newCustomers = await _customerService.CountCreatedFromAsync(from);

            return Ok(new
            {
                TotalCustomers = total,
                VipCustomers = vip,
                NewCustomersSinceDate = newCustomers,
                SinceDate = from
            });
        }

        /// <summary>
        /// Đếm khách hàng tạo mới trong khoảng tháng cụ thể.
        /// </summary>
        [HttpGet]
        [Route("count-in-month")]
        public async Task<IHttpActionResult> CountCreatedInMonth(DateTime fromDate, DateTime toDate)
        {
            var count = await _customerService.CountCreatedInMonthAsync(fromDate, toDate);
            return Ok(new { Count = count, FromDate = fromDate, ToDate = toDate });
        }

        /// <summary>
        /// Lấy phân khúc khách hàng (VIP, Thường, Mới...).
        /// </summary>
        [HttpGet]
        [Route("segments")]
        public async Task<IHttpActionResult> GetCustomerSegments()
        {
            var data = await _customerService.GetCustomerSegmentsAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy thống kê số lượng khách hàng tạo theo từng tháng (dùng vẽ biểu đồ).
        /// </summary>
        [HttpGet]
        [Route("created-by-month")]
        public async Task<IHttpActionResult> GetCustomersCreatedByMonth()
        {
            var data = await _customerService.GetCustomerCreatedByMonthAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy top khách hàng theo tiêu chí hệ thống (Doanh thu, Đơn hàng...).
        /// </summary>
        [HttpGet]
        [Route("top")]
        public async Task<IHttpActionResult> GetTopCustomers(int top = 10)
        {
            var data = await _customerService.GetTopCustomersAsync(top);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Customer Address APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ địa chỉ của một khách hàng.
        /// </summary>
        [HttpGet]
        [Route("address/by-customer/{customerId:int}")]
        public async Task<IHttpActionResult> GetAddressesByCustomer(int customerId)
        {
            var data = await _addressService.GetCustomerAddressAsync(customerId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy địa chỉ mặc định của khách hàng (dùng cho giao hàng).
        /// </summary>
        [HttpGet]
        [Route("address/default/{customerId:int}")]
        public async Task<IHttpActionResult> GetDefaultAddress(int customerId)
        {
            var data = await _addressService.GetDefaultCustomerAddressAsync(customerId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy chi tiết một địa chỉ theo ID.
        /// </summary>
        [HttpGet]
        [Route("address/{id:int}")]
        public async Task<IHttpActionResult> GetAddressById(int id)
        {
            var data = await _addressService.GetCustomerAddressByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Loyalty Program APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ chương trình Loyalty (Khách hàng thân thiết).
        /// </summary>
        [HttpGet]
        [Route("loyalty")]
        public async Task<IHttpActionResult> GetAllLoyalty()
        {
            var data = await _loyaltyService.GetAllLoyaltyAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy chi tiết chương trình Loyalty theo ID.
        /// </summary>
        [HttpGet]
        [Route("loyalty/{id:int}")]
        public async Task<IHttpActionResult> GetLoyaltyById(int id)
        {
            var data = await _loyaltyService.GetLoyaltyByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion
    }
}
