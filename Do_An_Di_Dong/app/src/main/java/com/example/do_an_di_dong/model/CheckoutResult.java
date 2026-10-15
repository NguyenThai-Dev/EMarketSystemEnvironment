package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class CheckoutResult {
    @SerializedName(value = "success", alternate = {"Success"})
    private boolean success;

    @SerializedName(value = "orderId", alternate = {"OrderId"})
    private int orderId;

    @SerializedName(value = "message", alternate = {"Message"})
    private String message;

    public boolean isSuccess() { return success; }
    public int getOrderId() { return orderId; }
    public String getMessage() { return message; }
}
