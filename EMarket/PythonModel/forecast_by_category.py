import os
import pandas as pd
import numpy as np
import xgboost as xgb
import urllib
import time
import warnings
from sqlalchemy import create_engine, text
from multiprocessing import Pool, set_start_method
from sklearn.metrics import r2_score
# [UPDATE] Import datetime để lấy thời gian thực
from datetime import timedelta, datetime 

# Tắt cảnh báo pandas cho sạch log
warnings.filterwarnings('ignore')

# =========================================================
# 1. KẾT NỐI
# =========================================================
params = urllib.parse.quote_plus(
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=127.0.0.1,1433;"
    "Database=EMarket_DB;"
    "UID=root_admin;"
    "PWD=SqlConnections2026!;"
    "TrustServerCertificate=yes;"
)
engine = create_engine(f"mssql+pyodbc:///?odbc_connect={params}", pool_size=20, max_overflow=0)

# =========================================================
# 2. HÀM XỬ LÝ CHÍNH: 1 MODEL -> 3 ĐẦU RA (ADVICE, WARNING, FORECAST)
# =========================================================
def train_and_dispatch_xgb(data_tuple):
    pid, branch_id, df_p, min_stock = data_tuple
    
    # Lấy thông tin Hạn sử dụng (Giữ nguyên tính năng cũ)
    curr_stock = df_p["current_stock"].iloc[0]
    nearest_expiry = df_p["nearest_expiry_days"].iloc[0] if "nearest_expiry_days" in df_p.columns else 9999
    qty_risk = df_p["qty_risk"].iloc[0] if "qty_risk" in df_p.columns else 0
    
    # Yêu cầu tối thiểu 45 điểm dữ liệu để train
    if len(df_p) < 45:
        return None

    try:
        # --- A. FEATURE ENGINEERING (CHO TRAINING) ---
        df_p = df_p.sort_values('ds').reset_index(drop=True)
        
        # Tạo Lags và Rolling Mean
        for i in [1, 7, 30]:
            df_p[f'lag_{i}'] = df_p['y'].shift(i)
        df_p['rolling_mean_7'] = df_p['y'].shift(1).rolling(window=7).mean()
        
        # Loại bỏ NaN sau khi shift
        df_clean = df_p.dropna().copy()
        
        # Features (Giữ nguyên)
        features = ['day_of_month', 'month', 'day_of_week', 'is_weekend', 'is_payday', 'is_festive', 
                    'lag_1', 'lag_7', 'lag_30', 'rolling_mean_7']
        
        # --- B. TRAINING MODEL ---
        model = xgb.XGBRegressor(n_estimators=100, max_depth=5, learning_rate=0.1, objective='reg:squarederror', n_jobs=1)
        model.fit(df_clean[features], df_clean['y'])
        
        # Tính độ tin cậy
        preds = model.predict(df_clean[features])
        r2 = r2_score(df_clean['y'], preds)
        conf = max(0, min(99.9, (r2 * 100) + 15)) 

        # --- C. RECURSIVE FORECAST (DỰ BÁO CUỐN CHIẾU 30 NGÀY TỪ HIỆN TẠI) ---
        future_forecasts = []
        
        # Lấy bộ feature cuối cùng của lịch sử để làm đà
        current_feats = df_clean.iloc[-1].copy()
        
        # [UPDATE QUAN TRỌNG] Neo thời gian bắt đầu từ HÔM NAY (Real-time)
        # Thay vì lấy last_date của dữ liệu, ta lấy datetime.now()
        start_forecast_date = datetime.now().replace(hour=0, minute=0, second=0, microsecond=0)
        
        # Vòng lặp 30 ngày tương lai
        for day_step in range(1, 31):
            # Tính ngày tiếp theo dựa trên HÔM NAY
            next_date = start_forecast_date + timedelta(days=day_step)
            
            m = next_date.month
            d = next_date.day
            wd = next_date.weekday() # 0=Monday, 6=Sunday
            
            # 1. Cập nhật các biến thời gian cơ bản
            current_feats['day_of_month'] = d
            current_feats['month'] = m
            current_feats['day_of_week'] = wd
            
            # [Logic Weekend] T7(5), CN(6)
            current_feats['is_weekend'] = 1 if wd >= 5 else 0
            
            # [Logic Payday] Ngày 15 và 30 (hoặc logic khác tùy bro)
            current_feats['is_payday'] = 1 if d in [15, 30] else 0
            
            # 2. [UPDATE] Logic Festive (Đồng bộ 100% với SQL View)
            is_festive = 0
            # Giáp Tết (Tháng 1 sau ngày 15)
            if (m == 1 and d > 15): is_festive = 1        
            # Tết (Tháng 2 trước ngày 15)
            elif (m == 2 and d < 15): is_festive = 1      
            # 30/4 (Tháng 4 từ ngày 28)
            elif (m == 4 and d >= 28): is_festive = 1     
            # 1/5 (Tháng 5 đầu tháng)
            elif (m == 5 and d <= 2): is_festive = 1      
            # 2/9 (Quốc khánh)
            elif (m == 9 and d == 2): is_festive = 1      
            # Black Friday (Cuối tháng 11)
            elif (m == 11 and d >= 25): is_festive = 1    
            # Noel & Tết Dương (Cuối tháng 12)
            elif (m == 12 and d >= 20): is_festive = 1    
            
            current_feats['is_festive'] = is_festive
            
            # 3. Dự báo
            pred_val = model.predict(pd.DataFrame([current_feats[features]]))[0]
            pred_val = max(0, pred_val) 
            
            # Lưu kết quả
            future_forecasts.append({
                'branch_id': branch_id,
                'product_id': pid,
                'forecast_date': next_date,
                'predicted_qty': int(round(pred_val)),
                'confidence_score': conf
            })
            
            # CẬP NHẬT LAGS (Recursive)
            current_feats['lag_30'] = current_feats['lag_7'] 
            current_feats['lag_7']  = current_feats['lag_1'] 
            current_feats['lag_1']  = pred_val 
            current_feats['rolling_mean_7'] = (current_feats['rolling_mean_7'] * 6 + pred_val) / 7

        # Tổng nhu cầu 30 ngày
        expected_30d = sum([f['predicted_qty'] for f in future_forecasts])

        # --- D. LOGIC PHÂN TÁCH & CẢNH BÁO (GIỮ NGUYÊN) ---
        res_advice = None
        res_warning = None
        
        avg_daily_sell = expected_30d / 30
        days_to_sell_out = int(curr_stock / avg_daily_sell) if avg_daily_sell > 0 else 999
        
        # 1. Luồng Nhập hàng
        if (curr_stock < (expected_30d * 0.8) or curr_stock < min_stock) and (nearest_expiry > 60):
            suggested = max(0, expected_30d - curr_stock)
            priority = "High" if curr_stock < (expected_30d * 0.3) else "Normal"
            res_advice = {
                'branch_id': branch_id, 'product_id': pid, 'current_stock': curr_stock,
                'expected_demand_30d': expected_30d, 'suggested_qty': suggested,
                'confidence_score': conf, 'priority_level': priority
            }
        
        # 2. Luồng Cảnh báo (3 Cấp độ)
        if days_to_sell_out > nearest_expiry:
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

        return ("RESULT", res_advice, res_warning, future_forecasts)

    except Exception as e:
        return None

# =========================================================
# 3. MANAGER (QUẢN LÝ TIẾN TRÌNH)
# =========================================================
def main():
    start_time = time.time()
    print("💎 [EMARKET V7.5] AI Engine (Real-time Anchor) Starting...")
    
    with engine.connect() as conn:
        df_all = pd.read_sql("SELECT * FROM v_AI_Master_Input_XGB WITH (NOLOCK)", conn)
    
    print(f"📦 Loaded {len(df_all)} rows. Grouping...")

    tasks = []
    for (pid, branch_id), df_group in df_all.groupby(['product_id', 'branch_id']):
        m_stock = df_group['min_stock'].iloc[0] if 'min_stock' in df_group.columns else 5
        tasks.append((pid, branch_id, df_group, m_stock))

    print(f"🚀 Processing {len(tasks)} SKUs on {os.cpu_count()} cores...")

    with Pool(processes=os.cpu_count()) as pool:
        results = pool.map(train_and_dispatch_xgb, tasks)

    advices = []
    warnings_list = []
    forecasts_list = []

    for r in results:
        if r and r[0] == "RESULT":
            if r[1]: advices.append(r[1])       
            if r[2]: warnings_list.append(r[2]) 
            if r[3]: forecasts_list.extend(r[3]) 

    print(f"💾 Saving: {len(advices)} Advices | {len(warnings_list)} Warnings | {len(forecasts_list)} Forecasts...")
    
    with engine.begin() as conn:
        conn.execute(text("TRUNCATE TABLE AI_ReplenishmentAdvice"))
        conn.execute(text("TRUNCATE TABLE AI_InventoryWarning"))
        conn.execute(text("TRUNCATE TABLE AI_SalesForecast"))
        
        if advices:
            pd.DataFrame(advices).to_sql('AI_ReplenishmentAdvice', conn, if_exists='append', index=False)
        if warnings_list:
            pd.DataFrame(warnings_list).to_sql('AI_InventoryWarning', conn, if_exists='append', index=False)
        if forecasts_list:
            df_forecast = pd.DataFrame(forecasts_list)
            df_forecast.to_sql('AI_SalesForecast', conn, if_exists='append', index=False, chunksize=1000)

    print(f"✨ DONE! Total Time: {round(time.time()-start_time, 2)}s")

if __name__ == "__main__":
    try: set_start_method('spawn', force=True)
    except: pass
    main()