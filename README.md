# EMarket Ecosystem - Enterprise Architecture & Technical Specification

Hệ sinh thái EMarket là một nền tảng quản trị doanh nghiệp lai (Hybrid ERP), kết hợp sức mạnh vận hành ổn định của .NET Framework 4.8.1 và khả năng phân tích thông minh của .NET 8. Hệ thống không chỉ dừng lại ở việc quản lý giao dịch bán lẻ đa chi nhánh mà còn tiến xa hơn với bộ não AI tích hợp, cho phép truy vấn dữ liệu bằng ngôn ngữ tự nhiên (NL2SQL) và dự báo tồn kho thông qua Machine Learning (XGBoost). Đây là minh chứng cho việc hiện đại hóa hệ thống Legacy bằng kiến trúc phân tán hiện đại.

Tài liệu này cung cấp cái nhìn chuyên sâu về kiến trúc tổng thể, mô hình kỹ thuật, chiến lược triển khai và các hạt nhân nghiệp vụ cốt lõi của EMarket Ecosystem. Hệ thống được thiết kế dưới dạng một nền tảng ERP phân tán lai (Hybrid Distributed ERP) nhằm tối ưu hóa vòng đời sản phẩm, phân tích dữ liệu bán lẻ theo thời gian thực và tự động hóa thao tác thông qua Trí tuệ Nhân tạo.

---

## 1. Phân tích Mô hình Lai (Hybrid Architecture Analysis)

Hệ sinh thái EMarket không đi theo mô hình Monolith truyền thống mà ứng dụng **Kiến trúc Lai (Hybrid Architecture)**, cho phép sự tồn tại song song và tương tác tương hỗ giữa hai nền tảng .NET khác biệt:

*   **Lớp Vận hành cốt lõi (Operational Layer - .NET Framework 4.8.1 - EMarket):**
    Được cấu trúc dựa trên kiến trúc ASP.NET MVC 5, đóng vai trò là "xương sống" xử lý đa số các luồng nghiệp vụ giao dịch (Transactional Workloads) như quản lý hàng kho, bán hàng (POS), luân chuyển chứng từ. Việc duy trì .NET Framework giúp tận dụng tối đa tính ổn định cho các thư viện legacy và tích hợp sâu với hạ tầng nền tảng Windows nội bộ.
*   **Lớp Thông minh nhân tạo (Intelligence Layer - .NET 8 - AIAssistantService):**
    Đóng vai trò là "bộ não" phân tích (Analytical Workloads). Quyết định chọn .NET 8 mang lại hiệu năng cao nhất (High-performance Computing), luồng I/O bất đồng bộ hoàn hảo để giao tiếp với các dịch vụ AI như OpenAI API, Semantic Kernel, đồng thời tối ưu hóa tài nguyên phần cứng thông qua container hóa nhẹ gọn.

**Cơ chế đồng bộ và tích hợp hệ thống (System Integration & Synchronization):**
Sự gắn kết của 2 hệ thống được thực thi qua **Shared Database Strategy (Chiến lược Cơ sở dữ liệu chia sẻ)**. Tại đây, lớp Vận hành (EMarket) chịu trách nhiệm ghi dữ liệu chuẩn hóa vào SQL Server thông qua Entity Framework, trong khi lớp Thông minh (AIAssistantService) ưu tiên các truy vấn thuần và cấu trúc dữ liệu không gian bằng Dapper để đảm bảo độ trễ thấp nhất. Việc quản lý các thông số định tuyến và cấu hình nhạy cảm được trừu tượng hóa nhờ bộ nguyên tắc **User Secrets / Config Builders**, cho phép cả hai lớp chia sẻ chung một hệ thống định dạng (appsettings vs Web.config) mà không làm rò rỉ các khóa API hoặc cấu trúc máy chủ nội bộ.

---

## 2. Khám phá Công nghệ & Mẫu thiết kế (Deep-tech Discovery)

Mã nguồn dự án thể hiện mức độ trưởng thành tối đa trong việc ứng dụng linh hoạt các mẫu thiết kế hướng đối tượng (OOP Design Patterns) và kiến trúc ứng dụng (Application Architecture).

**Technology Stack:**
*   **Core Systems:** C#, ASP.NET MVC 5 (.NET 4.8.1), ASP.NET Core 8 Web API.
*   **Data Access Layer:** Entity Framework 6 (Code-First & Database-First), Dapper Micro-ORM.
*   **Front-end & Real-time:** Bootstrap, jQuery, SignalR, Chart.js.
*   **Machine Learning & Extensions:** XGBoost (Python), Ngrok, Newtonsoft.Json.

**Design Patterns Thực tiễn:**
*   **Dependency Injection (DI):** Trong EMarket, **SimpleInjector** được khởi tạo như một container cấp cao để quản lý trọn vẹn vòng đời các Interface (Transient, Scoped/WebRequest, Singleton), giải phóng Controller khỏi việc ép kiểu trực tiếp thành phần. Lớp Web API tận dụng kiến trúc DI tích hợp sẵn của .NET 8.
*   **Repository & Unit of Work:** Quản lý tập trung các kết nối của Entity Framework, thu gọn bộ ngữ cảnh nghiệp vụ, và đảm bảo mọi Transaction (Commit/Rollback) luôn đồng bộ và tuân thủ ACID.
*   **Chỉ huy - Truy vấn hỗn hợp (Hybrid CQRS):** Một sự phân tách logic triệt để được tìm thấy trong kho lưu trữ dữ liệu. Các luồng *Command (Create, Update, Delete)* yêu cầu bảo toàn toàn vẹn dữ liệu được xử lý bằng EF với Tracking Context. Các luồng *Query (Select, Aggregate)* yêu cầu tốc độ cao và linh hoạt đọc khối lớn dữ liệu được bàn giao cho Dapper với các truy vấn SQL thuần.
*   **Real-time Notification (SignalR):** Đẩy thông báo Push hai chiều, hiện thực hóa khái niệm Event-Driven ngay trên giao diện người dùng mà không cần Long-Polling.

---

## 3. Đặc tả Nghiệp vụ Đột phá (Breakthrough Business Logic)

Ngoài các kiến trúc phần mềm, dự án ẩn chứa những thuật toán vận hành với khả năng giải quyết trực tiếp các điểm nghẽn (bottleneck) kinh điển trong bài toán phân phối:

*   **NL2SQL & Self-Healing Loop (Trợ lý tự phục hồi):** Nằm gọn trong khối `ChatController` của AIAssistantService, hệ thống dịch ngôn ngữ tự nhiên thành T-SQL (NL2SQL). Khác với các mô hình Prompt thông thường, hệ thống tích hợp vòng lặp **Self-Healing (Tự sửa chữa)**. Nếu cấu trúc SQL do AI tạo ra vấp phải lỗi `SqlException` khi thực thi, máy chủ thay vì trả về lỗi cho người dùng sẽ "bắt" (Catch) thông điệp lỗi đó, đóng gói kèm câu lệch SQL sai, và gửi ngược lại một phiên Prompt ẩn tới AI để tái cấu trúc. Luồng này sẽ lặp lại trong số lần nhất định cho đến khi trả về kết quả đúng đắn, mang tính ứng biến thông minh chưa từng có.
*   **FEFO Inventory Routing (Thuật toán trừ kho FEFO):** Trái ngược với FIFO truyền thống không phân loại được thời gian lưu trữ sản phẩm hóa học, EMarket can thiệp sâu vào `ProductLots` (Lô sản phẩm). Thuật toán được lập trình để tạo danh sách hàng đợi ảo, ưu tiên sắp xếp và khấu trừ các Lô hàng có `ExpirationDate` gần nhất trong kho. Trong trường hợp Lô ưu tiên không đủ sức chứa số lượng cần bán, thuật toán tự động chia nhỏ (split) số lượng trừ qua lô kế tiếp.
*   **FlattenExpando Engine (Trình biên dịch cấu trúc không bằng phẳng):** Nhờ cơ chế `ExpandoObject` của Dapper, các dữ liệu có mức độ phân cấp động từ SQL đa chiều hay JSON phức tạp có thể được quét đệ quy (recursive traversal). Nó biến một cấu trúc cây (Tree-struct) khó thao tác trên View thành các báo cáo dạng lưới không gian 2D (Flat Table), giảm thiểu tối đa tài nguyên bộ máy Front-end khi Render.
*   **Parallel KPI Processing (Xử lý tác vụ song song):** Tại Controller của Dashboard, thay vì tuần tự chạy báo cáo doanh thu, tồn kho, lợi nhuận (gây tắc nghẽn IO-bound), `Task Parallel Library (TPL)` với cơ chế `Task.WhenAll` được gọi. 8 truy vấn cồng kềnh nhất được "ném" độc lập vào 8 luồng Pool, giúp tải biểu đồ đồ sộ chỉ trong vài trăm mili-giây, tối đa hóa thông lượng (Throughput) theo định luật Amdahl.
*   **PythonModel Integration (Mô hình XGBoost Dự báo):** Tương tác chéo nền tảng qua `AIService.cs` để nhúng mô hình ranh giới quyết định theo Machine Learning. Dữ liệu bán hàng lịch sử được xuất và truyền thẳng xuống Python Scripts đính kèm để áp dụng thuật toán tối ưu hóa Gradient (XGBoost) nhằm dự đoán sản lượng và xu hướng sản phẩm.
*   **Ngrok Webhook API Proxying:** Vượt ra ngoài ranh giới nội mạng cục bộ (localhost), hệ thống gọi API ngrok trực tiếp từ cấu hình để thiết lập đường hầm Proxy an toàn (Secure Tunnel). Điều này cho phép hệ sinh thái nhận các tín hiệu Webhook đa kênh từ các đối tác thanh toán/bên thứ 3 mà không cần cấu hình NAT Router phần cứng phức tạp.

---

## 4. Bảo mật & Tính toàn vẹn (Security & Integrity)

Mọi cổng giao tiếp đều được thiết kế với tiêu chuẩn kháng cự (Defense-in-Depth):

1.  **Anti-Forgery Tokenization:** Bảo vệ khỏi rủi ro Cross-Site Request Forgery (CSRF). Mọi Form gửi dữ liệu ở EMarket MVC đều bị hệ thống phát sinh chuỗi Token động ở cấp Session nhằm đối chiếu độ chân thực ngữ cảnh Request.
2.  **Parameterized Queries Strategy:** Bất chấp việc hệ thống sử dụng Dapper hay EF, 100% Parameter được định nghĩa ngầm để triệt tiêu mọi khả năng chèn mã SQL Injection ẩn.
3.  **Hidden Prompt Schema Architecture:** Ở AIAssistantService, các file thiết lập "chỉ thị cốt lõi" (System Prompts) định hướng cho mô hình LLM được lưu ở file rời hoàn toàn nằm ngoài mã nguồn và quản lý qua User Secrets/Git-ignore. Việc này bảo mật chất xám Schema SQL không bị đối thủ biên dịch đảo ngược nếu mã ứng dụng chính bị xâm phạm.
4.  **CORS & Secure Headers Policy:** Chỉ mở quyền truy cập tài nguyên chữ thập (Cross-Origin) có kiểm soát giữa domain của hệ thống EMarket và AIAssistantService trên lớp Middleware của .NET 8.

---

## 5. Đội ngũ phát triển (Development Team)

Đội ngũ phát triển (Development Team)
Dự án được nghiên cứu, thiết kế và phát triển độc lập 100% bởi Nguyen Ha Phuong Thai từ trường Đại học Thủ Dầu Một (TDMU). Hệ thống được xây dựng hoàn toàn từ con số 0 để minh chứng cho khả năng làm chủ kiến trúc phần mềm và tư duy giải quyết nghiệp vụ thực tế.

---

## 6. Hướng dẫn Triển khai (Full-stack Deployment Guide)

Phần này mô tả tiến trình khởi động lại toàn bộ Hệ sinh thái một cách tuần tự (đảm bảo rằng máy chủ cài đặt IIS Express, .NET 8 SDK, .NET Framework 4.8.1 và SQL Server có sẵn).

### Bước 1: Khởi tạo Dữ liệu gốc (Database Provisioning)
1.  Truy cập SQL Server Management Studio (SSMS).
2.  Mở lệnh script trong hệ thống chứa khởi tạo cho EMarket. Chạy mã thực thi (F5) để khôi phục Table Schema và Insert dữ liệu siêu dữ liệu (Metadata/Seed data).

### Bước 2: Setup lớp Operational (EMarket Project)
1.  Mở `EMarket.sln` bằng Visual Studio (được khuyến nghị bản 2022).
2.  Tiến hành **Restore NuGet Packages** bằng cách mở *Package Manager Console* chạy: `Update-Package -reinstall`.
3.  Mở tập tin `Web.config` sửa nút `connectionStrings` tương ứng với chuỗi định tuyến SQL của bạn (đảm bảo quyền truy cập `Integrated Security=True` hoặc tài khoản sa).
4.  Nhấn nút `F5` hoặc IIS Express Run để hệ thống bắt đầu biên dịch, tải các Container SimpleInjector và xuất hiện giao diện Admin EMarket.

### Bước 3: Setup lớp Intelligence (AIAssistantService)
1.  Mở thư mục `AIAssistantService` qua Visual Studio hoặc Terminal.
2.  Sử dụng Terminal khởi tạo cấu hình bảo mật bằng hệ thống Secret Manager của .NET 8:
    ```bash
    dotnet user-secrets init
    dotnet user-secrets set "OpenAI:ApiKey" "YOUR_API_KEY_HERE"
    ```
3.  Sao chép chung chuỗi định tuyến SQL nội bộ vào `appsettings.json` trong Node `"ConnectionStrings"`.
4.  Thiết lập tập tin Prompt nội bộ nếu cần. Sau đó trên Terminal thực thi:
    ```bash
    dotnet run
    ```
5.  Service sẽ được biên dịch và lắng nghe ở Cổng được khai báo (Ví dụ: `http://localhost:5000` hoặc qua Swagger UI tại `/swagger`).

### Bước 4: Khớp nối Hệ sinh thái toàn diện
1.  Đảm bảo cả EMarket và AIAssistantService đều đã khởi chạy hoàn chỉnh mà không vấp quá trình ngoại lệ (Exception Handling).
2.  Đăng nhập vào màn hình giao dịch EMarket, tạo các hóa đơn hoặc luân chuyển kho hàng. Hệ thống Database dùng chung sẽ lập tức ghi chú lại hành vi.
3.  Truy cập vào công cụ Chat từ AIAssistantService, đưa ra các truy vấn về hệ thống thống kê như "*Sản phẩm nào đã vượt quá hạn mức FEFO trong tháng này?*". 
4.  Cả 2 nền tảng ngay lúc này sẽ hoạt động đồng bộ, chia sẻ cấu hình, dữ liệu và tương tác liên tục cho thấy một giải pháp ERP toàn diện đã triển khai thành công.
