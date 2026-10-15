package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class ProductPagedResponse {
    @SerializedName(value = "total", alternate = {"recordsTotal", "Total"})
    private int total;

    @SerializedName(value = "filtered", alternate = {"recordsFiltered", "Filtered"})
    private int filtered;

    @SerializedName(value = "data", alternate = {"Data", "products", "Products"})
    private List<Product> data;

    public int getTotal() { return total; }
    public void setTotal(int total) { this.total = total; }

    public int getFiltered() { return filtered; }
    public void setFiltered(int filtered) { this.filtered = filtered; }

    public List<Product> getData() { return data; }
    public void setData(List<Product> data) { this.data = data; }
}
