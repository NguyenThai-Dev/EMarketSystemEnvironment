using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using EMarket.Events.Class;
using EMarket.Modules.ProductModule.DTOs;

namespace EMarket.Modules.ProductModule.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProductAsync();
        Task<List<object>> GetFilteredProductAsync(string keyWord, int? categoryId, int? branchId, int? supplierId, int? warehouseId);
        Task<List<ProductDTO>> GetFilteredProductAsync(string keyword, int? branchId);
        Task<List<ProductDTO>> GetProductsByIdsAsync(List<int> ids);

        // Lấy sản phẩm bị khóa trong hệ thống
        Task<List<object>> GetFilteredInactiveProductAsync(string keyWord, int? categoryId, int? branchId, int? supplierId, int? warehouseId);
        Task<bool> ActiveProductAsync(int productId, int minStock, int maxStock);


        Task<Dictionary<int, string>> GetProductNamesByIdsAsync(List<int> productIds);
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<int> CreateProductAsync(ProductDTO dto, HttpPostedFileBase file);
        Task<bool> UpdateProductAsync(ProductDTO dto, string rootPath);
        Task<bool> DeleteProductAsync(int id);

        Task<List<ProductImageDTO>> GetAllProductImageByProductIdAsync(int productId);
        Task<ProductImageDTO> GetProductImageByIdAsync(int id);
        Task<ProductImageDTO> CreateProductImageAsync(ProductImageDTO dto);
        Task<bool> DeleteProductImageAsync(int id);

        TempImageDTO UploadTempImage(HttpPostedFileBase file);
        Task<List<ProductImageDTO>> MoveTempImagesToProductAsync(int productId, List<string> tempFiles);
        bool DeleteTempImageAsync(string fileName);

        byte[] GenerateProductImportTemplate();

        Task<ProductImportResult> ImportAsync(HttpPostedFileBase excelFile, string rootPath);
        byte[] GetErrorReport(string token);

        Task<List<LowStockAlertDTO>> ReadLowStockAlertsAsync(int top = 10);
    }
}