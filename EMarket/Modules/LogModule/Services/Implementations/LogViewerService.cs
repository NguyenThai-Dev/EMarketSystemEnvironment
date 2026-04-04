using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Models;
using EMarket.Modules.LogModule.DTOs;
using EMarket.Modules.LogModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace EMarket.Modules.LogModule.Services.Implementations
{
    public class LogViewerService : ILogViewerService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IUserService _userService;
        private readonly Container _container;

        public LogViewerService(EMarket_DBEntities db, IUserService userService, Container container)
        {
            _db = db;
            _userService = userService;
            _container = container;
        }

        // 1. XỬ LÝ AUDIT LOGS
        public async Task<(List<AuditLogDTO> Data, int Total)> GetAuditLogsForTableAsync(AuditLogFilter filter)
        {
            IQueryable<AuditLog> query = _db.AuditLogs.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.TableName))
                query = query.Where(x => x.table_name.Contains(filter.TableName));

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(x => x.action_type == filter.Action);

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.primary_key_id.Contains(filter.Search) || x.ip_address.Contains(filter.Search));

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.created_at >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Date.AddDays(1);
                query = query.Where(x => x.created_at < toDate);
            }

            int filteredTotal = await query.CountAsync();

            // Thực hiện phân trang
            var rawData = await query
                .OrderByDescending(x => x.created_at)
                .Skip(filter.Start)
                .Take(filter.Length)
                .ToListAsync();

            var userDict = await _userService.GetUserDictAsync();

            var mappedData = rawData.Select(x => new AuditLogDTO
            {
                AuditId = x.audit_id,
                UserId = x.user_id,
                Username = (x.user_id.HasValue && userDict.ContainsKey(x.user_id.Value))
                           ? userDict[x.user_id.Value].FullName
                           : "Hệ thống/Ẩn danh",
                TableName = x.table_name,
                ActionType = x.action_type,
                PrimaryKeyId = x.primary_key_id,
                IpAddress = x.ip_address,
                CreatedAt = x.created_at ?? DateTime.Now
            }).ToList();

            return (mappedData, filteredTotal);
        }

        public async Task<(List<AppLogDTO> Data, int Total)> GetAppLogsForTableAsync(AppLogFilter filter)
        {
            IQueryable<AppLog> query = _db.AppLogs.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.LogLevel))
                query = query.Where(x => x.log_level == filter.LogLevel);

            if (!string.IsNullOrEmpty(filter.Logger))
                query = query.Where(x => x.logger.Contains(filter.Logger));

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.message.Contains(filter.Search));

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.created_at >= filter.FromDate.Value);

            int filteredTotal = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.created_at)
                .Skip(filter.Start)
                .Take(filter.Length)
                .Select(x => new AppLogDTO
                {
                    LogId = x.log_id,
                    LogLevel = x.log_level,
                    Logger = x.logger,
                    Message = x.message,
                    Thread = x.thread,
                    CreatedAt = x.created_at ?? DateTime.Now
                }).ToListAsync();

            return (data, filteredTotal);
        }

        public async Task<AuditLogDTO> GetAuditLogDetailAsync(long id)
        {
            var log = await _db.AuditLogs.FirstOrDefaultAsync(x => x.audit_id == id);
            if (log == null) return null;

            return new AuditLogDTO
            {
                AuditId = log.audit_id,
                OldValues = log.old_values,
                NewValues = log.new_values,
                TableName = log.table_name,
                ActionType = log.action_type
            };
        }

        public async Task<AppLogDTO> GetAppLogDetailAsync(long id)
        {
            var log = await _db.AppLogs.FirstOrDefaultAsync(x => x.log_id == id);
            if (log == null) return null;

            return new AppLogDTO
            {
                LogId = log.log_id,
                Message = log.message,
                Exception = log.exception,
                Logger = log.logger
            };
        }

        public async Task<LogStatsDTO> GetLogStatisticsAsync()
        {
            var today = DateTime.Today;

            // Tạo 3 Task chạy song song hoàn toàn
            var task1 = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();
                    return await db.AuditLogs.AsNoTracking().CountAsync(x => x.created_at >= today);
                }
            });

            var task2 = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();
                    return await db.AppLogs.AsNoTracking()
                        .CountAsync(x => x.created_at >= today && (x.log_level == "ERROR" || x.log_level == "FATAL"));
                }
            });

            var task3 = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();
                    return await db.AuditLogs.AsNoTracking().CountAsync();
                }
            });

            await Task.WhenAll(task1, task2, task3);

            return new LogStatsDTO
            {
                TodayAudit = task1.Result,
                TodayErrors = task2.Result,
                TotalAudit = task3.Result
            };
        }

        public async Task<List<SystemEventDTO>> GetLatestSystemEventsAsync()
        {
            var importantTables = new[] { "Suppliers", "SystemConfigs", "Users", "Roles", "Permissions", "Branches", "Expenses" };

            var auditLogs = await _db.AuditLogs.AsNoTracking()
                .Where(x => importantTables.Contains(x.table_name) || x.action_type == "Deleted")
                .OrderByDescending(x => x.created_at)
                .Take(10)
                .ToListAsync();

            var appLogs = await _db.AppLogs.AsNoTracking()
                .Where(x => x.log_level == "ERROR" || x.log_level == "FATAL")
                .OrderByDescending(x => x.created_at)
                .Take(5)
                .ToListAsync();

            var userIds = auditLogs.Where(x => x.user_id.HasValue).Select(x => x.user_id.Value).Distinct().ToList();
            var userDic = await _userService.GetUserDictAsync(userIds);

            var result = new List<SystemEventDTO>();

            foreach (var log in auditLogs)
            {
                var user = log.user_id.HasValue && userDic.ContainsKey(log.user_id.Value) ? userDic[log.user_id.Value] : null;
                result.Add(new SystemEventDTO
                {
                    Id = log.audit_id,
                    Title = user?.Username ?? "Hệ thống",
                    Summary = $"{TranslateAction(log.action_type)} {TranslateTable(log.table_name)} (#{log.primary_key_id})",
                    RawDate = log.created_at ?? DateTime.Now,
                    UserImgUrl = user?.Image ?? "/assets/img/EMarket_Logo.png",
                    IsError = false,
                    LogLevel = log.action_type
                });
            }

            foreach (var log in appLogs)
            {
                result.Add(new SystemEventDTO
                {
                    Id = log.log_id,
                    Title = $"HỆ THỐNG ({log.log_level})",
                    Summary = $"Sự cố tại {GetShortLogger(log.logger)}",
                    RawDate = log.created_at ?? DateTime.Now,
                    UserImgUrl = "/assets/img/icons/error-icon.png",
                    IsError = true,
                    LogLevel = log.log_level
                });
            }

            return result.OrderByDescending(x => x.RawDate).Take(10).ToList();
        }

        private string TranslateTable(string table)
        {
            var map = new Dictionary<string, string> {
        { "ProductImage", "Ảnh sản phẩm" },
        { "SystemConfigs", "Cấu hình" },
        { "Suppliers", "Nhà cung cấp" },
        { "Users", "Nhân viên" }
    };
            return map.ContainsKey(table) ? map[table] : table;
        }

        private string TranslateAction(string action)
        {
            var map = new Dictionary<string, string> {
        { "Added", "đã thêm" }, { "Modified", "đã sửa" }, { "Deleted", "đã xóa" }
    };
            return map.ContainsKey(action) ? map[action] : action;
        }

        private string GetShortLogger(string logger)
        {
            if (string.IsNullOrEmpty(logger)) return "Module ẩn danh";
            var parts = logger.Split('.');
            return parts.Last();
        }
    }
}