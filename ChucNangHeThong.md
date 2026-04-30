# I. Phân tích Chức năng Chi tiết (Exhaustive Functional Breakdown)

Dựa trên việc kiểm tra sâu cấu trúc mã nguồn tại các `ApiControllers`, `Services`, và `Modules`, hệ thống EMarket không chỉ là ứng dụng CRUD cơ bản mà triển khai các luồng nghiệp vụ phức tạp sau:

- **1. Quản trị Kho vận & Chuỗi cung ứng (InventoryModule & ProductModule)**
  - **Theo dõi Tồn kho Đa chiều (Multi-dimensional Stock Tracking):** Hàm `GetTotalStockAsync` tổng hợp tồn kho thực tế bằng cách cộng dồn các biến động lịch sử (Stock Movements), tách bạch rạch ròi với tồn kho danh nghĩa.
  - **Quản trị Lô hàng (Product Lot Management):** API `stock/filterTime` và `IProductLotService` thực thi logic quản lý hàng hóa theo từng Lô (Lot). Hệ thống kiểm soát chéo giữa `ManufacturingDate` và `ExpiryDate` nhằm đảm bảo tính truy xuất nguồn gốc.
  - **Giám sát Biến động (Stock Movement Engine):** Hàm `GetStockMovementsDataTableAsync` cung cấp bộ lọc không gian (theo kho `warehouseId`) và thời gian, cho phép theo dõi mọi hành vi nhập/xuất/chuyển kho.
  - **Quản trị Dòng tiền Nhà cung cấp (Supplier Debt Aging):** `SupplierServiceDebtAndPaymentService` chứa các logic phân loại nợ tinh vi như `GetOverdueDebtsAsync` (Nợ quá hạn) và `GetDebtsNearDueDateAsync` (Nợ sắp đến hạn trong N ngày).

- **2. Quản trị Giao dịch & Bán hàng (SalesModule)**
  - **Xử lý Đơn hàng (Order Processing):** `SalesAdminApiController` cung cấp các API để cập nhật trạng thái đơn (Status Transition) như Duyệt, Hủy, Đã giao.
  - **Tích hợp Webhook & Real-time (PayOS & SignalR):** Khi có thanh toán, hàm `WebhookHandler` xác thực Payload từ PayOS. Nếu hợp lệ (Code == "00"), hệ thống gọi `OrderHub` qua SignalR để "bắn" thông điệp (Payload: `status = "PAID"`) trực tiếp đến nhóm Client (`Group("PAYMENT_" + orderCode)`) đang chờ, thay vì bắt UI phải polling.
  - **Cơ chế Khuyến mãi (Promotion Engine):** `IPromotionService` lọc và kích hoạt các quy tắc giảm giá dựa trên thời gian thực (`GetActivePromotionsAsync`).

- **3. Quản lý Nhân sự & Chi nhánh (UserModule)**
  - **Truy vấn Vị trí Không gian (Geospatial Search):** Hàm `GetNearestBranchAsync(lat, lng, maxDist)` tính toán khoảng cách tọa độ GPS để đề xuất chi nhánh gần nhất với khách hàng.
  - **Phân quyền Động (Dynamic RBAC):** Kiến trúc `IRoleService` móc nối giữa `Role` và `Permission IDs`, cho phép ứng dụng truy vấn và giới hạn quyền truy cập xuống cấp độ API endpoint.
  - **Thống kê tăng trưởng (HR Analytics):** Logic `GetUserStats` gom nhóm đa luồng dữ liệu (RoleDistribution, MonthlyGrowth) để vẽ biểu đồ phân bổ nhân sự.

---

# II. Điểm sáng Kiến trúc (Architectural & Technical Highlights)

- **1. Ranh giới Modular Monolith (Modular Monolith Boundaries)**
  Toàn bộ hệ thống Backend được phân mảnh thành các Domain cô lập (`InventoryModule`, `SalesModule`, `UserModule`). Ranh giới mã nguồn được quy định nghiêm ngặt: Controller không bao giờ gọi trực tiếp DbContext. Thay vào đó, chúng phụ thuộc 100% vào các Interface (ví dụ: `IOrderService`). Dependency Injection chịu trách nhiệm cấp phát Implementation tương ứng. Việc này ngăn chặn tình trạng "Spaghetti Code", giúp giới hạn vòng đời của luồng xử lý dữ liệu ngay tại Module của nó.

- **2. Sự tách biệt của REST API (REST API Decoupling)**
  Bằng cách sử dụng `ApiControllers` thuần túy trả về JSON, Core Logic hoàn toàn độc lập với Frontend. Sự hiện diện của Webhook (cho PayOS) và SignalR (cho Web/Mobile App) minh chứng cho việc các cổng giao tiếp có thể bị tháo lắp, thay đổi (chuyển từ ASP.NET MVC UI sang Vue/Blazor) mà Business Logic (`Implementations`) không phải sửa dù chỉ một dòng code.

- **3. Phân tách tác vụ Đọc/Ghi (Read/Write Segregation Pattern)**
  Dựa trên mã nguồn, Entity Framework (EF) được tin dùng cho các luồng Command (Insert/Update) nhằm tận dụng Tracking Context và quản lý Transaction (Rollback khi lỗi). Tuy nhiên, đối với các luồng Query lớn hoặc AI Analytics (`DatabasePlugin.cs`), hệ thống mở kết nối trực tiếp (`ExecuteQueryAsync`) hoặc dùng Dapper để map dữ liệu vào đối tượng ẩn danh/Flat DTO. Việc ép EF phải Load hàng chục nghìn record bảng phụ được triệt tiêu hoàn toàn, bảo toàn bộ nhớ RAM cho server.

---

# III. Phân tích Lõi AI & Tối ưu hóa Kho FEFO

- **1. Cơ sở toán học của XGBoost & Hàm Loss Poisson (`forecast_by_category.py`)**
  Bản chất dữ liệu bán lẻ tồn tại nhiều ngày có lượng bán bằng 0, không phân phối chuẩn. Nếu dùng hàm RMSE (Root Mean Square Error), mô hình AI sẽ dự báo ra các số âm hoặc thập phân.
  - **Cơ chế:** Script thiết lập tham số `objective='count:poisson'`. Nó biến bài toán dự báo thành tính toán giá trị kỳ vọng (Lambda) của một sự kiện đếm số lượng (Count Data) trong thời gian phân tán.
  - **Feature Engineering:** Mã nguồn áp dụng `ema_7` (Exponential Moving Average) để gán trọng số cao hơn cho dữ liệu gần nhất, thay vì trung bình cộng thông thường. Các biến phi tuyến tính (non-linear) như `is_payday`, `is_festive` được nội suy cứng vào logic để huấn luyện AI phản ứng với đợt bùng nổ nhu cầu. Cùng với đó, `np.random.poisson(pred_lambda)` được dùng để bơm nhiễu hạt (Noise Injection), tạo ra chuỗi ngày bán răng cưa thay vì một đường thẳng vô lý.

- **2. Thuật toán Mô phỏng FEFO (FEFO Simulation Engine)**
  Mã nguồn triển khai thuật toán tính rủi ro tồn kho tài chính (Financial Risk Provisioning) qua các bước cực kỳ sắc bén:
  - **Bước 1:** Trích xuất nhu cầu dự báo 30 n(`ProductLot`) theo chiều tăng dần của `expiry_date` (FEFO - Lô nào hết hạn trước xếp lên đầu).
  - \*\*Bước 3 - Tính Sức mua thị trường (Market Capgày (`demand_30d`) và trung bình ngày (`daily_avg`) từ mô hình XGBoost.
  - **Bước 2:** Gom nhóm và sắp xếp các lô hàng acity):\*_ Nếu lô hàng còn `X` ngày hết hạn, `max_market_capacity` = `daily_avg` _ `X`. (Lượng tối đa thị trường có thể tiêu thụ trước khi lô này hỏng).
  - **Bước 4 - Xác định Rủi ro (Risk Qty):** Nếu `lot_qty > max_market_capacity`, phần dư ra chính là `risk_qty`.
  - **Bước 5 - Trích lập (Provisioning Value):** Hệ thống lấy `risk_qty` nhân với giá vốn (`cost_price`) của chính lô hàng đó để ra con số thiệt hại tài chính. Biến `remaining_demand` bị trừ dần (offset) qua từng vòng lặp, mô phỏng chính xác quá trình "rút ruột" từng lô trong thực tế.

- **3. Vòng lặp Tự phục hồi NL2SQL (The Self-Healing Loop)**
  Phân tích từ `ChatController.cs` (AIAssistantService), hệ thống không chỉ gọi API OpenAI một lần mà xây dựng một kiến trúc State Machine vòng lặp (Max Retries = 3):
  - **Giai đoạn 1 (Architect):** LLM sinh ra định dạng JSON chứa cấu trúc `Sql` và `Thought` dựa trên Prompts `SqlGenerator` (được chèn Schema bảng động dựa theo Regex matching trong câu hỏi User).
  - **Giai đoạn 2 (Engine):** Lớp `DatabasePlugin` gọi `ExecuteQueryAsync(Sql)`.
  - **Giai đoạn 3 (Self-Healing):** Nếu câu lệnh sai cú pháp hoặc sai tên cột, ngoại lệ `SqlException` bị văng ra. Thay vì báo lỗi cho khách, khối `catch` chặn ngoại lệ, lấy `ex.Message` gán vào biến `lastError`.
  - **Giai đoạn 4 (Feedback):** Hệ thống mở lại ngữ cảnh Chat, đóng vai Assistant trả lại chính câu SQL sai, sau đó đóng vai User gửi prompt `SqlFixer` kèm theo `lastError` yêu cầu LLM sửa lại. Vòng lặp chỉ kết thúc khi luồng Engine thực thi thành công, hoặc hết 3 lần thử nghiệm.
