using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;
using Newtonsoft.Json;
using SimpleInjector.Lifestyles;

namespace EMarket.Models
{
    public partial class EMarket_DBEntities : DbContext
    {
        private readonly IEventDispatcher _dispatcher;
        private readonly IUserContext _userContext;

        public EMarket_DBEntities(IEventDispatcher dispatcher, IUserContext userContext) : base("name=EMarket_DBEntities")
        {
            _dispatcher = dispatcher;
            _userContext = userContext;
        }

        public override async Task<int> SaveChangesAsync()
        {
            // 1. Dò tìm thay đổi và Snapshot data thô ngay tại đây
            // Phải làm TRƯỚC khi base.Save vì sau khi Save, State sẽ bị reset về Unchanged
            var auditEntries = CreateAuditEntries();

            int? currentUserId = _userContext?.IsAuthenticated == true ? _userContext.UserId : (int?)null;
            string currentIp = System.Web.HttpContext.Current?.Request.UserHostAddress ?? "127.0.0.1";

            // 2. THỰC THI LƯU DB (Thread chính)
            var result = await base.SaveChangesAsync();

            // 3. Xử lý ID cho các record mới (Added) 
            // Sau khi Save thành công, các cột Identity mới có giá trị
            var eventsToLog = new List<AuditLogEvent>();
            foreach (var entryInternal in auditEntries)
            {
                // Nếu là hàng mới tạo, bây giờ mới lấy được Primary Key
                if (entryInternal.StateBeforeSave == EntityState.Added.ToString())
                {
                    // primaryKeyId lúc này đã được EF điền vào Entity object
                    // Ta cần map lại một lần nữa trước khi gửi đi
                    // (Bạn có thể bổ sung logic lấy PK ở đây tương tự GetPrimaryKey)
                }

                eventsToLog.Add(new AuditLogEvent
                {
                    UserId = currentUserId,
                    TableName = entryInternal.TableName,
                    ActionType = entryInternal.StateBeforeSave,
                    PrimaryKeyId = entryInternal.PrimaryKeyId, // Cần xử lý PK cho trường hợp Added
                    OldValues = JsonConvert.SerializeObject(entryInternal.OldValues),
                    NewValues = JsonConvert.SerializeObject(entryInternal.NewValues),
                    IpAddress = currentIp,
                });
            }

            // 4. BẮN LOG FIRE & FORGET (Dữ liệu lúc này hoàn toàn là POCO, không dính tới DbContext)
            if (eventsToLog.Any())
            {
                _ = Task.Run(async () =>
                {
                    using (AsyncScopedLifestyle.BeginScope(GlobalContainer.Container))
                    {
                        try
                        {
                            var loggingService = GlobalContainer.Container.GetInstance<ILoggingService>();
                            foreach (var ev in eventsToLog)
                            {
                                await loggingService.SaveAuditLogAsync(ev);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[Audit Log Background Error]: {ex.Message}");
                        }
                    }
                });
            }

            return result;
        }

        private List<AuditLogEntryInternal> CreateAuditEntries()
        {
            this.ChangeTracker.DetectChanges();
            var list = new List<AuditLogEntryInternal>();

            foreach (var entry in this.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                // Snapshot data ngay lập tức
                list.Add(new AuditLogEntryInternal(entry, this));
            }
            return list;
        }
    }

    internal class AuditLogEntryInternal
    {
        public string TableName { get; set; }
        public string StateBeforeSave { get; set; }
        public string PrimaryKeyId { get; set; }
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

        public AuditLogEntryInternal(DbEntityEntry entry, DbContext context)
        {
            TableName = System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(entry.Entity.GetType()).Name;
            StateBeforeSave = entry.State.ToString();

            // 1. Snapshot values ngay lập tức
            CaptureValues(entry);

            // 2. Lấy Key sớm (nếu là Modified/Deleted)
            if (entry.State != EntityState.Added)
            {
                PrimaryKeyId = GetPrimaryKey(entry, context);
            }
        }

        private void CaptureValues(DbEntityEntry entry)
        {
            // Duyệt qua tất cả các property hiện có trong Metadata của Entity
            var propertyNames = entry.State == EntityState.Deleted
                ? entry.OriginalValues.PropertyNames
                : entry.CurrentValues.PropertyNames;

            foreach (var propName in propertyNames)
            {
                var prop = entry.Property(propName);
                if (entry.State == EntityState.Added)
                {
                    NewValues[propName] = prop.CurrentValue;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    OldValues[propName] = prop.OriginalValue;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Chỉ log những cột thực sự thay đổi hoặc quan trọng
                    if (prop.IsModified)
                    {
                        OldValues[propName] = prop.OriginalValue;
                        NewValues[propName] = prop.CurrentValue;
                    }
                }
            }
        }

        public string GetPrimaryKey(DbEntityEntry entry, DbContext context)
        {
            try
            {
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var stateEntry = objectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
                if (stateEntry.EntityKey?.EntityKeyValues != null && stateEntry.EntityKey.EntityKeyValues.Length > 0)
                {
                    return stateEntry.EntityKey.EntityKeyValues[0].Value.ToString();
                }
            }
            catch { }
            return "N/A";
        }
    }
}