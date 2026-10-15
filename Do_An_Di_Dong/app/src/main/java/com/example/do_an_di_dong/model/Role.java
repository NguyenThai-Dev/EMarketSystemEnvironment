package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class Role {
    @SerializedName(value = "roleId", alternate = {"RoleId", "id", "Id"})
    private int roleId;

    @SerializedName(value = "name", alternate = {"Name", "roleName", "RoleName"})
    private String name;

    @SerializedName(value = "permissions", alternate = {"Permissions"})
    private List<Permission> permissions;

    public int getRoleId() { return roleId; }
    public String getName() { return name; }
    public List<Permission> getPermissions() { return permissions; }
}
