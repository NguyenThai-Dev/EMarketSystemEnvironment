import 'package:dio/dio.dart';
import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../../core/network/api_exception.dart';
import '../models/product_model.dart';

class ProductApiService {
  final ApiClient _client = ApiClient();

  /// Get Products with Pagination and Filters
  Future<ProductPagedResponse> getProductsPaged({
    int start = 0,
    int length = 20,
    String? keyword,
    int? categoryId,
    int? branchId,
    int? warehouseId,
  }) async {
    final queryParams = <String, dynamic>{
      'start': start,
      'length': length,
      if (keyword != null && keyword.trim().isNotEmpty) 'keyword': keyword.trim(),
      if (categoryId != null && categoryId > 0) 'categoryId': categoryId,
      if (branchId != null && branchId > 0) 'branchId': branchId,
      if (warehouseId != null && warehouseId > 0) 'warehouseId': warehouseId,
    };

    try {
      final res = await _client.get(
        ApiEndpoints.pagedProducts,
        queryParameters: queryParams,
      );

      return ProductPagedResponse.fromJson(res.data);
    } catch (_) {
      return ProductPagedResponse(total: 0, filtered: 0, data: []);
    }
  }

  /// Search products by barcode or text (supports scanner, camera & manual typing)
  Future<List<ProductModel>> searchByBarcode(String barcode, {int? branchId}) async {
    final query = barcode.trim();
    if (query.isEmpty) return [];

    final queryParams = <String, dynamic>{
      'barcode': query,
      'keyword': query,
      if (branchId != null && branchId > 0) 'branchId': branchId,
    };

    // 1. Try search-simple (supports both product name and barcode)
    try {
      final res = await _client.get(
        ApiEndpoints.searchProductsSimple,
        queryParameters: queryParams,
      );

      dynamic rawList = res.data;
      if (res.data is Map<String, dynamic>) {
        rawList = res.data['data'] ?? res.data['Data'] ?? res.data['items'];
      }

      if (rawList is List && rawList.isNotEmpty) {
        return rawList.map((e) => ProductModel.fromJson(e)).toList();
      }
    } catch (_) {}

    // 2. Fallback to by-barcode
    try {
      final res = await _client.get(
        '${ApiEndpoints.allProducts}/by-barcode',
        queryParameters: queryParams,
      );

      dynamic rawList = res.data;
      if (res.data is Map<String, dynamic>) {
        rawList = res.data['data'] ?? res.data['Data'] ?? res.data['items'];
      }

      if (rawList is List) {
        return rawList.map((e) => ProductModel.fromJson(e)).toList();
      } else if (rawList is Map<String, dynamic>) {
        return [ProductModel.fromJson(rawList)];
      }
    } catch (_) {}
    return [];
  }

  /// Get Product Categories
  Future<List<ProductCategoryModel>> getCategories() async {
    try {
      final res = await _client.get(ApiEndpoints.productCategories);
      if (res.data is List) {
        return (res.data as List).map((e) => ProductCategoryModel.fromJson(e)).toList();
      }
      if (res.data is Map<String, dynamic>) {
        final list = res.data['data'] ?? res.data['Data'] ?? res.data['categories'];
        if (list is List) {
          return list.map((e) => ProductCategoryModel.fromJson(e)).toList();
        }
      }
    } catch (_) {}
    return [];
  }

  /// Get Lots for a specific product (FEFO Tracking)
  Future<List<ProductLotModel>> getProductLots(int productId) async {
    try {
      final res = await _client.get('${ApiEndpoints.productLots}/$productId');
      if (res.data is List) {
        return (res.data as List).map((e) => ProductLotModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Get Low Stock Alerts
  Future<List<ProductModel>> getLowStockAlerts({int top = 20}) async {
    try {
      final res = await _client.get(
        ApiEndpoints.lowStockAlerts,
        queryParameters: {'top': top},
      );
      if (res.data is List) {
        return (res.data as List).map((e) => ProductModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Get Suppliers list
  Future<List<SupplierModel>> getSuppliers({String? keyword}) async {
    try {
      final res = await _client.get(
        ApiEndpoints.suppliers,
        queryParameters: {
          if (keyword != null && keyword.trim().isNotEmpty) 'name': keyword.trim(),
        },
      );
      if (res.data is List) {
        return (res.data as List).map((e) => SupplierModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Create new supplier
  Future<bool> createSupplier(Map<String, dynamic> data) async {
    try {
      final res = await _client.post(ApiEndpoints.suppliers, data: data);
      return res.statusCode == 200 || res.statusCode == 201;
    } catch (_) {
      return false;
    }
  }

  /// Update existing supplier
  Future<bool> updateSupplier(Map<String, dynamic> data) async {
    try {
      final res = await _client.put(ApiEndpoints.suppliers, data: data);
      return res.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  /// Delete supplier
  Future<bool> deleteSupplier(int supplierId) async {
    try {
      final res = await _client.delete('${ApiEndpoints.suppliers}/$supplierId');
      return res.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  /// Get supplier debts
  Future<List<SupplierDebtModel>> getSupplierDebts({int? supplierId, String? status, String? keyword}) async {
    try {
      final queryParams = <String, dynamic>{
        if (supplierId != null && supplierId > 0) 'supplierId': supplierId,
        if (status != null && status.isNotEmpty && status != 'ALL') 'status': status,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
      };
      final res = await _client.get(ApiEndpoints.supplierDebtsPaged, queryParameters: queryParams);
      dynamic raw = res.data;
      if (raw is Map<String, dynamic>) {
        raw = raw['data'] ?? raw['Data'];
      }
      if (raw is List) {
        return raw.map((e) => SupplierDebtModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Get supplier debt summary KPI
  Future<SupplierDebtSummaryModel> getSupplierDebtSummary({int? supplierId}) async {
    try {
      final res = await _client.get(
        ApiEndpoints.supplierDebtSummary,
        queryParameters: {
          if (supplierId != null && supplierId > 0) 'supplierId': supplierId,
        },
      );
      if (res.data is Map<String, dynamic>) {
        return SupplierDebtSummaryModel.fromJson(res.data);
      }
    } catch (_) {}
    return SupplierDebtSummaryModel();
  }

  /// Make payment for supplier debt
  Future<bool> paySupplierDebt({
    required int debtId,
    required double amount,
    String paymentMethod = 'Tiền mặt',
    String? proof,
  }) async {
    try {
      final res = await _client.post(
        ApiEndpoints.supplierDebtPayment,
        data: {
          'DebtId': debtId,
          'Amount': amount,
          'PaymentMethod': paymentMethod,
          'PaymentProof': proof,
          'PaymentDate': DateTime.now().toIso8601String(),
        },
      );
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return res.statusCode == 200 || res.statusCode == 201;
    } catch (_) {
      return false;
    }
  }

  /// Create new product and return new productId
  Future<int?> createProduct(Map<String, dynamic> productData) async {
    try {
      final res = await _client.post(
        ApiEndpoints.createProduct,
        data: productData,
      );
      if (res.data is Map<String, dynamic>) {
        final success = res.data['success'] ?? res.data['Success'] ?? true;
        if (success == false) {
          throw Exception(res.data['message'] ?? res.data['Message'] ?? 'Không thể tạo sản phẩm.');
        }
        final pId = res.data['productId'] ?? res.data['ProductId'] ?? res.data['id'];
        if (pId != null) return int.tryParse(pId.toString());
        return 1;
      }
      return (res.statusCode == 200 || res.statusCode == 201) ? 1 : null;
    } on ApiException {
      rethrow;
    } catch (e) {
      throw Exception('Lỗi tạo sản phẩm: $e');
    }
  }

  /// Update existing product
  Future<bool> updateProduct(Map<String, dynamic> productData) async {
    try {
      final res = await _client.post(
        ApiEndpoints.updateProduct,
        data: productData,
      );
      if (res.data is Map<String, dynamic>) {
        final success = res.data['success'] ?? res.data['Success'] ?? true;
        if (success == false) {
          throw Exception(res.data['message'] ?? res.data['Message'] ?? 'Không thể cập nhật sản phẩm.');
        }
        return true;
      }
      return res.statusCode == 200;
    } on ApiException {
      rethrow;
    } catch (e) {
      throw Exception('Lỗi cập nhật sản phẩm: $e');
    }
  }

  /// Delete product
  Future<bool> deleteProduct(int productId) async {
    try {
      final res = await _client.post('${ApiEndpoints.deleteProduct}/$productId');
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return res.statusCode == 200;
    } on ApiException {
      rethrow;
    } catch (e) {
      throw Exception('Lỗi xóa sản phẩm: $e');
    }
  }

  /// Upload product image file via multipart form
  Future<String?> uploadProductImage({required String filePath, String? fileName, int? productId}) async {
    try {
      final formData = FormData();
      formData.files.add(MapEntry(
        'file',
        await MultipartFile.fromFile(filePath, filename: fileName ?? 'product.jpg'),
      ));
      if (productId != null && productId > 0) {
        formData.fields.add(MapEntry('productId', productId.toString()));
      }

      final res = await _client.post(
        ApiEndpoints.uploadProductImage,
        data: formData,
      );

      if (res.data is Map<String, dynamic>) {
        final url = res.data['url'] ?? res.data['Image'] ?? res.data['data'];
        if (url != null) return url.toString();
      }
      return null;
    } on ApiException {
      rethrow;
    } catch (e) {
      throw Exception('Lỗi tải ảnh sản phẩm: $e');
    }
  }
}
