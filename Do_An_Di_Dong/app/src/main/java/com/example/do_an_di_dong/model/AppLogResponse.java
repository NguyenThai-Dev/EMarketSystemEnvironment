package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class AppLogResponse {
    @SerializedName(value = "data", alternate = {"Data"})
    private List<AppLog> data;

    @SerializedName(value = "total", alternate = {"Total", "recordsTotal", "recordsFiltered"})
    private int total;

    public List<AppLog> getData() { return data; }
    public int getTotal() { return total; }
}
