package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class CheckoutRequest {
    @SerializedName(value = "userId", alternate = {"UserId"})
    private Integer userId = 1;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private Integer branchId = 1;

    @SerializedName(value = "customerId", alternate = {"CustomerId"})
    private Integer customerId = 1;

    @SerializedName(value = "pointsUsed", alternate = {"PointsUsed"})
    private int pointsUsed = 0;

    @SerializedName(value = "pointsEarned", alternate = {"PointsEarned"})
    private int pointsEarned = 0;

    @SerializedName(value = "totalAmount", alternate = {"TotalAmount"})
    private double totalAmount;

    @SerializedName(value = "manualDiscount", alternate = {"ManualDiscount"})
    private double manualDiscount = 0;

    @SerializedName(value = "manualDiscountReason", alternate = {"ManualDiscountReason"})
    private String manualDiscountReason = "";

    @SerializedName(value = "status", alternate = {"Status"})
    private String status = "Completed";

    @SerializedName(value = "paymentMethod", alternate = {"PaymentMethod"})
    private String paymentMethod = "Cash";

    @SerializedName(value = "connectionId", alternate = {"ConnectionId"})
    private String connectionId = "";

    @SerializedName(value = "items", alternate = {"Items"})
    private List<CartItem> items;

    public CheckoutRequest() {}

    public CheckoutRequest(Integer customerId, double totalAmount, String paymentMethod, List<CartItem> items) {
        this.customerId = (customerId != null && customerId > 0) ? customerId : 1;
        this.totalAmount = totalAmount;
        this.paymentMethod = paymentMethod;
        this.items = items;
        this.status = "Completed";
    }

    public Integer getUserId() { return userId; }
    public void setUserId(Integer userId) { this.userId = userId; }

    public Integer getBranchId() { return branchId; }
    public void setBranchId(Integer branchId) { this.branchId = branchId; }

    public Integer getCustomerId() { return customerId; }
    public void setCustomerId(Integer customerId) { this.customerId = (customerId != null && customerId > 0) ? customerId : 1; }

    public double getTotalAmount() { return totalAmount; }
    public void setTotalAmount(double totalAmount) { this.totalAmount = totalAmount; }

    public String getPaymentMethod() { return paymentMethod; }
    public void setPaymentMethod(String paymentMethod) { this.paymentMethod = paymentMethod; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }

    public int getPointsUsed() { return pointsUsed; }
    public void setPointsUsed(int pointsUsed) { this.pointsUsed = pointsUsed; }

    public int getPointsEarned() { return pointsEarned; }
    public void setPointsEarned(int pointsEarned) { this.pointsEarned = pointsEarned; }

    public double getManualDiscount() { return manualDiscount; }
    public void setManualDiscount(double manualDiscount) { this.manualDiscount = manualDiscount; }

    public String getManualDiscountReason() { return manualDiscountReason; }
    public void setManualDiscountReason(String manualDiscountReason) { this.manualDiscountReason = manualDiscountReason; }

    public String getConnectionId() { return connectionId; }
    public void setConnectionId(String connectionId) { this.connectionId = connectionId; }

    public List<CartItem> getItems() { return items; }
    public void setItems(List<CartItem> items) { this.items = items; }
}
