using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIAssistantService.Plugins
{
    public class TimePlugin
    {
        [KernelFunction]
        [Description("Lấy ngày và giờ hệ thống hiện tại. Cực kỳ quan trọng để tính toán khoảng thời gian cho các báo cáo.")]
        public string GetCurrentTime()
        {
            // Trả về định dạng chuẩn ISO để AI dễ cộng trừ ngày tháng
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss dddd");
        }

        [KernelFunction]
        [Description("Tính toán ngày dựa trên độ lệch (offset). Ví dụ: offset = -1 là hôm qua, offset = -7 là tuần trước.")]
        public string GetOffsetDate(int offsetDays)
        {
            return DateTime.Now.AddDays(offsetDays).ToString("yyyy-MM-dd");
        }
    }
}