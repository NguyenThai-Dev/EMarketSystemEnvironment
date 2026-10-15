package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ExpenseCategory {
    @SerializedName(value = "categoryId", alternate = {"CategoryId", "id", "Id"})
    private int categoryId;

    @SerializedName(value = "name", alternate = {"Name", "categoryName", "CategoryName"})
    private String name;

    @SerializedName(value = "description", alternate = {"Description"})
    private String description;

    @SerializedName(value = "isActive", alternate = {"IsActive"})
    private Boolean isActive;

    public int getCategoryId() { return categoryId; }
    public String getName() { return name; }
    public String getDescription() { return description; }
    public Boolean getIsActive() { return isActive; }
}
