using System;

namespace EMarket.Modules.LogModule.DTOs
{
    public class AppLogDTO
    {
        public long LogId { get; set; }
        public string LogLevel { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Thread { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}