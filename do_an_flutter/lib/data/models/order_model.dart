class OrderDetailModel {
  final int? orderDetailId;
  final int productId;
  final String productName;
  final int quantity;
  final double unitPrice;
  final double discount;
  final double totalPrice;

  OrderDetailModel({
    this.orderDetailId,
    required this.productId,
    required this.productName,
    required this.quantity,
    required this.unitPrice,
    this.discount = 0.0,
    required this.totalPrice,
  });

  factory OrderDetailModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return OrderDetailModel(productId: 0, productName: '', quantity: 1, unitPrice: 0.0, totalPrice: 0.0);
    }
    return OrderDetailModel(
      orderDetailId: (raw['orderDetailId'] ?? raw['OrderDetailId'] as num?)?.toInt(),
      productId: (raw['productId'] ?? raw['ProductId'] as num?)?.toInt() ?? 0,
      productName: (raw['productName'] ?? raw['ProductName'] ?? '').toString(),
      quantity: (raw['quantity'] ?? raw['Quantity'] as num?)?.toInt() ?? 1,
      unitPrice: (raw['unitPrice'] ?? raw['UnitPrice'] ?? raw['price'] as num?)?.toDouble() ?? 0.0,
      discount: (raw['discount'] ?? raw['Discount'] as num?)?.toDouble() ?? 0.0,
      totalPrice: (raw['totalPrice'] ?? raw['TotalPrice'] ?? raw['total'] as num?)?.toDouble() ?? 0.0,
    );
  }

  Map<String, dynamic> toJson() => {
    'orderDetailId': orderDetailId,
    'productId': productId,
    'productName': productName,
    'quantity': quantity,
    'unitPrice': unitPrice,
    'discount': discount,
    'totalPrice': totalPrice,
  };
}

class OrderModel {
  final int orderId;
  final String orderCode;
  final DateTime? orderDate;
  final double totalAmount;
  final String status;
  final String? customerName;
  final String? paymentMethod;
  final String? note;
  final List<OrderDetailModel> details;

  OrderModel({
    required this.orderId,
    required this.orderCode,
    this.orderDate,
    required this.totalAmount,
    required this.status,
    this.customerName,
    this.paymentMethod,
    this.note,
    this.details = const [],
  });

  factory OrderModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return OrderModel(orderId: 0, orderCode: 'ORD-#', totalAmount: 0.0, status: 'Completed');
    }

    DateTime? parseDate(dynamic d) {
      if (d == null) return null;
      try {
        return DateTime.parse(d.toString());
      } catch (_) {
        return null;
      }
    }

    List<OrderDetailModel> parseDetails() {
      final list = raw['details'] ?? raw['Details'] ?? raw['items'] ?? raw['Items'];
      if (list != null && list is List) {
        return list.map((e) => OrderDetailModel.fromJson(e)).toList();
      }
      return [];
    }

    int parseId(dynamic m) {
      final v = m['orderId'] ?? m['OrderId'] ?? m['order_id'] ?? m['id'] ?? m['Id'];
      if (v == null) return 0;
      if (v is num) return v.toInt();
      return int.tryParse(v.toString()) ?? 0;
    }

    final id = parseId(raw);
    final code = raw['orderCode'] ?? raw['OrderCode'] ?? raw['order_code'] ?? raw['code'] ?? raw['Code'];
    final effectiveCode = (code != null && code.toString().trim().isNotEmpty && !code.toString().contains('ORD-#'))
        ? code.toString()
        : (id > 0 ? '#ORD-$id' : '#ORD-${raw.hashCode.abs() % 100000}');

    return OrderModel(
      orderId: id,
      orderCode: effectiveCode,
      orderDate: parseDate(raw['orderDate'] ?? raw['OrderDate'] ?? raw['createdAt']),
      totalAmount: (raw['totalAmount'] ?? raw['TotalAmount'] ?? raw['total'] as num?)?.toDouble() ?? 0.0,
      status: (raw['status'] ?? raw['Status'] ?? 'Completed').toString(),
      customerName: raw['customerName'] ?? raw['CustomerName'],
      paymentMethod: raw['paymentMethod'] ?? raw['PaymentMethod'],
      note: raw['note'] ?? raw['Note'],
      details: parseDetails(),
    );
  }

  Map<String, dynamic> toJson() => {
    'orderId': orderId,
    'orderCode': orderCode,
    'orderDate': orderDate?.toIso8601String(),
    'totalAmount': totalAmount,
    'status': status,
    'customerName': customerName,
    'paymentMethod': paymentMethod,
    'note': note,
    'details': details.map((e) => e.toJson()).toList(),
  };
}

/// Maps directly to CartItemDTO in EMarket.Modules.SalesModule.DTOs
class CheckoutItemRequest {
  final int productId;
  final int quantity;
  final double price;
  final double discount;

  CheckoutItemRequest({
    required this.productId,
    required this.quantity,
    required this.price,
    this.discount = 0.0,
  });

  Map<String, dynamic> toJson() => {
    'ProductId': productId,
    'Quantity': quantity,
    'Price': price,
    'Discount': discount,
  };
}

/// Maps directly to CheckoutRequestDTO in EMarket.Modules.SalesModule.DTOs
class CheckoutRequest {
  final int? userId;
  final int? branchId;
  final int? customerId;
  final int pointsUsed;
  final int pointsEarned;
  final double totalAmount;
  final double manualDiscount;
  final String? manualDiscountReason;
  final int? parentOrderId;
  final String status;
  final String paymentMethod;
  final String? connectionId;
  final List<CheckoutItemRequest> items;

  CheckoutRequest({
    this.userId,
    this.branchId,
    this.customerId,
    this.pointsUsed = 0,
    this.pointsEarned = 0,
    required this.totalAmount,
    this.manualDiscount = 0.0,
    this.manualDiscountReason,
    this.parentOrderId,
    this.status = 'Completed',
    required this.paymentMethod,
    this.connectionId,
    required this.items,
  });

  Map<String, dynamic> toJson() => {
    'UserId': userId,
    'BranchId': branchId,
    'CustomerId': customerId,
    'PointsUsed': pointsUsed,
    'PointsEarned': pointsEarned,
    'TotalAmount': totalAmount,
    'ManualDiscount': manualDiscount,
    'ManualDiscountReason': manualDiscountReason ?? '',
    'ParentOrderId': parentOrderId,
    'Status': status,
    'PaymentMethod': paymentMethod,
    'ConnectionId': connectionId ?? '',
    'Items': items.map((e) => e.toJson()).toList(),
  };
}

/// Maps directly to CheckoutResultDTO in EMarket.Modules.SalesModule.DTOs
class CheckoutResult {
  final bool success;
  final int? orderId;
  final String? orderCode;
  final String? message;

  CheckoutResult({
    required this.success,
    this.orderId,
    this.orderCode,
    this.message,
  });

  factory CheckoutResult.fromJson(Map<String, dynamic> json) {
    return CheckoutResult(
      success: json['success'] ?? json['Success'] ?? true,
      orderId: json['orderId'] ?? json['OrderId'],
      orderCode: json['orderCode'] ?? json['OrderCode'] ?? (json['orderId'] != null ? '#ORD-${json['orderId']}' : null),
      message: json['message'] ?? json['Message'],
    );
  }
}

/// Maps directly to CreateQrRequestDTO in EMarket.Modules.SalesModule.DTOs
class CreateQrRequest {
  final int amount;
  final String description;

  CreateQrRequest({
    required this.amount,
    required this.description,
  });

  Map<String, dynamic> toJson() => {
    'Amount': amount,
    'Description': description,
  };
}

/// Maps directly to CreateQrResponseDTO in EMarket.Modules.SalesModule.DTOs
class CreateQrResponse {
  final bool success;
  final String? checkoutUrl;
  final String? qrCode;
  final int? orderCode;
  final String? message;

  CreateQrResponse({
    required this.success,
    this.checkoutUrl,
    this.qrCode,
    this.orderCode,
    this.message,
  });

  factory CreateQrResponse.fromJson(Map<String, dynamic> json) {
    int? parseOrderCode(dynamic v) {
      if (v == null) return null;
      if (v is int) return v;
      return int.tryParse(v.toString());
    }

    return CreateQrResponse(
      success: json['success'] ?? json['Success'] ?? false,
      checkoutUrl: json['checkoutUrl'] ?? json['CheckoutUrl'],
      qrCode: json['qrCode'] ?? json['QrCode'],
      orderCode: parseOrderCode(json['orderCode'] ?? json['OrderCode']),
      message: json['message'] ?? json['Message'],
    );
  }
}

/// Response from api/admin/sales/orders/check-payos-status/{orderCode}
class PayOsStatusResponse {
  final bool success;
  final bool isPaid;
  final int? orderCode;

  PayOsStatusResponse({
    required this.success,
    required this.isPaid,
    this.orderCode,
  });

  factory PayOsStatusResponse.fromJson(Map<String, dynamic> json) {
    int? parseOrderCode(dynamic v) {
      if (v == null) return null;
      if (v is int) return v;
      return int.tryParse(v.toString());
    }

    return PayOsStatusResponse(
      success: json['success'] ?? json['Success'] ?? false,
      isPaid: json['isPaid'] ?? json['IsPaid'] ?? false,
      orderCode: parseOrderCode(json['orderCode'] ?? json['OrderCode']),
    );
  }
}
