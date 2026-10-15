package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ProductCategoryDTO {
    @SerializedName(value = "categoryId", alternate = {"CategoryId", "id", "Id"})
    private int categoryId;

    @SerializedName(value = "name", alternate = {"Name", "categoryName", "CategoryName"})
    private String name;

    @SerializedName(value = "description", alternate = {"Description"})
    private String description;

    @SerializedName(value = "canBeDeleted", alternate = {"CanBeDeleted"})
    private boolean canBeDeleted;

    public int getCategoryId() { return categoryId; }
    public void setCategoryId(int categoryId) { this.categoryId = categoryId; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public boolean isCanBeDeleted() { return canBeDeleted; }
    public void setCanBeDeleted(boolean canBeDeleted) { this.canBeDeleted = canBeDeleted; }
}
