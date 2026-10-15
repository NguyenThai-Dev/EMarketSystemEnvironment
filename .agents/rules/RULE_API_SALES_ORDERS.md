# API Manual: Sales, Orders, POS Checkout, PayOS, Quotation & Promotions

> **Module Base Paths:**  
> - Bán hàng & Đơn hàng: `/api/admin/sales`  
> - Báo giá: `/api/admin/quotation`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. Orders Management (Quản lý Đơn Hàng)

| Phương thức | Đường dẫn | Query / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/sales/orders` | `start=0&length=10&userId=&branchId=&status=&fromDate=&toDate=&keyword=` | Lấy danh sách đơn hàng có phân trang & lọc |
| `GET` | `/api/admin/sales/orders/all` | Không | Lấy toàn bộ đơn hàng |
| `GET` | `/api/admin/sales/orders/{id}` | Path: `id` | Lấy chi tiết đơn hàng (kèm danh sách món) |
| `GET` | `/api/admin/sales/orders/full-by-branch` | `branchId=&fromDate=&toDate=` | Lấy danh sách đơn hàng đầy đủ theo chi nhánh |
| `GET` | `/api/admin/sales/orders/by-branch` | `branchId=&fromDate=&toDate=` | Lấy tóm tắt đơn hàng theo chi nhánh |
| `GET` | `/api/admin/sales/orders/{orderId}/details` | Path: `orderId` | Lấy danh sách OrderDetailDTO của đơn hàng |
| `POST` | `/api/admin/sales/orders/create` | `OrderDTO` | Tạo đơn hàng thủ công |
| `POST` | `/api/admin/sales/orders/{orderId}/status` | Body: `{ "Status": "Completed", "ConnectionId": "" }` | Cập nhật trạng thái đơn (Pending, Completed, Cancelled...) |
| `DELETE` / `POST` | `/api/admin/sales/orders/{orderId}`<br>`/api/admin/sales/orders/delete/{orderId}` | Path: `orderId` | Xóa đơn hàng |

---

## 2. POS Checkout (Thanh Toán Đơn Hàng Tại Quầy / App)

| Phương thức | Đường dẫn | Body | Mô tả |
|---|---|---|---|
| `POST` | `/api/admin/sales/orders/checkout` | `CheckoutRequestDTO` | Tạo đơn hàng tự động trừ tồn kho, áp dụng điểm thưởng & khuyến mãi |

### Cấu trúc DTO Checkout (`CheckoutRequestDTO`)
```json
{
  "CustomerId": 1,
  "UserId": 2,
  "BranchId": 1,
  "PaymentMethod": "Cash", // "Cash", "PayOS", "Banking"
  "PromotionId": null,
  "UsePoints": 50,
  "Notes": "Đóng gói cẩn thận",
  "Items": [
    {
      "ProductId": 10,
      "Quantity": 2,
      "UnitPrice": 25000,
      "LotId": 3
    }
  ]
}

// Phản hồi Checkout Thành Công
{
  "Success": true,
  "Message": "Thanh toán thành công!",
  "OrderId": 1024,
  "TotalAmount": 50000,
  "DiscountAmount": 5000,
  "FinalAmount": 45000,
  "PointsEarned": 45
}
```

---

## 3. Order Details CRUD (Chi tiết món trong đơn)

| Phương thức | Đường dẫn | Body | Mô tả |
|---|---|---|---|
| `POST` | `/api/admin/sales/orders/details/create` | `OrderDetailDTO` | Thêm sản phẩm vào đơn |
| `POST` / `PUT` | `/api/admin/sales/orders/details/update`<br>`/api/admin/sales/orders/details/{id}` | `OrderDetailDTO` | Sửa số lượng / giá sản phẩm trong đơn |
| `DELETE` / `POST` | `/api/admin/sales/orders/details/{id}`<br>`/api/admin/sales/orders/details/delete/{id}` | Path: `id` | Xóa sản phẩm khỏi đơn |

---

## 4. Payment & PayOS Integration (Cổng Thanh Toán QR PayOS)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `POST` | `/api/admin/sales/orders/create-payos-link` | `CreateQrRequestDTO` | Tạo link / QR Code thanh toán VietQR qua PayOS |
| `GET` | `/api/admin/sales/orders/check-payos-status/{orderCode}` | Path: `orderCode` (long) | Kiểm tra trạng thái thanh toán PayOS (Poller) |
| `GET` | `/api/admin/sales/orders/{orderId}/payments` | Path: `orderId` | Lấy lịch sử các giao dịch thanh toán của đơn |

### Cấu trúc `CreateQrRequestDTO` & Phản hồi PayOS
```json
// Request
{
  "Amount": 150000,
  "Description": "Thanh toan don DH1024",
  "ReturnUrl": "emarket://payment-success",
  "CancelUrl": "emarket://payment-cancel"
}

// Response
{
  "Success": true,
  "OrderCode": 1718293847291,
  "CheckoutUrl": "https://pay.payos.vn/web/...",
  "QrCode": "00020101021238540010A00000072701260006970422...",
  "Amount": 150000
}
```

---

## 5. Quotation Management (Quản lý Báo Giá)

> **Base Route:** `/api/admin/quotation`

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/quotation/list` | `keyword=&branchId=&status=&fromDate=&toDate=` | Lấy toàn bộ danh sách báo giá theo bộ lọc |
| `GET` | `/api/admin/quotation/paged` | `start=0&length=20&keyword=&branchId=&status=` | Phân trang danh sách báo giá |
| `GET` | `/api/admin/quotation/{id}` | Path: `id` | Lấy chi tiết báo giá và các sản phẩm báo giá |
| `POST` | `/api/admin/quotation/create`<br>`/api/admin/quotation/save` | `QuotationDTO` | Tạo mới hoặc lưu báo giá |
| `POST` / `PUT` | `/api/admin/quotation/update`<br>`/api/admin/quotation/{id}` | `QuotationDTO` | Cập nhật nội dung báo giá |
| `POST` | `/api/admin/quotation/{id}/status` | Body: `{ "Status": "Approved" }` | Chuyển đổi trạng thái báo giá (Draft, Approved, Rejected, Cancelled) |
| `POST` | `/api/admin/quotation/{id}/convert-to-order` | Query: `userId=1` | **Chốt đơn**: Chuyển đổi báo giá thành Đơn hàng chính thức |
| `DELETE` / `POST` | `/api/admin/quotation/{id}`<br>`/api/admin/quotation/delete/{id}` | Path: `id` | Xóa báo giá (chỉ áp dụng khi ở trạng thái Nháp) |

### Cấu trúc DTO Báo Giá (`QuotationDTO`)
```json
{
  "QuotationId": 0,
  "QuotationCode": "BG-202610-001",
  "CustomerId": 5,
  "CustomerName": "Công ty TNHH ABC",
  "BranchId": 1,
  "TotalAmount": 12000000,
  "Status": "Draft", // "Draft", "Sent", "Approved", "Converted", "Cancelled"
  "ValidUntil": "2026-10-31T23:59:59",
  "Notes": "Báo giá thiết bị văn phòng",
  "Details": [
    {
      "ProductId": 12,
      "ProductName": "Bàn làm việc thông minh",
      "Quantity": 4,
      "UnitPrice": 3000000,
      "Discount": 0
    }
  ]
}
```

---

## 6. Promotions (Quản lý Chương Trình Khuyến Mãi)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/sales/promotions` | Không | Lấy danh sách toàn bộ khuyến mãi |
| `GET` | `/api/admin/sales/promotions/active` | Không | Lấy các khuyến mãi đang có hiệu lực hiện tại |
| `GET` | `/api/admin/sales/promotions/search` | `keyword=&categoryId=&discountType=&cusType=&fromDate=&toDate=` | Tìm kiếm và lọc chương trình khuyến mãi |
| `GET` | `/api/admin/sales/promotions/{id}` | Path: `id` | Lấy chi tiết khuyến mãi |
| `POST` | `/api/admin/sales/promotions/create` | `PromotionDTO` | Tạo chương trình khuyến mãi mới |
| `POST` / `PUT` | `/api/admin/sales/promotions/update`<br>`/api/admin/sales/promotions/{id}` | `PromotionDTO` | Cập nhật chương trình khuyến mãi |
| `DELETE` / `POST` | `/api/admin/sales/promotions/{id}`<br>`/api/admin/sales/promotions/delete/{id}` | Path: `id` | Xóa chương trình khuyến mãi |

### Cấu trúc DTO Khuyến Mãi (`PromotionDTO`)
```json
{
  "PromotionId": 1,
  "Name": "Giảm giá 10% khách VIP",
  "Code": "VIP10",
  "DiscountType": "Percentage", // "Percentage" hoặc "FixedAmount"
  "DiscountValue": 10.0,
  "MinimumOrderValue": 500000,
  "MaxDiscount": 100000,
  "StartDate": "2026-10-01T00:00:00",
  "EndDate": "2026-10-31T23:59:59",
  "IsActive": true
}
```

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **POS Quick Scan:** Khi quét mã sản phẩm bằng camera, thêm sản phẩm trực tiếp vào giỏ hàng cục bộ `CartProvider`.
2. **Checkout Flow:** Khi nhấn Thanh toán:
   - Nếu chọn `PayOS`: Gọi `/create-payos-link`, hiển thị QR Code lên màn hình kèm bộ đếm ngược (Timer 3 phút) định kỳ gọi `/check-payos-status/{orderCode}` mỗi 3 giây.
   - Nếu chọn `Cash`: Gọi `/checkout` trực tiếp với `PaymentMethod = "Cash"`.
3. **SignalR Cập Nhật Thời Gian Thực:** Kết nối SignalR Hub `OrderHub` lắng nghe sự kiện `orderChanged` để tự động xác nhận đơn khi webhook PayOS báo thành công.
