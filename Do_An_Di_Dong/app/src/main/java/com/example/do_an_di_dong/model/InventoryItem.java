package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.io.Serializable;

public class InventoryItem implements Serializable {
    @SerializedName(value = "inventoryId", alternate = {"InventoryId", "id", "Id"})
    private int inventoryId;

    @SerializedName(value = "lotId", alternate = {"LotId"})
    private int lotId;

    @SerializedName(value = "productId", alternate = {"ProductId"})
    private int productId;

    @SerializedName(value = "productName", alternate = {"ProductName", "name", "Name"})
    private String productName;

    @SerializedName(value = "productImage", alternate = {"ProductImage", "image", "Image"})
    private String productImage;

    @SerializedName(value = "barcode", alternate = {"Barcode", "sku", "Sku"})
    private String barcode;

    @SerializedName(value = "batchCode", alternate = {"BatchCode"})
    private String batchCode;

    @SerializedName(value = "unit", alternate = {"Unit"})
    private String unit;

    @SerializedName(value = "warehouseId", alternate = {"WarehouseId"})
    private int warehouseId;

    @SerializedName(value = "warehouseName", alternate = {"WarehouseName"})
    private String warehouseName;

    @SerializedName(value = "branchName", alternate = {"BranchName"})
    private String branchName;

    @SerializedName(value = "quantity", alternate = {"Quantity", "stock", "Stock"})
    private int quantity;

    @SerializedName(value = "price", alternate = {"Price"})
    private Double price;

    @SerializedName(value = "lastUpdate", alternate = {"LastUpdate"})
    private String lastUpdate;

    public InventoryItem() {
    }

    public int getInventoryId() {
        return inventoryId;
    }

    public void setInventoryId(int inventoryId) {
        this.inventoryId = inventoryId;
    }

    public int getLotId() {
        return lotId;
    }

    public void setLotId(int lotId) {
        this.lotId = lotId;
    }

    public int getProductId() {
        return productId;
    }

    public void setProductId(int productId) {
        this.productId = productId;
    }

    public String getProductName() {
        return productName != null && !productName.trim().isEmpty() ? productName : "Sản phẩm";
    }

    public void setProductName(String productName) {
        this.productName = productName;
    }

    public String getProductImage() {
        return productImage;
    }

    public void setProductImage(String productImage) {
        this.productImage = productImage;
    }

    public String getBarcode() {
        return barcode;
    }

    public void setBarcode(String barcode) {
        this.barcode = barcode;
    }

    public String getBatchCode() {
        return batchCode;
    }

    public void setBatchCode(String batchCode) {
        this.batchCode = batchCode;
    }

    public String getUnit() {
        return unit;
    }

    public void setUnit(String unit) {
        this.unit = unit;
    }

    public int getWarehouseId() {
        return warehouseId;
    }

    public void setWarehouseId(int warehouseId) {
        this.warehouseId = warehouseId;
    }

    public String getWarehouseName() {
        return warehouseName;
    }

    public void setWarehouseName(String warehouseName) {
        this.warehouseName = warehouseName;
    }

    public String getBranchName() {
        return branchName;
    }

    public void setBranchName(String branchName) {
        this.branchName = branchName;
    }

    public int getQuantity() {
        return quantity;
    }

    public void setQuantity(int quantity) {
        this.quantity = quantity;
    }

    public Double getPrice() {
        return price;
    }

    public void setPrice(Double price) {
        this.price = price;
    }

    public String getLastUpdate() {
        return lastUpdate;
    }

    public void setLastUpdate(String lastUpdate) {
        this.lastUpdate = lastUpdate;
    }

    public double getTotalValuation() {
        double unitPrice = price != null ? price : 0.0;
        return quantity * unitPrice;
    }
}
