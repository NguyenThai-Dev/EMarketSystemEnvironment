using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EMarket.Forecast.DTOs;
using EMarket.Forecast.Services.Interfaces;
using static EMarket.Forecast.DTOs.AIReplenishmentDTO;

namespace EMarket.Forecast.Services.Implementations
{
    public class AIService : IAIService
    {
        // Nếu bạn muốn tận dụng connection string từ EMarketContext
        private readonly string _connectionString;

        public AIService()
        {
            _connectionString = ConfigurationManager
           .ConnectionStrings["EMarket_Connections"]
           .ConnectionString;
        }

        // 1. Chạy phân tích (Gọi Stored Procedure)
        public async Task<bool> RunAnalysisAsync(int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    // Dapper gọi SP cực ngắn gọn
                    var p = new DynamicParameters();
                    p.Add("@TargetBranchId", branchId);

                    await conn.ExecuteAsync("sp_AI_Run_Full_Analysis", p, commandType: CommandType.StoredProcedure);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // 2. Lấy danh sách gợi ý nhập hàng (Mapping tự động vào DTO)
        public async Task<List<AI_RecommendationDTO>> GetRecommendationsAsync(int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        r.id AS Id,
                        r.product_id AS ProductId,
                        p.name AS ProductName,
                        c.name AS CategoryName,
                        p.unit AS Unit,
                        r.forecast_demand AS ForecastDemand,
                        r.current_stock AS CurrentStock,
                        r.recommended_min AS RecommendedMin,
                        r.recommended_max AS RecommendedMax,
                        r.reason AS Reason,
                        r.confidence_level AS ConfidenceLevel,
                        CASE 
                            WHEN r.reason LIKE N'%Bất thường%' THEN N'🔥'
                            WHEN r.reason LIKE N'%Mùa vụ%' THEN N'📅'
                            WHEN r.reason LIKE N'%hết hàng%' THEN N''
                            ELSE N''
                        END AS InsightIcon
                    FROM AI_Purchase_Recommendation r
                    JOIN Products p ON r.product_id = p.product_id
                    LEFT JOIN ProductCategories c ON r.category_id = c.category_id
                    WHERE r.branch_id = @branchId
                    ORDER BY r.confidence_level DESC, r.recommended_min DESC";

                // Dapper tự map column name sang property của DTO
                var result = await conn.QueryAsync<AI_RecommendationDTO>(sql, new { branchId = branchId });
                return result.AsList();
            }
        }

        // 3. Lấy danh sách bất thường
        public async Task<List<AI_AnomalyDTO>> GetAnomaliesAsync(int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        a.category_id AS CategoryId,
                        c.name AS CategoryName,
                        a.actual_qty AS ActualQty,
                        a.forecast_qty AS ForecastQty,
                        a.deviation_percent AS DeviationPercent,
                        a.anomaly_type AS AnomalyType,
                        a.severity AS Severity
                    FROM AI_Anomaly_Category a
                    JOIN ProductCategories c ON a.category_id = c.category_id
                    WHERE a.branch_id = @branchId
                    ORDER BY a.deviation_percent DESC";

                var result = await conn.QueryAsync<AI_AnomalyDTO>(sql, new { branchId = branchId });
                return result.AsList();
            }
        }

        // 4. Lấy Insight sản phẩm
        public async Task<List<AI_InsightDTO>> GetProductInsightsAsync(int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        i.product_id AS ProductId,
                        p.name AS ProductName,
                        i.growth_percent AS GrowthPercent,
                        i.contribution_percent AS ContributionPercent,
                        i.insight_level AS InsightLevel
                    FROM AI_Product_Insight i
                    JOIN Products p ON i.product_id = p.product_id
                    WHERE i.branch_id = @branchId
                    ORDER BY i.growth_percent DESC";

                var result = await conn.QueryAsync<AI_InsightDTO>(sql, new { branchId = branchId });
                return result.AsList();
            }
        }


        // 1. Chạy quy trình AI toàn diện
        public async Task<bool> RunAIPipelineAsync()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    // Bước A: Làm sạch dữ liệu đầu vào (Gọi SP SQL)
                    await conn.ExecuteAsync("sp_AI_Refresh_Training_Data", commandType: CommandType.StoredProcedure);
                }

                // Bước B: Gọi Script Python chạy ngầm

                // Lấy đường dẫn gốc của project EMarket
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // Đường dẫn tới python.exe (nằm trong venv bên trong PythonModel)
                string pythonExePath = Path.Combine(baseDir, "PythonModel", "Python", "venv", "Scripts", "python.exe");

                // Đường dẫn tới file script ai_engine.py
                string scriptPath = Path.Combine(baseDir, "PythonModel", "forecast_by_category.py");

                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = pythonExePath;
                start.Arguments = scriptPath;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true; // Đọc log trả về
                start.CreateNoWindow = true; // Chạy ẩn không hiện cửa sổ đen

                using (Process process = Process.Start(start))
                {
                    // Đọc log để debug nếu cần
                    string result = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit();

                    // Kiểm tra xem Python có chạy xong không
                    if (process.ExitCode == 0) return true;
                    else
                    {
                        // Log lỗi ở đây nếu cần (result chứa log)
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi hệ thống
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // 2. Lấy dữ liệu hiển thị lên Chatbox/Dashboard
        public async Task<List<AIReplenishmentDTO>> GetReplenishmentAdviceAsync(int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
                    SELECT 
                        r.product_id AS ProductId,
                        p.name AS ProductName,
                        c.name AS CategoryName,
                        p.unit AS Unit,
                        p.image AS ProductImage,
                        r.current_stock AS CurrentStock,
                        r.expected_demand AS ExpectedDemand,
                        r.safety_stock AS SafetyStock,
                        r.suggested_qty AS SuggestedQty,
                        r.confidence_level AS ConfidenceLevel,
                        r.reason AS Reason
                    FROM AI_ReplenishmentAdvice r
                    JOIN Products p ON r.product_id = p.product_id
                    LEFT JOIN ProductCategories c ON p.category_id = c.category_id
                    WHERE r.branch_id = @BranchId
                    ORDER BY 
                        -- Ưu tiên tin cậy cao và số lượng nhập lớn lên đầu
                        CASE WHEN r.confidence_level = 'HIGH' THEN 1 
                             WHEN r.confidence_level = 'MEDIUM' THEN 2 
                             ELSE 3 END,
                        r.suggested_qty DESC";

                var result = await conn.QueryAsync<AIReplenishmentDTO>(sql, new { BranchId = branchId });
                return result.AsList();
            }
        }

        public async Task<IReadOnlyList<ProductHistoryDTO>> GetProductHistoryAsync(
        int productId,
        int branchId,
        DateTime startDate,
        DateTime endDate
    )
        {
            const string sql = @"
            SELECT 
                sale_date AS [Date],
                SUM(qty) AS Qty
            FROM AI_Training_Data
            WHERE product_id = @ProductId
              AND branch_id = @BranchId
              AND sale_date BETWEEN @StartDate AND @EndDate
            GROUP BY sale_date
            ORDER BY sale_date ASC;
        ";

            using (var conn = new SqlConnection(_connectionString))
            {
                var result = await conn.QueryAsync<ProductHistoryDTO>(sql, new
                {
                    ProductId = productId,
                    BranchId = branchId,
                    StartDate = startDate,
                    EndDate = endDate
                });

                return result.ToList();
            }
        }

        // 1. Lấy dự báo nhập hàng (Dữ liệu từ AI_ReplenishmentAdvice)
        public async Task<List<AI_InventoryForecastDTO>> GetInventoryForecastAsync(int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
            SELECT 
                r.product_id AS ProductId,
                p.name AS ProductName,
                r.current_stock AS CurrentStock,
                r.expected_demand_30d AS ExpectedDemand30d,
                r.suggested_qty AS SuggestedQty,
                r.confidence_score AS ConfidenceScore,
                r.priority_level AS PriorityLevel
            FROM AI_ReplenishmentAdvice r WITH (NOLOCK)
            JOIN Products p ON r.product_id = p.product_id
            WHERE r.branch_id = @branchId
            ORDER BY r.suggested_qty DESC";

                var result = await conn.QueryAsync<AI_InventoryForecastDTO>(sql, new { branchId });
                return result.AsList();
            }
        }

        // 2. Lấy cảnh báo rủi ro (Dữ liệu từ AI_InventoryWarning)
        public async Task<List<AI_DeadstockDTO>> GetDeadstockAnalysisAsync(int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
            SELECT 
                p.name AS ProductName,
                w.current_stock AS CurrentStock,
                w.days_to_exhaust AS DaysToExhaust,
                w.warning_type AS WarningType,
                w.risk_reason AS RiskReason,
                w.confidence_score AS ConfidenceScore, -- Bổ sung thêm cột này cho đủ bộ
                CASE 
                    WHEN w.warning_type LIKE N'%tồn đọng%' OR w.warning_type LIKE N'%Deadstock%' THEN N'Xả hàng/Khuyến mãi'
                    WHEN w.days_to_exhaust < 7 THEN N'Nhập hàng khẩn cấp'
                    ELSE N'Theo dõi định kỳ'
                END AS Recommendation
            FROM AI_InventoryWarning w WITH (NOLOCK)
            JOIN Products p ON w.product_id = p.product_id
            WHERE w.branch_id = @branchId
            ORDER BY w.days_to_exhaust ASC";

                var result = await conn.QueryAsync<AI_DeadstockDTO>(sql, new { branchId });
                return result.AsList();
            }
        }

        // 3. Lấy chuỗi dữ liệu dự báo bán hàng 30 ngày tới
        public async Task<List<AI_SalesForecastDTO>> GetSalesForecastAsync(int productId, int branchId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // Query bốc dữ liệu dự báo để vẽ đường biểu đồ tương lai
                string sql = @"
            SELECT 
                product_id AS ProductId,
                forecast_date AS ForecastDate,
                predicted_qty AS PredictedQty,
                confidence_score AS ConfidenceScore
            FROM AI_SalesForecast WITH (NOLOCK)
            WHERE product_id = @productId 
              AND branch_id = @branchId
              AND forecast_date >= CAST(GETDATE() AS DATE) -- Chỉ lấy từ hôm nay trở đi
            ORDER BY forecast_date ASC";

                var result = await conn.QueryAsync<AI_SalesForecastDTO>(sql, new
                {
                    productId = productId,
                    branchId = branchId
                });

                return result.AsList();
            }
        }

        public async Task<List<AI_TopForecastDTO>> GetTopPredictedProductsAsync(int branchId, int topCount)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
            SELECT TOP (@TopCount)
                f.product_id AS ProductId,
                p.name AS ProductName,
                SUM(f.predicted_qty) AS TotalPredictedQty,
                AVG(f.confidence_score) AS AvgConfidence
            FROM AI_SalesForecast f WITH (NOLOCK)
            JOIN Products p ON f.product_id = p.product_id
            WHERE f.branch_id = @BranchId 
              AND f.forecast_date >= CAST(GETDATE() AS DATE)
            GROUP BY f.product_id, p.name
            ORDER BY TotalPredictedQty DESC";

                var result = await conn.QueryAsync<AI_TopForecastDTO>(sql, new
                {
                    BranchId = branchId,
                    TopCount = topCount
                });
                return result.AsList();
            }
        }
    }
}