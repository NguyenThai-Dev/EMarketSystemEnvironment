using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.QuotationModule.DTOs;
using EMarket.Modules.SalesModule.DTOs;

namespace EMarket.Modules.QuotationModule.Services.Interfaces
{
    public interface IQuotationService
    {
        Task<List<QuotationDTO>> GetAllQuotationsAsync(string keyword, int? branchId, string status, DateTime? fromDate, DateTime? toDate);
        Task<QuotationDTO> GetQuotationByIdAsync(int id);

        Task<int> CreateQuotationAsync(QuotationDTO dto); // Trả về ID
        Task<bool> UpdateQuotationAsync(QuotationDTO dto);
        Task<bool> ChangeStatusAsync(int id, string newStatus);
        Task<bool> DeleteQuotationAsync(int id);

        // Hàm quan trọng: Chuyển báo giá thành đơn hàng thật
        Task<CheckoutResultDTO> ConvertQuotationToOrderAsync(int quotationId, int userId);
    }
}
