# EMarket Management System - ERP & Retail Core Platform.

## 1. Tiêu đề & Tổng quan (Title & Executive Summary)

Hệ thống **EMarket Management System** là một giải pháp hoạch định nguồn lực doanh nghiệp (ERP) và lõi quản lý bán lẻ toàn diện, được xây dựng trên nền tảng .NET Framework. Đây là một hệ thống thiết kế với kiến trúc hướng phân hệ (Modular Design) nhằm giải bài toán vận hành chuỗi đa chi nhánh (multi-branch operations) trong thực tiễn. Dựa trên việc phân tích mã nguồn thực tế tại hệ thống, EMarket cung cấp các luồng nghiệp vụ khép kín từ quản lý nhà cung cấp, kiểm soát biến động tồn kho chi tiết đến từng lô hàng (Lot/Batch), cho đến xử lý giao dịch tại quầy (POS) và tổng hợp báo cáo bằng các công cụ truy vấn hiệu năng cao.

Hệ thống đóng vai trò như một môi trường dữ liệu đồng nhất, ngăn chặn triệt để tình trạng thất thoát hàng hóa, cảnh báo các rủi ro tồn kho (điển hình như hàng hết hạn hoặc tồn đọng vốn) thông qua phân tích dự báo (AI Forecast & Warning), và hỗ trợ các nhà quản trị đưa ra quyết định thông qua phân hệ Bảng điều khiển (Dashboard) theo thời gian thực.

## 2. Phân tích Kiến trúc Kỹ thuật (Technical Architecture Analysis)

Dựa trên cấu hình `packages.config` và `.csproj`, nền tảng kỹ thuật và các mẫu thiết kế (Design Patterns) của hệ thống được quy định chi tiết:

### Nền tảng Công nghệ (Tech Stack)
- **Backend Framework**: ASP.NET MVC 5 trên nền .NET Framework 4.8.1.
- **Dependency Injection (DI)**: Sử dụng **SimpleInjector** (`SimpleInjector.Integration.Web.Mvc`) để quản lý vòng đời (Lifecycle) của các dịch vụ (Services) và DbContext qua mô hình `AsyncScopedLifestyle`.
- **Database ORM & Access**: Kết hợp song song **Entity Framework 6.5.1** (xử lý CRUD, tracking và navigation properties) và **Dapper 2.1.66** (xử lý các thủ tục Stored Procedures và Queries tốc độ cao).
- **Background Jobs**: **Hangfire 1.8.22** kết hợp `Hangfire.SqlServer` và `Hangfire.SimpleInjector` cho các tác vụ lên lịch hoặc chạy nền.
- **Real-time Communication**: **SignalR 2.4.3** để cập nhật trạng thái dữ liệu hai chiều (bidirectional communication) không đồng bộ.
- **Export & Parsing**: Sử dụng **ClosedXML 0.105.0** và **Newtonsoft.Json 13.0.4** cho việc bóc tách và trích xuất dữ liệu.
- **Frontend**: jQuery, Bootstrap 5, tích hợp Ajax.

### Mẫu Thiết kế Áp dụng (Design Patterns)
- **Kiến trúc MVC & Modular phân rã (Monolithic Modular via Areas)**: Phân tách hệ thống thông qua `Areas/Admin` kết hợp các `Modules` riêng biệt (`DashboardModule`, `SalesModule`, `InventoryModule`, v.v.). Điều này duy trì tính tổ chức code (Separation of Concerns).
- **Service Layer Pattern**: Lớp Data Access và Business Logic được tách biệt sử dụng mô hình Services (ví dụ: `IDashboardService` và lớp thực thi `DashboardService`), tách biệt logic khỏi các Controllers.

### Tương tác Dữ liệu (Database Interaction Strategy)
Kiến trúc mã nguồn áp dụng **CQRS (Command Query Responsibility Segregation) theo dạng thực tiễn**:
- **Entity Framework** đóng vai trò chủ đạo trong việc quản lý các giao dịch (Command / CRUD), theo dõi thay đổi (`DbFunctions`, `AsNoTracking()`) dựa trên `EMarket_DBEntities`.
- **Dapper** (micro-ORM) được sử dụng để đọc (Query) các dữ liệu phức tạp khối lượng lớn (như trong `DashboardService.cs` thông qua `conn.QueryAsync` hoặc `conn.QueryMultipleAsync`), tránh hoàn toàn độ trễ do cơ chế Tracking của EF.

## 3. Bóc tách Nghiệp vụ Cốt lõi (Core Business Modules Highlight)

### Quản lý Tồn kho & Lô hàng (Inventory & Batch Management)
Lõi luân chuyển và quản trị trạng thái hàng hóa trực tiếp tác động vào các thực thể: `Inventories`, `ProductLots`, `Warehouses` và cơ chế `StockMovements`.
- **Triển khai tại Service**: Việc quản lý lô được hệ thống kiểm soát bằng cách ghép nối `inventory_id` và `lot_id` (`ProductLots`). Các biến động (Nhập/Xuất/Kiểm kê) đều được ghi nhận (Audit Trail) thông qua `StockMovements`, tính toán sự chênh lệch mảng giá trị (quantity < 0 với xuất, > 0 với nhập).
- **Cảnh báo hết hạn thuật toán theo ngày**: Quét đệ quy `i.ProductLot.expiry_date <= warningDate` trên cấu trúc hàng đợi, phân nhóm báo nguy hiểm (Expired) hoặc cảnh báo (Expiring Soon). Tích điểm các yếu tố này thông qua `AI_InventoryWarning` (tính Score).

### Xử lý Giao dịch (Sales/POS)
Flow xử lý đơn hàng tại POS (`OrderService`) tuân thủ tính ACID trong cơ sở dữ liệu:
- Tính toán dựa trên danh sách `OrderDetails`, tính chiết khấu linh hoạt trên tổng thể đơn theo các chiến lược định giá.
- Hệ thống áp dụng **Trừ kho theo thời gian thực (Real-time Exact Deduction)**: Duyệt tuần tự các lô (Lot) liên quan đến một `productId`, ưu tiên trừ lô có hạn sử dụng gần nhất (FEFO logic) để rớt tồn kho một cách an toàn.

### Công cụ Trích xuất Báo cáo Động (Dynamic Document/Export Service)
Tại phân hệ này (`PartialController.cs`), hệ thống định nghĩa hàm hạt nhân **`FlattenExpando`**:
- **Nguyên lý hoạt động**: Tiếp nhận chuỗi JSON phức tạp có lồng ghép nhiều nhánh (nguồn từ API hoặc Service) và áp dụng cấu trúc đệ quy (Recursive Function) cùng `ExpandoObject` kết hợp `ExpandoObjectConverter`.
- **Xử lý đồ thị dữ liệu (Data Graph)**: Bóc tách từng thuộc tính (`IDictionary<string, object>`), trải phẳng (flatten) các danh sách lồng và tạo ra Keys duy nhất (prefix keys). Dữ liệu Flattened Dictionary này sẽ lập tức được `ClosedXML` nhận diện qua `XLWorkbook` để ép kiểu vào sheet Excel, cấu hình động các Header thay vì yêu cầu hard-code cho mọi biểu mẫu báo cáo.

### Dashboard Quản trị & Tối ưu luồng (Admin Dashboard & TPL)
Kiến trúc Dashboard chứng tỏ ưu thế kỹ thuật thông qua `Task Parallel Library (TPL)`:
- Mã nguồn trong `DashboardService.GetOverviewAsync` hoặc `GetWarehouseDashboardAsync` xử lý phân hạch bằng việc khởi tạo đồng thời vô số Tasks: `trendAndSalesTask`, `inventoryTask`, `movementTask`, `aiForeastCastTask` bằng phương thức `Task.Run()` cho những kết nối cơ sở dữ liệu biệt lập.
- Gom khối (orchestration) toàn diện thông qua **`await Task.WhenAll(...)`**. Kết quả là các câu lệnh truy cập cơ sở dữ liệu (cả EF và Dapper) được chạy phân luồng bất đồng bộ cùng thời điểm, loại bỏ nút thắt cổ chai (bottleneck) và giảm độ trễ response từ vài giây xuống mili-giây.

## 4. Bảo mật & Tối ưu hóa (Security & Performance)

### Biện pháp Bảo mật (Security Implementations)
- **Ngăn chặn CSRF (Cross-Site Request Forgery)**: Thuộc tính `[ValidateAntiForgeryToken]` được thiết lập bắt buộc trên các `[HttpPost]` Actions (như trong `PartialController.Generate`).
- **Ngăn chặn SQL Injection**: Cơ sở dữ liệu tương tác hoàn toàn bằng Parameterized Queries. Đối với Entity Framework là qua LINQ. Đối với khối Dapper, câu lệnh gán vào biến đối tượng nguyên mẫu, chẳng hạn: `new { BranchId = branchId }` (như trong chuỗi truy vấn thủ tục `sp_Admin_Dashboard_Summary`).
- Mật khẩu mã hoá thông qua thư viện `BCrypt.Net-Next`.

### Tối ưu hóa Hiệu năng (Performance Tweaks)
- Sử dụng Memory Cache (`IMemoryCache`) được inject cho các truy xuất tần suất cao như thiết lập tham số hệ thống.
- Client-side dùng DataTables với chiến lược phân trang qua mạng (Server-side Pagination qua AJAX) ở các danh sách hàng ngàn bản ghi.
- Tại lớp Repository, chỉ sử dụng tracking object của Entity Framework khi có lệnh Modify. Với các câu lệnh đọc dữ liệu thuần tuý (Read-Only), sử dụng `.AsNoTracking()` hoàn toàn (như trong thuật toán quét DashboardKPI).

## Đội ngũ phát triển (Development Team)

Dự án được nghiên cứu, thiết kế và phát triển độc lập 100% bởi Nguyen Ha Phuong Thai từ trường Đại học Thủ Dầu Một (TDMU). Hệ thống được xây dựng hoàn toàn từ con số 0 để minh chứng cho khả năng làm chủ kiến trúc phần mềm và tư duy giải quyết nghiệp vụ thực tế.

## 6. Hướng dẫn Triển khai Hệ thống (System Deployment Guide)

1. **Phục hồi gói phụ thuộc (NuGet Restore)**
   Mở File giải pháp `EMarket.sln` bằng Visual Studio (Khuyên dùng v2022). Chuột phải lên Solution cụm `Solution Explorer` và lựa chọn khối lệnh **Restore NuGet Packages** để tái kích hoạt và cài đặt toàn bộ Packages theo manifest của `packages.config`.

2. **Cấu hình Chuỗi Kết nối Thiết lập (Web.config)**
   Điều chỉnh chuỗi kết nối SQL Server tĩnh tại file gốc `Web.config`:
   ```xml
   <connectionStrings>
     <!-- Thay 'Data Source' thành Server tương ứng -->
     <add name="EMarket_Connections" connectionString="Data Source=.;Initial Catalog=EMarket_DB;Integrated Security=True;" providerName="System.Data.SqlClient" />
     <add name="EMarket_DBEntities" connectionString="metadata=res://*/Models.EMarket_DB.csdl|res://*/Models.EMarket_DB.ssdl|res://*/Models.EMarket_DB.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=.;initial catalog=EMarket_DB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
   </connectionStrings>
   ```

3. **Cập nhật Cơ sở dữ liệu (Database Migrations/Script Execution)**
   Sửa dụng cơ chế của HangFire hoặc EF để render Database, mở **Package Manager Console** (PMC) thiết đặt:
   ```powershell
   Update-Database
   ```
   *Lưu ý: Nếu sử dụng Database-First (theo cấu trúc `edmx`), hãy đính kèm và thi hành tệp kịch bản `.sql` cơ sở dữ liệu trên SSMS (SQL Server Management Studio).*

4. **Biên dịch và Lưu trữ (IIS Express / Build Execute)**
   Chọn project `EMarket` làm Startup Project. Nhấn phím `F5` hoặc `Ctrl + F5` để kích hoạt Build Solution, điều phối các thư viện DLL vào `/bin` và host trực tiếp trên IIS Express. Trình thám thính (Browser) sẽ khởi chạy port localhost mặc định. Dữ liệu thời gian thực SignalR sẽ đồng bộ hoạt động.
