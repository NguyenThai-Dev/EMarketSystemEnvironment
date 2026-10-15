package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class DebtSummary {
    @SerializedName(value = "TotalDebt", alternate = {"totalDebt"})
    private double totalDebt;

    @SerializedName(value = "TotalPaid", alternate = {"totalPaid"})
    private double totalPaid;

    @SerializedName(value = "TotalRemaining", alternate = {"totalRemaining"})
    private double totalRemaining;

    @SerializedName(value = "PayRate", alternate = {"payRate"})
    private double payRate;

    @SerializedName(value = "NearDueCount", alternate = {"nearDueCount"})
    private int nearDueCount;

    @SerializedName(value = "OverdueCount", alternate = {"overdueCount"})
    private int overdueCount;

    @SerializedName(value = "TotalRecords", alternate = {"totalRecords"})
    private int totalRecords;

    public double getTotalDebt() { return totalDebt; }
    public double getTotalPaid() { return totalPaid; }
    public double getTotalRemaining() { return totalRemaining; }
    public double getPayRate() { return payRate; }
    public int getNearDueCount() { return nearDueCount; }
    public int getOverdueCount() { return overdueCount; }
    public int getTotalRecords() { return totalRecords; }
}
