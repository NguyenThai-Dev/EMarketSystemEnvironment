import streamlit as st
import pandas as pd
import urllib
import plotly.express as px
from sqlalchemy import create_engine

# --- CẤU HÌNH TRANG TỔNG QUAN ---
st.set_page_config(page_title="EMarket AI Analytics", page_icon="📈", layout="wide")
st.title("📈 EMarket - AI Financial & Inventory Dashboard")
st.markdown("Bảng điều khiển nhanh kiểm tra kết quả từ AI Forecast Engine.")

# --- KẾT NỐI DATABASE (Tự động cache để truy xuất nhanh) ---
@st.cache_resource
def init_connection():
    params = urllib.parse.quote_plus(
        "Driver={ODBC Driver 17 for SQL Server};"
        "Server=127.0.0.1,1433;"
        "Database=EMarket_DB;"
        "UID=root_admin;"
        "PWD=SqlConnections2026!;"
        "TrustServerCertificate=yes;"
    )
    return create_engine(f"mssql+pyodbc:///?odbc_connect={params}")

engine = init_connection()

# --- TRUY XUẤT DỮ LIỆU ---
@st.cache_data(ttl=60) # Tự động làm mới data mỗi 60s
def load_data():
    with engine.connect() as conn:
        df_risk = pd.read_sql("SELECT * FROM AI_LotFinancialRisk ORDER BY provision_value DESC", conn)
        df_advice = pd.read_sql("SELECT * FROM AI_ReplenishmentAdvice ORDER BY suggested_qty DESC", conn)
        df_warning = pd.read_sql("SELECT * FROM AI_InventoryWarning", conn)
        df_forecast = pd.read_sql("SELECT * FROM AI_SalesForecast", conn)
    return df_risk, df_advice, df_warning, df_forecast

df_risk, df_advice, df_warning, df_forecast = load_data()

# --- THỐNG KÊ TỔNG QUAN (KPIs) ---
total_provision = df_risk['provision_value'].sum() if not df_risk.empty else 0
total_items_to_buy = df_advice['product_id'].nunique() if not df_advice.empty else 0
critical_warnings = len(df_warning[df_warning['warning_type'] == 'EXPIRED_STOCK']) if not df_warning.empty else 0

col1, col2, col3 = st.columns(3)
col1.metric("Tổng Trích Lập Dự Phòng (VNĐ)", f"{total_provision:,.0f} ₫", "Rủi ro tài chính lô hàng", delta_color="inverse")
col2.metric("Sản Phẩm Cần Nhập", f"{total_items_to_buy}", "Mã SKU", delta_color="off")
col3.metric("Cảnh Báo Hết Hạn Khẩn", f"{critical_warnings}", "Lô hàng", delta_color="inverse")

st.markdown("---")

# --- HIỂN THỊ CHI TIẾT THEO TAB ---
tab1, tab2, tab3, tab4 = st.tabs(["💰 Rủi Ro Tài Chính (Lô Hàng)", "🛒 Khuyên Nhập Hàng", "⚠️ Cảnh Báo Tồn Kho", "📈 Dự Báo Doanh Số 30 Ngày"])

with tab1:
    st.subheader("Phân tích Rủi ro Tài chính theo chuẩn FEFO")
    if not df_risk.empty:
        # Highlight các dòng có rủi ro cao (Provision > 1.000.000)
        def highlight_high_risk(s):
            return ['background-color: #ffcccc' if v > 1000000 else '' for v in s]
        
        st.dataframe(df_risk.style.apply(highlight_high_risk, subset=['provision_value']), use_container_width=True)
    else:
        st.success("Tuyệt vời! Không có rủi ro tài chính nào được phát hiện.")

with tab2:
    st.subheader("Khuyến nghị Bổ sung Hàng hóa (AI Replenishment)")
    if not df_advice.empty:
        # Lọc nhanh theo độ ưu tiên
        priority_filter = st.multiselect("Lọc theo mức độ ưu tiên:", options=df_advice['priority_level'].unique(), default=df_advice['priority_level'].unique())
        filtered_advice = df_advice[df_advice['priority_level'].isin(priority_filter)]
        st.dataframe(filtered_advice, use_container_width=True)
    else:
        st.info("Kho hàng đang ở trạng thái an toàn.")

with tab3:
    st.subheader("Cảnh báo Tồn kho")
    if not df_warning.empty:
        st.dataframe(df_warning, use_container_width=True)
    else:
        st.info("Không có cảnh báo nào.")

with tab4:
    st.subheader("Biểu đồ Dự báo Doanh số (XGBoost Poisson)")
    if not df_forecast.empty:
        # Gom nhóm tổng dự báo theo ngày để vẽ biểu đồ
        df_trend = df_forecast.groupby('forecast_date')['predicted_qty'].sum().reset_index()
        fig = px.line(df_trend, x='forecast_date', y='predicted_qty', title="Tổng nhu cầu dự báo toàn hệ thống", markers=True)
        fig.update_layout(xaxis_title="Ngày", yaxis_title="Số lượng dự báo")
        st.plotly_chart(fig, use_container_width=True)
        
        st.markdown("**Chi tiết theo SKU:**")
        st.dataframe(df_forecast, use_container_width=True)
    else:
        st.info("Chưa có dữ liệu dự báo.")