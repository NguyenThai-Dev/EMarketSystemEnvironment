using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.SalesModule.DTOs;

namespace EMarket.Modules.SalesModule.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentDTO>> GetPaymentsByOrderIdAsync(int orderId);
        Task<int> CreatePaymentAsync(PaymentDTO dto);
        Task<bool> UpdatePaymentAsync(PaymentDTO dto);
        Task<bool> DeletePaymentAsync(int paymentId);
    }
}
