package com.example.do_an_di_dong.model;

import com.google.gson.annotations.SerializedName;

public class AuditLog {
    @SerializedName(value = "auditId", alternate = {"AuditId", "id", "Id"})
    private long auditId;

    @SerializedName(value = "userId", alternate = {"UserId"})
    private Integer userId;

    @SerializedName(value = "username", alternate = {"Username", "userName", "UserName"})
    private String username;

    @SerializedName(value = "tableName", alternate = {"TableName"})
    private String tableName;

    @SerializedName(value = "primaryKeyId", alternate = {"PrimaryKeyId"})
    private String primaryKeyId;

    @SerializedName(value = "actionType", alternate = {"ActionType", "action", "Action"})
    private String actionType;

    @SerializedName(value = "oldValues", alternate = {"OldValues"})
    private String oldValues;

    @SerializedName(value = "newValues", alternate = {"NewValues"})
    private String newValues;

    @SerializedName(value = "ipAddress", alternate = {"IpAddress"})
    private String ipAddress;

    @SerializedName(value = "createdAt", alternate = {"CreatedAt", "timestamp", "Timestamp"})
    private String createdAt;

    @SerializedName(value = "userImgUrl", alternate = {"UserImgUrl", "image", "Image"})
    private String userImgUrl;

    public long getAuditId() { return auditId; }
    public Integer getUserId() { return userId; }
    public String getUsername() { return username; }
    public String getTableName() { return tableName; }
    public String getPrimaryKeyId() { return primaryKeyId; }
    public String getActionType() { return actionType; }
    public String getOldValues() { return oldValues; }
    public String getNewValues() { return newValues; }
    public String getIpAddress() { return ipAddress; }
    public String getCreatedAt() { return createdAt; }
    public String getUserImgUrl() { return userImgUrl; }

    public String getFormattedAction() {
        if (actionType == null) return "Thao tác";
        switch (actionType.trim()) {
            case "Added":
            case "Create":
            case "INSERT":
                return "Thêm mới";
            case "Modified":
            case "Update":
            case "UPDATE":
                return "Cập nhật";
            case "Deleted":
            case "Delete":
            case "DELETE":
                return "Đã xóa";
            default:
                return actionType;
        }
    }

    public String getFormattedTable() {
        if (tableName == null) return "Bảng dữ liệu";
        switch (tableName.trim()) {
            case "Products":
            case "Product":
                return "Sản phẩm";
            case "Users":
            case "User":
                return "Người dùng";
            case "Suppliers":
            case "Supplier":
                return "Nhà cung cấp";
            case "Quotations":
            case "Quotation":
                return "Báo giá";
            case "Orders":
            case "Order":
                return "Đơn hàng";
            case "SystemConfigs":
                return "Cấu hình hệ thống";
            default:
                return tableName;
        }
    }
}
