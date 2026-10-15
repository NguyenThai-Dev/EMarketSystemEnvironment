package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class DashboardTrend {
    @SerializedName(value = "labels", alternate = {"Labels"})
    private List<String> labels;

    @SerializedName(value = "sales", alternate = {"Sales", "revenue", "Revenue"})
    private List<Double> sales;

    @SerializedName(value = "purchases", alternate = {"Purchases"})
    private List<Double> purchases;

    public List<String> getLabels() { return labels; }
    public List<Double> getSales() { return sales; }
    public List<Double> getPurchases() { return purchases; }
}
