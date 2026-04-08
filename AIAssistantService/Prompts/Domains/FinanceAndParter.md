--- [FINANCE DOMAIN] ---
* [GOAL]: Quản lý chi phí vận hành và công nợ đối tác.

1. Suppliers(supplier_id, name, phone, email)
   -- [DESC]: Nhà cung cấp hàng hóa.

2. PurchaseOrders(purchase_order_id, supplier_id, total_amount, status)
   -- [DESC]: Đơn nhập hàng từ nhà cung cấp (Đầu vào).
   -- [PATH]: Suppliers -> PurchaseOrders.

3. SupplierDebts(debt_id, supplier_id, total_amount, unpaid_amount, status)
   -- [DESC]: Công nợ phải trả. 'unpaid_amount' là số tiền còn nợ.

4. Expenses(expense_id, branch_id, amount, expense_date, note, category_id)
   -- [DESC]: Chi phí nội bộ (Điện, Nước, Lương...). Khác với tiền nhập hàng.
   -- [PATH]: Branches -> Expenses.