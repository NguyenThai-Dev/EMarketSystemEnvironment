using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly ISupplierServiceDebtAndPaymentService _supplierDebtAndPaymentService;
        private readonly ILoginService _loginService;


        public SupplierController(ISupplierService supplierService, ISupplierServiceDebtAndPaymentService supplierServiceDebtAndPaymentService, ILoginService loginService)
        {
            _supplierService = supplierService;
            _supplierDebtAndPaymentService = supplierServiceDebtAndPaymentService;
            _loginService = loginService;
        }

        #region Supplier

        [EMarketAuthorize(Module = "InventoryModule, ProductModule")]
        public ActionResult SupplierList()
        {
            return View();
        }


        // DataTable loader
        public async Task<JsonResult> GetAllSupplier()
        {
            var data = await _supplierService.GetAllSupplierAsync();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetfilteredSupplier(string supplierName)
        {
            var data = await _supplierService.GetFilteredSupplierAsync(supplierName);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "InventoryModule, ProductModule")]
        public async Task<JsonResult> CreateSupplier(SupplierDTO dto)
        {
            var result = await _supplierService.CreateSupplierAsync(dto);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "InventoryModule, ProductModule")]
        public async Task<JsonResult> UpdateSupplier(SupplierDTO dto)
        {
            var result = await _supplierService.UpdateSupplierAsync(dto);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "InventoryModule, ProductModule")]
        public async Task<JsonResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            return Json(new { success = result });
        }

        #endregion

        #region Supplier Debt Report View

        [EMarketAuthorize(Module = "DebtModule")]
        public ActionResult SupplierDebtAndPayment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetAllDebt(
            string keyword,
    int? supplierId,
    string status,
    DateTime? fromDate,
    DateTime? toDate
)
        {

            var data = await _supplierDebtAndPaymentService.GetAllSupplierDebtsAsync(
                keyword,
                supplierId,
                status,
                fromDate,
                toDate
            );

            return Json(new
            {
                success = true,
                data
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> GetSupplierDebtById(int id)
        {
            var item = await _supplierDebtAndPaymentService.GetSupplierDebtByIdAsync(id);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetSupplierDebtByPurchaseOrder(int purchaseOrderId)
        {
            var item = await _supplierDebtAndPaymentService.GetSupplierDebtByPurchaseOrderIdAsync(purchaseOrderId);
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "DebtModule")]
        public async Task<ActionResult> CreateSupplierDebt(SupplierDebtDTO dto)
        {
            var ok = await _supplierDebtAndPaymentService.CreateSupplierDebtAsync(dto);
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "DebtModule")]
        public async Task<ActionResult> UpdateSupplierDebt(SupplierDebtDTO dto)
        {
            var ok = await _supplierDebtAndPaymentService.UpdateSupplierDebtAsync(dto);
            return Json(new { success = ok });
        }

        public async Task<ActionResult> GetPaymentByDebt(int debtId)
        {
            var list = await _supplierDebtAndPaymentService.GetPaymentsByDebtIdAsync(debtId);
            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "DebtModule")]
        public async Task<ActionResult> CreateSupplierPayment(
     SupplierPaymentDTO dto,
     HttpPostedFileBase ProofImage
 )
        {
            var currentUserId = _loginService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException();
            var currentUser = await _loginService.GetUserByIdAsync(currentUserId.Value);

            if (currentUser.IsSupplier)
                throw new InvalidOperationException("Supplier không được phép tạo thanh toán");

            if (ProofImage != null && ProofImage.ContentLength > 0)
            {
                dto.PaymentProof = SavePaymentProof(ProofImage, dto.DebtId);
            }

            var ok = await _supplierDebtAndPaymentService
                .CreateSupplierPaymentAsync(dto);

            return Json(new { success = ok });
        }

        private string SavePaymentProof(HttpPostedFileBase file, int debtId)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            // Root lưu chứng từ thanh toán NCC
            var rootFolder = "~/Uploads/SupplierPayments";

            // Folder theo DebtId
            var debtFolder = Path.Combine(rootFolder, debtId.ToString());

            // Map ra physical path
            var physicalDebtFolder = Server.MapPath(debtFolder);

            // Đảm bảo folder tồn tại
            if (!Directory.Exists(physicalDebtFolder))
            {
                Directory.CreateDirectory(physicalDebtFolder);
            }

            // Tên file an toàn, không trùng
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{extension}";

            var fullPhysicalPath = Path.Combine(physicalDebtFolder, fileName);

            file.SaveAs(fullPhysicalPath);

            // Trả về path dạng virtual để lưu DB
            return VirtualPathUtility.ToAbsolute(
                $"{debtFolder}/{fileName}"
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "DebtModule")]
        public async Task<ActionResult> DeleteSupplierPayment(int id)
        {
            var currentUserId = _loginService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException();
            var currentUser = await _loginService.GetUserByIdAsync(currentUserId.Value);

            if (currentUser.IsSupplier)
                throw new InvalidOperationException("Supplier không được phép xóa thanh toán");

            var ok = await _supplierDebtAndPaymentService.DeleteSupplierPaymentAsync(id);
            return Json(new { success = ok });
        }

        #endregion
    }
}
