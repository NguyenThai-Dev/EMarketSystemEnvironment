using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.QuotationModule.DTOs;
using EMarket.Modules.QuotationModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [AllowAnonymous]
    [RoutePrefix("api/admin/quotation")]
    public class QuotationAdminApiController : ApiController
    {
        private readonly IQuotationService _quotationService;

        public QuotationAdminApiController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        [HttpGet, Route("list")]
        public async Task<IHttpActionResult> GetQuotations(string keyword = null, int? branchId = null, string status = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _quotationService.GetAllQuotationsAsync(keyword, branchId, status, fromDate, toDate)); }

        /// <summary>
        /// Lấy danh sách báo giá có phân trang.
        /// </summary>
        [HttpGet, Route("paged")]
        public async Task<IHttpActionResult> GetQuotationsPaged(
            int start = 0,
            int length = 20,
            string keyword = null,
            int? branchId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var pageSize = Math.Min(length > 0 ? length : 20, 100);
            var all = await _quotationService.GetAllQuotationsAsync(keyword, branchId, status, fromDate, toDate);
            var totalCount = all.Count;
            var pagedData = all.Skip(start).Take(pageSize).ToList();
            return Ok(new { total = totalCount, filtered = totalCount, data = pagedData });
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetQuotationDetail(int id)
        {
            var d = await _quotationService.GetQuotationByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost]
        [Route("save")]
        [Route("create")]
        public async Task<IHttpActionResult> SaveQuotation([FromBody] QuotationDTO dto)
        {
            if (dto == null) return BadRequest("Quotation data required.");
            if (dto.QuotationId > 0)
            {
                var ok = await _quotationService.UpdateQuotationAsync(dto);
                return Ok(new { success = ok, quotationId = dto.QuotationId });
            }
            else
            {
                var id = await _quotationService.CreateQuotationAsync(dto);
                return Ok(new { success = id > 0, quotationId = id });
            }
        }

        [HttpPost, Route("update")]
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> UpdateQuotation([FromBody] QuotationDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Quotation data required.");
            if (id.HasValue && dto.QuotationId <= 0) dto.QuotationId = id.Value;
            var success = await _quotationService.UpdateQuotationAsync(dto);
            return Ok(new { success });
        }

        public class ChangeStatusModel
        {
            public string Status { get; set; }
        }

        [HttpPost, Route("{id:int}/status")]
        public async Task<IHttpActionResult> ChangeStatus(int id, [FromBody] ChangeStatusModel model)
        {
            var status = model?.Status ?? "Approved";
            var success = await _quotationService.ChangeStatusAsync(id, status);
            return Ok(new { success });
        }

        [HttpPost, Route("{id:int}/convert-to-order")]
        public async Task<IHttpActionResult> ConvertToOrder(int id, [FromUri] int userId = 1)
        {
            var result = await _quotationService.ConvertQuotationToOrderAsync(id, userId);
            return Ok(result);
        }

        [HttpPost, Route("delete/{id:int}")]
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> DeleteQuotation(int id)
        {
            var success = await _quotationService.DeleteQuotationAsync(id);
            return Ok(new { success });
        }
    }
}