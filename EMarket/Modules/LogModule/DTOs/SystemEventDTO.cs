using System;

namespace EMarket.Modules.LogModule.DTOs
{
    public class SystemEventDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string CreatedAt { get; set; }
        public DateTime RawDate { get; set; }
        public string UserImgUrl { get; set; }
        public bool IsError { get; set; }
        public string LogLevel { get; set; }
    }
}