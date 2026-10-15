package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class DashboardSummary {
    @SerializedName(value = "totalSales", alternate = {"TotalSales", "revenue", "Revenue"})
    private double totalSales;

    @SerializedName(value = "totalPurchase", alternate = {"TotalPurchase"})
    private double totalPurchase;

    @SerializedName(value = "totalOrders", alternate = {"TotalOrders", "orders", "Orders"})
    private int totalOrders;

    @SerializedName(value = "lowStockProducts", alternate = {"LowStockProducts", "lowStock", "LowStock"})
    private int lowStockProducts;

    @SerializedName(value = "totalProducts", alternate = {"TotalProducts", "newCustomers", "NewCustomers"})
    private int totalProducts;

    @SerializedName(value = "grossProfit", alternate = {"GrossProfit"})
    private double grossProfit;

    @SerializedName(value = "actualProfit", alternate = {"ActualProfit"})
    private double actualProfit;

    public double getTotalSales() { return totalSales; }
    public double getTotalPurchase() { return totalPurchase; }
    public int getTotalOrders() { return totalOrders; }
    public int getLowStockProducts() { return lowStockProducts; }
    public int getTotalProducts() { return totalProducts; }
    public double getGrossProfit() { return grossProfit; }
    public double getActualProfit() { return actualProfit; }
}
