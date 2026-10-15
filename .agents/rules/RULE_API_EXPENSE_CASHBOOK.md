# API Manual: Expenses, Cashbook & Categories

> **Module Base Path:** `/api/admin/expense`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. Expenses (Quản lý Phiếu Chi / Chi Phí Cửa Hàng)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/expense/list` | `branchId=&categoryId=&fromDate=&toDate=&status=` | Lấy danh sách toàn bộ phiếu chi theo bộ lọc |
| `GET` | `/api/admin/expense/paged` | `start=0&length=20&branchId=&categoryId=&fromDate=&toDate=&status=` | Lấy danh sách phiếu chi có phân trang |
| `GET` | `/api/admin/expense/summary` | `branchId=&fromDate=&toDate=` | Thống kê tổng tiền chi, số phiếu theo trạng thái (Approved, Pending, Rejected) |
| `GET` | `/api/admin/expense/{id}` | Path: `id` | Lấy chi tiết thông tin phiếu chi |
| `POST` | `/api/admin/expense/create` | `ExpenseDTO` | Lập phiếu chi mới (chờ duyệt) |
| `POST` | `/api/admin/expense/update-status`<br>`/api/admin/expense/{id}/status` | `UpdateExpenseStatusRequest` | **Duyệt / Từ chối phiếu chi** (Approved, Rejected) |
| `DELETE` / `POST` | `/api/admin/expense/{id}`<br>`/api/admin/expense/delete/{id}` | Path: `id` | Xóa phiếu chi |

### Cấu trúc DTOs Phiếu Chi (`ExpenseDTO`)
```json
// ExpenseDTO (Tạo phiếu chi)
{
  "ExpenseId": 0,
  "Title": "Tiền điện nước tháng 10/2026",
  "Amount": 3500000,
  "CategoryId": 1,
  "BranchId": 1,
  "UserId": 2,
  "PaymentMethod": "Cash", // "Cash", "Banking"
  "RefImage": "/Uploads/Expenses/2026/10/hoa_don_dien.jpg",
  "Notes": "Hóa đơn EVN mã khách hàng PE012345678",
  "Status": "Pending" // "Pending", "Approved", "Rejected"
}

// UpdateExpenseStatusRequest (Duyệt phiếu)
{
  "ExpenseId": 15,
  "Status": "Approved",      // "Approved" hoặc "Rejected"
  "RejectReason": null,      // Bắt buộc nếu Status = "Rejected"
  "PaymentMethod": "Cash"
}

// Phản hồi Summary (`GET /api/admin/expense/summary`)
{
  "TotalExpenses": 25,
  "TotalApprovedAmount": 42000000,
  "TotalPendingAmount": 8500000,
  "TotalRejectedAmount": 1200000,
  "ApprovedCount": 18,
  "PendingCount": 5,
  "RejectedCount": 2
}
```

---

## 2. Expense Categories (Danh Mục Chi Phí)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/expense/categories` | Không | Lấy toàn bộ danh mục chi |
| `GET` | `/api/admin/expense/categories/active` | Không | Lấy danh mục chi đang hoạt động (dùng cho Dropdown lập phiếu) |
| `GET` | `/api/admin/expense/categories/{categoryId}` | Path: `categoryId` | Lấy chi tiết một danh mục chi |
| `POST` | `/api/admin/expense/categories/create` | `ExpenseCategoryDTO` | Tạo mới danh mục chi |
| `POST` / `PUT` | `/api/admin/expense/categories/update`<br>`/api/admin/expense/categories/{id}` | `ExpenseCategoryDTO` | Cập nhật danh mục chi |
| `DELETE` / `POST` | `/api/admin/expense/categories/{categoryId}`<br>`/api/admin/expense/categories/delete/{categoryId}` | Path: `categoryId` | Xóa danh mục chi |

```json
// ExpenseCategoryDTO
{
  "CategoryId": 1,
  "Name": "Chi phí điện nước & tiện ích",
  "Description": "Các hóa đơn tiện ích định kỳ hàng tháng",
  "IsActive": true
}
```

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **Chụp Ảnh Hóa Đơn:** Trên màn hình tạo phiếu chi, cho phép người dùng chụp ảnh hóa đơn giấy hoặc chọn từ thư viện ảnh bằng `image_picker`.
2. **Quy Trình Duyệt Chi Phí (Approval Workflow):**
   - Nhân viên tạo phiếu -> Trạng thái mặc định là `Pending` (Màu vàng).
   - Quản lý / Chủ cửa hàng nhận thông báo -> Bấm "Duyệt" (xanh lá) hoặc "Từ chối" (đỏ kèm lý do từ chối `RejectReason`).
