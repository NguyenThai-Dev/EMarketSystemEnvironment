---
trigger: always_on
---

# API Manual: Authentication, Users, Branches, Roles & Permissions

> **Module Base Path:** `/api/admin/user-management`  
> **Host Default:** `https://localhost:44339`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. Authentication (Xác thực & Đăng nhập)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `POST` | `/api/admin/user-management/auth/login` | `LoginRequestDTO` | Đăng nhập tài khoản mật khẩu, trả về JWT Token và thông tin User |
| `POST` | `/api/admin/user-management/auth/google-login`<br>`/api/admin/user-management/auth/external-login` | `ExternalLoginRequestDTO` | Đăng nhập qua tài khoản Google Firebase / OAuth |
| `POST` | `/api/admin/user-management/auth/request-otp` | `{ "Email": "string" }` | Gửi mã OTP xác thực về email người dùng |
| `POST` | `/api/admin/user-management/auth/reset-password` | `ResetPasswordRequestDTO` | Đặt lại mật khẩu mới bằng OTP |

### Cấu trúc DTOs Authentication

```json
// LoginRequestDTO
{
  "EmailOrUsername": "admin@emarket.com",
  "Password": "password123"
}

// Phản hồi Login Thành công
{
  "success": true,
  "message": "Đăng nhập thành công.",
  "token": "eyJhbGciOi...",
  "user": {
    "UserId": 1,
    "Username": "admin",
    "FullName": "Quản Trị Viên",
    "Email": "admin@emarket.com",
    "Phone": "0987654321",
    "Avatar": "/Uploads/avatars/admin.jpg",
    "BranchId": 1,
    "BranchName": "Chi nhánh Trung tâm",
    "PrimaryRoleId": 1,
    "RoleName": "Admin",
    "IsAdmin": true,
    "Permissions": ["ProductModule", "SalesModule", "InventoryModule", ...]
  }
}

// ResetPasswordRequestDTO
{
  "Email": "user@emarket.com",
  "Otp": "123456",
  "NewPassword": "NewSecurePassword123!"
}
```

---

## 2. User Management (Quản lý Người Dùng & Nhân Viên)

| Phương thức | Đường dẫn | Query / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/user-management/users` | Không | Lấy toàn bộ danh sách người dùng |
| `GET` | `/api/admin/user-management/users/paged`<br>`/api/admin/user-management/users/search` | `start=0&length=10&keyword=...` | Tìm kiếm & phân trang người dùng |
| `GET` | `/api/admin/user-management/users/{id}` | Path: `id` (int) | Lấy thông tin chi tiết một người dùng |
| `POST` | `/api/admin/user-management/users/by-ids` | Body: `[1, 2, 3]` | Lấy danh sách người dùng theo danh sách ID |
| `GET` | `/api/admin/user-management/users/dict` | Không | Lấy map ID -> FullName (Key-Value) cho dropdown |
| `GET` | `/api/admin/user-management/users/stats` | Không | Lấy số liệu thống kê nhân sự (Active, Roles) |
| `GET` | `/api/admin/user-management/users/count-new` | `days=30` | Đếm số nhân sự mới tạo trong N ngày qua |
| `GET` | `/api/admin/user-management/users/recent-avatars` | `top=5` | Lấy danh sách avatar nhân viên hoạt động gần đây |
| `GET` | `/api/admin/user-management/users/warehouse-managers-emails` | Không | Lấy email các quản lý kho (phục vụ thông báo) |
| `POST` | `/api/admin/user-management/users/create` | `CurrentUserDTO` | Tạo mới người dùng |
| `POST` / `PUT` | `/api/admin/user-management/users/update`<br>`/api/admin/user-management/users/{id}` | `CurrentUserDTO` | Cập nhật thông tin người dùng |
| `DELETE` / `POST` | `/api/admin/user-management/users/{id}`<br>`/api/admin/user-management/users/delete/{id}` | Path: `id` | Xóa người dùng |
| `POST` | `/api/admin/user-management/users/update-email` | `{ "UserId": 1, "NewEmail": "..." }` | Cập nhật trực tiếp email (Admin) |
| `POST` | `/api/admin/user-management/users/verify-change-email` | `VerifyChangeEmailModel` | Xác thực OTP và cập nhật email cho cá nhân |

### Cấu trúc DTO Người Dùng (`CurrentUserDTO`)
```json
{
  "UserId": 1,
  "Username": "nv_kho01",
  "FullName": "Nguyễn Văn A",
  "Email": "nva@emarket.com",
  "Phone": "0912345678",
  "Avatar": "/Uploads/Users/nva.png",
  "BranchId": 1,
  "SupplierId": null,
  "RoleIds": [3],
  "Status": true
}
```

---

## 3. Branches (Chi Nhánh & Vị Trí)

| Phương thức | Đường dẫn | Query / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/user-management/branches` | Không | Lấy danh sách toàn bộ chi nhánh |
| `GET` | `/api/admin/user-management/branches/search` | `name=&lat=&lng=&maxDist=10` | Tìm kiếm chi nhánh theo tên hoặc tọa độ GPS |
| `GET` | `/api/admin/user-management/branches/nearest` | `lat=10.7769&lng=106.7009&maxDist=100` | Tìm chi nhánh gần nhất theo GPS của điện thoại |
| `GET` | `/api/admin/user-management/branches/{id}` | Path: `id` | Lấy chi tiết chi nhánh |
| `POST` | `/api/admin/user-management/branches/by-ids` | Body: `[1, 2]` | Lấy danh sách chi nhánh theo danh sách ID |
| `GET` | `/api/admin/user-management/branches/dict` | Không | Lấy map ID -> Tên chi nhánh cho dropdown |
| `POST` | `/api/admin/user-management/branches/create` | `BranchDTO` | Tạo mới chi nhánh |
| `POST` / `PUT` | `/api/admin/user-management/branches/update`<br>`/api/admin/user-management/branches/{id}` | `BranchDTO` | Cập nhật chi nhánh |
| `DELETE` / `POST` | `/api/admin/user-management/branches/{id}`<br>`/api/admin/user-management/branches/delete/{id}` | Path: `id` | Xóa chi nhánh |

### Cấu trúc DTO Chi Nhánh (`BranchDTO`)
```json
{
  "BranchId": 1,
  "Name": "EMarket Quận 1",
  "Address": "123 Lê Lợi, Phường Bến Thành, Quận 1, TP.HCM",
  "AddressUrl": "https://maps.google.com/?q=...",
  "Latitude": 10.776889,
  "Longitude": 106.700806,
  "Distance": 2.5
}
```

---

## 4. Roles & Permissions (Vai Trò & Quyền Hạn)

| Phương thức | Đường dẫn | Query / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/user-management/roles` | Không | Lấy danh sách tất cả các vai trò |
| `GET` | `/api/admin/user-management/roles/{id}` | Path: `id` | Lấy thông tin một vai trò |
| `GET` | `/api/admin/user-management/roles/{id}/permissions` | Path: `id` | Lấy danh sách ID quyền hạn của vai trò |
| `POST` | `/api/admin/user-management/roles/create` | `RoleDTO` | Tạo vai trò mới |
| `POST` | `/api/admin/user-management/roles/update-permissions` | `RolePermissionUpdateDTO` | Cập nhật danh sách quyền cho vai trò |
| `DELETE` / `POST` | `/api/admin/user-management/roles/{id}`<br>`/api/admin/user-management/roles/delete/{id}` | Path: `id` | Xóa vai trò |
| `GET` | `/api/admin/user-management/permissions` | Không | Lấy danh sách tất cả các quyền hệ thống |
| `GET` | `/api/admin/user-management/permissions/{id}` | Path: `id` | Lấy chi tiết quyền hạn |

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **Lưu trữ Token:** Lưu trữ JWT Token an toàn vào `flutter_secure_storage` hoặc Android `EncryptedSharedPreferences`.
2. **Dio Interceptor:** Thiết lập `InterceptorsWrapper` tự động đính kèm `Authorization: Bearer <token>` vào mọi request. Khi gặp mã lỗi `401 Unauthorized`, tự động xóa token và chuyển màn hình về `LoginScreen`.
3. **Phân quyền UI:** Đọc mảng `user.Permissions` sau khi login thành công để ẩn/hiện các tab chức năng trên Bottom Navigation Bar (ví dụ: chỉ hiện Quản lý Kho nếu có `InventoryModule`).
