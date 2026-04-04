using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Modules.LogModule.DTOs;

namespace EMarket.Modules.LogModule.Services.Interfaces
{
    public interface ILogViewerService
    {
        Task<(List<AuditLogDTO> Data, int Total)> GetAuditLogsForTableAsync(AuditLogFilter filter);
        Task<AuditLogDTO> GetAuditLogDetailAsync(long id);

        Task<(List<AppLogDTO> Data, int Total)> GetAppLogsForTableAsync(AppLogFilter filter);
        Task<AppLogDTO> GetAppLogDetailAsync(long id);
        Task<LogStatsDTO> GetLogStatisticsAsync();

        Task<List<SystemEventDTO>> GetLatestSystemEventsAsync();
    }
}
