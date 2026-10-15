package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class Permission {
    @SerializedName(value = "permissionId", alternate = {"PermissionId", "id", "Id"})
    private int permissionId;

    @SerializedName(value = "name", alternate = {"Name", "permissionName", "PermissionName"})
    private String name;

    @SerializedName(value = "module", alternate = {"Module"})
    private String module;

    public int getPermissionId() { return permissionId; }
    public String getName() { return name; }
    public String getModule() { return module; }
}
