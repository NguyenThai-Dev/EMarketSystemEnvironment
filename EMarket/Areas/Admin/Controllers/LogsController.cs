using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.LogModule.DTOs;
using EMarket.Modules.LogModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    public class LogsController : Controller
    {
        private readonly ILogViewerService _logViewerService;

        public LogsController(ILogViewerService logViewerService)
        {
            _logViewerService = logViewerService;
        }

        [EMarketAuthorize(RequireAdmin = true)]
        public ActionResult SystemLog() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<ActionResult> GetAuditLogsTable()
        {
            try
            {
                // DataTable gửi tham số qua Form POST
                var draw = Request.Form["draw"];
                var start = Convert.ToInt32(Request.Form["start"] ?? "0");
                var length = Convert.ToInt32(Request.Form["length"] ?? "10");
                var searchValue = Request.Form["search[value]"];

                var filter = new AuditLogFilter
                {
                    Start = start,
                    Length = length,
                    Search = searchValue,
                    TableName = Request.Form["tableName"],
                    Action = Request.Form["actionType"]
                };

                var (data, total) = await _logViewerService.GetAuditLogsForTableAsync(filter);

                return Json(new
                {
                    draw = draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new { draw = 0, recordsTotal = 0, recordsFiltered = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<ActionResult> GetAppLogsTable()
        {
            try
            {
                var draw = Request.Form["draw"];
                var start = Convert.ToInt32(Request.Form["start"] ?? "0");
                var length = Convert.ToInt32(Request.Form["length"] ?? "10");
                var searchValue = Request.Form["search[value]"];

                var filter = new AppLogFilter
                {
                    Start = start,
                    Length = length,
                    Search = searchValue,
                    LogLevel = Request.Form["logLevel"]
                };

                var (data, total) = await _logViewerService.GetAppLogsForTableAsync(filter);

                return Json(new
                {
                    draw = draw,
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = data
                });
            }
            catch
            {
                return Json(new { draw = 0, recordsTotal = 0, recordsFiltered = 0, data = new List<object>() });
            }
        }

        [HttpGet]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<ActionResult> GetAuditDetail(long id)
        {
            var detail = await _logViewerService.GetAuditLogDetailAsync(id);
            if (detail == null) return HttpNotFound();

            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<ActionResult> GetAppLogDetail(long id)
        {
            var detail = await _logViewerService.GetAppLogDetailAsync(id);
            if (detail == null) return HttpNotFound();

            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [EMarketAuthorize(RequireAdmin = true)]
        public async Task<JsonResult> GetStats()
        {
            try
            {
                var stats = await _logViewerService.GetLogStatisticsAsync();

                return Json(new
                {
                    todayAudit = stats.TodayAudit,
                    todayErrors = stats.TodayErrors,
                    totalAudit = stats.TotalAudit
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    todayAudit = 0,
                    todayErrors = 0,
                    totalAudit = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetLatestSystemEvents()
        {
            try
            {
                var logs = await _logViewerService.GetLatestSystemEventsAsync();
                return Json(logs, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new List<AuditLogDTO>(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}