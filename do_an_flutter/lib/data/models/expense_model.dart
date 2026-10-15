class ExpenseCategoryModel {
  final int categoryId;
  final String name;
  final String? description;
  final bool isActive;

  ExpenseCategoryModel({
    required this.categoryId,
    required this.name,
    this.description,
    this.isActive = true,
  });

  factory ExpenseCategoryModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return ExpenseCategoryModel(categoryId: 0, name: raw?.toString() ?? '');
    }
    final rawActive = raw['isActive'] ?? raw['IsActive'] ?? true;
    return ExpenseCategoryModel(
      categoryId: (raw['categoryId'] ?? raw['CategoryId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      name: (raw['name'] ?? raw['Name'] ?? '').toString(),
      description: raw['description'] ?? raw['Description'],
      isActive: rawActive == true || rawActive.toString().toLowerCase() == 'true',
    );
  }

  Map<String, dynamic> toJson() => {
    'CategoryId': categoryId,
    'Name': name,
    'Description': description,
    'IsActive': isActive,
  };
}

class ExpenseModel {
  final int expenseId;
  final int branchId;
  final String? branchName;
  final int categoryId;
  final String? categoryName;
  final int userId;
  final String? userName;
  final double amount;
  final DateTime expenseDate;
  final String? note;
  final String? refImage;
  final String paymentMethod; // Cash, Transfer
  final String status; // Pending, Approved, Rejected
  final int? approvedBy;
  final String? approverName;
  final DateTime? approvedAt;
  final int? rejectedBy;
  final String? rejectorName;
  final DateTime? rejectedAt;
  final String? rejectionReason;

  ExpenseModel({
    required this.expenseId,
    required this.branchId,
    this.branchName,
    required this.categoryId,
    this.categoryName,
    required this.userId,
    this.userName,
    required this.amount,
    required this.expenseDate,
    this.note,
    this.refImage,
    this.paymentMethod = 'Cash',
    this.status = 'Pending',
    this.approvedBy,
    this.approverName,
    this.approvedAt,
    this.rejectedBy,
    this.rejectorName,
    this.rejectedAt,
    this.rejectionReason,
  });

  factory ExpenseModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return ExpenseModel(
        expenseId: 0,
        branchId: 1,
        categoryId: 0,
        userId: 1,
        amount: 0.0,
        expenseDate: DateTime.now(),
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

    DateTime? parseNullableDate(dynamic d) {
      if (d == null) return null;
      try {
        return DateTime.parse(d.toString());
      } catch (_) {
        return null;
      }
    }

    return ExpenseModel(
      expenseId: (raw['expenseId'] ?? raw['ExpenseId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      branchId: (raw['branchId'] ?? raw['BranchId'] as num?)?.toInt() ?? 1,
      branchName: raw['branchName'] ?? raw['BranchName'],
      categoryId: (raw['categoryId'] ?? raw['CategoryId'] as num?)?.toInt() ?? 0,
      categoryName: raw['categoryName'] ?? raw['CategoryName'],
      userId: (raw['userId'] ?? raw['UserId'] as num?)?.toInt() ?? 1,
      userName: raw['userName'] ?? raw['UserName'],
      amount: (raw['amount'] ?? raw['Amount'] as num?)?.toDouble() ?? 0.0,
      expenseDate: parseDate(raw['expenseDate'] ?? raw['ExpenseDate']),
      note: raw['note'] ?? raw['Note'],
      refImage: raw['refImage'] ?? raw['RefImage'],
      paymentMethod: (raw['paymentMethod'] ?? raw['PaymentMethod'] ?? 'Cash').toString(),
      status: (raw['status'] ?? raw['Status'] ?? 'Pending').toString(),
      approvedBy: (raw['approvedBy'] ?? raw['ApprovedBy'] as num?)?.toInt(),
      approverName: raw['approverName'] ?? raw['ApproverName'],
      approvedAt: parseNullableDate(raw['approvedAt'] ?? raw['ApprovedAt']),
      rejectedBy: (raw['rejectedBy'] ?? raw['RejectedBy'] as num?)?.toInt(),
      rejectorName: raw['rejectorName'] ?? raw['RejectorName'],
      rejectedAt: parseNullableDate(raw['rejectedAt'] ?? raw['RejectedAt']),
      rejectionReason: raw['rejectionReason'] ?? raw['RejectionReason'],
    );
  }

  Map<String, dynamic> toJson() => {
    'ExpenseId': expenseId,
    'BranchId': branchId,
    'CategoryId': categoryId,
    'UserId': userId,
    'Amount': amount,
    'ExpenseDate': expenseDate.toIso8601String(),
    'Note': note,
    'RefImage': refImage,
    'PaymentMethod': paymentMethod,
    'Status': status,
  };
}

class CreateExpenseRequest {
  final int branchId;
  final int categoryId;
  final int userId;
  final double amount;
  final String? note;
  final String paymentMethod;
  final String? refImage;

  CreateExpenseRequest({
    required this.branchId,
    required this.categoryId,
    required this.userId,
    required this.amount,
    this.note,
    this.paymentMethod = 'Cash',
    this.refImage,
  });

  Map<String, dynamic> toJson() => {
    'BranchId': branchId,
    'CategoryId': categoryId,
    'UserId': userId,
    'Amount': amount,
    'ExpenseDate': DateTime.now().toIso8601String(),
    'Note': note,
    'PaymentMethod': paymentMethod,
    'Status': 'Pending',
    'RefImage': refImage,
  };
}

class ExpenseSummaryModel {
  final int totalExpenses;
  final double totalApprovedAmount;
  final double totalPendingAmount;
  final double totalRejectedAmount;
  final int approvedCount;
  final int pendingCount;
  final int rejectedCount;

  ExpenseSummaryModel({
    required this.totalExpenses,
    required this.totalApprovedAmount,
    required this.totalPendingAmount,
    required this.totalRejectedAmount,
    required this.approvedCount,
    required this.pendingCount,
    required this.rejectedCount,
  });

  factory ExpenseSummaryModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return ExpenseSummaryModel(
        totalExpenses: 0,
        totalApprovedAmount: 0.0,
        totalPendingAmount: 0.0,
        totalRejectedAmount: 0.0,
        approvedCount: 0,
        pendingCount: 0,
        rejectedCount: 0,
      );
    }
    return ExpenseSummaryModel(
      totalExpenses: (raw['totalExpenses'] ?? raw['TotalExpenses'] as num?)?.toInt() ?? 0,
      totalApprovedAmount: (raw['totalApprovedAmount'] ?? raw['TotalApprovedAmount'] as num?)?.toDouble() ?? 0.0,
      totalPendingAmount: (raw['totalPendingAmount'] ?? raw['TotalPendingAmount'] as num?)?.toDouble() ?? 0.0,
      totalRejectedAmount: (raw['totalRejectedAmount'] ?? raw['TotalRejectedAmount'] as num?)?.toDouble() ?? 0.0,
      approvedCount: (raw['approvedCount'] ?? raw['ApprovedCount'] as num?)?.toInt() ?? 0,
      pendingCount: (raw['pendingCount'] ?? raw['PendingCount'] as num?)?.toInt() ?? 0,
      rejectedCount: (raw['rejectedCount'] ?? raw['RejectedCount'] as num?)?.toInt() ?? 0,
    );
  }
}
