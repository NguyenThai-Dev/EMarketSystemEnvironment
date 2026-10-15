# API Manual: System Configuration, Audit Logs & System Events

> **Module Base Paths:**  
> - Cấu hình hệ thống: `/api/admin/system-config`  
> - Nhật ký & Sự kiện: `/api/admin/logs`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. System Configurations (Cấu Hình Hệ Thống Toàn Cục)

> **Base Route:** `/api/admin/system-config`

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/system-config` | Không | Lấy toàn bộ tham số cấu hình hệ thống (dạng đối tượng tổng hợp) |
| `GET` | `/api/admin/system-config/all` | Không | Lấy danh sách tất cả các cặp Key - Value cấu hình |
| `GET` | `/api/admin/system-config/value/{key}` | Path: `key` | Lấy giá trị của một cấu hình theo Key |
| `POST` | `/api/admin/system-config/update` | `List<ConfigUpdateDTO>` | Cập nhật danh sách các giá trị cấu hình hệ thống |

### Cấu trúc DTO Cấu Hình Hệ Thống (`SystemConfigViewModel`)
```json
{
  "VAT": 8.0,                       // Tỷ lệ thuế VAT mặc định (%)
  "BankID": "970422",               // Mã ngân hàng MBBank cho VietQR
  "BankNum": "0987654321",          // Số tài khoản ngân hàng thụ hưởng
  "VipDiscount": 10.0,              // Tỷ lệ giảm giá cho khách hàng VIP (%)
  "MemberDiscount": 5.0,            // Tỷ lệ giảm giá cho khách hàng thành viên (%)
  "PointExchangeRate": 1000.0,      // Giá trị quy đổi: 1 điểm = 1.000 VNĐ
  "PointEarnedRate": 10000.0,       // Tích lũy điểm: Cứ mỗi 10.000 VNĐ chi tiêu được 1 điểm
  "TelegramEnabled": true,          // Bật thông báo qua Telegram Bot
  "TelegramToken": "bot...",
  "TelegramChatId": "-100...",
  "EmailEnabled": true,
  "MailHost": "smtp.gmail.com",
  "MailDisplayName": "Hệ Thống Siêu Thị EMarket",
  "AppBaseUrl": "https://emarket.vn"
}

// Cập nhật cấu hình: POST /api/admin/system-config/update
[
  { "Key": "EMarketVAT", "Value": "8.0" },
  { "Key": "VIPDiscount", "Value": "10.0" }
]
```

---

## 2. Audit Logs (Nhật Ký Thao Tác Người Dùng & Dữ Liệu)

> **Base Route:** `/api/admin/logs`

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` / `POST` | `/api/admin/logs/audit` | `start=0&length=10&tableName=&action=&search=&fromDate=&toDate=` | Truy vấn danh sách Audit Log (Ai đã làm gì, vào thời điểm nào) |
| `GET` | `/api/admin/logs/audit/{id}` | Path: `id` (long) | Chi tiết một bản ghi Audit (kèm Diff dữ liệu cũ vs dữ liệu mới) |

```json
// Cấu trúc AuditLogDTO
{
  "AuditLogId": 1024,
  "UserId": 2,
  "UserName": "nv_kho01",
  "Action": "UPDATE", // "INSERT", "UPDATE", "DELETE"
  "TableName": "Products",
  "RecordId": "10",
  "OldValues": "{\"SalePrice\": 32000}",
  "NewValues": "{\"SalePrice\": 35000}",
  "Timestamp": "2026-10-07T09:15:20"
}
```

---

## 3. Application Logs & System Events (Nhật Ký Lỗi & Sự Kiện Hệ Thống)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` / `POST` | `/api/admin/logs/app` | `start=0&length=10&logLevel=&search=&fromDate=&toDate=` | Truy vấn nhật ký ứng dụng (Error, Warning, Info) |
| `GET` | `/api/admin/logs/app/{id}` | Path: `id` (long) | Xem chi tiết Call Stack Trace lỗi |
| `GET` | `/api/admin/logs/stats` | Không | Thống kê số lượng log hôm nay: `todayAudit`, `todayErrors`, `totalAudit` |
| `GET` | `/api/admin/logs/events/latest` | Không | Danh sách các sự kiện hệ thống mới phát sinh gần đây nhất |

```json
// Cấu trúc Log Statistics
{
  "todayAudit": 142,
  "todayErrors": 3,
  "totalAudit": 45800
}
```

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **Đọc Cấu Hình Khi Khởi Động App:** Khi Splash Screen chạy, gọi `GET /api/admin/system-config` và lưu trữ vào Provider toàn cục `SystemConfigProvider` để màn hình POS có ngay thuế VAT, tỷ lệ đổi điểm, số tài khoản nhận tiền chuyển khoản.
2. **Màn Hình Lịch Sử Hoạt Động (Security & Activity Feed):** Hiển thị danh sách sự kiện theo dòng thời gian Timeline với các icon tương ứng cho hành động: Thêm mới (Xanh lá), Sửa đổi (Vàng), Xóa (Đỏ).
