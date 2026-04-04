using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;
using EMarket.Hubs;
using EMarket.Models;
using EMarket.Modules.UserModule.Services.Interfaces;
using Microsoft.AspNet.SignalR;

namespace EMarket.Events.Implementations
{
    public class LoggingService : ILoggingService
    {
        private readonly EMarket_DBEntities _db;
        private readonly IUserContext _userContext;
        private readonly IHubContext _hub;
        private readonly string _connectionString;

        public LoggingService(EMarket_DBEntities db, IUserContext userContext)
        {
            _db = db;
            _userContext = userContext;
            _hub = GlobalHost.ConnectionManager.GetHubContext<SystemLogHub>();
            _connectionString = ConfigurationManager.ConnectionStrings["EMarket_Connections"].ConnectionString;
        }

        private string GetCurrentIpAddress()
        {
            try
            {
                var context = System.Web.HttpContext.Current;
                if (context != null)
                {
                    string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = context.Request.UserHostAddress;
                    }
                    return ip;
                }
            }
            catch { }
            return "127.0.0.1";
        }

        public async Task SaveAuditLogAsync(AuditLogEvent ev)
        {
            const string sql = @"
            INSERT INTO AuditLogs (user_id, ip_address, table_name, primary_key_id, action_type, old_values, new_values, created_at)
            VALUES (@UserId, @IpAddress, @TableName, @PrimaryKeyId, @ActionType, @OldValues, @NewValues, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as bigint);";

            using (var conn = new SqlConnection(_connectionString))
            {
                var auditId = await conn.QuerySingleAsync<long>(sql, new
                {
                    UserId = ev.UserId ?? (int?)null,
                    IpAddress = ev.IpAddress,
                    TableName = ev.TableName,
                    PrimaryKeyId = ev.PrimaryKeyId,
                    ActionType = ev.ActionType,
                    OldValues = ev.OldValues,
                    NewValues = ev.NewValues,
                    CreatedAt = DateTime.Now
                });

                // Gửi thông báo Real-time (SignalR/Socket) nếu cần
                await NotifyNewLogAsync(auditId, "Audit");
            }
        }

        public async Task SaveAppLogAsync(AppLogEvent ev)
        {
            const string sql = @"
            INSERT INTO AppLogs (log_level, logger, message, exception, thread, created_at)
            VALUES (@LogLevel, @Logger, @Message, @Exception, @Thread, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as bigint);";

            using (var conn = new SqlConnection(_connectionString))
            {
                var logId = await conn.QuerySingleAsync<long>(sql, new
                {
                    LogLevel = ev.LogLevel,
                    Logger = ev.Logger,
                    Message = ev.Message,
                    Exception = ev.Exception,
                    Thread = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(),
                    CreatedAt = DateTime.Now
                });

                await NotifyNewLogAsync(logId, "AppLog");
            }
        }

        public async Task NotifyNewLogAsync(object payload, string logType)
        {
            await Task.Run(() =>
            {
                _hub.Clients.Group("ADMIN_LOG_GROUP").onNewLogReceived(new
                {
                    type = logType,
                    data = payload,
                    serverTime = DateTime.Now.ToString("HH:mm:ss")
                });
            });
        }
    }
}