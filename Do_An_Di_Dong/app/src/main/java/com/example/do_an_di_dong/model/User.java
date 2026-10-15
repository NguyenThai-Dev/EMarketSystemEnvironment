package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;
import java.util.List;

public class User {
    @SerializedName(value = "userId", alternate = {"UserId", "id", "Id"})
    private int userId;

    @SerializedName(value = "username", alternate = {"Username", "userName", "UserName"})
    private String username;

    @SerializedName(value = "fullName", alternate = {"FullName", "name", "Name"})
    private String fullName;

    @SerializedName(value = "email", alternate = {"Email"})
    private String email;

    @SerializedName(value = "phone", alternate = {"Phone"})
    private String phone;

    @SerializedName(value = "image", alternate = {"Image", "userImg", "UserImg", "avatar", "Avatar"})
    private String image;

    @SerializedName(value = "status", alternate = {"Status"})
    private String status;

    @SerializedName(value = "branchId", alternate = {"BranchId"})
    private Integer branchId;

    @SerializedName(value = "supplierId", alternate = {"SupplierId"})
    private Integer supplierId;

    @SerializedName(value = "roleId", alternate = {"RoleId"})
    private int roleId;

    @SerializedName(value = "roles", alternate = {"Roles"})
    private List<Role> roles;

    @SerializedName(value = "permissions", alternate = {"Permissions"})
    private List<Permission> permissions;

    @SerializedName(value = "isAdmin", alternate = {"IsAdmin"})
    private boolean isAdmin;

    @SerializedName(value = "isSupplier", alternate = {"IsSupplier"})
    private boolean isSupplier;

    public int getUserId() { return userId; }
    public String getUsername() { return username; }
    public String getFullName() { return (fullName != null && !fullName.isEmpty()) ? fullName : username; }
    public String getEmail() { return email; }
    public String getPhone() { return phone; }
    public String getImage() { return image; }
    public String getStatus() { return status; }
    public Integer getBranchId() { return branchId; }
    public Integer getSupplierId() { return supplierId; }
    public int getRoleId() { return roleId; }
    public List<Role> getRoles() { return roles; }
    public List<Permission> getPermissions() { return permissions; }
    public boolean isAdmin() { return isAdmin; }
    public boolean isSupplier() { return isSupplier; }

    public String getPrimaryRoleName() {
        if (roles != null && !roles.isEmpty() && roles.get(0) != null) {
            return roles.get(0).getName();
        }
        return isAdmin ? "Quản trị viên" : "Nhân viên";
    }

    public boolean hasPermission(String moduleOrPermission) {
        if (isAdmin) return true;
        if (moduleOrPermission == null || moduleOrPermission.trim().isEmpty()) return true;
        if (permissions == null || permissions.isEmpty()) return false;

        String target = moduleOrPermission.trim().toLowerCase();
        for (Permission p : permissions) {
            if (p == null) continue;
            if (p.getModule() != null && p.getModule().toLowerCase().contains(target)) return true;
            if (p.getName() != null && p.getName().toLowerCase().contains(target)) return true;
        }
        return false;
    }

    public boolean hasAnyPermission(String... modulesOrPermissions) {
        if (isAdmin) return true;
        if (modulesOrPermissions == null || modulesOrPermissions.length == 0) return true;
        for (String perm : modulesOrPermissions) {
            if (hasPermission(perm)) return true;
        }
        return false;
    }
}
