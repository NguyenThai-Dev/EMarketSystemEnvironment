package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class SystemConfig {
    @SerializedName("EmailEnabled")
    private boolean emailEnabled;

    @SerializedName("TelegramEnabled")
    private boolean telegramEnabled;

    @SerializedName("TelegramToken")
    private String telegramToken;

    @SerializedName("TelegramChatId")
    private String telegramChatId;

    @SerializedName("AppBaseUrl")
    private String appBaseUrl;

    @SerializedName("MailHost")
    private String mailHost;

    @SerializedName("MailDisplayName")
    private String mailDisplayName;

    @SerializedName("VAT")
    private String vat;

    @SerializedName("BankID")
    private String bankId;

    @SerializedName("BankNum")
    private String bankNum;

    @SerializedName("VipDiscount")
    private String vipDiscount;

    @SerializedName("MemberDiscount")
    private String memberDiscount;

    @SerializedName("PointExchangeRate")
    private int pointExchangeRate;

    @SerializedName("PointEarnedRate")
    private int pointEarnedRate;

    public boolean isEmailEnabled() { return emailEnabled; }
    public boolean isTelegramEnabled() { return telegramEnabled; }
    public String getTelegramToken() { return telegramToken; }
    public String getTelegramChatId() { return telegramChatId; }
    public String getAppBaseUrl() { return appBaseUrl; }
    public String getMailHost() { return mailHost; }
    public String getMailDisplayName() { return mailDisplayName; }
    public String getVat() { return vat; }
    public String getBankId() { return bankId; }
    public String getBankNum() { return bankNum; }
    public String getVipDiscount() { return vipDiscount; }
    public String getMemberDiscount() { return memberDiscount; }
    public int getPointExchangeRate() { return pointExchangeRate; }
    public int getPointEarnedRate() { return pointEarnedRate; }
}
