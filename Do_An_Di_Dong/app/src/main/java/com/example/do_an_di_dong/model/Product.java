package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Product {
    @SerializedName(value = "productId", alternate = {"ProductId", "product_id", "Id", "id"})
    private int productId;

    @SerializedName(value = "name", alternate = {"Name", "productName", "ProductName"})
    private String name;

    @SerializedName(value = "barcode", alternate = {"Barcode", "sku", "Sku", "SKU"})
    private String sku;

    @SerializedName(value = "price", alternate = {"Price", "finalPrice", "FinalPrice", "originalPrice", "OriginalPrice"})
    private double price;

    @SerializedName(value = "quantity", alternate = {"Quantity", "stockQuantity", "StockQuantity", "stock", "Stock", "currentStock", "CurrentStock"})
    private int stockQuantity;

    @SerializedName(value = "image", alternate = {"Image", "imageUrl", "ImageUrl", "productImage", "ProductImage"})
    private String imageUrl;

    @SerializedName(value = "unit", alternate = {"Unit"})
    private String unit;

    @SerializedName(value = "description", alternate = {"Description"})
    private String description;

    @SerializedName(value = "categoryName", alternate = {"CategoryName"})
    private String categoryName;

    @SerializedName(value = "supplierName", alternate = {"SupplierName"})
    private String supplierName;

    public Product() {}

    public Product(int productId, String name, String sku, double price, int stockQuantity, String imageUrl) {
        this.productId = productId;
        this.name = name;
        this.sku = sku;
        this.price = price;
        this.stockQuantity = stockQuantity;
        this.imageUrl = imageUrl;
    }

    public int getProductId() { return productId; }
    public void setProductId(int productId) { this.productId = productId; }

    public String getName() { return name != null ? name : ""; }
    public void setName(String name) { this.name = name; }

    public String getSku() { return sku != null ? sku : ""; }
    public void setSku(String sku) { this.sku = sku; }

    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    public int getStockQuantity() { return stockQuantity; }
    public void setStockQuantity(int stockQuantity) { this.stockQuantity = stockQuantity; }

    public String getImageUrl() { return imageUrl; }
    public void setImageUrl(String imageUrl) { this.imageUrl = imageUrl; }

    public String getUnit() { return unit; }
    public void setUnit(String unit) { this.unit = unit; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public String getCategoryName() { return categoryName; }
    public void setCategoryName(String categoryName) { this.categoryName = categoryName; }

    public String getSupplierName() { return supplierName; }
    public void setSupplierName(String supplierName) { this.supplierName = supplierName; }

    public int getActualStock() { return stockQuantity; }
    public String getImage() { return imageUrl; }
    public String getBarcode() { return sku; }
    public String getFullImageUrl() {
        return com.example.do_an_di_dong.data.api.ApiClient.getFullImageUrl(imageUrl);
    }
}
