package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class OrderDetail {
    @SerializedName(value = "orderDetailId", alternate = {"OrderDetailId", "id", "Id", "detailId", "DetailId"})
    private int orderDetailId;

    @SerializedName(value = "orderId", alternate = {"OrderId"})
    private int orderId;

    @SerializedName(value = "productId", alternate = {"ProductId"})
    private int productId;

    @SerializedName(value = "productName", alternate = {"ProductName", "name", "Name"})
    private String productName;

    @SerializedName(value = "quantity", alternate = {"Quantity", "qty", "Qty"})
    private int quantity;

    @SerializedName(value = "unitPrice", alternate = {"UnitPrice", "price", "Price"})
    private double unitPrice;

    @SerializedName(value = "discount", alternate = {"Discount"})
    private double discount;

    @SerializedName(value = "totalPrice", alternate = {"TotalPrice"})
    private double totalPrice;

    @SerializedName(value = "quantityReturned", alternate = {"QuantityReturned"})
    private int quantityReturned;

    public int getOrderDetailId() { return orderDetailId; }
    public int getOrderId() { return orderId; }
    public int getProductId() { return productId; }
    public String getProductName() { return productName; }
    public int getQuantity() { return quantity; }
    public double getUnitPrice() { return unitPrice; }
    public double getDiscount() { return discount; }
    public double getTotalPrice() { return (totalPrice > 0) ? totalPrice : ((quantity * unitPrice) - discount); }
    public double getSubtotal() { return getTotalPrice(); }
    public int getQuantityReturned() { return quantityReturned; }
}
