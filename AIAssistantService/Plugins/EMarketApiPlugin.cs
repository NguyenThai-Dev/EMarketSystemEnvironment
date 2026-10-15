using System;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using Microsoft.SemanticKernel;

namespace AIAssistantService.Plugins
{
    public class EMarketApiPlugin
    {
        private readonly HttpClient _client;

        public EMarketApiPlugin(IHttpClientFactory factory, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _client = factory.CreateClient("EMarketClient");
            
            var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private string SmartRefineJson(string url, string rawJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawJson)) return rawJson;
                string trimmed = rawJson.TrimStart();
                if (!trimmed.StartsWith("{") && !trimmed.StartsWith("[")) return rawJson;

                var node = JsonNode.Parse(rawJson);
                if (node == null) return rawJson;

                int maxItems = url.Contains("anomalies") || url.Contains("forecast") || url.Contains("insights") ? 20 : 10;

                RefineNodeInPlace(node, maxItems);
                
                return node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
            }
            catch
            {
                return rawJson;
            }
        }

        private void RefineNodeInPlace(JsonNode node, int maxItems)
        {
            if (node is JsonArray array)
            {
                while (array.Count > maxItems)
                {
                    array.RemoveAt(array.Count - 1);
                }
                foreach (var item in array)
                {
                    if (item != null) RefineNodeInPlace(item, maxItems);
                }
            }
            else if (node is JsonObject obj)
            {
                string[] noisyFields = { "createdAt", "updatedAt", "createdBy", "lastUpdatedBy", "concurrencyToken", "isDeleted", "isActive" };
                foreach (var field in noisyFields)
                {
                    var keysToRemove = new List<string>();
                    foreach(var k in obj.Select(x => x.Key)) {
                        if (k.Equals(field, StringComparison.OrdinalIgnoreCase)) keysToRemove.Add(k);
                    }
                    foreach(var k in keysToRemove) obj.Remove(k);
                }

                foreach (var kvp in obj.ToList())
                {
                    if (kvp.Value != null) RefineNodeInPlace(kvp.Value, maxItems);
                }
            }
        }

        private async Task<string> SafeGetAsync(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SYSTEM LOG] AI ĐANG GỌI API: {url}");
                Console.ResetColor();
                if (response.IsSuccessStatusCode) 
                {
                    string raw = await response.Content.ReadAsStringAsync();
                    return SmartRefineJson(url, raw);
                }
                return $"Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";
            }
            catch (Polly.CircuitBreaker.BrokenCircuitException)
            {
                return "Lỗi ngắt mạch (Circuit Breaker): Hệ thống EMarket đang quá tải hoặc tạm thời không khả dụng, vui lòng thử lại sau.";
            }
            catch (Polly.Timeout.TimeoutRejectedException)
            {
                return "Lỗi Timeout: Hệ thống EMarket phản hồi quá chậm, vui lòng thử lại sau.";
            }
            catch (Exception ex) { return $"Failed to execute API call: {ex.Message}"; }
        }

        private async Task<string> SafePostAsync<T>(string url, T data)
        {
            try
            {
                var response = await _client.PostAsJsonAsync(url, data);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SYSTEM LOG] AI ĐANG GỌI API: {url}");
                Console.ResetColor();
                if (response.IsSuccessStatusCode) 
                {
                    string raw = await response.Content.ReadAsStringAsync();
                    return SmartRefineJson(url, raw);
                }
                return $"Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";
            }
            catch (Polly.CircuitBreaker.BrokenCircuitException)
            {
                return "Lỗi ngắt mạch (Circuit Breaker): Hệ thống EMarket đang quá tải hoặc tạm thời không khả dụng, vui lòng thử lại sau.";
            }
            catch (Polly.Timeout.TimeoutRejectedException)
            {
                return "Lỗi Timeout: Hệ thống EMarket phản hồi quá chậm, vui lòng thử lại sau.";
            }
            catch (Exception ex) { return $"Failed to execute API call: {ex.Message}"; }
        }

        private string BuildQs(params (string Key, object Value)[] args)
        {
            var qs = new List<string>();
            foreach (var arg in args)
            {
                if (arg.Value is int i && i != 0) qs.Add($"{arg.Key}={i}");
                else if (arg.Value is string s && !string.IsNullOrEmpty(s)) qs.Add($"{arg.Key}={Uri.EscapeDataString(s)}");
                else if (arg.Value is double d && d != 0) qs.Add($"{arg.Key}={d}");
                else if (arg.Value is long l && l != 0) qs.Add($"{arg.Key}={l}");
            }
            return qs.Count > 0 ? "?" + string.Join("&", qs) : "";
        }

        // ==========================================
        // 1. AI Analysis (api/admin/ai-analysis)
        // ==========================================
        [KernelFunction]
        [Description("Calls AI-driven features like inventory forecast, sales forecast, product insights, anomalies, and replenishment advice. Use this tool exclusively for any AI prediction or analysis queries. RETURNS detailed JSON metrics.")]
        public Task<string> GetAIAnalysis(
            [Description("The specific analysis action to run. VALID VALUES: 'replenishment-advice', 'recommendations', 'anomalies', 'insights', 'inventory-forecast', 'sales-forecast', 'top-predicted', 'deadstock', 'product-history', 'lot-financial-risk'")] string action,
            [Description("ID of the branch. Default is 1 if unspecified.")] int branchId = 1,
            [Description("ID of the product. Required ONLY for 'sales-forecast' and 'product-history'.")] int productId = 0,
            [Description("Number of top results to return. Required ONLY for 'top-predicted'.")] int topCount = 10,
            [Description("Start date filter in 'YYYY-MM-DD' format. Used for 'product-history'.")] string start = "",
            [Description("End date filter in 'YYYY-MM-DD' format. Used for 'product-history'.")] string end = "")
        {
            return action switch
            {
                "replenishment-advice" => SafeGetAsync($"api/admin/ai-analysis/replenishment-advice/{branchId}"),
                "recommendations" => SafeGetAsync($"api/admin/ai-analysis/recommendations/{branchId}"),
                "anomalies" => SafeGetAsync($"api/admin/ai-analysis/anomalies/{branchId}"),
                "insights" => SafeGetAsync($"api/admin/ai-analysis/insights/{branchId}"),
                "inventory-forecast" => SafeGetAsync($"api/admin/ai-analysis/inventory-forecast/{branchId}"),
                "sales-forecast" => SafeGetAsync($"api/admin/ai-analysis/sales-forecast/{productId}/{branchId}"),
                "top-predicted" => SafeGetAsync($"api/admin/ai-analysis/top-predicted/{branchId}{BuildQs(("topCount", topCount))}"),
                "deadstock" => SafeGetAsync($"api/admin/ai-analysis/deadstock/{branchId}"),
                "product-history" => SafeGetAsync($"api/admin/ai-analysis/product-history/{productId}/{branchId}{BuildQs(("start", start), ("end", end))}"),
                "lot-financial-risk" => SafeGetAsync($"api/admin/ai-analysis/lot-financial-risk/{branchId}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'. Please refer to the valid values in the action parameter description.")
            };
        }

        [KernelFunction]
        [Description("Triggers the Prophet AI engine manually to train/run predictive models globally.")]
        public Task<string> PostAIAnalysis([Description("Always pass 'run-prophet' as the action string.")] string action)
        {
            if (action == "run-prophet") return SafePostAsync("api/admin/ai-analysis/run-prophet", new { });
            return Task.FromResult("Error: Invalid action");
        }

        // ==========================================
        // 2. Customer Management (api/admin/customer)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves specific customer data. DO NOT list all customers. Only use 'search', 'by-id', 'stats', 'top'.")]
        public Task<string> GetCustomerData(
            [Description("The specific customer query. VALID VALUES: 'list', 'search', 'by-id', 'email', 'stats', 'count-in-month', 'segments', 'created-by-month', 'top', 'address-by-customer', 'default-address', 'address-by-id'")] string action,
            [Description("Customer or Address ID. Required when fetching by ID or fetching addresses.")] int id = 0,
            [Description("Search query string for finding customers by name, phone, or email.")] string keyword = "",
            [Description("Filter by start date ('YYYY-MM-DD').")] string fromDate = "",
            [Description("Filter by end date ('YYYY-MM-DD').")] string toDate = "",
            [Description("Number of customers to return. Required for 'top' action.")] int top = 10)
        {
            return action switch
            {
                "list" => SafeGetAsync("api/admin/customer"),
                "search" => SafeGetAsync($"api/admin/customer/search{BuildQs(("keyword", keyword))}"),
                "by-id" => SafeGetAsync($"api/admin/customer/{id}"),
                "email" => SafeGetAsync($"api/admin/customer/{id}/email"),
                "stats" => SafeGetAsync($"api/admin/customer/stats{BuildQs(("fromDate", fromDate))}"),
                "count-in-month" => SafeGetAsync($"api/admin/customer/count-in-month{BuildQs(("fromDate", fromDate), ("toDate", toDate))}"),
                "segments" => SafeGetAsync("api/admin/customer/segments"),
                "created-by-month" => SafeGetAsync("api/admin/customer/created-by-month"),
                "top" => SafeGetAsync($"api/admin/customer/top{BuildQs(("top", top))}"),
                "address-by-customer" => SafeGetAsync($"api/admin/customer/address/by-customer/{id}"),
                "default-address" => SafeGetAsync($"api/admin/customer/address/default/{id}"),
                "address-by-id" => SafeGetAsync($"api/admin/customer/address/{id}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Retrieves customer loyalty programs and membership tier records.")]
        public Task<string> GetLoyaltyData(
            [Description("VALID VALUES: 'list', 'by-id'")] string action, 
            [Description("Loyalty Tier ID. Required for 'by-id'.")] int id = 0)
        {
            return action switch
            {
                "list" => SafeGetAsync("api/admin/customer/loyalty"),
                "by-id" => SafeGetAsync($"api/admin/customer/loyalty/{id}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        // ==========================================
        // 3. Dashboard (api/admin/dashboard)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves high-level dashboard metrics, charts, and summaries. Use this tool heavily for generating insights instead of fetching raw lists.")]
        public Task<string> GetDashboardData(
            [Description("The specific dashboard report. VALID VALUES: 'summary', 'cards', 'branch-performance', 'stock-chart', 'overview', 'people', 'warehouse', 'finance', 'debt', 'super-admin'")] string action,
            [Description("Filter metrics by a specific Branch ID. Defaults to 1.")] int branchId = 1,
            [Description("Filter metrics from this date ('YYYY-MM-DD').")] string fromDate = "",
            [Description("Filter metrics up to this date ('YYYY-MM-DD').")] string toDate = "",
            [Description("Grouping method. Often 'day' or 'month'.")] string groupBy = "day",
            [Description("Number of days backward to evaluate. Default is 30.")] int dayBacks = 30,
            [Description("Filter by a specific Warehouse ID.")] int warehouseId = 0,
            [Description("Filter by a specific Supplier ID.")] int supplierId = 0)
        {
            return action switch
            {
                "summary" => SafeGetAsync($"api/admin/dashboard/summary{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate), ("groupBy", groupBy))}"),
                "cards" or "kpi-summary" => SafeGetAsync($"api/admin/dashboard/cards{BuildQs(("branchId", branchId))}"),
                "branch-performance" => SafeGetAsync($"api/admin/dashboard/branch-performance{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate))}"),
                "stock-chart" => SafeGetAsync($"api/admin/dashboard/stock-chart{BuildQs(("branchId", branchId))}"),
                "overview" => SafeGetAsync($"api/admin/dashboard/overview{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate), ("groupBy", groupBy))}"),
                "people" => SafeGetAsync("api/admin/dashboard/people"),
                "warehouse" => SafeGetAsync($"api/admin/dashboard/warehouse{BuildQs(("dayBacks", dayBacks), ("branchId", branchId), ("warehouseId", warehouseId))}"),
                "finance" => SafeGetAsync($"api/admin/dashboard/finance{BuildQs(("daysBack", dayBacks), ("branchId", branchId))}"),
                "debt" => SafeGetAsync($"api/admin/dashboard/debt{BuildQs(("branchId", branchId), ("supplierId", supplierId), ("fromDate", fromDate), ("toDate", toDate))}"),
                "super-admin" => SafeGetAsync("api/admin/dashboard/super-admin"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        // ==========================================
        // 4. Expense Management (api/admin/expense)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves financial expenses. DO NOT fetch full lists. Use 'summary', 'detail', 'categories-all', or heavily filter lists.")]
        public Task<string> GetExpenseData(
            [Description("The specific expense query. VALID VALUES: 'list', 'paged', 'summary', 'detail', 'categories-all', 'categories-active', 'category-by-id'")] string action,
            [Description("ID of the specific expense record or category.")] int id = 0,
            [Description("Filter expenses by branch ID.")] int branchId = 1,
            [Description("Filter expenses by Category ID.")] int categoryId = 0,
            [Description("Filter expenses from this date.")] string fromDate = "",
            [Description("Filter expenses up to this date.")] string toDate = "",
            [Description("Filter expenses by status string (e.g., 'Approved', 'Pending', 'Rejected').")] string status = "")
        {
            return action switch
            {
                "list" => SafeGetAsync($"api/admin/expense/list{BuildQs(("branchId", branchId), ("categoryId", categoryId), ("fromDate", fromDate), ("toDate", toDate), ("status", status))}"),
                "paged" => SafeGetAsync($"api/admin/expense/paged{BuildQs(("branchId", branchId), ("categoryId", categoryId), ("fromDate", fromDate), ("toDate", toDate), ("status", status))}"),
                "summary" => SafeGetAsync($"api/admin/expense/summary{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate))}"),
                "detail" => SafeGetAsync($"api/admin/expense/{id}"),
                "categories-all" => SafeGetAsync("api/admin/expense/categories"),
                "categories-active" => SafeGetAsync("api/admin/expense/categories/active"),
                "category-by-id" => SafeGetAsync($"api/admin/expense/categories/{id}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        // ==========================================
        // 5. Inventory & Warehouse (api/admin/inventory)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves stock levels and movements. DO NOT fetch 'all' stock. Always filter by Product ID, Warehouse ID, Branch ID, or use 'total-actual'.")]
        public Task<string> GetStockData(
            [Description("The inventory action. VALID VALUES: 'all', 'filter', 'by-branch', 'by-id', 'movements', 'total-actual'")] string action,
            [Description("Specific Stock Item ID.")] int id = 0,
            [Description("Filter stock by Branch ID.")] int branchId = 1,
            [Description("Filter stock by Product ID.")] int productId = 0,
            [Description("Filter stock by Warehouse ID.")] int warehouseId = 0,
            [Description("Pagination start index.")] int start = 0,
            [Description("Pagination length.")] int length = 10,
            [Description("Type of stock movement to filter by (e.g., 'IN', 'OUT').")] string type = "",
            [Description("Filter from date.")] string fromDate = "",
            [Description("Filter to date.")] string toDate = "",
            [Description("Search query keyword.")] string keyword = "")
        {
            return action switch
            {
                "all" => SafeGetAsync("api/admin/inventory/items"),
                "filter" => SafeGetAsync($"api/admin/inventory/items{BuildQs(("productId", productId), ("warehouseId", warehouseId), ("branchId", branchId))}"),
                "by-branch" => SafeGetAsync($"api/admin/inventory/items{BuildQs(("branchId", branchId))}"),
                "by-id" => SafeGetAsync($"api/admin/inventory/items/{id}"),
                "movements" => SafeGetAsync($"api/admin/inventory/stock/movements{BuildQs(("start", start), ("length", length), ("warehouseId", warehouseId), ("type", type), ("fromDate", fromDate), ("toDate", toDate), ("keyword", keyword))}"),
                "total-actual" => SafeGetAsync($"api/admin/inventory/stock/total-actual{BuildQs(("productId", productId), ("warehouseId", warehouseId))}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Retrieves Purchase Orders. Avoid 'all'. Use 'search', 'by-id', or specific filters.")]
        public Task<string> GetPurchaseData(
            [Description("Purchase query action. VALID VALUES: 'all', 'search', 'by-id', 'by-branch'")] string action,
            [Description("Purchase Order ID.")] int id = 0,
            [Description("Search keyword for tracking codes or suppliers.")] string keyword = "",
            [Description("Filter by Supplier ID.")] int supplierId = 0,
            [Description("Filter by Branch ID.")] int branchId = 1,
            [Description("Filter by Warehouse ID.")] int warehouseId = 0,
            [Description("Order completion status.")] string status = "",
            [Description("Payment settlement status.")] string paymentStatus = "",
            [Description("Filter from date.")] string fromDate = "",
            [Description("Filter to date.")] string toDate = "")
        {
            return action switch
            {
                "all" => SafeGetAsync("api/admin/inventory/purchase/all"),
                "search" => SafeGetAsync($"api/admin/inventory/purchase/search{BuildQs(("keyword", keyword), ("supplierId", supplierId), ("branchId", branchId), ("warehouseId", warehouseId), ("status", status), ("paymentStatus", paymentStatus), ("fromDate", fromDate), ("toDate", toDate))}"),
                "by-id" => SafeGetAsync($"api/admin/inventory/purchase/{id}"),
                "by-branch" => SafeGetAsync($"api/admin/inventory/purchase/by-branch{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate))}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Retrieves Debt records. Prefer 'summary', 'overdue', 'near-due', or 'by-id'. Avoid generic 'all'.")]
        public Task<string> GetDebtData(
            [Description("Debt query action. VALID VALUES: 'all', 'list', 'summary', 'by-id', 'by-purchase', 'overdue', 'near-due', 'payments-by-debt'")] string action,
            [Description("Debt ID.")] int id = 0,
            [Description("Search keyword.")] string keyword = "",
            [Description("Filter by Supplier ID.")] int supplierId = 0,
            [Description("Debt settlement status.")] string status = "",
            [Description("Filter from date.")] string fromDate = "",
            [Description("Filter to date.")] string toDate = "",
            [Description("Look up debt by Purchase Order ID.")] int purchaseOrderId = 0,
            [Description("Number of days before debt is overdue. Required for 'near-due'.")] int days = 7)
        {
            return action switch
            {
                "all" => SafeGetAsync("api/admin/inventory/debt/all"),
                "list" => SafeGetAsync($"api/admin/inventory/debt/list{BuildQs(("keyword", keyword), ("supplierId", supplierId), ("status", status), ("fromDate", fromDate), ("toDate", toDate))}"),
                "summary" => SafeGetAsync($"api/admin/inventory/debt/summary{BuildQs(("keyword", keyword), ("supplierId", supplierId), ("status", status), ("fromDate", fromDate), ("toDate", toDate))}"),
                "by-id" => SafeGetAsync($"api/admin/inventory/debt/{id}"),
                "by-purchase" => SafeGetAsync($"api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}"),
                "overdue" => SafeGetAsync("api/admin/inventory/debt/overdue"),
                "near-due" => SafeGetAsync($"api/admin/inventory/debt/near-due{BuildQs(("days", days))}"),
                "payments-by-debt" => SafeGetAsync($"api/admin/inventory/debt/{id}/payments"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Retrieves details regarding physical Warehouses. Use this tool when the user queries 'kho' (warehouse) information, locations, and dictionary references.")]
        public Task<string> GetWarehouseData(
            [Description("Warehouse query action. VALID VALUES: 'all', 'by-branch', 'search', 'detail', 'dict'")] string action,
            [Description("Warehouse ID.")] int id = 0,
            [Description("Filter by Branch ID.")] int branchId = 1,
            [Description("Search by warehouse name.")] string name = "")
        {
            return action switch
            {
                "all" => SafeGetAsync($"api/admin/inventory/warehouses/all{BuildQs(("name", name), ("branchId", branchId))}"),
                "by-branch" => SafeGetAsync($"api/admin/inventory/warehouses/by-branch/{branchId}"),
                "search" => SafeGetAsync($"api/admin/inventory/warehouses/all{BuildQs(("name", name), ("branchId", branchId))}"),
                "detail" => SafeGetAsync($"api/admin/inventory/warehouses/{id}"),
                "dict" => SafeGetAsync("api/admin/inventory/warehouses/all"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        // ==========================================
        // 6. Log & Audit (api/admin/logs)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves system logs and statistics. DO NOT list large logs. Prefer 'statistics' or 'system-events'.")]
        public Task<string> GetLogData(
            [Description("The fundamental log type. VALID VALUES: 'audit', 'app', 'statistics', 'system-events'")] string logType,
            [Description("Whether to list logs or view detail. VALID VALUES: 'list', 'detail'")] string action = "list",
            [Description("Log Event ID for fetching details.")] long id = 0,
            [Description("Pagination start offset.")] int start = 0,
            [Description("Pagination batch length.")] int length = 10,
            [Description("Target database table for audit search.")] string tableName = "",
            [Description("Action type in audit log (e.g., 'INSERT', 'UPDATE', 'DELETE').")] string logAction = "",
            [Description("Keyword to search through the logs.")] string search = "",
            [Description("Filter from date.")] string fromDate = "",
            [Description("Filter to date.")] string toDate = "",
            [Description("Filter by Log Level (e.g., Error, Info, Warning). Valid for app logs.")] string logLevel = "",
            [Description("Filter by Logger namespace. Valid for app logs.")] string logger = "")
        {
            return logType switch
            {
                "audit" => action == "detail" ? SafeGetAsync($"api/admin/logs/audit/{id}") : 
                           SafeGetAsync($"api/admin/logs/audit{BuildQs(("start", start), ("length", length), ("tableName", tableName), ("action", logAction), ("search", search), ("fromDate", fromDate), ("toDate", toDate))}"),
                "app" => action == "detail" ? SafeGetAsync($"api/admin/logs/app/{id}") : 
                         SafeGetAsync($"api/admin/logs/app{BuildQs(("start", start), ("length", length), ("logLevel", logLevel), ("logger", logger), ("search", search), ("fromDate", fromDate), ("toDate", toDate))}"),
                "statistics" or "stats" => SafeGetAsync("api/admin/logs/statistics"),
                "system-events" => SafeGetAsync("api/admin/logs/system-events"),
                _ => Task.FromResult($"Error: Invalid logType '{logType}'.")
            };
        }

        // ==========================================
        // 7. Product Management (api/admin/product)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves product information. DO NOT list all. Use 'search', 'by-id', 'low-stock-alerts'.")]
        public Task<string> GetProductData(
            [Description("The product operation to perform. VALID VALUES: 'list', 'search', 'search-simple', 'by-id', 'inactive', 'low-stock-alerts', 'images', 'image-by-id'")] string action,
            [Description("Product ID.")] int id = 0,
            [Description("Product search keyword.")] string keyword = "",
            [Description("Filter products by Category ID.")] int categoryId = 0,
            [Description("Filter products by Branch ID.")] int branchId = 1,
            [Description("Filter products supplied by Supplier ID.")] int supplierId = 0,
            [Description("Filter products physically in Warehouse ID.")] int warehouseId = 0,
            [Description("Limit number of items for low-stock alerts.")] int top = 10)
        {
            return action switch
            {
                "list" => SafeGetAsync("api/admin/product/products"),
                "search" => SafeGetAsync($"api/admin/product/products/search{BuildQs(("keyword", keyword), ("categoryId", categoryId), ("branchId", branchId), ("supplierId", supplierId), ("warehouseId", warehouseId))}"),
                "search-simple" => SafeGetAsync($"api/admin/product/products/search-simple{BuildQs(("keyword", keyword), ("branchId", branchId))}"),
                "by-id" => SafeGetAsync($"api/admin/product/products/{id}"),
                "inactive" => SafeGetAsync($"api/admin/product/products/inactive{BuildQs(("keyword", keyword), ("categoryId", categoryId), ("branchId", branchId), ("supplierId", supplierId), ("warehouseId", warehouseId))}"),
                "low-stock-alerts" => SafeGetAsync($"api/admin/product/products/low-stock-alerts{BuildQs(("top", top))}"),
                "images" => SafeGetAsync($"api/admin/product/products/{id}/images"),
                "image-by-id" => SafeGetAsync($"api/admin/product/products/images/{id}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Fetches detailed objects or names for multiple products given an array of Product IDs.")]
        public Task<string> PostProductData([Description("VALID VALUES: 'by-ids', 'names-by-ids'")] string action, [Description("Valid JSON array of Product IDs. Example: [1, 2, 3]")] string jsonPayload)
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize<object>(jsonPayload);
            return action switch
            {
                "by-ids" => SafePostAsync("api/admin/product/products/by-ids", payload),
                "names-by-ids" => SafePostAsync("api/admin/product/products/names-by-ids", payload),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Retrieves product Lots. DO NOT fetch 'all'. Use 'by-product', 'lot-detail'.")]
        public Task<string> GetLotData(
            [Description("Lot tracking query action. VALID VALUES: 'all', 'by-product', 'lot-detail', 'lot-ids-by-product', 'find-existing'")] string action,
            [Description("Lot ID.")] int lotId = 0,
            [Description("Parent Product ID.")] int productId = 0,
            [Description("Manufacturing date lookup ('YYYY-MM-DD').")] string manufacturingDate = "",
            [Description("Expiry date lookup ('YYYY-MM-DD').")] string expiryDate = "")
        {
            return action switch
            {
                "all" => SafeGetAsync("api/admin/product/lots"),
                "by-product" => SafeGetAsync($"api/admin/product/products/{productId}/lots"),
                "lot-detail" => SafeGetAsync($"api/admin/product/lots/{lotId}"),
                "lot-ids-by-product" => SafeGetAsync($"api/admin/product/lots/by-product/{productId}/lot-ids"),
                "find-existing" => SafeGetAsync($"api/admin/product/lots/find-existing{BuildQs(("productId", productId), ("manufacturingDate", manufacturingDate), ("expiryDate", expiryDate))}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Fetches comprehensive details for lots given an array of Lot IDs.")]
        public Task<string> PostLotData([Description("VALID VALUES: 'by-ids', 'details-by-ids'")] string action, [Description("Valid JSON array of Lot IDs. Example: [1, 2, 3]")] string jsonPayload)
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize<object>(jsonPayload);
            return action switch
            {
                "by-ids" => SafePostAsync("api/admin/product/lots/by-ids", payload),
                "details-by-ids" => SafePostAsync("api/admin/product/lots/details-by-ids", payload),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Retrieves Product Categories and external Suppliers. Use 'search' or 'by-id'.")]
        public Task<string> GetCategorySupplierData(
            [Description("The target taxonomy. VALID VALUES: 'category', 'supplier'")] string type, 
            [Description("Action to perform. VALID VALUES: 'list', 'by-id', 'search'")] string action, 
            [Description("ID of category or supplier.")] int id = 0, 
            [Description("Search by name.")] string name = "")
        {
            string basePath = type == "category" ? "api/admin/product/categories" : "api/admin/product/suppliers";
            return action switch
            {
                "list" => SafeGetAsync(basePath),
                "by-id" => SafeGetAsync($"{basePath}/{id}"),
                "search" => SafeGetAsync($"{basePath}/search{BuildQs(("name", name))}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Fetches categories or suppliers in bulk via an array of IDs.")]
        public Task<string> PostCategorySupplierData([Description("VALID VALUES: 'category', 'supplier'")] string type, [Description("VALID VALUES: 'by-ids'")] string action, [Description("Valid JSON array of IDs. Example: [1, 2, 3]")] string jsonPayload)
        {
            if (action == "by-ids")
            {
                string basePath = type == "category" ? "api/admin/product/categories" : "api/admin/product/suppliers";
                var payload = System.Text.Json.JsonSerializer.Deserialize<object>(jsonPayload);
                return SafePostAsync($"{basePath}/by-ids", payload);
            }
            return Task.FromResult($"Error: Invalid action '{action}'.");
        }

        // ==========================================
        // 8. Quotation (api/admin/quotation)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves business quotations, price estimates, and sales quotes sent to customers.")]
        public Task<string> GetQuotationData(
            [Description("Quotation query action. VALID VALUES: 'list', 'paged', 'detail'")] string action, 
            [Description("Quotation ID.")] int id = 0, 
            [Description("Keyword search for client name or quote reference.")] string keyword = "", 
            [Description("Filter by Branch ID.")] int branchId = 1, 
            [Description("Status of the quotation (e.g., Pending, Accepted).")] string status = "", 
            [Description("From date.")] string fromDate = "", 
            [Description("To date.")] string toDate = "")
        {
            return action switch
            {
                "list" => SafeGetAsync($"api/admin/quotation/list{BuildQs(("keyword", keyword), ("branchId", branchId), ("status", status), ("fromDate", fromDate), ("toDate", toDate))}"),
                "paged" => SafeGetAsync($"api/admin/quotation/paged{BuildQs(("keyword", keyword), ("branchId", branchId), ("status", status), ("fromDate", fromDate), ("toDate", toDate))}"),
                "detail" => SafeGetAsync($"api/admin/quotation/{id}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        // ==========================================
        // 9. Sales Management (api/admin/sales)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves sales orders. DO NOT use 'all' or 'list' for general queries. Use 'detail' for specific IDs or 'by-branch' with strict dates.")]
        public Task<string> GetOrderData(
            [Description("The sales order query. VALID VALUES: 'all', 'list', 'detail', 'full-by-branch', 'by-branch', 'order-details', 'payments-by-order'")] string action,
            [Description("Sales Order ID.")] int id = 0,
            [Description("Pagination start offset.")] int start = 0,
            [Description("Pagination item limit.")] int length = 10,
            [Description("Filter orders belonging to User/Customer ID.")] int userId = 0,
            [Description("Filter orders from Branch ID.")] int branchId = 1,
            [Description("Order lifecycle status string.")] string status = "",
            [Description("From date filter.")] string fromDate = "",
            [Description("To date filter.")] string toDate = "",
            [Description("Search by Order Code or Customer Name.")] string keyword = "")
        {
            return action switch
            {
                "all" => SafeGetAsync("api/admin/sales/orders/all"),
                "list" => SafeGetAsync($"api/admin/sales/orders{BuildQs(("start", start), ("length", length), ("userId", userId), ("branchId", branchId), ("status", status), ("fromDate", fromDate), ("toDate", toDate), ("keyword", keyword))}"),
                "detail" => SafeGetAsync($"api/admin/sales/orders/{id}"),
                "full-by-branch" => SafeGetAsync($"api/admin/sales/orders/full-by-branch{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate))}"),
                "by-branch" => SafeGetAsync($"api/admin/sales/orders/by-branch{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate))}"),
                "order-details" => SafeGetAsync($"api/admin/sales/orders/{id}/details"),
                "payments-by-order" => SafeGetAsync($"api/admin/sales/orders/{id}/payments"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Retrieves active discount promotions, marketing campaigns, and voucher details.")]
        public Task<string> GetPromotionData(
            [Description("Promotion query. VALID VALUES: 'all', 'search', 'by-id', 'active'")] string action,
            [Description("Promotion ID.")] int id = 0,
            [Description("Keyword search.")] string keyword = "",
            [Description("Filter by applicable Category ID.")] int categoryId = 0,
            [Description("Discount Type (e.g., 'Percentage', 'Fixed').")] string discountType = "",
            [Description("Applicable Customer Type string.")] string cusType = "",
            [Description("From date.")] string fromDate = "",
            [Description("To date.")] string toDate = "")
        {
            return action switch
            {
                "all" => SafeGetAsync("api/admin/sales/promotions"),
                "search" => SafeGetAsync($"api/admin/sales/promotions/search{BuildQs(("keyword", keyword), ("categoryId", categoryId), ("discountType", discountType), ("cusType", cusType), ("fromDate", fromDate), ("toDate", toDate))}"),
                "by-id" => SafeGetAsync($"api/admin/sales/promotions/{id}"),
                "active" => SafeGetAsync("api/admin/sales/promotions/active"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        // ==========================================
        // 10. System Configuration (api/admin/system-config)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves system-wide configurations including VAT, Bank Accounts, Loyalty Point exchange rates, Telegram, and Email settings.")]
        public Task<string> GetSystemConfigData(
            [Description("Configuration query action. VALID VALUES: 'overview', 'all', 'value'")] string action,
            [Description("Configuration Key name (required for 'value' action, e.g., 'EMarketVAT', 'VIPDiscount').")] string key = "")
        {
            return action switch
            {
                "overview" => SafeGetAsync("api/admin/system-config"),
                "all" => SafeGetAsync("api/admin/system-config/all"),
                "value" => SafeGetAsync($"api/admin/system-config/value/{key}"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        // ==========================================
        // 11. User Management (api/admin/user-management)
        // ==========================================
        [KernelFunction]
        [Description("Retrieves users. Avoid 'list'. Use 'search', 'by-id', or 'stats'.")]
        public Task<string> GetUserData(
            [Description("User query action. VALID VALUES: 'list', 'search', 'by-id', 'dict', 'stats', 'count-new', 'recent-avatars', 'warehouse-managers-emails'")] string action,
            [Description("System User ID.")] int id = 0,
            [Description("Search user by Name, Email, or Phone.")] string keyword = "",
            [Description("Filter creation from date.")] string fromDate = "",
            [Description("Top limit for recent queries.")] int top = 5)
        {
            return action switch
            {
                "list" => SafeGetAsync("api/admin/user-management/users"),
                "search" => SafeGetAsync($"api/admin/user-management/users/search{BuildQs(("keyword", keyword))}"),
                "by-id" => SafeGetAsync($"api/admin/user-management/users/{id}"),
                "dict" => SafeGetAsync("api/admin/user-management/users/dict"),
                "stats" => SafeGetAsync("api/admin/user-management/users/stats"),
                "count-new" => SafeGetAsync($"api/admin/user-management/users/count-new{BuildQs(("fromDate", fromDate))}"),
                "recent-avatars" => SafeGetAsync($"api/admin/user-management/users/recent-avatars{BuildQs(("top", top))}"),
                "warehouse-managers-emails" => SafeGetAsync("api/admin/user-management/users/warehouse-managers-emails"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Fetches detailed user objects via an array of User IDs.")]
        public Task<string> PostUserData([Description("VALID VALUES: 'by-ids'")] string action, [Description("JSON array containing User IDs. Example: [1, 2, 3]")] string jsonPayload)
        {
            if (action == "by-ids")
            {
                var payload = System.Text.Json.JsonSerializer.Deserialize<object>(jsonPayload);
                return SafePostAsync("api/admin/user-management/users/by-ids", payload);
            }
            return Task.FromResult($"Error: Invalid action '{action}'.");
        }

        [KernelFunction]
        [Description("Retrieves organizational structures, branches (Chi nhánh), roles, and permission levels. Use this tool exclusively to get Branch info, Role policies, or system privileges!")]
        public Task<string> GetBranchRolePermissionData(
            [Description("The organizational dimension. MUST BE EXACTLY ONE OF: 'branch', 'role', 'permission'")] string type,
            [Description("The query operation. VALID VALUES: 'list', 'search', 'nearest', 'by-id', 'dict', 'role-permissions'")] string action,
            [Description("ID of the Branch, Role, or Permission.")] int id = 0,
            [Description("Name to search for.")] string name = "",
            [Description("Latitude coordinate for spatial search.")] double lat = 0,
            [Description("Longitude coordinate for spatial search.")] double lng = 0,
            [Description("Maximum radius distance for branch geolocation.")] double maxDist = 10)
        {
            string basePath = type switch
            {
                "branch" => "api/admin/user-management/branches",
                "role" => "api/admin/user-management/roles",
                "permission" => "api/admin/user-management/permissions",
                _ => ""
            };

            if (string.IsNullOrEmpty(basePath)) return Task.FromResult($"Error: Invalid type '{type}'. Must be 'branch', 'role', or 'permission'.");

            return action switch
            {
                "list" => SafeGetAsync(basePath),
                "by-id" => SafeGetAsync($"{basePath}/{id}"),
                "search" => SafeGetAsync($"{basePath}/search{BuildQs(("name", name), ("lat", lat), ("lng", lng), ("maxDist", maxDist))}"),
                "nearest" => SafeGetAsync($"{basePath}/nearest{BuildQs(("lat", lat), ("lng", lng), ("maxDist", maxDist))}"),
                "dict" => SafeGetAsync($"{basePath}/dict"),
                "role-permissions" => SafeGetAsync($"{basePath}/{id}/permissions"),
                _ => Task.FromResult($"Error: Invalid action '{action}'.")
            };
        }

        [KernelFunction]
        [Description("Fetches detailed Branch configurations given a JSON payload array of Branch IDs.")]
        public Task<string> PostBranchData([Description("VALID VALUES: 'by-ids'")] string action, [Description("JSON array of Branch IDs. Example: [1, 2, 3]")] string jsonPayload)
        {
            if (action == "by-ids")
            {
                var payload = System.Text.Json.JsonSerializer.Deserialize<object>(jsonPayload);
                return SafePostAsync("api/admin/user-management/branches/by-ids", payload);
            }
            return Task.FromResult($"Error: Invalid action '{action}'.");
        }

        // ==========================================
        // 12. Composite Aggregated Reports
        // ==========================================
        [KernelFunction]
        [Description("Retrieves a comprehensive business report for a specific branch by its name. Use this tool when the user asks about the overall situation, sales, or business of a specific branch like 'Bến Cát'. It automatically finds the branch ID and aggregates dashboard overview, performance, and AI insights into a single dataset.")]
        public async Task<string> GetComprehensiveBranchReport(
            [Description("The name of the branch to search for (e.g., 'Bến Cát').")] string branchName,
            [Description("Filter from date ('YYYY-MM-DD'). Optional.")] string fromDate = "",
            [Description("Filter to date ('YYYY-MM-DD'). Optional.")] string toDate = "")
        {
            try
            {
                var branchDictUrl = "api/admin/user-management/branches/dict";
                var branchDictRaw = await SafeGetAsync(branchDictUrl);
                
                int branchId = 0;
                string branchFullName = branchName;

                try
                {
                    var node = JsonNode.Parse(branchDictRaw);
                    var jsonObject = node as JsonObject;

                    if (jsonObject != null && jsonObject.Count > 0)
                    {
                        var branches = jsonObject.Select(kvp => kvp.Value).Where(v => v != null).Cast<JsonNode>().ToList();

                        var exactMatch = branches.FirstOrDefault(b =>
                            string.Equals(b?["Name"]?.GetValue<string>(), branchName, StringComparison.OrdinalIgnoreCase));

                        var containsMatches = exactMatch == null
                            ? branches.Where(b => (b?["Name"]?.GetValue<string>() ?? "").Contains(branchName, StringComparison.OrdinalIgnoreCase)).ToList()
                            : new List<JsonNode>();

                        if (exactMatch != null)
                        {
                            branchId = exactMatch["BranchId"]?.GetValue<int>() ?? 0;
                            branchFullName = exactMatch["Name"]?.GetValue<string>() ?? branchName;
                        }
                        else if (containsMatches.Count == 1)
                        {
                            branchId = containsMatches[0]?["BranchId"]?.GetValue<int>() ?? 0;
                            branchFullName = containsMatches[0]?["Name"]?.GetValue<string>() ?? branchName;
                        }
                        else if (containsMatches.Count > 1)
                        {
                            var listNames = string.Join(", ", containsMatches.Select(x => x?["Name"]?.GetValue<string>()));
                            return $"Hệ thống tìm thấy {containsMatches.Count} chi nhánh có tên chứa '{branchName}' ({listNames}). Vui lòng cung cấp tên cụ thể hơn.";
                        }
                    }
                }
                catch { /* Parse error, branchId remains 0 */ }

                if (branchId == 0)
                {
                    return $"Không tìm thấy chi nhánh nào có tên '{branchName}'. Vui lòng kiểm tra lại tên chi nhánh.";
                }

                var overviewTask = SafeGetAsync($"api/admin/dashboard/overview{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate))}");
                var performanceTask = SafeGetAsync($"api/admin/dashboard/branch-performance{BuildQs(("branchId", branchId), ("fromDate", fromDate), ("toDate", toDate))}");
                var aiInsightsTask = SafeGetAsync($"api/admin/ai-analysis/insights/{branchId}");
                var stockChartTask = SafeGetAsync($"api/admin/dashboard/stock-chart{BuildQs(("branchId", branchId))}");

                await Task.WhenAll(overviewTask, performanceTask, aiInsightsTask, stockChartTask);

                JsonNode? SafeParse(string json) { try { return JsonNode.Parse(json); } catch { return null; } }

                var result = new JsonObject
                {
                    ["BranchId"] = branchId,
                    ["BranchName"] = branchFullName,
                    ["DashboardOverview"] = SafeParse(overviewTask.Result),
                    ["BranchPerformance"] = SafeParse(performanceTask.Result),
                    ["StockChart"] = SafeParse(stockChartTask.Result),
                    ["AiInsights"] = SafeParse(aiInsightsTask.Result)
                };

                return SmartRefineJson("aggregated-branch-report", result.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
            }
            catch (Exception ex)
            {
                return $"Lỗi khi tổng hợp dữ liệu chi nhánh: {ex.Message}";
            }
        }

        [KernelFunction]
        [Description("Retrieves a comprehensive business report for a specific product by its name or code. Use this tool when the user asks about the situation, sales, or inventory of a specific product. It automatically finds the product ID and aggregates product info, stock data, and sales forecast into a single dataset.")]
        public async Task<string> GetComprehensiveProductReport(
            [Description("The name or keyword of the product to search for.")] string productName,
            [Description("Filter by Branch ID. Default is 1.")] int branchId = 1)
        {
            try
            {
                var productSearchUrl = $"api/admin/product/products/search-simple?keyword={Uri.EscapeDataString(productName)}&branchId={branchId}";
                var productSearchRaw = await SafeGetAsync(productSearchUrl);
                
                int productId = 0;
                string productFullName = productName;

                try
                {
                    var node = JsonNode.Parse(productSearchRaw);
                    JsonArray? items = node as JsonArray ?? node?["data"] as JsonArray ?? node?["items"] as JsonArray;
                    
                    if (items != null && items.Count > 0)
                    {
                        var exactMatch = items.FirstOrDefault(i => string.Equals(i?["Name"]?.GetValue<string>(), productName, StringComparison.OrdinalIgnoreCase) || string.Equals(i?["Code"]?.GetValue<string>(), productName, StringComparison.OrdinalIgnoreCase));
                        var selectedProduct = exactMatch ?? items[0];
                        
                        if (items.Count > 1 && exactMatch == null)
                        {
                            return $"Hệ thống tìm thấy {items.Count} sản phẩm có tên hoặc mã gần giống '{productName}'. Vui lòng cung cấp tên chính xác hơn để tôi lấy dữ liệu.";
                        }

                        productId = selectedProduct?["ProductId"]?.GetValue<int>() ?? 0;
                        productFullName = selectedProduct?["Name"]?.GetValue<string>() ?? productName;
                    }
                }
                catch { }

                if (productId == 0)
                {
                    return $"Không tìm thấy sản phẩm nào có tên '{productName}' tại chi nhánh {branchId}.";
                }

                var productDetailTask = SafeGetAsync($"api/admin/product/products/{productId}");
                var stockTask = SafeGetAsync($"api/admin/inventory/items{BuildQs(("productId", productId), ("branchId", branchId))}");
                var salesForecastTask = SafeGetAsync($"api/admin/ai-analysis/sales-forecast/{productId}/{branchId}");

                await Task.WhenAll(productDetailTask, stockTask, salesForecastTask);

                JsonNode? SafeParse(string json) { try { return JsonNode.Parse(json); } catch { return null; } }

                var result = new JsonObject
                {
                    ["ProductId"] = productId,
                    ["ProductName"] = productFullName,
                    ["ProductDetail"] = SafeParse(productDetailTask.Result),
                    ["StockData"] = SafeParse(stockTask.Result),
                    ["SalesForecast"] = SafeParse(salesForecastTask.Result)
                };

                return SmartRefineJson("aggregated-product-report", result.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
            }
            catch (Exception ex)
            {
                return $"Lỗi khi tổng hợp dữ liệu sản phẩm: {ex.Message}";
            }
        }
    }
}