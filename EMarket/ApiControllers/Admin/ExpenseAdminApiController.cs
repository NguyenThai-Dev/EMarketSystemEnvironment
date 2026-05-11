using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.ExpenseModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// Read-only API for Expense and Expense Category data.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/admin/expense")]
    public class ExpenseAdminApiController : ApiController
    {
        private readonly IExpenseService _expenseService;

        public ExpenseAdminApiController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // ======================================================
        #region Expense List & Filter
        // ======================================================

        /// <summary>
        /// Lấy danh sách chi phí với bộ lọc: chi nhánh, danh mục, ngày, trạng thái.
        /// </summary>
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

        #endregion

        // ======================================================
        #region Expense Detail
        // ======================================================

        /// <summary>
        /// Lấy thông tin chi tiết một khoản chi phí theo ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetExpenseDetail(int id)
        {
            var data = await _expenseService.GetExpenseByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion

        // ======================================================
        #region Expense Categories
        // ======================================================

        /// <summary>
        /// Lấy toàn bộ danh mục chi phí (bao gồm cả Active và Inactive).
        /// </summary>
        [HttpGet]
        [Route("categories")]
        public async Task<IHttpActionResult> GetAllExpenseCategories()
        {
            var data = await _expenseService.GetAllExpenseCategoriesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy chỉ các danh mục chi phí đang hoạt động (Active).
        /// </summary>
        [HttpGet]
        [Route("categories/active")]
        public async Task<IHttpActionResult> GetActiveExpenseCategories()
        {
            var data = await _expenseService.GetActiveExpenseCategoriesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy chi tiết một danh mục chi phí theo ID.
        /// </summary>
        [HttpGet]
        [Route("categories/{categoryId:int}")]
        public async Task<IHttpActionResult> GetExpenseCategoryById(int categoryId)
        {
            var data = await _expenseService.GetExpenseCategoryByIdAsync(categoryId);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion
    }
}
