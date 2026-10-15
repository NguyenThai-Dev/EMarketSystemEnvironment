class QuotationDetailModel {
  final int detailId;
  final int productId;
  final String productName;
  final String? productImage;
  final String? unit;
  final int quantity;
  final double unitPrice;
  final double discount;
  final double totalPrice;
  final String? note;

  QuotationDetailModel({
    this.detailId = 0,
    required this.productId,
    required this.productName,
    this.productImage,
    this.unit = 'Cái',
    required this.quantity,
    required this.unitPrice,
    this.discount = 0,
    required this.totalPrice,
    this.note,
  });

  factory QuotationDetailModel.fromJson(Map<String, dynamic> json) {
    return QuotationDetailModel(
      detailId: json['detailId'] as int? ?? json['DetailId'] as int? ?? 0,
      productId: json['productId'] as int? ?? json['ProductId'] as int? ?? 0,
      productName: (json['productName'] ?? json['ProductName'] ?? '').toString(),
      productImage: json['productImage'] ?? json['ProductImage'],
      unit: json['unit'] ?? json['Unit'] ?? 'Cái',
      quantity: (json['quantity'] as num? ?? json['Quantity'] as num? ?? 1).toInt(),
      unitPrice: (json['unitPrice'] as num? ?? json['UnitPrice'] as num? ?? 0).toDouble(),
      discount: (json['discount'] as num? ?? json['Discount'] as num? ?? 0).toDouble(),
      totalPrice: (json['totalPrice'] as num? ?? json['TotalPrice'] as num? ?? 0).toDouble(),
      note: json['note'] ?? json['Note'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'DetailId': detailId,
      'ProductId': productId,
      'ProductName': productName,
      'ProductImage': productImage,
      'Unit': unit,
      'Quantity': quantity,
      'UnitPrice': unitPrice,
      'Discount': discount,
      'TotalPrice': totalPrice,
      'Note': note,
    };
  }
}

class QuotationModel {
  final int quotationId;
  final String quotationCode;
  final int branchId;
  final String? branchName;
  final int? customerId;
  final String? customerName;
  final String? customerPhone;
  final String? customerAddress;
  final int userId;
  final String? creatorName;
  final DateTime issueDate;
  final DateTime expiryDate;
  final double totalAmount;
  final double discountAmount;
  final double manualDiscount;
  final String? discountReason;
  final double finalAmount;
  final String status; // Draft, Sent, Accepted, Converted, Cancelled
  final String? note;
  final int? convertedOrderId;
  final List<QuotationDetailModel> details;

  QuotationModel({
    this.quotationId = 0,
    required this.quotationCode,
    required this.branchId,
    this.branchName,
    this.customerId,
    this.customerName,
    this.customerPhone,
    this.customerAddress,
    this.userId = 1,
    this.creatorName,
    required this.issueDate,
    required this.expiryDate,
    this.totalAmount = 0,
    this.discountAmount = 0,
    this.manualDiscount = 0,
    this.discountReason,
    this.finalAmount = 0,
    this.status = 'Draft',
    this.note,
    this.convertedOrderId,
    this.details = const [],
  });

  factory QuotationModel.fromJson(Map<String, dynamic> json) {
    final rawDetails = (json['details'] ?? json['Details']) as List<dynamic>? ?? [];
    return QuotationModel(
      quotationId: json['quotationId'] as int? ?? json['QuotationId'] as int? ?? 0,
      quotationCode: (json['quotationCode'] ?? json['QuotationCode'] ?? '').toString(),
      branchId: json['branchId'] as int? ?? json['BranchId'] as int? ?? 1,
      branchName: json['branchName'] ?? json['BranchName'],
      customerId: json['customerId'] as int? ?? json['CustomerId'] as int?,
      customerName: json['customerName'] ?? json['CustomerName'],
      customerPhone: json['customerPhone'] ?? json['CustomerPhone'],
      customerAddress: json['customerAddress'] ?? json['CustomerAddress'],
      userId: json['userId'] as int? ?? json['UserId'] as int? ?? 1,
      creatorName: json['creatorName'] ?? json['CreatorName'],
      issueDate: json['issueDate'] != null
          ? DateTime.tryParse(json['issueDate'].toString()) ?? DateTime.now()
          : (json['IssueDate'] != null
              ? DateTime.tryParse(json['IssueDate'].toString()) ?? DateTime.now()
              : DateTime.now()),
      expiryDate: json['expiryDate'] != null
          ? DateTime.tryParse(json['expiryDate'].toString()) ?? DateTime.now().add(const Duration(days: 7))
          : (json['ExpiryDate'] != null
              ? DateTime.tryParse(json['ExpiryDate'].toString()) ?? DateTime.now().add(const Duration(days: 7))
              : DateTime.now().add(const Duration(days: 7))),
      totalAmount: (json['totalAmount'] as num? ?? json['TotalAmount'] as num? ?? 0).toDouble(),
      discountAmount: (json['discountAmount'] as num? ?? json['DiscountAmount'] as num? ?? 0).toDouble(),
      manualDiscount: (json['manualDiscount'] as num? ?? json['ManualDiscount'] as num? ?? 0).toDouble(),
      discountReason: json['discountReason'] ?? json['DiscountReason'],
      finalAmount: (json['finalAmount'] as num? ?? json['FinalAmount'] as num? ?? 0).toDouble(),
      status: (json['status'] ?? json['Status'] ?? 'Draft').toString(),
      note: json['note'] ?? json['Note'],
      convertedOrderId: json['convertedOrderId'] as int? ?? json['ConvertedOrderId'] as int?,
      details: rawDetails.map((e) => QuotationDetailModel.fromJson(e as Map<String, dynamic>)).toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'QuotationId': quotationId,
      'QuotationCode': quotationCode,
      'BranchId': branchId,
      'BranchName': branchName,
      'CustomerId': customerId,
      'CustomerName': customerName,
      'CustomerPhone': customerPhone,
      'CustomerAddress': customerAddress,
      'UserId': userId,
      'CreatorName': creatorName,
      'IssueDate': issueDate.toIso8601String(),
      'ExpiryDate': expiryDate.toIso8601String(),
      'TotalAmount': totalAmount,
      'DiscountAmount': discountAmount,
      'ManualDiscount': manualDiscount,
      'DiscountReason': discountReason,
      'FinalAmount': finalAmount,
      'Status': status,
      'Note': note,
      'ConvertedOrderId': convertedOrderId,
      'Details': details.map((d) => d.toJson()).toList(),
    };
  }
}
