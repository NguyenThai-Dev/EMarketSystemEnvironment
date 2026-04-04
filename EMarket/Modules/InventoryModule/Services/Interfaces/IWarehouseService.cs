using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.InventoryModule.DTOs;

namespace EMarket.Modules.InventoryModule.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<List<WarehouseDTO>> GetAllWarehousesByBranchIdAsync();
        Task<List<WarehouseDTO>> GetWarehouseByIdsAsync(List<int> ids);
        Task<List<WarehouseDTO>> GetAllWarehouseByBranchId(int? branchId);
        Task<List<WarehouseDTO>> GetFilteredWarehouseAsync(string name, int? branchId);
        Task<WarehouseDTO> GetWarehouseByIdAsync(int warehouseId);
        Task<int> CreateWarehouseAsync(WarehouseDTO dto);
        Task<bool> UpdateWarehouseAsync(WarehouseDTO dto);
        Task<bool> DeleteWarehouseAsync(int warehouseId);
        Task<Dictionary<int, WarehouseDTO>> GetWarehouseDictAsync();
    }
}
