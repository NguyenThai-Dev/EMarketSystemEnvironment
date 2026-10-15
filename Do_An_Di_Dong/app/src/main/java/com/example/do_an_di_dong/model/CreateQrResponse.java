package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class CreateQrResponse {
    @SerializedName(value = "success", alternate = {"Success"})
    private boolean success;

    @SerializedName(value = "checkoutUrl", alternate = {"CheckoutUrl"})
    private String checkoutUrl;

    @SerializedName(value = "qrCode", alternate = {"QrCode"})
    private String qrCode;

    @SerializedName(value = "orderCode", alternate = {"OrderCode"})
    private long orderCode;

    @SerializedName(value = "message", alternate = {"Message"})
    private String message;

    public boolean isSuccess() { return success; }
    public void setSuccess(boolean success) { this.success = success; }

    public String getCheckoutUrl() { return checkoutUrl; }
    public void setCheckoutUrl(String checkoutUrl) { this.checkoutUrl = checkoutUrl; }

    public String getQrCode() { return qrCode; }
    public void setQrCode(String qrCode) { this.qrCode = qrCode; }

    public long getOrderCode() { return orderCode; }
    public void setOrderCode(long orderCode) { this.orderCode = orderCode; }

    public String getMessage() { return message; }
    public void setMessage(String message) { this.message = message; }
}
