--- [INVENTORY DOMAIN] ---
* [GOAL]: Truy vấn số lượng hàng ĐANG CÓ thực tế, Hạn sử dụng và Lịch sử biến động.

1. Products(product_id, name, category_id, supplier_id, barcode, price, unit, min_stock) 
   -- [DESC]: Thông tin chung của sản phẩm.
   -- [IMPORTANT]: Cột tên sản phẩm là 'name'. Tuyệt đối không bịa ra 'ProductName'.
   
2. ProductLots(lot_id, product_id, expiry_date, cost_price, batch_code) 
   -- [DESC]: Quản lý Lô hàng nhập vào. 'cost_price' là Giá Vốn.
   -- [LOGIC]: Tìm hàng hết hạn dùng: WHERE expiry_date < GETDATE().
   -- [PATH]: Products -> ProductLots (1-n).

3. Inventory(inventory_id, warehouse_id, lot_id, quantity) 
   -- [DESC]: Số lượng tồn kho chi tiết theo từng Lô tại từng Kho.
   -- [PATH QUAN TRỌNG]: Inventory kết nối với Products THÔNG QUA ProductLots.
      (Inventory.lot_id -> ProductLots.lot_id -> Products.product_id).
   -- [LOGIC]: Tồn kho = SUM(quantity). Luôn lọc quantity > 0.
4. StockMovements(movement_id, product_id, movement_type, quantity, reason, movement_date)
   -- [DESC]: Lịch sử xuất/nhập/điều chuyển kho.
   -- [VALUES]: Cột 'movement_type' CHỈ NHẬN các giá trị sau:
      + 'Import'     : Nhập hàng mới.
      + 'Return'     : Trả hàng (về nhà cung cấp hoặc khách trả lại).
      + 'Sale'       : Xuất bán (khi tạo đơn hàng).
      + 'Adjustment' : Kiểm kê / Cân bằng kho (khi kho thực tế khác hệ thống).
      + 'Internal'   : Điều chuyển nội bộ (từ kho này sang kho khác).