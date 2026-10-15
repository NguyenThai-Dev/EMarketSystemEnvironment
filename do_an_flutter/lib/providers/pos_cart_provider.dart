import 'package:flutter/foundation.dart';
import '../data/models/cart_item_model.dart';
import '../data/models/order_model.dart';
import '../data/models/product_model.dart';
import '../data/repositories/data_repository.dart';

import '../data/models/customer_model.dart';

class PosCartProvider with ChangeNotifier {
  final DataRepository _repository = DataRepository();

  final Map<int, CartItemModel> _items = {};
  double _orderDiscount = 0.0;
  String _paymentMethod = 'Cash'; // 'Cash' | 'VietQR'
  String? _note;
  int? _customerId;
  CustomerModel? _selectedCustomer;
  int _pointsUsed = 0;
  bool _isCheckingOut = false;
  String? _checkoutError;

  List<CartItemModel> get items => _items.values.toList();
  int get itemCount => _items.values.fold(0, (sum, i) => sum + i.quantity);
  double get orderDiscount => _orderDiscount;
  String get paymentMethod => _paymentMethod;
  String? get note => _note;
  int? get customerId => _customerId;
  CustomerModel? get selectedCustomer => _selectedCustomer;
  int get pointsUsed => _pointsUsed;
  bool get isCheckingOut => _isCheckingOut;
  String? get checkoutError => _checkoutError;

  double get subtotal => _items.values.fold(0.0, (sum, i) => sum + i.subtotal);
  double get pointsDiscount => (_pointsUsed * 1000).toDouble(); // 1 điểm = 1.000 VNĐ
  double get totalDiscount => _orderDiscount + pointsDiscount + _items.values.fold(0.0, (sum, i) => sum + i.discount);
  double get finalTotal => (subtotal - totalDiscount).clamp(0.0, double.infinity);

  bool get isEmpty => _items.isEmpty;

  void selectCustomer(CustomerModel? customer) {
    _selectedCustomer = customer;
    _customerId = customer?.customerId;
    _pointsUsed = 0;
    // Tự động áp dụng chiết khấu hạng thành viên nếu có
    if (customer != null && customer.discountRate > 0) {
      _orderDiscount = subtotal * customer.discountRate;
    }
    notifyListeners();
  }

  void setPointsUsed(int points) {
    if (_selectedCustomer == null) return;
    _pointsUsed = points.clamp(0, _selectedCustomer!.pointBalance);
    notifyListeners();
  }

  void addToCart(ProductModel product, {int quantity = 1}) {
    if (_items.containsKey(product.productId)) {
      _items[product.productId]!.increment(quantity);
    } else {
      _items[product.productId] = CartItemModel(
        product: product,
        quantity: quantity,
      );
    }
    notifyListeners();
  }

  void updateQuantity(int productId, int delta) {
    if (!_items.containsKey(productId)) return;
    if (delta > 0) {
      _items[productId]!.increment(delta);
    } else if (delta < 0) {
      if (_items[productId]!.quantity <= 1) {
        removeFromCart(productId);
        return;
      }
      _items[productId]!.decrement(-delta);
    }
    notifyListeners();
  }

  void removeFromCart(int productId) {
    _items.remove(productId);
    notifyListeners();
  }

  void clearCart() {
    _items.clear();
    _orderDiscount = 0.0;
    _note = null;
    _customerId = null;
    _selectedCustomer = null;
    _pointsUsed = 0;
    _checkoutError = null;
    notifyListeners();
  }

  void applyDiscount(double amount) {
    _orderDiscount = amount.clamp(0.0, subtotal);
    notifyListeners();
  }

  void setPaymentMethod(String method) {
    _paymentMethod = method;
    notifyListeners();
  }

  void setNote(String? noteText) {
    _note = noteText;
  }

  void setCustomer(int? id) {
    _customerId = id;
    notifyListeners();
  }

  /// Add product by barcode scan matching
  bool addByBarcode(String barcode, List<ProductModel> catalog) {
    final cleanBarcode = barcode.trim().toLowerCase();
    for (final prod in catalog) {
      if (prod.sku.toLowerCase() == cleanBarcode) {
        addToCart(prod);
        return true;
      }
    }
    return false;
  }

  /// Create real PayOS QR payment link via backend
  Future<CreateQrResponse> createPayOsPayment(int amount, {String? description}) async {
    final desc = description ?? 'Thanh toan POS';
    return await _repository.createPayOsLink(CreateQrRequest(amount: amount, description: desc));
  }

  /// Check payment status via backend
  Future<PayOsStatusResponse> checkPayOsStatus(int orderCode) async {
    return await _repository.checkPayOsStatus(orderCode);
  }

  /// Execute checkout via EMarket Web API
  Future<CheckoutResult?> checkout({
    int? branchId,
    int? userId,
    String? connectionId,
  }) async {
    if (isEmpty) {
      _checkoutError = 'Giỏ hàng đang trống.';
      notifyListeners();
      return null;
    }

    _isCheckingOut = true;
    _checkoutError = null;
    notifyListeners();

    try {
      final calculatedPointsEarned = (finalTotal / 1000).floor();

      final request = CheckoutRequest(
        userId: userId,
        branchId: branchId,
        customerId: (_customerId != null && _customerId! > 0) ? _customerId : 1,
        pointsUsed: _pointsUsed,
        pointsEarned: calculatedPointsEarned,
        totalAmount: finalTotal,
        manualDiscount: _orderDiscount + pointsDiscount,
        manualDiscountReason: _note,
        status: 'Completed',
        paymentMethod: _paymentMethod,
        connectionId: connectionId,
        items: _items.values.map((i) {
          return CheckoutItemRequest(
            productId: i.product.productId,
            quantity: i.quantity,
            price: i.price,
            discount: i.discount,
          );
        }).toList(),
      );

      final result = await _repository.checkout(request);
      if (result.success) {
        clearCart();
      } else {
        _checkoutError = result.message ?? 'Tạo đơn hàng không thành công.';
      }
      _isCheckingOut = false;
      notifyListeners();
      return result;
    } catch (e) {
      _checkoutError = e.toString().replaceFirst('Exception: ', '');
      _isCheckingOut = false;
      notifyListeners();
      return null;
    }
  }
}
