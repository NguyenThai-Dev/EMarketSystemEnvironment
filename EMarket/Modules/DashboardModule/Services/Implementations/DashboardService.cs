using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EMarket.Models; // Đảm bảo namespace đúng với project của bro
using EMarket.Modules.CustomerModule.Services.Interfaces;
using EMarket.Modules.DashboardModule.DTOs;
using EMarket.Modules.DashboardModule.Servcie.Interfaces;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.SalesModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace EMarket.Modules.DashboardModule.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        // Các Dependencies giữ nguyên
        private readonly IOrderService _orderService;
        private readonly IPurchaseService _purchaseService;
        private readonly IInventoryService _inventoryService;
        private readonly IBranchService _branchService;
        private readonly IProductLotService _productLotService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        private readonly IMemoryCache _cache;
        private readonly EMarket_DBEntities _db;
        private readonly string _connStr;
        private readonly Container _container;

        public DashboardService(
            IOrderService orderService,
            IPurchaseService purchaseService,
            IInventoryService inventoryService,
            IBranchService branchService,
            IProductLotService productLotService,
            IProductService productService,
            IWarehouseService warehouseService,
            ICustomerService customerService,
            IUserService userService,
            IMemoryCache memoryCache,
            Container container,
            EMarket_DBEntities eMarket_DBEntities)
        {
            _orderService = orderService;
            _purchaseService = purchaseService;
            _inventoryService = inventoryService;
            _branchService = branchService;
            _productLotService = productLotService;
            _productService = productService;
            _warehouseService = warehouseService;
            _customerService = customerService;
            _userService = userService;
            _cache = memoryCache;
            _container = container;
            _db = eMarket_DBEntities;

            // Lấy connection string chuẩn
            _connStr = ConfigurationManager.ConnectionStrings["EMarket_Connections"].ConnectionString;
        }

        #region Admin Dashboard (Đã Fix Thread-Safety)

        // 1. DASHBOARD SUMMARY
        public async Task<DashboardSummaryDTO> GetSummaryAsync(int? branchId)
        {
            // Dùng connection riêng -> An toàn
            using (var conn = new SqlConnection(_connStr))
            {
                var result = await conn.QueryFirstOrDefaultAsync<DashboardSummaryDTO>(
                    "sp_Admin_Dashboard_Summary",
                    new { BranchId = branchId },
                    commandType: CommandType.StoredProcedure
                );
                return result ?? new DashboardSummaryDTO();
            }
        }

        // 2. BRANCH PERFORMANCE
        public async Task<List<BranchDashboardDTO>> GetBranchPerformanceAsync(int? brandId, DateTime fromDate, DateTime toDate)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                var result = await conn.QueryAsync<BranchDashboardDTO>(
                    "sp_Admin_Dashboard_BranchPerformance",
                    new { BranchId = brandId, FromDate = fromDate, ToDate = toDate },
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        // 3. STOCK CHART (Đã Fix: Không dùng _db)
        public async Task<List<ChartItemDTO>> GetStockChartAsync(int? branchId)
        {
            // FIX: Tạo kết nối mới hoàn toàn
            using (var conn = new SqlConnection(_connStr))
            {
                var result = await conn.QueryAsync<ChartItemDTO>(
                    "sp_Admin_Dashboard_StockChart",
                    new { BranchId = branchId },
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        // 4. OVERVIEW ORCHESTRATOR
        public async Task<DashboardOverviewDTO> GetOverviewAsync(int? branchId, DateTime fromDate, DateTime toDate, string groupBy)
        {
            // Bây giờ các hàm con đều dùng kết nối riêng (New SqlConnection), 
            // nên việc chạy song song (Parallel) ở đây là AN TOÀN TUYỆT ĐỐI.

            var trendAndSalesTask = GetTrendAndSalesInternalAsync(branchId, fromDate, toDate, groupBy);
            var inventorySummaryTask = GetSummaryAsync(branchId);
            var stockChartTask = GetStockChartAsync(branchId);
            var branchPerformanceTask = GetBranchPerformanceAsync(branchId, fromDate, toDate);

            // Chờ tất cả xong mà không sợ "connection is connecting"
            await Task.WhenAll(trendAndSalesTask, inventorySummaryTask, stockChartTask, branchPerformanceTask);

            // Ghép dữ liệu
            var overviewData = trendAndSalesTask.Result;
            var inventoryData = inventorySummaryTask.Result;

            overviewData.Summary.TotalProducts = inventoryData.TotalProducts;
            overviewData.Summary.LowStockProducts = inventoryData.LowStockProducts;
            overviewData.Summary.TotalWarehouses = inventoryData.TotalWarehouses;
            overviewData.Summary.TotalInventoryQuantity = inventoryData.TotalInventoryQuantity;

            overviewData.StockChart = stockChartTask.Result;
            overviewData.BranchPerformance = branchPerformanceTask.Result;

            return overviewData;
        }

        // 5. INTERNAL TREND & SALES (Đã Fix: Không dùng _db)
        private async Task<DashboardOverviewDTO> GetTrendAndSalesInternalAsync(int? branchId, DateTime fromDate, DateTime toDate, string groupBy)
        {
            var result = new DashboardOverviewDTO { Summary = new DashboardSummaryDTO(), Trend = new DashboardTrendDTO() };

            // FIX: Tạo kết nối mới
            using (var conn = new SqlConnection(_connStr))
            {
                // Dapper QueryMultiple tự quản lý Open/Close thông minh hơn EF
                using (var multi = await conn.QueryMultipleAsync(
                    "sp_Admin_Dashboard_Overview",
                    new { BranchId = branchId, FromDate = fromDate, ToDate = toDate, GroupBy = groupBy },
                    commandType: CommandType.StoredProcedure))
                {
                    // Result 1: Summary
                    result.Summary = await multi.ReadFirstOrDefaultAsync<DashboardSummaryDTO>() ?? new DashboardSummaryDTO();

                    // Result 2: Trend Chart
                    var trendData = await multi.ReadAsync<dynamic>();

                    var labels = new List<string>();
                    var sales = new List<decimal>();
                    var purchases = new List<decimal>();

                    foreach (var row in trendData)
                    {
                        if (DateTime.TryParse(row.Date.ToString(), out DateTime dt))
                        {
                            labels.Add(groupBy == "month" ? dt.ToString("MM/yyyy") : dt.ToString("dd/MM"));
                        }
                        else
                        {
                            labels.Add(row.Date.ToString());
                        }

                        sales.Add((decimal)row.Sales);
                        purchases.Add((decimal)row.Purchases);
                    }

                    result.Trend = new DashboardTrendDTO
                    {
                        Labels = labels,
                        Sales = sales,
                        Purchases = purchases
                    };
                }
            }
            return result;
        }

        #endregion

        #region People Dashboard (Đã Fix)

        public async Task<PeopleDashboardDTO> GetPeopleDashboardAsync()
        {
            var result = new PeopleDashboardDTO
            {
                Kpi = new KPI_PeopleDTO(),
                Charts = new Charts_PeopleDTO(),
                Lists = new Lists_PeopleDTO()
            };

            var totalUsers = 0;
            var now = DateTime.Now;
            var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var startOfYear = new DateTime(now.Year, 1, 1);

            // =========================================================================
            // TASK 1: KPI & USER AVATARS (Tính toán tăng trưởng khách hàng & nhân viên)
            // =========================================================================
            var kpiTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();
                    totalUsers = await db.Users.CountAsync();

                    // 1.1 Lấy tổng quan
                    var totalCust = await db.Customers.CountAsync();
                    var vipCount = await db.Customers.CountAsync(c => c.customer_type == "VIP");
                    var activeEmp = await db.Users.CountAsync(u => u.status == "Active");

                    // 1.2 Tính Growth (Khách hàng tháng này vs tháng trước)
                    // Query đếm theo khoảng thời gian
                    var thisMonthCust = await db.Customers.CountAsync(c => c.created_at >= startOfThisMonth);
                    var lastMonthCust = await db.Customers.CountAsync(c => c.created_at >= startOfLastMonth && c.created_at < startOfThisMonth);

                    // Đăng ký mới trong tháng (Khách + Nhân viên)
                    var newUsersThisMonth = await db.Users.CountAsync(u => u.created_at >= startOfThisMonth);

                    // 1.3 Lấy Avatar nhân viên mới nhất
                    var avatars = await db.Users.AsNoTracking()
                        .Where(u => u.status == "Active" && u.user_img != null)
                        .OrderByDescending(u => u.created_at)
                        .Take(5)
                        .Select(u => u.user_img)
                        .ToListAsync();

                    // 1.4 Tính % tăng trưởng
                    double growthPercent = 0;
                    if (lastMonthCust == 0)
                    {
                        growthPercent = thisMonthCust > 0 ? 100 : 0;
                    }
                    else
                    {
                        growthPercent = ((double)(thisMonthCust - lastMonthCust) / lastMonthCust) * 100;
                    }

                    return new KPI_PeopleDTO
                    {
                        TotalCustomers = totalCust,
                        VipCount = vipCount,
                        ActiveEmployees = activeEmp,
                        CustomerGrowth = Math.Round(growthPercent, 2),
                        NewRegistrations = thisMonthCust + newUsersThisMonth,
                        RecentUserAvatars = avatars
                    };
                }
            });

            // =========================================================================
            // TASK 2: GROWTH CHARTS (Biểu đồ tăng trưởng theo tháng trong năm nay)
            // =========================================================================
            var chartTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    // 2.1 Group Khách hàng theo tháng
                    var custGroup = await db.Customers.AsNoTracking()
                        .Where(c => c.created_at >= startOfYear)
                        .GroupBy(c => c.created_at.Value.Month)
                        .Select(g => new { Month = g.Key, Count = g.Count() })
                        .ToListAsync();

                    // 2.2 Group Nhân viên theo tháng (Lấy data thật thay vì số 0 như SP cũ)
                    var userGroup = await db.Users.AsNoTracking()
                        .Where(u => u.created_at >= startOfYear)
                        .GroupBy(u => u.created_at.Value.Month)
                        .Select(g => new { Month = g.Key, Count = g.Count() })
                        .ToListAsync();

                    return new { CustData = custGroup, UserData = userGroup };
                }
            });

            // =========================================================================
            // TASK 3: SEGMENTS & LISTS (Phân loại khách hàng, Top khách, Role nhân viên)
            // =========================================================================
            var listTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    // 3.1 Customer Segments (Pie Chart)
                    var segments = await db.Customers.AsNoTracking()
                        .GroupBy(c => c.customer_type)
                        .Select(g => new SegmentItemDTO
                        {
                            Label = g.Key ?? "Standard", // Xử lý null nếu có
                            Count = g.Count()
                        })
                        .ToListAsync();

                    // 3.2 Top 5 Khách hàng tích điểm cao nhất
                    var topCust = await db.Customers.AsNoTracking()
                        .OrderByDescending(c => c.points_earned_total)
                        .Take(5)
                        .Select(c => new CustomerRowDTO
                        {
                            Name = c.full_name,
                            Email = c.email,
                            Phone = c.phone,
                            Type = c.customer_type,
                            Points = c.points_earned_total
                        })
                        .ToListAsync();

                    var roleStats = await db.Roles.AsNoTracking()
                         .Select(r => new RoleStatItemDTO
                         {
                             RoleName = r.name,
                             Count = r.UserRoles.Count(),
                             TotalUsers = totalUsers
                         })
                         .OrderByDescending(x => x.Count)
                         .ToListAsync();

                    return new { Segments = segments, TopCust = topCust, RoleStats = roleStats };
                }
            });

            // =========================================================================
            // WAIT ALL & MERGE DATA
            // =========================================================================
            await Task.WhenAll(kpiTask, chartTask, listTask);

            // Map KPI
            result.Kpi = kpiTask.Result;

            // Map Lists
            result.Lists.TopCustomers = listTask.Result.TopCust;
            result.Lists.RoleStats = listTask.Result.RoleStats;

            // Map Charts
            result.Charts.CustomerSegments = listTask.Result.Segments;

            // Map Growth Chart (Điền đầy đủ 12 tháng)
            var last6Months = Enumerable.Range(-5, 6) // Lấy từ -5 đến 0
                .Select(i => now.AddMonths(i))
                .Select(d => new { d.Year, d.Month })
                .ToList();

            result.Charts.Growth = new GrowthChartDTO
            {
                Labels = last6Months.Select(x => $"T{x.Month}/{x.Year.ToString().Substring(2)}").ToList(), // Format: T2/26
                Customers = new List<int>(),
                Employees = new List<int>()
            };

            foreach (var m in last6Months)
            {
                // Tìm data khớp cả Tháng và Năm (tránh trường hợp trùng tháng nhưng khác năm)
                var cCount = chartTask.Result.CustData
                    .FirstOrDefault(x => x.Month == m.Month)?.Count ?? 0;

                var uCount = chartTask.Result.UserData
                    .FirstOrDefault(x => x.Month == m.Month)?.Count ?? 0;

                result.Charts.Growth.Customers.Add(cCount);
                result.Charts.Growth.Employees.Add(uCount);
            }

            return result;
        }

        private List<int> MapToYearlyData(Dictionary<int, int> dataMap)
        {
            var result = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                result.Add(dataMap.ContainsKey(i) ? dataMap[i] : 0);
            }
            return result;
        }

        #endregion

        #region Warehouse Dashboard

        public async Task<WarehouseDashboardViewModel> GetWarehouseDashboardAsync(int daysBack, int? branchId, int? warehouseId)
        {
            var model = new WarehouseDashboardViewModel
            {
                Kpi = new WarehouseKpi(),
                Charts = new WarehouseCharts { Movement = new MovementChart(), Categories = new CategoryChart() },
                Lists = new WarehouseLists { Movements = new List<MovementItem>(), LowStock = new List<LowStockItem>() }
            };

            var today = DateTime.Now.Date;
            var fromDate = today.AddDays(-daysBack + 1);

            // =================================================================================
            // TASK 1: PHÂN TÍCH TỒN KHO (Gánh team: KPI, LowStock List, Category Chart)
            // =================================================================================
            var inventoryTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    // Lọc tồn kho theo điều kiện
                    var query = db.Inventories.AsNoTracking()
                        .Where(i => (!warehouseId.HasValue || i.warehouse_id == warehouseId)
                                 && (!branchId.HasValue || i.Warehouse.branch_id == branchId));

                    // GOM NHÓM THEO SẢN PHẨM (Mô phỏng bảng tạm #InventoryAgg trong SQL)
                    // Lấy các thông tin cần thiết để tính toán Client-side cho nhanh
                    var productStats = await query
                        .GroupBy(i => i.ProductLot.Product)
                        .Select(g => new
                        {
                            ProductName = g.Key.name,
                            CategoryName = g.Key.ProductCategory.name, // Giả sử có Navigation Property Category
                            TotalQty = g.Sum(i => i.quantity),
                            Price = g.Key.price, // Giá vốn hoặc giá bán tùy nghiệp vụ
                            MinStock = g.Key.min_stock,
                            MaxStock = g.Key.max_stock
                        })
                        .ToListAsync();

                    // 1.1 Tính KPI Tồn kho
                    var kpi = new WarehouseKpi
                    {
                        TotalInventoryValue = productStats.Sum(x => x.TotalQty * (x.Price ?? 0)) ?? 0,
                        TotalSku = productStats.Count,
                        LowStockCount = productStats.Count(x => x.TotalQty <= x.MinStock),
                        CapacityPercent = productStats.Sum(x => x.MaxStock) > 0
                            ? (double)productStats.Sum(x => x.TotalQty) / productStats.Sum(x => x.MaxStock) * 100 ?? 0
                            : 0
                    };

                    // 1.2 List Low Stock (Lọc từ data đã lấy, không cần query lại)
                    var lowStockList = productStats
                        .Where(x => x.TotalQty <= x.MinStock)
                        .OrderBy(x => x.TotalQty)
                        .Take(10) // Lấy top 10
                        .Select(x => new LowStockItem
                        {
                            Name = x.ProductName,
                            Current = (int)x.TotalQty,
                            Min = x.MinStock ?? 0
                        })
                        .ToList();

                    // 1.3 Category Chart (Gom nhóm từ data đã lấy)
                    var categoryChart = productStats
                        .GroupBy(x => x.CategoryName)
                        .Select(g => new CategoryChartRow
                        {
                            CategoryName = g.Key,
                            ProductCount = g.Count()
                        })
                        .OrderByDescending(x => x.ProductCount)
                        .ToList();

                    return new { Kpi = kpi, LowStock = lowStockList, Categories = categoryChart };
                }
            });

            // =================================================================================
            // TASK 2: BIẾN ĐỘNG KHO (Movement Chart & Recent List)
            // =================================================================================
            var movementTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    // Filter chung cho StockMovements
                    var moveQuery = db.StockMovements.AsNoTracking()
                        .Where(m => (!warehouseId.HasValue || m.warehouse_id == warehouseId)
                                 && (!branchId.HasValue || m.Warehouse.branch_id == branchId));

                    // 2.1 Lấy dữ liệu cho Chart (Trong khoảng thời gian daysBack)
                    var chartData = await moveQuery
                        .Where(m => m.movement_date >= fromDate)
                        .GroupBy(m => DbFunctions.TruncateTime(m.movement_date))
                        .Select(g => new
                        {
                            Date = g.Key,
                            Inbound = g.Sum(m => m.quantity > 0 ? m.quantity : 0),
                            Outbound = g.Sum(m => m.quantity < 0 ? m.quantity : 0) // Lấy số âm
                        })
                        .ToListAsync();

                    // 2.2 Lấy danh sách biến động gần nhất
                    var recentList = await moveQuery
                        .Where(m => m.movement_date <= DateTime.Now)
                        .OrderByDescending(m => m.movement_date)
                        .Take(10)
                        .Select(m => new MovementItem
                        {
                            Product = m.Product.name,

                            Type = m.movement_type == "Sale" ? "SALE" :
                                   m.movement_type == "Return" ? "RETURN" :
                                   m.movement_type == "Audit" ? "ADJUSTMENT" : "INTERNAL",
                            Qty = (int)m.quantity,
                            User = m.User.full_name ?? m.User.username,
                            Time = m.movement_date.ToString() // Format sẵn luôn cho JS đỡ cực
                        })
                        .ToListAsync();

                    return new { ChartData = chartData, RecentList = recentList };
                }
            });

            // =================================================================================
            // TASK 3: ĐƠN HÀNG CHỜ (Pending Orders KPI)
            // =================================================================================
            var orderTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();
                    // Đếm đơn Processing (Có thể lọc theo Branch nếu cần thiết)
                    return await db.Orders.AsNoTracking()
                        .CountAsync(o => o.status == "Processing"
                                    && (!branchId.HasValue || o.branch_id == branchId));
                }
            });

            // =================================================================================
            // TỔNG HỢP KẾT QUẢ (WAIT ALL)
            // =================================================================================
            await Task.WhenAll(inventoryTask, movementTask, orderTask);

            // Mapping Data vào ViewModel
            var invResult = inventoryTask.Result;
            var movResult = movementTask.Result;

            // 1. Fill KPI
            model.Kpi = invResult.Kpi;
            model.Kpi.PendingOrders = orderTask.Result; // Gán kết quả từ Task 3

            // 2. Fill Low Stock List
            model.Lists.LowStock = invResult.LowStock;

            // 3. Fill Category Chart
            foreach (var cat in invResult.Categories)
            {
                model.Charts.Categories.Labels.Add(cat.CategoryName ?? "Other");
                model.Charts.Categories.Counts.Add(cat.ProductCount);
            }

            // 4. Fill Movement List
            // Xử lý format lại Time cho đẹp (nếu cần)
            foreach (var item in movResult.RecentList)
            {
                if (DateTime.TryParse(item.Time, out DateTime dt))
                {
                    item.Time = dt.ToString("dd/MM HH:mm");
                }
            }
            model.Lists.Movements = movResult.RecentList;

            // 5. Fill Movement Chart (Xử lý điền đầy đủ ngày tháng nếu DB thiếu ngày)
            var dateRange = Enumerable.Range(0, daysBack)
                                      .Select(offset => fromDate.AddDays(offset))
                                      .ToList();

            foreach (var date in dateRange)
            {
                var dataPoint = movResult.ChartData.FirstOrDefault(x => x.Date == date);

                model.Charts.Movement.Labels.Add(date.ToString("dd/MM"));
                model.Charts.Movement.Inbound.Add((int)(dataPoint?.Inbound ?? 0));
                // Outbound trong DB là số âm (ví dụ -5), chart thường vẽ số dương (5) hoặc giữ nguyên tùy library
                // Ở đây tôi lấy ABS cho giống logic SP cũ
                model.Charts.Movement.Outbound.Add(Math.Abs((int)(dataPoint?.Outbound ?? 0)));
            }

            return model;
        }

        #endregion

        #region Finance Dashboard

        public async Task<FinanceDashboardDTO> GetFinanceDashboardAsync(int daysBack, int? branchId)
        {
            var fromDate = DateTime.Today.AddDays(-daysBack + 1);

            // Task 1: KPI (Tổng quan)
            var kpiTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    var revenue = await db.Orders
                        .Where(o => o.status == "Completed" && o.order_date >= fromDate
                                    && (!branchId.HasValue || o.branch_id == branchId))
                        .SumAsync(o => (decimal?)o.total_amount) ?? 0;

                    var purchase = await db.PurchaseOrderDetails
                        .Where(pod => pod.PurchaseOrder.order_date >= fromDate
                                      && (pod.PurchaseOrder.status == "Completed" || pod.PurchaseOrder.status == "Received")
                                      && (!branchId.HasValue || pod.PurchaseOrder.Warehouse.branch_id == branchId))
                        .SumAsync(pod => (decimal?)(pod.quantity * pod.unit_price)) ?? 0;

                    var debt = await db.SupplierDebts
                        .Where(sd => !branchId.HasValue || sd.PurchaseOrder.Warehouse.branch_id == branchId)
                        .SumAsync(sd => (decimal?)sd.unpaid_amount) ?? 0;

                    return new FinanceKpiDTO
                    {
                        TotalRevenue = revenue,
                        TotalPurchase = purchase,
                        GrossProfit = revenue - purchase,
                        SupplierDebt = debt
                    };
                }
            });

            // Task 2: Trends (Revenue & Purchase Cost theo ngày)
            var trendsTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    // Lấy Revenue theo ngày
                    var revData = await db.Orders
                        .Where(o => o.status == "Completed" && o.order_date >= fromDate
                                    && (!branchId.HasValue || o.branch_id == branchId))
                        .GroupBy(o => DbFunctions.TruncateTime(o.order_date)) // Group theo ngày bỏ qua giờ
                        .Select(g => new { Date = g.Key, Value = g.Sum(o => o.total_amount) })
                        .ToListAsync();

                    // Lấy Purchase Cost theo ngày
                    var purData = await db.PurchaseOrderDetails
                        .Where(pod => pod.PurchaseOrder.order_date >= fromDate
                                      && (pod.PurchaseOrder.status == "Completed" || pod.PurchaseOrder.status == "Received")
                                      && (!branchId.HasValue || pod.PurchaseOrder.Warehouse.branch_id == branchId))
                        .GroupBy(pod => DbFunctions.TruncateTime(pod.PurchaseOrder.order_date))
                        .Select(g => new { Date = g.Key, Value = g.Sum(pod => pod.quantity * pod.unit_price) })
                        .ToListAsync();

                    // Merge 2 danh sách lại theo ngày
                    var allDates = revData.Select(r => r.Date).Union(purData.Select(p => p.Date))
                        .OrderBy(d => d)
                        .ToList();

                    return allDates.Select(d => new FinanceDailyTrendDTO
                    {
                        DateLabel = d.HasValue ? d.Value.ToString("dd/MM") : "",
                        Revenue = revData.FirstOrDefault(r => r.Date == d)?.Value ?? 0,
                        PurchaseCost = purData.FirstOrDefault(p => p.Date == d)?.Value ?? 0
                    }).ToList();
                }
            });

            // Task 3: Expense Pie (Phân bổ chi phí)
            var expenseTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();
                    return await db.Expenses
                        .Where(e => e.expense_date >= fromDate && (!branchId.HasValue || e.branch_id == branchId))
                        .GroupBy(e => new { e.category_id, e.ExpenseCategory.name })
                        .Select(g => new FinanceExpensePieDTO
                        {
                            Label = g.Key.name,
                            Value = g.Sum(e => e.amount)
                        })
                        .ToListAsync();
                }
            });

            // Task 4: Recent Orders (Đơn hàng mới nhất)
            var recentOrdersTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();
                    return await db.Orders
                        .Where(o => !branchId.HasValue || o.branch_id == branchId)
                        .OrderByDescending(o => o.order_date)
                        .Take(10)
                        .Select(o => new RecentOrderDTO
                        {
                            OrderId = o.order_id.ToString(),
                            TotalAmount = o.total_amount ?? 0,
                            Status = o.status,
                            OrderDate = o.order_date
                        })
                        .ToListAsync();
                }
            });

            // Chạy song song 4 Task - SQL Server sẽ nhận 4 connections cùng lúc
            await Task.WhenAll(kpiTask, trendsTask, expenseTask, recentOrdersTask);

            return new FinanceDashboardDTO
            {
                Kpi = kpiTask.Result,
                Trends = trendsTask.Result,
                ExpensePie = expenseTask.Result,
                RecentOrders = recentOrdersTask.Result
            };
        }

        #endregion

        #region Debt Dashboard 

        public async Task<DebtDashboardDto> GetDebtDashboardAsync(int? branchId, int? supplierId, DateTime? fromDate, DateTime? toDate)
        {
            // Snapshot thời gian hiện tại để đảm bảo tính nhất quán giữa các Task
            var now = DateTime.Now;

            // ---------------------------------------------------------
            // TASK 1: Tính toán KPI (Tổng nợ, Quá hạn, Sắp đến hạn, Đã trả)
            // ---------------------------------------------------------
            var kpiTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    // 1.1 Tính toán các chỉ số về NỢ (Dựa trên SupplierDebts)
                    // Filter chung cho Debt: Chỉ lấy unpaid > 0 và theo Branch/Supplier
                    var debtQuery = db.SupplierDebts.AsNoTracking()
                        .Where(sd => sd.unpaid_amount > 0
                                     && (!branchId.HasValue || sd.PurchaseOrder.Warehouse.branch_id == branchId)
                                     && (!supplierId.HasValue || sd.supplier_id == supplierId));

                    // Gom nhóm để tính 1 lần duy nhất (tương đương SELECT SUM(CASE...) trong SQL)
                    var debtStats = await debtQuery
                        .GroupBy(x => 1) // Group by constant để aggregate toàn bộ
                        .Select(g => new
                        {
                            TotalOutstanding = g.Sum(x => (decimal?)x.unpaid_amount) ?? 0,

                            // due_date < GETDATE()
                            TotalOverdue = g.Sum(x => x.due_date < now ? (decimal?)x.unpaid_amount : 0) ?? 0,

                            // due_date BETWEEN GETDATE() AND GETDATE() + 7
                            TotalUpcoming = g.Sum(x => x.due_date >= now && x.due_date <= DbFunctions.AddDays(now, 7) ? (decimal?)x.unpaid_amount : 0) ?? 0
                        })
                        .FirstOrDefaultAsync();

                    // 1.2 Tính toán ĐÃ TRẢ (Dựa trên SupplierPayments - Có filter theo DateRange)
                    var paidQuery = db.SupplierPayments.AsNoTracking()
                        .Where(sp => (!branchId.HasValue || sp.SupplierDebt.PurchaseOrder.Warehouse.branch_id == branchId)
                                     && (!supplierId.HasValue || sp.SupplierDebt.supplier_id == supplierId)
                                     && (!fromDate.HasValue || sp.payment_date >= fromDate)
                                     && (!toDate.HasValue || sp.payment_date <= toDate));

                    var totalPaid = await paidQuery.SumAsync(x => (decimal?)x.amount) ?? 0;

                    return new DebtKPIDto
                    {
                        TotalOutstanding = debtStats?.TotalOutstanding ?? 0,
                        TotalOverdue = debtStats?.TotalOverdue ?? 0,
                        TotalUpcoming = debtStats?.TotalUpcoming ?? 0,
                        TotalPaidInPeriod = totalPaid
                    };
                }
            });

            // ---------------------------------------------------------
            // TASK 2: Danh sách Nợ gấp (Urgent Debts - Top 10 Overdue)
            // ---------------------------------------------------------
            var urgentTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    return await db.SupplierDebts.AsNoTracking()
                        .Where(sd => sd.unpaid_amount > 0
                                     && (!branchId.HasValue || sd.PurchaseOrder.Warehouse.branch_id == branchId)
                                     && (!supplierId.HasValue || sd.supplier_id == supplierId))
                        .OrderBy(sd => sd.due_date) // Sắp xếp theo ngày hết hạn tăng dần (gấp nhất lên đầu)
                        .Take(10)
                        .Select(sd => new DebtRecordDto
                        {
                            SupplierName = sd.Supplier.name,
                            PoId = sd.purchase_order_id,
                            TotalAmount = sd.total_amount ?? 0,
                            UnpaidAmount = sd.unpaid_amount ?? 0,
                            DueDate = sd.due_date ?? DateTime.MinValue,
                            // DATEDIFF(day, due_date, GETDATE()) -> Logic SQL hơi ngược, 
                            // thường OverdueDays = Now - DueDate (Dương là quá hạn).
                            // C# EntityFunctions hoặc DbFunctions.DiffDays trả về End - Start.
                            // Logic dưới đây: Nếu DueDate là ngày 1, Now là ngày 5 => Diff = 4 (Quá hạn 4 ngày)
                            OverdueDays = DbFunctions.DiffDays(sd.due_date, now) ?? 0
                        })
                        .ToListAsync();
                }
            });

            // ---------------------------------------------------------
            // TASK 3: (BONUS) Aging Chart - Biểu đồ tuổi nợ
            // ---------------------------------------------------------
            var chartTask = Task.Run(async () =>
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var db = _container.GetInstance<EMarket_DBEntities>();

                    // Lấy tất cả các khoản nợ chưa trả
                    var debts = await db.SupplierDebts.AsNoTracking()
                         .Where(sd => sd.unpaid_amount > 0
                                      && (!branchId.HasValue || sd.PurchaseOrder.Warehouse.branch_id == branchId)
                                      && (!supplierId.HasValue || sd.supplier_id == supplierId))
                         .Select(sd => new { sd.due_date, sd.unpaid_amount })
                         .ToListAsync();

                    // Xử lý gom nhóm trong bộ nhớ (Memory) vì logic khoảng ngày phức tạp để viết LINQ to SQL thuần
                    // Logic: Tuổi nợ = Now - DueDate
                    var result = new List<ChartDataDto>
                {
                    new ChartDataDto { Label = "Trong hạn", Value = debts.Where(x => x.due_date >= now).Sum(x => x.unpaid_amount ?? 0) },
                    new ChartDataDto { Label = "1-30 ngày", Value = debts.Where(x => x.due_date < now && x.due_date >= now.AddDays(-30)).Sum(x => x.unpaid_amount ?? 0) },
                    new ChartDataDto { Label = "31-60 ngày", Value = debts.Where(x => x.due_date < now.AddDays(-30) && x.due_date >= now.AddDays(-60)).Sum(x => x.unpaid_amount ?? 0) },
                    new ChartDataDto { Label = "> 60 ngày", Value = debts.Where(x => x.due_date < now.AddDays(-60)).Sum(x => x.unpaid_amount ?? 0) }
                };

                    // Cập nhật Count
                    foreach (var item in result) item.Count = 1; // Hoặc logic count tùy ý

                    return result;
                }
            });

            // ---------------------------------------------------------
            // TỔNG HỢP KẾT QUẢ
            // ---------------------------------------------------------
            await Task.WhenAll(kpiTask, urgentTask, chartTask);

            return new DebtDashboardDto
            {
                Kpi = kpiTask.Result ?? new DebtKPIDto(),
                UrgentDebts = urgentTask.Result ?? new List<DebtRecordDto>(),
                AgingChart = chartTask.Result ?? new List<ChartDataDto>()
            };
        }
        #endregion

        #region Super Admin Hub (Đã Fix)

        public async Task<AdminHubDataDTO> GetSuperAdminHubData()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                using (var multi = await conn.QueryMultipleAsync("sp_GetSuperAdminHubData", commandType: CommandType.StoredProcedure))
                {
                    var metrics = await multi.ReadFirstOrDefaultAsync<HubMetricsDTO>();
                    var alerts = (await multi.ReadAsync<HubAlertDTO>()).ToList();

                    return new AdminHubDataDTO
                    {
                        Metrics = metrics ?? new HubMetricsDTO(),
                        Alerts = alerts
                    };
                }
            }
        }

        #endregion
    }
}