using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.CustomerModule.DTOs;
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

        #region Customer Core APIs

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            var data = await _customerService.GetAllCustomerAsync();
            return Ok(data);
        }

        [HttpGet]
        [Route("search")]
        [Route("paged")]
        public async Task<IHttpActionResult> SearchCustomers(int start = 0, int length = 10, string keyword = null)
        {
            var pageSize = Math.Min(length > 0 ? length : 10, 100);
            var all = await _customerService.GetAllCustomerFilteredAsync(keyword ?? "");
            var totalCount = all.Count;
            var pagedData = all.Skip(start).Take(pageSize).ToList();
            return Ok(new { total = totalCount, filtered = totalCount, data = pagedData });
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetCustomerById(int id)
        {
            var data = await _customerService.GetCustomerByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpGet]
        [Route("{id:int}/email")]
        public async Task<IHttpActionResult> GetCustomerEmail(int id)
        {
            var email = await _customerService.GetCustomerEmailAsync(id);
            return Ok(email);
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> CreateCustomer([FromBody] CustomerCreateDTO dto)
        {
            if (dto == null) return BadRequest("Customer data required.");
            var id = await _customerService.CreateCustomerAsync(dto, null);
            return Ok(new { success = id > 0, customerId = id });
        }

        [HttpPost, Route("update")]
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> UpdateCustomer([FromBody] CustomerUpdateDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Customer data required.");
            if (id.HasValue && dto.CustomerId <= 0) dto.CustomerId = id.Value;
            var success = await _customerService.UpdateCustomerAsync(dto, null);
            return Ok(new { success });
        }

        [HttpPost, Route("delete/{id:int}")]
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            var success = await _customerService.DeleteCustomerAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Customer Dashboard & Analytics

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

        [HttpGet]
        [Route("count-in-month")]
        public async Task<IHttpActionResult> CountCreatedInMonth(DateTime fromDate, DateTime toDate)
        {
            var count = await _customerService.CountCreatedInMonthAsync(fromDate, toDate);
            return Ok(new { Count = count, FromDate = fromDate, ToDate = toDate });
        }

        [HttpGet]
        [Route("segments")]
        public async Task<IHttpActionResult> GetCustomerSegments()
        {
            var data = await _customerService.GetCustomerSegmentsAsync();
            return Ok(data);
        }

        [HttpGet]
        [Route("created-by-month")]
        public async Task<IHttpActionResult> GetCustomersCreatedByMonth()
        {
            var data = await _customerService.GetCustomerCreatedByMonthAsync();
            return Ok(data);
        }

        [HttpGet]
        [Route("top")]
        public async Task<IHttpActionResult> GetTopCustomers(int top = 10)
        {
            var data = await _customerService.GetTopCustomersAsync(top);
            return Ok(data);
        }

        #endregion

        #region Customer Address APIs

        [HttpGet]
        [Route("address/by-customer/{customerId:int}")]
        public async Task<IHttpActionResult> GetAddressesByCustomer(int customerId)
        {
            var data = await _addressService.GetCustomerAddressAsync(customerId);
            return Ok(data);
        }

        [HttpGet]
        [Route("address/default/{customerId:int}")]
        public async Task<IHttpActionResult> GetDefaultAddress(int customerId)
        {
            var data = await _addressService.GetDefaultCustomerAddressAsync(customerId);
            return Ok(data);
        }

        [HttpGet]
        [Route("address/{id:int}")]
        public async Task<IHttpActionResult> GetAddressById(int id)
        {
            var data = await _addressService.GetCustomerAddressByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost, Route("address/create")]
        public async Task<IHttpActionResult> CreateAddress([FromBody] CustomerAddressCreateDTO dto)
        {
            if (dto == null) return BadRequest("Address data required.");
            var success = await _addressService.CreateCustomerAddressAsync(dto);
            return Ok(new { success });
        }

        [HttpPost, Route("address/update")]
        [HttpPut, Route("address/{id:int}")]
        public async Task<IHttpActionResult> UpdateAddress([FromBody] CustomerAddressUpdateDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Address data required.");
            if (id.HasValue && dto.AddressId <= 0) dto.AddressId = id.Value;
            var success = await _addressService.UpdateCustomerAddressAsync(dto);
            return Ok(new { success });
        }

        [HttpPost, Route("address/delete/{id:int}")]
        [HttpDelete, Route("address/{id:int}")]
        public async Task<IHttpActionResult> DeleteAddress(int id)
        {
            var success = await _addressService.DeleteCustomerAddressAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Loyalty Program APIs

        [HttpGet]
        [Route("loyalty")]
        public async Task<IHttpActionResult> GetAllLoyalty()
        {
            var data = await _loyaltyService.GetAllLoyaltyAsync();
            return Ok(data);
        }

        [HttpGet]
        [Route("loyalty/{id:int}")]
        public async Task<IHttpActionResult> GetLoyaltyById(int id)
        {
            var data = await _loyaltyService.GetLoyaltyByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost, Route("loyalty/create")]
        public async Task<IHttpActionResult> CreateLoyalty([FromBody] LoyaltyProgramDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu chương trình tích điểm không hợp lệ.");
            var ok = await _loyaltyService.CreateLoyaltyAsync(dto);
            return Ok(new { success = ok });
        }

        [HttpPost, Route("loyalty/update")]
        [HttpPut, Route("loyalty/{id:int}")]
        public async Task<IHttpActionResult> UpdateLoyalty([FromBody] LoyaltyProgramDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Dữ liệu chương trình tích điểm không hợp lệ.");
            if (id.HasValue && dto.LoyaltyId <= 0) dto.LoyaltyId = id.Value;
            var ok = await _loyaltyService.UpdateLoyaltyAsync(dto);
            return Ok(new { success = ok });
        }

        [HttpPost, Route("loyalty/delete/{id:int}")]
        [HttpDelete, Route("loyalty/{id:int}")]
        public async Task<IHttpActionResult> DeleteLoyalty(int id)
        {
            var ok = await _loyaltyService.DeleteLoyaltyAsync(id);
            return Ok(new { success = ok });
        }

        #endregion
    }
}
