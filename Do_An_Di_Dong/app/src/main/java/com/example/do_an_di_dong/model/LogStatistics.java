package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class LogStatistics {
    @SerializedName(value = "todayAudit", alternate = {"TodayAudit", "todayAuditLogs", "TodayAuditLogs"})
    private int todayAudit;

    @SerializedName(value = "todayErrors", alternate = {"TodayErrors", "todayAppErrors", "TodayAppErrors", "errorLogs", "ErrorLogs"})
    private int todayErrors;

    @SerializedName(value = "totalAudit", alternate = {"TotalAudit", "totalLogs", "TotalLogs"})
    private int totalAudit;

    public int getTodayAudit() { return todayAudit; }
    public int getTodayErrors() { return todayErrors; }
    public int getTotalAudit() { return totalAudit; }

    // Backward compatibility helpers
    public int getTotalLogs() { return totalAudit > 0 ? totalAudit : todayAudit; }
    public int getErrorLogs() { return todayErrors; }
}
