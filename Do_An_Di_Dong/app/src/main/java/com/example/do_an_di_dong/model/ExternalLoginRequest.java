package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class ExternalLoginRequest {
    @SerializedName(value = "provider", alternate = {"Provider"})
    private String provider = "Google";

    @SerializedName(value = "email", alternate = {"Email"})
    private String email;

    @SerializedName(value = "idToken", alternate = {"IdToken"})
    private String idToken;

    @SerializedName(value = "fullName", alternate = {"FullName", "name", "Name"})
    private String fullName;

    public ExternalLoginRequest() {
    }

    public ExternalLoginRequest(String provider, String email) {
        this.provider = provider;
        this.email = email;
    }

    public ExternalLoginRequest(String provider, String email, String idToken, String fullName) {
        this.provider = provider;
        this.email = email;
        this.idToken = idToken;
        this.fullName = fullName;
    }

    public String getProvider() {
        return provider;
    }

    public void setProvider(String provider) {
        this.provider = provider;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getIdToken() {
        return idToken;
    }

    public void setIdToken(String idToken) {
        this.idToken = idToken;
    }

    public String getFullName() {
        return fullName;
    }

    public void setFullName(String fullName) {
        this.fullName = fullName;
    }
}
