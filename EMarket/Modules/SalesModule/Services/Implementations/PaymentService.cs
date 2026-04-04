using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.SalesModule.DTOs;
using EMarket.Modules.SalesModule.Services.Interfaces;

namespace EMarket.Modules.SalesModule.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly EMarket_DBEntities _db;
        private readonly DateTime _defaultDate = new DateTime(2000, 01, 01);

        public PaymentService(EMarket_DBEntities db)
        {
            _db = db;
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
    }
}