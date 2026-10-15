package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class SupplierDebt {
    @SerializedName(value = "debtId", alternate = {"DebtId", "id", "Id"})
    private int debtId;

    @SerializedName(value = "supplierId", alternate = {"SupplierId"})
    private int supplierId;

    @SerializedName(value = "supplierName", alternate = {"SupplierName"})
    private String supplierName;

    @SerializedName(value = "purchaseOrderId", alternate = {"PurchaseOrderId", "poId", "PoId"})
    private int purchaseOrderId;

    @SerializedName(value = "totalDebt", alternate = {"TotalDebt", "totalAmount", "TotalAmount"})
    private double totalDebt;

    @SerializedName(value = "paidAmount", alternate = {"PaidAmount"})
    private double paidAmount;

    @SerializedName(value = "unpaidAmount", alternate = {"UnpaidAmount"})
    private double unpaidAmount;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status; // Unpaid, PartiallyPaid, Paid

    @SerializedName(value = "dueDate", alternate = {"DueDate", "createdAt", "CreatedAt"})
    private String dueDate;

    public int getDebtId() { return debtId; }
    public void setDebtId(int debtId) { this.debtId = debtId; }

    public int getSupplierId() { return supplierId; }
    public void setSupplierId(int supplierId) { this.supplierId = supplierId; }

    public String getSupplierName() { return supplierName; }
    public void setSupplierName(String supplierName) { this.supplierName = supplierName; }

    public int getPurchaseOrderId() { return purchaseOrderId; }
    public void setPurchaseOrderId(int purchaseOrderId) { this.purchaseOrderId = purchaseOrderId; }

    public double getTotalDebt() { return (totalDebt > 0) ? totalDebt : unpaidAmount; }
    public void setTotalDebt(double totalDebt) { this.totalDebt = totalDebt; }

    public double getPaidAmount() { return paidAmount; }
    public void setPaidAmount(double paidAmount) { this.paidAmount = paidAmount; }

    public double getUnpaidAmount() { return unpaidAmount; }
    public void setUnpaidAmount(double unpaidAmount) { this.unpaidAmount = unpaidAmount; }

    public String getStatus() { return status; }
    public void setStatus(String status) { this.status = status; }

    public String getDueDate() { return dueDate; }
    public void setDueDate(String dueDate) { this.dueDate = dueDate; }
}
