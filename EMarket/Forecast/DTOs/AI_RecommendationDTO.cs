using System;

namespace EMarket.Forecast.DTOs
{
    public class AI_RecommendationDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // Join từ bảng Products
        public string CategoryName { get; set; } // Join từ bảng Categories
        public string Unit { get; set; }
        public int ForecastDemand { get; set; }
        public int CurrentStock { get; set; }
        public int RecommendedMin { get; set; }
        public int RecommendedMax { get; set; }
        public string Reason { get; set; }
        public string ConfidenceLevel { get; set; }
    }

    // 2. DTO cho Bất thường ngành hàng
    public class AI_AnomalyDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ActualQty { get; set; }
        public int ForecastQty { get; set; }
        public decimal DeviationPercent { get; set; }
        public string AnomalyType { get; set; } // Spike / Dip
        public string Severity { get; set; } // High / Medium / Low
    }

    // 3. DTO cho Insight chi tiết sản phẩm
    public class AI_InsightDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal GrowthPercent { get; set; }
        public decimal ContributionPercent { get; set; }
        public string InsightLevel { get; set; } // Star, Trending, Warning
    }

    public class AIReplenishmentDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string Unit { get; set; }
        public string ProductImage { get; set; } // Hiển thị ảnh cho trực quan

        // Các trường số liệu từ AI
        public int CurrentStock { get; set; }
        public string Reason { get; set; }
        public int ExpectedDemand { get; set; } // Nhu cầu dự báo
        public int SafetyStock { get; set; }    // Tồn kho an toàn
        public int SuggestedQty { get; set; }   // Số lượng cần nhập

        public string ConfidenceLevel { get; set; } 

        // Trường tính toán để hiển thị Icon/Màu sắc trên UI
        public string UrgencyColor
        {
            get
            {
                if (ConfidenceLevel == "HIGH" && SuggestedQty > 100) return "text-danger"; // Đỏ (Gấp)
                if (ConfidenceLevel == "HIGH") return "text-warning"; // Vàng
                return "text-success"; // Xanh
            }
        }

        public class ProductHistoryDTO
        {
            public DateTime Date { get; set; }
            public int Qty { get; set; }
        }

        public class AI_InventoryForecastDTO
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int CurrentStock { get; set; }
            public int ExpectedDemand30d { get; set; }
            public int SuggestedQty { get; set; }
            public decimal ConfidenceScore { get; set; }
            public string PriorityLevel { get; set; }
        }

        public class AI_DeadstockDTO
        {
            public string ProductName { get; set; }
            public int CurrentStock { get; set; }
            public int DaysToExhaust { get; set; }
            public string WarningType { get; set; }
            public string RiskReason { get; set; }
            public decimal ConfidenceScore { get; set; }
            public string Recommendation { get; set; }
        }

        public class AI_SalesForecastDTO
        {
            public int ProductId { get; set; }
            public DateTime ForecastDate { get; set; } // Ngày dự báo
            public decimal PredictedQty { get; set; }   // Sản lượng dự báo
            public decimal ConfidenceScore { get; set; }
        }

        public class AI_TopForecastDTO
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal TotalPredictedQty { get; set; }
            public decimal AvgConfidence { get; set; }
        }

        // DTO cho Phân tích Rủi ro Tài chính theo Lô hàng (FEFO)
        public class AI_LotFinancialRiskDTO
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string LotId { get; set; }
            public int Quantity { get; set; }
            public int DaysToExpiry { get; set; }
            public decimal CostPrice { get; set; }
            public int RiskQty { get; set; }
            public decimal ProvisionValue { get; set; }
            public string LotStatus { get; set; }
            public string Recommendation { get; set; }
            public decimal ConfidenceScore { get; set; }

            // Computed: Màu badge cho UI
            public string StatusColor
            {
                get
                {
                    switch (LotStatus)
                    {
                        case "EXPIRED": return "badge-critical";
                        case "DANGER": return "badge-critical";
                        case "WARNING": return "badge-high";
                        default: return "badge-normal";
                    }
                }
            }
        }

        // DTO tổng hợp cho popup thống kê rủi ro tài chính
        public class AI_LotRiskSummaryDTO
        {
            public decimal TotalProvisionValue { get; set; }
            public int TotalRiskLots { get; set; }
            public int DangerCount { get; set; }
            public int WarningCount { get; set; }
            public int SafeCount { get; set; }
            public System.Collections.Generic.List<AI_LotFinancialRiskDTO> Details { get; set; }
        }
    }
}
