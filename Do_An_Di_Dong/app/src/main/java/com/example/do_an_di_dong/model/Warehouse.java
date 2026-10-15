package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Warehouse {
    @SerializedName(value = "warehouseId", alternate = {"WarehouseId", "id", "Id"})
    private int warehouseId;

    @SerializedName(value = "name", alternate = {"Name", "warehouseName", "WarehouseName"})
    private String name;

    @SerializedName(value = "location", alternate = {"Location", "address", "Address"})
    private String location;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private int branchId;

    public int getWarehouseId() { return warehouseId; }
    public String getName() { return name; }
    public String getLocation() { return location; }
    public int getBranchId() { return branchId; }
}
