using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.ProductModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [RoutePrefix("api/admin/product-management")]
    public class ProductAdminApiController : ApiController
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;
        private readonly IProductLotService _lotService;
        private readonly ISupplierService _supplierService;

        public ProductAdminApiController(
            IProductService productService,
            IProductCategoryService categoryService,
            IProductLotService lotService,
            ISupplierService supplierService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _lotService = lotService;
            _supplierService = supplierService;
        }

        // ============================================================
        #region Product Core APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ danh sách sản phẩm.
        /// </summary>
        [HttpGet]
        [Route("products")]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            var data = await _productService.GetAllProductAsync();
            return Ok(data);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm nâng cao với nhiều bộ lọc (Category, Branch, Supplier, Warehouse).
        /// </summary>
        [HttpGet]
        [Route("products/search")]
        public async Task<IHttpActionResult> SearchProducts(
            string keyword = null, int? categoryId = null, int? branchId = null,
            int? supplierId = null, int? warehouseId = null)
        {
            var data = await _productService.GetFilteredProductAsync(keyword, categoryId, branchId, supplierId, warehouseId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy thông tin chi tiết sản phẩm theo ID.
        /// </summary>
        [HttpGet]
        [Route("products/{id:int}")]
        public async Task<IHttpActionResult> GetProductById(int id)
        {
            var data = await _productService.GetProductByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách các sản phẩm đang dưới định mức tồn kho (Cảnh báo tồn kho thấp).
        /// </summary>
        [HttpGet]
        [Route("products/low-stock-alerts")]
        public async Task<IHttpActionResult> GetLowStockAlerts(int top = 10)
        {
            var data = await _productService.ReadLowStockAlertsAsync(top);
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Product Lot & Expiry APIs
        // ============================================================

        /// <summary>
        /// Lấy thông tin các lô hàng của một sản phẩm (Dùng để kiểm tra HSD/Ngày SX).
        /// </summary>
        [HttpGet]
        [Route("products/{productId:int}/lots")]
        public async Task<IHttpActionResult> GetLotsByProduct(int productId)
        {
            var data = await _lotService.GetProductLotsByProductIdAsync(productId);
            return Ok(data);
        }

        /// <summary>
        /// Lấy thông tin chi tiết một lô hàng cụ thể.
        /// </summary>
        [HttpGet]
        [Route("lots/{lotId:int}")]
        public async Task<IHttpActionResult> GetLotDetail(int lotId)
        {
            var data = await _lotService.GetProductLotByIdAsync(lotId);
            if (data == null) return NotFound();
            return Ok(data);
        }

        #endregion

        // ============================================================
        #region Category & Supplier APIs
        // ============================================================

        /// <summary>
        /// Lấy toàn bộ danh mục sản phẩm.
        /// </summary>
        [HttpGet]
        [Route("categories")]
        public async Task<IHttpActionResult> GetAllCategories()
        {
            var data = await _categoryService.GetAllProductCategoryAsync();
            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ nhà cung cấp.
        /// </summary>
        [HttpGet]
        [Route("suppliers")]
        public async Task<IHttpActionResult> GetAllSuppliers()
        {
            var data = await _supplierService.GetAllSupplierAsync();
            return Ok(data);
        }

        /// <summary>
        /// Tìm kiếm nhà cung cấp theo tên.
        /// </summary>
        [HttpGet]
        [Route("suppliers/search")]
        public async Task<IHttpActionResult> SearchSuppliers(string name)
        {
            var data = await _supplierService.GetFilteredSupplierAsync(name);
            return Ok(data);
        }

        #endregion
    }
}