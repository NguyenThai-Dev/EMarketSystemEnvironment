package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class PayOSStatusResponse {
    @SerializedName(value = "success", alternate = {"Success"})
    private boolean success;

    @SerializedName(value = "isPaid", alternate = {"IsPaid", "paid"})
    private boolean isPaid;

    @SerializedName(value = "orderCode", alternate = {"OrderCode"})
    private long orderCode;

    public boolean isSuccess() { return success; }
    public void setSuccess(boolean success) { this.success = success; }

    public boolean isPaid() { return isPaid; }
    public void setPaid(boolean paid) { isPaid = paid; }

    public long getOrderCode() { return orderCode; }
    public void setOrderCode(long orderCode) { this.orderCode = orderCode; }
}
