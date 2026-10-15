import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../models/order_model.dart';

class OrderApiService {
  final ApiClient _client = ApiClient();

  /// Get Orders List with pagination and filters
  Future<List<OrderModel>> getOrders({
    int start = 0,
    int length = 20,
    String? status,
    int? branchId,
    String? keyword,
    DateTime? fromDate,
    DateTime? toDate,
  }) async {
    final effectiveToDate = toDate ?? DateTime.now();
    final queryParams = <String, dynamic>{
      'start': start,
      'length': length,
      if (status != null && status.isNotEmpty && status != 'ALL') 'status': status,
      if (branchId != null && branchId > 0) 'branchId': branchId,
      if (keyword != null && keyword.trim().isNotEmpty) 'keyword': keyword.trim(),
      if (fromDate != null) 'fromDate': fromDate.toIso8601String().split('T').first,
      'toDate': effectiveToDate.toIso8601String().split('T').first,
    };

    final res = await _client.get(
      ApiEndpoints.pagedOrders,
      queryParameters: queryParams,
    );

    if (res.data is List) {
      return (res.data as List).map((e) => OrderModel.fromJson(e)).toList();
    }
    if (res.data is Map<String, dynamic>) {
      final list = res.data['data'] ?? res.data['Data'] ?? res.data['orders'];
      if (list is List) {
        return list.map((e) => OrderModel.fromJson(e)).toList();
      }
    }
    return [];
  }

  /// Get Single Order Detail with line items
  Future<OrderModel?> getOrderDetail(int orderId) async {
    try {
      final res = await _client.get('${ApiEndpoints.pagedOrders}/$orderId');
      if (res.data is Map<String, dynamic>) {
        final orderRaw = res.data['Order'] ?? res.data['order'] ?? res.data;
        final detailsRaw = res.data['Details'] ?? res.data['details'];
        if (orderRaw is Map<String, dynamic>) {
          final Map<String, dynamic> combined = Map<String, dynamic>.from(orderRaw);
          if (detailsRaw != null) combined['details'] = detailsRaw;
          return OrderModel.fromJson(combined);
        }
      }
    } catch (_) {}
    return null;
  }

  /// Cancel Order (Update status to Cancelled)
  Future<bool> cancelOrder(int orderId) async {
    try {
      final res = await _client.post(
        '${ApiEndpoints.pagedOrders}/$orderId/status',
        data: {'Status': 'Cancelled'},
      );
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] == true;
      }
      return true;
    } catch (_) {
      return false;
    }
  }

  /// Create / Checkout Order via EMarket REST Web API
  Future<CheckoutResult> checkout(CheckoutRequest request) async {
    final res = await _client.post(
      ApiEndpoints.checkout,
      data: request.toJson(),
    );

    if (res.data is Map<String, dynamic>) {
      return CheckoutResult.fromJson(res.data);
    }
    return CheckoutResult(success: true, message: 'Đã tạo đơn thành công');
  }

  /// Create real PayOS QR payment link via backend SalesAdminApiController
  Future<CreateQrResponse> createPayOsLink(CreateQrRequest request) async {
    final res = await _client.post(
      ApiEndpoints.createPayOsLink,
      data: request.toJson(),
    );

    if (res.data is Map<String, dynamic>) {
      return CreateQrResponse.fromJson(res.data);
    }
    return CreateQrResponse(success: false, message: 'Không thể tạo mã thanh toán.');
  }

  /// Check payment status for an order code via backend SalesAdminApiController
  Future<PayOsStatusResponse> checkPayOsStatus(int orderCode) async {
    final res = await _client.get(
      '${ApiEndpoints.checkPayOsStatus}/$orderCode',
    );

    if (res.data is Map<String, dynamic>) {
      return PayOsStatusResponse.fromJson(res.data);
    }
    return PayOsStatusResponse(success: false, isPaid: false);
  }
}
