using System;

namespace EMarket.Modules.LogModule.DTOs
{
    public class AuditLogDTO
    {
        public long AuditId { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyId { get; set; }
        public string ActionType { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserImgUrl { get; set; }
    }
}