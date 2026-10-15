package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Anomaly {
    @SerializedName(value = "categoryId", alternate = {"CategoryId", "category_id"})
    private int categoryId;

    @SerializedName(value = "categoryName", alternate = {"CategoryName", "name", "Name"})
    private String categoryName;

    @SerializedName(value = "anomalyType", alternate = {"AnomalyType", "detectionType", "DetectionType", "anomaly_type"})
    private String detectionType; // Spike/Dip

    @SerializedName(value = "actualQty", alternate = {"ActualQty", "actual_qty"})
    private int actualQty;

    @SerializedName(value = "forecastQty", alternate = {"ForecastQty", "forecast_qty"})
    private int forecastQty;

    @SerializedName(value = "deviationPercent", alternate = {"DeviationPercent", "deviation_percent"})
    private double deviationPercent;

    @SerializedName(value = "severity", alternate = {"Severity", "details", "Details"})
    private String details;

    public int getCategoryId() { return categoryId; }
    public String getCategoryName() { return categoryName; }
    public String getDetectionType() { return detectionType; }
    public int getActualQty() { return actualQty; }
    public int getForecastQty() { return forecastQty; }
    public double getDeviationPercent() { return deviationPercent; }
    public String getDetails() { return (details != null && !details.isEmpty()) ? details : ("Độ lệch: " + deviationPercent + "% (Thực tế: " + actualQty + ", Dự báo: " + forecastQty + ")"); }
}
