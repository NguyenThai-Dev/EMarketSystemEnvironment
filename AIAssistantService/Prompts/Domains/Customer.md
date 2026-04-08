--- [CUSTOMER DOMAIN] ---
* [SECURITY]: TUYỆT ĐỐI KHÔNG dùng bảng Customers gốc. Dữ liệu phải được ẩn danh.

1. v_AI_Customer_Analytics(customer_id, customer_type, points_balance, masked_name)
   -- [DESC]: View tổng hợp thông tin khách hàng.
   -- [WARNING]: Cột tên khách hàng là 'masked_name'. KHÔNG ĐƯỢC DÙNG 'CustomerName' hay 'Name'.
   -- [PATH]: v_AI_Customer_Analytics -> Orders (qua customer_id).

2. LoyaltyPrograms(loyalty_id, customer_id, points_earned, points_redeemed)
   -- [DESC]: Lịch sử tích điểm và đổi điểm.