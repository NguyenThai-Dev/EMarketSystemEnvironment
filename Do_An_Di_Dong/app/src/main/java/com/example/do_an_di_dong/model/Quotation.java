package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class Quotation {
    @SerializedName(value = "quotationId", alternate = {"QuotationId", "id", "Id"})
    private int quotationId;

    @SerializedName(value = "quotationCode", alternate = {"QuotationCode", "code", "Code"})
    private String quotationCode;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private int branchId;

    @SerializedName(value = "branchName", alternate = {"BranchName"})
    private String branchName;

    @SerializedName(value = "customerId", alternate = {"CustomerId"})
    private Integer customerId;

    @SerializedName(value = "customerName", alternate = {"CustomerName"})
    private String customerName;

    @SerializedName(value = "customerPhone", alternate = {"CustomerPhone"})
    private String customerPhone;

    @SerializedName(value = "customerAddress", alternate = {"CustomerAddress"})
    private String customerAddress;

    @SerializedName(value = "userId", alternate = {"UserId"})
    private int userId;

    @SerializedName(value = "creatorName", alternate = {"CreatorName"})
    private String creatorName;

    @SerializedName(value = "issueDate", alternate = {"IssueDate", "createdAt", "CreatedAt"})
    private String issueDate;

    @SerializedName(value = "expiryDate", alternate = {"ExpiryDate"})
    private String expiryDate;

    @SerializedName(value = "totalAmount", alternate = {"TotalAmount"})
    private double totalAmount;

    @SerializedName(value = "discountAmount", alternate = {"DiscountAmount"})
    private double discountAmount;

    @SerializedName(value = "manualDiscount", alternate = {"ManualDiscount"})
    private double manualDiscount;

    @SerializedName(value = "discountReason", alternate = {"DiscountReason"})
    private String discountReason;

    @SerializedName(value = "finalAmount", alternate = {"FinalAmount"})
    private double finalAmount;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status;

    @SerializedName(value = "note", alternate = {"Note"})
    private String note;

    @SerializedName(value = "convertedOrderId", alternate = {"ConvertedOrderId"})
    private Integer convertedOrderId;

    @SerializedName(value = "details", alternate = {"Details"})
    private List<QuotationDetailRequest> details;

    public int getQuotationId() { return quotationId; }
    public String getQuotationCode() { return quotationCode; }
    public int getBranchId() { return branchId; }
    public String getBranchName() { return branchName; }
    public Integer getCustomerId() { return customerId; }
    public String getCustomerName() { return customerName; }
    public String getCustomerPhone() { return customerPhone; }
    public String getCustomerAddress() { return customerAddress; }
    public int getUserId() { return userId; }
    public String getCreatorName() { return creatorName; }
    public String getIssueDate() { return issueDate; }
    public String getExpiryDate() { return expiryDate; }
    public double getTotalAmount() { return totalAmount; }
    public double getDiscountAmount() { return discountAmount; }
    public double getManualDiscount() { return manualDiscount; }
    public String getDiscountReason() { return discountReason; }
    public double getFinalAmount() { return (finalAmount > 0) ? finalAmount : (totalAmount - discountAmount - manualDiscount); }
    public String getStatus() { return status; }
    public String getNote() { return note; }
    public Integer getConvertedOrderId() { return convertedOrderId; }
    public List<QuotationDetailRequest> getDetails() { return details; }
}
