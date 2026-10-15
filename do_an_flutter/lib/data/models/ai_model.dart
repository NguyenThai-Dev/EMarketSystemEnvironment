class AIReplenishmentModel {
  final int productId;
  final String productName;
  final String? categoryName;
  final String? unit;
  final String? productImage;
  final int currentStock;
  final String? reason;
  final int expectedDemand;
  final int safetyStock;
  final int suggestedQty;
  final String confidenceLevel; // HIGH, MEDIUM, LOW

  AIReplenishmentModel({
    required this.productId,
    required this.productName,
    this.categoryName,
    this.unit,
    this.productImage,
    required this.currentStock,
    this.reason,
    this.expectedDemand = 0,
    this.safetyStock = 0,
    required this.suggestedQty,
    this.confidenceLevel = 'HIGH',
  });

  factory AIReplenishmentModel.fromJson(Map<String, dynamic> json) {
    return AIReplenishmentModel(
      productId: json['productId'] as int? ?? json['ProductId'] as int? ?? 0,
      productName: (json['productName'] ?? json['ProductName'] ?? '').toString(),
      categoryName: json['categoryName'] ?? json['CategoryName'],
      unit: json['unit'] ?? json['Unit'] ?? 'Cái',
      productImage: json['productImage'] ?? json['ProductImage'],
      currentStock: (json['currentStock'] as num? ?? json['CurrentStock'] as num? ?? 0).toInt(),
      reason: json['reason'] ?? json['Reason'],
      expectedDemand: (json['expectedDemand'] as num? ?? json['ExpectedDemand'] as num? ?? 0).toInt(),
      safetyStock: (json['safetyStock'] as num? ?? json['SafetyStock'] as num? ?? 0).toInt(),
      suggestedQty: (json['suggestedQty'] as num? ?? json['SuggestedQty'] as num? ?? 0).toInt(),
      confidenceLevel: (json['confidenceLevel'] ?? json['ConfidenceLevel'] ?? 'HIGH').toString(),
    );
  }
}

class AIAnomalyModel {
  final int categoryId;
  final String categoryName;
  final int actualQty;
  final int forecastQty;
  final double deviationPercent;
  final String anomalyType; // Spike, Dip
  final String severity; // High, Medium, Low

  AIAnomalyModel({
    required this.categoryId,
    required this.categoryName,
    required this.actualQty,
    required this.forecastQty,
    required this.deviationPercent,
    required this.anomalyType,
    required this.severity,
  });

  factory AIAnomalyModel.fromJson(Map<String, dynamic> json) {
    return AIAnomalyModel(
      categoryId: json['categoryId'] as int? ?? json['CategoryId'] as int? ?? 0,
      categoryName: (json['categoryName'] ?? json['CategoryName'] ?? '').toString(),
      actualQty: (json['actualQty'] as num? ?? json['ActualQty'] as num? ?? 0).toInt(),
      forecastQty: (json['forecastQty'] as num? ?? json['ForecastQty'] as num? ?? 0).toInt(),
      deviationPercent: (json['deviationPercent'] as num? ?? json['DeviationPercent'] as num? ?? 0).toDouble(),
      anomalyType: (json['anomalyType'] ?? json['AnomalyType'] ?? 'Spike').toString(),
      severity: (json['severity'] ?? json['Severity'] ?? 'Medium').toString(),
    );
  }
}

class AIInsightModel {
  final int productId;
  final String productName;
  final double growthPercent;
  final double contributionPercent;
  final String insightLevel; // Star, Trending, Warning

  AIInsightModel({
    required this.productId,
    required this.productName,
    required this.growthPercent,
    required this.contributionPercent,
    required this.insightLevel,
  });

  factory AIInsightModel.fromJson(Map<String, dynamic> json) {
    return AIInsightModel(
      productId: json['productId'] as int? ?? json['ProductId'] as int? ?? 0,
      productName: (json['productName'] ?? json['ProductName'] ?? '').toString(),
      growthPercent: (json['growthPercent'] as num? ?? json['GrowthPercent'] as num? ?? 0).toDouble(),
      contributionPercent: (json['contributionPercent'] as num? ?? json['ContributionPercent'] as num? ?? 0).toDouble(),
      insightLevel: (json['insightLevel'] ?? json['InsightLevel'] ?? 'Trending').toString(),
    );
  }
}
