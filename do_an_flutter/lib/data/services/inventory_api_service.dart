import 'package:flutter/foundation.dart';
import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../models/inventory_model.dart';

class InventoryApiService {
  final ApiClient _client = ApiClient();

  /// Get list of warehouses by branch ID (or all warehouses if branchId is null/0)
  Future<List<WarehouseModel>> getWarehousesByBranch([int? branchId]) async {
    try {
      if (branchId != null && branchId > 0) {
        final res = await _client.get('${ApiEndpoints.warehousesByBranch}/$branchId');
        if (res.data is List && (res.data as List).isNotEmpty) {
          return (res.data as List).map((e) => WarehouseModel.fromJson(e)).toList();
        }
      }
      final resAll = await _client.get(ApiEndpoints.getAllWarehouses);
      if (resAll.data is List) {
        return (resAll.data as List).map((e) => WarehouseModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Get inventory items in warehouse or branch
  Future<List<InventoryItemModel>> getInventoryItems({
    int? branchId,
    int? warehouseId,
    int? productId,
  }) async {
    try {
      final queryParams = <String, dynamic>{
        if (branchId != null && branchId > 0) 'branchId': branchId,
        if (warehouseId != null && warehouseId > 0) 'warehouseId': warehouseId,
        if (productId != null && productId > 0) 'productId': productId,
      };

      final res = await _client.get(
        ApiEndpoints.inventoryItems,
        queryParameters: queryParams,
      );

      if (res.data is List) {
        return (res.data as List).map((e) => InventoryItemModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Get stock movements audit log
  Future<List<StockMovementModel>> getStockMovements({
    int? warehouseId,
    int? branchId,
    int start = 0,
    int length = 50,
    String? type,
    String? keyword,
    DateTime? fromDate,
    DateTime? toDate,
  }) async {
    try {
      final effectiveToDate = toDate ?? DateTime.now();
      final queryParams = <String, dynamic>{
        'start': start,
        'length': length,
        if (warehouseId != null && warehouseId > 0) 'warehouseId': warehouseId,
        if (branchId != null && branchId > 0) 'branchId': branchId,
        if (type != null && type.isNotEmpty && type != 'ALL') 'type': type,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        if (fromDate != null) 'fromDate': fromDate.toIso8601String().split('T').first,
        'toDate': effectiveToDate.toIso8601String().split('T').first,
      };

      final res = await _client.get(
        ApiEndpoints.stockMovements,
        queryParameters: queryParams,
      );

      final list = (res.data is Map<String, dynamic>)
          ? (res.data['data'] ?? res.data['Data'] ?? [])
          : (res.data is List ? res.data : []);

      debugPrint('================================================================');
      debugPrint('[API BIẾN ĐỘNG KHO] Link API: ${res.requestOptions.uri}');
      debugPrint('[API BIẾN ĐỘNG KHO] Trạng thái HTTP: ${res.statusCode}');
      debugPrint('[API BIẾN ĐỘNG KHO] Số dòng dữ liệu trả về: ${list is List ? list.length : 0}');
      if (res.data is Map<String, dynamic>) {
        debugPrint('[API BIẾN ĐỘNG KHO] Total in DB: ${res.data['total'] ?? res.data['recordsTotal']}');
      }
      debugPrint('================================================================');

      if (list is List) {
        return list.map((e) => StockMovementModel.fromJson(e)).toList();
      }
    } catch (e, stack) {
      debugPrint('================================================================');
      debugPrint('[API BIẾN ĐỘNG KHO LỖI]: $e');
      debugPrint(stack.toString());
      debugPrint('================================================================');
    }
    return [];
  }

  /// Adjust stock after physical count / stocktaking
  Future<bool> adjustStock(StockAdjustmentRequest request) async {
    final res = await _client.post(
      ApiEndpoints.stockAdjustment,
      data: request.toJson(),
    );

    if (res.data is Map<String, dynamic>) {
      return res.data['success'] ?? res.data['Success'] ?? false;
    }
    return true;
  }

  /// Get purchase orders paged
  Future<List<PurchaseOrderModel>> getPurchases({
    int start = 0,
    int length = 20,
    String? keyword,
    int? branchId,
  }) async {
    try {
      final queryParams = <String, dynamic>{
        'start': start,
        'length': length,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        if (branchId != null && branchId > 0) 'branchId': branchId,
      };

      final res = await _client.get(
        ApiEndpoints.purchaseOrdersPaged,
        queryParameters: queryParams,
      );

      dynamic raw = res.data;
      if (raw is Map<String, dynamic>) {
        raw = raw['data'] ?? raw['Data'] ?? raw['items'] ?? raw['Items'];
      }
      if (raw is List) {
        return raw.map((e) => PurchaseOrderModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Get purchase order detail by ID
  Future<PurchaseOrderModel?> getPurchaseDetail(int purchaseOrderId) async {
    try {
      final res = await _client.get('${ApiEndpoints.purchaseOrders}/$purchaseOrderId');
      if (res.data is Map<String, dynamic>) {
        return PurchaseOrderModel.fromJson(res.data);
      }
    } catch (_) {}
    return null;
  }

  /// Create new purchase order (inbound goods)
  Future<bool> createPurchase(PurchaseOrderModel order) async {
    try {
      final res = await _client.post(
        ApiEndpoints.purchaseOrders,
        data: order.toJson(),
      );
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return res.statusCode == 200 || res.statusCode == 201;
    } catch (_) {
      return false;
    }
  }

  /// Delete or cancel purchase order
  Future<bool> deletePurchase(int purchaseOrderId) async {
    try {
      final res = await _client.delete('${ApiEndpoints.purchaseOrders}/$purchaseOrderId');
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return res.statusCode == 200;
    } catch (_) {
      return false;
    }
  }
}
