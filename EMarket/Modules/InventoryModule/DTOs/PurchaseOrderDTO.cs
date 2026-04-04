using System;
using System.Collections.Generic;

namespace EMarket.Modules.InventoryModule.DTOs
{
    public class PurchaseOrderDTO
    {
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public int WarehouseId { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; }
        public List<PurchaseOrderDetailDTO> Details { get; set; }
        public List<SupplierDebtDTO> SupplierDetail { get; set; }

        // Calculated / UI fields
        public decimal? PaidAmount { get; set; }
        public decimal? DueAmount { get; set; }

        // Names for UI
        public string SupplierName { get; set; }
        public string WarehouseName { get; set; }
        public string UserName { get; set; }
        public string BranchName { get; set; }
    }
}