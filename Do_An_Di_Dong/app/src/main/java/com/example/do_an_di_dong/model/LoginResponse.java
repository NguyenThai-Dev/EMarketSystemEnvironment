package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class LoginResponse {
    @SerializedName(value = "success", alternate = {"Success", "status", "Status"})
    private boolean success;

    @SerializedName(value = "message", alternate = {"Message", "msg", "Msg"})
    private String message;

    @SerializedName(value = "token", alternate = {"Token", "accessToken", "AccessToken"})
    private String token;

    @SerializedName(value = "user", alternate = {"User", "currentUser", "CurrentUser", "data", "Data"})
    private User user;

    public boolean isSuccess() { return success || (user != null); }
    public String getMessage() { return message; }
    public String getToken() { return token; }
    public User getUser() { return user; }
}
