using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.ProductModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// REST Web API for Product Management: Products, Categories, Suppliers, and Lots.
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/admin/product")]
    public class ProductAdminApiController : ApiController
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private readonly IProductLotService _lotService;

        public ProductAdminApiController(
            IProductService productService,
            IProductCategoryService categoryService,
            ISupplierService supplierService,
            IProductLotService lotService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _lotService = lotService;
        }

        #region Products

        /// <summary>
        /// Lấy tất cả sản phẩm đang hoạt động.
        /// </summary>
        [HttpGet, Route("products")]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm có phân trang (dành cho Mobile &amp; Web DataTables).
        /// </summary>
        [HttpGet, Route("products/paged")]
        public async Task<IHttpActionResult> GetProductsPaged(
            int start = 0,
            int length = 20,
            string keyword = null,
            int? categoryId = null,
            int? branchId = null,
            int? supplierId = null,
            int? warehouseId = null)
        {
            int? targetBranch = (branchId.HasValue && branchId.Value > 0) ? branchId : null;
            var all = await _productService.GetFilteredProductAsync(keyword, categoryId, targetBranch, supplierId, warehouseId);
            var totalCount = all.Count;
            var pageSize = Math.Min(length > 0 ? length : 20, 100);
            var pagedData = all.Skip(start).Take(pageSize).ToList();
            return Ok(new { total = totalCount, filtered = totalCount, data = pagedData });
        }

        /// <summary>
        /// Tìm kiếm nâng cao sản phẩm theo từ khóa, danh mục, chi nhánh, nhà cung cấp, kho.
        /// </summary>
        [HttpGet, Route("products/search")]
        public async Task<IHttpActionResult> SearchProducts(
            string keyword = null,
            int? categoryId = null,
            int? branchId = null,
            int? supplierId = null,
            int? warehouseId = null)
        {
            int? targetBranch = (branchId.HasValue && branchId.Value > 0) ? branchId : null;
            var result = await _productService.GetFilteredProductAsync(keyword, categoryId, targetBranch, supplierId, warehouseId);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm đơn giản sản phẩm theo từ khóa và chi nhánh.
        /// </summary>
        [HttpGet, Route("products/search-simple")]
        public async Task<IHttpActionResult> SearchProductsSimple(string keyword = null, int? branchId = null)
        {
            int? targetBranch = (branchId.HasValue && branchId.Value > 0) ? branchId : null;
            var result = await _productService.GetFilteredProductAsync(keyword, targetBranch);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm bằng mã vạch (Barcode) quét từ camera điện thoại hoặc máy quét.
        /// </summary>
        [HttpGet]
        [Route("products/barcode/{barcode}")]
        [Route("products/by-barcode")]
        public async Task<IHttpActionResult> SearchProductByBarcode(string barcode = null, string keyword = null, int? branchId = null)
        {
            var q = !string.IsNullOrWhiteSpace(barcode) ? barcode : keyword;
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Mã vạch hoặc nội dung tìm kiếm không được để trống.");
            int? targetBranch = (branchId.HasValue && branchId.Value > 0) ? branchId : null;
            var result = await _productService.GetFilteredProductAsync(q.Trim(), targetBranch);
            return Ok(new { success = true, data = result });
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm theo ID.
        /// </summary>
        [HttpGet, Route("products/{id:int}")]
        public async Task<IHttpActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo danh sách ID.
        /// </summary>
        [HttpPost, Route("products/by-ids")]
        public async Task<IHttpActionResult> GetProductsByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return Ok(new List<ProductDTO>());
            var result = await _productService.GetProductsByIdsAsync(ids);
            return Ok(result);
        }

        /// <summary>
        /// Lấy Bảng Tra Cứu Tên Sản Phẩm theo danh sách ID (Dictionary: ID -> Name).
        /// </summary>
        [HttpPost, Route("products/names-by-ids")]
        public async Task<IHttpActionResult> GetProductNamesByIds([FromBody] List<int> productIds)
        {
            if (productIds == null || productIds.Count == 0) return Ok(new Dictionary<int, string>());
            var result = await _productService.GetProductNamesByIdsAsync(productIds);
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới sản phẩm.
        /// </summary>
        [HttpPost]
        [Route("products")]
        [Route("products/create")]
        public async Task<IHttpActionResult> CreateProduct([FromBody] ProductDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu sản phẩm không hợp lệ.");
            try
            {
                var newId = await _productService.CreateProductAsync(dto, null);
                if (newId > 0)
                    return Ok(new { success = true, productId = newId });
                return Ok(new { success = false, message = "Không thể tạo sản phẩm." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"[ProductAdminApiController.CreateProduct Error] {ex}");
                return Ok(new { success = false, message = "Lỗi tạo sản phẩm: " + ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm qua method PUT.
        /// </summary>
        [HttpPut]
        [Route("products")]
        public async Task<IHttpActionResult> UpdateProductPut([FromBody] ProductDTO dto)
        {
            return await UpdateProduct(dto);
        }

        /// <summary>
        /// Cập nhật sản phẩm.
        /// </summary>
        [HttpPost]
        [HttpPut]
        [Route("products/update")]
        public async Task<IHttpActionResult> UpdateProduct([FromBody] ProductDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu cập nhật không hợp lệ.");
            try
            {
                string rootPath = System.Web.Hosting.HostingEnvironment.MapPath("~")
                    ?? (System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Server.MapPath("~") : null);
                var success = await _productService.UpdateProductAsync(dto, rootPath);
                return Ok(new { success });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"[ProductAdminApiController.UpdateProduct Error] {ex}");
                return Ok(new { success = false, message = "Lỗi cập nhật sản phẩm: " + ex.Message });
            }
        }

        /// <summary>
        /// Xóa / Ẩn sản phẩm theo ID.
        /// </summary>
        [HttpDelete, Route("products/{id:int}")]
        [HttpPost, Route("products/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            return Ok(new { success });
        }

        [HttpPost]
        [Route("products/upload-image")]
        [Route("products/{id:int}/image")]
        public async Task<IHttpActionResult> UploadProductImage(int? id = null)
        {
            var httpContext = System.Web.HttpContext.Current;
            if (httpContext == null || httpContext.Request.Files.Count == 0)
                return BadRequest("Vui lòng chọn file ảnh sản phẩm.");

            var file = httpContext.Request.Files[0];
            if (file == null || file.ContentLength == 0)
                return BadRequest("File không hợp lệ.");

            int targetProductId = id ?? (int.TryParse(httpContext.Request.Form["productId"], out int pId) ? pId : 0);
            string subFolder = targetProductId > 0 ? targetProductId.ToString() : "common";
            string folderRelativePath = $"/Uploads/Products/{subFolder}/";
            string folderMapPath = httpContext.Server.MapPath("~" + folderRelativePath);

            if (!System.IO.Directory.Exists(folderMapPath))
                System.IO.Directory.CreateDirectory(folderMapPath);

            string ext = System.IO.Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext)) ext = ".jpg";
            string fileName = $"{Guid.NewGuid()}{ext}";
            string fullPath = System.IO.Path.Combine(folderMapPath, fileName);
            file.SaveAs(fullPath);

            string relativeUrl = folderRelativePath + fileName;

            if (targetProductId > 0)
            {
                var prod = await _productService.GetProductByIdAsync(targetProductId);
                if (prod != null)
                {
                    prod.Image = relativeUrl;
                    string rootPath = System.Web.Hosting.HostingEnvironment.MapPath("~")
                        ?? (httpContext != null ? httpContext.Server.MapPath("~") : null);
                    await _productService.UpdateProductAsync(prod, rootPath);
                }
            }

            return Ok(new { success = true, url = relativeUrl, Image = relativeUrl });
        }

        /// <summary>
        /// Kích hoạt lại sản phẩm bị ngưng hoạt động.
        /// </summary>
        [HttpPost, Route("products/active")]
        public async Task<IHttpActionResult> ActiveProduct(int productId, int minStock = 5, int maxStock = 500)
        {
            var success = await _productService.ActiveProductAsync(productId, minStock, maxStock);
            return Ok(new { success });
        }

        /// <summary>
        /// Lấy danh sách sản phẩm bị ngưng hoạt động.
        /// </summary>
        [HttpGet, Route("products/inactive")]
        public async Task<IHttpActionResult> GetInactiveProducts(
            string keyword = null, int? categoryId = null, int? branchId = null,
            int? supplierId = null, int? warehouseId = null)
        {
            var result = await _productService.GetFilteredInactiveProductAsync(keyword, categoryId, branchId, supplierId, warehouseId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách cảnh báo tồn kho thấp.
        /// </summary>
        [HttpGet, Route("products/low-stock-alerts")]
        public async Task<IHttpActionResult> GetLowStockAlerts(int top = 10)
        {
            var alerts = await _productService.ReadLowStockAlertsAsync(top);
            return Ok(alerts);
        }

        /// <summary>
        /// Lấy tất cả ảnh phụ của một sản phẩm.
        /// </summary>
        [HttpGet, Route("products/{productId:int}/images")]
        public async Task<IHttpActionResult> GetProductImages(int productId)
        {
            var images = await _productService.GetAllProductImageByProductIdAsync(productId);
            return Ok(images);
        }

        /// <summary>
        /// Thêm ảnh cho sản phẩm.
        /// </summary>
        [HttpPost, Route("products/images")]
        public async Task<IHttpActionResult> CreateProductImage([FromBody] ProductImageDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu ảnh sản phẩm không hợp lệ.");
            var res = await _productService.CreateProductImageAsync(dto);
            return Ok(res);
        }

        /// <summary>
        /// Xóa ảnh phụ của sản phẩm.
        /// </summary>
        [HttpDelete, Route("products/images/{id:int}")]
        [HttpPost, Route("products/images/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteProductImage(int id)
        {
            var success = await _productService.DeleteProductImageAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Categories

        /// <summary>
        /// Lấy tất cả danh mục sản phẩm.
        /// </summary>
        [HttpGet, Route("categories")]
        public async Task<IHttpActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllProductCategoryAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Lấy chi tiết danh mục theo ID.
        /// </summary>
        [HttpGet, Route("categories/{id:int}")]
        public async Task<IHttpActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetProductCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>
        /// Tạo danh mục mới.
        /// </summary>
        [HttpPost, Route("categories")]
        public async Task<IHttpActionResult> CreateCategory([FromBody] ProductCategoryDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu danh mục không hợp lệ.");
            var success = await _categoryService.CreateProductCategoryAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Cập nhật danh mục.
        /// </summary>
        [HttpPut, Route("categories")]
        [HttpPost, Route("categories/update")]
        public async Task<IHttpActionResult> UpdateCategory([FromBody] ProductCategoryDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu danh mục không hợp lệ.");
            var success = await _categoryService.UpdateProductCategoryAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Xóa danh mục.
        /// </summary>
        [HttpDelete, Route("categories/{id:int}")]
        [HttpPost, Route("categories/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteProductCategoryAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Suppliers

        /// <summary>
        /// Lấy tất cả nhà cung cấp.
        /// </summary>
        [HttpGet, Route("suppliers")]
        public async Task<IHttpActionResult> GetAllSuppliers(string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var filtered = await _supplierService.GetFilteredSupplierAsync(name);
                return Ok(filtered);
            }
            var suppliers = await _supplierService.GetAllSupplierAsync();
            return Ok(suppliers);
        }

        /// <summary>
        /// Lấy thông tin nhà cung cấp theo ID.
        /// </summary>
        [HttpGet, Route("suppliers/{id:int}")]
        public async Task<IHttpActionResult> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        /// <summary>
        /// Tạo nhà cung cấp mới.
        /// </summary>
        [HttpPost, Route("suppliers")]
        public async Task<IHttpActionResult> CreateSupplier([FromBody] SupplierDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu nhà cung cấp không hợp lệ.");
            var success = await _supplierService.CreateSupplierAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Cập nhật nhà cung cấp.
        /// </summary>
        [HttpPut, Route("suppliers")]
        [HttpPost, Route("suppliers/update")]
        public async Task<IHttpActionResult> UpdateSupplier([FromBody] SupplierDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu nhà cung cấp không hợp lệ.");
            var success = await _supplierService.UpdateSupplierAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Xóa nhà cung cấp.
        /// </summary>
        [HttpDelete, Route("suppliers/{id:int}")]
        [HttpPost, Route("suppliers/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteSupplier(int id)
        {
            var success = await _supplierService.DeleteSupplierAsync(id);
            return Ok(new { success });
        }

        #endregion

        #region Product Lots

        /// <summary>
        /// Lấy tất cả lô sản phẩm.
        /// </summary>
        [HttpGet, Route("lots")]
        public async Task<IHttpActionResult> GetAllLots()
        {
            var lots = await _lotService.GetAllProductLotAsync();
            return Ok(lots);
        }

        /// <summary>
        /// Lấy chi tiết lô theo Lot ID.
        /// </summary>
        [HttpGet, Route("lots/{id:int}")]
        public async Task<IHttpActionResult> GetLotById(int id)
        {
            var lot = await _lotService.GetProductLotByIdAsync(id);
            if (lot == null) return NotFound();
            return Ok(lot);
        }

        /// <summary>
        /// Lấy danh sách lô theo Product ID.
        /// </summary>
        [HttpGet, Route("lots/by-product/{productId:int}")]
        public async Task<IHttpActionResult> GetLotsByProductId(int productId)
        {
            var lots = await _lotService.GetProductLotsByProductIdAsync(productId);
            return Ok(lots);
        }

        /// <summary>
        /// Tạo lô sản phẩm mới.
        /// </summary>
        [HttpPost, Route("lots")]
        public async Task<IHttpActionResult> CreateLot([FromBody] ProductLotDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu lô sản phẩm không hợp lệ.");
            var newId = await _lotService.CreateProductLotAsync(dto);
            return Ok(new { success = true, lotId = newId });
        }

        /// <summary>
        /// Cập nhật thông tin lô sản phẩm.
        /// </summary>
        [HttpPut, Route("lots")]
        [HttpPost, Route("lots/update")]
        public async Task<IHttpActionResult> UpdateLot([FromBody] ProductLotDTO dto)
        {
            if (dto == null) return BadRequest("Dữ liệu lô không hợp lệ.");
            var success = await _lotService.UpdateProductLotAsync(dto);
            return Ok(new { success });
        }

        /// <summary>
        /// Xóa lô sản phẩm.
        /// </summary>
        [HttpDelete, Route("lots/{id:int}")]
        [HttpPost, Route("lots/delete/{id:int}")]
        public async Task<IHttpActionResult> DeleteLot(int id)
        {
            var success = await _lotService.DeleteProductLotAsync(id);
            return Ok(new { success });
        }

        #endregion
    }
}
