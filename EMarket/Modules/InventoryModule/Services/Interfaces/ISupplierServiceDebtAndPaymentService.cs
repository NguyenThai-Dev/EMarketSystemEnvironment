using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.InventoryModule.DTOs;

namespace EMarket.Modules.InventoryModule.Services.Interfaces
{
    public interface ISupplierServiceDebtAndPaymentService
    {
        Task<List<SupplierDebtDTO>> GetAllSupplierDebtsAsync();
        Task<List<SupplierDebtDTO>> GetAllSupplierDebtsAsync(string keyword, int? supplierId, string status, DateTime? fromDate, DateTime? toDate);
        Task<SupplierDebtDTO> GetSupplierDebtByIdAsync(int id);
        Task<SupplierDebtDTO> GetSupplierDebtByPurchaseOrderIdAsync(int purchaseOrderId);

        Task<List<SupplierDebtDTO>> GetSupplierDebtsByIdsAsync(List<int> ids);

        Task<SupplierPaymentDTO> GetPaymentMailInfoAsync(int paymentId);

        Task<bool> CreateSupplierDebtAsync(SupplierDebtDTO dto);
        Task<bool> UpdateSupplierDebtAsync(SupplierDebtDTO dto);

        Task<List<SupplierPaymentDTO>> GetPaymentsByDebtIdAsync(int debtId);
        Task<bool> CreateSupplierPaymentAsync(SupplierPaymentDTO dto);
        Task<bool> DeleteSupplierPaymentAsync(int id);
        Task<List<InternalDebtNotificationDTO>> GetInternalDebtDetailAsync(List<int> debtIds);
        Task<List<SupplierDebtDTO>> GetDebtsNearDueDateAsync(int daysBefore);
        Task<List<SupplierDebtDTO>> GetOverdueDebtsAsync();
    }
}
