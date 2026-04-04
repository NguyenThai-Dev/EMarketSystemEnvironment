using System;

namespace EMarket.Modules.CustomerModule.DTOs
{
    public class LoyaltyProgramDTO
    {
        public int LoyaltyId { get; set; }
        public int CustomerId { get; set; }
        public int? OrderId { get; set; }
        public int PointsEarned { get; set; }
        public int PointsRedeemed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}