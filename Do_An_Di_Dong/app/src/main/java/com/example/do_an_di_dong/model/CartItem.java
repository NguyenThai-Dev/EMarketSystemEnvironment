package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class CartItem {
    @SerializedName("productId")
    private int productId;

    @SerializedName("name")
    private String productName;

    @SerializedName("sku")
    private String sku;

    @SerializedName("price")
    private double price;

    @SerializedName("quantity")
    private int quantity;

    @SerializedName("discount")
    private double discount;

    public CartItem() {}

    public CartItem(int productId, String productName, String sku, double price, int quantity) {
        this.productId = productId;
        this.productName = productName;
        this.sku = sku;
        this.price = price;
        this.quantity = quantity;
        this.discount = 0;
    }

    public int getProductId() { return productId; }
    public void setProductId(int productId) { this.productId = productId; }

    public String getProductName() { return productName != null ? productName : ""; }
    public void setProductName(String productName) { this.productName = productName; }

    public String getSku() { return sku != null ? sku : ""; }
    public void setSku(String sku) { this.sku = sku; }

    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    public int getQuantity() { return quantity; }
    public void setQuantity(int quantity) { this.quantity = quantity; }

    public double getDiscount() { return discount; }
    public void setDiscount(double discount) { this.discount = discount; }

    public double getSubtotal() {
        return (price * quantity) - discount;
    }
}
