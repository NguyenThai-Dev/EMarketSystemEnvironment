using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.SalesModule.DTOs;

namespace EMarket.Modules.SalesModule.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetAllOrdersAsync();
        Task<List<OrderDTO>> GetFullOrdersByBranchIdAsync(int? branchId, DateTime? fromDate, DateTime? toDate);
        Task<List<OrderDTO>> GetOrdersByBranchIdAsync(int? branchId, DateTime? fromDate, DateTime? toDate);
        Task<OrderDTO> GetOrderByIdAsync(int orderId);

        Task<(int total, int filtered, List<OrderDTO> data)> GetOrdersDataTableAsync(
        int draw,
        int start,
        int length,
        int? userId,
        int? branchId,
        string status,
        DateTime? fromDate,
        DateTime? toDate,
        string keyword
    );

        Task<bool> UpdateOrderStatusAsync(int orderId, string status, string connectionId);

        Task<int> CreateOrderAsync(OrderDTO dto);
        Task<bool> UpdateOrderAsync(OrderDTO dto);
        Task<bool> DeleteOrderAsync(int orderId);

        Task<List<OrderDetailDTO>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<int> CreateOrderDetailAsync(OrderDetailDTO dto);
        Task<bool> UpdateOrderDetailAsync(OrderDetailDTO dto);
        Task<bool> DeleteOrderDetailAsync(int id);

        Task<CheckoutResultDTO> CheckoutAsync(CheckoutRequestDTO request);
    }
}
