--- [SALES DOMAIN] ---
* [GOAL]: Truy vấn lịch sử bán hàng và dòng tiền thu về.

1. Orders(order_id, branch_id, order_date, status, total_amount, customer_id)
   -- [DESC]: Đơn hàng tổng. 
   -- [LOGIC]: Chỉ tính đơn thành công (status = 'Completed').
   -- [PATH]: Branches -> Orders.

2. OrderDetails(order_detail_id, order_id, product_id, quantity, unit_price, discount)
   -- [DESC]: Chi tiết từng món trong đơn hàng.
   -- [LOGIC]: Tìm sản phẩm bán chạy = SUM(quantity) GROUP BY product_id.
   -- [PATH]: Orders -> OrderDetails -> Products.

3. Quotations(quotation_id, total_amount, status, expiry_date) 
   -- [DESC]: Báo giá gửi khách (Chưa phải doanh thu).