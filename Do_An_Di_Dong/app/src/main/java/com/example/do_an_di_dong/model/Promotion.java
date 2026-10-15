package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Promotion {
    @SerializedName(value = "promotionId", alternate = {"PromotionId", "id", "Id"})
    private int promotionId;

    @SerializedName(value = "name", alternate = {"Name", "promotionName", "PromotionName"})
    private String name;

    @SerializedName(value = "discountType", alternate = {"DiscountType"})
    private String discountType;

    @SerializedName(value = "discountValue", alternate = {"DiscountValue"})
    private double discountValue;

    @SerializedName(value = "startDate", alternate = {"StartDate"})
    private String startDate;

    @SerializedName(value = "endDate", alternate = {"EndDate"})
    private String endDate;

    @SerializedName(value = "priority", alternate = {"Priority"})
    private int priority;

    @SerializedName(value = "isActive", alternate = {"IsActive"})
    private boolean isActive;

    @SerializedName(value = "categoryName", alternate = {"CategoryName"})
    private String categoryName;

    public int getPromotionId() { return promotionId; }
    public String getName() { return name; }
    public String getDiscountType() { return discountType; }
    public double getDiscountValue() { return discountValue; }
    public String getStartDate() { return startDate; }
    public String getEndDate() { return endDate; }
    public int getPriority() { return priority; }
    public boolean isActive() { return isActive; }
    public String getCategoryName() { return categoryName; }

    public double getMinOrderValue() { return 0.0; }
    public double getDiscountAmount() { return discountValue; }
}
