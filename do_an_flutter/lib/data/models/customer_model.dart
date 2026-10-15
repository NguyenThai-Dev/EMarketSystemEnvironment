import 'package:flutter/material.dart';

class CustomerModel {
  final int customerId;
  final String fullName;
  final String? email;
  final String? phone;
  final String? customerType;
  final int pointBalance;
  final int pointEarnedTotal;
  final String? userImg;
  final DateTime createdAt;

  CustomerModel({
    required this.customerId,
    required this.fullName,
    this.email,
    this.phone,
    this.customerType,
    this.pointBalance = 0,
    this.pointEarnedTotal = 0,
    this.userImg,
    required this.createdAt,
  });

  factory CustomerModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return CustomerModel(
        customerId: 0,
        fullName: raw?.toString() ?? 'Khách vãng lai',
        createdAt: DateTime.now(),
      );
    }
    return CustomerModel(
      customerId: (raw['customerId'] ?? raw['CustomerId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      fullName: (raw['fullName'] ?? raw['FullName'] ?? raw['name'] ?? raw['Name'] ?? 'Khách vãng lai').toString(),
      email: raw['email'] ?? raw['Email'],
      phone: raw['phone'] ?? raw['Phone'],
      customerType: raw['customerType'] ?? raw['CustomerType'],
      pointBalance: (raw['pointBalance'] ?? raw['PointBalance'] as num?)?.toInt() ?? 0,
      pointEarnedTotal: (raw['pointEarnedTotal'] ?? raw['PointEarnedTotal'] as num?)?.toInt() ?? 0,
      userImg: raw['userImg'] ?? raw['UserImg'] ?? raw['avatar'] ?? raw['Avatar'],
      createdAt: raw['createdAt'] != null
          ? DateTime.tryParse(raw['createdAt'].toString()) ?? DateTime.now()
          : (raw['CreatedAt'] != null
              ? DateTime.tryParse(raw['CreatedAt'].toString()) ?? DateTime.now()
              : DateTime.now()),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'customerId': customerId,
      'fullName': fullName,
      'email': email,
      'phone': phone,
      'customerType': customerType,
      'pointBalance': pointBalance,
      'pointEarnedTotal': pointEarnedTotal,
      'userImg': userImg,
      'createdAt': createdAt.toIso8601String(),
    };
  }

  String get name => fullName;

  /// Tên hạng thành viên theo điểm tích lũy hoặc loại khách hàng
  String get tierName {
    if (customerType != null && customerType!.isNotEmpty && customerType != 'Standard') {
      return customerType!;
    }
    if (pointEarnedTotal >= 1000) return 'Kim Cương';
    if (pointEarnedTotal >= 500) return 'Vàng';
    if (pointEarnedTotal >= 200) return 'Bạc';
    return 'Đồng';
  }

  /// Màu sắc đại diện cho hạng thành viên
  Color get tierColor {
    switch (tierName.toLowerCase()) {
      case 'kim cương':
      case 'diamond':
        return const Color(0xFF673AB7);
      case 'vàng':
      case 'gold':
      case 'vip':
        return const Color(0xFFF59E0B);
      case 'bạc':
      case 'silver':
        return const Color(0xFF64748B);
      default:
        return const Color(0xFFCD7F32);
    }
  }

  /// Tỷ lệ giảm giá tương ứng theo thứ hạng
  double get discountRate {
    switch (tierName.toLowerCase()) {
      case 'kim cương':
      case 'diamond':
        return 0.10;
      case 'vàng':
      case 'gold':
      case 'vip':
        return 0.05;
      case 'bạc':
      case 'silver':
        return 0.02;
      default:
        return 0.0;
    }
  }
}

class CustomerCreateRequest {
  final String fullName;
  final String? email;
  final String? phone;
  final String? customerType;
  final String? userImg;

  CustomerCreateRequest({
    required this.fullName,
    this.email,
    this.phone,
    this.customerType = 'Standard',
    this.userImg,
  });

  Map<String, dynamic> toJson() {
    return {
      'FullName': fullName,
      'Email': email,
      'Phone': phone,
      'CustomerType': customerType,
      'UserImg': userImg,
    };
  }
}

class CustomerUpdateRequest extends CustomerCreateRequest {
  final int customerId;

  CustomerUpdateRequest({
    required this.customerId,
    required super.fullName,
    super.email,
    super.phone,
    super.customerType,
    super.userImg,
  });

  @override
  Map<String, dynamic> toJson() {
    final map = super.toJson();
    map['CustomerId'] = customerId;
    return map;
  }
}

class LoyaltyProgramModel {
  final int loyaltyId;
  final int customerId;
  final int? orderId;
  final int pointsEarned;
  final int pointsRedeemed;
  final DateTime createdAt;

  LoyaltyProgramModel({
    required this.loyaltyId,
    required this.customerId,
    this.orderId,
    required this.pointsEarned,
    required this.pointsRedeemed,
    required this.createdAt,
  });

  factory LoyaltyProgramModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return LoyaltyProgramModel(
        loyaltyId: 0,
        customerId: 0,
        pointsEarned: 0,
        pointsRedeemed: 0,
        createdAt: DateTime.now(),
      );
    }
    return LoyaltyProgramModel(
      loyaltyId: (raw['loyaltyId'] ?? raw['LoyaltyId'] as num?)?.toInt() ?? 0,
      customerId: (raw['customerId'] ?? raw['CustomerId'] as num?)?.toInt() ?? 0,
      orderId: (raw['orderId'] ?? raw['OrderId'] as num?)?.toInt(),
      pointsEarned: (raw['pointsEarned'] ?? raw['PointsEarned'] as num?)?.toInt() ?? 0,
      pointsRedeemed: (raw['pointsRedeemed'] ?? raw['PointsRedeemed'] as num?)?.toInt() ?? 0,
      createdAt: raw['createdAt'] != null
          ? DateTime.tryParse(raw['createdAt'].toString()) ?? DateTime.now()
          : (raw['CreatedAt'] != null
              ? DateTime.tryParse(raw['CreatedAt'].toString()) ?? DateTime.now()
              : DateTime.now()),
    );
  }
}

class CustomerStatsModel {
  final int totalCustomers;
  final int vipCustomers;
  final int newCustomersSinceDate;

  CustomerStatsModel({
    required this.totalCustomers,
    required this.vipCustomers,
    required this.newCustomersSinceDate,
  });

  factory CustomerStatsModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return CustomerStatsModel(totalCustomers: 0, vipCustomers: 0, newCustomersSinceDate: 0);
    }
    return CustomerStatsModel(
      totalCustomers: (raw['totalCustomers'] ?? raw['TotalCustomers'] as num?)?.toInt() ?? 0,
      vipCustomers: (raw['vipCustomers'] ?? raw['VipCustomers'] as num?)?.toInt() ?? 0,
      newCustomersSinceDate: (raw['newCustomersSinceDate'] ?? raw['NewCustomersSinceDate'] as num?)?.toInt() ?? 0,
    );
  }
}
