package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ProductLot {
    @SerializedName(value = "lotId", alternate = {"LotId"})
    private int lotId;

    @SerializedName(value = "productId", alternate = {"ProductId"})
    private int productId;

    @SerializedName(value = "batchCode", alternate = {"BatchCode"})
    private String batchCode;

    @SerializedName(value = "costPrice", alternate = {"CostPrice"})
    private double costPrice;

    @SerializedName(value = "manufacturingDate", alternate = {"ManufacturingDate"})
    private String manufacturingDate;

    @SerializedName(value = "expiryDate", alternate = {"ExpiryDate"})
    private String expiryDate;

    public int getLotId() { return lotId; }
    public void setLotId(int lotId) { this.lotId = lotId; }

    public int getProductId() { return productId; }
    public void setProductId(int productId) { this.productId = productId; }

    public String getBatchCode() { return batchCode != null ? batchCode : ("LOT-" + lotId); }
    public void setBatchCode(String batchCode) { this.batchCode = batchCode; }

    public double getCostPrice() { return costPrice; }
    public void setCostPrice(double costPrice) { this.costPrice = costPrice; }

    public String getManufacturingDate() { return manufacturingDate; }
    public void setManufacturingDate(String manufacturingDate) { this.manufacturingDate = manufacturingDate; }

    public String getExpiryDate() { return expiryDate; }
    public void setExpiryDate(String expiryDate) { this.expiryDate = expiryDate; }
}
