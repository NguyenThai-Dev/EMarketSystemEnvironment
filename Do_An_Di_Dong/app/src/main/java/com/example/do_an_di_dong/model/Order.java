package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class Order {
    @SerializedName(value = "orderId", alternate = {"OrderId", "id", "Id"})
    private int orderId;

    @SerializedName(value = "orderCode", alternate = {"OrderCode", "code", "Code"})
    private String orderCode;

    @SerializedName(value = "customerId", alternate = {"CustomerId"})
    private int customerId;

    @SerializedName(value = "customerName", alternate = {"CustomerName"})
    private String customerName;

    @SerializedName(value = "userId", alternate = {"UserId"})
    private Integer userId;

    @SerializedName(value = "userName", alternate = {"UserName"})
    private String userName;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private Integer branchId;

    @SerializedName(value = "orderDate", alternate = {"OrderDate", "createdAt", "CreatedAt"})
    private String orderDate;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status;

    @SerializedName(value = "totalAmount", alternate = {"TotalAmount"})
    private double totalAmount;

    @SerializedName(value = "paymentMethod", alternate = {"PaymentMethod"})
    private String paymentMethod;

    @SerializedName(value = "paymentStatus", alternate = {"PaymentStatus"})
    private String paymentStatus;

    @SerializedName(value = "orderDetails", alternate = {"OrderDetails", "details", "Details"})
    private List<OrderDetail> orderDetails;

    public int getOrderId() { return orderId; }
    public String getOrderCode() { return (orderCode != null && !orderCode.isEmpty()) ? orderCode : ("DH" + orderId); }
    public int getCustomerId() { return customerId; }
    public String getCustomerName() { return customerName; }
    public Integer getUserId() { return userId; }
    public String getUserName() { return userName; }
    public Integer getBranchId() { return branchId; }
    public String getOrderDate() { return orderDate; }
    public String getStatus() { return status; }
    public double getTotalAmount() { return totalAmount; }
    public String getPaymentMethod() { return paymentMethod; }
    public String getPaymentStatus() { return paymentStatus; }
    public List<OrderDetail> getOrderDetails() { return orderDetails; }
}
