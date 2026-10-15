# EMarket Mobile Engineering Standards & Business-Parity Blueprint
### Kim Chỉ Nam Phát Triển Ứng Dụng Di Động Cho Hệ Thống EMarket (Hỗ Trợ Toàn Diện Android Java & Flutter)

---

## MỤC LỤC
1. [Tôn Chỉ & Nguyên Tắc Cốt Lõi (Core Principles)](#1-tôn-chỉ--nguyên-tắc-cốt-lõi-core-principles)
2. [Bản Đồ Nghiệp Vụ Toàn Diện Hệ Thống EMarket (10 Modules + AI)](#2-bản-đồ-nghiệp-vụ-toàn-diện-hệ-thống-emarket-10-modules--ai)
   - [2.1. Module Xác Thực, Người Dùng & Chi Nhánh (Auth, User & Branch)](#21-module-xác-thực-người-dùng--chi-nhánh-auth-user--branch)
   - [2.2. Module Sản Phẩm, Danh Mục & Lô Hàng (Product, Category & Lot)](#22-module-sản-phẩm-danh-mục--lô-hàng-product-category--lot)
   - [2.3. Module Kho Hàng & Biến Động Tồn Kho (Warehouse & Stock Movement)](#23-module-kho-hàng--biến-động-tồn-kho-warehouse--stock-movement)
   - [2.4. Module Nhập Hàng & Công Nợ Nhà Cung Cấp (Purchase Order & Supplier Debt)](#24-module-nhập-hàng--công-nợ-nhà-cung-cấp-purchase-order--supplier-debt)
   - [2.5. Module Bán Hàng, Thu Ngân & POS (Sales, Cashier & Checkout)](#25-module-bán-hàng-thu-ngân--pos-sales-cashier--checkout)
   - [2.6. Module Khách Hàng, Điểm Thưởng & Khuyến Mãi (Customer, Loyalty & Promotion)](#26-module-khách-hàng-điểm-thưởng--khuyến-mãi-customer-loyalty--promotion)
   - [2.7. Module Sổ Thu Chi & Phiếu Chi (Expense Management)](#27-module-sổ-thu-chi--phiếu-chi-expense-management)
   - [2.8. Module Báo Giá & Chuyển Đổi Đơn Hàng (Quotation)](#28-module-báo-giá--chuyển-đổi-đơn-hàng-quotation)
   - [2.9. Module Báo Cáo & Thống Kê Dashboard (Dashboard & Analytics)](#29-module-báo-cáo--thống-kê-dashboard-dashboard--analytics)
   - [2.10. Module Trí Tuệ Nhân Tạo & Dự Báo (AI Forecast & Analytics)](#210-module-trí-tuệ-nhân-tạo--dự-báo-ai-forecast--analytics)
   - [2.11. Module Cấu Hình Hệ Thống & Nhật Ký Kiểm Toán (System Config & Audit Logs)](#211-module-cấu-hình-hệ-thống--nhật-ký-kiểm-toán-system-config--audit-logs)
3. [Chuẩn Kiến Trúc Ứng Dụng Android Native (Java)](#3-chuẩn-kiến-trúc-ứng-dụng-android-native-java)
4. [Chuẩn Kiến Trúc Ứng Dụng Cross-Platform (Flutter)](#4-chuẩn-kiến-trúc-ứng-dụng-cross-platform-flutter)
5. [Bảng Đối Chiếu Triển Khai Kỹ Thuật: Java vs. Flutter](#5-bảng-đối-chiếu-triển-khai-kỹ-thuật-java-vs-flutter)
6. [Chuẩn Tương Tác API, Big Data & Phân Trang Server-Side](#6-chuẩn-tương-tác-api-big-data--phân-trang-server-side)
7. [Bảo Mật, Token JWT & Cơ Chế Phân Quyền (RBAC)](#7-bảo-mật-token-jwt--cơ-chế-phân-quyền-rbac)
8. [Chuẩn Đồng Bộ Thời Gian Thực (Realtime SignalR & WebSocket)](#8-chuẩn-đồng-bộ-thời-gian-thực-realtime-signalr--websocket)
9. [Xử Lý Dữ Liệu Tài Chính & Số Thập Phân (Decimal Safety)](#9-xử-lý-dữ-liệu-tài-chính--số-thập-phân-decimal-safety)
10. [Quy Trình Phát Triển Tính Năng (Feature Workflow)](#10-quy-trình-phát-triển-tính-năng-feature-workflow)
11. [Tiêu Chuẩn Hoàn Thành (Definition of Done - DoD)](#11-tiêu-chuẩn-hoàn-thành-definition-of-done---dod)

---

# 1. TÔN CHỈ & NGUYÊN TẮC CỐT LÕI (CORE PRINCIPLES)

### 1.1. Backend / Database Là "Source Of Truth" Tuyệt Đối
* Ứng dụng di động (bao gồm cả phiên bản Android Java Native và Flutter Cross-Platform) là **các client giao tiếp trực tiếp với hệ sinh thái EMarket**.
* Mọi quy tắc kinh doanh (business logic), luồng duyệt, tính toán tài chính, trạng thái tồn kho, phân hạng khách hàng và chuyển đổi trạng thái đơn hàng **phải phản ánh trung thực 100% logic backend** (ASP.NET Web API tại `EMarket/ApiControllers/Admin`, Modules tại `EMarket/Modules`, và cơ sở dữ liệu `CSDL.sql`).
* **Nghiêm cấm tự biên soạn business rules riêng trên client** mà không có sự đồng thuận hoặc ánh xạ từ backend.

### 1.2. Business Parity ≠ UI Parity
* **Business Parity (Bình đẳng nghiệp vụ)**: Mọi điều kiện hợp lệ, kiểm tra ràng buộc, mã lỗi, trạng thái giao dịch phải tương đương với hệ thống Web.
* **UI Parity (Tối ưu trải nghiệm)**: Mobile không sao chép máy móc giao diện bảng biểu phức tạp của Desktop. Mobile được phép và khuyến khích:
  - Chuyển đổi bảng dữ liệu lớn (DataTables) thành danh sách thẻ cuộn mượt (Card Lists / RecyclerView / ListView) kèm bộ lọc theo BottomSheet.
  - Tối ưu thao tác một tay, chạm nhanh, quét mã vạch bằng camera (Barcode/QR scanner), thanh toán nhanh (Quick Cash, QR PayOS).
  - Sử dụng Gestures (Swipe to action, Pull-to-refresh).

### 1.3. Zero Tolerance Cho Fake Data & Fake Success
* Tuyệt đối không giả lập trạng thái thành công (`Fake Success`) trước khi nhận được phản hồi HTTP 200/201 kèm dữ liệu hợp lệ từ Server.
* Tuyệt đối không sinh mã định danh giả (`Fake OrderId`, `Fake PaymentCode`, `Fake StockId`) trên client rồi cập nhật trạng thái như thể đã lưu thành công.
* Khi API gặp lỗi hoặc mất mạng:
  - Phải hiển thị lỗi rõ ràng theo mã lỗi nghiệp vụ của Server.
  - Phải giữ lại toàn bộ dữ liệu đang nhập (Form State / Cart State) để người dùng có thể bấm **Thử lại (Retry)** mà không mất công nhập lại.

### 1.4. Chiến Lược API: Ưu Tiên REST API Chuẩn Hóa
* Tất cả tính năng mới trên Mobile phải kết nối qua cụm endpoint RESTful chuẩn: `/api/admin/...` với xác thực Bearer Token (JWT).
* Không sử dụng Cookie/Session MVC truyền thống (`/Admin/...`) cho client mới trừ các tính năng đặc thù đang trong quá trình chuyển giao.

---

# 2. BẢN ĐỒ NGHIỆP VỤ TOÀN DIỆN HỆ THỐNG EMARKET (10 MODULES + AI)

Mọi lập trình viên (dù viết Java hay Flutter) trước khi làm việc phải đối chiếu module tương ứng dưới đây:

```
EMarket Architecture Blueprint
│
├── 1. Auth & User Module ────► JWT Auth, Roles, Permissions, Multi-Branch, Google OAuth, OTP
├── 2. Product Module ────────► Products, Categories, Suppliers, Lots/Batches, Stock Thresholds
├── 3. Inventory Module ──────► Warehouses, Stock In/Out, Stock Adjustments, Movements Audit
├── 4. Purchase & Debt ───────► Purchase Orders, Supplier Debts, Payment Milestones, Overdue
├── 5. Sales & POS ───────────► POS Cart, Barcode, Promos, Points, sp_Sales_Checkout, PayOS QR
├── 6. Customer & Loyalty ────► Customer Profiles, Addresses, VIP/Loyalty Points, Tier Discount
├── 7. Expense Module ────────► Expense Vouchers, Categories, Approval (Pending/Approved/Rejected)
├── 8. Quotation Module ──────► Quotations, Validity Period, Convert Quotation to Real Order
├── 9. Dashboard Analytics ───► Revenues, Profit, Inventory Value, Branch Performance, SuperAdmin
├── 10. AI Forecast & Labs ───► Prophet Pipeline, Replenishment Advice, Deadstock, FEFO Lot Risk
└── 11. System & Realtime ────► System Configurations (VAT, Point Rates), SignalR OrderHub
```

---

### 2.1. Module Xác Thực, Người Dùng & Chi Nhánh (Auth, User & Branch)
* **API Controller**: `EMarket.ApiControllers.Admin.UserAdminApiController`
* **Route Prefix**: `/api/admin/user-management`
* **Nghiệp vụ cốt lõi**:
  - **Đăng nhập mật khẩu**: `POST /auth/login` (Body: `{ EmailOrUsername, Password }`). Trả về JWT Token và `CurrentUserDTO` (UserId, FullName, Email, RoleId, BranchId, Avatar).
  - **Google OAuth Login**: `POST /auth/external-login` hoặc `POST /auth/google-login` (Body: `{ Email }`). Chỉ email đã được Admin kích hoạt trong hệ thống mới được cấp token.
  - **Quên mật khẩu qua OTP**: `POST /auth/request-otp` -> gửi mã về email -> `POST /auth/reset-password` (Body: `{ Email, Otp, NewPassword }`).
  - **Đa Chi Nhánh (Multi-Branch)**: Người dùng thuộc về một chi nhánh mặc định (`BranchId`). Super Admin có quyền xem và chuyển đổi giữa tất cả chi nhánh.
  - **Định vị chi nhánh gần nhất**: `GET /branches/nearest?lat={lat}&lng={lng}&maxDist={km}` phục vụ tự động nhận diện kho/chi nhánh theo GPS của thiết bị.
  - **Phân quyền RBAC**: `GET /roles/{id}/permissions` xác định các quyền hạn được kích hoạt (Xem, Thêm, Sửa, Xóa, Phê duyệt).

---

### 2.2. Module Sản Phẩm, Danh Mục & Lô Hàng (Product, Category & Lot)
* **API Controller**: `EMarket.ApiControllers.Admin.ProductAdminApiController`
* **Route Prefix**: `/api/admin/product-management`
* **Nghiệp vụ cốt lõi**:
  - **Sản phẩm (Product)**: Quản lý SKU, mã vạch (Barcode), tên, giá vốn (`CostPrice`), giá bán (`Price`), đơn vị tính (`Unit`), thuế VAT, ảnh đại diện và bộ sưu tập ảnh phụ (`ProductImageDTO`).
  - **Ngưỡng tồn an toàn (Safety Stock)**: `MinStock` (ngưỡng tối thiểu để cảnh báo nhập hàng) và `MaxStock` (ngưỡng tối đa tránh ứ đọng vốn).
  - **Trạng thái sản phẩm**: Active (`IsActive = true`) và Inactive (`/products/inactive`). Cho phép tái kích hoạt (`POST /products/active?productId=...&minStock=...&maxStock=...`).
  - **Cảnh báo hết hàng**: `GET /products/low-stock-alerts?top=10` lấy danh sách sản phẩm có tồn kho thấp hơn ngưỡng `MinStock`.
  - **Lô sản phẩm (Product Lot / Batch)**: Phục vụ quản lý hạn sử dụng (FEFO/FIFO). Mỗi lô lưu: `LotNumber`, `ManufactureDate`, `ExpiryDate`, `Quantity`, `CostPrice`.

---

### 2.3. Module Kho Hàng & Biến Động Tồn Kho (Warehouse & Stock Movement)
* **API Controller**: `EMarket.ApiControllers.Admin.InventoryAdminApiController`
* **Route Prefix**: `/api/admin/inventory`
* **Nghiệp vụ cốt lõi**:
  - **Kho theo Chi Nhánh**: Một chi nhánh có thể sở hữu một hoặc nhiều kho vật lý (`GET /warehouses/by-branch/{branchId}`).
  - **Tồn kho thực tế (Actual Stock)**: `GET /stock/total-actual?productId={id}&warehouseId={id}` lấy số lượng thực tế hiện tại.
  - **Nhật ký biến động kho (Stock Movements Audit)**: Bắt buộc ghi nhận mọi hành vi làm thay đổi tồn kho (`GET /stock/movements` có phân trang):
    - `IN`: Nhập hàng từ nhà cung cấp hoặc điều chuyển đến.
    - `OUT`: Xuất bán hàng (qua POS/Order) hoặc trả hàng NCC.
    - `ADJUST`: Kiểm kê, hao hụt, hư hỏng.
  - **Điều chỉnh tồn kho (Stock Adjustment)**: `POST /stock/adjust` nhận `StockAdjustmentDTO` (ProductId, WarehouseId, OldQuantity, NewQuantity, Reason, AdjustedBy).

---

### 2.4. Module Nhập Hàng & Công Nợ Nhà Cung Cấp (Purchase Order & Supplier Debt)
* **API Controller**: `EMarket.ApiControllers.Admin.InventoryAdminApiController`
* **Route Prefix**: `/api/admin/inventory`
* **Nghiệp vụ cốt lõi**:
  - **Đơn nhập hàng (Purchase Order)**:
    - Trạng thái: `Pending` (Chờ duyệt), `Approved` (Đã duyệt), `Completed` (Đã nhập kho), `Cancelled` (Đã hủy).
    - Trạng thái thanh toán: `Unpaid` (Chưa thanh toán), `Partial` (Thanh toán một phần), `Paid` (Đã thanh toán đủ).
    - Khi PO chuyển sang `Completed`: Hệ thống tự động tạo bản ghi tăng tồn kho và tự động ghi nhận một khoản Công Nợ Nhà Cung Cấp (`SupplierDebt`).
  - **Quản lý công nợ NCC (Supplier Debt)**:
    - `TotalAmount`: Tổng tiền đơn hàng.
    - `PaidAmount`: Tổng số tiền đã thanh toán cho NCC.
    - `UnpaidAmount`: Số tiền còn nợ (`TotalAmount - PaidAmount`).
    - `DueDate`: Ngày đến hạn trả nợ.
    - Phân loại cảnh báo: Công nợ sắp đến hạn (`GET /debt/near-due?daysBefore=7`) và Công nợ quá hạn (`GET /debt/overdue`).
  - **Thanh toán công nợ**: `POST /debt/payment` ghi nhận phiếu chi trả nợ NCC. Cập nhật ngay `PaidAmount` và `UnpaidAmount`. Không cho phép trả quá số nợ còn lại (`PaidAmount + NewPayment <= TotalAmount`).

---

### 2.5. Module Bán Hàng, Thu Ngân & POS (Sales, Cashier & Checkout)
* **API Controller**: `EMarket.ApiControllers.Admin.SalesAdminApiController`
* **Route Prefix**: `/api/admin/sales`
* **Nghiệp vụ cốt lõi**:
  - **Quy trình Checkout tập trung qua Stored Procedure**:
    - Gọi API `POST /orders/checkout` nhận `CheckoutRequestDTO`.
    - Dưới backend, toàn bộ nghiệp vụ chạy trong transaction SQL `sp_Sales_Checkout` sử dụng Table-Valued Parameter `dbo.OrderDetailType`.
    - Các bước SP thực thi nguyên tử (Atomic):
      1. Kiểm tra tồn kho từng mặt hàng trong giỏ.
      2. Tạo hóa đơn `Orders` và chi tiết `OrderDetails`.
      3. Trừ tồn kho tương ứng trong kho hàng chi nhánh.
      4. Khấu trừ điểm tích lũy khách sử dụng (`PointsUsed`) và tích điểm mới (`PointsEarned`).
      5. Nếu là đơn hoàn/trả hàng (`ParentOrderId != null`), kiểm tra không cho phép trả vượt số lượng đã mua (Bắt lỗi SQL `50003`).
      6. Kích hoạt SignalR thông báo toàn hệ thống qua `NotifyOrderCreatedAsync`.
  - **Tích hợp Cổng thanh toán tự động (PayOS QR)**:
    - Gọi `POST /orders/create-payos-link` tạo mã VietQR động theo chuẩn Napas247.
    - Kiểm tra trạng thái: `GET /orders/check-payos-status/{orderCode}` hoặc nhận tín hiệu thời gian thực từ SignalR khi Webhook PayOS khớp tiền.
  - **Đơn đổi trả / Hoàn tiền**: Bắt buộc liên kết với `ParentOrderId`. Nếu số lượng hoàn vượt quá thực mua, Server trả về lỗi SQL `50003`, Mobile phải hiển thị thông báo nghiệp vụ rõ ràng cho thu ngân.

---

### 2.6. Module Khách Hàng, Điểm Thưởng & Khuyến Mãi (Customer, Loyalty & Promotion)
* **API Controller**: `EMarket.ApiControllers.Admin.CustomerAdminApiController`
* **Route Prefix**: `/api/admin/customer`
* **Nghiệp vụ cốt lõi**:
  - **Khách hàng**: Thông tin cá nhân, số điện thoại VN chuẩn, email, danh sách địa chỉ nhận hàng (`CustomerAddressDTO`).
  - **Phân khúc khách hàng**: Vãng lai, Tiềm năng, Thường xuyên, VIP.
  - **Hệ thống Điểm thưởng (Loyalty Program)**:
    - Tỷ lệ tích điểm: Lấy từ `SystemConfig` (`PointEarnedRate`, ví dụ: tiêu 100.000 VNĐ được 1 điểm).
    - Tỷ lệ tiêu điểm: Lấy từ `SystemConfig` (`PointExchangeRate`, ví dụ: 1 điểm = 1.000 VNĐ).
    - Giảm giá thành viên: `MemberDiscount` (%) và `VIPDiscount` (%).
  - **Chương trình khuyến mãi (Promotion)**:
    - Loại giảm giá: Giảm theo % (`Percentage`) có mức giảm tối đa, hoặc Giảm số tiền cố định (`FixedAmount`).
    - Điều kiện áp dụng: Giá trị đơn hàng tối thiểu (`MinOrderAmount`), danh mục áp dụng, thời gian bắt đầu - kết thúc (`FromDate`, `ToDate`).

---

### 2.7. Module Sổ Thu Chi & Phiếu Chi (Expense Management)
* **API Controller**: `EMarket.ApiControllers.Admin.ExpenseAdminApiController`
* **Route Prefix**: `/api/admin/expense`
* **Nghiệp vụ cốt lõi**:
  - **Mục đích**: Theo dõi toàn bộ dòng tiền ra của từng chi nhánh (tiền thuê mặt bằng, điện nước, lương thưởng, mua sắm công cụ dụng cụ).
  - **Danh mục chi phí (Expense Category)**: Phân nhóm rõ ràng (Chi phí vận hành, Chi phí marketing, v.v.).
  - **Vòng đời phiếu chi (Expense Lifecycle)**:
    - `Pending` (Chờ duyệt): Nhân viên lập phiếu chi.
    - `Approved` (Đã duyệt): Quản lý duyệt phiếu chi, tiền được tính vào tổng chi thực tế.
    - `Rejected` (Từ chối): Bắt buộc nhập lý do từ chối (`RejectReason`).
  - **API Thống kê nhanh**: `GET /summary?branchId=...` trả về tổng chi phí, tổng số tiền đã duyệt, đang chờ và đã từ chối.

---

### 2.8. Module Báo Giá & Chuyển Đổi Đơn Hàng (Quotation)
* **API Controller**: `EMarket.ApiControllers.Admin.QuotationAdminApiController`
* **Route Prefix**: `/api/admin/quotation`
* **Nghiệp vụ cốt lõi**:
  - **Lập báo giá**: Dành cho khách hàng B2B hoặc đơn hàng số lượng lớn cần duyệt giá trước.
  - **Trạng thái báo giá**: `Draft`, `Sent`, `Approved`, `Rejected`, `Expired`.
  - **Chuyển đổi thành Đơn hàng (Convert To Order)**:
    - Endpoint: `POST /{id}/convert-to-order?userId={id}`.
    - Nghiệp vụ: Khi khách chấp nhận báo giá, một nút "Chuyển thành Đơn hàng" sẽ gọi API này để tự động sinh ra `Order` kèm đầy đủ chi tiết mặt hàng, giúp nhân viên không phải gõ lại từ đầu.

---

### 2.9. Module Báo Cáo & Thống Kê Dashboard (Dashboard & Analytics)
* **API Controller**: `EMarket.ApiControllers.Admin.DashboardAdminApiController`
* **Route Prefix**: `/api/admin/dashboard`
* **Nghiệp vụ cốt lõi (Tất cả đều là Aggregation Endpoint)**:
  - `GET /summary`: Thống kê tổng hợp (Doanh thu, Số đơn, Khách mới, Tồn kho) gom theo ngày (`day`) hoặc tháng (`month`).
  - `GET /branch-performance`: So sánh hiệu suất bán hàng giữa các chi nhánh.
  - `GET /warehouse`: Báo cáo nhập - xuất - tồn và tốc độ luân chuyển kho.
  - `GET /finance`: Báo cáo tài chính, lợi nhuận gộp, dòng tiền thu - chi.
  - `GET /debt`: Báo cáo tổng nợ phải trả cho nhà cung cấp.
  - `GET /super-admin`: Chỉ dành cho tài khoản Admin cấp cao, bao quát chỉ số toàn hệ sinh thái.

---

### 2.10. Module Trí Tuệ Nhân Tạo & Dự Báo (AI Forecast & Analytics)
* **API Controller**: `EMarket.ApiControllers.Admin.AIAnalysisApiController`
* **Route Prefix**: `/api/admin/ai-analysis`
* **Nghiệp vụ cốt lõi**:
  - **Đề xuất nhập hàng (Replenishment Advice)**: `GET /replenishment-advice/{branchId}` trả về danh sách sản phẩm cần nhập kèm số lượng đề xuất do thuật toán AI tính toán dựa trên tốc độ bán thực tế và tồn kho.
  - **Cảnh báo bất thường ngành hàng (Anomaly Detection)**: `GET /anomalies/{branchId}` phát hiện các đợt tăng/giảm đột biến (Spike/Dip).
  - **Hàng tồn đọng khó bán (Deadstock Analysis)**: `GET /deadstock/{branchId}` phát hiện sản phẩm có nguy cơ tồn đọng vốn lâu hoặc sắp hết hạn.
  - **Rủi ro tài chính theo hạn dùng (FEFO Lot-level Risk)**: `GET /lot-financial-risk/{branchId}` ước lượng tổng số tiền rủi ro nếu các lô sản phẩm cận date không kịp bán.
  - **Dự báo nhu cầu bán hàng**: `GET /sales-forecast/{productId}/{branchId}` trả về dữ liệu chuỗi thời gian dự báo cho từng sản phẩm.

---

### 2.11. Module Cấu Hình Hệ Thống & Nhật Ký Kiểm Toán (System Config & Audit Logs)
* **API Controllers**: `SystemConfigAdminApiController`, `LogAdminApiController`
* **Nghiệp vụ cốt lõi**:
  - **Cấu hình động**: `GET /api/admin/system-config` cung cấp:
    - Thuế suất VAT chung (`VAT`, mặc định 10% hoặc 8%).
    - Thông tin chuyển khoản QR ngân hàng (`BankID`, `BankNum`).
    - Điểm thưởng (`PointEarnedRate`, `PointExchangeRate`).
    - Giảm giá thành viên (`VipDiscount`, `MemberDiscount`).
    - Các cờ tính năng (`EmailEnabled`, `TelegramEnabled`).
  - **Nhật ký kiểm toán (Audit Logs)**: Ghi lại `Ai làm gì? Lúc nào? Trên bản ghi nào? Giá trị cũ và mới là gì?` để bảo vệ tính minh bạch dữ liệu.

---

# 3. CHUẨN KIẾN TRÚC ỨNG DỤNG ANDROID NATIVE (JAVA)

Mọi dự án Android Native viết bằng Java (`Do_An_Di_Dong`) phải tuân thủ cấu trúc MVVM chuẩn mực sau:

```
com.example.do_an_di_dong
│
├── data
│   ├── api                  # Retrofit ApiService, RetrofitClient, AuthInterceptor
│   ├── model                # DTOs khớp chính xác backend (ProductDTO, CheckoutRequest, etc.)
│   ├── repository           # Repository phân phối dữ liệu (ProductRepository, OrderRepository)
│   ├── realtime             # SignalRRealtimeClient (OkHttp WebSocket)
│   └── database             # SQLite / Room lưu cache offline & thông tin phiên đăng nhập
│
├── ui
│   ├── base                 # BaseActivity, BaseFragment, BaseViewModel, UiState
│   ├── auth                 # Đăng nhập, Google Sign-in, Quên mật khẩu
│   ├── pos                  # Bán hàng, giỏ hàng, chọn sản phẩm, quét mã, thanh toán QR
│   ├── product              # Danh sách sản phẩm, chi tiết, quản lý lô, ảnh
│   ├── inventory            # Quản lý kho, kiểm kho, biến động tồn
│   ├── debt                 # Công nợ NCC, lịch sử trả tiền, widget nhắc nợ
│   ├── expense              # Quản lý phiếu chi, duyệt phiếu
│   ├── quotation            # Báo giá, chuyển thành đơn hàng
│   └── dashboard            # Biểu đồ doanh thu, báo cáo tài chính, AI Insights
│
└── util                     # CurrencyFormatter, DateUtils, DebounceClickListener, ViewExtensions
```

### 3.1. Mô Hình Hóa Trạng Thái Giao Diện (UiState Pattern)
Mọi `ViewModel` trong Java phải quản lý trạng thái màn hình bằng `LiveData<UiState<T>>` để đảm bảo xử lý đầy đủ các trường hợp:
```java
public class UiState<T> {
    public enum Status { LOADING, SUCCESS, EMPTY, ERROR }

    private final Status status;
    private final T data;
    private final String errorMessage;

    public static <T> UiState<T> loading() {
        return new UiState<>(Status.LOADING, null, null);
    }
    public static <T> UiState<T> success(T data) {
        return new UiState<>(Status.SUCCESS, data, null);
    }
    public static <T> UiState<T> empty() {
        return new UiState<>(Status.EMPTY, null, null);
    }
    public static <T> UiState<T> error(String message) {
        return new UiState<>(Status.ERROR, null, message);
    }
    // Getters
}
```

### 3.2. Quy Định Về Retrofit & Interceptor (Network Layer)
* Phải có `AuthInterceptor` tự động gắn `Authorization: Bearer <token>` vào tất cả request.
* Phải có `HttpLoggingInterceptor` (chỉ bật ở môi trường DEBUG).
* Bắt mã HTTP 401: Khi token hết hạn, Interceptor phải xóa phiên làm việc và gửi thông báo hoặc broadcast để đưa người dùng về màn hình Login.

### 3.3. Quy Định Về Gson & Model Mapping
* Luôn sử dụng `@SerializedName("tên_thuộc_tính_backend")`.
* **Tuyệt đối không để 2 trường Java cùng trỏ vào 1 `@SerializedName`** gây xung đột Gson.
* Các trường số tiền phải dùng `BigDecimal` hoặc `Double` (ưu tiên `BigDecimal` khi tính toán cộng trừ nhân chia trong giỏ hàng).

---

# 4. CHUẨN KIẾN TRÚC ỨNG DỤNG CROSS-PLATFORM (FLUTTER)

Khi phát triển ứng dụng di động EMarket bằng **Flutter**, bắt buộc áp dụng cấu trúc **Clean Architecture theo Feature-First**:

```
lib/
├── core/
│   ├── constants/           # ApiEndpoints, AppColors, AppStrings
│   ├── network/             # DioClient, AuthInterceptor, ErrorHandler
│   ├── theme/               # Material 3 Light/Dark Theme, Typography
│   ├── utils/               # CurrencyFormatters, DateFormatters, Validators
│   └── widgets/             # StateViews (Loading, ErrorState, EmptyState), CustomButton
│
└── features/
    ├── auth/                # Login, Google Auth, OTP, User Session
    ├── pos_checkout/        # Bán hàng POS, Giỏ hàng, Quét Barcode, PayOS QR
    ├── product_inventory/   # Sản phẩm, Lô hàng, Danh mục, Tồn kho, Biến động
    ├── supplier_debt/       # Đơn nhập hàng, Công nợ NCC, Thanh toán nợ
    ├── expense/             # Quản lý phiếu chi, duyệt/từ chối chi phí
    ├── quotation/           # Lập báo giá, chuyển đổi đơn hàng
    ├── dashboard_ai/        # Biểu đồ doanh thu, AI Forecast, Replenishment Advice
    └── system_logs/         # Cấu hình VAT/Điểm, Nhật ký Audit Logs
        ├── data/
        │   ├── datasources/ # RemoteDataSource (Dio), LocalDataSource (Storage)
        │   ├── models/      # DTOs kèm fromJson / toJson (freezed / json_serializable)
        │   └── repositories/# Triển khai Repository tương ứng
        ├── domain/
        │   ├── entities/    # Thực thể nghiệp vụ thuần túy
        │   └── repositories/# Abstract Repository Interface
        └── presentation/
            ├── controllers/ # StateNotifier / BLoC / Cubit điều phối trạng thái
            └── views/       # Các Screen, BottomSheet, Component Widget
```

### 4.1. Quản Lý State (State Management)
* Sử dụng **Riverpod** (`AsyncValue<T>`) hoặc **BLoC/Cubit**:
  - `AsyncLoading()`: Hiển thị Shimmer / Spinner.
  - `AsyncData(data)`: Hiển thị dữ liệu. Nếu danh sách rỗng (`data.isEmpty`), hiển thị `EmptyWidget`.
  - `AsyncError(err, stack)`: Hiển thị thông báo lỗi thân thiện kèm nút **Thử lại (Retry)**.
* Tuyệt đối không gọi trực tiếp API trong hàm `initState` của `StatefulWidget` mà không thông qua Controller/ViewModel.

### 4.2. Cấu Hình Dio & Network Layer
* Cấu hình Timeout rõ ràng: `connectTimeout: 15s`, `receiveTimeout: 20s`.
* Triển khai `QueuedInterceptorsWrapper` để xử lý Bearer JWT:
  ```dart
  options.headers['Authorization'] = 'Bearer $token';
  ```
* Bắt ngoại lệ `DioException` và ánh xạ sang lỗi miền nghiệp vụ (`FailureMessage`), không bao giờ để lộ Exception thô hoặc văng màn hình đỏ (Red Screen of Death).

### 4.3. Lưu Trữ Dữ Liệu Cục Bộ An Toàn
* Token JWT và thông tin tài khoản: Bắt buộc dùng `flutter_secure_storage`.
* Cache danh mục, chi nhánh, cấu hình VAT: Dùng `shared_preferences` hoặc `hive`.

---

# 5. BẢNG ĐỐI CHIẾU TRIỂN KHAI KỸ THUẬT: JAVA VS. FLUTTER

Để đảm bảo tính đồng nhất 100% về nghiệp vụ giữa hai nền tảng, các thành phần kỹ thuật được quy ước tương đương như sau:

| Khía Cạnh | Chuẩn Android Native (Java) | Chuẩn Cross-Platform (Flutter) | Ghi Chú Ràng Buộc Nghiệp Vụ |
| :--- | :--- | :--- | :--- |
| **Mô hình kiến trúc** | MVVM (Model-View-ViewModel) | Clean Architecture (Feature-first) | View không chứa logic tính toán tiền/thuế |
| **Xác thực HTTP** | Retrofit 2 + OkHttp Interceptor | Dio + QueuedInterceptor | Tự động thêm `Bearer <JWT_TOKEN>` |
| **Quản lý State** | LiveData / StateFlow `<UiState<T>>` | Riverpod (`AsyncValue`) / BLoC | Bắt buộc đủ 4 trạng thái: Load, Success, Empty, Error |
| **Mô hình hóa dữ liệu** | POJO + `@SerializedName` (Gson) | Dart Class + `json_serializable` | Key JSON phải khớp 100% C# DTO |
| **Lưu Token an toàn** | `EncryptedSharedPreferences` | `FlutterSecureStorage` | Không lưu Token ở dạng PlainText |
| **Quét mã vạch (POS)** | ZXing Android Embedded / ML Kit | `mobile_scanner` / `qr_code_scanner` | Tự động điền sản phẩm vào giỏ khi quét trúng |
| **Tải & Cache ảnh** | Glide / Picasso | `cached_network_image` | Có ảnh fallback khi URL null hoặc mạng lỗi |
| **Xử lý số tiền tệ** | `BigDecimal` + `DecimalFormat` | `Decimal` / `num` + `intl` NumberFormat | Định dạng hiển thị chuẩn VNĐ (VD: 120.000 đ) |
| **Kết nối Realtime** | OkHttp WebSocket / SignalR Client | `web_socket_channel` / SignalR client | Lắng nghe event `orderChanged` từ `OrderHub` |
| **Xử lý bất đồng bộ** | `CompletableFuture` / `Executor` | `async` / `await` / `Future` | Luôn có khối `try - catch` hoặc functional error |

---

# 6. CHUẨN TƯƠNG TÁC API, BIG DATA & PHÂN TRANG SERVER-SIDE

### 6.1. Quy Tắc Nghiêm Cấm "GET ALL"
* **Tuyệt đối cấm gọi các API lấy toàn bộ danh sách không giới hạn** trên các tập dữ liệu có thể tăng trưởng theo thời gian:
  - Danh sách đơn hàng (`Orders`).
  - Danh sách chi tiết đơn hàng (`OrderDetails`).
  - Nhật ký biến động kho (`StockMovements`).
  - Lịch sử công nợ và phiếu chi (`Debts`, `Expenses`).
  - Nhật ký hệ thống (`AuditLogs`).
* Việc tải hàng nghìn bản ghi về thiết bị di động sẽ gây tràn bộ nhớ RAM (OOM Crash), tốn dung lượng 4G/5G và làm nghẽn Server EMarket.

### 6.2. Quy Chuẩn Phân Trang (Server-Side Pagination)
Khi gọi danh sách, bắt buộc truyền các tham số phân trang chuẩn của hệ thống EMarket:
* `start`: Vị trí bắt đầu của bản ghi (bắt đầu từ 0).
* `length`: Số lượng bản ghi cần lấy (mặc định 10 hoặc 20, tối đa không quá 100).
* `keyword`: Từ khóa tìm kiếm (nếu có).
* `fromDate` & `toDate`: Khoảng thời gian lọc theo định dạng ISO-8601 (`yyyy-MM-dd`).

Cấu trúc response phân trang trả về từ Server:
```json
{
  "total": 150,
  "filtered": 150,
  "data": [ ... danh sách các items ... ]
}
```
* **Phía Client**: Phải hiển thị cơ chế cuộn vô tận (Infinite Scroll / Load More) hoặc phân trang trang số rõ ràng.

---

# 7. BẢO MẬT, TOKEN JWT & CƠ CHẾ PHÂN QUYỀN (RBAC)

### 7.1. Chu Kỳ Của Token
1. **Lấy Token**: Sau khi gọi `POST /api/admin/user-management/auth/login` hoặc `external-login`, Server trả về chuỗi JWT Token có thời hạn (thường 24 giờ).
2. **Lưu trữ**: Lưu ngay vào vùng nhớ an toàn (`EncryptedSharedPreferences` trên Java, `FlutterSecureStorage` trên Flutter).
3. **Sử dụng**: Mọi request sau đó đều kèm header:
   ```http
   Authorization: Bearer <TOKEN_STRING>
   ```
4. **Hết hạn (401 Unauthorized)**:
   - Client phải tự động hủy Token trong kho lưu trữ.
   - Hiển thị thông báo: `"Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại."`
   - Điều hướng người dùng về màn hình đăng nhập, xóa sạch ngăn xếp Navigation (Clear Task / PushAndRemoveUntil).

### 7.2. Phân Quyền Vai Trò (Role-Based Access Control - RBAC)
* Mobile UI phải ẩn hoặc vô hiệu hóa các nút chức năng nếu quyền của người dùng không thỏa mãn:
  - **Thu ngân (Cashier)**: Chỉ truy cập màn hình Bán hàng (POS), tạo đơn, xem hóa đơn chi nhánh của mình. Không được sửa giá vốn sản phẩm, không được xem Dashboard tài chính hoặc duyệt phiếu chi.
  - **Thủ kho (Warehouse Keeper)**: Truy cập Kho hàng, Kiểm kho, Đơn nhập hàng. Không truy cập Báo cáo tài chính cấp cao.
  - **Quản lý chi nhánh (Manager)**: Quản lý toàn diện trong phạm vi chi nhánh (`BranchId`).
  - **Super Admin**: Quyền cao nhất, được phép xem dữ liệu tất cả chi nhánh và truy cập `Super Admin Hub`.

### 7.3. Phòng Chống Payload Độc Hại
Backend EMarket có cơ chế kiểm duyệt bảo mật tự động `IsDangerous(body)` trong `Startup.cs` chặn đứng các từ khóa XSS & SQLi:
`<script`, `javascript:`, `onerror=`, `drop table`, `delete from`.
* **Phía Client**: Cần sanitize dữ liệu, không cho phép người dùng vô tình nhập các chuỗi mã lệnh lạ có thể khiến request bị Server chặn với mã HTTP 400 (`Security Violation: Dangerous Body`).

---

# 8. CHUẨN ĐỒNG BỘ THỜI GIAN THỰC (REALTIME SIGNALR & WEBSOCKET)

Hệ thống EMarket sử dụng SignalR Hub tại `/signalr` (hoặc `/orderHub`) để cập nhật trạng thái đơn hàng tức thời giữa máy thu ngân, web admin và các thiết bị di động.

### 8.1. Cơ Chế Kết Nối
Khi người dùng đăng nhập thành công vào app:
1. Mở kết nối WebSocket/SignalR tới server với query params:
   ```
   /signalr/connect?branchId={CURRENT_BRANCH_ID}&isAdmin={IS_ADMIN}
   ```
2. Hub backend sẽ tự động phân bổ client vào các nhóm:
   - `BRANCH_{branchId}`: Nhận các biến động thuộc chi nhánh đó.
   - `ADMIN_ALL`: Nếu là Admin, nhận thông báo toàn hệ thống.

### 8.2. Xử Lý Sự Kiện `orderChanged`
Khi nhận được event `orderChanged` từ server:
```json
{
  "orderId": 1234,
  "status": "Completed",
  "branchId": 1,
  "serverTime": "15:30:20"
}
```
* Nếu người dùng đang ở màn hình Danh sách Đơn hàng: Tự động tải lại trang hoặc chèn đơn mới vào đầu danh sách với hiệu ứng sinh động.
* Nếu đang ở màn hình Bán hàng POS chờ khách chuyển khoản PayOS: Tự động đóng dialog chờ thanh toán, hiển thị thông báo `"Thanh toán đơn #1234 thành công!"`, và in hóa đơn (nếu có kết nối máy in).

### 8.3. Quản Lý Vòng Đời Kết Nối
* Khi app vào chế độ nền (`Background / Paused`): Giảm tải hoặc đóng tạm kết nối để tiết kiệm pin.
* Khi app quay lại tiền cảnh (`Foreground / Resumed`): Kiểm tra và kết nối lại ngay lập tức.

---

# 9. XỬ LÝ DỮ LIỆU TÀI CHÍNH & SỐ THẬP PHÂN (DECIMAL SAFETY)

Trong hệ thống EMarket, các nghiệp vụ Bán hàng, Công nợ, Tích điểm và Chi phí liên quan trực tiếp đến tiền tệ.

### 9.1. Tránh Lỗi Sai Số Dấu Phẩy Động (Floating Point Inaccuracy)
* **Tuyệt đối không dùng kiểu `float` hoặc `double` sơ sài để cộng trừ tiền tệ dồn tích** (VD: `0.1 + 0.2 = 0.30000000000000004`).
* **Android Java**: Sử dụng `java.math.BigDecimal` cho mọi phép tính giỏ hàng, chiết khấu, thuế VAT và tổng tiền.
* **Flutter**: Sử dụng package `decimal` hoặc giữ số nguyên đơn vị tiền tệ cơ bản trước khi format.

### 9.2. Công Thức Tính Tiền Đơn Hàng Chuẩn Parity Với Backend
Mọi phép tính giỏ hàng POS trên Mobile phải tuân thủ đúng trình tự sau:
```
1. Tiền_Từng_Món = (Đơn_Giá * Số_Lượng) - Chiết_Khấu_Món
2. Tổng_Tiền_Hàng = Tổng (Tiền_Từng_Món)
3. Tiền_Giảm_VIP_Member = Tổng_Tiền_Hàng * (Tỷ_Lệ_Giảm_Hạng / 100)
4. Tiền_Giảm_Khuyến_Mãi = Tính theo Promotion hợp lệ
5. Tiền_Trừ_Điểm = Số_Điểm_Muốn_Đổi * PointExchangeRate
6. Tiền_Giảm_Thủ_Công = ManualDiscount (nếu có lý do hợp lệ)
7. Tiền_Chưa_Thuế = Max(0, Tổng_Tiền_Hàng - Tiền_Giảm_VIP_Member - Tiền_Giảm_Khuyến_Mãi - Tiền_Trừ_Điểm - Tiền_Giảm_Thủ_Công)
8. Tiền_VAT = Tiền_Chưa_Thuế * (VAT_Rate / 100) (nếu cấu hình VAT bật)
9. Tổng_Thanh_Toán = Tiền_Chưa_Thuế + Tiền_VAT
10. Điểm_Tích_Lũy_Mới = Floor(Tổng_Thanh_Toán / PointEarnedRate)
```

---

# 10. QUY TRÌNH PHÁT TRIỂN TÍNH NĂNG (FEATURE WORKFLOW)

Mọi lập trình viên khi tiếp nhận hoặc nâng cấp một tính năng trên Mobile (Java/Flutter) phải thực hiện nghiêm ngặt quy trình 10 bước sau:

```
[1. Khảo Sát Source Web/Backend]
         │ (Đọc Controller, Service, DTO, Stored Procedure tương ứng)
         ▼
[2. Xác Định Ràng Buộc Nghiệp Vụ & Validation]
         │ (Kiểm tra điều kiện bắt buộc, mã lỗi, trạng thái giao dịch)
         ▼
[3. Xác Định Contract API & DTO]
         │ (HTTP Method, URL, Query/Body, Headers, Response format)
         ▼
[4. Thiết Kế UX Di Động Thân Thiện]
         │ (Tối ưu cảm ứng, bàn phím, bottom sheet, không bê nguyên Table web)
         ▼
[5. Xây Dựng Data Layer (Repository & ApiService)]
         │ (Tạo Model DTO, hàm gọi API, map dữ liệu an toàn)
         ▼
[6. Xây Dựng Presentation Layer (ViewModel / Controller)]
         │ (Quản lý UiState: Loading, Success, Empty, Error)
         ▼
[7. Xây Dựng UI & Binding]
         │ (Render giao diện, gắn sự kiện, xử lý định dạng tiền tệ/ngày tháng)
         ▼
[8. Xử Lý Các Trường Hợp Biên (Edge Cases)]
         │ (Mạng yếu, mạng ngắt, server lỗi 500, token hết hạn 401, danh sách rỗng)
         ▼
[9. Kiểm Thử Đối Chiếu Nghiệp Vụ Với Web]
         │ (Tạo dữ liệu trên Mobile -> Kiểm tra Web xem có xuất hiện chính xác không)
         ▼
[10. Nghiệm Thu Theo Definition of Done]
```

---

# 11. TIÊU CHUẨN HOÀN THÀNH (DEFINITION OF DONE - DOD)

Một tính năng mobile (dù phát triển bằng Java hay Flutter) **CHỈ ĐƯỢC COI LÀ HOÀN THÀNH** khi vượt qua toàn bộ danh sách kiểm tra sau:

- [ ] **Khớp Nghiệp Vụ 100%**: Luồng nghiệp vụ, tính toán tiền, tích điểm, trừ kho hoạt động chuẩn xác theo logic backend EMarket.
- [ ] **Không Fake Data / Fake Success**: Mọi ID và trạng thái thành công đều nhận về từ response thật của API.
- [ ] **Đầy Đủ 4 Trạng Thái UI**:
  - `Loading`: Có indicator / shimmer rõ ràng, không làm đơ ứng dụng.
  - `Success`: Hiển thị dữ liệu chính xác, cập nhật kịp thời.
  - `Empty`: Khi không có dữ liệu, hiển thị hình minh họa và thông báo hướng dẫn (không để màn hình trắng trơn).
  - `Error`: Khi có sự cố, hiển thị thông báo lỗi dễ hiểu và có nút **Thử lại (Retry)**.
- [ ] **Không Crash Ứng Dụng**: Đã kiểm tra và xử lý an toàn các trường hợp dữ liệu `null`, rỗng, mất kết nối mạng đột ngột hoặc timeout.
- [ ] **Không Lạm Dụng Bộ Nhớ**: Các danh sách lớn đều được phân trang (Server-side Pagination); không gọi `GET ALL` vô tội vạ.
- [ ] **Bảo Mật An Toàn**: Token JWT được lưu trong bộ nhớ an toàn; tự động đăng xuất khi gặp lỗi 401.
- [ ] **Định Dạng Chuẩn Địa Phương**: Số tiền được format dạng VNĐ (`###,### đ`), ngày tháng hiển thị theo định dạng Việt Nam (`dd/MM/yyyy HH:mm`).
- [ ] **Bảo Toàn Trạng Thái Nhập Liệu**: Nếu gửi biểu mẫu thất bại do lỗi mạng, dữ liệu người dùng vừa nhập không bị xóa trắng.
- [ ] **Đồng Bộ Thời Gian Thực (nếu có)**: Các màn hình danh sách đơn hàng hoặc POS nhận đúng tín hiệu SignalR khi có giao dịch mới.
