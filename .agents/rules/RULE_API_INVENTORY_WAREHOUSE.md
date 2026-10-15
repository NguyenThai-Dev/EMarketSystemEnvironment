# API Manual: Inventory, Warehouses, Stock Control, Purchase Orders & Supplier Debts

> **Module Base Path:** `/api/admin/inventory`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. Warehouses (Quản lý Kho Bãi)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/inventory/warehouses/all` | Không | Lấy tất cả kho bãi trong hệ thống |
| `GET` | `/api/admin/inventory/warehouses/by-branch/{branchId}` | Path: `branchId` | Lấy danh sách kho thuộc một chi nhánh |
| `GET` | `/api/admin/inventory/warehouses/{id}` | Path: `id` | Lấy chi tiết thông tin kho |
| `POST` | `/api/admin/inventory/warehouses` | `WarehouseDTO` | Tạo mới kho |
| `PUT` / `POST` | `/api/admin/inventory/warehouses`<br>`/api/admin/inventory/warehouses/update` | `WarehouseDTO` | Cập nhật thông tin kho |
| `DELETE` / `POST` | `/api/admin/inventory/warehouses/{id}`<br>`/api/admin/inventory/warehouses/delete/{id}` | Path: `id` | Xóa kho |

```json
// WarehouseDTO
{
  "WarehouseId": 1,
  "Name": "Kho Chính Q1",
  "BranchId": 1,
  "BranchName": "Chi nhánh Trung tâm",
  "Location": "Tầng hầm B1, TTTM",
  "IsActive": true
}
```

---

## 2. Stock Movements & Adjustment (Biến Động Tồn Kho & Kiểm Kê)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` / `POST` | `/api/admin/inventory/stock/movements` | `start=0&length=10&warehouseId=&type=&fromDate=&toDate=&keyword=` | Nhật ký xuất/nhập/điều chuyển tồn kho (DataTable) |
| `GET` | `/api/admin/inventory/stock/total-actual` | `productId=10&warehouseId=1` | Lấy số lượng tồn thực tế của sản phẩm tại kho |
| `POST` | `/api/admin/inventory/stock/adjust` | `StockAdjustmentDTO` | **Kiểm kê / Điều chỉnh tồn**: Cân bằng số lượng thực tế & số lượng sổ sách |

```json
// StockAdjustmentDTO
{
  "WarehouseId": 1,
  "ProductId": 10,
  "LotId": 5,
  "ActualQuantity": 95,      // Số lượng đếm thực tế
  "SystemQuantity": 100,     // Số lượng trên phần mềm
  "Reason": "Hàng bị vỡ hỏng trong quá trình vận chuyển",
  "UserId": 2
}
```

---

## 3. Inventory Items (Tồn Kho Sản Phẩm Tại Kho)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/inventory/items` | `warehouseId=&productId=` | Danh sách số dư tồn kho chi tiết |
| `GET` | `/api/admin/inventory/items/{id}` | Path: `id` | Chi tiết số dư tồn kho |
| `POST` | `/api/admin/inventory/items` | `InventoryItemDTO` | Khởi tạo số dư tồn kho |
| `PUT` / `POST` | `/api/admin/inventory/items`<br>`/api/admin/inventory/items/update` | `InventoryItemDTO` | Cập nhật thông số tồn kho |
| `DELETE` / `POST` | `/api/admin/inventory/items/{id}`<br>`/api/admin/inventory/items/delete/{id}` | Path: `id` | Xóa bản ghi tồn kho |

---

## 4. Purchase Orders (Đơn Nhập Hàng Nhà Cung Cấp)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/inventory/purchase/all` | Không | Lấy tất cả đơn nhập hàng |
| `GET` / `POST` | `/api/admin/inventory/purchase/paged`<br>`/api/admin/inventory/purchase/search` | `start=0&length=10&keyword=&supplierId=&branchId=&warehouseId=&status=&paymentStatus=&fromDate=&toDate=` | Tìm kiếm & phân trang đơn nhập hàng |
| `GET` | `/api/admin/inventory/purchase/{id}` | Path: `id` | Lấy chi tiết đơn nhập kèm danh sách hàng nhập |
| `POST` | `/api/admin/inventory/purchase` | `PurchaseOrderDTO` | Tạo mới đơn đặt hàng / nhập kho NCC |
| `PUT` / `POST` | `/api/admin/inventory/purchase`<br>`/api/admin/inventory/purchase/update` | `PurchaseOrderDTO` | Cập nhật đơn nhập hàng |
| `DELETE` / `POST` | `/api/admin/inventory/purchase/{id}`<br>`/api/admin/inventory/purchase/delete/{id}` | Path: `id` | Hủy / Xóa đơn nhập hàng |

```json
// PurchaseOrderDTO
{
  "PurchaseOrderId": 0,
  "SupplierId": 3,
  "BranchId": 1,
  "WarehouseId": 1,
  "OrderDate": "2026-10-07T10:00:00",
  "ExpectedDeliveryDate": "2026-10-10T10:00:00",
  "TotalAmount": 25000000,
  "PaidAmount": 10000000,
  "Status": "Completed",        // "Pending", "Received", "Completed", "Cancelled"
  "PaymentStatus": "Partial",   // "Unpaid", "Partial", "Paid"
  "Notes": "Nhập hàng sữa đợt 1 tháng 10",
  "Details": [
    {
      "ProductId": 10,
      "Quantity": 500,
      "UnitCost": 28000,
      "LotNumber": "LOT-202610-01",
      "ExpiryDate": "2027-04-01T00:00:00"
    }
  ]
}
```

---

## 5. Supplier Debts & Payments (Công Nợ & Trả Tiền NCC)

| Phương thức | Đường dẫn | Tham số / Body | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/inventory/debt/all` | Không | Lấy toàn bộ danh sách công nợ |
| `GET` | `/api/admin/inventory/debt/summary` | `supplierId=&branchId=` | Tổng hợp công nợ: Tổng nợ, Đã trả, Còn nợ |
| `GET` / `POST` | `/api/admin/inventory/debt/paged`<br>`/api/admin/inventory/debt/list` | `start=0&length=10&keyword=&supplierId=&status=&fromDate=&toDate=` | Danh sách công nợ phân trang |
| `GET` | `/api/admin/inventory/debt/{id}` | Path: `id` | Chi tiết bản ghi nợ |
| `GET` | `/api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}` | Path: `purchaseOrderId` | Tra cứu khoản nợ theo mã đơn nhập hàng |
| `POST` | `/api/admin/inventory/debt/create`<br>`/api/admin/inventory/debt` | `SupplierDebtDTO` | Tạo khoản nợ NCC |
| `POST` / `PUT` | `/api/admin/inventory/debt/update`<br>`/api/admin/inventory/debt/{id}` | `SupplierDebtDTO` | Cập nhật khoản nợ |
| `GET` | `/api/admin/inventory/debt/near-due` | `daysThreshold=7` | Danh sách công nợ sắp đến hạn thanh toán |
| `GET` | `/api/admin/inventory/debt/overdue` | Không | Danh sách công nợ đã quá hạn thanh toán |
| `GET` | `/api/admin/inventory/debt/{debtId}/payments` | Path: `debtId` | Lịch sử các lần thanh toán tiền nợ |
| `POST` | `/api/admin/inventory/debt/payment` | `SupplierPaymentDTO` | **Ghi nhận thanh toán trả nợ**: Giảm trừ công nợ NCC |
| `DELETE` / `POST` | `/api/admin/inventory/debt/payment/{id}` | Path: `id` | Hủy phiếu thanh toán nợ |

```json
// SupplierPaymentDTO
{
  "DebtId": 12,
  "Amount": 5000000,
  "PaymentDate": "2026-10-07T11:00:00",
  "PaymentMethod": "Banking",
  "Note": "Ủy nhiệm chi Vietcombank chuyển tiền đợt 2"
}
```

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **Giao diện Kiểm Kê Di Động:** Thiết kế màn hình Stock Adjustment cho phép nhân viên cầm điện thoại đi quét từng mã vạch sản phẩm tại kệ. App tự động đối chiếu `SystemQuantity` và hiển thị ô nhập `ActualQuantity`. Nếu có chênh lệch, tô màu đỏ/vàng cảnh báo.
2. **Cảnh Báo Công Nợ Đến Hạn:** Sử dụng Badge đỏ trên Icon "Công Nợ" hiển thị số lượng hóa đơn lấy từ `GET /debt/near-due` và `GET /debt/overdue`.
