package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class DebtDashboard {
    @SerializedName(value = "data", alternate = {"Data"})
    private DebtDashboard data;

    @SerializedName(value = "kpi", alternate = {"Kpi"})
    private DebtKpi kpi;

    @SerializedName(value = "totalDebt", alternate = {"TotalDebt", "totalOutstanding", "TotalOutstanding"})
    private double totalDebt;

    @SerializedName(value = "overdueDebt", alternate = {"OverdueDebt", "totalOverdue", "TotalOverdue"})
    private double overdueDebt;

    @SerializedName(value = "dueSoonDebt", alternate = {"DueSoonDebt", "totalUpcoming", "TotalUpcoming"})
    private double dueSoonDebt;

    public double getTotalDebt() {
        if (data != null) return data.getTotalDebt();
        if (kpi != null && kpi.getTotalOutstanding() > 0) return kpi.getTotalOutstanding();
        return totalDebt;
    }

    public double getOverdueDebt() {
        if (data != null) return data.getOverdueDebt();
        if (kpi != null && kpi.getTotalOverdue() > 0) return kpi.getTotalOverdue();
        return overdueDebt;
    }

    public double getDueSoonDebt() {
        if (data != null) return data.getDueSoonDebt();
        if (kpi != null && kpi.getTotalUpcoming() > 0) return kpi.getTotalUpcoming();
        return dueSoonDebt;
    }

    public static class DebtKpi {
        @SerializedName(value = "totalOutstanding", alternate = {"TotalOutstanding"})
        private double totalOutstanding;

        @SerializedName(value = "totalOverdue", alternate = {"TotalOverdue"})
        private double totalOverdue;

        @SerializedName(value = "totalUpcoming", alternate = {"TotalUpcoming"})
        private double totalUpcoming;

        public double getTotalOutstanding() { return totalOutstanding; }
        public double getTotalOverdue() { return totalOverdue; }
        public double getTotalUpcoming() { return totalUpcoming; }
    }
}
