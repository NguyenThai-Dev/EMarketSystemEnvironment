package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class InventoryForecast {
    @SerializedName(value = "productId", alternate = {"ProductId", "product_id"})
    private int productId;

    @SerializedName(value = "productName", alternate = {"ProductName", "name", "Name"})
    private String productName;

    @SerializedName(value = "currentStock", alternate = {"CurrentStock", "current_stock"})
    private int currentStock;

    @SerializedName(value = "expectedDemand30d", alternate = {"ExpectedDemand30d", "expected_demand_30d", "predictedStockIn30Days", "PredictedStockIn30Days"})
    private int predictedStockIn30Days;

    @SerializedName(value = "suggestedQty", alternate = {"SuggestedQty", "suggested_qty"})
    private int suggestedQty;

    @SerializedName(value = "outOfStockRisk", alternate = {"OutOfStockRisk"})
    private boolean outOfStockRisk;

    @SerializedName(value = "priorityLevel", alternate = {"PriorityLevel", "priority_level"})
    private String priorityLevel;

    public int getProductId() { return productId; }
    public String getProductName() { return productName; }
    public int getCurrentStock() { return currentStock; }
    public int getPredictedStockIn30Days() { return predictedStockIn30Days; }
    public int getSuggestedQty() { return suggestedQty; }
    public boolean isOutOfStockRisk() { return outOfStockRisk || (currentStock <= 5) || "CRITICAL".equalsIgnoreCase(priorityLevel) || "HIGH".equalsIgnoreCase(priorityLevel); }
    public String getPriorityLevel() { return priorityLevel; }
}
