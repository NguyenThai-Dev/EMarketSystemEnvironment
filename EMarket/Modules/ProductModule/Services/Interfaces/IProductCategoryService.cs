using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.ProductModule.DTOs;

namespace EMarket.Modules.ProductModule.Services.Interfaces
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDTO>> GetAllProductCategoryAsync();
        Task<ProductCategoryDTO> GetProductCategoryByIdAsync(int id);
        Task<Dictionary<int, string>> GetCategoriesByIdsAsync(List<int> ids);
        Task<List<ProductCategoryDTO>> GetFilteredProductCategoriesAsync(string name);
        Task<bool> CreateProductCategoryAsync(ProductCategoryDTO dto);
        Task<bool> UpdateProductCategoryAsync(ProductCategoryDTO dto);
        Task<bool> DeleteProductCategoryAsync(int id);
    }
}