using System.Collections.Generic;
using EMarket.Modules.ProductModule.DTOs;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class PurchaseOrderDetailDTO
    {
        public int PurchaseOrderDetailId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int LotId { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }

        // UI helpers
        public string ProductName { get; set; }
        public string CategoryName { get; set; }

        public List<ProductLotDTO> ProductLots { get; set; }
    }
}