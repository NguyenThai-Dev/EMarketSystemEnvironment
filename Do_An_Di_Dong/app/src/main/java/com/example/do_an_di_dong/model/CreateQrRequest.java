package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class CreateQrRequest {
    @SerializedName(value = "amount", alternate = {"Amount"})
    private int amount;

    @SerializedName(value = "description", alternate = {"Description"})
    private String description;

    public CreateQrRequest() {}

    public CreateQrRequest(int amount, String description) {
        this.amount = amount;
        this.description = description;
    }

    public int getAmount() { return amount; }
    public void setAmount(int amount) { this.amount = amount; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }
}
