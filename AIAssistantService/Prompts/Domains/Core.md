### BẢNG DỮ LIỆU CẤU TRÚC HỆ THỐNG (CORE DATA)
[LƯU Ý: Chứa các thông tin danh mục chung dùng để JOIN giữa các miền]

1. Bảng: `Branches` (Danh sách Chi nhánh/Cửa hàng)
   - Cột: `branch_id` (PK), `name`, `address`

2. Bảng: `Warehouses` (Danh sách Kho bãi)
   - Cột: `warehouse_id` (PK), `branch_id` (FK), `name`

3. Bảng: `Products` (Thông tin sản phẩm master)
   - Cột: `product_id` (PK), `name`, `barcode`, `unit`, `price`

4. Bảng: `ProductCategories` (Danh mục sản phẩm)
   - Cột: `category_id` (PK), `name`

📌 MỐI QUAN HỆ: 
- Đây là các bảng "Gốc". Mọi câu hỏi có chứa tên Chi nhánh, tên Sản phẩm hoặc tên Kho đều phải JOIN với các bảng này để lọc theo Name.