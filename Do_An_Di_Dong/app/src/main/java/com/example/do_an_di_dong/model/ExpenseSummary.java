package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ExpenseSummary {
    @SerializedName(value = "TotalExpenses", alternate = {"totalExpenses"})
    private int totalExpenses;

    @SerializedName(value = "TotalApprovedAmount", alternate = {"totalApprovedAmount"})
    private double totalApprovedAmount;

    @SerializedName(value = "TotalPendingAmount", alternate = {"totalPendingAmount"})
    private double totalPendingAmount;

    @SerializedName(value = "TotalRejectedAmount", alternate = {"totalRejectedAmount"})
    private double totalRejectedAmount;

    @SerializedName(value = "ApprovedCount", alternate = {"approvedCount"})
    private int approvedCount;

    @SerializedName(value = "PendingCount", alternate = {"pendingCount"})
    private int pendingCount;

    @SerializedName(value = "RejectedCount", alternate = {"rejectedCount"})
    private int rejectedCount;

    public int getTotalExpenses() { return totalExpenses; }
    public double getTotalApprovedAmount() { return totalApprovedAmount; }
    public double getTotalPendingAmount() { return totalPendingAmount; }
    public double getTotalRejectedAmount() { return totalRejectedAmount; }
    public int getApprovedCount() { return approvedCount; }
    public int getPendingCount() { return pendingCount; }
    public int getRejectedCount() { return rejectedCount; }
}
