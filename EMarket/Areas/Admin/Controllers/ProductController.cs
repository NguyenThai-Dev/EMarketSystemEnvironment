using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BarcodeStandard;
using EMarket.Filters;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;
using SkiaSharp;


namespace EMarket.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {

        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;
        private readonly IProductCategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private readonly IBranchService _branchService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUserContext _userContext;

        public ProductController(IProductService productService,
            IInventoryService inventoryService,
            IProductCategoryService categoryService,
            ISupplierService supplierService,
            IBranchService branchService,
            IWarehouseService warehouseService,
            IUserContext userContext)
        {
            _productService = productService;
            _inventoryService = inventoryService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _branchService = branchService;
            _warehouseService = warehouseService;
            _userContext = userContext;
        }

        #region Product List
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult ProductList()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetAllProduct(string keyWord, int? categoryId, int? branchId, int? supplierId, int? warehouseId)
        {
            var products = await _productService.GetFilteredProductAsync(keyWord, categoryId, branchId, supplierId, warehouseId);
            return Json(new { products }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> LoadDropdowns(int? branchId)
        {
            var categories = await _categoryService.GetAllProductCategoryAsync();
            var suppliers = await _supplierService.GetAllSupplierAsync();
            var branches = await _branchService.GetAllBranchesAsync();

            List<WarehouseDTO> warehouses;

            if (branchId.HasValue && branchId > 0)
                warehouses = await _warehouseService.GetAllWarehouseByBranchId(branchId.Value);
            else
                warehouses = await _warehouseService.GetAllWarehousesByBranchIdAsync();

            return Json(new
            {
                categories,
                suppliers,
                branches,
                warehouses
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> LoadWarehousesByBranch(int branchId)
        {
            var warehouses = await _warehouseService.GetAllWarehouseByBranchId(branchId);
            return Json(warehouses, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> GetProductById(int id)
        {
            try
            {
                var dto = await _productService.GetProductByIdAsync(id);
                if (dto == null)
                    return Json(null, JsonRequestBehavior.AllowGet);

                return Json(dto, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<ActionResult> UpdateProduct()
        {
            try
            {
                // ---- 1. PARSE BASIC FIELDS ----
                int productId = Convert.ToInt32(Request.Form["ProductId"]);
                string name = Request.Form["Name"];
                string barcode = Request.Form["Barcode"];
                string unit = Request.Form["Unit"];
                string description = Request.Form["Description"];

                int? categoryId = string.IsNullOrEmpty(Request.Form["CategoryId"])
                    ? (int?)null
                    : Convert.ToInt32(Request.Form["CategoryId"]);

                int? supplierId = string.IsNullOrEmpty(Request.Form["SupplierId"])
                    ? (int?)null
                    : Convert.ToInt32(Request.Form["SupplierId"]);

                decimal price = Convert.ToDecimal(Request.Form["Price"]);
                int minStock = Convert.ToInt32(Request.Form["MinStock"]);
                int maxStock = Convert.ToInt32(Request.Form["MaxStock"]);

                DateTime tempDate;

                // Nếu chuỗi rỗng thì giữ nguyên null. Nếu chuỗi có dữ liệu, thử parse.
                DateTime? createdAt = string.IsNullOrEmpty(Request.Form["CreatedAt"])
                    ? (DateTime?)null
                    : (DateTime.TryParse(Request.Form["CreatedAt"], out tempDate) ? tempDate : (DateTime?)null);

                DateTime? expiryDate = string.IsNullOrEmpty(Request.Form["ExpiryDate"])
                    ? (DateTime?)null
                    : (DateTime.TryParse(Request.Form["ExpiryDate"], out tempDate) ? tempDate : (DateTime?)null);

                // ---- 2. HANDLE IMAGE FILE IF EXISTS ----
                string imagePath = null;

                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files["Image"];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);

                        string folderPath = Server.MapPath("~/Uploads/Products/" + productId + "/");
                        string savePath = System.IO.Path.Combine(folderPath, fileName);

                        if (!System.IO.Directory.Exists(folderPath))
                        {
                            System.IO.Directory.CreateDirectory(folderPath);
                        }

                        file.SaveAs(savePath);

                        imagePath = "/Uploads/Products/" + productId + "/" + fileName;
                    }
                }

                // ---- 3. BUILD DTO ----
                var dto = new ProductDTO
                {
                    ProductId = productId,
                    Name = name,
                    CategoryId = categoryId,
                    SupplierId = supplierId,
                    Barcode = barcode,
                    Price = price,
                    Unit = unit,
                    Description = description,
                    MinStock = minStock,
                    MaxStock = maxStock,
                    Image = imagePath
                };

                // ---- 4. UPDATE SERVICE ----
                string rootPath = Server.MapPath("~");
                var success = await _productService.UpdateProductAsync(dto, rootPath);

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<ActionResult> CreateProduct(ProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                // === SỬA ĐOẠN NÀY ===

                // 1. Trích xuất tất cả các lỗi từ ModelState
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // 2. Gom các lỗi thành một chuỗi duy nhất để hiển thị
                // Ví dụ: "Tên sản phẩm là bắt buộc. Giá bán phải lớn hơn 0."
                var detailedMessage = string.Join(". ", errors);

                // 3. Trả về thông báo chi tiết
                return Json(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ: " + detailedMessage
                });
                // ==================
            }

            var file = Request.Files["Image"];   // LẤY FILE TẠI ĐÂY (đúng chỗ nhất)

            var newProductId = await _productService.CreateProductAsync(dto, file);

            if (newProductId > 0)
                return Json(new { success = true, productId = newProductId });

            return Json(new { success = false, message = "Lỗi trong quá trình lưu trữ dữ liệu." });
        }


        public ActionResult GenerateBarcode(string code)
        {
            const string fallback = "~/assets/img/EMarket_Logo.png";

            if (string.IsNullOrWhiteSpace(code))
                return File(Server.MapPath(fallback), "image/png");

            try
            {
                // Giữ nguyên mã bạn nhập, không thay đổi, không tính lại checksum
                string rawCode = new string(code.Where(char.IsDigit).ToArray());

                var barcode = new Barcode();
                object raw = barcode.Encode(
                    BarcodeStandard.Type.Ean13,
                    rawCode,
                    SKColors.Black,
                    SKColors.White,
                    300,
                    100
                );

                if (raw is byte[] bs)
                    return File(bs, "image/png");

                if (raw is SKData sk)
                    return File(sk.ToArray(), "image/png");

                if (raw is SKImage img)
                {
                    using (img)
                    using (var data = img.Encode(SKEncodedImageFormat.Png, 100))
                        return File(data.ToArray(), "image/png");
                }

                if (raw is SKBitmap bmp)
                {
                    using (var data = bmp.Encode(SKEncodedImageFormat.Png, 100))
                        return File(data.ToArray(), "image/png");
                }

                if (raw is System.Drawing.Image gImg)
                {
                    using (var ms = new MemoryStream())
                    {
                        gImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return File(ms.ToArray(), "image/png");
                    }
                }

                return File(Server.MapPath(fallback), "image/png");
            }
            catch
            {
                return File(Server.MapPath(fallback), "image/png");
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProductImageByProductId(int productId)
        {
            var items = await _productService.GetAllProductImageByProductIdAsync(productId);
            return Json(new { data = items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<ActionResult> CreateProductImage(int productId)
        {
            if (Request.Files.Count == 0)
                return Json(new { error = "No file uploaded" });

            var file = Request.Files[0];
            if (file == null || file.ContentLength == 0)
                return Json(new { error = "Invalid file" });

            // 1. Chuẩn bị đường dẫn thư mục
            string folderPath = Server.MapPath("~/Uploads/Products/" + productId + "/");

            // 2. Kiểm tra và tạo thư mục nếu chưa có
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            // 3. Lưu file
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string savePath = Path.Combine(folderPath, fileName);
            file.SaveAs(savePath);

            string imageUrl = "/Uploads/Products/" + productId + "/" + fileName;

            // 4. Lưu DB
            var dto = new ProductImageDTO
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                SortOrder = 1
            };

            var result = await _productService.CreateProductImageAsync(dto);

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<ActionResult> DeleteProductImage(int id)
        {
            // Lấy ảnh theo image_id để biết đường dẫn file
            var img = await _productService.GetProductImageByIdAsync(id);
            if (img == null)
                return Json(new { success = false, message = "Image not found" });

            // Xóa file vật lý
            try
            {
                string path = Server.MapPath(img.ImageUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                // Không return lỗi, nhưng log lại
                System.Diagnostics.Debug.WriteLine("File delete error: " + ex.Message);
            }

            // Xóa DB
            var success = await _productService.DeleteProductImageAsync(id);

            return Json(new { success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult UploadTempImage()
        {
            var file = Request.Files[0];
            var dto = _productService.UploadTempImage(file);

            return Json(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<ActionResult> MoveTempImages(int productId, List<string> files)
        {
            if (files == null || files.Count == 0)
                return Json(new { success = false, message = "No files provided." });

            var result = await _productService.MoveTempImagesToProductAsync(productId, files);

            return Json(new
            {
                success = true,
                images = result
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult DeleteTempImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Json(new { success = false, message = "Filename empty" });

            var ok = _productService.DeleteTempImageAsync(fileName);

            return Json(new
            {
                success = ok,
                message = ok ? "Deleted" : "Cannot delete file"
            });
        }

        #endregion


        #region Product Category
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult ProductCategoryList()
        {
            return View();
        }

        // DataTable load
        [HttpGet]
        public async Task<JsonResult> GetAllProductCategory()
        {
            var data = await _categoryService.GetAllProductCategoryAsync();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetFilteredProductCategory(string categoryName)
        {
            var data = await _categoryService.GetFilteredProductCategoriesAsync(categoryName);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<JsonResult> CreateProductCategory(ProductCategoryDTO dto)
        {
            var result = await _categoryService.CreateProductCategoryAsync(dto);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<JsonResult> UpdateProductCategory(ProductCategoryDTO dto)
        {
            var result = await _categoryService.UpdateProductCategoryAsync(dto);
            return Json(new { success = result });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<JsonResult> DeleteProductCategory(int id)
        {
            var result = await _categoryService.DeleteProductCategoryAsync(id);
            return Json(new { success = result });
        }
        #endregion

        #region Print Barcode

        public async Task<JsonResult> SearchProductByName(string keyword, int? branchId)
        {
            var iBranchId = branchId ?? _userContext.CurrentBranchId ?? null;
            var data = await _productService.GetFilteredProductAsync(keyword, iBranchId);
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SearchProductByBarcode(string barcode)
        {
            var branchId = _userContext.CurrentBranchId ?? null;
            var data = await _productService.GetFilteredProductAsync(barcode, branchId);
            return Json(new { success = true, data }, JsonRequestBehavior.AllowGet);
        }

        // PREVIEW
        public ActionResult PrintBarcodePreview(string data)
        {
            ViewBag.Data = data;
            return View();
        }
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult PrintBarcode()
        {
            return View();
        }


        #endregion

        #region Import Product
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult ImportProduct()
        {
            return View();
        }

        [HttpGet]
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult DownloadImportSample()
        {
            var bytes = _productService.GenerateProductImportTemplate();
            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Product_Import_Template.xlsx"
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(Module = "ProductModule")]
        public async Task<ActionResult> Import(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "File không hợp lệ"
                });
            }

            var result = await _productService.ImportAsync(file, Server.MapPath("~"));

            if (result.Success)
            {
                return Json(new
                {
                    success = true,
                    imported = result.ImportedRows,
                    total = result.TotalRows
                });
            }

            // FAIL → trả token để tải file lỗi
            return Json(new
            {
                success = false,
                message = "Import thất bại",
                errorToken = result.ErrorToken
            });
        }

        [HttpGet]
        [EMarketAuthorize(Module = "ProductModule")]
        public ActionResult DownloadError(string token)
        {
            var file = _productService.GetErrorReport(token);
            if (file == null)
                return HttpNotFound();

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ProductImportErrors.xlsx"
            );
        }

        #endregion
    }
}