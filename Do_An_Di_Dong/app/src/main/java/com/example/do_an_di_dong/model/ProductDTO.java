package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ProductDTO {
    @SerializedName(value = "productId", alternate = {"ProductId", "id", "Id"})
    private int productId;

    @SerializedName(value = "name", alternate = {"Name", "productName", "ProductName"})
    private String name;

    @SerializedName(value = "barcode", alternate = {"Barcode", "sku", "Sku"})
    private String barcode;

    @SerializedName(value = "unit", alternate = {"Unit"})
    private String unit;

    @SerializedName(value = "description", alternate = {"Description"})
    private String description;

    @SerializedName(value = "categoryId", alternate = {"CategoryId"})
    private int categoryId;

    @SerializedName(value = "supplierId", alternate = {"SupplierId"})
    private int supplierId;

    @SerializedName(value = "price", alternate = {"Price", "finalPrice", "FinalPrice"})
    private double price;

    @SerializedName(value = "minStock", alternate = {"MinStock"})
    private int minStock;

    @SerializedName(value = "maxStock", alternate = {"MaxStock"})
    private int maxStock;

    @SerializedName(value = "image", alternate = {"Image", "imageUrl", "ImageUrl"})
    private String image;

    public ProductDTO() {}

    public ProductDTO(int productId, String name, String barcode, String unit, String description,
                      int categoryId, int supplierId, double price, int minStock, int maxStock) {
        this.productId = productId;
        this.name = name;
        this.barcode = barcode;
        this.unit = unit;
        this.description = description;
        this.categoryId = categoryId;
        this.supplierId = supplierId;
        this.price = price;
        this.minStock = minStock;
        this.maxStock = maxStock;
    }

    public int getProductId() { return productId; }
    public void setProductId(int productId) { this.productId = productId; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public String getBarcode() { return barcode; }
    public void setBarcode(String barcode) { this.barcode = barcode; }

    public String getUnit() { return unit; }
    public void setUnit(String unit) { this.unit = unit; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public int getCategoryId() { return categoryId; }
    public void setCategoryId(int categoryId) { this.categoryId = categoryId; }

    public int getSupplierId() { return supplierId; }
    public void setSupplierId(int supplierId) { this.supplierId = supplierId; }

    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    public int getMinStock() { return minStock; }
    public void setMinStock(int minStock) { this.minStock = minStock; }

    public int getMaxStock() { return maxStock; }
    public void setMaxStock(int maxStock) { this.maxStock = maxStock; }

    public String getImage() { return image; }
    public void setImage(String image) { this.image = image; }
}
