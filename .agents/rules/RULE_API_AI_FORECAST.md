# API Manual: AI Forecast, Replenishment, Anomaly Detection & FEFO Risk

> **Module Base Path:** `/api/admin/ai-analysis`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. AI Pipeline Triggers (Kích Hoạt Huấn Luyện & Tính Toán Dự Báo)

| Phương thức | Đường dẫn | Body | Mô tả |
|---|---|---|---|
| `POST` | `/api/admin/ai-analysis/run-analysis` | `RunAnalysisRequest` | Kích hoạt phân tích dự báo: `Mode = "system"` (Rule-based) hoặc `Mode = "ai"` (XGBoost/Prophet) |
| `POST` | `/api/admin/ai-analysis/run-prophet` | Không | Kích hoạt chạy riêng Python Prophet Machine Learning Pipeline |

```json
// RunAnalysisRequest
{
  "Mode": "system", // "system" hoặc "ai"
  "BranchId": 1
}
```

---

## 2. Replenishment & Recommendations (Gợi Ý Nhập Hàng Tự Động)

| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/ai-analysis/replenishment-advice/{branchId}` | Path: `branchId` | **Đề xuất nhập hàng**: AI tính toán lượng tồn kho hiện tại, nhu cầu tiêu thụ dự báo, số lượng đề xuất nhập kèm lý do cụ thể |
| `GET` | `/api/admin/ai-analysis/recommendations/{branchId}` | Path: `branchId` | Danh sách khuyến nghị nhập hàng phân theo ngành hàng |

### Cấu trúc DTO Replenishment Advice
```json
[
  {
    "ProductId": 10,
    "ProductName": "Sữa Tươi Tiệt Trùng Vinamilk 1L",
    "CurrentStock": 15,
    "PredictedDemand": 80,
    "SuggestedRestockQuantity": 100,
    "SupplierId": 3,
    "SupplierName": "Công ty Cổ phần Vinamilk",
    "Urgency": "High", // "High", "Medium", "Low"
    "Reason": "Tồn kho chỉ còn đủ bán trong 2 ngày tới dựa trên tốc độ tiêu thụ dự báo"
  }
]
```

---

## 3. Product Insights & Anomaly Detection (Phát Hiện Bất Thường & Xếp Hạng)

| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/ai-analysis/anomalies/{branchId}` | Path: `branchId` | Phát hiện biến động bất thường của doanh số sản phẩm (Spike tăng đột biến / Dip sụt giảm mạnh) |
| `GET` | `/api/admin/ai-analysis/insights/{branchId}` | Path: `branchId` | Phân loại sản phẩm thông minh: **Star** (bán chạy lợi nhuận cao), **Trending** (đang tăng trưởng), **Warning** (suy giảm) |

---

## 4. Sales & Inventory Forecasts (Dự Báo Bán Hàng & Tồn Kho Chuỗi Thời Gian)

| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/ai-analysis/inventory-forecast/{branchId}` | Path: `branchId` | Dự báo số dư tồn kho toàn bộ sản phẩm tại chi nhánh trong tương lai |
| `GET` | `/api/admin/ai-analysis/sales-forecast/{productId}/{branchId}` | Path: `productId`, `branchId` | Dự báo chuỗi thời gian doanh số bán của một sản phẩm trong 7 - 30 ngày tới |
| `GET` | `/api/admin/ai-analysis/top-predicted/{branchId}` | `topCount=10` | Top sản phẩm được dự báo sẽ có sức mua lớn nhất |
| `GET` | `/api/admin/ai-analysis/product-history/{productId}/{branchId}` | `start=yyyy-MM-dd&end=yyyy-MM-dd` | Lịch sử bán hàng thực tế theo từng ngày (dùng để vẽ biểu đồ so sánh với đường dự báo) |

---

## 5. Deadstock & FEFO Financial Risk Analysis (Phân Tích Hàng Tồn Đọng & Rủi Ro Hết Hạn)

| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/ai-analysis/deadstock/{branchId}` | Path: `branchId` | Danh sách hàng hóa chậm luân chuyển (Deadstock), tồn lâu ngày không bán được |
| `GET` | `/api/admin/ai-analysis/lot-financial-risk/{branchId}` | Path: `branchId` | **Mô phỏng rủi ro FEFO theo lô hàng**: Tính toán nguy cơ lô hàng hết hạn trước khi bán hết, tổng giá trị dự phòng rủi ro |

### Cấu trúc DTO Phân Tích Rủi Ro Lô Hàng (`LotFinancialRisk`)
```json
{
  "TotalProvisionValue": 15400000, // Tổng giá trị tiền có nguy cơ mất do hết hạn
  "TotalRiskLots": 6,
  "DangerCount": 2, // Lô hàng chắc chắn không kịp tiêu thụ trước hạn dùng
  "WarningCount": 4, // Lô hàng cần chạy khuyến mãi để đẩy nhanh tốc độ bán
  "SafeCount": 48,
  "Details": [
    {
      "LotId": 14,
      "LotNumber": "LOT-202604-02",
      "ProductId": 8,
      "ProductName": "Bánh Mì Tươi Sandwich",
      "ExpiryDate": "2026-10-15T00:00:00",
      "RemainingStock": 45,
      "PredictedSalesBeforeExpiry": 20,
      "UnsoldQuantity": 25,
      "EstimatedFinancialLoss": 500000,
      "RiskLevel": "Danger" // "Danger", "Warning", "Safe"
    }
  ]
}
```

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **One-Tap Purchase Order Generation (Tạo Đơn Nhập Hàng Từ Gợi Ý AI):**
   - Màn hình "Gợi Ý Nhập Hàng" hiển thị danh sách Replenishment Advice.
   - Thêm nút "Tạo Phiếu Nhập Kho": Tự động gom các sản phẩm được đề xuất thành một `PurchaseOrderDTO` và chuyển sang màn hình xác nhận đơn nhập hàng `POST /api/admin/inventory/purchase`.
2. **Biểu Đồ So Sánh Thực Tế & Dự Báo (History vs Forecast Chart):**
   - Tích hợp biểu đồ đa đường: Đường nét liền thể hiện dữ liệu lịch sử thực tế (`product-history`), đường nét đứt thể hiện dữ liệu dự đoán tương lai (`sales-forecast`).
3. **Cảnh Báo Lô Hàng Nguy Hiểm:**
   - Dùng badge màu đỏ (Danger) nhấp nháy cho các lô hàng trong `lot-financial-risk`, kèm nút hành động nhanh "Tạo Khuyến Mãi Thanh Lý" chuyển sang form tạo `PromotionDTO` giảm giá.
