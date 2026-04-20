### BẢNG DỮ LIỆU MIỀN KINH DOANH (SALES DOMAIN)
[LƯU Ý: Viết ĐÚNG CHÍNH XÁC phân biệt hoa/thường (Case-Sensitive) như danh sách dưới đây]

1. Bảng: `Orders` (Hóa đơn bán hàng)
   - Cột: `order_id` (PK), `customer_id` (FK), `branch_id`, `order_date`, `status`, `total_amount`
   - Filter Rule: Chỉ tính doanh thu khi `status = 'Paid'`.

2. Bảng: `OrderDetails` (Chi tiết hóa đơn - số lượng bán)
   - Cột: `order_detail_id` (PK), `order_id` (FK), `product_id` (FK), `quantity` (Số lượng bán), `unit_price`, `discount`

3. Bảng: `Payments` (Thanh toán)
   - Cột: `payment_id` (PK), `order_id` (FK), `payment_method` (cash, credit_card...), `amount`, `status`

4. Bảng: `Promotions` (Chương trình khuyến mãi)
   - Cột: `promotion_id` (PK), `name`, `start_date`, `end_date`, `discount_type`, `discount_value`

📌 MỐI QUAN HỆ (JOIN RULES):
- `Orders` JOIN `OrderDetails` ON `Orders.order_id = OrderDetails.order_id`
- Doanh thu = `SUM(Orders.total_amount)`
- Số lượng SP bán ra = `SUM(OrderDetails.quantity)`