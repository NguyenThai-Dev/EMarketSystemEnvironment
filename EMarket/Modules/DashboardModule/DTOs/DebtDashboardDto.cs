using System;
using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class DebtDashboardDto
    {
        public DebtKPIDto Kpi { get; set; }
        public List<ChartDataDto> AgingChart { get; set; }
        public List<DebtRecordDto> UrgentDebts { get; set; }
    }

    public class DebtKPIDto
    {
        public decimal TotalOutstanding { get; set; } // Tổng nợ phải trả
        public decimal TotalOverdue { get; set; }     // Nợ quá hạn
        public decimal TotalUpcoming { get; set; }    // Sắp đến hạn
        public decimal TotalPaidInPeriod { get; set; } // Đã trả trong kỳ
    }

    public class ChartDataDto
    {
        public string Label { get; set; }
        public int Count { get; set; }
        public decimal Value { get; set; }
    }

    public class DebtRecordDto
    {
        public string SupplierName { get; set; }
        public int PoId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public DateTime DueDate { get; set; }
        public int OverdueDays { get; set; } // >0 là quá hạn, <0 là còn hạn
    }
}