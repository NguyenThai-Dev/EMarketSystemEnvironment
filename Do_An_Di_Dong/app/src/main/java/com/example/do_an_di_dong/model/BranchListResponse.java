package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.ArrayList;
import java.util.List;

public class BranchListResponse {
    @SerializedName(value = "success", alternate = {"Success"})
    private boolean success;

    @SerializedName(value = "data", alternate = {"Data", "branches", "Branches"})
    private List<Branch> data;

    public boolean isSuccess() { return success; }
    public void setSuccess(boolean success) { this.success = success; }

    public List<Branch> getData() {
        return data != null ? data : new ArrayList<>();
    }
    public void setData(List<Branch> data) { this.data = data; }
}
