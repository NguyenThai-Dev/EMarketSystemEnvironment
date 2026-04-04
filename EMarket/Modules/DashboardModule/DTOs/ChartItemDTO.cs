namespace EMarket.Modules.DashboardModule.DTOs
{
    public class ChartItemDTO
    {
        public string Label { get; set; }
        public int Value { get; set; }        // Số lượng
        public decimal ExtraValue { get; set; }   // Tổng tiền (Giá trị tồn kho)
        public string Status { get; set; }        // Ví dụ: "Normal", "Critical"
    }
}