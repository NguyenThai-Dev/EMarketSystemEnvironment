using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.ExpenseModule.DTOs;
using EMarket.Modules.ExpenseModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [AllowAnonymous]
    [RoutePrefix("api/admin/expense")]
    public class ExpenseAdminApiController : ApiController
    {
        private readonly IExpenseService _expenseService;

        public ExpenseAdminApiController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        #region Expense List & Filter

        [HttpGet]
        [Route("list")]
        public async Task<IHttpActionResult> GetExpenses(
            int? branchId = null,
            int? categoryId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string status = null)
        {
            var data = await _expenseService.GetExpensesAsync(branchId, categoryId, fromDate, toDate, status);
            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách phiếu chi có phân trang.
        /// </summary>
        [HttpGet]
        [Route("paged")]
        public async Task<IHttpActionResult> GetExpensesPaged(
            int start = 0,
            int length = 20,
            int? branchId = null,
            int? categoryId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string status = null)
        {
            var pageSize = Math.Min(length > 0 ? length : 20, 100);
            var all = await _expenseService.GetExpensesAsync(branchId, categoryId, fromDate, toDate, status);
            var totalCount = all.Count;
            var pagedData = all.Skip(start).Take(pageSize).ToList();
            return Ok(new { total = totalCount, filtered = totalCount, data = pagedData });
        }

        /// <summary>
        /// Thống kê tổng hợp chi phí theo trạng thái &amp; tổng tiền.
        /// </summary>
        [HttpGet]
        [Route("summary")]
        public async Task<IHttpActionResult> GetExpenseSummary(
            int? branchId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var all = await _expenseService.GetExpensesAsync(branchId, null, fromDate, toDate, null);
            var approved = all.Where(e => string.Equals(e.Status, "Approved", StringComparison.OrdinalIgnoreCase)).ToList();
            var pending = all.Where(e => string.Equals(e.Status, "Pending", StringComparison.OrdinalIgnoreCase)).ToList();
            var rejected = all.Where(e => string.Equals(e.Status, "Rejected", StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(new
            {
                TotalExpenses = all.Count,
                TotalApprovedAmount = approved.Sum(e => e.Amount),
                TotalPendingAmount = pending.Sum(e => e.Amount),
                TotalRejectedAmount = rejected.Sum(e => e.Amount),
                ApprovedCount = approved.Count,
                PendingCount = pending.Count,
                RejectedCount = rejected.Count
            });
        }

        #endregion

        #region Expense Detail

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetExpenseDetail(int id)
        {
            var data = await _expenseService.GetExpenseByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> CreateExpense([FromBody] ExpenseDTO dto)
        {
            if (dto == null) return BadRequest("Expense data required.");
            var success = await _expenseService.CreateExpenseAsync(dto);
            return Ok(new { success });
        }

        [HttpPost]
        [Route("update-status")]
        [Route("{id:int}/status")]
        public async Task<IHttpActionResult> UpdateExpenseStatus([FromBody] UpdateExpenseStatusRequest req, int? id = null)
        {
            if (req == null) return BadRequest("Status update data required.");
            var expenseId = id.HasValue && id.Value > 0 ? id.Value : req.ExpenseId;
            await _expenseService.UpdateStatusAsync(expenseId, req.Status, 1, req.RejectReason, req.PaymentMethod);
            return Ok(new { success = true });
        }

        [HttpPost, Route("delete/{id:int}")]
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> DeleteExpense(int id)
        {
            var success = await _expenseService.DeleteExpenseAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Expense Categories

        [HttpGet]
        [Route("categories")]
        public async Task<IHttpActionResult> GetAllExpenseCategories()
        {
            var data = await _expenseService.GetAllExpenseCategoriesAsync();
            return Ok(data);
        }

        [HttpGet]
        [Route("categories/active")]
        public async Task<IHttpActionResult> GetActiveExpenseCategories()
        {
            var data = await _expenseService.GetActiveExpenseCategoriesAsync();
            return Ok(data);
        }

        [HttpGet]
        [Route("categories/{categoryId:int}")]
        public async Task<IHttpActionResult> GetExpenseCategoryById(int categoryId)
        {
            var data = await _expenseService.GetExpenseCategoryByIdAsync(categoryId);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost, Route("categories/create")]
        public async Task<IHttpActionResult> CreateExpenseCategory([FromBody] ExpenseCategoryDTO dto)
        {
            if (dto == null) return BadRequest("Category data required.");
            var success = await _expenseService.CreateExpenseCategoryAsync(dto);
            return Ok(new { success });
        }

        [HttpPost, Route("categories/update")]
        [HttpPut, Route("categories/{id:int}")]
        public async Task<IHttpActionResult> UpdateExpenseCategory([FromBody] ExpenseCategoryDTO dto, int? id = null)
        {
            if (dto == null) return BadRequest("Category data required.");
            if (id.HasValue && dto.CategoryId <= 0) dto.CategoryId = id.Value;
            var success = await _expenseService.UpdateExpenseCategoryAsync(dto);
            return Ok(new { success });
        }

        [HttpPost, Route("categories/delete/{categoryId:int}")]
        [HttpDelete, Route("categories/{categoryId:int}")]
        public async Task<IHttpActionResult> DeleteExpenseCategory(int categoryId)
        {
            var success = await _expenseService.DeleteExpenseCategoryAsync(categoryId);
            return Ok(new { success });
        }

        #endregion
    }
}
