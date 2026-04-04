using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.CustomerModule.DTOs;
using EMarket.Modules.CustomerModule.Services.Interfaces;

namespace EMarket.Modules.CustomerModule.Services.Implementations
{
    public class LoyaltyProgramService : ILoyaltyProgramService
    {
        private readonly EMarket_DBEntities _db;
        private readonly DateTime defaultDate = new DateTime(2000, 1, 1);

        public LoyaltyProgramService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<List<LoyaltyProgramDTO>> GetAllLoyaltyAsync()
        {
            return await _db.LoyaltyPrograms
                .Select(x => new LoyaltyProgramDTO
                {
                    LoyaltyId = x.loyalty_id,
                    CustomerId = x.customer_id,
                    OrderId = x.order_id,
                    PointsEarned = x.points_earned ?? 0,
                    PointsRedeemed = x.points_redeemed ?? 0,
                    CreatedAt = x.created_at ?? defaultDate
                })
                .ToListAsync();
        }

        public async Task<LoyaltyProgramDTO> GetLoyaltyByIdAsync(int id)
        {
            var e = await _db.LoyaltyPrograms.FindAsync(id);
            if (e == null) return null;

            return new LoyaltyProgramDTO
            {
                LoyaltyId = e.loyalty_id,
                CustomerId = e.customer_id,
                OrderId = e.order_id,
                PointsEarned = e.points_earned ?? 0,
                PointsRedeemed = e.points_redeemed ?? 0,
                CreatedAt = e.created_at ?? defaultDate
            };
        }

        public async Task<bool> CreateLoyaltyAsync(LoyaltyProgramDTO dto)
        {
            try
            {
                var e = new LoyaltyProgram
                {
                    customer_id = dto.CustomerId,
                    order_id = dto.OrderId,
                    points_earned = dto.PointsEarned,
                    points_redeemed = dto.PointsRedeemed,
                    created_at = DateTime.Now
                };

                _db.LoyaltyPrograms.Add(e);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateLoyaltyAsync(LoyaltyProgramDTO dto)
        {
            var e = await _db.LoyaltyPrograms.FindAsync(dto.LoyaltyId);
            if (e == null) return false;

            e.customer_id = dto.CustomerId;
            e.order_id = dto.OrderId;
            e.points_earned = dto.PointsEarned;
            e.points_redeemed = dto.PointsRedeemed;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLoyaltyAsync(int id)
        {
            var e = await _db.LoyaltyPrograms.FindAsync(id);
            if (e == null) return false;

            _db.LoyaltyPrograms.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}