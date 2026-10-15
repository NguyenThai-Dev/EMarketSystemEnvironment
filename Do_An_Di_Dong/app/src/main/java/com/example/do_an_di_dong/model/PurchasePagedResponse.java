package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class PurchasePagedResponse {
    @SerializedName(value = "data", alternate = {"Data", "items", "Items"})
    private List<PurchaseOrder> data;

    @SerializedName(value = "total", alternate = {"Total", "recordsTotal"})
    private int total;

    @SerializedName(value = "filtered", alternate = {"Filtered", "recordsFiltered"})
    private int filtered;

    public List<PurchaseOrder> getData() { return data; }
    public int getTotal() { return total > 0 ? total : (filtered > 0 ? filtered : (data != null ? data.size() : 0)); }
    public int getFiltered() { return filtered > 0 ? filtered : total; }
}
