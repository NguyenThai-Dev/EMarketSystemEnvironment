class ApiEndpoints {
  // Authentication & Session (UserAdminApiController)
  static const String directLogin = 'api/admin/user-management/auth/login';
  static const String externalLogin = 'api/admin/user-management/auth/external-login';
  static const String googleLogin = 'api/admin/user-management/auth/google-login';
  static const String requestOtp = 'api/admin/user-management/auth/request-otp';
  static const String resetPassword = 'api/admin/user-management/auth/reset-password';
  static const String verifyChangeEmail = 'api/admin/user-management/users/verify-change-email';
  static const String updateEmail = 'api/admin/user-management/users/update-email';
  static const String allUsers = 'api/admin/user-management/users';
  static const String pagedUsers = 'api/admin/user-management/users/paged';
  static const String userStats = 'api/admin/user-management/users/stats';
  static const String userRoles = 'api/admin/user-management/roles';
  static const String userPermissions = 'api/admin/user-management/permissions';
  static const String createUser = 'api/admin/user-management/users/create';
  static const String updateUser = 'api/admin/user-management/users/update';
  static const String deleteUser = 'api/admin/user-management/users/delete';
  static const String uploadUserAvatar = 'api/admin/user-management/users/upload-avatar';

  // Fallback MVC Session Login
  static const String loginPage = 'Admin/Login/Login';
  static const String loginPost = 'Admin/Login/Login';
  static const String signOut = 'Admin/Login/SignOut';
  static const String getCurrentUser = 'Admin/Login/GetCurrentUser';

  // Branch & Warehouse (UserAdminApiController & InventoryAdminApiController)
  static const String getAllBranches = 'api/admin/user-management/branches';
  static const String searchBranches = 'api/admin/user-management/branches/search';
  static const String nearestBranch = 'api/admin/user-management/branches/nearest';
  static const String getAllWarehouses = 'api/admin/inventory/warehouses/all';
  static const String warehousesByBranch = 'api/admin/inventory/warehouses/by-branch';

  // Dashboard (DashboardAdminApiController)
  static const String dashboardKpiCards = 'api/admin/dashboard/kpi-summary';
  static const String dashboardSummary = 'api/admin/dashboard/summary';
  static const String dashboardOverview = 'api/admin/dashboard/overview';
  static const String branchPerformance = 'api/admin/dashboard/branch-performance';
  static const String stockChart = 'api/admin/dashboard/stock-chart';
  static const String warehouseDashboard = 'api/admin/dashboard/warehouse';
  static const String financeDashboard = 'api/admin/dashboard/finance';
  static const String debtDashboard = 'api/admin/dashboard/debt';
  static const String peopleDashboard = 'api/admin/dashboard/people';

  // Product Management (ProductAdminApiController: RoutePrefix api/admin/product)
  static const String allProducts = 'api/admin/product/products';
  static const String pagedProducts = 'api/admin/product/products/paged';
  static const String searchProducts = 'api/admin/product/products/search';
  static const String searchProductsSimple = 'api/admin/product/products/search-simple';
  static const String productsByBarcode = 'api/admin/product/products/barcode';
  static const String productCategories = 'api/admin/product/categories';
  static const String suppliers = 'api/admin/product/suppliers';
  static const String productLots = 'api/admin/product/lots/by-product';
  static const String allLots = 'api/admin/product/lots';
  static const String lowStockAlerts = 'api/admin/product/products/low-stock-alerts';
  static const String createProduct = 'api/admin/product/products';
  static const String updateProduct = 'api/admin/product/products/update';
  static const String deleteProduct = 'api/admin/product/products/delete';
  static const String activeProduct = 'api/admin/product/products/active';
  static const String inactiveProducts = 'api/admin/product/products/inactive';
  static const String uploadProductImage = 'api/admin/product/products/upload-image';

  // Sales & Orders (SalesAdminApiController: RoutePrefix api/admin/sales)
  static const String allOrders = 'api/admin/sales/orders/all';
  static const String pagedOrders = 'api/admin/sales/orders';
  static const String checkout = 'api/admin/sales/orders/checkout';
  static const String createOrder = 'api/admin/sales/orders/create';
  static const String orderStatus = 'api/admin/sales/orders';
  static const String createPayOsLink = 'api/admin/sales/orders/create-payos-link';
  static const String checkPayOsStatus = 'api/admin/sales/orders/check-payos-status';
  static const String promotions = 'api/admin/sales/promotions';
  static const String activePromotions = 'api/admin/sales/promotions/active';

  // System Config (SystemConfigAdminApiController: RoutePrefix api/admin/system-config)
  static const String systemConfig = 'api/admin/system-config';
  static const String getBankId = 'Admin/Order/GetEMarketBankID';
  static const String getBankNum = 'Admin/Order/GetEMarketBankNum';

  // Customer Management (CustomerAdminApiController: RoutePrefix api/admin/customer)
  static const String allCustomers = 'api/admin/customer';
  static const String searchCustomers = 'api/admin/customer/search';
  static const String createCustomer = 'api/admin/customer/create';
  static const String updateCustomer = 'api/admin/customer/update';
  static const String deleteCustomer = 'api/admin/customer/delete';
  static const String customerStats = 'api/admin/customer/stats';
  static const String loyaltyPrograms = 'api/admin/customer/loyalty';
  static const String customerAddresses = 'api/admin/customer/address/by-customer';

  // Quotation Management (QuotationAdminApiController: RoutePrefix api/admin/quotation)
  static const String quotationList = 'api/admin/quotation/list';
  static const String quotationPaged = 'api/admin/quotation/paged';
  static const String quotationSave = 'api/admin/quotation/save';
  static const String quotationStatus = 'api/admin/quotation';
  static const String quotationConvertToOrder = 'api/admin/quotation';
  static const String quotationDelete = 'api/admin/quotation/delete';

  // Inventory Management (InventoryAdminApiController: RoutePrefix api/admin/inventory)
  static const String inventoryItems = 'api/admin/inventory/items';
  static const String stockMovements = 'api/admin/inventory/stock-movements';
  static const String stockTotalActual = 'api/admin/inventory/stock/total-actual';
  static const String stockAdjustment = 'api/admin/inventory/stock/adjust';
  static const String purchaseOrders = 'api/admin/inventory/purchase';
  static const String purchaseOrdersPaged = 'api/admin/inventory/purchase/paged';
  static const String supplierDebts = 'api/admin/inventory/debt';
  static const String supplierDebtsPaged = 'api/admin/inventory/debt/paged';
  static const String supplierDebtSummary = 'api/admin/inventory/debt/summary';
  static const String supplierDebtPayment = 'api/admin/inventory/debt/payment';

  // Expense Management (ExpenseAdminApiController: RoutePrefix api/admin/expense)
  static const String expenseList = 'api/admin/expense/list';
  static const String expensePaged = 'api/admin/expense/paged';
  static const String expenseSummary = 'api/admin/expense/summary';
  static const String createExpense = 'api/admin/expense/create';
  static const String updateExpenseStatus = 'api/admin/expense/update-status';
  static const String deleteExpense = 'api/admin/expense/delete';
  static const String expenseCategories = 'api/admin/expense/categories';

  // AI Analysis (AIAnalysisApiController: RoutePrefix api/admin/ai-analysis)
  static const String aiRunAnalysis = 'api/admin/ai-analysis/run-analysis';
  static const String aiRunProphet = 'api/admin/ai-analysis/run-prophet';
  static const String aiReplenishmentAdvice = 'api/admin/ai-analysis/replenishment-advice';
  static const String aiRecommendations = 'api/admin/ai-analysis/recommendations';
  static const String aiAnomalies = 'api/admin/ai-analysis/anomalies';
  static const String aiInsights = 'api/admin/ai-analysis/insights';
  static const String aiInventoryForecast = 'api/admin/ai-analysis/inventory-forecast';
  static const String aiSalesForecast = 'api/admin/ai-analysis/sales-forecast';
  static const String aiTopPredicted = 'api/admin/ai-analysis/top-predicted';
  static const String aiDeadstock = 'api/admin/ai-analysis/deadstock';
  static const String aiLotRisk = 'api/admin/ai-analysis/lot-financial-risk';

  // AI Assistant Service (Cổng 5001 HTTPS / 5000 HTTP)
  static const String aiChat = 'api/chat/ask';
  static const String aiForecast = 'api/forecast';
}
