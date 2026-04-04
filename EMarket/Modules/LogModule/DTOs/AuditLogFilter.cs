namespace EMarket.Modules.LogModule.DTOs
{
    public class AuditLogFilter
    {
        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;
        public string TableName { get; set; }
        public string Action { get; set; }
        public string Search { get; set; }
        public System.DateTime? FromDate { get; set; }
        public System.DateTime? ToDate { get; set; }
    }
}