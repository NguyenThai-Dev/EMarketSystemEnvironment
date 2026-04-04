using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.CustomerModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [AllowAnonymous]
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
        #region Customer APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ khách hàng.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            var data = await _customerService.GetAllCustomerAsync();
            return Ok(data);
        }

        /// <summary>
        /// Tìm kiếm khách hàng.
        /// </summary>
        [HttpGet]
        [Route("search")]
        public async Task<IHttpActionResult> Search(string keyword)
        {
            var data = await _customerService.GetAllCustomerFilteredAsync(keyword ?? "");
            return Ok(data);
        }

        /// <summary>
        /// Lấy chi tiết khách hàng theo ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetCustomerById(int id)
        {
            var data = await _customerService.GetCustomerByIdAsync(id);
            if (data == null)
                return NotFound();

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

        /// <summary>
        /// Lấy danh sách phân khúc khách hàng.
        /// </summary>
        [HttpGet]
        [Route("segments")]
        public async Task<IHttpActionResult> GetCustomerSegments()
        {
            var data = await _customerService.GetCustomerSegmentsAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy thống kê số lượng khách hàng tạo theo từng tháng.
        /// </summary>
        [HttpGet]
        [Route("created-by-month")]
        public async Task<IHttpActionResult> GetCustomersCreatedByMonth()
        {
            var data = await _customerService.GetCustomerCreatedByMonthAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy top khách hàng theo tiêu chí hệ thống.
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



        // ============================================================
        #region Customer Address APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ địa chỉ của khách hàng.
        /// </summary>
        [HttpGet]
        [Route("address/by-customer/{customerId:int}")]
        public async Task<IHttpActionResult> GetAddressesByCustomer(int customerId)
        {
            var data = await _addressService.GetCustomerAddressAsync(customerId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy địa chỉ mặc định của khách hàng.
        /// </summary>
        [HttpGet]
        [Route("address/default/{customerId:int}")]
        public async Task<IHttpActionResult> GetDefaultAddress(int customerId)
        {
            var data = await _addressService.GetDefaultCustomerAddressAsync(customerId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy địa chỉ theo ID.
        /// </summary>
        [HttpGet]
        [Route("address/{id:int}")]
        public async Task<IHttpActionResult> GetAddressById(int id)
        {
            var data = await _addressService.GetCustomerAddressByIdAsync(id);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        #endregion
        // ============================================================



        // ============================================================
        #region Loyalty Program APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ chương trình Loyalty.
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
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        #endregion
        // ============================================================
    }
}
