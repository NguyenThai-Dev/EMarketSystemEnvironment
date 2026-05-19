using EMarket.Models;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Modules.SalesModule.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly EMarket_DBEntities _db;
        private readonly PayOSClient _payOS;
        private readonly DateTime _defaultDate = new DateTime(2000, 01, 01);

        public PaymentService(EMarket_DBEntities db)
        {
            _db = db;
            string clientId = System.Configuration.ConfigurationManager.AppSettings["PayOSClientId"];
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["PayOSApiKey"];
            string checksumKey = System.Configuration.ConfigurationManager.AppSettings["ChecksumKey"];

            _payOS = new PayOSClient(clientId, apiKey, checksumKey);
        }

        public async Task<List<PaymentDTO>> GetPaymentsByOrderIdAsync(int orderId)
        {
            return await _db.Payments
                .Where(p => p.order_id == orderId)
                .Select(p => new PaymentDTO
                {
                    PaymentId = p.payment_id,
                    OrderId = p.order_id,
                    PaymentMethod = p.payment_method,
                    Amount = p.amount ?? 0,
                    Status = p.status,
                    PaymentDate = p.payment_date ?? _defaultDate
                }).ToListAsync();
        }

        public async Task<int> CreatePaymentAsync(PaymentDTO dto)
        {
            try
            {
                var entity = new Payment
                {
                    order_id = dto.OrderId,
                    payment_method = dto.PaymentMethod,
                    amount = dto.Amount,
                    status = dto.Status,
                    payment_date = DateTime.Now
                };

                _db.Payments.Add(entity);
                await _db.SaveChangesAsync();

                return entity.payment_id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating payment: " + ex.Message);
            }
        }

        public async Task<bool> UpdatePaymentAsync(PaymentDTO dto)
        {
            try
            {
                var entity = await _db.Payments.FindAsync(dto.PaymentId);
                if (entity == null) return false;

                entity.payment_method = dto.PaymentMethod;
                entity.amount = dto.Amount;
                entity.status = dto.Status;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating payment: " + ex.Message);
            }
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            try
            {
                var entity = await _db.Payments.FindAsync(paymentId);
                if (entity == null) return false;

                _db.Payments.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting payment: " + ex.Message);
            }
        }

        public async Task<CreateQrResponseDTO> CreatePayOSLinkAsync(CreateQrRequestDTO request)
        {
            try
            {
                long transactionCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = transactionCode,
                    Amount = request.Amount,
                    Description = request.Description,
                    CancelUrl = ConfigurationManager.AppSettings["DomainUrl"] + "/checkout/cancel",
                    ReturnUrl = ConfigurationManager.AppSettings["DomainUrl"] + "/checkout/success",
                    Items = new List<PaymentLinkItem>()
                };

                var payOSResult = await _payOS.PaymentRequests.CreateAsync(paymentRequest);

                return new CreateQrResponseDTO
                {
                    Success = true,
                    CheckoutUrl = payOSResult.CheckoutUrl,
                    QrCode = payOSResult.QrCode,
                    OrderCode = payOSResult.OrderCode,
                    Message = "Tạo link thanh toán thành công"
                };
            }
            catch (Exception ex)
            {
                return new CreateQrResponseDTO
                {
                    Success = false,
                    Message = "Lỗi khi tạo PayOS: " + ex.Message
                };
            }
        }

        public async Task<WebhookData> VerifyPayOSWebhookAsync(Webhook webhookBody)
        {
            try
            {
                // 1. Lấy OrderCode từ Webhook (Lúc này Webhook chỉ đóng vai trò là "người đưa tin")
                long currentOrderCode = webhookBody.Data.OrderCode;
                Debug.WriteLine($"[PAYOS DEBUG] Đang check chéo trạng thái thật của đơn hàng: {currentOrderCode}");

                // 2. Gọi NGƯỢC lại API của PayOS để lấy thông tin ĐÃ ĐƯỢC XÁC THỰC
                // Hàm này dùng API_KEY và CLIENT_ID của bro, nên bảo mật là tuyệt đối 100%
                var paymentInfo = await _payOS.PaymentRequests.GetAsync(currentOrderCode);

                // 3. Kiểm tra xem Server PayOS có công nhận đơn này đã trả tiền chưa?
                // (Lưu ý: status trả về của PayOS thường là "PAID" hoặc "CANCELLED")
                if (paymentInfo.Status == PaymentLinkStatus.Paid)
                {
                    Debug.WriteLine($"[PAYOS SUCCESS] API xác nhận: Tiền đã vào túi cho đơn {currentOrderCode}!");

                    // Trả về data từ Webhook để luồng SignalR chạy tiếp như bình thường
                    return webhookBody.Data;
                }
                else
                {
                    // Kẻ gian dùng Postman bắn Webhook giả, nhưng API PayOS báo là chưa trả tiền
                    Debug.WriteLine($"[PAYOS WARNING] Webhook báo có tiền nhưng API Server báo trạng thái là: {paymentInfo.Status}");
                    throw new Exception("Trạng thái thanh toán bị từ chối bởi Server PayOS.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PAYOS ERROR] Xác thực API thất bại: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CheckPayOSPaymentStatusAsync(long orderCode)
        {
            try
            {
                var paymentInfo = await _payOS.PaymentRequests.GetAsync(orderCode);
                return paymentInfo.Status == PaymentLinkStatus.Paid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PAYOS ERROR] Lỗi khi kiểm tra trạng thái đơn {orderCode}: {ex.Message}");
                return false;
            }
        }
    }
}