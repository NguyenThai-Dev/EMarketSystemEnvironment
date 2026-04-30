using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.QuotationModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [RoutePrefix("api/admin/quotation")]
    public class QuotationAdminApiController : ApiController
    {
        private readonly IQuotationService _quotationService;

        public QuotationAdminApiController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        /// <summary>
        /// Tra cứu danh sách báo giá với bộ lọc: từ khóa, chi nhánh, trạng thái, thời gian.
        /// </summary>
        [HttpGet, Route("list")]
        public async Task<IHttpActionResult> GetQuotations(string keyword = null, int? branchId = null, string status = null, DateTime? fromDate = null, DateTime? toDate = null)
        { return Ok(await _quotationService.GetAllQuotationsAsync(keyword, branchId, status, fromDate, toDate)); }

        /// <summary>
        /// Lấy chi tiết một báo giá theo ID (bao gồm danh sách sản phẩm, đơn giá).
        /// </summary>
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetQuotationDetail(int id)
        {
            var d = await _quotationService.GetQuotationByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }
    }
}