package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class WarehouseDashboard {
    @SerializedName(value = "data", alternate = {"Data"})
    private WarehouseDashboard data;

    @SerializedName(value = "kpi", alternate = {"Kpi"})
    private WarehouseKpi kpi;

    @SerializedName(value = "totalStockIn", alternate = {"TotalStockIn", "totalInventoryValue", "TotalInventoryValue"})
    private double totalStockIn;

    @SerializedName(value = "totalStockOut", alternate = {"TotalStockOut", "totalInventoryExpiryValue", "TotalInventoryExpiryValue"})
    private double totalStockOut;

    @SerializedName(value = "stockMovementCount", alternate = {"StockMovementCount", "totalSku", "TotalSku"})
    private int stockMovementCount;

    public double getTotalStockIn() {
        if (data != null) return data.getTotalStockIn();
        if (kpi != null && kpi.getTotalInventoryValue() > 0) return kpi.getTotalInventoryValue();
        return totalStockIn;
    }

    public double getTotalStockOut() {
        if (data != null) return data.getTotalStockOut();
        if (kpi != null && kpi.getTotalInventoryExpiryValue() > 0) return kpi.getTotalInventoryExpiryValue();
        return totalStockOut;
    }

    public int getStockMovementCount() {
        if (data != null) return data.getStockMovementCount();
        if (kpi != null && kpi.getTotalSku() > 0) return kpi.getTotalSku();
        return stockMovementCount;
    }

    public static class WarehouseKpi {
        @SerializedName(value = "totalInventoryValue", alternate = {"TotalInventoryValue"})
        private double totalInventoryValue;

        @SerializedName(value = "totalInventoryExpiryValue", alternate = {"TotalInventoryExpiryValue"})
        private double totalInventoryExpiryValue;

        @SerializedName(value = "totalSku", alternate = {"TotalSku"})
        private int totalSku;

        public double getTotalInventoryValue() { return totalInventoryValue; }
        public double getTotalInventoryExpiryValue() { return totalInventoryExpiryValue; }
        public int getTotalSku() { return totalSku; }
    }
}
