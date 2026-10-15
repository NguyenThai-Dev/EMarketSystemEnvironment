package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class RolePermissionUpdateRequest {
    @SerializedName(value = "roleId", alternate = {"RoleId"})
    private int roleId;

    @SerializedName(value = "permissionIds", alternate = {"PermissionIds"})
    private List<Integer> permissionIds;

    public RolePermissionUpdateRequest(int roleId, List<Integer> permissionIds) {
        this.roleId = roleId;
        this.permissionIds = permissionIds;
    }

    public int getRoleId() { return roleId; }
    public List<Integer> getPermissionIds() { return permissionIds; }
}
