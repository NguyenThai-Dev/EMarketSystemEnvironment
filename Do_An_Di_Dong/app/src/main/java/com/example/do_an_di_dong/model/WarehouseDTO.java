package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class WarehouseDTO {
    @SerializedName(value = "warehouseId", alternate = {"WarehouseId", "id", "Id"})
    private int warehouseId;

    @SerializedName(value = "name", alternate = {"Name", "warehouseName", "WarehouseName"})
    private String name;

    @SerializedName(value = "address", alternate = {"Address", "location", "Location"})
    private String address;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private int branchId;

    @SerializedName(value = "canBeDeleted", alternate = {"CanBeDeleted"})
    private boolean canBeDeleted;

    public int getWarehouseId() { return warehouseId; }
    public void setWarehouseId(int warehouseId) { this.warehouseId = warehouseId; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public String getAddress() { return address; }
    public void setAddress(String address) { this.address = address; }

    public int getBranchId() { return branchId; }
    public void setBranchId(int branchId) { this.branchId = branchId; }

    public boolean isCanBeDeleted() { return canBeDeleted; }
    public void setCanBeDeleted(boolean canBeDeleted) { this.canBeDeleted = canBeDeleted; }
}
