using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.ProductModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    [Authorize]
    [RoutePrefix("api/admin/product-management")]
    public class ProductAdminApiController : ApiController
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;
        private readonly IProductLotService _lotService;
        private readonly ISupplierService _supplierService;

        public ProductAdminApiController(
            IProductService productService, IProductCategoryService categoryService,
            IProductLotService lotService, ISupplierService supplierService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _lotService = lotService;
            _supplierService = supplierService;
        }

        #region Product Core

        [HttpGet, Route("products")]
        public async Task<IHttpActionResult> GetAllProducts()
        { return Ok(await _productService.GetAllProductAsync()); }

        [HttpGet, Route("products/search")]
        public async Task<IHttpActionResult> SearchProducts(string keyword = null, int? categoryId = null, int? branchId = null, int? supplierId = null, int? warehouseId = null)
        { return Ok(await _productService.GetFilteredProductAsync(keyword, categoryId, branchId, supplierId, warehouseId)); }

        [HttpGet, Route("products/search-simple")]
        public async Task<IHttpActionResult> SearchProductsSimple(string keyword = null, int? branchId = null)
        { return Ok(await _productService.GetFilteredProductAsync(keyword, branchId)); }

        [HttpGet, Route("products/{id:int}")]
        public async Task<IHttpActionResult> GetProductById(int id)
        {
            var d = await _productService.GetProductByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("products/by-ids")]
        public async Task<IHttpActionResult> GetProductsByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("ids required.");
            return Ok(await _productService.GetProductsByIdsAsync(ids));
        }

        [HttpPost, Route("products/names-by-ids")]
        public async Task<IHttpActionResult> GetProductNamesByIds([FromBody] List<int> productIds)
        {
            if (productIds == null || productIds.Count == 0) return BadRequest("productIds required.");
            return Ok(await _productService.GetProductNamesByIdsAsync(productIds));
        }

        [HttpGet, Route("products/inactive")]
        public async Task<IHttpActionResult> GetInactiveProducts(string keyword = null, int? categoryId = null, int? branchId = null, int? supplierId = null, int? warehouseId = null)
        { return Ok(await _productService.GetFilteredInactiveProductAsync(keyword, categoryId, branchId, supplierId, warehouseId)); }

        [HttpGet, Route("products/low-stock-alerts")]
        public async Task<IHttpActionResult> GetLowStockAlerts(int top = 10)
        { return Ok(await _productService.ReadLowStockAlertsAsync(top)); }

        #endregion

        #region Product Images

        [HttpGet, Route("products/{productId:int}/images")]
        public async Task<IHttpActionResult> GetProductImages(int productId)
        { return Ok(await _productService.GetAllProductImageByProductIdAsync(productId)); }

        [HttpGet, Route("products/images/{imageId:int}")]
        public async Task<IHttpActionResult> GetProductImageById(int imageId)
        {
            var d = await _productService.GetProductImageByIdAsync(imageId);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        #endregion

        #region Product Lots

        [HttpGet, Route("lots")]
        public async Task<IHttpActionResult> GetAllLots()
        { return Ok(await _lotService.GetAllProductLotAsync()); }

        [HttpGet, Route("products/{productId:int}/lots")]
        public async Task<IHttpActionResult> GetLotsByProduct(int productId)
        { return Ok(await _lotService.GetProductLotsByProductIdAsync(productId)); }

        [HttpGet, Route("lots/{lotId:int}")]
        public async Task<IHttpActionResult> GetLotDetail(int lotId)
        {
            var d = await _lotService.GetProductLotByIdAsync(lotId);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("lots/by-ids")]
        public async Task<IHttpActionResult> GetLotsByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("ids required.");
            return Ok(await _lotService.GetAllProductLotsByIdsAsync(ids));
        }

        [HttpPost, Route("lots/details-by-ids")]
        public async Task<IHttpActionResult> GetLotDetailsByIds([FromBody] List<int> lotIds)
        {
            if (lotIds == null || lotIds.Count == 0) return BadRequest("lotIds required.");
            return Ok(await _lotService.GetLotsByIdsAsync(lotIds));
        }

        [HttpGet, Route("lots/by-product/{productId:int}/lot-ids")]
        public async Task<IHttpActionResult> GetLotIdsByProductId(int productId)
        { return Ok(await _lotService.GetLotIdsByProductIdAsync(productId)); }

        [HttpGet, Route("lots/find-existing")]
        public async Task<IHttpActionResult> FindExistingLot(int productId, DateTime? manufacturingDate = null, DateTime? expiryDate = null)
        { return Ok(await _lotService.FindExistingLotIdAsync(productId, manufacturingDate, expiryDate)); }

        #endregion

        #region Categories

        [HttpGet, Route("categories")]
        public async Task<IHttpActionResult> GetAllCategories()
        { return Ok(await _categoryService.GetAllProductCategoryAsync()); }

        [HttpGet, Route("categories/{id:int}")]
        public async Task<IHttpActionResult> GetCategoryById(int id)
        {
            var d = await _categoryService.GetProductCategoryByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpGet, Route("categories/search")]
        public async Task<IHttpActionResult> SearchCategories(string name = null)
        { return Ok(await _categoryService.GetFilteredProductCategoriesAsync(name)); }

        [HttpPost, Route("categories/by-ids")]
        public async Task<IHttpActionResult> GetCategoriesByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("ids required.");
            return Ok(await _categoryService.GetCategoriesByIdsAsync(ids));
        }

        #endregion

        #region Suppliers

        [HttpGet, Route("suppliers")]
        public async Task<IHttpActionResult> GetAllSuppliers()
        { return Ok(await _supplierService.GetAllSupplierAsync()); }

        [HttpGet, Route("suppliers/search")]
        public async Task<IHttpActionResult> SearchSuppliers(string name = null)
        { return Ok(await _supplierService.GetFilteredSupplierAsync(name)); }

        [HttpGet, Route("suppliers/{id:int}")]
        public async Task<IHttpActionResult> GetSupplierById(int id)
        {
            var d = await _supplierService.GetSupplierByIdAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        [HttpPost, Route("suppliers/by-ids")]
        public async Task<IHttpActionResult> GetSuppliersByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0) return BadRequest("ids required.");
            return Ok(await _supplierService.GetAllSupplierByIdAsync(ids));
        }

        #endregion
    }
}