package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class QuotationDetailRequest {
    @SerializedName(value = "detailId", alternate = {"DetailId", "id", "Id"})
    private int detailId;

    @SerializedName(value = "productId", alternate = {"ProductId"})
    private int productId;

    @SerializedName(value = "productName", alternate = {"ProductName", "name", "Name"})
    private String productName;

    @SerializedName(value = "unit", alternate = {"Unit"})
    private String unit;

    @SerializedName(value = "quantity", alternate = {"Quantity", "qty", "Qty"})
    private int quantity;

    @SerializedName(value = "unitPrice", alternate = {"UnitPrice", "price", "Price"})
    private double unitPrice;

    @SerializedName(value = "discount", alternate = {"Discount"})
    private double discount;

    @SerializedName(value = "totalPrice", alternate = {"TotalPrice"})
    private double totalPrice;

    public QuotationDetailRequest() {}

    public QuotationDetailRequest(int productId, int quantity, double unitPrice) {
        this.productId = productId;
        this.productName = "";
        this.unit = "Cái";
        this.quantity = quantity;
        this.unitPrice = unitPrice;
        this.discount = 0;
        this.totalPrice = quantity * unitPrice;
    }

    public QuotationDetailRequest(int productId, String productName, String unit, int quantity, double unitPrice, double discount, double totalPrice) {
        this.productId = productId;
        this.productName = productName;
        this.unit = unit;
        this.quantity = quantity;
        this.unitPrice = unitPrice;
        this.discount = discount;
        this.totalPrice = totalPrice;
    }

    public int getDetailId() { return detailId; }
    public int getProductId() { return productId; }
    public String getProductName() { return productName; }
    public String getUnit() { return unit; }
    public int getQuantity() { return quantity; }
    public double getUnitPrice() { return unitPrice; }
    public double getDiscount() { return discount; }
    public double getTotalPrice() { return totalPrice; }
}
