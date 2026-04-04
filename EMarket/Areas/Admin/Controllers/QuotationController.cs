using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.QuotationModule.DTOs;
using EMarket.Modules.QuotationModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;
using Newtonsoft.Json;

namespace EMarket.Areas.Admin.Controllers
{
    public class QuotationController : Controller
    {
        private readonly IQuotationService _quoteService;
        private readonly IBranchService _branchService;
        private readonly ILoginService _loginService;

        public QuotationController(IQuotationService quoteService, IBranchService branchService, ILoginService loginService)
        {
            _quoteService = quoteService;
            _branchService = branchService;
            _loginService = loginService;
        }

        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> QuotationManagement()
        {
            ViewBag.Branches = new SelectList(await _branchService.GetAllBranchesAsync(), "BranchId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetQuotationList(string keyword, int? branchId, string status, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var data = await _quoteService.GetAllQuotationsAsync(keyword, branchId, status, fromDate, toDate);
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<object>(), error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetQuotationDetail(int id)
        {
            try
            {
                var data = await _quoteService.GetQuotationByIdAsync(id);
                if (data == null) return Json(new { success = false, message = "Không tìm thấy dữ liệu" }, JsonRequestBehavior.AllowGet);
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<JsonResult> SaveQuotation()
        {
            try
            {
                // 1. TỰ ĐỌC JSON BODY TỪ STREAM
                string json;
                // Đảm bảo con trỏ stream ở vị trí bắt đầu
                Request.InputStream.Position = 0;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    json = await reader.ReadToEndAsync();
                }

                // 2. Kiểm tra chuỗi JSON có rỗng không
                if (string.IsNullOrEmpty(json))
                {
                    return Json(new { success = false, message = "Dữ liệu JSON trống!" });
                }

                // 3. Convert JSON chuỗi thành Object C#
                // Sử dụng cấu hình IsoDateTimeConverter để tránh lỗi định dạng ngày tháng
                var settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                };
                var dto = JsonConvert.DeserializeObject<QuotationDTO>(json, settings);

                // 4. Validate thủ công sau khi convert
                if (dto == null || dto.Details == null || dto.Details.Count == 0)
                {
                    return Json(new { success = false, message = "Dữ liệu báo giá không hợp lệ hoặc chi tiết hàng hóa trống!" });
                }

                // 5. Gán UserId từ login session
                dto.UserId = _loginService.GetCurrentUserId() ?? 0;

                // 6. Gọi Service xử lý lưu vào DB
                var newId = await _quoteService.CreateQuotationAsync(dto);

                if (newId > 0)
                {
                    return Json(new { success = true, message = "Lưu báo giá thành công!", quotationId = newId });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể lưu dữ liệu vào cơ sở dữ liệu." });
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết để dễ debug (Trong môi trường production nên ẩn bớt)
                return Json(new { success = false, message = "Lỗi Server: " + ex.Message });
            }
        }
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<JsonResult> UpdateQuotation()
        {
            try
            {
                // 1. TỰ ĐỌC JSON BODY TỪ STREAM (Giống hệt SaveQuotation)
                string json;
                Request.InputStream.Position = 0;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    json = await reader.ReadToEndAsync();
                }

                // 2. Kiểm tra chuỗi JSON
                if (string.IsNullOrEmpty(json))
                {
                    return Json(new { success = false, message = "Dữ liệu gửi lên bị trống!" });
                }

                // 3. Convert JSON -> DTO
                var settings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                };
                var dto = JsonConvert.DeserializeObject<QuotationDTO>(json, settings);

                // 4. Validate dữ liệu
                if (dto == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
                }

                // [QUAN TRỌNG] Update thì bắt buộc phải có ID
                if (dto.QuotationId <= 0)
                {
                    return Json(new { success = false, message = "Không xác định được báo giá cần sửa (Thiếu ID)." });
                }

                if (dto.Details == null || dto.Details.Count == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 sản phẩm." });
                }

                // 5. Gán UserId (Người sửa)
                dto.UserId = _loginService.GetCurrentUserId() ?? 0;

                // 6. Gọi Service Update
                // Hàm này trả về bool (true = thành công, false = thất bại do sai trạng thái hoặc không tìm thấy)
                var isSuccess = await _quoteService.UpdateQuotationAsync(dto);

                if (isSuccess)
                {
                    return Json(new { success = true, message = "Cập nhật báo giá thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật. Có thể báo giá đã bị xóa, đã chốt đơn hoặc bị hủy." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi Server: " + ex.Message });
            }
        }
        // 5. API: CHỐT ĐƠN
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<ActionResult> ConvertToOrder(int id)
        {
            try
            {
                var userId = _loginService.GetCurrentUserId() ?? 0;
                var result = await _quoteService.ConvertQuotationToOrderAsync(id, userId);
                return Json(new { success = result.Success, result.OrderId, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<JsonResult> ChangeStatus(int id, string status)
        {
            try
            {
                var result = await _quoteService.ChangeStatusAsync(id, status);
                if (result)
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công." });
                else
                    return Json(new { success = false, message = "Không tìm thấy báo giá hoặc trạng thái không hợp lệ." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "SalesModule")]
        public async Task<JsonResult> DeleteQuotation(int id)
        {
            try
            {
                var result = await _quoteService.DeleteQuotationAsync(id);
                if (result)
                    return Json(new { success = true, message = "Đã xóa báo giá." });
                else
                    return Json(new { success = false, message = "Chỉ có thể xóa báo giá ở trạng thái Nháp." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}