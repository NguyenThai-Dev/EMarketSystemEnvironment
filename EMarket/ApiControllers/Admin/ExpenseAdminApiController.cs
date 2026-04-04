using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.ExpenseModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// API dành cho việc đọc dữ liệu chi phí (Expense) trong hệ thống quản trị.
    /// </summary>
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
        /// Lấy danh sách chi phí với các bộ lọc tùy chọn: chi nhánh, danh mục, ngày, trạng thái.
        /// </summary>
        /// <param name="branchId">ID chi nhánh.</param>
        /// <param name="categoryId">ID danh mục chi phí.</param>
        /// <param name="fromDate">Ngày bắt đầu.</param>
        /// <param name="toDate">Ngày kết thúc.</param>
        /// <param name="status">Trạng thái: pending | approved | rejected | paid.</param>
        [HttpGet]
        [Route("list")]
        public async Task<IHttpActionResult> GetExpenses(
            int? branchId = null,
            int? categoryId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string status = null)
        {
            var data = await _expenseService.GetExpensesAsync(
                branchId,
                categoryId,
                fromDate,
                toDate,
                status);

            return Ok(data);
        }

        #endregion


        // ======================================================
        #region Expense Detail
        // ======================================================

        /// <summary>
        /// Lấy thông tin chi tiết một khoản chi phí.
        /// </summary>
        /// <param name="id">ID chi phí.</param>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetExpenseDetail(int id)
        {
            var data = await _expenseService.GetExpenseByIdAsync(id);
            return Ok(data);
        }

        #endregion


        // ======================================================
        #region Expense Categories
        // ======================================================

        /// <summary>
        /// Lấy danh sách tất cả danh mục chi phí.
        /// </summary>
        [HttpGet]
        [Route("categories")]
        public async Task<IHttpActionResult> GetExpenseCategories()
        {
            var data = await _expenseService.GetAllExpenseCategoriesAsync();
            return Ok(data);
        }

        #endregion
    }
}
