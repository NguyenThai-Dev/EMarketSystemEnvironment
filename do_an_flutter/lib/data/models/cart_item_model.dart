import 'product_model.dart';

class CartItemModel {
  final ProductModel product;
  int quantity;
  double price;
  double discount;

  CartItemModel({
    required this.product,
    this.quantity = 1,
    double? price,
    this.discount = 0.0,
  }) : price = price ?? product.price;

  double get subtotal => price * quantity;
  double get total => (subtotal - discount).clamp(0, double.infinity);

  void increment([int count = 1]) {
    quantity += count;
  }

  void decrement([int count = 1]) {
    if (quantity > count) {
      quantity -= count;
    } else {
      quantity = 1;
    }
  }
}
