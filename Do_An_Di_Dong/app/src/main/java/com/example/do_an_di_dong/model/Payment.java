package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Payment {
    @SerializedName(value = "paymentId", alternate = {"PaymentId", "id", "Id"})
    private int paymentId;

    @SerializedName(value = "orderId", alternate = {"OrderId"})
    private int orderId;

    @SerializedName(value = "paymentMethod", alternate = {"PaymentMethod"})
    private String paymentMethod;

    @SerializedName(value = "amount", alternate = {"Amount"})
    private double amount;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status;

    @SerializedName(value = "paymentDate", alternate = {"PaymentDate", "createdAt", "CreatedAt"})
    private String paymentDate;

    public int getPaymentId() { return paymentId; }
    public int getOrderId() { return orderId; }
    public String getPaymentMethod() { return paymentMethod; }
    public double getAmount() { return amount; }
    public String getStatus() { return status; }
    public String getPaymentDate() { return paymentDate; }
}
