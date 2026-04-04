using System.Threading.Tasks;
using EMarket.Events.Class;

namespace EMarket.Events.Interfaces
{
    public interface ILoggingService
    {
        Task SaveAuditLogAsync(AuditLogEvent auditEvent);

        Task SaveAppLogAsync(AppLogEvent logEvent);

        Task NotifyNewLogAsync(object payload, string logType);
    }
}
