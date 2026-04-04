using EMarket.Events.Interfaces;

namespace EMarket.Events.Class
{
    public class AuditLogEvent : IEvent
    {
        public int? UserId { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyId { get; set; }
        public string ActionType { get; set; } // INSERT, UPDATE, DELETE
        public string OldValues { get; set; } // JSON string
        public string NewValues { get; set; } // JSON string
        public string IpAddress { get; set; }
    }
}