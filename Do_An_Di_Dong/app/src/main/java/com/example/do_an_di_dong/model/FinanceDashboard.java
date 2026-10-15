package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class FinanceDashboard {
    @SerializedName(value = "data", alternate = {"Data"})
    private FinanceDashboard data;

    @SerializedName(value = "kpi", alternate = {"Kpi"})
    private FinanceKpi kpi;

    @SerializedName(value = "revenue", alternate = {"Revenue", "totalRevenue", "TotalRevenue", "totalSales", "TotalSales"})
    private double revenue;

    @SerializedName(value = "profit", alternate = {"Profit", "grossProfit", "GrossProfit", "actualProfit", "ActualProfit"})
    private double profit;

    @SerializedName(value = "cashFlow", alternate = {"CashFlow", "totalPurchase", "TotalPurchase"})
    private double cashFlow;

    public double getRevenue() {
        if (data != null) return data.getRevenue();
        if (kpi != null && kpi.getTotalRevenue() > 0) return kpi.getTotalRevenue();
        return revenue;
    }

    public double getProfit() {
        if (data != null) return data.getProfit();
        if (kpi != null && kpi.getGrossProfit() > 0) return kpi.getGrossProfit();
        return profit;
    }

    public double getCashFlow() {
        if (data != null) return data.getCashFlow();
        if (kpi != null && kpi.getActualProfit() > 0) return kpi.getActualProfit();
        return cashFlow;
    }

    public static class FinanceKpi {
        @SerializedName(value = "totalRevenue", alternate = {"TotalRevenue"})
        private double totalRevenue;

        @SerializedName(value = "totalPurchase", alternate = {"TotalPurchase"})
        private double totalPurchase;

        @SerializedName(value = "grossProfit", alternate = {"GrossProfit"})
        private double grossProfit;

        @SerializedName(value = "actualProfit", alternate = {"ActualProfit"})
        private double actualProfit;

        public double getTotalRevenue() { return totalRevenue; }
        public double getTotalPurchase() { return totalPurchase; }
        public double getGrossProfit() { return grossProfit; }
        public double getActualProfit() { return actualProfit; }
    }
}
