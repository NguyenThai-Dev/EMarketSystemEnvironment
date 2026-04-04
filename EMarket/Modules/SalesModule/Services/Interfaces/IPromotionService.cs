using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.ProductModule.DTOs;
using EMarket.Modules.SalesModule.DTOs;

namespace EMarket.Modules.SalesModule.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<List<PromotionDTO>> GetAllPromotionsAsync();
        Task<List<PromotionDTO>> GetFilteredPromotionAsync(string keyword, int? categoryId, string discountType, string cusType, DateTime? fromDate, DateTime? toDate);
        Task<PromotionDTO> GetPromotionByIdAsync(int id);
        Task<int> CreatePromotionAsync(PromotionDTO dto);
        Task<bool> UpdatePromotionAsync(PromotionDTO dto);
        Task<bool> DeletePromotionAsync(int id);

        Task<List<PromotionDTO>> GetActivePromotionsAsync();

        // Tính toán giá cho 1 sản phẩm dựa trên list KM đã lấy
        void ApplyBestPromotion(ProductDTO product, List<PromotionDTO> activePromotions);
    }
}
