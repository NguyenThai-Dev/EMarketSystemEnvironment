using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.InventoryModule.DTOs;

namespace EMarket.Modules.InventoryModule.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<List<PurchaseOrderDTO>> GetAllPurchaseAsync();
        Task<PurchaseOrderDTO> GetPurchaseByIdAsync(int id);
        Task<int> CreatePurchaseAsync(PurchaseOrderDTO dto);
        Task<bool> UpdatePurchaseAsync(PurchaseOrderDTO dto);
        Task<bool> DeletePurchaseAsync(int id);

        Task<List<PurchaseOrderDTO>> GetFilteredPurchasesAsync(
            string keyword,
            int? supplierId,
            int? branchId,
            int? warehouseId,
            string status,
            string paymentStatus,
            DateTime? fromDate,
            DateTime? toDate
        );

        /// <summary>Server-side DataTable: trả về (total, filtered, page data)</summary>
        Task<(int total, int filtered, List<PurchaseOrderDTO> data)> GetPurchaseOrdersDataTableAsync(
            int start, int length,
            string keyword, int? supplierId, int? branchId, int? warehouseId,
            string status, string paymentStatus,
            DateTime? fromDate, DateTime? toDate
        );

        Task<List<PurchaseOrderDTO>> GetPurchaseByBranchIdAsync(int? branchId, DateTime? fromDate, DateTime? toDate);
    }
}
