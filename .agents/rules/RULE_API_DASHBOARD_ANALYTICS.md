# API Manual: Dashboard, Business Analytics & Reports

> **Module Base Path:** `/api/admin/dashboard`  
> **Header Yêu Cầu:** `Authorization: Bearer <token>`, `Content-Type: application/json`

---

## 1. Executive & Operational Dashboards (Tổng Quan Hoạt Động Doanh Nghiệp)

| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/dashboard/kpi-summary`<br>`/api/admin/dashboard/cards` | `branchId=` | **Thẻ KPI Card**: Doanh thu hôm nay/tháng, số đơn hôm nay, khách hàng mới, số mặt hàng sắp hết kho |
| `GET` | `/api/admin/dashboard/summary`<br>`/api/admin/dashboard/overview` | `fromDate=&toDate=&groupBy=day&branchId=` | Biểu đồ doanh thu, số đơn, lợi nhuận gộp theo ngày ("day") hoặc tháng ("month") |
| `GET` | `/api/admin/dashboard/branch-performance` | `branchId=&fromDate=&toDate=` | So sánh hiệu suất kinh doanh giữa các chi nhánh |
| `GET` | `/api/admin/dashboard/stock-chart` | `branchId=` | Tỷ trọng cơ cấu tồn kho theo danh mục sản phẩm |
| `GET` | `/api/admin/dashboard/super-admin` | Không | Trung tâm chỉ huy Super Admin (Tổng hợp toàn hệ thống) |

### Cấu trúc DTO Thẻ KPI (`DashboardSummaryDTO`)
```json
{
  "TodayRevenue": 15450000,
  "MonthRevenue": 348000000,
  "TodayOrders": 42,
  "MonthOrders": 980,
  "TotalCustomers": 1250,
  "LowStockCount": 8
}
```

### Cấu trúc DTO Overview (`DashboardOverviewDTO`)
```json
{
  "Labels": ["01/10", "02/10", "03/10", "04/10", "05/10", "06/10", "07/10"],
  "RevenueSeries": [12000000, 15000000, 11500000, 18200000, 14000000, 19500000, 15450000],
  "OrdersSeries": [30, 38, 28, 45, 36, 52, 42],
  "TotalRevenue": 105650000,
  "TotalOrders": 271
}
```

---

## 2. Department-Specific Dashboards (Báo Cáo Chuyên Sâu Theo Bộ Phận)

### A. Kho Bãi (`/warehouse`)
| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/dashboard/warehouse` | `dayBacks=30&branchId=&warehouseId=` | Tỷ lệ xuất - nhập - tồn kho, tỷ lệ hao hụt kiểm kê, sản phẩm luân chuyển chậm |

### B. Tài Chính & Dòng Tiền (`/finance`)
| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/dashboard/finance` | `daysBack=30&branchId=` | Doanh thu thuần, Chi phí vận hành, Lợi nhuận gộp, Dòng tiền mặt vs Chuyển khoản |

### C. Công Nợ Nhà Cung Cấp (`/debt`)
| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/dashboard/debt` | `branchId=&supplierId=&fromDate=&toDate=` | Tổng nợ phải trả, Nợ trong hạn, Nợ quá hạn, Kế hoạch dòng tiền thanh toán nợ |

### D. Nhân Sự & Vận Hành (`/people`)
| Phương thức | Đường dẫn | Tham số | Mô tả |
|---|---|---|---|
| `GET` | `/api/admin/dashboard/people` | Không | Số lượng nhân sự theo vai trò (Thu ngân, Thủ kho, Quản lý), Nhân sự tích cực |

---

## Hướng Dẫn Tích Hợp Flutter / Android

1. **Thư viện Vẽ Biểu Đồ:** Sử dụng package `fl_chart` để render biểu đồ đường (LineChart) cho Doanh thu, BarChart cho Hiệu suất chi nhánh và PieChart cho Cơ cấu tồn kho.
2. **Bộ Lọc Thời Gian Nhanh (Date Quick Filter):** Cung cấp các nút bấm nhanh: "Hôm nay", "7 ngày qua", "30 ngày qua", "Tháng này" tương ứng truyền giá trị `fromDate` và `toDate` chuẩn ISO.
3. **Pull-to-Refresh:** Trang bị `RefreshIndicator` ở đầu màn hình Dashboard để cho phép chủ cửa hàng kéo xuống cập nhật lại dữ liệu mới nhất tức thì.
