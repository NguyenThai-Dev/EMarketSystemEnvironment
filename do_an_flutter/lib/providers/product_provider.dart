import 'package:flutter/foundation.dart';
import '../data/models/product_model.dart';
import '../data/repositories/data_repository.dart';

class ProductProvider with ChangeNotifier {
  final DataRepository _repository = DataRepository();

  List<ProductModel> _products = [];
  List<ProductCategoryModel> _categories = [];
  ProductCategoryModel? _selectedCategory;
  bool _isLoading = false;
  String? _errorMessage;
  String _keyword = '';

  List<ProductModel> get products => _products;
  List<ProductCategoryModel> get categories => _categories;
  ProductCategoryModel? get selectedCategory => _selectedCategory;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;
  String get keyword => _keyword;

  Future<void> init({int? branchId}) async {
    await loadCategories();
    await loadProducts(branchId: branchId);
  }

  Future<void> loadCategories() async {
    try {
      _categories = await _repository.getCategories();
      notifyListeners();
    } catch (_) {}
  }

  Future<void> loadProducts({int? branchId}) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      final res = await _repository.getProducts(
        start: 0,
        length: 50,
        keyword: _keyword.isNotEmpty ? _keyword : null,
        categoryId: _selectedCategory?.categoryId,
        branchId: branchId,
      );
      _products = res.data;

      // Fallback thông minh: Nếu chi nhánh/kho chưa được phân bổ tồn kho theo lô (khiến res.data rỗng),
      // tự động lấy danh mục sản phẩm chung để POS luôn hiện danh sách món gợi ý cho thu ngân bán hàng
      if (_products.isEmpty && branchId != null && branchId > 0) {
        final fallbackRes = await _repository.getProducts(
          start: 0,
          length: 50,
          keyword: _keyword.isNotEmpty ? _keyword : null,
          categoryId: _selectedCategory?.categoryId,
          branchId: null,
        );
        if (fallbackRes.data.isNotEmpty) {
          _products = fallbackRes.data;
        }
      }
    } catch (e) {
      _errorMessage = e.toString().replaceFirst('Exception: ', '');
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  void selectCategory(ProductCategoryModel? category, {int? branchId}) {
    if (_selectedCategory == category) {
      _selectedCategory = null;
    } else {
      _selectedCategory = category;
    }
    loadProducts(branchId: branchId);
  }

  void search(String query, {int? branchId}) {
    _keyword = query.trim();
    loadProducts(branchId: branchId);
  }
}
