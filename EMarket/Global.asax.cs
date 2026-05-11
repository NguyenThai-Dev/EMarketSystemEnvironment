using EMarket.Events.Implementations;
using EMarket.Events.Interfaces;
using EMarket.Filters;
using EMarket.Forecast.Services.Implementations;
using EMarket.Forecast.Services.Interfaces;
using EMarket.Models;
using EMarket.Modules.CustomerModule.Services.Implementations;
using EMarket.Modules.CustomerModule.Services.Interfaces;
using EMarket.Modules.DashboardModule.Servcie.Interfaces;
using EMarket.Modules.DashboardModule.Services.Implementations;
using EMarket.Modules.ExpenseModule.Services.Implementations;
using EMarket.Modules.ExpenseModule.Services.Interfaces;
using EMarket.Modules.InventoryModule.Services.Implementations;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.LogModule.Services.Implementations;
using EMarket.Modules.LogModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Implementations;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.QuotationModule.Services.Implementations;
using EMarket.Modules.QuotationModule.Services.Interfaces;
using EMarket.Modules.SalesModule.Services.Implementations;
using EMarket.Modules.SalesModule.Services.Interfaces;
using EMarket.Modules.SystemConfigModule.Services.Implementations;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Implementations;
using EMarket.Modules.UserModule.Services.Interfaces;
using Hangfire;
using Hangfire.SimpleInjector;
using Microsoft.Extensions.Caching.Memory;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TvdP.SimpleInjector;
using MemoryCache = Microsoft.Extensions.Caching.Memory.MemoryCache;

namespace EMarket
{
    /// <summary>
    /// Represents the ASP.NET MVC application class for EMarket.
    /// Handles application startup, dependency injection, Hangfire configuration, and shutdown logic.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        private BackgroundJobServer _backgroundJobServer;
        /// <summary>
        /// Handles application startup logic, including dependency injection, Hangfire configuration, and MVC setup.
        /// </summary>
        protected void Application_Start()
        {
            System.Net.ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.Name;

            var container = new Container();
            GlobalContainer.Container = container;
            container.Options.DefaultScopedLifestyle = Lifestyle.CreateHybrid(
               defaultLifestyle: new WebRequestLifestyle(),
               fallbackLifestyle: new AsyncScopedLifestyle()
           );

            // [THÊM DÒNG NÀY] Đăng ký chính xác cái class Handler để nó có thể được khởi tạo riêng lẻ
            container.Register<SupplierPaymentNotificationHandler>(Lifestyle.Scoped);
            container.Register<LogHandler>(Lifestyle.Scoped);
            // =============================================================
            // 1. REGISTER DBCONTEXT (Thay cho HierarchicalLifetimeManager)
            // =============================================================
            container.Register<EMarket_DBEntities>(() =>
            {
                // Lấy các service cần thiết
                var dispatcher = container.GetInstance<IEventDispatcher>();
                var userContext = container.GetInstance<IUserContext>();

                // Khởi tạo đúng constructor 2 tham số của ông
                return new EMarket_DBEntities(dispatcher, userContext);
            }, Lifestyle.Scoped);

            container.Register<ErrorLoggingInterceptor>(Lifestyle.Scoped);
            container.InterceptWith<ErrorLoggingInterceptor>(type => type.Name.EndsWith("Service"));

            container.InterceptWith<ErrorLoggingInterceptor>(
    serviceType => serviceType.Name.EndsWith("Service")
);
            // =============================================================
            // 2. USER SERVICES
            // =============================================================
            container.Register<ILoginService, LoginService>(Lifestyle.Scoped);
            container.Register<IBranchService, BranchService>(Lifestyle.Scoped);
            container.Register<IRoleService, RoleService>(Lifestyle.Scoped);
            container.Register<IPermissionService, PermissionService>(Lifestyle.Scoped);
            container.Register<IUserService, UserService>(Lifestyle.Scoped);
            container.Register<IUserContext, UserContext>(Lifestyle.Scoped);
            // FIX: Sử dụng kiểm tra null để không bị oẳng lúc Verify container
            container.Register<HttpContextBase>(() =>
            {
                var context = HttpContext.Current;
                if (context == null)
                {
                    // Trả về một dummy hoặc null tùy logic, 
                    // nhưng quan trọng là không được để HttpContextWrapper nhận null
                    return new HttpContextWrapper(new HttpContext(new HttpRequest("", "http://temp.uri", ""), new HttpResponse(null)));
                }
                return new HttpContextWrapper(context);
            }, Lifestyle.Scoped);
            container.Register<RequireLoginFilter>(Lifestyle.Scoped);

            // =============================================================
            // 3. CUSTOMER SERVICES
            // =============================================================
            container.Register<ICustomerService, CustomerService>(Lifestyle.Scoped);
            container.Register<ICustomerAddressService, CustomerAddressService>(Lifestyle.Scoped);
            container.Register<ILoyaltyProgramService, LoyaltyProgramService>(Lifestyle.Scoped);

            // =============================================================
            // 4. INVENTORY SERVICES
            // =============================================================
            container.Register<IInventoryService, InventoryService>(Lifestyle.Scoped);
            container.Register<IWarehouseService, WarehouseService>(Lifestyle.Scoped);
            container.Register<IPurchaseService, PurchaseService>(Lifestyle.Scoped);
            container.Register<IStockMovementService, StockMovementService>(Lifestyle.Scoped);
            container.Register<ISupplierServiceDebtAndPaymentService, SupplierDebtAndPaymentService>(Lifestyle.Scoped);

            // =============================================================
            // 5. PRODUCT SERVICES
            // =============================================================
            container.Register<IProductService, ProductService>(Lifestyle.Scoped);
            container.Register<IProductCategoryService, ProductCategoryService>(Lifestyle.Scoped);
            container.Register<ISupplierService, SupplierService>(Lifestyle.Scoped);
            container.Register<IProductLotService, ProductLotService>(Lifestyle.Scoped);

            // =============================================================
            // 6. SALES & DASHBOARD SERVICES
            // =============================================================
            container.Register<IOrderService, OrderService>(Lifestyle.Scoped);
            container.Register<IPaymentService, PaymentService>(Lifestyle.Scoped);
            container.Register<IPromotionService, PromotionService>(Lifestyle.Scoped);
            container.Register<IDashboardService, DashboardService>(Lifestyle.Scoped);
            container.RegisterInstance<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));

            // =============================================================
            // 7. EVENT SYSTEM & EMAIL
            // =============================================================
            container.Register<IEmailService, SmtpEmailService>(Lifestyle.Scoped);

            container.Register<IAIService, AIService>(Lifestyle.Scoped);

            container.Register<IInventoryAlertService, InventoryAlertService>(Lifestyle.Scoped);

            container.Register<ISystemConfigService, SystemConfigService>(Lifestyle.Scoped);

            container.Register<ITelegramService, TelegramService>(Lifestyle.Scoped);

            container.Register<ILoggingService, LoggingService>(Lifestyle.Scoped);

            container.Register<IEventDispatcher>(() => new InMemoryEventDispatcher(container), Lifestyle.Singleton);

            container.Register<IOrderRealtimeService, OrderRealtimeService>(Lifestyle.Scoped);

            // Tự động tìm các Handler trong cùng Assembly với SmtpEmailService
            container.Collection.Register(typeof(IEventHandler<>), typeof(SmtpEmailService).Assembly);
            container.Register<SupplierDebtNotificationJob>(Lifestyle.Scoped);

            container.Register<IQuotationService, QuotationService>(Lifestyle.Scoped);
            container.Register<IExpenseService, ExpenseService>(Lifestyle.Scoped);
            container.Register<ILogViewerService, LogViewerService>(Lifestyle.Scoped);
            // =============================================================
            // 8. KẾT NỐI VỚI MVC
            // =============================================================
            container.RegisterWebApiControllers(System.Web.Http.GlobalConfiguration.Configuration);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            // Kiểm tra cấu hình (Cực kỳ quan trọng, sẽ báo lỗi nếu thiếu dependency)
            container.Verify();

            // Thiết lập Resolver cho ASP.NET MVC
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver =
    new SimpleInjectorWebApiDependencyResolver(container);

            // 3. CẤU HÌNH HANGFIRE (Đặt ngay sau khi DI xong)

            // Kết nối DB
            GlobalConfiguration.Configuration.UseSqlServerStorage("EMarket_Connections");

            // Kết nối Simple Injector
            GlobalConfiguration.Configuration.UseActivator(new SimpleInjectorJobActivator(container));

            // 4. KHỞI ĐỘNG HANGFIRE SERVER (Dùng cách cũ này ổn định hơn cho MVC 5)
            _backgroundJobServer = new BackgroundJobServer();

            // 5. ĐĂNG KÝ RECURRING JOBS
            // Xóa các job cũ (nếu có) để tránh trùng lặp khi đổi tên method
            // RecurringJob.RemoveIfExists("supplier-debt-near-due");

            // 8h00: Thông báo nợ sắp đến hạn
            RecurringJob.AddOrUpdate<SupplierDebtNotificationJob>(
                "supplier-debt-near-due",
                job => job.NotifyNearDueDebts(),
                Cron.Daily(8, 0)
            );

            // 8h15: Kiểm tra tồn kho (Lệch 15p cho nhẹ máy)
            RecurringJob.AddOrUpdate<IInventoryAlertService>(
                "inventory-daily-check",
                job => job.CheckAndSendAlertsAsync(),
                Cron.Daily(8, 15)
            );

            // 9h00: Thông báo nợ quá hạn
            RecurringJob.AddOrUpdate<SupplierDebtNotificationJob>(
                "supplier-debt-overdue",
                job => job.NotifyOverdueDebts(),
                Cron.Daily(9, 0)
            );

            // 6. Config MVC mặc định
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, container);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Handles application shutdown logic, including disposing of the Hangfire background job server.
        /// </summary>
        protected void Application_End()
        {
            _backgroundJobServer?.Dispose();
        }
    }
}
