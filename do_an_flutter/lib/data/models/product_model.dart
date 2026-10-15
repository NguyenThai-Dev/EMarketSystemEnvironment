import '../../core/utils/image_helper.dart';

class ProductCategoryModel {
  final int categoryId;
  final String categoryName;
  final String? description;

  ProductCategoryModel({
    required this.categoryId,
    required this.categoryName,
    this.description,
  });

  factory ProductCategoryModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return ProductCategoryModel(categoryId: 0, categoryName: raw?.toString() ?? '');
    }
    return ProductCategoryModel(
      categoryId: (raw['categoryId'] ?? raw['CategoryId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      categoryName: (raw['categoryName'] ?? raw['CategoryName'] ?? raw['name'] ?? raw['Name'] ?? '').toString(),
      description: raw['description'] ?? raw['Description'],
    );
  }

  Map<String, dynamic> toJson() => {
    'categoryId': categoryId,
    'categoryName': categoryName,
    'description': description,
  };
}

class ProductModel {
  final int productId;
  final String name;
  final String sku;
  final double price;
  final double costPrice;
  final int stockQuantity;
  final String? imageUrl;
  final String? unit;
  final String? description;
  final String? categoryName;
  final int? categoryId;
  final String? supplierName;

  ProductModel({
    required this.productId,
    required this.name,
    required this.sku,
    required this.price,
    this.costPrice = 0.0,
    required this.stockQuantity,
    this.imageUrl,
    this.unit,
    this.description,
    this.categoryName,
    this.categoryId,
    this.supplierName,
  });

  int get quantity => stockQuantity;
  String get productName => name;
  double get retailPrice => price;

  String? get fullImageUrl => ImageHelper.getFullImageUrl(imageUrl);

  factory ProductModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return ProductModel(
        productId: 0,
        name: raw?.toString() ?? '',
        sku: '',
        price: 0.0,
        stockQuantity: 0,
      );
    }
    return ProductModel(
      productId: (raw['productId'] ?? raw['ProductId'] ?? raw['product_id'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      name: (raw['name'] ?? raw['Name'] ?? raw['productName'] ?? raw['ProductName'] ?? '').toString(),
      sku: (raw['barcode'] ?? raw['Barcode'] ?? raw['sku'] ?? raw['Sku'] ?? raw['SKU'] ?? '').toString(),
      price: (raw['price'] ?? raw['Price'] ?? raw['finalPrice'] ?? raw['FinalPrice'] ?? raw['salePrice'] ?? raw['SalePrice'] as num?)?.toDouble() ?? 0.0,
      costPrice: (raw['costPrice'] ?? raw['CostPrice'] ?? raw['originalPrice'] ?? raw['OriginalPrice'] as num?)?.toDouble() ?? 0.0,
      stockQuantity: (raw['quantity'] ?? raw['Quantity'] ?? raw['stockQuantity'] ?? raw['StockQuantity'] ?? raw['stock'] ?? raw['Stock'] as num?)?.toInt() ?? 0,
      imageUrl: raw['image'] ?? raw['Image'] ?? raw['imageUrl'] ?? raw['ImageUrl'] ?? raw['productImage'],
      unit: raw['unit'] ?? raw['Unit'],
      description: raw['description'] ?? raw['Description'],
      categoryName: raw['categoryName'] ?? raw['CategoryName'],
      categoryId: (raw['categoryId'] ?? raw['CategoryId'] as num?)?.toInt(),
      supplierName: raw['supplierName'] ?? raw['SupplierName'],
    );
  }

  Map<String, dynamic> toJson() => {
    'productId': productId,
    'name': name,
    'sku': sku,
    'price': price,
    'costPrice': costPrice,
    'stockQuantity': stockQuantity,
    'imageUrl': imageUrl,
    'unit': unit,
    'description': description,
    'categoryName': categoryName,
    'categoryId': categoryId,
    'supplierName': supplierName,
  };
}

class ProductPagedResponse {
  final int total;
  final int filtered;
  final List<ProductModel> data;

  ProductPagedResponse({
    required this.total,
    required this.filtered,
    required this.data,
  });

  factory ProductPagedResponse.fromJson(dynamic json) {
    if (json is List) {
      final list = json.map((e) => ProductModel.fromJson(e)).toList();
      return ProductPagedResponse(total: list.length, filtered: list.length, data: list);
    }
    if (json is Map<String, dynamic>) {
      final list = (json['data'] ?? json['Data'] ?? json['items'] ?? json['Items'] ?? []) as List;
      return ProductPagedResponse(
        total: (json['total'] ?? json['Total'] as num?)?.toInt() ?? list.length,
        filtered: (json['filtered'] ?? json['Filtered'] as num?)?.toInt() ?? list.length,
        data: list.map((e) => ProductModel.fromJson(e)).toList(),
      );
    }
    return ProductPagedResponse(total: 0, filtered: 0, data: []);
  }
}

/// Maps to ProductLotDTO in backend EMarket.Modules.ProductModule.DTOs
class ProductLotModel {
  final int lotId;
  final int productId;
  final DateTime expiryDate;
  final String batchCode;
  final double? costPrice;
  final DateTime? manufacturingDate;

  ProductLotModel({
    required this.lotId,
    required this.productId,
    required this.expiryDate,
    required this.batchCode,
    this.costPrice,
    this.manufacturingDate,
  });

  bool get isExpired => expiryDate.isBefore(DateTime.now());
  bool get isNearExpired => !isExpired && expiryDate.difference(DateTime.now()).inDays <= 30;
  int get daysRemaining => expiryDate.difference(DateTime.now()).inDays;

  factory ProductLotModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return ProductLotModel(
        lotId: 0,
        productId: 0,
        expiryDate: DateTime.now(),
        batchCode: '',
      );
    }

    DateTime parseDate(dynamic d) {
      if (d == null) return DateTime.now().add(const Duration(days: 365));
      try {
        return DateTime.parse(d.toString());
      } catch (_) {
        return DateTime.now().add(const Duration(days: 365));
      }
    }

    DateTime? parseNullableDate(dynamic d) {
      if (d == null) return null;
      try {
        return DateTime.parse(d.toString());
      } catch (_) {
        return null;
      }
    }

    return ProductLotModel(
      lotId: (raw['lotId'] ?? raw['LotId'] as num?)?.toInt() ?? 0,
      productId: (raw['productId'] ?? raw['ProductId'] as num?)?.toInt() ?? 0,
      expiryDate: parseDate(raw['expiryDate'] ?? raw['ExpiryDate']),
      batchCode: (raw['batchCode'] ?? raw['BatchCode'] ?? 'LOT-#').toString(),
      costPrice: (raw['costPrice'] ?? raw['CostPrice'] as num?)?.toDouble(),
      manufacturingDate: parseNullableDate(raw['manufacturingDate'] ?? raw['ManufacturingDate']),
    );
  }

  Map<String, dynamic> toJson() => {
    'LotId': lotId,
    'ProductId': productId,
    'ExpiryDate': expiryDate.toIso8601String(),
    'BatchCode': batchCode,
    'CostPrice': costPrice,
    'ManufacturingDate': manufacturingDate?.toIso8601String(),
  };
}

/// Maps to SupplierDTO in backend EMarket.Modules.ProductModule.DTOs
class SupplierModel {
  final int supplierId;
  final String name;
  final String? email;
  final String? phone;
  final String? address;
  final String? contactPerson;
  final double debtAmount;

  SupplierModel({
    required this.supplierId,
    required this.name,
    this.email,
    this.phone,
    this.address,
    this.contactPerson,
    this.debtAmount = 0.0,
  });

  String? get contactName => contactPerson;

  factory SupplierModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return SupplierModel(supplierId: 0, name: raw?.toString() ?? '');
    }
    return SupplierModel(
      supplierId: (raw['supplierId'] ?? raw['SupplierId'] as num?)?.toInt() ?? 0,
      name: (raw['name'] ?? raw['Name'] ?? '').toString(),
      email: raw['email'] ?? raw['Email'],
      phone: raw['phone'] ?? raw['Phone'],
      address: raw['address'] ?? raw['Address'],
      contactPerson: raw['contactPerson'] ?? raw['ContactPerson'],
      debtAmount: (raw['debtAmount'] ?? raw['DebtAmount'] ?? raw['unpaidAmount'] ?? raw['UnpaidAmount'] as num?)?.toDouble() ?? 0.0,
    );
  }

  Map<String, dynamic> toJson() => {
    'SupplierId': supplierId,
    'Name': name,
    'Email': email,
    'Phone': phone,
    'Address': address,
    'ContactPerson': contactPerson,
    'DebtAmount': debtAmount,
  };
}

/// Maps to SupplierDebtDTO in backend EMarket.Modules.InventoryModule.DTOs
class SupplierDebtModel {
  final int debtId;
  final int supplierId;
  final String supplierName;
  final int? purchaseOrderId;
  final double totalAmount;
  final double paidAmount;
  final double unpaidAmount;
  final String status;
  final DateTime? dueDate;
  final DateTime? createdAt;
  final String? note;

  SupplierDebtModel({
    required this.debtId,
    required this.supplierId,
    required this.supplierName,
    this.purchaseOrderId,
    required this.totalAmount,
    required this.paidAmount,
    required this.unpaidAmount,
    required this.status,
    this.dueDate,
    this.createdAt,
    this.note,
  });

  double get totalDebtAmount => totalAmount;
  double get remainingAmount => unpaidAmount;
  DateTime? get createdDate => createdAt;

  factory SupplierDebtModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return SupplierDebtModel(
        debtId: 0,
        supplierId: 0,
        supplierName: '',
        totalAmount: 0.0,
        paidAmount: 0.0,
        unpaidAmount: 0.0,
        status: 'Unpaid',
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

    return SupplierDebtModel(
      debtId: (raw['debtId'] ?? raw['DebtId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      supplierId: (raw['supplierId'] ?? raw['SupplierId'] as num?)?.toInt() ?? 0,
      supplierName: (raw['supplierName'] ?? raw['SupplierName'] ?? 'Nhà cung cấp').toString(),
      purchaseOrderId: (raw['purchaseOrderId'] ?? raw['PurchaseOrderId'] as num?)?.toInt(),
      totalAmount: (raw['totalAmount'] ?? raw['TotalAmount'] as num?)?.toDouble() ?? 0.0,
      paidAmount: (raw['paidAmount'] ?? raw['PaidAmount'] as num?)?.toDouble() ?? 0.0,
      unpaidAmount: (raw['unpaidAmount'] ?? raw['UnpaidAmount'] as num?)?.toDouble() ?? 0.0,
      status: (raw['status'] ?? raw['Status'] ?? 'Unpaid').toString(),
      dueDate: parseDate(raw['dueDate'] ?? raw['DueDate']),
      createdAt: parseDate(raw['createdAt'] ?? raw['CreatedAt'] ?? raw['debtDate']),
      note: raw['note'] ?? raw['Note'],
    );
  }
}

/// Maps to DebtSummary in EMarket InventoryAdminApiController
class SupplierDebtSummaryModel {
  final double totalDebt;
  final double totalPaid;
  final double totalRemaining;
  final double payRate;
  final int nearDueCount;
  final int overdueCount;
  final int totalRecords;

  SupplierDebtSummaryModel({
    this.totalDebt = 0.0,
    this.totalPaid = 0.0,
    this.totalRemaining = 0.0,
    this.payRate = 0.0,
    this.nearDueCount = 0,
    this.overdueCount = 0,
    this.totalRecords = 0,
  });

  factory SupplierDebtSummaryModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return SupplierDebtSummaryModel();
    }
    return SupplierDebtSummaryModel(
      totalDebt: (raw['totalDebt'] ?? raw['TotalDebt'] as num?)?.toDouble() ?? 0.0,
      totalPaid: (raw['totalPaid'] ?? raw['TotalPaid'] as num?)?.toDouble() ?? 0.0,
      totalRemaining: (raw['totalRemaining'] ?? raw['TotalRemaining'] as num?)?.toDouble() ?? 0.0,
      payRate: (raw['payRate'] ?? raw['PayRate'] as num?)?.toDouble() ?? 0.0,
      nearDueCount: (raw['nearDueCount'] ?? raw['NearDueCount'] as num?)?.toInt() ?? 0,
      overdueCount: (raw['overdueCount'] ?? raw['OverdueCount'] as num?)?.toInt() ?? 0,
      totalRecords: (raw['totalRecords'] ?? raw['TotalRecords'] as num?)?.toInt() ?? 0,
    );
  }
}
