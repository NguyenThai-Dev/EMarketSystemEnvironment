using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Forecast.DTOs;
using static EMarket.Forecast.DTOs.AIReplenishmentDTO;

namespace EMarket.Forecast.Services.Interfaces
{
    public interface IAIService
    {
        // Chạy lệnh phân tích (Gọi Master Stored Procedure)
        Task<bool> RunAnalysisAsync(int branchId);

        // Lấy dữ liệu gợi ý nhập hàng (Có Join tên sản phẩm)
        Task<List<AI_RecommendationDTO>> GetRecommendationsAsync(int branchId);

        // Lấy dữ liệu bất thường
        Task<List<AI_AnomalyDTO>> GetAnomaliesAsync(int branchId);

        // Lấy insight sản phẩm
        Task<List<AI_InsightDTO>> GetProductInsightsAsync(int branchId);

        Task<bool> RunAIPipelineAsync();

        // 2. Lấy danh sách gợi ý nhập hàng (Kết quả từ AI)
        Task<List<AIReplenishmentDTO>> GetReplenishmentAdviceAsync(int branchId);
        Task<IReadOnlyList<ProductHistoryDTO>> GetProductHistoryAsync(
               int productId,
               int branchId,
               DateTime startDate,
               DateTime endDate
           );

        Task<List<AI_InventoryForecastDTO>> GetInventoryForecastAsync(int branchId);
        Task<List<AI_DeadstockDTO>> GetDeadstockAnalysisAsync(int branchId);
        Task<List<AI_SalesForecastDTO>> GetSalesForecastAsync(int productId, int branchId);
        Task<List<AI_TopForecastDTO>> GetTopPredictedProductsAsync(int branchId, int topCount);
    }

}
