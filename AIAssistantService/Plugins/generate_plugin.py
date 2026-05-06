import os

endpoints = """
// 1. AI Analysis
GetReplenishmentAdvice(int branchId = 1) -> GET api/admin/ai-analysis/replenishment-advice/{branchId}
GetRecommendations(int branchId = 1) -> GET api/admin/ai-analysis/recommendations/{branchId}
GetAnomalies(int branchId = 1) -> GET api/admin/ai-analysis/anomalies/{branchId}
GetProductInsights(int branchId = 1) -> GET api/admin/ai-analysis/insights/{branchId}
GetInventoryForecast(int branchId = 1) -> GET api/admin/ai-analysis/inventory-forecast/{branchId}
GetSalesForecast(int productId, int branchId = 1) -> GET api/admin/ai-analysis/sales-forecast/{productId}/{branchId}
GetTopPredictedProducts(int branchId = 1, int topCount = 10) -> GET api/admin/ai-analysis/top-predicted/{branchId}?topCount={topCount}
GetDeadstockAnalysis(int branchId = 1) -> GET api/admin/ai-analysis/deadstock/{branchId}
GetProductHistory(int productId, int branchId = 1, string start = "", string end = "") -> GET api/admin/ai-analysis/product-history/{productId}/{branchId}?start={start}&end={end}
GetLotFinancialRisk(int branchId = 1) -> GET api/admin/ai-analysis/lot-financial-risk/{branchId}
RunProphet() -> POST api/admin/ai-analysis/run-prophet

// 2. Customer
GetAllCustomers() -> GET api/admin/customer
SearchCustomers(string keyword = "") -> GET api/admin/customer/search?keyword={keyword}
GetCustomerById(int id) -> GET api/admin/customer/{id}
GetCustomerEmail(int id) -> GET api/admin/customer/{id}/email
GetCustomerStats(string fromDate = "") -> GET api/admin/customer/stats?fromDate={fromDate}
CountCreatedInMonth(string fromDate, string toDate) -> GET api/admin/customer/count-in-month?fromDate={fromDate}&toDate={toDate}
GetCustomerSegments() -> GET api/admin/customer/segments
GetCustomersCreatedByMonth() -> GET api/admin/customer/created-by-month
GetTopCustomers(int top = 10) -> GET api/admin/customer/top?top={top}
GetAddressesByCustomer(int customerId) -> GET api/admin/customer/address/by-customer/{customerId}
GetDefaultAddress(int customerId) -> GET api/admin/customer/address/default/{customerId}
GetAddressById(int id) -> GET api/admin/customer/address/{id}
GetAllLoyalty() -> GET api/admin/customer/loyalty
GetLoyaltyById(int id) -> GET api/admin/customer/loyalty/{id}

// 3. Dashboard
GetSummary(int branchId = 1) -> GET api/admin/dashboard/summary?branchId={branchId}
GetBranchPerformance(int branchId = 1, string fromDate = "", string toDate = "") -> GET api/admin/dashboard/branch-performance?branchId={branchId}&fromDate={fromDate}&toDate={toDate}
GetStockChart(int branchId = 1) -> GET api/admin/dashboard/stock-chart?branchId={branchId}
GetOverview(int branchId = 1, string fromDate = "", string toDate = "", string groupBy = "day") -> GET api/admin/dashboard/overview?branchId={branchId}&fromDate={fromDate}&toDate={toDate}&groupBy={groupBy}
GetPeopleDashboard() -> GET api/admin/dashboard/people
GetWarehouseDashboard(int dayBacks = 30, int branchId = 1, int warehouseId = 0) -> GET api/admin/dashboard/warehouse?dayBacks={dayBacks}&branchId={branchId}&warehouseId={warehouseId}
GetFinanceDashboard(int daysBack = 30, int branchId = 1) -> GET api/admin/dashboard/finance?daysBack={daysBack}&branchId={branchId}
GetDebtDashboard(int branchId = 1, int supplierId = 0, string fromDate = "", string toDate = "") -> GET api/admin/dashboard/debt?branchId={branchId}&supplierId={supplierId}&fromDate={fromDate}&toDate={toDate}
GetSuperAdminHub() -> GET api/admin/dashboard/super-admin

// 4. Expense
GetExpenses(int branchId = 1, int categoryId = 0, string fromDate = "", string toDate = "", string status = "") -> GET api/admin/expense/list?branchId={branchId}&categoryId={categoryId}&fromDate={fromDate}&toDate={toDate}&status={status}
GetExpenseDetail(int id) -> GET api/admin/expense/{id}
GetAllExpenseCategories() -> GET api/admin/expense/categories
GetActiveExpenseCategories() -> GET api/admin/expense/categories/active
GetExpenseCategoryById(int categoryId) -> GET api/admin/expense/categories/{categoryId}

// 5. Inventory
GetAllInventory() -> GET api/admin/inventory/stock/all
GetFilteredInventory(int productId = 0, int warehouseId = 0) -> GET api/admin/inventory/stock/filter?productId={productId}&warehouseId={warehouseId}
GetInventoryByBranch(int branchId = 1) -> GET api/admin/inventory/stock/by-branch?branchId={branchId}
GetInventoryById(int id) -> GET api/admin/inventory/stock/{id}
GetInventoryByProductIds(string productIdsJson, int warehouseId = 0, int branchId = 1) -> POST api/admin/inventory/stock/by-product-ids?warehouseId={warehouseId}&branchId={branchId}
GetStockMovements(int start = 0, int length = 10, int warehouseId = 0, string type = "", string fromDate = "", string toDate = "", string keyword = "") -> GET api/admin/inventory/stock/movements?start={start}&length={length}&warehouseId={warehouseId}&type={type}&fromDate={fromDate}&toDate={toDate}&keyword={keyword}
GetTotalStock(int productId, int warehouseId) -> GET api/admin/inventory/stock/total-actual?productId={productId}&warehouseId={warehouseId}
GetAllPurchases() -> GET api/admin/inventory/purchase/all
SearchPurchases(string keyword = "", int supplierId = 0, int branchId = 1, int warehouseId = 0, string status = "", string paymentStatus = "", string fromDate = "", string toDate = "") -> GET api/admin/inventory/purchase/search?keyword={keyword}&supplierId={supplierId}&branchId={branchId}&warehouseId={warehouseId}&status={status}&paymentStatus={paymentStatus}&fromDate={fromDate}&toDate={toDate}
GetPurchaseById(int id) -> GET api/admin/inventory/purchase/{id}
GetPurchasesByBranch(int branchId = 1, string fromDate = "", string toDate = "") -> GET api/admin/inventory/purchase/by-branch?branchId={branchId}&fromDate={fromDate}&toDate={toDate}
GetAllDebts() -> GET api/admin/inventory/debt/all
GetFilteredDebts(string keyword = "", int supplierId = 0, string status = "", string fromDate = "", string toDate = "") -> GET api/admin/inventory/debt/list?keyword={keyword}&supplierId={supplierId}&status={status}&fromDate={fromDate}&toDate={toDate}
GetDebtById(int id) -> GET api/admin/inventory/debt/{id}
GetDebtByPurchaseOrder(int purchaseOrderId) -> GET api/admin/inventory/debt/by-purchase/{purchaseOrderId}
GetDebtsByIds(string idsJson) -> POST api/admin/inventory/debt/by-ids
GetOverdueDebts() -> GET api/admin/inventory/debt/overdue
GetNearDueDebts(int days = 7) -> GET api/admin/inventory/debt/near-due?days={days}
GetPaymentsByDebt(int debtId) -> GET api/admin/inventory/debt/{debtId}/payments
GetPaymentMailInfo(int paymentId) -> GET api/admin/inventory/debt/payment-mail-info/{paymentId}
GetInternalDebtDetail(string debtIdsJson) -> POST api/admin/inventory/debt/internal-detail
GetAllWarehouses() -> GET api/admin/inventory/warehouses/all
GetWarehousesByBranch(int branchId = 1) -> GET api/admin/inventory/warehouses?branchId={branchId}
SearchWarehouses(string name = "", int branchId = 1) -> GET api/admin/inventory/warehouses/search?name={name}&branchId={branchId}
GetWarehouseDetail(int id) -> GET api/admin/inventory/warehouses/{id}
GetWarehouseDict() -> GET api/admin/inventory/warehouses/dict
GetWarehousesByIds(string idsJson) -> POST api/admin/inventory/warehouses/by-ids

// 6. Log
GetAuditLogs(int start = 0, int length = 10, string tableName = "", string action = "", string search = "", string fromDate = "", string toDate = "") -> GET api/admin/logs/audit?start={start}&length={length}&tableName={tableName}&action={action}&search={search}&fromDate={fromDate}&toDate={toDate}
GetAuditLogDetail(long id) -> GET api/admin/logs/audit/{id}
GetAppLogs(int start = 0, int length = 10, string logLevel = "", string logger = "", string search = "", string fromDate = "", string toDate = "") -> GET api/admin/logs/app?start={start}&length={length}&logLevel={logLevel}&logger={logger}&search={search}&fromDate={fromDate}&toDate={toDate}
GetAppLogDetail(long id) -> GET api/admin/logs/app/{id}
GetLogStatistics() -> GET api/admin/logs/statistics
GetLatestSystemEvents() -> GET api/admin/logs/system-events

// 7. Product
GetAllProducts() -> GET api/admin/product-management/products
SearchProducts(string keyword = "", int categoryId = 0, int branchId = 1, int supplierId = 0, int warehouseId = 0) -> GET api/admin/product-management/products/search?keyword={keyword}&categoryId={categoryId}&branchId={branchId}&supplierId={supplierId}&warehouseId={warehouseId}
SearchProductsSimple(string keyword = "", int branchId = 1) -> GET api/admin/product-management/products/search-simple?keyword={keyword}&branchId={branchId}
GetProductById(int id) -> GET api/admin/product-management/products/{id}
GetProductsByIds(string idsJson) -> POST api/admin/product-management/products/by-ids
GetProductNamesByIds(string productIdsJson) -> POST api/admin/product-management/products/names-by-ids
GetInactiveProducts(string keyword = "", int categoryId = 0, int branchId = 1, int supplierId = 0, int warehouseId = 0) -> GET api/admin/product-management/products/inactive?keyword={keyword}&categoryId={categoryId}&branchId={branchId}&supplierId={supplierId}&warehouseId={warehouseId}
GetLowStockAlerts(int top = 10) -> GET api/admin/product-management/products/low-stock-alerts?top={top}
GetProductImages(int productId) -> GET api/admin/product-management/products/{productId}/images
GetProductImageById(int imageId) -> GET api/admin/product-management/products/images/{imageId}
GetAllLots() -> GET api/admin/product-management/lots
GetLotsByProduct(int productId) -> GET api/admin/product-management/products/{productId}/lots
GetLotDetail(int lotId) -> GET api/admin/product-management/lots/{lotId}
GetLotsByIds(string idsJson) -> POST api/admin/product-management/lots/by-ids
GetLotDetailsByIds(string lotIdsJson) -> POST api/admin/product-management/lots/details-by-ids
GetLotIdsByProductId(int productId) -> GET api/admin/product-management/lots/by-product/{productId}/lot-ids
FindExistingLot(int productId, string manufacturingDate = "", string expiryDate = "") -> GET api/admin/product-management/lots/find-existing?productId={productId}&manufacturingDate={manufacturingDate}&expiryDate={expiryDate}
GetAllCategories() -> GET api/admin/product-management/categories
GetCategoryById(int id) -> GET api/admin/product-management/categories/{id}
SearchCategories(string name = "") -> GET api/admin/product-management/categories/search?name={name}
GetCategoriesByIds(string idsJson) -> POST api/admin/product-management/categories/by-ids
GetAllSuppliers() -> GET api/admin/product-management/suppliers
SearchSuppliers(string name = "") -> GET api/admin/product-management/suppliers/search?name={name}
GetSupplierById(int id) -> GET api/admin/product-management/suppliers/{id}
GetSuppliersByIds(string idsJson) -> POST api/admin/product-management/suppliers/by-ids

// 8. Quotation
GetQuotations(string keyword = "", int branchId = 1, string status = "", string fromDate = "", string toDate = "") -> GET api/admin/quotation/list?keyword={keyword}&branchId={branchId}&status={status}&fromDate={fromDate}&toDate={toDate}
GetQuotationDetail(int id) -> GET api/admin/quotation/{id}

// 9. Sales
GetAllOrders() -> GET api/admin/sales/orders/all
GetOrders(int start = 0, int length = 10, int userId = 0, int branchId = 1, string status = "", string fromDate = "", string toDate = "", string keyword = "") -> GET api/admin/sales/orders?start={start}&length={length}&userId={userId}&branchId={branchId}&status={status}&fromDate={fromDate}&toDate={toDate}&keyword={keyword}
GetOrderDetail(int id) -> GET api/admin/sales/orders/{id}
GetFullOrdersByBranch(int branchId = 1, string fromDate = "", string toDate = "") -> GET api/admin/sales/orders/full-by-branch?branchId={branchId}&fromDate={fromDate}&toDate={toDate}
GetOrdersByBranch(int branchId = 1, string fromDate = "", string toDate = "") -> GET api/admin/sales/orders/by-branch?branchId={branchId}&fromDate={fromDate}&toDate={toDate}
GetOrderDetails(int orderId) -> GET api/admin/sales/orders/{orderId}/details
GetPaymentsByOrder(int orderId) -> GET api/admin/sales/orders/{orderId}/payments
GetAllPromotions() -> GET api/admin/sales/promotions
SearchPromotions(string keyword = "", int categoryId = 0, string discountType = "", string cusType = "", string fromDate = "", string toDate = "") -> GET api/admin/sales/promotions/search?keyword={keyword}&categoryId={categoryId}&discountType={discountType}&cusType={cusType}&fromDate={fromDate}&toDate={toDate}
GetPromotionById(int id) -> GET api/admin/sales/promotions/{id}
GetActivePromotions() -> GET api/admin/sales/promotions/active

// 10. User Management
GetAllUsers() -> GET api/admin/user-management/users
SearchUsers(string keyword = "") -> GET api/admin/user-management/users/search?keyword={keyword}
GetUserById(int id) -> GET api/admin/user-management/users/{id}
GetUsersByIds(string userIdsJson) -> POST api/admin/user-management/users/by-ids
GetUserDict() -> GET api/admin/user-management/users/dict
GetUserStats() -> GET api/admin/user-management/users/stats
CountNewUsers(string fromDate = "") -> GET api/admin/user-management/users/count-new?fromDate={fromDate}
GetRecentAvatars(int top = 5) -> GET api/admin/user-management/users/recent-avatars?top={top}
GetWarehouseManagerEmails() -> GET api/admin/user-management/users/warehouse-managers-emails
GetAllBranches() -> GET api/admin/user-management/branches
SearchBranches(string name = "", double lat = 0, double lng = 0, double maxDist = 10) -> GET api/admin/user-management/branches/search?name={name}&lat={lat}&lng={lng}&maxDist={maxDist}
GetBranchById(int id) -> GET api/admin/user-management/branches/{id}
GetBranchesByIds(string idsJson) -> POST api/admin/user-management/branches/by-ids
GetBranchDict() -> GET api/admin/user-management/branches/dict
GetAllRoles() -> GET api/admin/user-management/roles
GetRoleById(int id) -> GET api/admin/user-management/roles/{id}
GetRolePermissions(int id) -> GET api/admin/user-management/roles/{id}/permissions
GetAllPermissions() -> GET api/admin/user-management/permissions
GetPermissionById(int id) -> GET api/admin/user-management/permissions/{id}
"""

lines = []
lines.append("using System;")
lines.append("using System.ComponentModel;")
lines.append("using System.Net.Http.Json;")
lines.append("using System.Collections.Generic;")
lines.append("using System.Threading.Tasks;")
lines.append("using System.Net.Http;")
lines.append("using Microsoft.SemanticKernel;")
lines.append("")
lines.append("namespace AIAssistantService.Plugins")
lines.append("{")
lines.append("    public class EMarketApiPlugin")
lines.append("    {")
lines.append("        private readonly HttpClient _client;")
lines.append("")
lines.append("        public EMarketApiPlugin(IHttpClientFactory factory)")
lines.append("        {")
lines.append('            _client = factory.CreateClient("EMarketClient");')
lines.append("        }")
lines.append("")
lines.append("        private async Task<string> SafeGetAsync(string url)")
lines.append("        {")
lines.append("            try")
lines.append("            {")
lines.append("                var response = await _client.GetAsync(url);")
lines.append("                Console.ForegroundColor = ConsoleColor.Green;")
lines.append('                Console.WriteLine($"[SYSTEM LOG] AI ĐANG GỌI API: {url}");')
lines.append("                Console.ResetColor();")
lines.append("                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();")
lines.append('                return $"Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";')
lines.append("            }")
lines.append('            catch (Exception ex) { return $"Failed to execute API call: {ex.Message}"; }')
lines.append("        }")
lines.append("")
lines.append("        private async Task<string> SafePostAsync<T>(string url, T data)")
lines.append("        {")
lines.append("            try")
lines.append("            {")
lines.append("                var response = await _client.PostAsJsonAsync(url, data);")
lines.append("                Console.ForegroundColor = ConsoleColor.Green;")
lines.append('                Console.WriteLine($"[SYSTEM LOG] AI ĐANG GỌI API: {url}");')
lines.append("                Console.ResetColor();")
lines.append("                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();")
lines.append('                return $"Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";')
lines.append("            }")
lines.append('            catch (Exception ex) { return $"Failed to execute API call: {ex.Message}"; }')
lines.append("        }")
lines.append("")

for line in endpoints.strip().split("\n"):
    line = line.strip()
    if not line:
        continue
    if line.startswith("//"):
        lines.append(f"        {line}")
        continue
    
    parts = line.split(" -> ")
    method_sig = parts[0]
    method_name = method_sig.split("(")[0]
    args_str = method_sig.split("(")[1].split(")")[0]
    
    method_call = parts[1]
    http_method = method_call.split(" ")[0]
    url = method_call.split(" ")[1]

    lines.append("        [KernelFunction]")
    lines.append(f'        [Description("ACTION {method_name}. USE WHEN requested. RETURNS JSON.")]')
    
    args = []
    if args_str:
        for arg in args_str.split(", "):
            arg_parts = arg.split(" ")
            arg_type = arg_parts[0]
            arg_name = arg_parts[1]
            default_val = None
            if "=" in arg:
                default_val = arg.split(" = ")[1]
                args.append(f'[Description("{arg_name}.")] {arg_type} {arg_name} = {default_val}')
            else:
                args.append(f'[Description("{arg_name}.")] {arg_type} {arg_name}')
                
    args_formatted = ", ".join(args)
    lines.append(f"        public Task<string> {method_name}({args_formatted})")
    lines.append("        {")
    
    # fix url variables, ignore ones that are 0 or ""
    path_url = url.split("?")[0]
    qs_url = url.split("?")[1] if "?" in url else ""
    
    # construct query string safely
    if qs_url:
        lines.append('            var qs = new List<string>();')
        for qp in qs_url.split("&"):
            k = qp.split("=")[0]
            v = qp.split("=")[1].strip("{}")
            # find type
            v_type = "string"
            for a in args_str.split(", "):
                if a.split(" ")[1] == v:
                    v_type = a.split(" ")[0]
            
            if v_type in ["int", "long", "double"]:
                lines.append(f'            if ({v} != 0) qs.Add($"{k}={{{v}}}");')
            elif v_type == "string":
                lines.append(f'            if (!string.IsNullOrEmpty({v})) qs.Add($"{k}={{{v}}}");')
            else:
                lines.append(f'            if ({v} != null) qs.Add($"{k}={{{v}}}");')
        
        lines.append('            var qStr = qs.Count > 0 ? "?" + string.Join("&", qs) : "";')
        
    if http_method == "GET":
        if qs_url:
            lines.append(f'            return SafeGetAsync($"{path_url}" + qStr);')
        else:
            lines.append(f'            return SafeGetAsync($"{path_url}");')
    else:
        # POST
        post_body = "new {}"
        for a in args_str.split(", "):
            if "Json" in a:
                v = a.split(" ")[1]
                post_body = v
                break
        
        if post_body != "new {}":
            # deserialize json to object
            lines.append(f'            var payload = System.Text.Json.JsonSerializer.Deserialize<object>({post_body});')
            post_body = "payload"
        
        if qs_url:
            lines.append(f'            return SafePostAsync($"{path_url}" + qStr, {post_body});')
        else:
            lines.append(f'            return SafePostAsync($"{path_url}", {post_body});')
            
    lines.append("        }")
    lines.append("")

lines.append("    }")
lines.append("}")

with open(r"d:\Bai_Tap_Cac_Mon\Nghien_Cuu_Khoa_Hoc\AIAssistantService\Plugins\EMarketApiPlugin.cs", "w", encoding="utf-8") as f:
    f.write("\n".join(lines))
