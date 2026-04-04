using System.Collections.Generic;

namespace EMarket.Modules.DashboardModule.DTOs
{
    public class Charts_PeopleDTO
    {
        public List<SegmentItemDTO> CustomerSegments { get; set; }
        public GrowthChartDTO Growth { get; set; }
    }

    public class SegmentItemDTO
    {
        public string Label { get; set; }
        public int Count { get; set; }
    }

    public class GrowthChartDTO
    {
        public List<string> Labels { get; set; }
        public List<int> Customers { get; set; }
        public List<int> Employees { get; set; }
    }
}