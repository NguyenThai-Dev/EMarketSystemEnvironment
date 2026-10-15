# API Manual: Customers, Addresses & Loyalty Programs

> **Module Base Path:** `/api/admin/customer`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. Customers (Quản lý Khách Hàng)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/customer` | Không | Lấy toàn bộ danh sách khách hàng |
| `GET` | `/api/admin/customer/search`<br>`/api/admin/customer/paged` | `start=0&length=10&keyword=` | Tìm kiếm & phân trang khách hàng |
| `GET` | `/api/admin/customer/{id}` | Path: `id` | Lấy chi tiết thông tin một khách hàng |
| `GET` | `/api/admin/customer/{id}/email` | Path: `id` | Lấy email của khách hàng |
| `POST` | `/api/admin/customer/create` | `CustomerCreateDTO` | Tạo mới khách hàng |
| `POST` / `PUT` | `/api/admin/customer/update`<br>`/api/admin/customer/{id}` | `CustomerUpdateDTO` | Cập nhật thông tin khách hàng |
| `DELETE` / `POST` | `/api/admin/customer/{id}`<br>`/api/admin/customer/delete/{id}` | Path: `id` | Xóa khách hàng |

### Thống Kê & Phân Khúc Khách Hàng (Analytics & CRM)

| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/customer/stats` | `fromDate=` | Thống kê số lượng: Tổng khách, Khách VIP, Khách mới |
| `GET` | `/api/admin/customer/count-in-month` | `fromDate=&toDate=` | Đếm số khách hàng mới tạo trong khoảng thời gian |
| `GET` | `/api/admin/customer/segments` | Không | Lấy phân khúc khách hàng (VIP, Thường, Mới) |
| `GET` | `/api/admin/customer/created-by-month` | Không | Biểu đồ tăng trưởng khách hàng theo từng tháng |
| `GET` | `/api/admin/customer/top` | `top=10` | Top khách hàng có tổng chi tiêu cao nhất |

```json
// CustomerCreateDTO
{
  "FullName": "Trần Thị B",
  "Phone": "0901234567",
  "Email": "tranthib@gmail.com",
  "CustomerType": "VIP", // "Standard", "VIP"
  "Points": 100,
  "Notes": "Khách hàng thân thiết khu vực Quận 1"
}

// Phản hồi Customer Detail
{
  "CustomerId": 5,
  "FullName": "Trần Thị B",
  "Phone": "0901234567",
  "Email": "tranthib@gmail.com",
  "CustomerType": "VIP",
  "Points": 150,
  "TotalSpent": 4500000,
  "OrderCount": 12,
  "CreatedAt": "2026-01-15T08:30:00"
}
```

---

## 2. Customer Addresses (Sổ Địa Chỉ Giao Hàng)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/customer/address/by-customer/{customerId}` | Path: `customerId` | Lấy danh sách tất cả địa chỉ của khách hàng |
| `GET` | `/api/admin/customer/address/default/{customerId}` | Path: `customerId` | Lấy địa chỉ mặc định của khách hàng |
| `GET` | `/api/admin/customer/address/{id}` | Path: `id` | Lấy chi tiết một địa chỉ |
| `POST` | `/api/admin/customer/address/create` | `CustomerAddressCreateDTO` | Thêm địa chỉ mới |
| `POST` / `PUT` | `/api/admin/customer/address/update`<br>`/api/admin/customer/address/{id}` | `CustomerAddressUpdateDTO` | Cập nhật địa chỉ |
| `DELETE` / `POST` | `/api/admin/customer/address/{id}`<br>`/api/admin/customer/address/delete/{id}` | Path: `id` | Xóa địa chỉ |

```json
// CustomerAddressCreateDTO
{
  "CustomerId": 5,
  "RecipientName": "Trần Thị B",
  "RecipientPhone": "0901234567",
  "AddressLine": "Số 45 Đường Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP.HCM",
  "IsDefault": true
}
```

---

## 3. Loyalty Programs (Chương Trình Tích Điểm Thành Viên)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/customer/loyalty` | Không | Lấy danh sách toàn bộ chương trình tích điểm |
| `GET` | `/api/admin/customer/loyalty/{id}` | Path: `id` | Lấy chi tiết chương trình tích điểm |
| `POST` | `/api/admin/customer/loyalty/create` | `LoyaltyProgramDTO` | Tạo mới chương trình tích điểm |
| `POST` / `PUT` | `/api/admin/customer/loyalty/update`<br>`/api/admin/customer/loyalty/{id}` | `LoyaltyProgramDTO` | Cập nhật chương trình tích điểm |
| `DELETE` / `POST` | `/api/admin/customer/loyalty/{id}`<br>`/api/admin/customer/loyalty/delete/{id}` | Path: `id` | Xóa chương trình tích điểm |

```json
// LoyaltyProgramDTO
{
  "LoyaltyId": 1,
  "Name": "Chương trình Tích điểm Mùa Vàng",
  "MinOrderValue": 100000,
  "PointsMultiplier": 1.0,
  "ExchangeRate": 1000,    // 1 điểm = 1.000 VNĐ khi thanh toán
  "StartDate": "2026-01-01T00:00:00",
  "EndDate": "2026-12-31T23:59:59",
  "IsActive": true
}
```

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **POS Quick Customer Lookup:** Trong màn hình POS Checkout, ô tìm kiếm khách hàng bằng số điện thoại gọi `/api/admin/customer/search?keyword={phone}`:
   - Hiển thị ngay số điểm tích lũy hiện tại (`Points`).
   - Tự động gợi ý checkbox "Đổi điểm giảm giá" nếu điểm > 0 dựa trên tỉ lệ quy đổi điểm.
2. **Quản lý Thẻ Thành Viên:** Hiển thị thẻ VIP/Standard ảo trên giao diện profile với mã Barcode/QRCode chứa `CustomerId` để quét thẻ nhanh tại quầy thu ngân.
