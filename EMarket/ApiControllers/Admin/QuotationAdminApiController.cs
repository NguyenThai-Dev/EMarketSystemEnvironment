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

        // ============================================================
        #region Quotation Read-Only APIs (For AI Assistant)
        // ============================================================

        /// <summary>
        /// Tra cứu danh sách báo giá với các bộ lọc: từ khóa, chi nhánh, trạng thái và thời gian.
        /// </summary>
        /// <param name="keyword">Tên khách hàng hoặc mã báo giá.</param>
        /// <param name="branchId">ID chi nhánh.</param>
        /// <param name="status">Trạng thái: Pending, Approved, Rejected, Converted...</param>
        [HttpGet]
        [Route("list")]
        public async Task<IHttpActionResult> GetQuotations(
            string keyword = null,
            int? branchId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var data = await _quotationService.GetAllQuotationsAsync(keyword, branchId, status, fromDate, toDate);
            return Ok(data);
        }

        /// <summary>
        /// Lấy chi tiết thông tin một bản báo giá theo ID (Bao gồm danh sách sản phẩm, đơn giá).
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetQuotationDetail(int id)
        {
            var data = await _quotationService.GetQuotationByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Conversion Logic (Actionable AI)
        // ============================================================

        /// <summary>
        /// Chuyển đổi một báo giá thành đơn hàng chính thức (Checkout).
        /// AI có thể gọi API này nếu người dùng yêu cầu: "Chốt đơn từ báo giá số #123 cho tôi".
        /// </summary>
        [HttpPost]
        [Route("{id:int}/convert-to-order")]
        public async Task<IHttpActionResult> ConvertToOrder(int id, [FromBody] int userId)
        {
            var result = await _quotationService.ConvertQuotationToOrderAsync(id, userId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        #endregion
    }
}