package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class StockMovement {
    @SerializedName(value = "movementId", alternate = {"MovementId", "id", "Id"})
    private int movementId;

    @SerializedName(value = "productId", alternate = {"ProductId"})
    private int productId;

    @SerializedName(value = "productName", alternate = {"ProductName", "product", "Product"})
    private String productName;

    @SerializedName(value = "productImage", alternate = {"ProductImage", "image", "Image"})
    private String productImage;

    @SerializedName(value = "barcode", alternate = {"Barcode"})
    private String barcode;

    @SerializedName(value = "warehouseId", alternate = {"WarehouseId"})
    private int warehouseId;

    @SerializedName(value = "warehouseName", alternate = {"WarehouseName"})
    private String warehouseName;

    @SerializedName(value = "movementType", alternate = {"MovementType", "type", "Type"})
    private String type;

    @SerializedName(value = "quantity", alternate = {"Quantity", "qty", "Qty"})
    private int quantity;

    @SerializedName(value = "movementDate", alternate = {"MovementDate", "createdDate", "CreatedDate", "createdAt", "CreatedAt", "time", "Time"})
    private String createdDate;

    @SerializedName(value = "reason", alternate = {"Reason"})
    private String reason;

    @SerializedName(value = "userName", alternate = {"UserName", "user", "User"})
    private String userName;

    public int getMovementId() { return movementId; }
    public int getProductId() { return productId; }
    public String getProductName() { return productName; }
    public String getProductImage() { return productImage; }
    public String getBarcode() { return barcode; }
    public int getWarehouseId() { return warehouseId; }
    public String getWarehouseName() { return warehouseName; }
    public String getType() { return type; }
    public int getQuantity() { return quantity; }
    public String getCreatedDate() { return createdDate; }
    public String getReason() { return reason; }
    public String getUserName() { return userName; }
}
