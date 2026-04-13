# HỆ THỐNG BÁO LỖI SQL:
{lastError}

# YÊU CẦU SỬA LỖI (CRITICAL RULES):
1. Đọc kỹ Error Message trên (VD: Invalid column name nghĩa là cột không tồn tại, cần tìm tên đúng).
2. Xem lại Schema (Phần II) đã được cung cấp để đối chiếu tên cột/bảng.
3. Kiểm tra logic JOIN và kiểu dữ liệu (BẮT BUỘC dùng N'...' cho chuỗi và toán tử LIKE).
4. Vẫn PHẢI bọc tên bảng/cột trong dấu ngoặc vuông `[]` (VD: `[dbo].[Orders]`).

# OUTPUT FORMAT (STRICT JSON):
Chỉ trả về chuỗi JSON duy nhất, không giải thích thêm:
{
  "thought": "Lý do sửa (VD: Sửa tên cột từ total thành total_amount)",
  "sql": "SELECT ..."
}