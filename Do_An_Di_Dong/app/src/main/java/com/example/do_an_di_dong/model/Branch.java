package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Branch {
    @SerializedName(value = "branchId", alternate = {"BranchId", "id", "Id"})
    private int branchId;

    @SerializedName(value = "name", alternate = {"Name", "branchName", "BranchName"})
    private String name;

    @SerializedName(value = "address", alternate = {"Address"})
    private String address;

    @SerializedName(value = "addressUrl", alternate = {"AddressUrl"})
    private String addressUrl;

    @SerializedName(value = "latitude", alternate = {"Latitude"})
    private Double latitude;

    @SerializedName(value = "longitude", alternate = {"Longitude"})
    private Double longitude;

    public int getBranchId() { return branchId; }
    public String getName() { return name; }
    public String getAddress() { return address; }
    public String getAddressUrl() { return addressUrl; }
    public Double getLatitude() { return latitude; }
    public Double getLongitude() { return longitude; }
}
