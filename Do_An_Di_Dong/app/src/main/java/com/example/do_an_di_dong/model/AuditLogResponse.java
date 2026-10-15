package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class AuditLogResponse {
    @SerializedName(value = "data", alternate = {"Data"})
    private List<AuditLog> data;

    @SerializedName(value = "total", alternate = {"Total", "recordsTotal", "recordsFiltered"})
    private int total;

    public List<AuditLog> getData() { return data; }
    public int getTotal() { return total; }
}
