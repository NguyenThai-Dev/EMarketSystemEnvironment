using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;

namespace EMarket.Events.Implementations
{
    public class LogHandler :
        IEventHandler<AuditLogEvent>,
        IEventHandler<AppLogEvent>
    {
        private readonly ILoggingService _loggingService;

        public LogHandler(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        // Xử lý Audit Log trên luồng riêng
        public async Task HandleAsync(AuditLogEvent domainEvent)
        {
            await _loggingService.SaveAuditLogAsync(domainEvent);
        }

        // Xử lý App Log trên luồng riêng
        public async Task HandleAsync(AppLogEvent domainEvent)
        {
            await _loggingService.SaveAppLogAsync(domainEvent);
        }
    }
}