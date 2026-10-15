package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class LoginRequest {
    @SerializedName(value = "EmailOrUsername", alternate = {"emailOrUsername", "username", "Username", "userName", "UserName", "email", "Email"})
    private String username;

    @SerializedName(value = "Password", alternate = {"password"})
    private String password;

    public LoginRequest(String username, String password) {
        this.username = username;
        this.password = password;
    }

    public String getUsername() { return username; }
    public String getPassword() { return password; }
}
