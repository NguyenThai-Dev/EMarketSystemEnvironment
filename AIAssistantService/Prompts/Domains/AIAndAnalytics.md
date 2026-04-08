--- [AI INSIGHTS DOMAIN - ƯU TIÊN CAO NHẤT] ---
* [GOAL]: Dữ liệu phân tích nâng cao từ Machine Learning.
* [MANDATORY RULE]: Nếu câu hỏi chứa 'ngày tới', 'nguy cơ', 'cháy hàng', BẮT BUỘC phải dùng bảng bên dưới. KHÔNG được tự tính toán (min_stock - quantity).

1. AI_SalesForecast(product_id, branch_id, forecast_date, predicted_qty, confidence_score)
   -- [DESC]: Dự đoán số lượng bán trong tương lai (Next 7-30 days).

2. AI_InventoryWarning(product_id, days_to_exhaust, warning_type, risk_reason)
   -- [DESC]: Cảnh báo rủi ro kho. 'days_to_exhaust': Số ngày còn lại trước khi hết hàng.
   -- [LOGIC]: 'Hết hàng trong X ngày tới' nghĩa là: WHERE days_to_exhaust <= X.
   -- [PATH]: AI_InventoryWarning -> Products (qua product_id).

3. AI_ReplenishmentAdvice(product_id, branch_id, suggested_qty, priority_level)
   -- [DESC]: Gợi ý nhập hàng. Priority: 'High', 'Medium', 'Low'.