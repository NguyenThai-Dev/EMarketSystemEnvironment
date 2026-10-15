import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/order_model.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/pos_cart_provider.dart';
import '../../widgets/vnd_text.dart';
import 'receipt_preview_dialog.dart';
import 'vietqr_dialog.dart';

enum PaymentMode { cash, transfer }

/// Modal thanh toán Tiền mặt & Chuyển khoản (PayOS QR) cho thu ngân
class PosPaymentSheet extends StatefulWidget {
  const PosPaymentSheet({super.key});

  static Future<void> show(BuildContext context) {
    return showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => const PosPaymentSheet(),
    );
  }

  @override
  State<PosPaymentSheet> createState() => _PosPaymentSheetState();
}

class _PosPaymentSheetState extends State<PosPaymentSheet> {
  PaymentMode _selectedMode = PaymentMode.cash;
  final TextEditingController _cashGivenCtrl = TextEditingController();
  double _cashGiven = 0.0;
  bool _isProcessing = false;

  @override
  void initState() {
    super.initState();
    final cart = context.read<PosCartProvider>();
    _cashGiven = cart.finalTotal;
    _cashGivenCtrl.text = Formatters.formatNumber(cart.finalTotal);
  }

  @override
  void dispose() {
    _cashGivenCtrl.dispose();
    super.dispose();
  }

  void _selectPresetAmount(double amount) {
    setState(() {
      _cashGiven = amount;
      _cashGivenCtrl.text = Formatters.formatNumber(amount);
    });
    HapticFeedback.selectionClick();
  }

  Future<void> _handleCompleteCheckout(String paymentMethodTitle, String methodCode) async {
    final cart = context.read<PosCartProvider>();
    final auth = context.read<AuthProvider>();

    setState(() => _isProcessing = true);
    cart.setPaymentMethod(methodCode);

    final snapshotTotal = cart.finalTotal;
    final snapshotDiscount = cart.totalDiscount;
    final checkoutItems = cart.items.map((i) => CheckoutItemRequest(
      productId: i.product.productId,
      quantity: i.quantity,
      price: i.price,
      discount: i.discount,
    )).toList();
    final productNames = {for (var i in cart.items) i.product.productId: i.product.name};

    final result = await cart.checkout(
      branchId: auth.selectedBranch?.branchId ?? 1,
      userId: auth.currentUser?.userId ?? 1,
    );

    if (!mounted) return;
    setState(() => _isProcessing = false);

    if (result != null && result.success) {
      // Đảm bảo giỏ hàng được clear sạch sau khi thanh toán thành công
      cart.clearCart();
      HapticFeedback.heavyImpact();
      Navigator.pop(context); // Close payment sheet

      ReceiptPreviewDialog.show(
        context,
        orderId: result.orderId ?? 0,
        orderCode: result.orderCode ?? '#ORD-${result.orderId ?? "SUCCESS"}',
        totalAmount: snapshotTotal,
        discount: snapshotDiscount,
        paymentMethod: paymentMethodTitle,
        cashierName: auth.currentUser?.fullName ?? 'Thu ngân',
        branchName: auth.selectedBranch?.branchName ?? 'Chi nhánh',
        items: checkoutItems,
        productNames: productNames,
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(cart.checkoutError ?? 'Không thể hoàn tất đơn hàng.'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  Future<void> _handlePayOsQrPayment(PosCartProvider cart) async {
    setState(() => _isProcessing = true);
    final res = await VietQrDialog.show(
      context,
      amount: cart.finalTotal,
      orderCode: 'POS-${DateTime.now().millisecondsSinceEpoch % 1000000}',
    );

    if (!mounted) return;
    setState(() => _isProcessing = false);

    if (res == true) {
      await _handleCompleteCheckout('Chuyển khoản VietQR (PayOS)', 'VietQR');
    }
  }

  @override
  Widget build(BuildContext context) {
    final cart = context.watch<PosCartProvider>();
    final total = cart.finalTotal;
    final change = (_cashGiven - total).clamp(0.0, double.infinity);

    return Container(
      constraints: BoxConstraints(
        maxHeight: MediaQuery.of(context).size.height * 0.9,
      ),
      padding: EdgeInsets.only(bottom: MediaQuery.of(context).viewInsets.bottom),
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      child: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Handle bar
            Center(
              child: Container(
                margin: const EdgeInsets.only(top: 10, bottom: 8),
                width: 44,
                height: 4,
                decoration: BoxDecoration(
                  color: Colors.grey.shade300,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
            ),

            // Top Total Card
            Container(
              margin: const EdgeInsets.all(16),
              padding: const EdgeInsets.all(18),
              decoration: BoxDecoration(
                color: AppColors.primary,
                borderRadius: BorderRadius.circular(18),
                boxShadow: AppColors.microShadow,
              ),
              child: Column(
                children: [
                  const Text(
                    'SỐ TIỀN CẦN THANH TOÁN',
                    style: TextStyle(color: Colors.white70, fontSize: 12, fontWeight: FontWeight.bold, letterSpacing: 0.5),
                  ),
                  const SizedBox(height: 6),
                  VndText(
                    total,
                    alignment: Alignment.center,
                    style: const TextStyle(
                      fontSize: 26,
                      fontWeight: FontWeight.w800,
                      color: AppColors.emerald,
                    ),
                  ),
                  if (cart.totalDiscount > 0) ...[
                    const SizedBox(height: 4),
                    Text(
                      'Đã áp dụng giảm giá: -${Formatters.formatVND(cart.totalDiscount)}',
                      style: const TextStyle(color: Colors.white60, fontSize: 11),
                    ),
                  ],
                ],
              ),
            ),

            // Payment Mode Selector Tabs (Chỉ 2 kênh: Tiền mặt hoặc Chuyển khoản PayOS)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Row(
                children: [
                  _PaymentTab(
                    icon: Icons.payments_outlined,
                    label: 'Tiền mặt',
                    isSelected: _selectedMode == PaymentMode.cash,
                    onTap: () => setState(() => _selectedMode = PaymentMode.cash),
                  ),
                  const SizedBox(width: 12),
                  _PaymentTab(
                    icon: Icons.qr_code_2_rounded,
                    label: 'Chuyển khoản (VietQR / PayOS)',
                    isSelected: _selectedMode == PaymentMode.transfer,
                    onTap: () => setState(() => _selectedMode = PaymentMode.transfer),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            const Divider(height: 1, color: AppColors.border),

            // Mode Content
            Padding(
              padding: const EdgeInsets.all(16),
              child: _buildModeContent(cart, total, change),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildModeContent(PosCartProvider cart, double total, double change) {
    switch (_selectedMode) {
      case PaymentMode.cash:
        return _buildCashSection(total, change);
      case PaymentMode.transfer:
        return _buildTransferSection(cart);
    }
  }

  Widget _buildCashSection(double total, double change) {
    final presets = [total, 50000.0, 100000.0, 200000.0, 500000.0];
    final validPresets = presets.toSet().where((p) => p >= total || p == total).toList()..sort();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const Text(
          'Gợi ý tiền khách đưa nhanh:',
          style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: AppColors.textSecondary),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: validPresets.map((amt) {
            final isSelected = (_cashGiven == amt);
            return ChoiceChip(
              label: Text(
                Formatters.formatVND(amt),
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                  color: isSelected ? Colors.white : AppColors.textPrimary,
                ),
              ),
              selected: isSelected,
              selectedColor: AppColors.primaryBrand,
              backgroundColor: AppColors.surfaceVariant,
              onSelected: (_) => _selectPresetAmount(amt),
            );
          }).toList(),
        ),
        const SizedBox(height: 12),

        // Custom amount input
        TextField(
          controller: _cashGivenCtrl,
          keyboardType: TextInputType.number,
          decoration: InputDecoration(
            labelText: 'Nhập số tiền khách đưa',
            suffixText: '₫',
            prefixIcon: const Icon(Icons.attach_money_rounded),
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
          ),
          onChanged: (val) {
            final parsed = double.tryParse(val.replaceAll('.', '').replaceAll(',', '')) ?? 0.0;
            setState(() => _cashGiven = parsed);
          },
        ),
        const SizedBox(height: 14),

        // Change display
        Container(
          padding: const EdgeInsets.all(14),
          decoration: BoxDecoration(
            color: change > 0 ? AppColors.emerald.withValues(alpha: 0.1) : AppColors.surfaceVariant,
            borderRadius: BorderRadius.circular(12),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Tiền trả lại khách:',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: change > 0 ? AppColors.emerald : AppColors.textSecondary,
                ),
              ),
              VndText(
                change,
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w800,
                  color: change > 0 ? AppColors.emerald : AppColors.textSecondary,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Checkout Button
        SizedBox(
          height: 48,
          child: ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.emerald,
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
            ),
            onPressed: (_isProcessing || _cashGiven < total)
                ? null
                : () => _handleCompleteCheckout('Tiền mặt (Cash)', 'Cash'),
            child: _isProcessing
                ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2))
                : const Text('XÁC NHẬN THANH TOÁN TIỀN MẶT', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
          ),
        ),
      ],
    );
  }

  Widget _buildTransferSection(PosCartProvider cart) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const Icon(Icons.qr_code_2_rounded, size: 64, color: AppColors.primaryBrand),
        const SizedBox(height: 12),
        const Text(
          'Thanh toán Chuyển khoản VietQR / PayOS',
          textAlign: TextAlign.center,
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 6),
        const Text(
          'Hệ thống tạo mã QR động đúng số tiền đơn hàng.\nỨng dụng tự động lắng nghe cổng PayOS và hoàn tất thanh toán ngay khi khách chuyển khoản thành công.',
          textAlign: TextAlign.center,
          style: TextStyle(fontSize: 12, color: AppColors.textSecondary, height: 1.4),
        ),
        const SizedBox(height: 20),
        SizedBox(
          height: 48,
          child: ElevatedButton.icon(
            icon: const Icon(Icons.qr_code_scanner_rounded),
            label: const Text('MỞ MÃ QR & CHỜ THANH TOÁN', style: TextStyle(fontWeight: FontWeight.bold)),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.primaryBrand,
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
            ),
            onPressed: _isProcessing ? null : () => _handlePayOsQrPayment(cart),
          ),
        ),
      ],
    );
  }
}

class _PaymentTab extends StatelessWidget {
  final IconData icon;
  final String label;
  final bool isSelected;
  final VoidCallback onTap;

  const _PaymentTab({
    required this.icon,
    required this.label,
    required this.isSelected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Container(
          padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 8),
          decoration: BoxDecoration(
            color: isSelected ? AppColors.primaryBrand.withValues(alpha: 0.1) : AppColors.surfaceVariant.withValues(alpha: 0.4),
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: isSelected ? AppColors.primaryBrand : Colors.transparent,
              width: 1.5,
            ),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(icon, size: 22, color: isSelected ? AppColors.primaryBrand : AppColors.textSecondary),
              const SizedBox(height: 6),
              Text(
                label,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
                  color: isSelected ? AppColors.primaryBrand : AppColors.textSecondary,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
