class DashboardOverview {
  final double todayRevenue;
  final int todayOrders;
  final double estimatedProfit;
  final int totalCustomers;
  final int lowStockProducts;
  final double totalPurchase;
  final int totalProducts;
  final int totalWarehouses;
  final double totalInventoryQuantity;
  final double salesGrowthPercent;
  final double inventoryLossValue;
  final DashboardTrendModel? trend;
  final List<BranchPerformanceModel> branchPerformance;
  final List<ChartItemModel> stockChart;

  DashboardOverview({
    this.todayRevenue = 0.0,
    this.todayOrders = 0,
    this.estimatedProfit = 0.0,
    this.totalCustomers = 0,
    this.lowStockProducts = 0,
    this.totalPurchase = 0.0,
    this.totalProducts = 0,
    this.totalWarehouses = 0,
    this.totalInventoryQuantity = 0.0,
    this.salesGrowthPercent = 0.0,
    this.inventoryLossValue = 0.0,
    this.trend,
    this.branchPerformance = const [],
    this.stockChart = const [],
  });

  factory DashboardOverview.fromJson(Map<String, dynamic> json) {
    // If response is nested DashboardOverviewDTO: { Summary: {...}, Trend: {...}, BranchPerformance: [...], StockChart: [...] }
    final summaryRaw = json['Summary'] ?? json['summary'] ?? json['data'] ?? json['Data'];
    final summary = summaryRaw is Map<String, dynamic> ? summaryRaw : json;

    DashboardTrendModel? parsedTrend;
    final trendRaw = json['Trend'] ?? json['trend'];
    if (trendRaw is Map<String, dynamic>) {
      parsedTrend = DashboardTrendModel.fromJson(trendRaw);
    }

    List<BranchPerformanceModel> parsedBranches = [];
    final branchesRaw = json['BranchPerformance'] ?? json['branchPerformance'];
    if (branchesRaw is List) {
      parsedBranches = branchesRaw.map((e) => BranchPerformanceModel.fromJson(e)).toList();
    }

    List<ChartItemModel> parsedStock = [];
    final stockRaw = json['StockChart'] ?? json['stockChart'];
    if (stockRaw is List) {
      parsedStock = stockRaw.map((e) => ChartItemModel.fromJson(e)).toList();
    }

    return DashboardOverview(
      todayRevenue: (summary['TotalSales'] ?? summary['totalSales'] ?? summary['todayRevenue'] ?? summary['TodayRevenue'] ?? summary['revenue'] ?? summary['Revenue'] as num?)?.toDouble() ?? 0.0,
      todayOrders: (summary['TotalOrders'] ?? summary['totalOrders'] ?? summary['todayOrders'] ?? summary['TodayOrders'] ?? summary['orders'] ?? summary['Orders'] as num?)?.toInt() ?? 0,
      estimatedProfit: (summary['ActualProfit'] ?? summary['actualProfit'] ?? summary['GrossProfit'] ?? summary['grossProfit'] ?? summary['estimatedProfit'] ?? summary['EstimatedProfit'] ?? summary['profit'] ?? summary['Profit'] as num?)?.toDouble() ?? 0.0,
      totalCustomers: (summary['TotalCustomers'] ?? summary['totalCustomers'] ?? summary['totalProducts'] ?? summary['TotalProducts'] ?? summary['customers'] ?? summary['Customers'] as num?)?.toInt() ?? 0,
      lowStockProducts: (summary['LowStockProducts'] ?? summary['lowStockProducts'] ?? summary['lowStock'] ?? summary['LowStock'] as num?)?.toInt() ?? 0,
      totalPurchase: (summary['TotalPurchase'] ?? summary['totalPurchase'] as num?)?.toDouble() ?? 0.0,
      totalProducts: (summary['TotalProducts'] ?? summary['totalProducts'] as num?)?.toInt() ?? 0,
      totalWarehouses: (summary['TotalWarehouses'] ?? summary['totalWarehouses'] as num?)?.toInt() ?? 0,
      totalInventoryQuantity: (summary['TotalInventoryQuantity'] ?? summary['totalInventoryQuantity'] as num?)?.toDouble() ?? 0.0,
      salesGrowthPercent: (summary['SalesGrowthPercent'] ?? summary['salesGrowthPercent'] as num?)?.toDouble() ?? 0.0,
      inventoryLossValue: (summary['InventoryLossValue'] ?? summary['inventoryLossValue'] as num?)?.toDouble() ?? 0.0,
      trend: parsedTrend,
      branchPerformance: parsedBranches,
      stockChart: parsedStock,
    );
  }
}

class DashboardTrendModel {
  final List<String> labels;
  final List<double> sales;
  final List<double> purchases;

  DashboardTrendModel({
    this.labels = const [],
    this.sales = const [],
    this.purchases = const [],
  });

  factory DashboardTrendModel.fromJson(Map<String, dynamic> json) {
    List<String> parseLabels() {
      final list = json['labels'] ?? json['Labels'];
      if (list is List) return list.map((e) => e.toString()).toList();
      return [];
    }

    List<double> parseDoubles(dynamic raw) {
      if (raw is List) {
        return raw.map((e) => (e as num?)?.toDouble() ?? 0.0).toList();
      }
      return [];
    }

    return DashboardTrendModel(
      labels: parseLabels(),
      sales: parseDoubles(json['sales'] ?? json['Sales']),
      purchases: parseDoubles(json['purchases'] ?? json['Purchases']),
    );
  }
}

class BranchModel {
  final int branchId;
  final String branchName;
  final String? address;
  final String? phone;

  BranchModel({
    required this.branchId,
    required this.branchName,
    this.address,
    this.phone,
  });

  String get name => branchName;

  factory BranchModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return BranchModel(branchId: 0, branchName: raw?.toString() ?? '');
    }
    return BranchModel(
      branchId: (raw['branchId'] ?? raw['BranchId'] ?? raw['id'] ?? raw['Id'] as num?)?.toInt() ?? 0,
      branchName: (raw['branchName'] ?? raw['BranchName'] ?? raw['name'] ?? raw['Name'] ?? '').toString(),
      address: raw['address'] ?? raw['Address'],
      phone: raw['phone'] ?? raw['Phone'],
    );
  }

  Map<String, dynamic> toJson() => {
    'branchId': branchId,
    'branchName': branchName,
    'address': address,
    'phone': phone,
  };
}

class BranchPerformanceModel {
  final int branchId;
  final String name;
  final double salesAmount;
  final double purchaseAmount;
  final double profit;
  final int lowStockCount;

  BranchPerformanceModel({
    required this.branchId,
    required this.name,
    this.salesAmount = 0,
    this.purchaseAmount = 0,
    this.profit = 0,
    this.lowStockCount = 0,
  });

  factory BranchPerformanceModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return BranchPerformanceModel(branchId: 0, name: 'Chi nhánh');
    }
    return BranchPerformanceModel(
      branchId: (raw['branchId'] ?? raw['BranchId'] as num?)?.toInt() ?? 0,
      name: (raw['name'] ?? raw['Name'] ?? raw['branchName'] ?? raw['BranchName'] ?? 'Chi nhánh').toString(),
      salesAmount: (raw['salesAmount'] ?? raw['SalesAmount'] as num?)?.toDouble() ?? 0.0,
      purchaseAmount: (raw['purchaseAmount'] ?? raw['PurchaseAmount'] as num?)?.toDouble() ?? 0.0,
      profit: (raw['profit'] ?? raw['Profit'] as num?)?.toDouble() ?? 0.0,
      lowStockCount: (raw['lowStockCount'] ?? raw['LowStockCount'] as num?)?.toInt() ?? 0,
    );
  }
}

class ChartItemModel {
  final String label;
  final int value;
  final double extraValue;
  final String status;

  ChartItemModel({
    required this.label,
    required this.value,
    this.extraValue = 0,
    this.status = 'Normal',
  });

  factory ChartItemModel.fromJson(dynamic raw) {
    if (raw is! Map<String, dynamic>) {
      return ChartItemModel(label: '', value: 0);
    }
    return ChartItemModel(
      label: (raw['label'] ?? raw['Label'] ?? raw['name'] ?? raw['Name'] ?? '').toString(),
      value: (raw['value'] ?? raw['Value'] ?? raw['count'] ?? raw['Count'] as num?)?.toInt() ?? 0,
      extraValue: (raw['extraValue'] ?? raw['ExtraValue'] as num?)?.toDouble() ?? 0.0,
      status: (raw['status'] ?? raw['Status'] ?? 'Normal').toString(),
    );
  }
}
