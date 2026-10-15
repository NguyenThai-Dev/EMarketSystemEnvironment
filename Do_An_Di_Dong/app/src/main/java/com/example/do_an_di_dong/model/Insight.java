package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Insight {
    @SerializedName(value = "productId", alternate = {"ProductId", "product_id", "id", "Id"})
    private int productId;

    @SerializedName(value = "productName", alternate = {"ProductName", "name", "Name"})
    private String productName;

    @SerializedName(value = "insightLevel", alternate = {"InsightLevel", "type", "Type", "insight_level"})
    private String type; // Star, Trending, Warning

    @SerializedName(value = "growthPercent", alternate = {"GrowthPercent", "growth_percent"})
    private double growthPercent;

    @SerializedName(value = "contributionPercent", alternate = {"ContributionPercent", "contribution_percent"})
    private double contributionPercent;

    @SerializedName(value = "description", alternate = {"Description"})
    private String description;

    public int getProductId() { return productId; }
    public String getProductName() { return productName; }
    public String getType() { return type; }
    public double getGrowthPercent() { return growthPercent; }
    public double getContributionPercent() { return contributionPercent; }
    public String getDescription() { return (description != null && !description.isEmpty()) ? description : ("Tăng trưởng: " + growthPercent + "% | Đóng góp: " + contributionPercent + "%"); }
}
