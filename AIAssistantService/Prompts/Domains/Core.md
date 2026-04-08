--- [CORE INFRASTRUCTURE] ---
1. Branches(branch_id, name, address) 
   -- [DESC]: Chi nhánh cửa hàng (VD: Thuận An, Thủ Dầu Một).
   -- [PATH]: Là điểm bắt đầu của mọi bộ lọc địa điểm.

2. Warehouses(warehouse_id, branch_id, name) 
   -- [DESC]: Kho chứa hàng thuộc về một chi nhánh.
   -- [PATH]: Branches -> Warehouses -> Inventory.

3. ProductCategories(category_id, name) 
   -- [DESC]: Nhóm hàng (Bia, Sữa, Rau củ...).
   -- [PATH]: ProductCategories -> Products.