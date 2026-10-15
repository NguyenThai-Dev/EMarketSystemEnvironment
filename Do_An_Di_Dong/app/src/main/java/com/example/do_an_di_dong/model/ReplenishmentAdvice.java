package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ReplenishmentAdvice {
    @SerializedName(value = "productId", alternate = {"ProductId", "product_id", "id", "Id"})
    private int productId;

    @SerializedName(value = "productName", alternate = {"ProductName", "name", "Name"})
    private String productName;

    @SerializedName(value = "currentStock", alternate = {"CurrentStock", "current_stock"})
    private int currentStock;

    @SerializedName(value = "suggestedQty", alternate = {"SuggestedQty", "suggestedQuantity", "SuggestedQuantity", "suggested_qty"})
    private int suggestedQuantity;

    @SerializedName(value = "expectedDemand", alternate = {"ExpectedDemand", "expectedDemand30d", "ExpectedDemand30d", "expected_demand_30d"})
    private int expectedDemand;

    @SerializedName(value = "reason", alternate = {"Reason", "priority_level", "PriorityLevel", "priorityLevel"})
    private String reason;

    @SerializedName(value = "confidenceLevel", alternate = {"ConfidenceLevel", "confidenceScore", "ConfidenceScore", "confidence_score"})
    private double confidenceLevel;

    public int getProductId() { return productId; }
    public String getProductName() { return productName; }
    public int getCurrentStock() { return currentStock; }
    public int getSuggestedQuantity() { return suggestedQuantity; }
    public int getExpectedDemand() { return expectedDemand; }
    public String getReason() { return reason; }
    public double getConfidenceLevel() { return confidenceLevel; }
}
