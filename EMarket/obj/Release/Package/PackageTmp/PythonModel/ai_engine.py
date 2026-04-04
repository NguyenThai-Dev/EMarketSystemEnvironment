import pandas as pd
import numpy as np
from prophet import Prophet
from sqlalchemy import create_engine, text
import urllib
import datetime
from lunarcalendar import Converter, Lunar
import holidays as pyholidays
import warnings
from sqlalchemy.types import NVARCHAR

# Tắt các cảnh báo không cần thiết của Prophet
warnings.filterwarnings("ignore")
import logging
logging.getLogger('cmdstanpy').setLevel(logging.WARNING)

# ==========================================
# 1. CẤU HÌNH KẾT NỐI SQL SERVER
# ==========================================
params = urllib.parse.quote_plus(
   "Driver={ODBC Driver 17 for SQL Server};"
    "Server=127.0.0.1,1433;"
    "Database=EMarket_DB;"
    "UID=root_admin;"
    "PWD=SqlConnections2026!;"
    "TrustServerCertificate=yes;"
)
engine = create_engine(f"mssql+pyodbc:///?odbc_connect={params}&charset=utf8")
# CẤU HÌNH AI
FORECAST_DAYS = 30       # Dự báo 30 ngày tới
SAFETY_BUFFER = 0.2      # Mua dư 20% (Safety Stock)
TRAINING_WINDOW = 1200    # [UPGRADE A] Chỉ học 900 ngày (2.5 năm) gần nchất

# ==========================================
# 2. HÀM TỰ ĐỘNG TÍNH LỊCH (DƯƠNG + ÂM)
# ==========================================
def generate_smart_holidays(start_year, end_year):
    holiday_list = []
    
    # A. Lễ Dương Lịch (Noel, 1/1, 30/4...)
    try:
        vn_holidays = pyholidays.VN(years=range(start_year, end_year + 2))
        for date, name in vn_holidays.items():
            holiday_list.append({
                'holiday': 'le_duong_lich',
                'ds': pd.to_datetime(date),
                'lower_window': 0, 'upper_window': 1
            })
    except Exception as e:
        print(f"⚠️ Không tải được lịch dương: {e}")

    # B. Lễ Âm Lịch (Tết Nguyên Đán)
    # print(f"🔹 Đang tính toán lịch Âm từ {start_year} đến {end_year + 1}...")
    for year in range(start_year, end_year + 2):
        lunar_date = Lunar(year, 1, 1)
        solar_date = Converter.Lunar2Solar(lunar_date)
        
        if solar_date:
            holiday_list.append({
                'holiday': 'tet_nguyen_dan',
                'ds': pd.to_datetime(f"{solar_date.year}-{solar_date.month}-{solar_date.day}"),
                'lower_window': -14, # Kích cầu từ 2 tuần trước Tết
                'upper_window': 5,   # Hết mùng 5
            })

    return pd.DataFrame(holiday_list)

# ==========================================
# 3. TẢI DỮ LIỆU TỪ SQL
# ==========================================
def load_data():
    print("🔹 Đang tải dữ liệu từ SQL (Bảng AI_Training_Data)...")
    sql = "SELECT branch_id, product_id, sale_date, qty FROM AI_Training_Data"
    df = pd.read_sql(sql, engine)
    df['sale_date'] = pd.to_datetime(df['sale_date'])
    return df

# ==========================================
# 4. TRAINING & DỰ BÁO (CORE ENGINE)
# ==========================================
def run_forecasting_pipeline(df):
    results = []
    
    # --- [UPGRADE A] ROLLING WINDOW FILTERING ---
    if not df.empty:
        max_date = df['sale_date'].max()
        cutoff_date = max_date - pd.Timedelta(days=TRAINING_WINDOW)
        # Chỉ giữ lại dữ liệu mới, bỏ dữ liệu quá cũ (> 2.5 năm)
        df_filtered = df[df['sale_date'] >= cutoff_date]
        print(f"🔹 Áp dụng Rolling Window: Lọc dữ liệu từ {cutoff_date.date()} đến {max_date.date()}")
    else:
        return pd.DataFrame()

    # Sinh lịch cho Prophet
    min_year = df_filtered['sale_date'].dt.year.min()
    max_year = datetime.datetime.now().year + 1
    holidays_df = generate_smart_holidays(min_year, max_year)
    
    grouped = df_filtered.groupby(['branch_id', 'product_id'])
    total = len(grouped)
    count = 0
    
    print(f"🚀 Bắt đầu training cho {total} sản phẩm...")
    
    for (branch_id, product_id), group in grouped:
        count += 1
        
        # Prophet cần ít nhất ~2 chu kỳ tuần hoặc 30 điểm dữ liệu để chạy ổn
        if len(group) < 30: 
            # (TODO: Có thể thêm logic fallback dùng trung bình cộng ở đây nếu muốn)
            continue 
        
        train_df = group[['sale_date', 'qty']].rename(columns={'sale_date': 'ds', 'qty': 'y'})
        
        # --- [UPGRADE B] TUNING HYPERPARAMETERS ---
        m = Prophet(
            holidays=holidays_df,
            weekly_seasonality=True,
            yearly_seasonality=True,
            daily_seasonality=False,
            seasonality_mode='multiplicative', # Bán lẻ: Biến động tăng theo doanh số
            
            # Tăng trọng số ngày lễ (cho phép Tết tăng vọt)
            holidays_prior_scale=20.0, 
            
            # Tăng độ linh hoạt trend (Bắt trend tăng/giảm nhanh hơn)
            changepoint_prior_scale=0.1 
        )
        
        try:
            m.fit(train_df)
            
            future = m.make_future_dataframe(periods=FORECAST_DAYS)
            forecast = m.predict(future).tail(FORECAST_DAYS)
            
            for _, row in forecast.iterrows():
                # Xử lý số âm (Doanh số ko thể âm)
                yhat = max(0, row['yhat'])
                y_upper = max(0, row['yhat_upper'])
                y_lower = max(0, row['yhat_lower'])
                
                # Tính độ tin cậy (Confidence Score)
                # Khoảng dự báo (Spread) càng hẹp -> Tin cậy càng cao
                spread = y_upper - y_lower
                confidence = 0
                if y_upper > 0:
                    confidence = max(0, 100 - (spread / y_upper * 100))
                
                results.append({
                    'branch_id': branch_id,
                    'product_id': product_id,
                    'forecast_date': row['ds'],
                    'expected_qty': int(yhat),
                    'lower_qty': int(y_lower),
                    'upper_qty': int(y_upper),
                    'confidence_score': round(confidence, 2),
                    'created_at': datetime.datetime.now()
                })
                
        except Exception as e:
            # print(f"⚠️ Lỗi sản phẩm {product_id}: {e}")
            pass

        if count % 10 == 0: print(f"   ...Đã xử lý {count}/{total} sản phẩm")
            
    return pd.DataFrame(results)

# ==========================================
# 5. TÍNH TOÁN GỢI Ý NHẬP HÀNG
# ==========================================
def build_advice_data():
    print("🔹 Đang tính toán gợi ý nhập hàng...")
    
    # 1. Lấy tồn kho thực tế
    sql_stock = """
        SELECT w.branch_id, pl.product_id, ISNULL(SUM(i.quantity), 0) as current_stock
        FROM Inventory i
        JOIN Warehouses w ON i.warehouse_id = w.warehouse_id
        JOIN ProductLots pl ON i.lot_id = pl.lot_id
        GROUP BY w.branch_id, pl.product_id
    """
    stock_df = pd.read_sql(sql_stock, engine)
    
    # 2. Lấy nhu cầu dự báo (Tổng 30 ngày)
    sql_demand = """
        SELECT branch_id, product_id, SUM(expected_qty) as expected_demand, AVG(confidence_score) as avg_conf
        FROM AI_ForecastResults
        GROUP BY branch_id, product_id
    """
    demand_df = pd.read_sql(sql_demand, engine)
    
    if demand_df.empty:
        print("⚠️ Không có dữ liệu dự báo để tính toán nhập hàng.")
        return pd.DataFrame()

    # 3. Merge và tính toán
    merged = pd.merge(demand_df, stock_df, on=['branch_id', 'product_id'], how='left')
    merged['current_stock'] = merged['current_stock'].fillna(0)
    
    advice_list = []
    for _, row in merged.iterrows():
        safety_stock = int(row['expected_demand'] * SAFETY_BUFFER)
        suggested = int(row['expected_demand'] + safety_stock - row['current_stock'])
        
        if suggested > 0:
            conf_level = 'High' if row['avg_conf'] >= 80 else ('Medium' if row['avg_conf'] >= 50 else 'Low')
            
            # --- LOGIC SINH LÝ DO (EXPLAINABILITY) ---
            reasons = []
            
            # Lý do 1: Hết kho
            if row['current_stock'] == 0:
                reasons.append("Kho đã hết sạch")
            elif row['current_stock'] < safety_stock:
                reasons.append("Chạm ngưỡng an toàn")
                
            # Lý do 2: Mùa vụ (Check tháng hiện tại có phải tháng Tết/Lễ không)
            # (Giả lập logic check ngày, thực tế có thể check holiday df)
            current_month = datetime.datetime.now().month
            if current_month in [1, 2, 12]: # Tháng cao điểm
                reasons.append("Cao điểm Mùa vụ/Tết")
            
            # Lý do 3: Nhu cầu lớn
            if row['expected_demand'] > 100: # Ngưỡng ví dụ
                reasons.append("Sức mua đang tăng mạnh")
            
            # Ghép lý do
            reason_str = " + ".join(reasons) if reasons else "Bổ sung định kỳ"

            advice_list.append({
                'branch_id': row['branch_id'],
                'product_id': row['product_id'],
                'current_stock': int(row['current_stock']),
                'expected_demand': int(row['expected_demand']),
                'safety_stock': safety_stock,
                'suggested_qty': max(0, suggested),
                'confidence_level': conf_level,
                'reason': reason_str, # <--- Cột mới
                'created_at': datetime.datetime.now()
            })
            
    return pd.DataFrame(advice_list)

# ==========================================
# 6. LƯU VÀO DB & MAIN EXECUTION
# ==========================================
# ==========================================
# 6. LƯU VÀO DB (PHIÊN BẢN FIX UNICODE)
# ==========================================
def save_to_sql(df, table_name):
    if df.empty: return
    
    # Định nghĩa kiểu dữ liệu cho các cột Text
    # Chỉ định rõ: reason là NVARCHAR (Unicode)
    dtype_mapping = {
        'reason': NVARCHAR(length=500),
        'confidence_level': NVARCHAR(length=50)
    }

    print(f"🔹 Đang lưu {len(df)} dòng vào {table_name}...")
    
    with engine.begin() as conn:
        # Xóa dữ liệu cũ
        conn.execute(text(f"DELETE FROM {table_name}")) 
        
        # Lưu mới với dtype map
        df.to_sql(
            table_name, 
            conn, 
            if_exists='append', 
            index=False,
            dtype=dtype_mapping # <--- ĐÂY LÀ CHÌA KHÓA FIX LỖI
        )
        
    print(f"✅ Đã lưu thành công vào bảng {table_name}")

if __name__ == "__main__":
    print("--- START AI ENGINE V2 (UPGRADED) ---")
    
    # 1. Load Data
    raw_data = load_data()
    
    if raw_data.empty:
        print("❌ Lỗi: Không có dữ liệu training. Hãy chạy script SQL Data Generator trước.")
    else:
        # 2. Forecast (Core)
        forecast_result = run_forecasting_pipeline(raw_data)
        save_to_sql(forecast_result, 'AI_ForecastResults')
        
        # 3. Advice (Business Logic)
        advice_result = build_advice_data()
        save_to_sql(advice_result, 'AI_ReplenishmentAdvice')
        
    print("--- FINISHED ---")