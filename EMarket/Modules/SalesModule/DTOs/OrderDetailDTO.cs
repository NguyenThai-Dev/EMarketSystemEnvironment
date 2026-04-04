namespace EMarket.Modules.SalesModule.DTOs
{
    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Discount { get; set; }

        public string ProductName { get; set; }
        public int? CustomerId { get; set; }
        public int? QuantityReturned { get; set; }
    }
}