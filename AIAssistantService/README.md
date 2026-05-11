# EMarket Chatbot User Guide & Prompt Manual

## Purpose: Strategic Decision Support & Risk Analysis

Welcome to the EMarket AI Assistant. This Chatbot is **not** a raw data exporter. It has been intentionally designed as an **Inventory & Risk Analyst** to help business owners, store managers, and executives make strategic, data-driven decisions.

The Chatbot leverages our advanced AI models (XGBoost Sales Forecasting and FEFO Financial Risk Analysis) to instantly highlight anomalies, forecast variances, and warn you about potential cash flow or deadstock issues.

## ⚠️ Important Note: Token Optimization & Hard Limits

To ensure blazing-fast response times and prevent the AI from exhausting processing limits (Token Quota / Out-Of-Memory errors), the system employs **Aggressive Payload Truncation (SmartRefine)**.

- **List Constraints**: The AI will **never** display your entire database table (e.g., all 1,000 orders or all 500 customers).
- **Top 5-20 Limit**: When asking for lists, the system will automatically truncate results. General lists will show a maximum of **10 items**, while critical AI analyses (Anomalies, Forecasts) will show up to **20 items**.
- **Drill Down**: The AI will provide an "Executive Summary" of the most important items. If you need to investigate a specific item, simply provide its ID to the Chatbot.

---

## 💡 How to Talk to the Chatbot (Prompt Examples)

To get the most out of the AI, ask high-level analytical questions rather than basic CRUD operations. Here are 7 highly effective prompt examples:

### 1. Risk Analysis (FEFO)
> *"Hãy phân tích rủi ro tài chính của các lô hàng sắp hết hạn trong kho. Lô nào có giá trị trích lập dự phòng cao nhất?"*
> *(Analyze the financial risk of expiring lots in the warehouse. Which lot has the highest provisioning value?)*

### 2. AI Forecasting (XGBoost)
> *"Tóm tắt dự báo bán hàng 30 ngày tới của chi nhánh 1. Có mã sản phẩm nào dự kiến sẽ bán vượt quá số lượng tồn kho thực tế không?"*
> *(Summarize the 30-day sales forecast for branch 1. Are there any product codes expected to sell beyond current actual stock?)*

### 3. Anomaly Detection
> *"Kiểm tra các bất thường (anomalies) trong hoạt động bán hàng tuần qua. Có sự cố nào cần tôi lưu ý ngay lập tức không?"*
> *(Check for anomalies in sales activities over the past week. Are there any issues I need to address immediately?)*

### 4. Cash Flow & Debt
> *"Tổng quan tình hình công nợ với nhà cung cấp. Liệt kê top 5 khoản nợ sắp đến hạn hoặc đã quá hạn để tôi ưu tiên thanh toán."*
> *(Overview of supplier debt. List the top 5 near-due or overdue debts for priority payment.)*

### 5. Deadstock Identification
> *"Hệ thống AI có phát hiện ra các mặt hàng nào đang bị tồn đọng (deadstock) và gây đọng vốn tại chi nhánh 1 không?"*
> *(Has the AI system detected any deadstock items tying up capital at branch 1?)*

### 6. Replenishment 
> *"Dựa trên dữ liệu bán hàng gần đây, hệ thống có lời khuyên bổ sung hàng hóa (replenishment advice) nào cho các kho không?"*
> *(Based on recent sales data, what replenishment advice does the system have for the warehouses?)*

### 7. KPI Dashboard Summary
> *"Cho tôi một tóm tắt siêu ngắn gọn về hiệu suất kinh doanh (branch-performance) và tình hình thu chi (finance) của chi nhánh 1 hôm nay."*
> *(Give me a super brief summary of business performance and finance for branch 1 today.)*

---
*Developed as part of the EMarket Mini-ERP system.*
