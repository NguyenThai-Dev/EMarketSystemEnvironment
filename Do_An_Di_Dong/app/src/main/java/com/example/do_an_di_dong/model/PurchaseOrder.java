package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class PurchaseOrder {
    @SerializedName(value = "purchaseOrderId", alternate = {"PurchaseOrderId", "poId", "PoId", "id", "Id"})
    private int purchaseOrderId;

    @SerializedName(value = "supplierId", alternate = {"SupplierId"})
    private int supplierId;

    @SerializedName(value = "supplierName", alternate = {"SupplierName"})
    private String supplierName;

    @SerializedName(value = "warehouseId", alternate = {"WarehouseId"})
    private int warehouseId;

    @SerializedName(value = "warehouseName", alternate = {"WarehouseName"})
    private String warehouseName;

    @SerializedName(value = "orderDate", alternate = {"OrderDate", "createdAt", "CreatedAt"})
    private String orderDate;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status;

    @SerializedName(value = "totalAmount", alternate = {"TotalAmount"})
    private double totalAmount;

    @SerializedName(value = "paidAmount", alternate = {"PaidAmount"})
    private double paidAmount;

    @SerializedName(value = "dueAmount", alternate = {"DueAmount"})
    private double dueAmount;

    @SerializedName(value = "notes", alternate = {"Notes", "note", "Note"})
    private String notes;

    public int getPurchaseOrderId() { return purchaseOrderId; }
    public int getSupplierId() { return supplierId; }
    public String getSupplierName() { return supplierName; }
    public int getWarehouseId() { return warehouseId; }
    public String getWarehouseName() { return warehouseName; }
    public String getOrderDate() { return orderDate; }
    public String getStatus() { return status; }
    public double getTotalAmount() { return totalAmount; }
    public double getPaidAmount() { return paidAmount; }
    public double getDueAmount() { return dueAmount; }
    public String getNotes() { return notes; }
}
