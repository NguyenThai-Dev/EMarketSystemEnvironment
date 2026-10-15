import '../models/ai_model.dart';
import '../models/customer_model.dart';
import '../models/dashboard_model.dart';
import '../models/expense_model.dart';
import '../models/inventory_model.dart';
import '../models/order_model.dart';
import '../models/product_model.dart';
import '../models/quotation_model.dart';
import '../models/user_model.dart';
import '../services/ai_api_service.dart';
import '../services/auth_api_service.dart';
import '../services/customer_api_service.dart';
import '../services/dashboard_api_service.dart';
import '../services/expense_api_service.dart';
import '../services/inventory_api_service.dart';
import '../services/order_api_service.dart';
import '../services/product_api_service.dart';
import '../services/quotation_api_service.dart';

/// Unified Data Repository Facade matching DataRepository.java from Do_An_Di_Dong
class DataRepository {
  static final DataRepository _instance = DataRepository._internal();
  factory DataRepository() => _instance;

  final AuthApiService authService;
  final ProductApiService productService;
  final OrderApiService orderService;
  final DashboardApiService dashboardService;
  final InventoryApiService inventoryService;
  final CustomerApiService customerService;
  final QuotationApiService quotationService;
  final ExpenseApiService expenseService;
  final AiApiService aiService;

  DataRepository._internal()
      : authService = AuthApiService(),
        productService = ProductApiService(),
        orderService = OrderApiService(),
        dashboardService = DashboardApiService(),
        inventoryService = InventoryApiService(),
        customerService = CustomerApiService(),
        quotationService = QuotationApiService(),
        expenseService = ExpenseApiService(),
        aiService = AiApiService();

  // Authentication & Password Recovery
  Future<LoginResponse> login(String username, String password) =>
      authService.login(username, password);

  Future<LoginResponse> loginWithGoogle({required String email, String? fullName, String? idToken}) =>
      authService.loginWithGoogle(email: email, fullName: fullName, idToken: idToken);

  Future<void> signOut() => authService.signOut();

  Future<UserModel?> getCurrentUser() => authService.getCurrentUser();

  Future<List<BranchModel>> getBranches() => authService.getBranches();

  Future<bool> requestOtp(String email) => authService.requestOtp(email);

  Future<bool> resetPassword(String email, String otp, String newPassword) =>
      authService.resetPassword(email, otp, newPassword);

  // User & Staff Management
  Future<List<UserModel>> getUsers({String? keyword, int start = 0, int length = 50}) =>
      authService.getUsers(keyword: keyword, start: start, length: length);

  Future<List<RoleModel>> getRoles() =>
      authService.getRoles();

  Future<bool> createUser(Map<String, dynamic> data) =>
      authService.createUser(data);

  Future<bool> updateUser(Map<String, dynamic> data) =>
      authService.updateUser(data);

  Future<bool> deleteUser(int userId) =>
      authService.deleteUser(userId);

  Future<String?> uploadUserAvatar({required String filePath, String? fileName, int? userId}) =>
      authService.uploadAvatar(filePath: filePath, fileName: fileName, userId: userId);

  // Dashboard & Analytics
  Future<DashboardOverview> getDashboardOverview({
    int? branchId,
    DateTime? fromDate,
    DateTime? toDate,
    String groupBy = 'day',
  }) =>
      dashboardService.getOverview(
        branchId: branchId,
        fromDate: fromDate,
        toDate: toDate,
        groupBy: groupBy,
      );

  Future<List<BranchPerformanceModel>> getBranchPerformance({
    int? branchId,
    DateTime? fromDate,
    DateTime? toDate,
  }) =>
      dashboardService.getBranchPerformance(
        branchId: branchId,
        fromDate: fromDate,
        toDate: toDate,
      );

  Future<List<ChartItemModel>> getStockChart({int? branchId}) =>
      dashboardService.getStockChart(branchId: branchId);

  // Products & Lots
  Future<ProductPagedResponse> getProducts({
    int start = 0,
    int length = 20,
    String? keyword,
    int? categoryId,
    int? branchId,
  }) =>
      productService.getProductsPaged(
        start: start,
        length: length,
        keyword: keyword,
        categoryId: categoryId,
        branchId: branchId,
      );

  Future<List<ProductCategoryModel>> getCategories() =>
      productService.getCategories();

  Future<List<ProductLotModel>> getProductLots(int productId) =>
      productService.getProductLots(productId);

  Future<List<ProductModel>> getLowStockAlerts({int top = 20}) =>
      productService.getLowStockAlerts(top: top);

  Future<List<ProductModel>> searchProductsByBarcode(String barcode, {int? branchId}) =>
      productService.searchByBarcode(barcode, branchId: branchId);

  // Suppliers & Debts
  Future<List<SupplierModel>> getSuppliers({String? keyword}) =>
      productService.getSuppliers(keyword: keyword);

  Future<bool> createSupplier(Map<String, dynamic> data) =>
      productService.createSupplier(data);

  Future<bool> updateSupplier(Map<String, dynamic> data) =>
      productService.updateSupplier(data);

  Future<bool> deleteSupplier(int supplierId) =>
      productService.deleteSupplier(supplierId);

  Future<List<SupplierDebtModel>> getSupplierDebts({int? supplierId, String? status, String? keyword}) =>
      productService.getSupplierDebts(supplierId: supplierId, status: status, keyword: keyword);

  Future<SupplierDebtSummaryModel> getSupplierDebtSummary({int? supplierId}) =>
      productService.getSupplierDebtSummary(supplierId: supplierId);

  Future<bool> paySupplierDebt({
    required int debtId,
    required double amount,
    String paymentMethod = 'Tiền mặt',
    String? proof,
  }) =>
      productService.paySupplierDebt(
        debtId: debtId,
        amount: amount,
        paymentMethod: paymentMethod,
        proof: proof,
      );

  Future<int?> createProduct(Map<String, dynamic> data) =>
      productService.createProduct(data);

  Future<bool> updateProduct(Map<String, dynamic> data) =>
      productService.updateProduct(data);

  Future<bool> deleteProduct(int id) =>
      productService.deleteProduct(id);

  Future<String?> uploadProductImage({required String filePath, String? fileName, int? productId}) =>
      productService.uploadProductImage(filePath: filePath, fileName: fileName, productId: productId);

  // Orders
  Future<List<OrderModel>> getOrders({
    int start = 0,
    int length = 20,
    String? status,
    int? branchId,
    String? keyword,
    DateTime? fromDate,
    DateTime? toDate,
  }) =>
      orderService.getOrders(
        start: start,
        length: length,
        status: status,
        branchId: branchId,
        keyword: keyword,
        fromDate: fromDate,
        toDate: toDate,
      );

  Future<OrderModel?> getOrderDetail(int orderId) =>
      orderService.getOrderDetail(orderId);

  Future<bool> cancelOrder(int orderId) =>
      orderService.cancelOrder(orderId);

  Future<CheckoutResult> checkout(CheckoutRequest request) =>
      orderService.checkout(request);

  Future<CreateQrResponse> createPayOsLink(CreateQrRequest request) =>
      orderService.createPayOsLink(request);

  Future<PayOsStatusResponse> checkPayOsStatus(int orderCode) =>
      orderService.checkPayOsStatus(orderCode);

  // Warehouse & Inventory Management
  Future<List<WarehouseModel>> getWarehousesByBranch([int? branchId]) =>
      inventoryService.getWarehousesByBranch(branchId);

  Future<List<InventoryItemModel>> getInventoryItems({
    int? branchId,
    int? warehouseId,
    int? productId,
  }) =>
      inventoryService.getInventoryItems(
        branchId: branchId,
        warehouseId: warehouseId,
        productId: productId,
      );

  Future<List<StockMovementModel>> getStockMovements({
    int? warehouseId,
    int? branchId,
    int start = 0,
    int length = 50,
    String? type,
    String? keyword,
    DateTime? fromDate,
    DateTime? toDate,
  }) =>
      inventoryService.getStockMovements(
        warehouseId: warehouseId,
        branchId: branchId,
        start: start,
        length: length,
        type: type,
        keyword: keyword,
        fromDate: fromDate,
        toDate: toDate,
      );

  Future<bool> adjustStock(StockAdjustmentRequest request) =>
      inventoryService.adjustStock(request);

  // Inbound / Purchase Orders
  Future<List<PurchaseOrderModel>> getPurchases({
    int start = 0,
    int length = 20,
    String? keyword,
    int? branchId,
  }) =>
      inventoryService.getPurchases(
        start: start,
        length: length,
        keyword: keyword,
        branchId: branchId,
      );

  Future<PurchaseOrderModel?> getPurchaseDetail(int purchaseOrderId) =>
      inventoryService.getPurchaseDetail(purchaseOrderId);

  Future<bool> createPurchase(PurchaseOrderModel order) =>
      inventoryService.createPurchase(order);

  Future<bool> deletePurchase(int purchaseOrderId) =>
      inventoryService.deletePurchase(purchaseOrderId);

  // Customer Management
  Future<List<CustomerModel>> getCustomers({String? keyword, int start = 0, int length = 50}) =>
      customerService.getCustomers(keyword: keyword, start: start, length: length);

  Future<CustomerModel?> getCustomerById(int id) =>
      customerService.getCustomerById(id);

  Future<int?> createCustomer(CustomerCreateRequest req) =>
      customerService.createCustomer(req);

  Future<bool> updateCustomer(CustomerUpdateRequest req) =>
      customerService.updateCustomer(req);

  Future<bool> deleteCustomer(int id) =>
      customerService.deleteCustomer(id);

  Future<List<LoyaltyProgramModel>> getLoyaltyHistory({int? customerId}) =>
      customerService.getLoyaltyHistory(customerId: customerId);

  Future<CustomerStatsModel?> getCustomerStats() =>
      customerService.getCustomerStats();

  // Quotation Management
  Future<List<QuotationModel>> getQuotations({
    String? keyword,
    int? branchId,
    String? status,
    DateTime? fromDate,
    DateTime? toDate,
  }) =>
      quotationService.getQuotations(
        keyword: keyword,
        branchId: branchId,
        status: status,
        fromDate: fromDate,
        toDate: toDate,
      );

  Future<QuotationModel?> getQuotationDetail(int id) =>
      quotationService.getQuotationDetail(id);

  Future<int?> saveQuotation(QuotationModel quotation) =>
      quotationService.saveQuotation(quotation);

  Future<bool> changeQuotationStatus(int id, String status) =>
      quotationService.changeQuotationStatus(id, status);

  Future<int?> convertQuotationToOrder(int id, {int userId = 1}) =>
      quotationService.convertQuotationToOrder(id, userId: userId);

  Future<bool> deleteQuotation(int id) =>
      quotationService.deleteQuotation(id);

  // Expense Management
  Future<List<ExpenseModel>> getExpenses({
    int? branchId,
    int? categoryId,
    String? status,
    DateTime? fromDate,
    DateTime? toDate,
  }) =>
      expenseService.getExpenses(
        branchId: branchId,
        categoryId: categoryId,
        status: status,
        fromDate: fromDate,
        toDate: toDate,
      );

  Future<ExpenseSummaryModel?> getExpenseSummary({
    int? branchId,
    DateTime? fromDate,
    DateTime? toDate,
  }) =>
      expenseService.getExpenseSummary(
        branchId: branchId,
        fromDate: fromDate,
        toDate: toDate,
      );

  Future<ExpenseModel?> getExpenseDetail(int id) =>
      expenseService.getExpenseDetail(id);

  Future<bool> createExpense(CreateExpenseRequest req) =>
      expenseService.createExpense(req);

  Future<bool> updateExpenseStatus(
    int expenseId,
    String status, {
    String? rejectReason,
    String? paymentMethod,
  }) =>
      expenseService.updateExpenseStatus(
        expenseId,
        status,
        rejectReason: rejectReason,
        paymentMethod: paymentMethod,
      );

  Future<bool> deleteExpense(int id) =>
      expenseService.deleteExpense(id);

  Future<List<ExpenseCategoryModel>> getExpenseCategories() =>
      expenseService.getCategories();

  // AI Assistant & Forecasting
  Future<String> askAi(String prompt, {String? userName}) => aiService.askAi(prompt, userName: userName);

  Future<List<AIReplenishmentModel>> getReplenishmentAdvice(int branchId) =>
      aiService.getReplenishmentAdvice(branchId);

  Future<List<AIAnomalyModel>> getAnomalies(int branchId) =>
      aiService.getAnomalies(branchId);

  Future<List<AIInsightModel>> getProductInsights(int branchId) =>
      aiService.getProductInsights(branchId);

  Future<bool> runAiAnalysis({String mode = 'system', int branchId = 1}) =>
      aiService.runAnalysis(mode: mode, branchId: branchId);
}
