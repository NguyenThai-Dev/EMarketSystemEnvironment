package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class DashboardOverview {
    @SerializedName(value = "summary", alternate = {"Summary"})
    private DashboardSummary summary;

    @SerializedName(value = "trend", alternate = {"Trend"})
    private DashboardTrend trend;

    @SerializedName(value = "branchPerformance", alternate = {"BranchPerformance"})
    private List<BranchPerformance> branchPerformance;

    public DashboardSummary getSummary() { return summary; }
    public DashboardTrend getTrend() { return trend; }
    public List<BranchPerformance> getBranchPerformance() { return branchPerformance; }
}
