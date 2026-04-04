using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.InventoryModule.DTOs;

namespace EMarket.Modules.InventoryModule.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<List<InventoryDTO>> GetAllInventoryAsync();
        Task<List<InventoryDTO>> GetFilteredInventoryAsync(int? productId, int? warehouseId);
        Task<List<InventoryDTO>> GetInventoryByProductIdsAsync(
           List<int> productIds,
           int? warehouseId = null);
        Task<List<InventoryDTO>> GetInventoryByProductIdsAsync(
           List<int> productIds,
           int? warehouseId = null,
           int? branchId = null);
        Task<List<InventoryDTO>> GetAllAsync(int? branchId);
        Task<InventoryDTO> GetInventoryByIdAsync(int id);
        Task<bool> CreateInventoryAsync(InventoryDTO dto);
        Task<bool> UpdateInventoryAsync(InventoryDTO dto);
        Task<bool> DeleteInventoryAsync(int id);
    }

}