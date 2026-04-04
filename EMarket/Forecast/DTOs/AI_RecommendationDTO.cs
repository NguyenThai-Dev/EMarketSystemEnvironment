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
        public string InsightIcon { get; set; } // Icon hiển thị cho đẹp (🔥, )
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
        public string ReasonIcon
        {
            get
            {
                if (Reason.Contains("Tết") || Reason.Contains("Mùa vụ")) return "🧧"; // Icon bao lì xì/lễ
                if (Reason.Contains("hết sạch")) return "🆘";
                if (Reason.Contains("tăng mạnh")) return "";
                return "🛒";
            }
        }
        public int ExpectedDemand { get; set; } // Nhu cầu dự báo
        public int SafetyStock { get; set; }    // Tồn kho an toàn
        public int SuggestedQty { get; set; }   // Số lượng cần nhập

        public string ConfidenceLevel { get; set; } // HIGH, MEDIUM, LOW

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

        public string UrgencyIcon
        {
            get
            {
                if (ConfidenceLevel == "HIGH" && SuggestedQty > 100) return "🔥";
                if (ConfidenceLevel == "HIGH") return "⚡";
                return "";
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
    }
}
