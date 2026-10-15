import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../models/expense_model.dart';

class ExpenseApiService {
  final ApiClient _client = ApiClient();

  /// Lấy danh sách phiếu chi
  Future<List<ExpenseModel>> getExpenses({
    int? branchId,
    int? categoryId,
    String? status,
    DateTime? fromDate,
    DateTime? toDate,
  }) async {
    try {
      final queryParams = <String, dynamic>{
        if (branchId != null && branchId > 0) 'branchId': branchId,
        if (categoryId != null && categoryId > 0) 'categoryId': categoryId,
        if (status != null && status.isNotEmpty) 'status': status,
        if (fromDate != null) 'fromDate': fromDate.toIso8601String(),
        if (toDate != null) 'toDate': toDate.toIso8601String(),
      };

      final res = await _client.get(
        ApiEndpoints.expenseList,
        queryParameters: queryParams,
      );

      if (res.data is List) {
        return (res.data as List).map((e) => ExpenseModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Lấy tổng hợp thống kê chi phí
  Future<ExpenseSummaryModel?> getExpenseSummary({
    int? branchId,
    DateTime? fromDate,
    DateTime? toDate,
  }) async {
    try {
      final queryParams = <String, dynamic>{
        if (branchId != null && branchId > 0) 'branchId': branchId,
        if (fromDate != null) 'fromDate': fromDate.toIso8601String(),
        if (toDate != null) 'toDate': toDate.toIso8601String(),
      };

      final res = await _client.get(
        ApiEndpoints.expenseSummary,
        queryParameters: queryParams,
      );

      if (res.data is Map<String, dynamic>) {
        return ExpenseSummaryModel.fromJson(res.data);
      }
    } catch (_) {}
    return null;
  }

  /// Lấy chi tiết một phiếu chi
  Future<ExpenseModel?> getExpenseDetail(int id) async {
    try {
      final res = await _client.get('${ApiEndpoints.expenseList}/$id');
      if (res.data is Map<String, dynamic>) {
        return ExpenseModel.fromJson(res.data);
      }
    } catch (_) {}
    return null;
  }

  /// Tạo phiếu chi mới
  Future<bool> createExpense(CreateExpenseRequest req) async {
    try {
      final res = await _client.post(
        ApiEndpoints.createExpense,
        data: req.toJson(),
      );
      if (res.data is Map) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {}
    return false;
  }

  /// Duyệt hoặc từ chối phiếu chi (Approved / Rejected)
  Future<bool> updateExpenseStatus(
    int expenseId,
    String status, {
    String? rejectReason,
    String? paymentMethod,
  }) async {
    try {
      final payload = <String, dynamic>{
        'ExpenseId': expenseId,
        'Status': status,
      };
      if (rejectReason != null) payload['RejectReason'] = rejectReason;
      if (paymentMethod != null) payload['PaymentMethod'] = paymentMethod;

      final res = await _client.post(
        ApiEndpoints.updateExpenseStatus,
        data: payload,
      );
      if (res.data is Map) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {}
    return false;
  }

  /// Xóa phiếu chi
  Future<bool> deleteExpense(int id) async {
    try {
      final res = await _client.post('${ApiEndpoints.deleteExpense}/$id');
      if (res.data is Map) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {}
    return false;
  }

  /// Lấy danh mục chi phí (Categories)
  Future<List<ExpenseCategoryModel>> getCategories() async {
    try {
      final res = await _client.get(ApiEndpoints.expenseCategories);
      if (res.data is List) {
        return (res.data as List).map((e) => ExpenseCategoryModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }
}
