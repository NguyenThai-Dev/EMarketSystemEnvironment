### BẢNG DỮ LIỆU MIỀN KHO & HÀNG HÓA (INVENTORY DOMAIN)
[LƯU Ý: Viết ĐÚNG CHÍNH XÁC phân biệt hoa/thường (Case-Sensitive) như danh sách dưới đây]

1. Bảng: `Products` (Thông tin sản phẩm)
   - Cột: `product_id` (PK), `name` (Tên SP), `category_id`, `supplier_id`, `price`, `unit`

2. Bảng: `ProductCategories` (Danh mục)
   - Cột: `category_id` (PK), `name` (Tên danh mục)

3. Bảng: `Branches` (Chi nhánh)
   - Cột: `branch_id` (PK), `name` (Tên chi nhánh)

4. Bảng: `Warehouses` (Kho bãi)
   - Cột: `warehouse_id` (PK), `branch_id` (FK), `name`

5. Bảng: `Inventory` (Tồn kho hiện tại)
   - Cột: `inventory_id` (PK), `warehouse_id` (FK), `lot_id` (FK), `quantity` (Số lượng tồn)

6. Bảng: `ProductLots` (Lô hàng - dùng tính hạn sử dụng FEFO)
   - Cột: `lot_id` (PK), `product_id` (FK), `expiry_date` (Hạn sử dụng)

📌 MỐI QUAN HỆ (JOIN RULES):
- Lọc theo Tên Sản Phẩm: `Products` JOIN `ProductLots` ON `Products.product_id = ProductLots.product_id` JOIN `Inventory` ON `ProductLots.lot_id = Inventory.lot_id`
- Hỏi "Hàng còn bao nhiêu" -> Dùng `SUM(Inventory.quantity)`