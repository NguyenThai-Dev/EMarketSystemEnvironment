package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class StockAdjustmentRequest {
    @SerializedName(value = "productId", alternate = {"ProductId"})
    private int productId;

    @SerializedName(value = "warehouseId", alternate = {"WarehouseId"})
    private int warehouseId;

    @SerializedName(value = "quantityChange", alternate = {"QuantityChange", "adjustedQuantity", "AdjustedQuantity", "quantity", "Quantity"})
    private int adjustedQuantity;

    @SerializedName(value = "movementType", alternate = {"MovementType"})
    private String movementType;

    @SerializedName(value = "reason", alternate = {"Reason"})
    private String reason;

    @SerializedName(value = "userId", alternate = {"UserId"})
    private int userId;

    public StockAdjustmentRequest(int productId, int warehouseId, int adjustedQuantity, String reason) {
        this.productId = productId;
        this.warehouseId = warehouseId;
        this.adjustedQuantity = adjustedQuantity;
        this.movementType = "ADJUSTMENT";
        this.reason = reason;
        this.userId = 1;
    }

    public int getProductId() { return productId; }
    public int getWarehouseId() { return warehouseId; }
    public int getAdjustedQuantity() { return adjustedQuantity; }
    public String getMovementType() { return movementType; }
    public String getReason() { return reason; }
    public int getUserId() { return userId; }
}
