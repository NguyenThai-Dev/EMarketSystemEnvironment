package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class LotFinancialRisk {
    @SerializedName(value = "lotId", alternate = {"LotId", "lotCode", "LotCode", "lot_id"})
    private String lotCode;

    @SerializedName(value = "productName", alternate = {"ProductName", "product_name", "name", "Name"})
    private String productName;

    @SerializedName(value = "daysToExpiry", alternate = {"DaysToExpiry", "expiryDate", "ExpiryDate", "days_to_expiry"})
    private String expiryDate;

    @SerializedName(value = "provisionValue", alternate = {"ProvisionValue", "riskValue", "RiskValue", "provision_value"})
    private double riskValue;

    @SerializedName(value = "lotStatus", alternate = {"LotStatus", "status", "Status", "lot_status"})
    private String status; // DANGER, WARNING, SAFE, Critical, High

    @SerializedName(value = "costPrice", alternate = {"CostPrice", "cost_price"})
    private double costPrice;

    @SerializedName(value = "quantity", alternate = {"Quantity"})
    private int quantity;

    @SerializedName(value = "riskQty", alternate = {"RiskQty", "risk_qty"})
    private int riskQty;

    @SerializedName(value = "recommendation", alternate = {"Recommendation"})
    private String recommendation;

    public String getLotCode() { return (lotCode != null) ? lotCode : "Lô"; }
    public String getProductName() { return (productName != null) ? productName : "Sản phẩm"; }
    public String getExpiryDate() { return (expiryDate != null) ? (expiryDate.contains("T") ? expiryDate.split("T")[0] : ("Còn " + expiryDate + " ngày")) : "—"; }
    public double getRiskValue() { return riskValue; }
    public String getStatus() { return status; }
    public double getCostPrice() { return costPrice; }
    public int getQuantity() { return quantity; }
    public int getRiskQty() { return riskQty; }
    public String getRecommendation() { return recommendation; }
}
