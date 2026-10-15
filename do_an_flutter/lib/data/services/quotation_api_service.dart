import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../models/quotation_model.dart';

class QuotationApiService {
  final ApiClient _client = ApiClient();

  /// Lấy danh sách báo giá
  Future<List<QuotationModel>> getQuotations({
    String? keyword,
    int? branchId,
    String? status,
    DateTime? fromDate,
    DateTime? toDate,
  }) async {
    try {
      final queryParams = <String, dynamic>{
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        if (branchId != null && branchId > 0) 'branchId': branchId,
        if (status != null && status.isNotEmpty) 'status': status,
        if (fromDate != null) 'fromDate': fromDate.toIso8601String(),
        if (toDate != null) 'toDate': toDate.toIso8601String(),
      };

      final res = await _client.get(
        ApiEndpoints.quotationList,
        queryParameters: queryParams,
      );

      if (res.data is List) {
        return (res.data as List).map((e) => QuotationModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Lấy chi tiết báo giá kèm danh sách mặt hàng
  Future<QuotationModel?> getQuotationDetail(int id) async {
    try {
      final res = await _client.get('${ApiEndpoints.quotationStatus}/$id');
      if (res.data is Map<String, dynamic>) {
        return QuotationModel.fromJson(res.data);
      }
    } catch (_) {}
    return null;
  }

  /// Tạo hoặc cập nhật báo giá
  Future<int?> saveQuotation(QuotationModel quotation) async {
    try {
      final res = await _client.post(
        ApiEndpoints.quotationSave,
        data: quotation.toJson(),
      );
      if (res.data is Map) {
        return res.data['quotationId'] as int?;
      }
    } catch (_) {}
    return null;
  }

  /// Đổi trạng thái báo giá (Draft, Sent, Accepted, Cancelled)
  Future<bool> changeQuotationStatus(int id, String status) async {
    try {
      final res = await _client.post(
        '${ApiEndpoints.quotationStatus}/$id/status',
        data: {'Status': status},
      );
      if (res.data is Map) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {}
    return false;
  }

  /// Chuyển đổi báo giá thành Đơn hàng chính thức (Convert to Order)
  Future<int?> convertQuotationToOrder(int id, {int userId = 1}) async {
    try {
      final res = await _client.post(
        '${ApiEndpoints.quotationConvertToOrder}/$id/convert-to-order',
        queryParameters: {'userId': userId},
      );
      if (res.data is Map) {
        return res.data['orderId'] as int? ?? res.data['OrderId'] as int?;
      }
    } catch (_) {}
    return null;
  }

  /// Xóa báo giá
  Future<bool> deleteQuotation(int id) async {
    try {
      final res = await _client.post('${ApiEndpoints.quotationDelete}/$id');
      if (res.data is Map) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {}
    return false;
  }
}
