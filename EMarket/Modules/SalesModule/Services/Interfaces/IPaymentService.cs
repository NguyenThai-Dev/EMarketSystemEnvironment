using EMarket.Modules.SalesModule.DTOs;
using PayOS.Models.Webhooks; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMarket.Modules.SalesModule.Services.Interfaces
{
    public interface IPaymentService
    {
        // 1. Nhóm CRUD Nội bộ
        Task<List<PaymentDTO>> GetPaymentsByOrderIdAsync(int orderId);
        Task<int> CreatePaymentAsync(PaymentDTO dto);
        Task<bool> UpdatePaymentAsync(PaymentDTO dto);
        Task<bool> DeletePaymentAsync(int paymentId);

        // 2. Nhóm Tích hợp Cổng thanh toán (Đã che giấu logic thư viện ngoài)
        Task<CreateQrResponseDTO> CreatePayOSLinkAsync(CreateQrRequestDTO request);
        Task<WebhookData> VerifyPayOSWebhookAsync(Webhook webhookBody);
    }
}