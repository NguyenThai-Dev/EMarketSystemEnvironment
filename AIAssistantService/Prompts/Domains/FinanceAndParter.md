### BẢNG DỮ LIỆU MIỀN TÀI CHÍNH & ĐỐI TÁC (FINANCE & PARTNER)
[LƯU Ý: Viết ĐÚNG CHÍNH XÁC phân biệt hoa/thường (Case-Sensitive)]

1. Bảng: `Suppliers` (Thông tin Nhà cung cấp)
   - Cột: `supplier_id` (PK), `name` (Tên công ty), `contact_person`, `phone`
   - Quy tắc: Chỉ dùng để lấy tên nhà cung cấp khi hỏi về nhập hàng/nợ.

2. Bảng: `PurchaseOrders` (Đơn nhập hàng từ nhà cung cấp)
   - Cột: `purchase_order_id` (PK), `supplier_id` (FK), `warehouse_id`, `status`, `total_amount`, `order_date`

3. Bảng: `PurchaseOrderDetails` (Chi tiết đơn nhập)
   - Cột: `purchase_order_detail_id` (PK), `purchase_order_id` (FK), `product_id`, `quantity`, `unit_price`, `total_price`

4. Bảng: `SupplierDebts` (Công nợ phải trả nhà cung cấp)
   - Cột: `debt_id` (PK), `purchase_order_id` (FK), `supplier_id` (FK), `total_amount`, `paid_amount`, `unpaid_amount` (Số tiền còn nợ), `due_date`

5. Bảng: `Expenses` (Chi phí vận hành - Điện, nước, lương...)
   - Cột: `expense_id` (PK), `branch_id`, `category_id`, `amount`, `expense_date`, `status`
   - Filter Rule: Chỉ tính chi phí khi `status = 'approved'`.

6. Bảng: `ExpenseCategories` (Loại chi phí)
   - Cột: `category_id` (PK), `name` (Tên loại chi phí)

📌 MỐI QUAN HỆ (JOIN RULES):
- Kiểm tra nợ: `Suppliers` JOIN `SupplierDebts` ON `Suppliers.supplier_id = SupplierDebts.supplier_id`
- Chi tiết nhập hàng: `PurchaseOrders` JOIN `PurchaseOrderDetails`