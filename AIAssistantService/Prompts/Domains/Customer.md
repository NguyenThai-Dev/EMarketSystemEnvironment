### BẢNG DỮ LIỆU MIỀN KHÁCH HÀNG (CUSTOMER DOMAIN)
[CRITICAL SECURITY: Bảng Customers gốc đã bị chặn. CHỈ ĐƯỢC PHÉP dùng view ẩn danh dưới đây]

1. View: `v_AI_Customer_Analytics` (Dữ liệu khách hàng ẩn danh)
   - Cột: `customer_id` (PK), `customer_type` (retail, wholesale), `points_balance` (Điểm hiện tại), `masked_name` (Tên đã che - VD: N********)
   - Quy tắc: Sử dụng view này thay thế hoàn toàn cho bảng khách hàng. Không được đoán tên bảng khác.

2. Bảng: `LoyaltyPrograms` (Lịch sử điểm thưởng)
   - Cột: `loyalty_id` (PK), `customer_id` (FK), `order_id`, `points_earned`, `points_redeemed`, `created_at`

📌 MỐI QUAN HỆ (JOIN RULES):
- `v_AI_Customer_Analytics` JOIN `Orders` ON `v_AI_Customer_Analytics.customer_id = Orders.customer_id`