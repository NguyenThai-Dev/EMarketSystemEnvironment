package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class CustomerAddress {
    @SerializedName(value = "addressId", alternate = {"AddressId", "id", "Id"})
    private int addressId;

    @SerializedName(value = "customerId", alternate = {"CustomerId"})
    private int customerId;

    @SerializedName(value = "recipientName", alternate = {"RecipientName"})
    private String recipientName;

    @SerializedName(value = "recipientPhone", alternate = {"RecipientPhone"})
    private String recipientPhone;

    @SerializedName(value = "address", alternate = {"Address", "addressLine", "AddressLine"})
    private String address;

    @SerializedName(value = "isDefault", alternate = {"IsDefault"})
    private boolean isDefault;

    public int getAddressId() { return addressId; }
    public int getCustomerId() { return customerId; }
    public String getRecipientName() { return recipientName; }
    public String getRecipientPhone() { return recipientPhone; }
    public String getAddress() { return address; }
    public boolean isDefault() { return isDefault; }
}
