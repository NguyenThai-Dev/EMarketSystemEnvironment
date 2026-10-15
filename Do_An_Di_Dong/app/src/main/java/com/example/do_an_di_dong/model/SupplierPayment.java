package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class SupplierPayment {
    @SerializedName(value = "paymentId", alternate = {"PaymentId", "id", "Id"})
    private int paymentId;

    @SerializedName(value = "userId", alternate = {"UserId"})
    private int userId;

    @SerializedName(value = "debtId", alternate = {"DebtId"})
    private int debtId;

    @SerializedName(value = "amount", alternate = {"Amount"})
    private double amount;

    @SerializedName(value = "paymentMethod", alternate = {"PaymentMethod"})
    private String paymentMethod; // Cash, BankTransfer

    @SerializedName(value = "paymentDate", alternate = {"PaymentDate", "createdAt", "CreatedAt"})
    private String paymentDate;

    @SerializedName(value = "paymentProof", alternate = {"PaymentProof", "proofImage", "ProofImage"})
    private String paymentProof;

    @SerializedName(value = "supplierName", alternate = {"SupplierName"})
    private String supplierName;

    @SerializedName(value = "supplierEmail", alternate = {"SupplierEmail"})
    private String supplierEmail;

    @SerializedName(value = "unpaidAmountAfterPayment", alternate = {"UnpaidAmountAfterPayment"})
    private double unpaidAmountAfterPayment;

    public int getPaymentId() { return paymentId; }
    public void setPaymentId(int paymentId) { this.paymentId = paymentId; }

    public int getUserId() { return userId; }
    public void setUserId(int userId) { this.userId = userId; }

    public int getDebtId() { return debtId; }
    public void setDebtId(int debtId) { this.debtId = debtId; }

    public double getAmount() { return amount; }
    public void setAmount(double amount) { this.amount = amount; }

    public String getPaymentMethod() { return paymentMethod; }
    public void setPaymentMethod(String paymentMethod) { this.paymentMethod = paymentMethod; }

    public String getPaymentDate() { return paymentDate; }
    public void setPaymentDate(String paymentDate) { this.paymentDate = paymentDate; }

    public String getPaymentProof() { return paymentProof; }
    public void setPaymentProof(String paymentProof) { this.paymentProof = paymentProof; }

    public String getSupplierName() { return supplierName; }
    public void setSupplierName(String supplierName) { this.supplierName = supplierName; }

    public String getSupplierEmail() { return supplierEmail; }
    public void setSupplierEmail(String supplierEmail) { this.supplierEmail = supplierEmail; }

    public double getUnpaidAmountAfterPayment() { return unpaidAmountAfterPayment; }
    public void setUnpaidAmountAfterPayment(double unpaidAmountAfterPayment) { this.unpaidAmountAfterPayment = unpaidAmountAfterPayment; }
}
