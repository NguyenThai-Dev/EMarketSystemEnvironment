package com.example.do_an_di_dong.data.api;

import com.example.do_an_di_dong.model.*;

import java.util.List;
import java.util.Map;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.Field;
import retrofit2.http.FormUrlEncoded;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.POST;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface ApiService {

    // =========================================================================
    // 1. NHÓM ĐĂNG NHẬP & XÁC THỰC (AUTHENTICATION)
    // =========================================================================

    @POST("api/admin/user-management/auth/login")
    Call<LoginResponse> loginRest(@Body LoginRequest request);

    @POST("api/admin/user-management/auth/google-login")
    Call<LoginResponse> googleLogin(@Body ExternalLoginRequest request);

    @POST("api/admin/user-management/auth/external-login")
    Call<LoginResponse> externalLogin(@Body ExternalLoginRequest request);

    @GET("Admin/Login/Login")
    Call<ResponseBody> getLoginPage();

    @FormUrlEncoded
    @POST("Admin/Login/Login")
    Call<LoginResponse> login(
            @Field("EmailOrUsername") String username,
            @Field("Password") String password,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @POST("Admin/Login/SignOut")
    Call<ResponseBody> signOut();

    // =========================================================================
    // 1.1 NHÓM CHI NHÁNH & KHO HÀNG (BRANCHES & WAREHOUSES)
    // =========================================================================

    @GET("Admin/Branch/GetAllBranch")
    Call<BranchListResponse> getBranchList();

    @GET("Admin/Warehouse/GetAllWarehouses")
    Call<WarehouseListResponse> getWarehouseList();

    @GET("Admin/Warehouse/GetFilteredWarehouse")
    Call<WarehouseListResponse> getFilteredWarehouseList(
            @Query("warehouseName") String warehouseName,
            @Query("branchId") Integer branchId
    );

    @GET("api/admin/inventory/warehouses/all")
    Call<List<Warehouse>> getWarehousesFiltered(
            @Query("name") String name,
            @Query("branchId") Integer branchId
    );

    @GET("api/admin/inventory/warehouses/by-branch/{branchId}")
    Call<List<Warehouse>> getWarehousesByBranchId(@Path("branchId") int branchId);

    // =========================================================================
    // 2. NHÓM BÁO CÁO TỔNG QUAN (DASHBOARD)
    // =========================================================================

    @GET("api/admin/dashboard/summary")
    Call<DashboardOverview> getDashboardSummary(
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate,
            @Query("groupBy") String groupBy
    );

    @GET("api/admin/dashboard/overview")
    Call<DashboardOverview> getOverview(
            @Query("branchId") Integer branchId,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate,
            @Query("groupBy") String groupBy
    );

    @GET("api/admin/dashboard/branch-performance")
    Call<List<BranchPerformance>> getBranchPerformance(
            @Query("branchId") Integer branchId,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/dashboard/stock-chart")
    Call<ResponseBody> getStockChart(
            @Query("branchId") Integer branchId
    );

    @GET("api/admin/dashboard/people")
    Call<ResponseBody> getPeopleDashboard();

    @GET("api/admin/dashboard/warehouse")
    Call<WarehouseDashboard> getWarehouseDashboard(
            @Query("dayBacks") int dayBacks,
            @Query("branchId") Integer branchId,
            @Query("warehouseId") Integer warehouseId
    );

    @GET("api/admin/dashboard/finance")
    Call<FinanceDashboard> getFinanceDashboard(
            @Query("daysBack") int daysBack,
            @Query("branchId") Integer branchId
    );

    @GET("api/admin/dashboard/debt")
    Call<DebtDashboard> getDebtDashboard(
            @Query("branchId") Integer branchId,
            @Query("supplierId") Integer supplierId,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/dashboard/super-admin")
    Call<ResponseBody> getSuperAdminHub();

    // =========================================================================
    // 3. NHÓM PHÂN TÍCH AI (AI ANALYSIS)
    // =========================================================================

    @GET("api/admin/ai-analysis/replenishment-advice/{branchId}")
    Call<List<ReplenishmentAdvice>> getReplenishmentAdvice(
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/recommendations/{branchId}")
    Call<List<Product>> getRecommendations(
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/anomalies/{branchId}")
    Call<List<Anomaly>> getAnomalies(
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/insights/{branchId}")
    Call<List<Insight>> getProductInsights(
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/inventory-forecast/{branchId}")
    Call<List<InventoryForecast>> getInventoryForecast(
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/sales-forecast/{productId}/{branchId}")
    Call<ResponseBody> getSalesForecast(
            @Path("productId") int productId,
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/top-predicted/{branchId}")
    Call<List<Product>> getTopPredictedProducts(
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/deadstock/{branchId}")
    Call<ResponseBody> getDeadstockAnalysis(
            @Path("branchId") int branchId
    );

    @GET("api/admin/ai-analysis/product-history/{productId}/{branchId}")
    Call<ResponseBody> getProductHistory(
            @Path("productId") int productId,
            @Path("branchId") int branchId,
            @Query("start") String start,
            @Query("end") String end
    );

    @GET("api/admin/ai-analysis/lot-financial-risk/{branchId}")
    Call<List<LotFinancialRisk>> getLotFinancialRisk(
            @Path("branchId") int branchId
    );

    @POST("api/admin/ai-analysis/run-prophet")
    Call<ResponseBody> runProphet();

    @POST("api/chat/ask")
    Call<ChatResponse> askAi(
            @Body ChatRequest request
    );

    // =========================================================================
    // 4. NHÓM NHẬT KÝ HỆ THỐNG (SYSTEM LOGS)
    // =========================================================================

    @GET("api/admin/logs/statistics")
    Call<LogStatistics> getLogStatistics();

    @GET("api/admin/logs/system-events")
    Call<List<SystemEvent>> getLatestSystemEvents();

    @GET("api/admin/logs/audit")
    Call<AuditLogResponse> getAuditLogs(
            @Query("start") int start,
            @Query("length") int length,
            @Query("tableName") String tableName,
            @Query("action") String action,
            @Query("search") String search,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/logs/app")
    Call<AppLogResponse> getAppLogs(
            @Query("start") int start,
            @Query("length") int length,
            @Query("logLevel") String logLevel,
            @Query("logger") String logger,
            @Query("search") String search,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/logs/audit/{id}")
    Call<ResponseBody> getAuditDetail(
            @Path("id") long id
    );

    @GET("api/admin/logs/app/{id}")
    Call<ResponseBody> getAppLogDetail(
            @Path("id") long id
    );

    // =========================================================================
    // 5. NHÓM QUẢN LÝ SẢN PHẨM & LÔ HÀNG (PRODUCTS & LOTS)
    // =========================================================================

    @GET("api/admin/product/products")
    Call<List<Product>> getAllProducts();

    @GET("api/admin/product/products/paged")
    Call<ProductPagedResponse> getProductsPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("keyword") String keyword,
            @Query("categoryId") Integer categoryId,
            @Query("branchId") Integer branchId,
            @Query("supplierId") Integer supplierId,
            @Query("warehouseId") Integer warehouseId
    );

    @GET("api/admin/product/products/search")
    Call<List<Product>> searchProducts(
            @Query("keyword") String keyword,
            @Query("categoryId") Integer categoryId,
            @Query("branchId") Integer branchId,
            @Query("supplierId") Integer supplierId,
            @Query("warehouseId") Integer warehouseId
    );

    @GET("api/admin/product/products/search-simple")
    Call<List<Product>> searchProductsSimple(
            @Query("keyword") String keyword,
            @Query("branchId") Integer branchId
    );

    @GET("api/admin/product/products/barcode/{barcode}")
    Call<ResponseBody> searchProductByBarcode(
            @Path("barcode") String barcode
    );

    @GET("api/admin/product/products/{id}")
    Call<Product> getProductById(
            @Path("id") int id
    );

    @GET("api/admin/product/lots/by-product/{productId}")
    Call<List<com.example.do_an_di_dong.model.ProductLot>> getProductLotsByProductId(
            @Path("productId") int productId
    );

    @GET("api/admin/product/lots")
    Call<List<com.example.do_an_di_dong.model.ProductLot>> getAllProductLots();

    @POST("api/admin/product/products/by-ids")
    Call<List<Product>> getProductsByIds(
            @Body List<Integer> ids
    );

    @POST("api/admin/product/products/names-by-ids")
    Call<Map<String, String>> getProductNamesByIds(
            @Body List<Integer> productIds
    );

    @POST("api/admin/product/products")
    Call<ResponseBody> createProductApi(
            @Body ProductDTO dto
    );

    @POST("api/admin/product/products/update")
    Call<ResponseBody> updateProductApi(
            @Body ProductDTO dto
    );

    @POST("api/admin/product/products/delete/{id}")
    Call<ResponseBody> deleteProductApi(
            @Path("id") int id
    );

    @POST("api/admin/product/products/active")
    Call<ResponseBody> activeProductApi(
            @Query("productId") int productId,
            @Query("minStock") int minStock,
            @Query("maxStock") int maxStock
    );

    @GET("api/admin/product/products/inactive")
    Call<List<Product>> getInactiveProducts(
            @Query("keyword") String keyword,
            @Query("categoryId") Integer categoryId,
            @Query("branchId") Integer branchId,
            @Query("supplierId") Integer supplierId,
            @Query("warehouseId") Integer warehouseId
    );

    @GET("api/admin/product/products/low-stock-alerts")
    Call<ResponseBody> getLowStockAlerts(
            @Query("top") int top
    );

    @POST("Admin/Product/CreateProduct")
    Call<ResponseBody> createProduct(
            @Body ProductDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Product/UpdateProduct")
    Call<ResponseBody> updateProduct(
            @Body ProductDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Product/DeleteProduct")
    Call<ResponseBody> deleteProduct(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Product/ActiveProduct")
    Call<ResponseBody> activeProduct(
            @Field("productId") int productId,
            @Field("minStock") int minStock,
            @Field("maxStock") int maxStock,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @GET("api/admin/product/categories")
    Call<List<ProductCategoryDTO>> getAllCategories();

    @GET("api/admin/product/categories/{id}")
    Call<ProductCategoryDTO> getCategoryById(
            @Path("id") int id
    );

    @POST("api/admin/product/categories")
    Call<ResponseBody> createProductCategoryApi(
            @Body ProductCategoryDTO dto
    );

    @POST("api/admin/product/categories/update")
    Call<ResponseBody> updateProductCategoryApi(
            @Body ProductCategoryDTO dto
    );

    @POST("api/admin/product/categories/delete/{id}")
    Call<ResponseBody> deleteProductCategoryApi(
            @Path("id") int id
    );

    @POST("Admin/Product/CreateProductCategory")
    Call<ResponseBody> createProductCategory(
            @Body ProductCategoryDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Product/UpdateProductCategory")
    Call<ResponseBody> updateProductCategory(
            @Body ProductCategoryDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Product/DeleteProductCategory")
    Call<ResponseBody> deleteProductCategory(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @GET("api/admin/product/suppliers")
    Call<List<SupplierDTO>> getAllSuppliers();

    @GET("api/admin/product/suppliers/{id}")
    Call<SupplierDTO> getSupplierById(
            @Path("id") int id
    );

    @POST("api/admin/product/suppliers")
    Call<ResponseBody> createSupplierApi(
            @Body SupplierDTO dto
    );

    @POST("api/admin/product/suppliers/update")
    Call<ResponseBody> updateSupplierApi(
            @Body SupplierDTO dto
    );

    @POST("api/admin/product/suppliers/delete/{id}")
    Call<ResponseBody> deleteSupplierApi(
            @Path("id") int id
    );

    @POST("Admin/Supplier/CreateSupplier")
    Call<ResponseBody> createSupplier(
            @Body SupplierDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Supplier/UpdateSupplier")
    Call<ResponseBody> updateSupplier(
            @Body SupplierDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Supplier/DeleteSupplier")
    Call<ResponseBody> deleteSupplier(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    // =========================================================================
    // 6. NHÓM TỒN KHO & PHIẾU NHẬP (INVENTORY & PURCHASES)
    // =========================================================================

    @GET("api/admin/inventory/stock/movements")
    Call<StockMovementPagedResponse> getStockMovementsPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("warehouseId") Integer warehouseId,
            @Query("type") String type,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate,
            @Query("keyword") String keyword
    );

    @GET("api/admin/inventory/stock/movements")
    Call<ResponseBody> getStockMovements(
            @Query("start") int start,
            @Query("length") int length,
            @Query("warehouseId") Integer warehouseId,
            @Query("type") String type,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate,
            @Query("keyword") String keyword
    );

    @POST("api/admin/inventory/stock/adjust")
    Call<ResponseBody> adjustStockApi(
            @Body StockAdjustmentRequest request
    );

    @POST("Admin/Warehouse/AdjustStock")
    Call<ResponseBody> adjustStock(
            @Body StockAdjustmentRequest request,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @GET("api/admin/inventory/stock/total-actual")
    Call<Integer> getTotalStock(
            @Query("productId") int productId,
            @Query("warehouseId") int warehouseId
    );

    @GET("api/admin/inventory/items")
    Call<List<InventoryItem>> getInventoryItems(
            @Query("warehouseId") Integer warehouseId,
            @Query("branchId") Integer branchId,
            @Query("productId") Integer productId
    );

    @GET("api/admin/inventory/purchase/all")
    Call<List<PurchaseOrder>> getAllPurchases();

    @GET("api/admin/inventory/purchase/search")
    Call<PurchasePagedResponse> searchPurchasesPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("keyword") String keyword,
            @Query("supplierId") Integer supplierId,
            @Query("branchId") Integer branchId,
            @Query("warehouseId") Integer warehouseId,
            @Query("status") String status,
            @Query("paymentStatus") String paymentStatus,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/inventory/purchase/search")
    Call<List<PurchaseOrder>> searchPurchases(
            @Query("keyword") String keyword,
            @Query("supplierId") Integer supplierId,
            @Query("branchId") Integer branchId,
            @Query("warehouseId") Integer warehouseId,
            @Query("status") String status,
            @Query("paymentStatus") String paymentStatus,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/inventory/purchase/{id}")
    Call<PurchaseOrder> getPurchaseById(
            @Path("id") int id
    );

    @POST("api/admin/inventory/purchase")
    Call<ResponseBody> createPurchaseRest(
            @Body PurchaseOrder dto
    );

    @POST("api/admin/inventory/purchase/update")
    Call<ResponseBody> updatePurchaseRest(
            @Body PurchaseOrder dto
    );

    @POST("api/admin/inventory/purchase/delete/{id}")
    Call<ResponseBody> deletePurchaseRest(
            @Path("id") int id
    );

    @POST("Admin/PurchaseOrder/SavePurchaseOrder")
    Call<ResponseBody> createPurchaseApi(
            @Body PurchaseOrder dto
    );

    @FormUrlEncoded
    @POST("Admin/PurchaseOrder/Delete")
    Call<ResponseBody> deletePurchaseApi(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @GET("api/admin/inventory/debt/all")
    Call<List<SupplierDebt>> getAllDebts();

    @GET("api/admin/inventory/debt/summary")
    Call<DebtSummary> getDebtSummary(
            @Query("supplierId") Integer supplierId,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/inventory/debt/list")
    Call<SupplierDebtPagedResponse> getDebtsPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("keyword") String keyword,
            @Query("supplierId") Integer supplierId,
            @Query("status") String status,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/inventory/debt/list")
    Call<List<SupplierDebt>> getFilteredDebts(
            @Query("keyword") String keyword,
            @Query("supplierId") Integer supplierId,
            @Query("status") String status,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/inventory/debt/{id}")
    Call<SupplierDebt> getDebtById(
            @Path("id") int id
    );

    @GET("api/admin/inventory/warehouses/all")
    Call<List<Warehouse>> getAllWarehouses();

    @GET("api/admin/inventory/warehouses/{id}")
    Call<Warehouse> getWarehouseDetail(
            @Path("id") int id
    );

    @POST("api/admin/inventory/warehouses")
    Call<ResponseBody> createWarehouseApi(
            @Body WarehouseDTO dto
    );

    @POST("api/admin/inventory/warehouses/update")
    Call<ResponseBody> updateWarehouseApi(
            @Body WarehouseDTO dto
    );

    @POST("api/admin/inventory/warehouses/delete/{id}")
    Call<ResponseBody> deleteWarehouseApi(
            @Path("id") int id
    );

    @POST("Admin/Warehouse/CreateWarehouse")
    Call<ResponseBody> createWarehouse(
            @Body WarehouseDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Warehouse/UpdateWarehouse")
    Call<ResponseBody> updateWarehouse(
            @Body WarehouseDTO dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Warehouse/DeleteWarehouse")
    Call<ResponseBody> deleteWarehouse(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    // =========================================================================
    // 7. NHÓM ĐƠN HÀNG & BÁN HÀNG (SALES & ORDERS)
    // =========================================================================

    @GET("api/admin/sales/orders/all")
    Call<List<Order>> getAllOrders();

    @GET("api/admin/sales/orders")
    Call<OrderPagedResponse> getOrdersPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("userId") Integer userId,
            @Query("branchId") Integer branchId,
            @Query("status") String status,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate,
            @Query("keyword") String keyword
    );

    @POST("api/admin/sales/orders/checkout")
    Call<CheckoutResult> checkoutOrder(
            @Body CheckoutRequest request
    );

    @GET("api/admin/sales/orders/{id}")
    Call<ResponseBody> getOrderDetail(
            @Path("id") int id
    );

    @GET("api/admin/sales/orders/{orderId}/details")
    Call<List<OrderDetail>> getOrderDetails(
            @Path("orderId") int orderId
    );

    @POST("api/admin/sales/orders/{orderId}/status")
    Call<ResponseBody> updateOrderStatusApi(
            @Path("orderId") int orderId,
            @Body Map<String, String> statusBody
    );

    @POST("api/admin/sales/orders/delete/{orderId}")
    Call<ResponseBody> deleteOrderApi(
            @Path("orderId") int orderId
    );

    @FormUrlEncoded
    @POST("Admin/Order/UpdateStatus")
    Call<ResponseBody> updateOrderStatus(
            @Field("orderId") int orderId,
            @Field("status") String status,
            @Field("connectionId") String connectionId,
            @Header("Request-Verification-Token") String token
    );

    @GET("api/admin/sales/orders/{orderId}/payments")
    Call<List<Payment>> getPaymentsByOrder(
            @Path("orderId") int orderId
    );

    @GET("api/admin/sales/promotions")
    Call<List<Promotion>> getAllPromotions();

    @GET("api/admin/sales/promotions/{id}")
    Call<Promotion> getPromotionById(
            @Path("id") int id
    );

    @GET("api/admin/sales/promotions/active")
    Call<List<Promotion>> getActivePromotions();

    @POST("api/admin/sales/promotions/create")
    Call<ResponseBody> createPromotionApi(
            @Body Promotion dto
    );

    @POST("api/admin/sales/promotions/update")
    Call<ResponseBody> updatePromotionApi(
            @Body Promotion dto
    );

    @POST("api/admin/sales/promotions/delete/{id}")
    Call<ResponseBody> deletePromotionApi(
            @Path("id") int id
    );

    @POST("Admin/Promotion/CreatePromotion")
    Call<ResponseBody> createPromotion(
            @Body Promotion dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Promotion/UpdatePromotion")
    Call<ResponseBody> updatePromotion(
            @Body Promotion dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Promotion/DeletePromotion")
    Call<ResponseBody> deletePromotion(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    // =========================================================================
    // 8. NHÓM BÁO GIÁ (QUOTATIONS)
    // =========================================================================

    @GET("api/admin/quotation/list")
    Call<List<Quotation>> getQuotations(
            @Query("keyword") String keyword,
            @Query("branchId") Integer branchId,
            @Query("status") String status,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/quotation/paged")
    Call<QuotationPagedResponse> getQuotationsPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("keyword") String keyword,
            @Query("branchId") Integer branchId,
            @Query("status") String status,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/quotation/{id}")
    Call<Quotation> getQuotationDetail(
            @Path("id") int id
    );

    @POST("api/admin/quotation/create")
    Call<ResponseBody> createQuotationApi(
            @Body QuotationSaveRequest request
    );

    @POST("api/admin/quotation/save")
    Call<ResponseBody> updateQuotationApi(
            @Body QuotationSaveRequest request
    );

    @POST("api/admin/quotation/delete/{id}")
    Call<ResponseBody> deleteQuotationApi(
            @Path("id") int id
    );

    @POST("Admin/Quotation/SaveQuotation")
    Call<ResponseBody> saveQuotation(
            @Body QuotationSaveRequest request,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Quotation/UpdateQuotation")
    Call<ResponseBody> updateQuotation(
            @Body QuotationSaveRequest request,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Quotation/ChangeStatus")
    Call<ResponseBody> changeQuotationStatus(
            @Field("id") int id,
            @Field("status") String status,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Quotation/DeleteQuotation")
    Call<ResponseBody> deleteQuotation(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Quotation/ConvertToOrder")
    Call<ResponseBody> convertQuotationToOrder(
            @Field("quotationId") int quotationId,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    // =========================================================================
    // 9. NHÓM CHI PHÍ (EXPENSES)
    // =========================================================================

    @GET("api/admin/expense/list")
    Call<List<Expense>> getExpenses(
            @Query("branchId") Integer branchId,
            @Query("categoryId") Integer categoryId,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate,
            @Query("status") String status
    );

    @GET("api/admin/expense/paged")
    Call<ExpensePagedResponse> getExpensesPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("branchId") Integer branchId,
            @Query("categoryId") Integer categoryId,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate,
            @Query("status") String status
    );

    @GET("api/admin/expense/summary")
    Call<ExpenseSummary> getExpenseSummary(
            @Query("branchId") Integer branchId,
            @Query("fromDate") String fromDate,
            @Query("toDate") String toDate
    );

    @GET("api/admin/expense/{id}")
    Call<Expense> getExpenseDetail(
            @Path("id") int id
    );

    @GET("api/admin/expense/categories")
    Call<List<ExpenseCategory>> getAllExpenseCategories();

    @POST("api/admin/expense/create")
    Call<ResponseBody> createExpenseApi(
            @Body Expense dto
    );

    @POST("api/admin/expense/update")
    Call<ResponseBody> updateExpenseApi(
            @Body Expense dto
    );

    @POST("api/admin/expense/delete/{id}")
    Call<ResponseBody> deleteExpenseApi(
            @Path("id") int id
    );

    @POST("Admin/Expense/CreateExpense")
    Call<ResponseBody> createExpense(
            @Body Expense dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Expense/UpdateStatus")
    Call<ResponseBody> updateExpenseStatus(
            @Field("expenseId") int expenseId,
            @Field("status") String status,
            @Field("rejectReason") String rejectReason,
            @Field("paymentMethod") String paymentMethod,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Expense/DeleteExpense")
    Call<ResponseBody> deleteExpense(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    // =========================================================================
    // 10. NHÓM KHÁCH HÀNG & CRM (CUSTOMERS & CRM)
    // =========================================================================

    @GET("api/admin/customer")
    Call<List<Customer>> getAllCustomers();

    @GET("api/admin/customer/search")
    Call<CustomerPagedResponse> getCustomersPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("keyword") String keyword
    );

    @GET("api/admin/customer/{id}")
    Call<Customer> getCustomerById(
            @Path("id") int id
    );

    @POST("api/admin/customer/create")
    Call<ResponseBody> createCustomerApi(
            @Body Customer dto
    );

    @POST("api/admin/customer/update")
    Call<ResponseBody> updateCustomerApi(
            @Body Customer dto
    );

    @POST("api/admin/customer/delete/{id}")
    Call<ResponseBody> deleteCustomerApi(
            @Path("id") int id
    );

    @POST("Admin/Customer/CreateCustomer")
    Call<ResponseBody> createCustomer(
            @Body Customer dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Customer/UpdateCustomer")
    Call<ResponseBody> updateCustomer(
            @Body Customer dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Customer/DeleteCustomer")
    Call<ResponseBody> deleteCustomer(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @GET("Admin/Loyalty/GetAllLoyalty")
    Call<List<LoyaltyProgram>> getAllLoyalty();

    // =========================================================================
    // 11. NHÓM NGƯỜI DÙNG & PHÂN QUYỀN (USERS, BRANCHES, ROLES)
    // =========================================================================

    @GET("api/admin/user-management/users")
    Call<List<User>> getAllUsers();

    @GET("api/admin/user-management/users/search")
    Call<UserPagedResponse> getUsersPaged(
            @Query("start") int start,
            @Query("length") int length,
            @Query("keyword") String keyword
    );

    @GET("api/admin/user-management/users/{id}")
    Call<User> getUserById(
            @Path("id") int id
    );

    @POST("api/admin/user-management/users/create")
    Call<ResponseBody> createUserApi(
            @Body User dto
    );

    @POST("api/admin/user-management/users/update")
    Call<ResponseBody> updateUserApi(
            @Body User dto
    );

    @POST("api/admin/user-management/users/delete/{id}")
    Call<ResponseBody> deleteUserApi(
            @Path("id") int id
    );

    @POST("Admin/User/CreateUser")
    Call<ResponseBody> createUser(
            @Body User dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/User/UpdateUser")
    Call<ResponseBody> updateUser(
            @Body User dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/User/DeleteUser")
    Call<ResponseBody> deleteUser(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @GET("api/admin/user-management/branches")
    Call<List<Branch>> getAllBranches();

    @GET("api/admin/user-management/branches/{id}")
    Call<Branch> getBranchById(
            @Path("id") int id
    );

    @POST("api/admin/user-management/branches/create")
    Call<ResponseBody> createBranchApi(
            @Body Branch dto
    );

    @POST("api/admin/user-management/branches/update")
    Call<ResponseBody> updateBranchApi(
            @Body Branch dto
    );

    @POST("api/admin/user-management/branches/delete/{id}")
    Call<ResponseBody> deleteBranchApi(
            @Path("id") int id
    );

    @POST("Admin/Branch/CreateBranch")
    Call<ResponseBody> createBranch(
            @Body Branch dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Branch/UpdateBranch")
    Call<ResponseBody> updateBranch(
            @Body Branch dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Branch/DeleteBranch")
    Call<ResponseBody> deleteBranch(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    @GET("api/admin/user-management/roles")
    Call<List<Role>> getAllRoles();

    @GET("api/admin/user-management/roles/{id}")
    Call<Role> getRoleById(
            @Path("id") int id
    );

    @POST("api/admin/user-management/roles/create")
    Call<ResponseBody> createRoleApi(
            @Body Role dto
    );

    @POST("api/admin/user-management/roles/update-permissions")
    Call<ResponseBody> updateRolePermissionsApi(
            @Body RolePermissionUpdateRequest request
    );

    @POST("api/admin/user-management/roles/delete/{id}")
    Call<ResponseBody> deleteRoleApi(
            @Path("id") int id
    );

    @POST("Admin/Role/CreateRole")
    Call<ResponseBody> createRole(
            @Body Role dto,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @POST("Admin/Role/UpdateRolePermissions")
    Call<ResponseBody> updateRolePermissions(
            @Body RolePermissionUpdateRequest request,
            @Header("Request-Verification-Token") String antiForgeryToken
    );

    @FormUrlEncoded
    @POST("Admin/Role/DeleteRole")
    Call<ResponseBody> deleteRole(
            @Field("id") int id,
            @Field("__RequestVerificationToken") String antiForgeryToken
    );

    // =========================================================================
    // 12. NHÓM CẤU HÌNH HỆ THỐNG (SYSTEM CONFIG)
    // =========================================================================

    @GET("api/admin/system-config")
    Call<SystemConfig> getSystemConfig();

    @POST("api/admin/system-config/update")
    Call<ResponseBody> updateSystemConfig(
            @Body List<ConfigUpdate> configs
    );

    // =========================================================================
    // 13. NHÓM CHƯƠNG TRÌNH TÍCH ĐIỂM (LOYALTY PROGRAM REST)
    // =========================================================================

    @GET("api/admin/customer/loyalty")
    Call<List<LoyaltyProgram>> getAllLoyaltyApi();

    @GET("api/admin/customer/loyalty/{id}")
    Call<LoyaltyProgram> getLoyaltyByIdApi(
            @Path("id") int id
    );

    @POST("api/admin/customer/loyalty/create")
    Call<ResponseBody> createLoyaltyApi(
            @Body LoyaltyProgram dto
    );

    @POST("api/admin/customer/loyalty/update")
    Call<ResponseBody> updateLoyaltyApi(
            @Body LoyaltyProgram dto
    );

    @POST("api/admin/customer/loyalty/delete/{id}")
    Call<ResponseBody> deleteLoyaltyApi(
            @Path("id") int id
    );

    // =========================================================================
    // 14. NHÓM CÔNG NỢ & THANH TOÁN NCC (SUPPLIER DEBT CUD)
    // =========================================================================

    @GET("api/admin/inventory/debt/by-purchase-order/{purchaseOrderId}")
    Call<SupplierDebt> getDebtByPurchaseOrder(
            @Path("purchaseOrderId") int purchaseOrderId
    );

    @POST("api/admin/inventory/debt/create")
    Call<ResponseBody> createSupplierDebtApi(
            @Body SupplierDebt dto
    );

    @POST("api/admin/inventory/debt/update")
    Call<ResponseBody> updateSupplierDebtApi(
            @Body SupplierDebt dto
    );

    @GET("api/admin/inventory/debt/{debtId}/payments")
    Call<List<SupplierPayment>> getPaymentsByDebtId(
            @Path("debtId") int debtId
    );

    @POST("api/admin/inventory/debt/payment")
    Call<ResponseBody> createSupplierPaymentApi(
            @Body SupplierPayment paymentDto
    );

    @POST("api/admin/inventory/debt/payment/delete/{id}")
    Call<ResponseBody> deleteSupplierPaymentApi(
            @Path("id") int id
    );

    // =========================================================================
    // 15. NHÓM ẢNH SẢN PHẨM (PRODUCT IMAGES)
    // =========================================================================

    @GET("api/admin/product/products/{productId}/images")
    Call<List<ProductImage>> getProductImages(
            @Path("productId") int productId
    );

    @POST("api/admin/product/products/images")
    Call<ResponseBody> createProductImageApi(
            @Body ProductImage dto
    );

    @POST("api/admin/product/products/images/delete/{id}")
    Call<ResponseBody> deleteProductImageApi(
            @Path("id") int id
    );

    // =========================================================================
    // 16. NHÓM THANH TOÁN PAYOS & CHI TIẾT ĐƠN HÀNG (PAYOS & ORDER DETAILS)
    // =========================================================================

    @POST("api/admin/sales/orders/create-payos-link")
    Call<CreateQrResponse> createPayOSLink(
            @Body CreateQrRequest qrRequest
    );

    @GET("api/admin/sales/orders/check-payos-status/{orderCode}")
    Call<PayOSStatusResponse> checkPayOSStatus(
            @Path("orderCode") long orderCode
    );

    @POST("api/admin/sales/orders/details/create")
    Call<ResponseBody> createOrderDetailApi(
            @Body OrderDetail dto
    );

    @POST("api/admin/sales/orders/details/update")
    Call<ResponseBody> updateOrderDetailApi(
            @Body OrderDetail dto
    );

    @POST("api/admin/sales/orders/details/delete/{id}")
    Call<ResponseBody> deleteOrderDetailApi(
            @Path("id") int id
    );

    // =========================================================================
    // 17. NHÓM CHI NHÁNH GẦN NHẤT & XÁC THỰC AUTH API
    // =========================================================================

    @GET("api/admin/user-management/branches/nearest")
    Call<Branch> getNearestBranch(
            @Query("lat") double lat,
            @Query("lng") double lng,
            @Query("maxDist") double maxDist
    );

    @POST("api/admin/user-management/auth/login")
    Call<ResponseBody> loginRestApi(
            @Body Map<String, String> credentials
    );

    @POST("api/admin/user-management/auth/request-otp")
    Call<ResponseBody> requestOtpRestApi(
            @Body Map<String, String> body
    );

    @POST("api/admin/user-management/auth/reset-password")
    Call<ResponseBody> resetPasswordRestApi(
            @Body Map<String, String> body
    );
}
