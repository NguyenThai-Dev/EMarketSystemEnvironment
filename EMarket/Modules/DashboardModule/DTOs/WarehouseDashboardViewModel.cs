using System;
using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class WarehouseDashboardViewModel
    {
        public WarehouseKpi Kpi { get; set; }
        public WarehouseCharts Charts { get; set; }
        public WarehouseLists Lists { get; set; }
    }

    public class WarehouseKpi
    {
        public decimal TotalInventoryValue { get; set; }
        public decimal TotalInventoryExpiryValue { get; set; }
        public int TotalSku { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfExpiryCount { get; set; }
        public double CapacityPercent { get; set; }


        public double InventoryTurnover { get; set; } // Tỷ lệ quay vòng (Lần)
        public int DeadStockCount { get; set; } // Số lượng SKU "bất động" trên 60 ngày
        public int HighRiskExpiryCount { get; set; } // Số lượng SKU AI cảnh báo rủi ro hết hạn (AI_InventoryWarning)
    }

    public class WarehouseCharts
    {
        public MovementChart Movement { get; set; }
        public CategoryChart Categories { get; set; }
    }

    public class MovementChart
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Inbound { get; set; } = new List<int>();
        public List<int> Outbound { get; set; } = new List<int>();
    }

    public class CategoryChart
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Counts { get; set; } = new List<int>();
    }

    public class WarehouseLists
    {
        public List<MovementItem> Movements { get; set; }
        public List<LowStockItem> LowStock { get; set; }

        public List<ExpiredItem> Expired { get; set; }


        public List<AIWarningItem> AIWarnings { get; set; } // Từ bảng AI_InventoryWarning
        public List<PurchaseRecommendationItem> PurchaseAdvice { get; set; } // Từ AI_Purchase_Recommendation
    }

    public class ExpiredItem
    {
        public string Name { get; set; }
        public string LotNumber { get; set; }
        public int Qty { get; set; }
        public string ExpiryDate { get; set; }
        public string Status { get; set; }
    }

    public class MovementItem
    {
        public string Product { get; set; }
        public string Type { get; set; } // "IN" or "OUT"
        public int Qty { get; set; }
        public string User { get; set; }
        public string Time { get; set; }
    }

    public class LowStockItem
    {
        public string Name { get; set; }
        public int Current { get; set; }
        public int Min { get; set; }
    }

    public class MovementChartRow
    {
        public string DateLabel { get; set; }
        public int InboundQty { get; set; }
        public int OutboundQty { get; set; }
    }

    public class CategoryChartRow
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
    }

    // DTO cho cảnh báo AI
    public class AIWarningItem
    {
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int DaysToExhaust { get; set; } // Số ngày dự kiến sẽ hết hàng
        public string RiskReason { get; set; } // Lý do: "CRITICAL: Expiring soon..."
        public double Confidence { get; set; }
    }

    // DTO cho gợi ý nhập hàng
    public class PurchaseRecommendationItem
    {
        public string ProductName { get; set; }
        public int SuggestedMinQty { get; set; } // forecast_demand hoặc suggested_qty
        public int SuggestedMaxQty { get; set; } 
        public string Reason { get; set; } // "Mùa vụ: Sắp vào đợt cao điểm..."
        public string Priority { get; set; } // High/Medium/Low
        public DateTime ForecastDate { get; set; }
    }
}