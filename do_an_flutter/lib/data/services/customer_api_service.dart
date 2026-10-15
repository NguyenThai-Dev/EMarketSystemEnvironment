import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../models/customer_model.dart';

class CustomerApiService {
  final ApiClient _client = ApiClient();

  /// Lấy danh sách khách hàng (có hỗ trợ tìm kiếm theo tên hoặc SĐT)
  Future<List<CustomerModel>> getCustomers({String? keyword, int start = 0, int length = 50}) async {
    try {
      if (keyword != null && keyword.trim().isNotEmpty) {
        final res = await _client.get(
          ApiEndpoints.searchCustomers,
          queryParameters: {
            'keyword': keyword.trim(),
            'start': start,
            'length': length,
          },
        );

        if (res.data is Map && res.data['data'] is List) {
          return (res.data['data'] as List).map((e) => CustomerModel.fromJson(e)).toList();
        }
      } else {
        final res = await _client.get(ApiEndpoints.allCustomers);
        if (res.data is List) {
          return (res.data as List).map((e) => CustomerModel.fromJson(e)).toList();
        } else if (res.data is Map && res.data['data'] is List) {
          return (res.data['data'] as List).map((e) => CustomerModel.fromJson(e)).toList();
        }
      }
    } catch (_) {}
    return [];
  }

  /// Lấy chi tiết khách hàng theo ID
  Future<CustomerModel?> getCustomerById(int id) async {
    try {
      final res = await _client.get('${ApiEndpoints.allCustomers}/$id');
      if (res.data is Map<String, dynamic>) {
        return CustomerModel.fromJson(res.data);
      }
    } catch (_) {}
    return null;
  }

  /// Tạo mới khách hàng
  Future<int?> createCustomer(CustomerCreateRequest req) async {
    try {
      final res = await _client.post(
        ApiEndpoints.createCustomer,
        data: req.toJson(),
      );
      if (res.data is Map) {
        return res.data['customerId'] as int?;
      }
    } catch (_) {}
    return null;
  }

  /// Cập nhật thông tin khách hàng
  Future<bool> updateCustomer(CustomerUpdateRequest req) async {
    try {
      final res = await _client.post(
        ApiEndpoints.updateCustomer,
        data: req.toJson(),
      );
      if (res.data is Map) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {}
    return false;
  }

  /// Xóa khách hàng
  Future<bool> deleteCustomer(int id) async {
    try {
      final res = await _client.post('${ApiEndpoints.deleteCustomer}/$id');
      if (res.data is Map) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {}
    return false;
  }

  /// Lấy lịch sử tích điểm hội viên
  Future<List<LoyaltyProgramModel>> getLoyaltyHistory({int? customerId}) async {
    try {
      final res = await _client.get(ApiEndpoints.loyaltyPrograms);
      if (res.data is List) {
        final all = (res.data as List).map((e) => LoyaltyProgramModel.fromJson(e)).toList();
        if (customerId != null && customerId > 0) {
          return all.where((l) => l.customerId == customerId).toList();
        }
        return all;
      }
    } catch (_) {}
    return [];
  }

  /// Lấy thống kê số lượng khách hàng
  Future<CustomerStatsModel?> getCustomerStats() async {
    try {
      final res = await _client.get(ApiEndpoints.customerStats);
      if (res.data is Map<String, dynamic>) {
        return CustomerStatsModel.fromJson(res.data);
      }
    } catch (_) {}
    return null;
  }
}
