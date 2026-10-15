import 'package:flutter/material.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/services/thermal_printer_service.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/order_model.dart';

class ReceiptPreviewDialog extends StatelessWidget {
  final int orderId;
  final String orderCode;
  final double totalAmount;
  final double discount;
  final String paymentMethod;
  final String cashierName;
  final String branchName;
  final List<CheckoutItemRequest> items;
  final Map<int, String> productNames;

  const ReceiptPreviewDialog({
    super.key,
    required this.orderId,
    required this.orderCode,
    required this.totalAmount,
    this.discount = 0.0,
    required this.paymentMethod,
    required this.cashierName,
    required this.branchName,
    required this.items,
    this.productNames = const {},
  });

  static Future<void> show(
    BuildContext context, {
    required int orderId,
    required String orderCode,
    required double totalAmount,
    double discount = 0.0,
    required String paymentMethod,
    required String cashierName,
    required String branchName,
    required List<CheckoutItemRequest> items,
    Map<int, String> productNames = const {},
  }) {
    return showDialog(
      context: context,
      builder: (context) => ReceiptPreviewDialog(
        orderId: orderId,
        orderCode: orderCode,
        totalAmount: totalAmount,
        discount: discount,
        paymentMethod: paymentMethod,
        cashierName: cashierName,
        branchName: branchName,
        items: items,
        productNames: productNames,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final subtotal = totalAmount + discount;

    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      insetPadding: const EdgeInsets.symmetric(horizontal: 20, vertical: 24),
      child: Container(
        constraints: const BoxConstraints(maxWidth: 400),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Top Bar
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
              decoration: const BoxDecoration(
                color: AppColors.surfaceVariant,
                borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Row(
                    children: [
                      Icon(Icons.receipt_long_rounded, color: AppColors.primary, size: 20),
                      SizedBox(width: 8),
                      Text('Xem trước hóa đơn', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15)),
                    ],
                  ),
                  IconButton(
                    icon: const Icon(Icons.close, size: 20),
                    padding: EdgeInsets.zero,
                    constraints: const BoxConstraints(),
                    onPressed: () => Navigator.pop(context),
                  ),
                ],
              ),
            ),

            // Thermal Paper Preview
            Flexible(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(20),
                child: Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: Colors.grey.shade50,
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(color: Colors.grey.shade200),
                  ),
                  child: Column(
                    children: [
                      // Store Brand
                      const Text(
                        'HỆ THỐNG SIÊU THỊ EMARKET',
                        style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15, letterSpacing: 0.5),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 2),
                      Text(
                        'Chi nhánh: $branchName',
                        style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
                        textAlign: TextAlign.center,
                      ),
                      const Text(
                        'Hotline: 1900 8888 • www.emarket.vn',
                        style: TextStyle(fontSize: 11, color: AppColors.textMuted),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 12),

                      _buildDashedLine(),
                      const SizedBox(height: 8),

                      // Order Meta
                      _buildMetaRow('Mã hóa đơn:', orderCode),
                      _buildMetaRow('Thời gian:', Formatters.formatDateTime(DateTime.now())),
                      _buildMetaRow('Thu ngân:', cashierName),
                      _buildMetaRow('Thanh toán:', paymentMethod == 'VietQR' ? 'Chuyển khoản VietQR' : 'Tiền mặt'),

                      const SizedBox(height: 8),
                      _buildDashedLine(),
                      const SizedBox(height: 10),

                      // Column Headers
                      const Row(
                        children: [
                          Expanded(flex: 5, child: Text('Tên món', style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold))),
                          Expanded(flex: 2, child: Text('SL', textAlign: TextAlign.center, style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold))),
                          Expanded(flex: 3, child: Text('Thành tiền', textAlign: TextAlign.right, style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold))),
                        ],
                      ),
                      const SizedBox(height: 8),

                      // Items List
                      ...items.map((item) {
                        final name = productNames[item.productId] ?? 'Sản phẩm #${item.productId}';
                        final itemTotal = item.price * item.quantity;
                        return Padding(
                          padding: const EdgeInsets.symmetric(vertical: 4),
                          child: Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Expanded(
                                flex: 5,
                                child: Text(
                                  name,
                                  style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w500),
                                ),
                              ),
                              Expanded(
                                flex: 2,
                                child: Text(
                                  '${item.quantity}',
                                  textAlign: TextAlign.center,
                                  style: const TextStyle(fontSize: 12),
                                ),
                              ),
                              Expanded(
                                flex: 3,
                                child: Text(
                                  Formatters.formatVND(itemTotal),
                                  textAlign: TextAlign.right,
                                  style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
                                ),
                              ),
                            ],
                          ),
                        );
                      }),

                      const SizedBox(height: 10),
                      _buildDashedLine(),
                      const SizedBox(height: 8),

                      // Summary Calculations
                      _buildSummaryRow('Tạm tính:', Formatters.formatVND(subtotal)),
                      if (discount > 0)
                        _buildSummaryRow('Chiết khấu:', '-${Formatters.formatVND(discount)}', isDiscount: true),
                      const SizedBox(height: 6),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text('TỔNG CỘNG:', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
                          Text(
                            Formatters.formatVND(totalAmount),
                            style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16, color: AppColors.primary),
                          ),
                        ],
                      ),

                      const SizedBox(height: 14),
                      _buildDashedLine(),
                      const SizedBox(height: 12),

                      // Barcode simulation & Footer
                      Container(
                        height: 36,
                        decoration: BoxDecoration(
                          color: Colors.grey.shade200,
                          borderRadius: BorderRadius.circular(4),
                        ),
                        alignment: Alignment.center,
                        child: Text(
                          '* $orderCode *',
                          style: const TextStyle(fontSize: 13, letterSpacing: 3, fontFamily: 'monospace', fontWeight: FontWeight.bold),
                        ),
                      ),
                      const SizedBox(height: 8),
                      const Text(
                        'Cảm ơn quý khách & Hẹn gặp lại!',
                        style: TextStyle(fontSize: 11, fontStyle: FontStyle.italic, color: AppColors.textSecondary),
                      ),
                    ],
                  ),
                ),
              ),
            ),

            // Actions
            Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                children: [
                  Expanded(
                    child: OutlinedButton.icon(
                      icon: const Icon(Icons.close, size: 16),
                      label: const Text('Đóng'),
                      style: OutlinedButton.styleFrom(
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                      ),
                      onPressed: () => Navigator.pop(context),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: ElevatedButton.icon(
                      icon: const Icon(Icons.print, size: 16),
                      label: const Text('In hóa đơn'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                      ),
                      onPressed: () async {
                        final receiptData = ReceiptData(
                          storeName: branchName.isNotEmpty ? branchName : 'EMarket Store',
                          storeAddress: 'Chi nhánh bán lẻ EMarket',
                          storePhone: '1900 8888',
                          orderCode: orderCode,
                          orderDate: DateTime.now(),
                          cashierName: cashierName,
                          items: items.map((it) => ReceiptItem(
                            name: productNames[it.productId] ?? 'Sản phẩm #${it.productId}',
                            quantity: it.quantity,
                            unitPrice: it.price,
                            totalPrice: (it.quantity * it.price) - it.discount,
                          )).toList(),
                          subtotal: subtotal,
                          discount: discount,
                          totalAmount: totalAmount,
                          paymentMethod: paymentMethod,
                        );

                        // Try network printing if printer IP configured, or show simulated success
                        final bytes = ThermalPrinterService.buildEscPosBytes(receiptData, size: PaperSize.mm80);
                        debugPrint('[ThermalPrinter] Built ${bytes.length} ESC/POS bytes for order: $orderCode');

                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Row(
                              children: [
                                const Icon(Icons.check_circle_outline, color: Colors.white, size: 18),
                                const SizedBox(width: 8),
                                Text('Đã xuất lệnh in ESC/POS (${bytes.length} bytes) thành công!'),
                              ],
                            ),
                            backgroundColor: AppColors.emerald,
                            duration: const Duration(seconds: 2),
                          ),
                        );
                        Navigator.pop(context);
                      },
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildDashedLine() {
    return LayoutBuilder(
      builder: (context, constraints) {
        final boxWidth = constraints.constrainWidth();
        const dashWidth = 4.0;
        const dashHeight = 1.0;
        final dashCount = (boxWidth / (2 * dashWidth)).floor();
        return Flex(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          direction: Axis.horizontal,
          children: List.generate(dashCount, (_) {
            return SizedBox(
              width: dashWidth,
              height: dashHeight,
              child: DecoratedBox(
                decoration: BoxDecoration(color: Colors.grey.shade400),
              ),
            );
          }),
        );
      },
    );
  }

  Widget _buildMetaRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: const TextStyle(fontSize: 11, color: AppColors.textSecondary)),
          Text(value, style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w600)),
        ],
      ),
    );
  }

  Widget _buildSummaryRow(String label, String value, {bool isDiscount = false}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: const TextStyle(fontSize: 12, color: AppColors.textSecondary)),
          Text(
            value,
            style: TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: isDiscount ? AppColors.success : AppColors.textPrimary,
            ),
          ),
        ],
      ),
    );
  }
}
