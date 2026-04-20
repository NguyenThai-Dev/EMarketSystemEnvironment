using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
            ViewBag.Categories = new SelectList(await _expenseService.GetActiveExpenseCategoriesAsync(), "CategoryId", "Name");
            ViewBag.Branches = new SelectList(await _branchService.GetAllBranchesAsync(), "BranchId", "Name");
            return View();
        }

        [EMarketAuthorize(Module = "ReportModule")]
        public async Task<ActionResult> ExpenseCategoryManagement()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetExpenseList(int? branchId, int? categoryId, DateTime? fromDate, DateTime? toDate, string status)
        {
            try
            {
                var data = await _expenseService.GetExpensesAsync(branchId, categoryId, fromDate, toDate, status);
                return Json(new { data = data }); 
            }
            catch (Exception ex)
            {
                return Json(new { data = new object[] { }, error = ex.Message });
            }
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> getAllExpenseCategory()
        {
            try
            {
                var categories = await _expenseService.GetAllExpenseCategoriesAsync();
                return Json(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetExpenseCategoryById(int id)
        {
            try
            {
                var expense = await _expenseService.GetExpenseByIdAsync(id);
                if (expense == null)
                    return Json(new { success = false, message = "Không tìm thấy chi phí." }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, data = expense }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ReportModule")]
        public async Task<ActionResult> CreateOrUpdateCategory(ExpenseCategoryDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Json(new { success = false, message = "Tên danh mục không được để trống." });
                bool success;

                if (dto.CategoryId > 0)
                {
                    success = await _expenseService.UpdateExpenseCategoryAsync(dto);
                    return Json(new { success, message = success ? "Cập nhật danh mục thành công!" : "Lỗi cập nhật." });
                }
                else
                {
                    success = await _expenseService.CreateExpenseCategoryAsync(dto);
                    return Json(new { success, message = success ? "Tạo danh mục thành công!" : "Lỗi tạo mới." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ReportModule")]

        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _expenseService.DeleteExpenseCategoryAsync(id);
                return Json(new { success, message = success ? "Đã xóa danh mục." : "Không thể xóa danh mục." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
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
