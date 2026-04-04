using System;
using System.Collections.Generic;

namespace EMarket.Modules.SalesModule.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int? DeliveryAddressId { get; set; }

        public List<OrderDetailDTO> OrderDetails { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        public string CustomerName { get; set; }
        public string UserName { get; set; }
    }
}