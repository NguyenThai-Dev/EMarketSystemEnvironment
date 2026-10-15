package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class SupplierDTO {
    @SerializedName(value = "supplierId", alternate = {"SupplierId", "id", "Id"})
    private int supplierId;

    @SerializedName(value = "name", alternate = {"Name", "supplierName", "SupplierName"})
    private String name;

    @SerializedName(value = "email", alternate = {"Email"})
    private String email;

    @SerializedName(value = "phone", alternate = {"Phone"})
    private String phone;

    @SerializedName(value = "address", alternate = {"Address"})
    private String address;

    @SerializedName(value = "addressUrl", alternate = {"AddressUrl"})
    private String addressUrl;

    @SerializedName(value = "contactPerson", alternate = {"ContactPerson"})
    private String contactPerson;

    @SerializedName(value = "canBeDeleted", alternate = {"CanBeDeleted"})
    private boolean canBeDeleted;

    public SupplierDTO() {}

    public SupplierDTO(int supplierId, String name, String email, String phone, String address, String contactPerson) {
        this.supplierId = supplierId;
        this.name = name;
        this.email = email;
        this.phone = phone;
        this.address = address;
        this.contactPerson = contactPerson;
    }

    public int getSupplierId() { return supplierId; }
    public void setSupplierId(int supplierId) { this.supplierId = supplierId; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public String getPhone() { return phone; }
    public void setPhone(String phone) { this.phone = phone; }

    public String getAddress() { return address; }
    public void setAddress(String address) { this.address = address; }

    public String getAddressUrl() { return addressUrl; }
    public void setAddressUrl(String addressUrl) { this.addressUrl = addressUrl; }

    public String getContactPerson() { return contactPerson; }
    public void setContactPerson(String contactPerson) { this.contactPerson = contactPerson; }

    public boolean isCanBeDeleted() { return canBeDeleted; }
    public void setCanBeDeleted(boolean canBeDeleted) { this.canBeDeleted = canBeDeleted; }
}
