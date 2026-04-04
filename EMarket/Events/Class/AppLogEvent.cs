using EMarket.Events.Interfaces;

namespace EMarket.Events.Class
{
    public class AppLogEvent : IEvent
    {
        public string LogLevel { get; set; } // INFO, ERROR, FATAL
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Thread { get; set; }
    }
}