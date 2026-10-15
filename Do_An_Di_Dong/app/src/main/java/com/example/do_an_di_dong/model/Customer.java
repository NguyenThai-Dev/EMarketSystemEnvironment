package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Customer {
    @SerializedName(value = "customerId", alternate = {"CustomerId", "id", "Id"})
    private int customerId;

    @SerializedName(value = "fullName", alternate = {"FullName", "customerName", "CustomerName", "name", "Name"})
    private String customerName;

    @SerializedName(value = "email", alternate = {"Email"})
    private String email;

    @SerializedName(value = "phone", alternate = {"Phone"})
    private String phone;

    @SerializedName(value = "customerType", alternate = {"CustomerType"})
    private String customerType; // VIP, Normal, New

    @SerializedName(value = "pointBalance", alternate = {"PointBalance"})
    private int pointBalance;

    @SerializedName(value = "userImg", alternate = {"UserImg", "image", "Image"})
    private String userImg;

    public Customer() {}

    public Customer(String customerName, String phone, String email) {
        this.customerName = customerName;
        this.phone = phone;
        this.email = email;
        this.customerType = "Normal";
    }

    public int getCustomerId() { return customerId; }
    public void setCustomerId(int customerId) { this.customerId = customerId; }

    public String getCustomerName() { return (customerName != null) ? customerName : "Khách hàng #" + customerId; }
    public void setCustomerName(String customerName) { this.customerName = customerName; }

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public String getPhone() { return phone; }
    public void setPhone(String phone) { this.phone = phone; }

    public String getCustomerType() { return customerType; }
    public void setCustomerType(String customerType) { this.customerType = customerType; }

    public int getPointBalance() { return pointBalance; }
    public void setPointBalance(int pointBalance) { this.pointBalance = pointBalance; }

    public String getUserImg() { return userImg; }
    public void setUserImg(String userImg) { this.userImg = userImg; }

    public String getFullName() { return getCustomerName(); }
    public int getLoyaltyPoints() { return pointBalance; }
}
