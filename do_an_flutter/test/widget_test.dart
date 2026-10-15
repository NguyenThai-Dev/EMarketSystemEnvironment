import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:do_an_flutter/ui/screens/auth/login_screen.dart';
import 'package:do_an_flutter/providers/auth_provider.dart';
import 'package:do_an_flutter/data/models/ai_model.dart';
import 'package:do_an_flutter/data/models/customer_model.dart';
import 'package:do_an_flutter/data/models/expense_model.dart';
import 'package:do_an_flutter/data/models/order_model.dart';
import 'package:do_an_flutter/data/models/product_model.dart';
import 'package:do_an_flutter/data/models/quotation_model.dart';
import 'package:provider/provider.dart';

void main() {
  testWidgets('EMarket login smoke test', (WidgetTester tester) async {
    await tester.pumpWidget(
      MultiProvider(
        providers: [
          ChangeNotifierProvider(create: (_) => AuthProvider()),
        ],
        child: const MaterialApp(
          home: LoginScreen(),
        ),
      ),
    );

    expect(find.text('EMARKET SYSTEM'), findsOneWidget);
  });

  test('CheckoutRequest serialization matches backend CheckoutRequestDTO', () {
    final request = CheckoutRequest(
      userId: 1,
      branchId: 2,
      customerId: 10,
      pointsUsed: 50,
      pointsEarned: 100,
      totalAmount: 250000.0,
      manualDiscount: 10000.0,
      manualDiscountReason: 'VIP',
      status: 'Completed',
      paymentMethod: 'Cash',
      connectionId: 'conn-123',
      items: [
        CheckoutItemRequest(
          productId: 101,
          quantity: 2,
          price: 130000.0,
          discount: 5000.0,
        ),
      ],
    );

    final json = request.toJson();
    expect(json['UserId'], 1);
    expect(json['BranchId'], 2);
    expect(json['CustomerId'], 10);
    expect(json['PointsUsed'], 50);
    expect(json['PointsEarned'], 100);
    expect(json['TotalAmount'], 250000.0);
    expect(json['ManualDiscount'], 10000.0);
    expect(json['ManualDiscountReason'], 'VIP');
    expect(json['Status'], 'Completed');
    expect(json['PaymentMethod'], 'Cash');
    expect(json['ConnectionId'], 'conn-123');

    final items = json['Items'] as List;
    expect(items.length, 1);
    expect(items[0]['ProductId'], 101);
    expect(items[0]['Quantity'], 2);
    expect(items[0]['Price'], 130000.0);
    expect(items[0]['Discount'], 5000.0);
  });

  test('PayOS QR request and response DTO parsing', () {
    final qrReq = CreateQrRequest(amount: 150000, description: 'Test QR');
    expect(qrReq.toJson(), {'Amount': 150000, 'Description': 'Test QR'});

    final qrRes = CreateQrResponse.fromJson({
      'Success': true,
      'CheckoutUrl': 'https://payos.vn/pay/123',
      'QrCode': '00020101021238540010A00000072701260006970407',
      'OrderCode': 987654,
      'Message': 'Success',
    });
    expect(qrRes.success, true);
    expect(qrRes.orderCode, 987654);
    expect(qrRes.qrCode, '00020101021238540010A00000072701260006970407');
  });

  test('ProductLotModel FEFO expiration calculations', () {
    final expiredLot = ProductLotModel(
      lotId: 1,
      productId: 10,
      batchCode: 'LOT-EXP',
      expiryDate: DateTime.now().subtract(const Duration(days: 5)),
    );
    expect(expiredLot.isExpired, true);
    expect(expiredLot.isNearExpired, false);

    final nearExpLot = ProductLotModel(
      lotId: 2,
      productId: 10,
      batchCode: 'LOT-NEAR',
      expiryDate: DateTime.now().add(const Duration(days: 15)),
    );
    expect(nearExpLot.isExpired, false);
    expect(nearExpLot.isNearExpired, true);

    final freshLot = ProductLotModel(
      lotId: 3,
      productId: 10,
      batchCode: 'LOT-FRESH',
      expiryDate: DateTime.now().add(const Duration(days: 120)),
    );
    expect(freshLot.isExpired, false);
    expect(freshLot.isNearExpired, false);
  });

  test('CustomerModel loyalty tier & discount calculations', () {
    final regular = CustomerModel(
      customerId: 1,
      fullName: 'Nguyen Van A',
      pointEarnedTotal: 50,
      createdAt: DateTime.now(),
    );
    expect(regular.tierName, 'Đồng');
    expect(regular.discountRate, 0.0);

    final silver = CustomerModel(
      customerId: 2,
      fullName: 'Tran Thi B',
      pointEarnedTotal: 250,
      createdAt: DateTime.now(),
    );
    expect(silver.tierName, 'Bạc');
    expect(silver.discountRate, 0.02);

    final gold = CustomerModel(
      customerId: 3,
      fullName: 'Le Van C',
      pointEarnedTotal: 600,
      createdAt: DateTime.now(),
    );
    expect(gold.tierName, 'Vàng');
    expect(gold.discountRate, 0.05);

    final diamond = CustomerModel(
      customerId: 4,
      fullName: 'Pham Thi D',
      pointEarnedTotal: 1500,
      createdAt: DateTime.now(),
    );
    expect(diamond.tierName, 'Kim Cương');
    expect(diamond.discountRate, 0.10);
  });

  test('QuotationModel serialization and status check', () {
    final quotation = QuotationModel(
      quotationCode: 'BG-TEST-001',
      branchId: 1,
      customerName: 'Cong ty ABC',
      customerPhone: '0901234567',
      issueDate: DateTime(2026, 10, 1),
      expiryDate: DateTime(2026, 10, 8),
      totalAmount: 500000,
      discountAmount: 50000,
      finalAmount: 450000,
      status: 'Sent',
      details: [
        QuotationDetailModel(
          productId: 10,
          productName: 'Ca phe hat',
          quantity: 5,
          unitPrice: 100000,
          totalPrice: 500000,
        ),
      ],
    );

    final json = quotation.toJson();
    expect(json['QuotationCode'], 'BG-TEST-001');
    expect(json['FinalAmount'], 450000.0);
    expect(json['Status'], 'Sent');
    expect(json['Details'], isA<List>());
    expect((json['Details'] as List).length, 1);
  });

  test('ExpenseModel and CreateExpenseRequest serialization', () {
    final req = CreateExpenseRequest(
      branchId: 1,
      categoryId: 2,
      userId: 1,
      amount: 120000,
      note: 'Mua van phong pham',
      paymentMethod: 'Cash',
    );

    final json = req.toJson();
    expect(json['BranchId'], 1);
    expect(json['CategoryId'], 2);
    expect(json['Amount'], 120000.0);
    expect(json['Status'], 'Pending');
    expect(json['PaymentMethod'], 'Cash');

    final expense = ExpenseModel.fromJson({
      'ExpenseId': 88,
      'BranchId': 1,
      'CategoryId': 2,
      'CategoryName': 'Văn phòng phẩm',
      'UserId': 1,
      'UserName': 'Admin',
      'Amount': 120000,
      'Status': 'Approved',
      'PaymentMethod': 'Cash',
      'ExpenseDate': '2026-10-01T08:00:00',
    });

    expect(expense.expenseId, 88);
    expect(expense.categoryName, 'Văn phòng phẩm');
    expect(expense.status, 'Approved');
  });

  test('AI Forecast and Replenishment DTO parsing', () {
    final rep = AIReplenishmentModel.fromJson({
      'ProductId': 15,
      'ProductName': 'Bia Heineken',
      'CurrentStock': 12,
      'ExpectedDemand': 80,
      'SuggestedQty': 70,
      'ConfidenceLevel': 'HIGH',
      'Reason': 'Nhu cầu cuối tuần tăng cao',
    });

    expect(rep.productId, 15);
    expect(rep.productName, 'Bia Heineken');
    expect(rep.suggestedQty, 70);
    expect(rep.confidenceLevel, 'HIGH');

    final anomaly = AIAnomalyModel.fromJson({
      'CategoryId': 3,
      'CategoryName': 'Đồ uống',
      'ActualQty': 250,
      'ForecastQty': 100,
      'DeviationPercent': 150.0,
      'AnomalyType': 'Spike',
      'Severity': 'High',
    });

    expect(anomaly.anomalyType, 'Spike');
    expect(anomaly.deviationPercent, 150.0);

    final insight = AIInsightModel.fromJson({
      'ProductId': 15,
      'ProductName': 'Bia Heineken',
      'GrowthPercent': 25.5,
      'ContributionPercent': 18.2,
      'InsightLevel': 'Star',
    });

    expect(insight.insightLevel, 'Star');
    expect(insight.growthPercent, 25.5);
  });
}
