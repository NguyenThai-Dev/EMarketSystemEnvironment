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
from datetime import timedelta, datetime 

# Tắt cảnh báo pandas cho sạch log
warnings.filterwarnings('ignore')

# =========================================================
# 1. KẾT NỐI DATABASE
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
        
        # 1. Luồng Nhập hàng (Replenishment Advice - V3.1)
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

        return ("RESULT", res_advice, res_warning, future_forecasts)

    except Exception as e:
        print(f"[LỖI] Branch {branch_id} - SKU {pid}: {str(e)}")
        return None

# =========================================================
# 3. MANAGER (QUẢN LÝ TIẾN TRÌNH)
# =========================================================
def main():
    start_time = time.time()
    print("💎 [EMARKET V8.0] AI Forecast Engine (Poisson Stochastic) Starting...")
    
    with engine.connect() as conn:
        df_all = pd.read_sql("SELECT * FROM v_AI_Master_Input_XGB WITH (NOLOCK)", conn)
    
    print(f"📦 Loaded {len(df_all)} rows. Grouping by SKU and Branch...")

    tasks = []
    for (pid, branch_id), df_group in df_all.groupby(['product_id', 'branch_id']):
        m_stock = df_group['min_stock'].iloc[0] if 'min_stock' in df_group.columns else 5
        tasks.append((pid, branch_id, df_group, m_stock))

    print(f"Processing {len(tasks)} combinations on {os.cpu_count()} cores...")

    with Pool(processes=os.cpu_count()) as pool:
        results = pool.map(train_and_dispatch_xgb, tasks)

    advices, warnings_list, forecasts_list = [], [], []

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
            df_forecast.to_sql('AI_SalesForecast', conn, if_exists='append', index=False, chunksize=5000)

    print(f"✨ DONE! Total Time: {round(time.time()-start_time, 2)}s")

if __name__ == "__main__":
    try: set_start_method('spawn', force=True)
    except: pass
    main()