/// Maps to WarehouseDTO in EMarket.Modules.InventoryModule.DTOs
class WarehouseModel {
  final int warehouseId;
  final int branchId;
  final String name;
  final String? address;
  final String? branchName;

  WarehouseModel({
    required this.warehouseId,
    required this.branchId,
    required this.name,
    this.address,
    this.branchName,
  });

  factory WarehouseModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return WarehouseModel(warehouseId: 0, branchId: 0, name: raw?.toString() ?? 'Kho hàng');
    }
    return WarehouseModel(
      warehouseId: (raw['warehouseId'] ?? raw['WarehouseId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      branchId: (raw['branchId'] ?? raw['BranchId'] as num?)?.toInt() ?? 0,
      name: (raw['name'] ?? raw['Name'] ?? 'Kho hàng').toString(),
      address: raw['address'] ?? raw['Address'],
      branchName: raw['branchName'] ?? raw['BranchName'],
    );
  }

  Map<String, dynamic> toJson() => {
    'WarehouseId': warehouseId,
    'BranchId': branchId,
    'Name': name,
    'Address': address,
    'BranchName': branchName,
  };
}

/// Maps to StockMovementDTO in EMarket.Modules.InventoryModule.DTOs
class StockMovementModel {
  final int movementId;
  final int productId;
  final String productName;
  final String? productImage;
  final String? barcode;
  final String? reason;
  final int? warehouseId;
  final String? warehouseName;
  final String movementType;
  final int quantity;
  final DateTime movementDate;
  final String? userName;

  StockMovementModel({
    required this.movementId,
    required this.productId,
    required this.productName,
    this.productImage,
    this.barcode,
    this.reason,
    this.warehouseId,
    this.warehouseName,
    required this.movementType,
    required this.quantity,
    required this.movementDate,
    this.userName,
  });

  factory StockMovementModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return StockMovementModel(
        movementId: 0,
        productId: 0,
        productName: 'Sản phẩm',
        movementType: 'ADJUSTMENT',
        quantity: 0,
        movementDate: DateTime.now(),
      );
    }

    DateTime parseDate(dynamic d) {
      if (d == null) return DateTime.now();
      try {
        return DateTime.parse(d.toString());
      } catch (_) {
        return DateTime.now();
      }
    }

    return StockMovementModel(
      movementId: (raw['movementId'] ?? raw['MovementId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      productId: (raw['productId'] ?? raw['ProductId'] as num?)?.toInt() ?? 0,
      productName: (raw['productName'] ?? raw['ProductName'] ?? 'Sản phẩm').toString(),
      productImage: raw['productImage'] ?? raw['ProductImage'] ?? raw['image'] ?? raw['Image'],
      barcode: raw['barcode'] ?? raw['Barcode'],
      reason: raw['reason'] ?? raw['Reason'],
      warehouseId: (raw['warehouseId'] ?? raw['WarehouseId'] as num?)?.toInt(),
      warehouseName: raw['warehouseName'] ?? raw['WarehouseName'],
      movementType: (raw['movementType'] ?? raw['MovementType'] ?? 'ADJUSTMENT').toString(),
      quantity: (raw['quantity'] ?? raw['Quantity'] as num?)?.toInt() ?? 0,
      movementDate: parseDate(raw['movementDate'] ?? raw['MovementDate'] ?? raw['createdAt']),
      userName: raw['userName'] ?? raw['UserName'],
    );
  }

  Map<String, dynamic> toJson() => {
    'MovementId': movementId,
    'ProductId': productId,
    'ProductName': productName,
    'ProductImage': productImage,
    'Barcode': barcode,
    'Reason': reason,
    'WarehouseId': warehouseId,
    'WarehouseName': warehouseName,
    'MovementType': movementType,
    'Quantity': quantity,
    'MovementDate': movementDate.toIso8601String(),
    'UserName': userName,
  };
}

/// Maps to StockAdjustmentDTO in EMarket.Modules.InventoryModule.DTOs
class StockAdjustmentRequest {
  final String movementType;
  final int warehouseId;
  final int productId;
  final double quantityChange;
  final String reason;
  final int userId;

  StockAdjustmentRequest({
    this.movementType = 'ADJUSTMENT',
    required this.warehouseId,
    required this.productId,
    required this.quantityChange,
    required this.reason,
    required this.userId,
  });

  Map<String, dynamic> toJson() => {
    'MovementType': movementType,
    'WarehouseId': warehouseId,
    'ProductId': productId,
    'QuantityChange': quantityChange,
    'Reason': reason,
    'UserId': userId,
  };
}

/// Item in warehouse inventory
class InventoryItemModel {
  final int inventoryId;
  final int productId;
  final String productName;
  final String? productImage;
  final String? barcode;
  final String? unit;
  final String? batchCode;
  final int warehouseId;
  final String? warehouseName;
  final int quantity;
  final double? price;

  InventoryItemModel({
    required this.inventoryId,
    required this.productId,
    required this.productName,
    this.productImage,
    this.barcode,
    this.unit,
    this.batchCode,
    required this.warehouseId,
    this.warehouseName,
    required this.quantity,
    this.price,
  });

  factory InventoryItemModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return InventoryItemModel(
        inventoryId: 0,
        productId: 0,
        productName: 'Sản phẩm',
        warehouseId: 0,
        quantity: 0,
      );
    }
    return InventoryItemModel(
      inventoryId: (raw['inventoryId'] ?? raw['InventoryId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      productId: (raw['productId'] ?? raw['ProductId'] as num?)?.toInt() ?? 0,
      productName: (raw['productName'] ?? raw['ProductName'] ?? raw['name'] ?? 'Sản phẩm').toString(),
      productImage: raw['productImage'] ?? raw['ProductImage'] ?? raw['image'] ?? raw['Image'],
      barcode: raw['barcode'] ?? raw['Barcode'] ?? raw['sku'],
      unit: raw['unit'] ?? raw['Unit'],
      batchCode: raw['batchCode'] ?? raw['BatchCode'],
      warehouseId: (raw['warehouseId'] ?? raw['WarehouseId'] as num?)?.toInt() ?? 0,
      warehouseName: raw['warehouseName'] ?? raw['WarehouseName'],
      quantity: (raw['quantity'] ?? raw['Quantity'] ?? raw['stock'] as num?)?.toInt() ?? 0,
      price: (raw['price'] ?? raw['Price'] as num?)?.toDouble(),
    );
  }
}

/// Maps to PurchaseOrderDTO in EMarket.Modules.InventoryModule.DTOs
class PurchaseOrderModel {
  final int purchaseOrderId;
  final int supplierId;
  final String? supplierName;
  final int warehouseId;
  final String? warehouseName;
  final int userId;
  final String? userName;
  final int branchId;
  final String? branchName;
  final DateTime? orderDate;
  final String status;
  final double totalAmount;
  final double paidAmount;
  final double dueAmount;
  final String paymentStatus;
  final String? notes;
  final List<PurchaseOrderDetailModel> details;

  PurchaseOrderModel({
    required this.purchaseOrderId,
    required this.supplierId,
    this.supplierName,
    required this.warehouseId,
    this.warehouseName,
    required this.userId,
    this.userName,
    required this.branchId,
    this.branchName,
    this.orderDate,
    required this.status,
    required this.totalAmount,
    this.paidAmount = 0,
    this.dueAmount = 0,
    this.paymentStatus = 'UNPAID',
    this.notes,
    this.details = const [],
  });

  factory PurchaseOrderModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return PurchaseOrderModel(
        purchaseOrderId: 0,
        supplierId: 0,
        warehouseId: 0,
        userId: 0,
        branchId: 0,
        status: 'PENDING',
        totalAmount: 0,
      );
    }

    DateTime? parseDate(dynamic d) {
      if (d == null) return null;
      try {
        return DateTime.parse(d.toString());
      } catch (_) {
        return null;
      }
    }

    final rawDetails = raw['details'] ?? raw['Details'];
    List<PurchaseOrderDetailModel> detailsList = [];
    if (rawDetails is List) {
      detailsList = rawDetails.map((e) => PurchaseOrderDetailModel.fromJson(e)).toList();
    }

    return PurchaseOrderModel(
      purchaseOrderId: (raw['purchaseOrderId'] ?? raw['PurchaseOrderId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      supplierId: (raw['supplierId'] ?? raw['SupplierId'] as num?)?.toInt() ?? 0,
      supplierName: raw['supplierName'] ?? raw['SupplierName'],
      warehouseId: (raw['warehouseId'] ?? raw['WarehouseId'] as num?)?.toInt() ?? 0,
      warehouseName: raw['warehouseName'] ?? raw['WarehouseName'],
      userId: (raw['userId'] ?? raw['UserId'] as num?)?.toInt() ?? 0,
      userName: raw['userName'] ?? raw['UserName'],
      branchId: (raw['branchId'] ?? raw['BranchId'] as num?)?.toInt() ?? 0,
      branchName: raw['branchName'] ?? raw['BranchName'],
      orderDate: parseDate(raw['orderDate'] ?? raw['OrderDate'] ?? raw['createdAt']),
      status: (raw['status'] ?? raw['Status'] ?? 'PENDING').toString(),
      totalAmount: (raw['totalAmount'] ?? raw['TotalAmount'] as num?)?.toDouble() ?? 0,
      paidAmount: (raw['paidAmount'] ?? raw['PaidAmount'] as num?)?.toDouble() ?? 0,
      dueAmount: (raw['dueAmount'] ?? raw['DueAmount'] as num?)?.toDouble() ?? 0,
      paymentStatus: (raw['paymentStatus'] ?? raw['PaymentStatus'] ?? 'UNPAID').toString(),
      notes: raw['notes'] ?? raw['Notes'],
      details: detailsList,
    );
  }

  Map<String, dynamic> toJson() => {
    'PurchaseOrderId': purchaseOrderId,
    'SupplierId': supplierId,
    'WarehouseId': warehouseId,
    'UserId': userId,
    'BranchId': branchId,
    if (orderDate != null) 'OrderDate': orderDate!.toIso8601String(),
    'Status': status,
    'TotalAmount': totalAmount,
    'PaymentStatus': paymentStatus,
    'Notes': notes,
    'Details': details.map((d) => d.toJson()).toList(),
  };
}

/// Maps to PurchaseOrderDetailDTO in EMarket.Modules.InventoryModule.DTOs
class PurchaseOrderDetailModel {
  final int purchaseOrderDetailId;
  final int purchaseOrderId;
  final int productId;
  final String productName;
  final int categoryId;
  final int lotId;
  final int quantity;
  final String? unit;
  final double unitPrice;
  final double totalPrice;

  PurchaseOrderDetailModel({
    required this.purchaseOrderDetailId,
    required this.purchaseOrderId,
    required this.productId,
    required this.productName,
    this.categoryId = 0,
    this.lotId = 0,
    required this.quantity,
    this.unit,
    required this.unitPrice,
    required this.totalPrice,
  });

  factory PurchaseOrderDetailModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return PurchaseOrderDetailModel(
        purchaseOrderDetailId: 0,
        purchaseOrderId: 0,
        productId: 0,
        productName: 'Sản phẩm',
        quantity: 0,
        unitPrice: 0,
        totalPrice: 0,
      );
    }
    return PurchaseOrderDetailModel(
      purchaseOrderDetailId: (raw['purchaseOrderDetailId'] ?? raw['PurchaseOrderDetailId'] as num?)?.toInt() ?? 0,
      purchaseOrderId: (raw['purchaseOrderId'] ?? raw['PurchaseOrderId'] as num?)?.toInt() ?? 0,
      productId: (raw['productId'] ?? raw['ProductId'] as num?)?.toInt() ?? 0,
      productName: (raw['productName'] ?? raw['ProductName'] ?? 'Sản phẩm').toString(),
      categoryId: (raw['categoryId'] ?? raw['CategoryId'] as num?)?.toInt() ?? 0,
      lotId: (raw['lotId'] ?? raw['LotId'] as num?)?.toInt() ?? 0,
      quantity: (raw['quantity'] ?? raw['Quantity'] as num?)?.toInt() ?? 0,
      unit: raw['unit'] ?? raw['Unit'],
      unitPrice: (raw['unitPrice'] ?? raw['UnitPrice'] as num?)?.toDouble() ?? 0,
      totalPrice: (raw['totalPrice'] ?? raw['TotalPrice'] as num?)?.toDouble() ?? 0,
    );
  }

  Map<String, dynamic> toJson() => {
    'PurchaseOrderDetailId': purchaseOrderDetailId,
    'PurchaseOrderId': purchaseOrderId,
    'ProductId': productId,
    'ProductName': productName,
    'CategoryId': categoryId,
    'LotId': lotId,
    'Quantity': quantity,
    'Unit': unit,
    'UnitPrice': unitPrice,
    'TotalPrice': totalPrice,
  };
}
