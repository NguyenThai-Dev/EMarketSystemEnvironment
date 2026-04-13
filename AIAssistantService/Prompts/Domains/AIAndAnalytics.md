### BẢNG DỮ LIỆU PHÂN TÍCH & DỰ BÁO AI (AI ANALYTICS)
[LƯU Ý: Miền này chứa kết quả tính toán từ máy học, dùng cho câu hỏi "về tương lai" hoặc "xu hướng"]

1. Bảng: `AI_SalesForecast` (Dự báo doanh số tương lai)
   - Cột: `product_id`, `branch_id`, `forecast_date`, `predicted_qty` (Số lượng dự kiến bán), `confidence_score`

2. Bảng: `AI_Product_Insight` (Phân tích hiệu suất sản phẩm)
   - Cột: `product_id`, `qty_sold`, `growth_percent` (% tăng trưởng), `contribution_percent` (% đóng góp doanh thu), `insight_level` (top_performer, underperformer)

3. Bảng: `AI_ReplenishmentAdvice` (Lời khuyên nhập hàng)
   - Cột: `product_id`, `current_stock`, `expected_demand`, `suggest_qty` (Số lượng khuyên nhập), `reason` (Lý do khuyên nhập)

4. Bảng: `AI_Anomaly_Category` (Cảnh báo bất thường)
   - Cột: `category_id`, `actual_qty`, `forecast_qty`, `deviation_percent` (% lệch), `anomaly_type` (spike, drop)

📌 QUY TẮC TRUY VẤN:
- Khi người dùng hỏi "Sắp tới bán được bao nhiêu?", "Cần nhập thêm gì không?", "Tại sao hàng này bán chậm?" -> Ưu tiên dùng các bảng AI này thay vì bảng Sales gốc.