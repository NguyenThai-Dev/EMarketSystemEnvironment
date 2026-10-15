package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Expense {
    @SerializedName(value = "expenseId", alternate = {"ExpenseId", "id", "Id"})
    private int expenseId;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private int branchId;

    @SerializedName(value = "branchName", alternate = {"BranchName"})
    private String branchName;

    @SerializedName(value = "categoryId", alternate = {"CategoryId"})
    private int categoryId;

    @SerializedName(value = "categoryName", alternate = {"CategoryName"})
    private String categoryName;

    @SerializedName(value = "userId", alternate = {"UserId"})
    private int userId;

    @SerializedName(value = "userName", alternate = {"UserName"})
    private String userName;

    @SerializedName(value = "amount", alternate = {"Amount"})
    private double amount;

    @SerializedName(value = "expenseDate", alternate = {"ExpenseDate", "createdAt", "CreatedAt"})
    private String expenseDate;

    @SerializedName(value = "note", alternate = {"Note", "description", "Description"})
    private String note;

    @SerializedName(value = "refImage", alternate = {"RefImage", "image", "Image"})
    private String refImage;

    @SerializedName(value = "paymentMethod", alternate = {"PaymentMethod"})
    private String paymentMethod;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status;

    @SerializedName(value = "approverName", alternate = {"ApproverName"})
    private String approverName;

    public int getExpenseId() { return expenseId; }
    public int getBranchId() { return branchId; }
    public String getBranchName() { return branchName; }
    public int getCategoryId() { return categoryId; }
    public String getCategoryName() { return categoryName; }
    public int getUserId() { return userId; }
    public String getUserName() { return userName; }
    public double getAmount() { return amount; }
    public String getExpenseDate() { return expenseDate; }
    public String getNote() { return note; }
    public String getRefImage() { return refImage; }
    public String getPaymentMethod() { return paymentMethod; }
    public String getStatus() { return status; }
    public String getApproverName() { return approverName; }
}
