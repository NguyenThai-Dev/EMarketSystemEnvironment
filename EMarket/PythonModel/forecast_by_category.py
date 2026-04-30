import os
import sys
import pandas as pd
import numpy as np
import xgboost as xgb
import urllib
import time
import warnings
from sqlalchemy import create_engine, text
from sqlalchemy.types import NVARCHAR
from datetime import timedelta, datetime 

# Tắt cảnh báo pandas cho sạch log
warnings.filterwarnings('ignore')

def _nvarchar_dtype(df):
    """Tự động ép tất cả cột string sang NVARCHAR(500) để tránh lỗi tiếng Việt."""
    return {col: NVARCHAR(500) for col in df.select_dtypes(include=['object']).columns}

# =========================================================
# 1. KẾT NỐI DATABASE (Chỉ tạo engine khi gọi từ main)
# =========================================================
def _create_engine():
    params = urllib.parse.quote_plus(
        "Driver={ODBC Driver 17 for SQL Server};"
        "Server=127.0.0.1,1433;"
        "Database=EMarket_DB;"
        "UID=root_admin;"
        "PWD=SqlConnections2026!;"
        "TrustServerCertificate=yes;"
    )
    return create_engine(f"mssql+pyodbc:///?odbc_connect={params}", pool_size=5, max_overflow=0)

# =========================================================
# 2. HÀM XỬ LÝ CHÍNH: XGBOOST POISSON MODEL
# =========================================================
def train_and_dispatch_xgb(data_tuple):
    pid, branch_id, df_p, min_stock = data_tuple
    
    curr_stock = df_p["current_stock"].iloc[0]
    nearest_expiry = df_p["nearest_expiry_days"].iloc[0] if "nearest_expiry_days" in df_p.columns else 9999
    qty_risk = df_p["qty_risk"].iloc[0] if "qty_risk" in df_p.columns else 0
    
    # Yêu cầu tối thiểu 30 điểm dữ liệu để train
    if len(df_p) < 30:
        return None

    try:
        # --- A. FEATURE ENGINEERING (TẠO BIẾN CHO AI) ---
        df_p = df_p.sort_values('ds').reset_index(drop=True)
        
        # Tạo Lags
        for i in [1, 7, 14]: # Thay lag_30 bằng lag_14 để tránh drop quá nhiều data
            df_p[f'lag_{i}'] = df_p['y'].shift(i)
            
        # [NÂNG CẤP]: Dùng EMA thay cho Rolling Mean thông thường để bắt trend nhạy hơn
        df_p['ema_7'] = df_p['y'].shift(1).ewm(span=7, adjust=False).mean()
        
        # Loại bỏ NaN sau khi shift
        columns_to_check = ['lag_1', 'lag_7', 'lag_14', 'ema_7']
        df_clean = df_p.dropna(subset=columns_to_check).copy()

        if len(df_clean) < 15: 
            return None

        # Features đưa vào huấn luyện
        features = ['day_of_month', 'month', 'day_of_week', 'is_weekend', 'is_payday', 'is_festive', 
                    'lag_1', 'lag_7', 'lag_14', 'ema_7']
        
        # --- B. TRAINING MODEL (CẤU HÌNH POISSON DÀNH CHO BÁN LẺ) ---
        # [NÂNG CẤP]: Chuyển sang objective='count:poisson' (chuẩn đếm số lượng)
        model = xgb.XGBRegressor(
            n_estimators=120, 
            max_depth=4, # Giảm depth để tránh overfit
            learning_rate=0.08, 
            objective='count:poisson', # Rất quan trọng cho count data
            n_jobs=1,
            random_state=42
        )
        model.fit(df_clean[features], df_clean['y'])
        
        # Tính độ tin cậy (R2_score đôi khi âm với poisson, ta dùng công thức đánh giá độ lệch chuẩn)
        preds_hist = model.predict(df_clean[features])
        mape = np.mean(np.abs((df_clean['y'] - preds_hist) / (df_clean['y'] + 1))) # Thêm 1 để tránh chia cho 0
        conf = max(0, min(99.9, 100 - (mape * 100) + 10)) 

        # --- C. RECURSIVE FORECAST (DỰ BÁO 30 NGÀY TƯƠNG LAI) ---
        future_forecasts = []
        current_feats = df_clean.iloc[-1].copy()
        start_forecast_date = datetime.now().replace(hour=0, minute=0, second=0, microsecond=0)
        
        for day_step in range(1, 31):
            next_date = start_forecast_date + timedelta(days=day_step)
            
            m = next_date.month
            d = next_date.day
            wd = next_date.weekday()
            
            # Cập nhật mốc thời gian
            current_feats['day_of_month'] = d
            current_feats['month'] = m
            current_feats['day_of_week'] = wd
            current_feats['is_weekend'] = 1 if wd >= 5 else 0
            current_feats['is_payday'] = 1 if d in [1,2,3,4,5, 15,16,17,18,19,20] else 0
            
            # Logic Lễ Hội
            is_festive = 0
            if (m == 1 and d > 15) or (m == 2 and d < 15) or (m == 4 and d >= 28) or \
               (m == 5 and d <= 2) or (m == 9 and d == 2) or (m == 11 and d >= 25) or (m == 12 and d >= 20):
                is_festive = 1        
            current_feats['is_festive'] = is_festive
            
            # [NÂNG CẤP]: Dự báo & Bơm Nhiễu Poisson (Poisson Noise Injection)
            # XGBoost trả về giá trị kỳ vọng (Lambda) của Poisson
            pred_lambda = model.predict(pd.DataFrame([current_feats[features]]))[0]
            pred_lambda = max(0.01, pred_lambda) # Không được <= 0
            
            # Dùng lambda để bốc thăm 1 con số thực tế (Tạo độ loãng/phân tán răng cưa)
            # Ví dụ: lambda = 1.5 -> Ngày bốc 0, ngày bốc 1, ngày bốc 3.
            simulated_sales = np.random.poisson(pred_lambda)
            
            future_forecasts.append({
                'branch_id': branch_id,
                'product_id': pid,
                'forecast_date': next_date,
                'predicted_qty': simulated_sales,
                'confidence_score': round(conf, 2)
            })
            
            # Cập nhật Lags cho vòng lặp tiếp theo
            current_feats['lag_14'] = current_feats['lag_7'] 
            current_feats['lag_7']  = current_feats['lag_1'] 
            current_feats['lag_1']  = simulated_sales
            # Công thức EMA = (Giá trị mới * alpha) + (EMA cũ * (1 - alpha)). Với span=7 -> alpha = 2/8 = 0.25
            current_feats['ema_7'] = (simulated_sales * 0.25) + (current_feats['ema_7'] * 0.75)

        expected_30d = sum([f['predicted_qty'] for f in future_forecasts])

        # --- D. LOGIC CẢNH BÁO (BẢO TOÀN TỪ V3) ---
        res_advice = None
        res_warning = None
        
        avg_daily_sell = expected_30d / 30
        days_to_sell_out = int(curr_stock / avg_daily_sell) if avg_daily_sell > 0 else 999
        
        # 1. Luồng Nhập hàng (Replenishment Advice)
        # Tính mức tồn kho mục tiêu (Target Stock) = 1.2 lần nhu cầu 30 ngày (đệm an toàn 20%)
        # Luôn đảm bảo Target Stock không được nhỏ hơn min_stock của sản phẩm
        target_stock = max(min_stock, int(expected_30d * 1.2))
        
        # Trừ hao số lượng hàng chuẩn bị vứt đi (đã hết hạn hoặc dưới 15 ngày)
        usable_stock = curr_stock
        if nearest_expiry < 15:
            usable_stock = max(0, curr_stock - qty_risk) # Coi như số hàng rủi ro không bán được
            
        # Kích hoạt lời khuyên khi kho hữu dụng thấp hơn mục tiêu
        if usable_stock < target_stock:
            suggested = int(target_stock - usable_stock)
            
            # Chỉ lên đơn khuyên nhập nếu số lượng > 0
            if suggested > 0:
                # Phân loại mức độ cấp bách
                if usable_stock <= min_stock or usable_stock < (expected_30d * 0.3):
                    priority = "CRITICAL" # Báo động đỏ: Kho sắp cạn hoặc thủng đáy
                elif usable_stock < (expected_30d * 0.7):
                    priority = "HIGH"     # Báo động cam: Cần nhập sớm
                else:
                    priority = "NORMAL"   # Bình thường: Nhập bù vào lượng đã bán
                    
                res_advice = {
                    'branch_id': branch_id, 'product_id': pid, 'current_stock': curr_stock,
                    'expected_demand_30d': expected_30d, 'suggested_qty': suggested,
                    'confidence_score': conf, 'priority_level': priority
                }
        
        # Sửa logic âm ngày (Đã hết hạn)
        if nearest_expiry < 0:
            res_warning = {
                'branch_id': branch_id, 'product_id': pid, 'current_stock': curr_stock,
                'days_to_exhaust': days_to_sell_out,
                'warning_type': 'EXPIRED_STOCK',
                'confidence_score': conf,
                'risk_reason': f"ACTION REQUIRED: Stock expired {abs(nearest_expiry)} days ago."
            }
        elif nearest_expiry >= 0 and days_to_sell_out > nearest_expiry:
            res_warning = {
                'branch_id': branch_id, 'product_id': pid, 'current_stock': curr_stock,
                'days_to_exhaust': days_to_sell_out,
                'warning_type': 'EXPIRY_RISK',
                'confidence_score': conf,
                'risk_reason': f"CRITICAL: Expiring in {nearest_expiry} days but needs {days_to_sell_out} days to sell."
            }
        elif qty_risk > (expected_30d * 1.5):
            res_warning = {
                'branch_id': branch_id, 'product_id': pid, 'current_stock': curr_stock,
                'days_to_exhaust': days_to_sell_out,
                'warning_type': 'PUSH_SALE_NEEDED',
                'confidence_score': conf,
                'risk_reason': f"Risk Qty ({qty_risk}) > 1.5x Monthly Demand ({expected_30d})."
            }
        elif curr_stock > (expected_30d * 3) and expected_30d > 0:
            res_warning = {
                'branch_id': branch_id, 'product_id': pid, 'current_stock': curr_stock,
                'days_to_exhaust': days_to_sell_out,
                'warning_type': 'DEADSTOCK',
                'confidence_score': conf,
                'risk_reason': f"Slow moving: Stock covers {days_to_sell_out} days."
            }

        return ("RESULT", res_advice, res_warning, future_forecasts, expected_30d, avg_daily_sell)

    except Exception as e:
        print(f"[LỖI] Branch {branch_id} - SKU {pid}: {str(e)}")
        return None

# =========================================================
# 3. [LUỒNG 4 - MỚI] PHÂN TÍCH RỦI RO TÀI CHÍNH THEO LÔ
#    (FEFO Simulation + Provisioning Value)
# =========================================================
def analyze_lot_financial_risk(df_lots, forecast_map):
    """
    Mô phỏng FEFO thực thụ: "Ăn" hàng theo thứ tự hết hạn,
    tính toán giá trị thiệt hại tài chính cho từng lô.

    Args:
        df_lots: DataFrame từ v_AI_Lot_Financial_Risk_Input
        forecast_map: dict { (branch_id, product_id): { 'demand_30d', 'daily_avg' } }

    Returns:
        List[dict]: Kết quả phân tích từng lô
    """
    if df_lots.empty:
        print("[LOT RISK] Không có dữ liệu lô hàng từ v_AI_Lot_Financial_Risk_Input.")
        return []

    lot_risks = []
    now = datetime.now()

    # Nhóm các lô theo (branch, product), sắp xếp FEFO
    grouped = df_lots.groupby(['branch_id', 'product_id'])

    for (branch_id, product_id), group in grouped:
        # Sắp xếp FEFO: Lô hết hạn sớm nhất lên đầu
        lots_sorted = group.sort_values('expiry_date').reset_index(drop=True)
        product_name = lots_sorted['name'].iloc[0]

        # Lấy dự báo từ XGBoost (nếu không có -> fallback = 0)
        forecast_info = forecast_map.get((branch_id, product_id), {'demand_30d': 0, 'daily_avg': 0})
        daily_avg = forecast_info['daily_avg']

        # Biến theo dõi lượng dự báo còn lại (mô phỏng FEFO tiêu thụ)
        remaining_demand = forecast_info['demand_30d']

        for _, lot in lots_sorted.iterrows():
            lot_qty = lot['quantity']
            cost_price = lot['cost_price']
            expiry_date = lot['expiry_date']
            days_to_expiry = (expiry_date - now).days

            # =====================================================
            # THUẬT TOÁN FEFO SIMULATION (Lõi nâng cấp)
            # =====================================================
            # Bước 1: Tính sức mua tối đa của thị trường
            #          cho đến khi lô này hết hạn
            if days_to_expiry <= 0:
                # Lô đã hết hạn -> Rủi ro 100%
                max_market_capacity = 0
            else:
                max_market_capacity = daily_avg * days_to_expiry

            # Bước 2: Lượng thực tế lô này được "ăn" bởi nhu cầu FEFO
            #          (Lô trước ăn trước, lô sau ăn phần còn lại)
            fefo_consumed = min(lot_qty, remaining_demand)

            # Bước 3: Lượng rủi ro = phần không bán được trước hạn
            #          So sánh tồn lô vs sức mua thực tế cho đến ngày hết hạn
            risk_qty = max(0, lot_qty - max_market_capacity)

            # Bước 4: Giá trị thiệt hại tài chính (Provision Value)
            provision_value = round(risk_qty * cost_price, 0)

            # Bước 5: Phân loại trạng thái lô
            if days_to_expiry <= 0:
                lot_status = 'EXPIRED'
                recommendation = 'Tiêu hủy / Thanh lý ngay'
            elif risk_qty >= lot_qty * 0.8:
                lot_status = 'DANGER'
                recommendation = 'Khuyến mãi flash sale / Đẩy hàng gấp'
            elif risk_qty > 0:
                lot_status = 'WARNING'
                recommendation = 'Giảm giá nhẹ / Ưu tiên xuất trước'
            else:
                lot_status = 'SAFE'
                recommendation = 'Bình thường'

            lot_risks.append({
                'branch_id': branch_id,
                'product_id': product_id,
                'product_name': product_name,
                'lot_id': lot['lot_id'],
                'quantity': int(lot_qty),
                'days_to_expiry': days_to_expiry,
                'cost_price': float(cost_price),
                'risk_qty': int(risk_qty),
                'provision_value': float(provision_value),
                'lot_status': lot_status,
                'recommendation': recommendation,
                'confidence_score': round(forecast_info.get('confidence', 75.0), 2)
            })

            # Cập nhật lượng demand còn lại cho lô tiếp theo
            remaining_demand = max(0, remaining_demand - fefo_consumed)

    return lot_risks

# =========================================================
# 4. MANAGER (QUẢN LÝ TIẾN TRÌNH)
# =========================================================
def main():
    start_time = time.time()
    print("=" * 60)
    print("[EMARKET V9.0] AI Forecast Engine (NCKH Edition)")
    print("  Modules: XGBoost Poisson | FEFO Simulation | Financial Risk")
    print("  Mode: Sequential (Windows-safe)")
    print("=" * 60)
    sys.stdout.flush()

    # Tạo engine trong main() để tránh subprocess spawn lại
    engine = _create_engine()
    
    # -------------------------------------------------------
    # PHASE 1: Load dữ liệu từ 2 SQL Views
    # -------------------------------------------------------
    try:
        with engine.connect() as conn:
            df_all = pd.read_sql("SELECT * FROM v_AI_Master_Input_XGB WITH (NOLOCK)", conn)
            df_lots = pd.read_sql("SELECT * FROM v_AI_Lot_Financial_Risk_Input WITH (NOLOCK)", conn)
    except Exception as e:
        print(f"[PHASE 1][LỖI] Không thể kết nối DB hoặc đọc view: {e}")
        sys.stdout.flush()
        return
    
    print(f"[PHASE 1] Loaded {len(df_all)} training rows + {len(df_lots)} lot rows.")
    sys.stdout.flush()

    if df_all.empty:
        print("[PHASE 1] Không có dữ liệu training. Dừng pipeline.")
        sys.stdout.flush()
        return

    # -------------------------------------------------------
    # PHASE 2: Huấn luyện XGBoost & Dự báo 30 ngày (Sequential - Windows Safe)
    # -------------------------------------------------------
    tasks = []
    for (pid, branch_id), df_group in df_all.groupby(['product_id', 'branch_id']):
        m_stock = df_group['min_stock'].iloc[0] if 'min_stock' in df_group.columns else 5
        tasks.append((pid, branch_id, df_group, m_stock))

    print(f"[PHASE 2] Training {len(tasks)} models (sequential)...")
    sys.stdout.flush()

    # Chạy tuần tự thay vì Pool.map để tránh deadlock trên Windows
    results = []
    for i, task in enumerate(tasks):
        try:
            r = train_and_dispatch_xgb(task)
            results.append(r)
        except Exception as e:
            print(f"[PHASE 2][LỖI] Task {i}: {e}")
            results.append(None)
        
        # In tiến trình mỗi 50 model
        if (i + 1) % 50 == 0:
            print(f"[PHASE 2] ... đã xong {i + 1}/{len(tasks)} models")
            sys.stdout.flush()

    advices, warnings_list, forecasts_list = [], [], []
    # Tạo bản đồ dự báo để Phase 3 sử dụng
    forecast_map = {}  # { (branch_id, product_id): { demand_30d, daily_avg, confidence } }

    for r in results:
        if r and r[0] == "RESULT":
            _, res_advice, res_warning, future_forecasts, expected_30d, avg_daily = r
            if res_advice: advices.append(res_advice)       
            if res_warning: warnings_list.append(res_warning) 
            if future_forecasts: 
                forecasts_list.extend(future_forecasts)
                # Ghi vào forecast_map cho Phase 3
                bid = future_forecasts[0]['branch_id']
                pid = future_forecasts[0]['product_id']
                conf = future_forecasts[0]['confidence_score']
                forecast_map[(bid, pid)] = {
                    'demand_30d': expected_30d,
                    'daily_avg': avg_daily,
                    'confidence': conf
                }

    print(f"[PHASE 2] Results: {len(advices)} Advices | {len(warnings_list)} Warnings | {len(forecasts_list)} Forecasts")
    sys.stdout.flush()

    # -------------------------------------------------------
    # PHASE 3: Phân tích rủi ro tài chính theo Lô (FEFO)
    # -------------------------------------------------------
    print(f"[PHASE 3] Analyzing {len(df_lots)} lots for financial risk (FEFO Simulation)...")
    sys.stdout.flush()
    lot_risks = analyze_lot_financial_risk(df_lots, forecast_map)

    # Thống kê nhanh
    if lot_risks:
        df_risk_summary = pd.DataFrame(lot_risks)
        total_provision = df_risk_summary['provision_value'].sum()
        danger_count = len(df_risk_summary[df_risk_summary['lot_status'].isin(['DANGER', 'EXPIRED'])])
        warning_count = len(df_risk_summary[df_risk_summary['lot_status'] == 'WARNING'])
        safe_count = len(df_risk_summary[df_risk_summary['lot_status'] == 'SAFE'])
        print(f"[PHASE 3] Lot Analysis: {danger_count} DANGER | {warning_count} WARNING | {safe_count} SAFE")
        print(f"[PHASE 3] >> TỔNG TRÍCH LẬP DỰ PHÒNG: {total_provision:,.0f} VNĐ")
    else:
        print("[PHASE 3] Không có dữ liệu lô hàng để phân tích.")
    sys.stdout.flush()

    # -------------------------------------------------------
    # PHASE 4: Ghi kết quả vào Database (4 bảng)
    # -------------------------------------------------------
    print(f"[PHASE 4] Saving to database...")
    sys.stdout.flush()
    
    try:
        with engine.begin() as conn:
            # Xóa dữ liệu cũ (4 bảng)
            conn.execute(text("TRUNCATE TABLE AI_ReplenishmentAdvice"))
            conn.execute(text("TRUNCATE TABLE AI_InventoryWarning"))
            conn.execute(text("TRUNCATE TABLE AI_SalesForecast"))
            conn.execute(text("""
                IF OBJECT_ID('AI_LotFinancialRisk', 'U') IS NOT NULL 
                    TRUNCATE TABLE AI_LotFinancialRisk
            """))
            
            # --- Luồng 1: Forecast ---
            if forecasts_list:
                df_forecast = pd.DataFrame(forecasts_list)
                df_forecast.to_sql('AI_SalesForecast', conn, if_exists='append', index=False, chunksize=5000, dtype=_nvarchar_dtype(df_forecast))
                
            # --- Luồng 2: Replenishment Advice ---
            if advices:
                df_adv = pd.DataFrame(advices)
                df_adv.to_sql('AI_ReplenishmentAdvice', conn, if_exists='append', index=False, dtype=_nvarchar_dtype(df_adv))
                
            # --- Luồng 3: Inventory Warning ---
            if warnings_list:
                df_warn = pd.DataFrame(warnings_list)
                df_warn.to_sql('AI_InventoryWarning', conn, if_exists='append', index=False, dtype=_nvarchar_dtype(df_warn))
                
            # --- Luồng 4 [MỚI]: Lot Financial Risk ---
            if lot_risks:
                df_lot = pd.DataFrame(lot_risks)
                df_lot.to_sql('AI_LotFinancialRisk', conn, if_exists='append', index=False, dtype=_nvarchar_dtype(df_lot))
    except Exception as e:
        print(f"[PHASE 4][LỖI] Ghi DB thất bại: {e}")
        sys.stdout.flush()
        return

    elapsed = round(time.time() - start_time, 2)
    print("=" * 60)
    print(f"PIPELINE HOÀN TẤT! Tổng thời gian: {elapsed}s")
    print(f"Forecasts:    {len(forecasts_list)} records")
    print(f"Advices:      {len(advices)} records")
    print(f"Warnings:     {len(warnings_list)} records")
    print(f"Lot Risks:    {len(lot_risks)} records")
    print("=" * 60)
    sys.stdout.flush()

if __name__ == "__main__":
    main()