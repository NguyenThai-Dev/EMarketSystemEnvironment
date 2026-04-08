# EMarket AI Assistant Service

## Giới thiệu
Dự án **EMarket AI Assistant Service** là một hệ thống phân tích dữ liệu tự động và truy xuất cơ sở dữ liệu dựa trên mô hình ngôn ngữ lớn (LLM), được thiết kế chuyên biệt cho hệ sinh thái ERP và quy trình phân phối bán lẻ (Retail). Trong kiến trúc doanh nghiệp hiện đại, năng lực trích xuất và phân tích dữ liệu trực tiếp đóng vai trò cốt lõi. Service này giải quyết bài toán phức tạp hóa kỹ thuật bằng cách chuyển đổi ngôn ngữ tự nhiên thành các truy vấn SQL cấu trúc (Natural Language to SQL - NL2SQL). Sự can thiệp này cho phép đội ngũ quản trị cấp cao giao tác trực tiếp với cơ sở dữ liệu thông qua giao thức hội thoại, từ đó đưa ra quyết định định hướng dữ liệu (data-driven) một cách chính xác, bảo mật và tối ưu chi phí vận hành nguồn nhân lực kỹ thuật.

---

## Đội ngũ phát triển (Development Team)
Dự án được nghiên cứu, thiết kế và phát triển độc lập 100% bởi **Nguyen Ha Phuong Thai**.

---

## Tính năng cốt lõi

- **NL2SQL Engine:** Dịch thuật ngữ nghĩa từ ngôn ngữ tự nhiên thành cấu trúc lệnh truy vấn SQL tiêu chuẩn với độ ổn định cao.
- **Cơ chế tự phục hồi (Self-Healing SQL):** Hệ thống tích hợp vòng lặp kiểm soát và tự động sửa lỗi. Nếu kịch bản truy vấn xuất hiện lỗi ngoại lệ (exception) trong quá trình thực thi cấp cơ sở dữ liệu, `Controller` sẽ can thiệp bắt lỗi, lồng ghép thông báo lỗi này vào vùng không gian ngữ cảnh và yêu cầu AI tự nhìn nhận, sửa chữa cấu trúc lệnh (giới hạn tối đa 3 chu kỳ lặp). Cơ chế này đảm bảo được tính bền bỉ và độ tin cậy của dịch vụ trước các ngoại lệ kỹ thuật.
- **Ánh xạ Schema động (Dynamic Schema Mapping):** Tối ưu hóa dung lượng token đầu vào (context window limit) và tiết giảm chi phí API bằng việc phân tích từ khóa ban đầu. Tùy theo phân nhóm từ khóa, hệ thống sẽ linh hoạt bổ sung schema ở các lĩnh vực cụ thể (Customer, Sales, Inventory, AI & Analytics...).
- **Báo cáo dữ liệu bảo mật (Secure Data Reporting):** Toàn bộ dữ liệu trích xuất dưới định dạng mảng JSON thô sẽ được che chắn và dịch thuần sang báo cáo văn bản có cấu trúc. Khối lệnh ngăn chặn tình trạng dữ liệu kỹ thuật nhạy cảm lộ lọt sang phía Client không tin cậy.

---

## Kiến trúc & Luồng xử lý
Quá trình phân tích yêu cầu được thiết kế theo luồng tuần tự nghiêm ngặt:

**Ngôn ngữ tự nhiên -> Nhận diện Schema -> Suy luận kiến trúc LLM -> Thực thi SQL -> Xử lý lỗi ngoại lệ (Self-Healing) -> Định dạng báo cáo đầu ra**

1. **Khởi tạo & Nhận diện miền dữ liệu (Phase 1):** Tiếp nhận Prompt từ hệ thống API. Engine phân tích từ khóa và khôi phục cấu trúc khai báo ngôn ngữ định nghĩa dữ liệu (DDL) tương thích, đồng bộ lịch sử bài học phân tích trước đó nhằm thiết lập bối cảnh ban đầu.
2. **Tư duy Kiến trúc (Thought Process):** Semantic Kernel sử dụng cấu hình OpenAIPromptExecutionSettings giới hạn tính ngẫu nhiên (Temperature = 0), yêu cầu phản hồi theo định dạng JSON xác định, phân chia chu trình thành 2 tham số: lập luận (`Thought`) và giải pháp (`Sql`).
3. **Thực thi Database Engine:** Tầng giao tiếp dữ liệu bắt chuỗi SQL sinh ra và thực thi trên Server.
4. **Vòng lặp tự phục hồi (The Self-Healing Loop - Phase 2 & 3):** Nếu Data Engine báo lỗi, khối logic `ChatController` sẽ tạo mới tham số `SqlFixer`, cung cấp chuỗi lỗi kỹ thuật và đẩy ngược lại luồng xử lý AI. 
5. **Định dạng báo cáo (Phase 5):** Sau khi thực thi SQL đạt chuẩn an toàn, `Reporter` sẽ đánh giá kết quả trả về, viết lược dịch và phản hồi thông điệp tường minh cuối cùng cho Client Request.

---

## Cấu trúc thư mục Prompts
Hệ thống tuân thủ nguyên lý thiết kế tách bạch (Decoupling) giữa bộ khai báo quy tắc nghiệp vụ (Prompt) và nền tảng Mã nguồn (Codebase). Toàn bộ định nghĩa Schema Cơ sở dữ liệu và System Message được quản lý ngoài mã nguồn tại phân vùng `Prompts/`:

```text
AIAssistantService/
├── Controllers/
│   └── ChatController.cs
├── Prompts/                  
│   ├── SqlGenerator.md       # Định nghĩa vai trò phân tích truy vấn SQL
│   ├── SqlFixer.md           # Template xử lý báo cáo lỗi tại pha Self-Healing
│   ├── Reporter.md           # Template kết xuất và dịch tóm tắt kết quả
│   └── Domains/              # Khai báo kiến trúc DDL theo từng miền nghiệp vụ
│       ├── Core.md           # Cấu trúc hệ thống nền
│       ├── Inventory.md      # Lưu trữ, Hàng hóa, Chuỗi cung ứng
│       ├── Sales.md          # Doanh thu, Đơn hàng
│       ├── Customer.md       # Thẻ thành viên, Thực thể Khách hàng
│       ├── AIAndAnalytics.md # Khối nghiệp vụ cảnh báo, phân tích dự báo
│       └── Fallbacks.md      # Quy tắc phòng ngừa rủi ro tổng hợp
├── PromptService.cs          # Pipeline IO và tích hợp chuỗi văn bản
```

**Giải phẫu kỹ thuật việc loại trừ `Prompts/` qua trình Git Ignore:**
Thư mục `Prompts/` chứa cấu trúc vật lý của hệ quản trị cơ sở dữ liệu cũng như luồng xử lý nghiệp vụ nội bộ tổ chức. Việc cách ly cấu trúc này khỏi phiên bản quản trị Git là bắt buộc nhằm:
1. Đảm bảo an toàn cơ sở hạ tầng, ngăn chặn rò rỉ metadata hệ thống.
2. Thiết lập quy trình cấu hình tức thời phi gián đoạn (Zero-Downtime Configuration). Đội ngũ Quản trị Hệ thống nâng cấp schema linh hoạt bằng thao tác tệp `markdown` mà không cần tiến hành chu kỳ Re-build và Re-deploy ứng dụng.

---

## Công nghệ sử dụng
- **Nền tảng lõi:** .NET 8 Web API
- **Kiến trúc AI Integration:** Microsoft Semantic Kernel
- **Truy xuất dữ liệu (ORM):** Tập lệnh Dapper
- **Hệ Quản trị Cơ sở dữ liệu:** Microsoft SQL Server
- **Biên dịch định dạng:** Newtonsoft.Json

---

## Hướng dẫn sử dụng

### 1. Triển khai máy chủ cục bộ
Trong bước cấu hình sau khi cloning kho chứa mã nguồn, do phân vùng nghiệp vụ `Prompts` được loại trừ, đội ngũ sẽ cần thực hiện:
1. Tạo thư mục `Prompts` trực thuộc cấp bậc thư mục gốc của dự án.
2. Xây dựng các tệp tin cấu trúc nền tảng bắt buộc: `SqlGenerator.md`, `Reporter.md`, `SqlFixer.md` ứng với logic nghiệp vụ của tổ chức.
3. Thiết lập thư mục con `Domains` và nạp vào các khai báo Data Definition Language tương thích.
4. Điều chỉnh cấu hình khóa bảo mật, chuỗi Connection String trong cấu trúc tệp `appsettings.json`.
Lưu ý: Luôn sử dụng tài khoản Database có quyền Read-only để đảm bảo an toàn tuyệt đối cho dữ liệu hệ thống.

### 2. Đặc tả giao thức API
Hệ thống sử dụng HTTP POST Request chuẩn hóa định dạng JSON tại luồng truyền tải.

**Định dạng tải trọng gửi đi (Endpoint: POST /api/chat/ask)**
```json
{
  "Prompt": "Báo cáo mức số dư sản phẩm tồn kho nhãn hiệu A. Xác định các lô hàng dự kiến hết hạn.",
  "UserName": "Admin_Principal"
}
```

**Cấu trúc dữ liệu phản hồi (200 OK)**
```json
{
  "answer": "Báo cáo truy xuất hệ thống quản trị kho ghi nhận tổng dư lượng cho nhãn hiệu A đang đạt 120 đơn vị kỹ thuật. Lô hàng lưu kho cuối cùng sẽ chạm ngưỡng hạn sử dụng trong chu kỳ 6 tháng tới."
}
```
