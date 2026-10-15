package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class LoyaltyProgram {
    @SerializedName(value = "loyaltyId", alternate = {"LoyaltyId", "id", "Id"})
    private int loyaltyId;

    @SerializedName(value = "customerId", alternate = {"CustomerId"})
    private int customerId;

    @SerializedName(value = "pointsEarned", alternate = {"PointsEarned"})
    private int pointsEarned;

    @SerializedName(value = "pointsRedeemed", alternate = {"PointsRedeemed"})
    private int pointsRedeemed;

    @SerializedName(value = "orderId", alternate = {"OrderId"})
    private Integer orderId;

    public LoyaltyProgram() {}

    public LoyaltyProgram(int customerId, int pointsEarned, int pointsRedeemed, Integer orderId) {
        this.customerId = customerId;
        this.pointsEarned = pointsEarned;
        this.pointsRedeemed = pointsRedeemed;
        this.orderId = orderId;
    }

    public int getLoyaltyId() { return loyaltyId; }
    public void setLoyaltyId(int loyaltyId) { this.loyaltyId = loyaltyId; }

    public int getCustomerId() { return customerId; }
    public void setCustomerId(int customerId) { this.customerId = customerId; }

    public int getPointsEarned() { return pointsEarned; }
    public void setPointsEarned(int pointsEarned) { this.pointsEarned = pointsEarned; }

    public int getPointsRedeemed() { return pointsRedeemed; }
    public void setPointsRedeemed(int pointsRedeemed) { this.pointsRedeemed = pointsRedeemed; }

    public Integer getOrderId() { return orderId; }
    public void setOrderId(Integer orderId) { this.orderId = orderId; }
}
