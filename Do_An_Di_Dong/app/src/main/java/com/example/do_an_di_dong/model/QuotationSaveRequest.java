package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class QuotationSaveRequest {
    @SerializedName(value = "quotationId", alternate = {"QuotationId"})
    private int quotationId;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private int branchId;

    @SerializedName(value = "customerId", alternate = {"CustomerId"})
    private Integer customerId;

    @SerializedName(value = "customerName", alternate = {"CustomerName"})
    private String customerName;

    @SerializedName(value = "customerPhone", alternate = {"CustomerPhone"})
    private String customerPhone;

    @SerializedName(value = "customerAddress", alternate = {"CustomerAddress"})
    private String customerAddress;

    @SerializedName(value = "note", alternate = {"Note"})
    private String note;

    @SerializedName(value = "totalAmount", alternate = {"TotalAmount"})
    private double totalAmount;

    @SerializedName(value = "discountAmount", alternate = {"DiscountAmount"})
    private double discountAmount;

    @SerializedName(value = "manualDiscount", alternate = {"ManualDiscount"})
    private double manualDiscount;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status;

    @SerializedName(value = "details", alternate = {"Details"})
    private List<QuotationDetailRequest> details;

    public QuotationSaveRequest(int quotationId, String customerName, int branchId, double totalAmount, List<QuotationDetailRequest> details) {
        this.quotationId = quotationId;
        this.customerName = customerName;
        this.branchId = branchId;
        this.totalAmount = totalAmount;
        this.discountAmount = 0;
        this.manualDiscount = 0;
        this.status = "Draft";
        this.note = "";
        this.details = details;
    }

    public QuotationSaveRequest(int quotationId, int branchId, Integer customerId, String customerName,
                                String customerPhone, String customerAddress, String note,
                                double totalAmount, double discountAmount, double manualDiscount,
                                String status, List<QuotationDetailRequest> details) {
        this.quotationId = quotationId;
        this.branchId = branchId;
        this.customerId = customerId;
        this.customerName = customerName;
        this.customerPhone = customerPhone;
        this.customerAddress = customerAddress;
        this.note = note;
        this.totalAmount = totalAmount;
        this.discountAmount = discountAmount;
        this.manualDiscount = manualDiscount;
        this.status = status;
        this.details = details;
    }

    public int getQuotationId() { return quotationId; }
    public int getBranchId() { return branchId; }
    public Integer getCustomerId() { return customerId; }
    public String getCustomerName() { return customerName; }
    public String getCustomerPhone() { return customerPhone; }
    public String getCustomerAddress() { return customerAddress; }
    public String getNote() { return note; }
    public double getTotalAmount() { return totalAmount; }
    public double getDiscountAmount() { return discountAmount; }
    public double getManualDiscount() { return manualDiscount; }
    public String getStatus() { return status; }
    public List<QuotationDetailRequest> getDetails() { return details; }
}
