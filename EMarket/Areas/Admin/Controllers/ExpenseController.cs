using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.ExpenseModule.DTOs;
using EMarket.Modules.ExpenseModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly IBranchService _branchService;
        private readonly ILoginService _loginService;

        public ExpenseController(IExpenseService expenseService, IBranchService branchService, ILoginService loginService)
        {
            _expenseService = expenseService;
            _branchService = branchService;
            _loginService = loginService;
        }

        [EMarketAuthorize(Module = "ReportModule")]
        public async Task<ActionResult> ExpenseManagement()
        {
            ViewBag.Categories = new SelectList(await _expenseService.GetAllExpenseCategoriesAsync(), "CategoryId", "Name");
            ViewBag.Branches = new SelectList(await _branchService.GetAllBranchesAsync(), "BranchId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetExpenseList(int? branchId, int? categoryId, DateTime? fromDate, DateTime? toDate, string status)
        {
            try
            {
                var data = await _expenseService.GetExpensesAsync(branchId, categoryId, fromDate, toDate, status);
                return Json(new { data = data }); // Trả về { data: [...] } đúng chuẩn DataTable
            }
            catch (Exception ex)
            {
                return Json(new { data = new object[] { }, error = ex.Message });
            }
        }

        // 3. TẠO MỚI (AJAX + FormData)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ReportModule")]
        public async Task<ActionResult> Create(ExpenseDTO dto, HttpPostedFileBase refImageFile)
        {
            try
            {
                // Validate Server-side
                if (dto.Amount < 1000) return Json(new { success = false, message = "Số tiền không hợp lệ." });

                dto.UserId = _loginService.GetCurrentUserId() ?? 0;

                // Xử lý ảnh (Dùng Helper chung hoặc hàm private bên dưới)
                if (refImageFile != null && refImageFile.ContentLength > 0)
                {
                    dto.RefImage = SaveExpenseImage(refImageFile);
                }

                var success = await _expenseService.CreateExpenseAsync(dto);

                return Json(new { success, message = success ? "Tạo phiếu chi thành công!" : "Lỗi lưu dữ liệu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // 4. DUYỆT / XÓA
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ReportModule")]
        public async Task<ActionResult> UpdateStatus(UpdateExpenseStatusRequest request)
        {
            var userId = _loginService.GetCurrentUserId() ?? 0;

            try
            {
                await _expenseService.UpdateStatusAsync(
                    request.ExpenseId,
                    request.Status,
                    userId,
                    request.RejectReason,
                    request.PaymentMethod
                );

                return Json(new
                {
                    success = true,
                    message = request.Status == ExpenseStatus.Approved
                        ? "Đã duyệt chi phí."
                        : "Đã từ chối chi phí."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ReportModule")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _expenseService.DeleteExpenseAsync(id);
            return Json(new { success, message = success ? "Đã xóa." : "Không thể xóa." });
        }

        #region Helpers
        private string SaveExpenseImage(HttpPostedFileBase file)
        {
            // Logic tạo folder theo Năm/Tháng để tránh quá tải folder
            var relativeFolder = $"~/Uploads/Expenses/{DateTime.Now:yyyy}/{DateTime.Now:MM}";
            var absoluteFolder = Server.MapPath(relativeFolder);

            if (!System.IO.Directory.Exists(absoluteFolder))
                System.IO.Directory.CreateDirectory(absoluteFolder);

            var fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(file.FileName)}";
            var savePath = System.IO.Path.Combine(absoluteFolder, fileName);

            file.SaveAs(savePath);
            return $"{relativeFolder.TrimStart('~')}/{fileName}"; // Trả về đường dẫn web
        }
        #endregion
    }
}
