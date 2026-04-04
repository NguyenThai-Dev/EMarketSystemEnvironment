using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.ProductModule.DTOs;

namespace EMarket.Modules.ProductModule.Services.Interfaces
{
    public interface IProductLotService
    {
        Task<List<ProductLotDTO>> GetAllProductLotAsync();
        Task<ProductLotDTO> GetProductLotByIdAsync(int lotId);
        Task<List<ProductLotDTO>> GetProductLotsByProductIdAsync(int productId);
        Task<List<ProductLotDTO>> GetAllProductLotsByIdsAsync(List<int> ids);
        Task<List<int>> GetLotIdsByProductAndLotAsync(List<int> productIds, List<int> lotIds);
        Task<int> CreateProductLotAsync(ProductLotDTO dto);
        Task<bool> UpdateProductLotAsync(ProductLotDTO dto);
        Task<bool> DeleteProductLotAsync(int lotId);
        Task DeleteProductLotsByIdsAsync(List<int> lotIds);
        Task<int?> FindExistingLotIdAsync(int productId, DateTime? manufacturingDate, DateTime? expiryDate);
        Task UpdateProductLotCostAsync(ProductLotDTO dto);

        Task<List<int>> GetLotIdsByProductIdAsync(int productId);

        // Hàm này giúp lấy chi tiết lô (HSD, Ngày SX) từ danh sách ID
        Task<List<ProductLotDTO>> GetLotsByIdsAsync(List<int> lotIds);
    }
}
