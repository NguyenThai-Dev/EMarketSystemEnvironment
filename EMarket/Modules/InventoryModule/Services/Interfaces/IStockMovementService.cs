using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.InventoryModule.DTOs;

namespace EMarket.Modules.InventoryModule.Services.Interfaces
{
    public interface IStockMovementService
    {
        Task<(int total, int filtered, List<StockMovementDTO> data)> GetStockMovementsDataTableAsync(
             int start,
             int length,
             int? warehouseId,
             string type,
             DateTime? fromDate,
             DateTime? toDate,
             string keyword
         );

        // Lấy tổng tồn kho của 1 sản phẩm tại 1 kho (Cộng dồn tất cả các Lot)
        Task<decimal> GetTotalStockAsync(int productId, int warehouseId);

        // Thực hiện điều chỉnh kho
        Task<bool> AdjustStockAsync(StockAdjustmentDTO dto);
    }
}
