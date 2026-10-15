package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.ArrayList;
import java.util.List;

public class WarehouseListResponse {
    @SerializedName(value = "success", alternate = {"Success"})
    private boolean success;

    @SerializedName(value = "warehouses", alternate = {"Warehouses", "data", "Data"})
    private List<Warehouse> warehouses;

    public boolean isSuccess() { return success; }
    public void setSuccess(boolean success) { this.success = success; }

    public List<Warehouse> getWarehouses() {
        return warehouses != null ? warehouses : new ArrayList<>();
    }
    public void setWarehouses(List<Warehouse> warehouses) { this.warehouses = warehouses; }
}
