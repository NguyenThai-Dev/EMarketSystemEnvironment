import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../models/dashboard_model.dart';

class DashboardApiService {
  final ApiClient _client = ApiClient();

  /// Lấy tổng quan Dashboard (Doanh thu, Đơn hàng, Lợi nhuận, Khách hàng, Cảnh báo tồn)
  /// Tối ưu tốc độ: Gọi song song cả summary và kpi-summary để đảm bảo số liệu KPI không bị kẹt khi stored procedure nặng.
  Future<DashboardOverview> getOverview({
    int? branchId,
    DateTime? fromDate,
    DateTime? toDate,
    String groupBy = 'day',
  }) async {
    final queryParams = <String, dynamic>{
      'groupBy': groupBy,
      if (branchId != null && branchId > 0) 'branchId': branchId,
      if (fromDate != null) 'fromDate': fromDate.toIso8601String(),
      if (toDate != null) 'toDate': toDate.toIso8601String(),
    };

    // 1. Gọi summary đầy đủ (chứa Trend, StockChart, BranchPerformance, Summary)
    try {
      final res = await _client.get(
        ApiEndpoints.dashboardSummary,
        queryParameters: queryParams,
      );

      if (res.data is Map<String, dynamic>) {
        final data = res.data['data'] ?? res.data['Data'] ?? res.data;
        if (data is Map<String, dynamic>) {
          return DashboardOverview.fromJson(data);
        }
      }
    } catch (_) {}

    // 2. Fallback sang kpi-summary (siêu nhanh, chạy trực tiếp SP summary)
    try {
      final kpiRes = await _client.get(
        ApiEndpoints.dashboardKpiCards,
        queryParameters: branchId != null && branchId > 0 ? {'branchId': branchId} : null,
      );
      if (kpiRes.data is Map<String, dynamic>) {
        final data = kpiRes.data['data'] ?? kpiRes.data['Data'] ?? kpiRes.data;
        if (data is Map<String, dynamic>) {
          return DashboardOverview.fromJson(data);
        }
      }
    } catch (_) {}

    return DashboardOverview();
  }

  /// Lấy hiệu suất hoạt động giữa các chi nhánh
  Future<List<BranchPerformanceModel>> getBranchPerformance({
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
        ApiEndpoints.branchPerformance,
        queryParameters: queryParams,
      );

      dynamic rawList = res.data;
      if (res.data is Map<String, dynamic>) {
        rawList = res.data['data'] ?? res.data['Data'];
      }

      if (rawList is List) {
        return rawList.map((e) => BranchPerformanceModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Lấy cơ cấu biểu đồ tồn kho / ngành hàng theo chi nhánh
  Future<List<ChartItemModel>> getStockChart({int? branchId}) async {
    try {
      final queryParams = <String, dynamic>{
        if (branchId != null && branchId > 0) 'branchId': branchId,
      };

      final res = await _client.get(
        ApiEndpoints.stockChart,
        queryParameters: queryParams,
      );

      dynamic rawList = res.data;
      if (res.data is Map<String, dynamic>) {
        rawList = res.data['data'] ?? res.data['Data'];
      }

      if (rawList is List) {
        return rawList.map((e) => ChartItemModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }
}
