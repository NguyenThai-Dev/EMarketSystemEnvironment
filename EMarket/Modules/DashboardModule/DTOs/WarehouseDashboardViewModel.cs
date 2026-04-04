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
        public int TotalSku { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockCount { get; set; }
        public double CapacityPercent { get; set; }
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
}