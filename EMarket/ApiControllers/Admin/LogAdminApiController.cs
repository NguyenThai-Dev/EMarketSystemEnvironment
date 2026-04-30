using System;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Events.Class;
using EMarket.Modules.LogModule.DTOs;
using EMarket.Modules.LogModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// Read-only API for Audit Logs, Application Logs, and System Events.
    /// </summary>
    [RoutePrefix("api/admin/logs")]
    public class LogAdminApiController : ApiController
    {
        private readonly ILogViewerService _logService;

        public LogAdminApiController(ILogViewerService logService)
        {
            _logService = logService;
        }

        #region Audit Logs

        /// <summary>
        /// Truy vấn Audit Log (Ai làm gì, khi nào) với phân trang và bộ lọc.
        /// </summary>
        [HttpGet, Route("audit")]
        public async Task<IHttpActionResult> GetAuditLogs(
            int start = 0, int length = 10, string tableName = null,
            string action = null, string search = null,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var filter = new AuditLogFilter
            {
                Start = start, Length = length,
                TableName = tableName, Action = action,
                Search = search, FromDate = fromDate, ToDate = toDate
            };
            var result = await _logService.GetAuditLogsForTableAsync(filter);
            return Ok(new { data = result.Data, total = result.Total });
        }

        /// <summary>
        /// Lấy chi tiết một bản ghi Audit Log theo ID.
        /// </summary>
        [HttpGet, Route("audit/{id:long}")]
        public async Task<IHttpActionResult> GetAuditLogDetail(long id)
        {
            var d = await _logService.GetAuditLogDetailAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        #endregion

        #region Application Logs

        /// <summary>
        /// Truy vấn Application Log (Error, Warning, Info) với phân trang và bộ lọc.
        /// </summary>
        [HttpGet, Route("app")]
        public async Task<IHttpActionResult> GetAppLogs(
            int start = 0, int length = 10, string logLevel = null,
            string logger = null, string search = null,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var filter = new AppLogFilter
            {
                Start = start, Length = length,
                LogLevel = logLevel, Logger = logger,
                Search = search, FromDate = fromDate, ToDate = toDate
            };
            var result = await _logService.GetAppLogsForTableAsync(filter);
            return Ok(new { data = result.Data, total = result.Total });
        }

        /// <summary>
        /// Lấy chi tiết một bản ghi Application Log theo ID.
        /// </summary>
        [HttpGet, Route("app/{id:long}")]
        public async Task<IHttpActionResult> GetAppLogDetail(long id)
        {
            var d = await _logService.GetAppLogDetailAsync(id);
            return d == null ? (IHttpActionResult)NotFound() : Ok(d);
        }

        #endregion

        #region Statistics & Events

        /// <summary>
        /// Lấy thống kê tổng hợp log (Tổng số, Error count, Warning count...).
        /// </summary>
        [HttpGet, Route("statistics")]
        public async Task<IHttpActionResult> GetLogStatistics()
        { return Ok(await _logService.GetLogStatisticsAsync()); }

        /// <summary>
        /// Lấy danh sách sự kiện hệ thống mới nhất (System Events).
        /// </summary>
        [HttpGet, Route("system-events")]
        public async Task<IHttpActionResult> GetLatestSystemEvents()
        { return Ok(await _logService.GetLatestSystemEventsAsync()); }

        #endregion
    }
}
