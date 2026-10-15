import 'package:flutter/material.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/product_model.dart';
import '../../../data/repositories/data_repository.dart';

class PaySupplierDebtDialog extends StatefulWidget {
  final SupplierDebtModel debt;
  final VoidCallback onPaid;

  const PaySupplierDebtDialog({
    super.key,
    required this.debt,
    required this.onPaid,
  });

  static Future<void> show(
    BuildContext context, {
    required SupplierDebtModel debt,
    required VoidCallback onPaid,
  }) {
    return showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => PaySupplierDebtDialog(
        debt: debt,
        onPaid: onPaid,
      ),
    );
  }

  @override
  State<PaySupplierDebtDialog> createState() => _PaySupplierDebtDialogState();
}

class _PaySupplierDebtDialogState extends State<PaySupplierDebtDialog> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _amountController;
  late TextEditingController _proofController;
  String _selectedMethod = 'Tiền mặt';
  bool _isLoading = false;
  final _repo = DataRepository();

  final List<String> _methods = ['Tiền mặt', 'Chuyển khoản', 'Thẻ / POS', 'Khác'];

  @override
  void initState() {
    super.initState();
    // Default to remaining amount
    final remaining = widget.debt.remainingAmount;
    _amountController = TextEditingController(
      text: remaining > 0 ? remaining.toStringAsFixed(0) : '',
    );
    _proofController = TextEditingController();
  }

  @override
  void dispose() {
    _amountController.dispose();
    _proofController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final payAmount = double.tryParse(_amountController.text.replaceAll(',', '').replaceAll('.', '')) ?? 0;
    if (payAmount <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          backgroundColor: AppColors.error,
          content: Text('Số tiền thanh toán phải lớn hơn 0!'),
        ),
      );
      return;
    }

    setState(() => _isLoading = true);

    final success = await _repo.paySupplierDebt(
      debtId: widget.debt.debtId,
      amount: payAmount,
      paymentMethod: _selectedMethod,
      proof: _proofController.text.trim().isEmpty ? null : _proofController.text.trim(),
    );

    if (mounted) {
      setState(() => _isLoading = false);
      if (success) {
        Navigator.pop(context);
        widget.onPaid();
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            backgroundColor: AppColors.success,
            content: Text('Đã ghi nhận thanh toán ${Formatters.formatVND(payAmount)} thành công!'),
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            backgroundColor: AppColors.error,
            content: Text('Không thể thanh toán công nợ. Vui lòng kiểm tra lại!'),
          ),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 440),
        child: Padding(
          padding: const EdgeInsets.all(22),
          child: Form(
            key: _formKey,
            child: SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Row(
                    children: [
                      Container(
                        padding: const EdgeInsets.all(10),
                        decoration: BoxDecoration(
                          color: AppColors.success.withOpacity(0.12),
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: const Icon(Icons.payment_rounded, color: AppColors.success, size: 24),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text(
                              'Thanh toán Công nợ NCC',
                              style: TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                            ),
                            Text(
                              'Mã phiếu nợ: #${widget.debt.debtId}',
                              style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
                            ),
                          ],
                        ),
                      ),
                      IconButton(
                        icon: const Icon(Icons.close_rounded, color: AppColors.textSecondary),
                        onPressed: () => Navigator.pop(context),
                      ),
                    ],
                  ),
                  const SizedBox(height: 16),
                  Container(
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(
                      color: AppColors.surface,
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: AppColors.border),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          widget.debt.supplierName.isNotEmpty ? widget.debt.supplierName : 'Nhà cung cấp',
                          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                        ),
                        const SizedBox(height: 6),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            const Text('Tổng nợ phiếu:', style: TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                            Text(Formatters.formatVND(widget.debt.totalDebtAmount), style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                          ],
                        ),
                        const SizedBox(height: 4),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            const Text('Đã thanh toán:', style: TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                            Text(Formatters.formatVND(widget.debt.paidAmount), style: const TextStyle(color: AppColors.success, fontWeight: FontWeight.w600, fontSize: 13)),
                          ],
                        ),
                        const Divider(height: 12),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            const Text('Còn phải trả:', style: TextStyle(fontSize: 13, fontWeight: FontWeight.bold, color: AppColors.error)),
                            Text(
                              Formatters.formatVND(widget.debt.remainingAmount),
                              style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold, color: AppColors.error),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _amountController,
                    keyboardType: TextInputType.number,
                    decoration: InputDecoration(
                      labelText: 'Số tiền thanh toán (VNĐ) *',
                      prefixIcon: const Icon(Icons.monetization_on_outlined, size: 20),
                      border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                      suffixText: 'đ',
                    ),
                    validator: (v) => (v == null || v.trim().isEmpty) ? 'Vui lòng nhập số tiền' : null,
                  ),
                  const SizedBox(height: 14),
                  DropdownButtonFormField<String>(
                    value: _selectedMethod,
                    decoration: InputDecoration(
                      labelText: 'Phương thức thanh toán',
                      prefixIcon: const Icon(Icons.account_balance_wallet_outlined, size: 20),
                      border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                    ),
                    items: _methods.map((m) => DropdownMenuItem(value: m, child: Text(m))).toList(),
                    onChanged: (val) {
                      if (val != null) setState(() => _selectedMethod = val);
                    },
                  ),
                  const SizedBox(height: 14),
                  TextFormField(
                    controller: _proofController,
                    decoration: InputDecoration(
                      labelText: 'Ghi chú / Mã tham chiếu',
                      hintText: 'VD: CK UNC-123456 qua Vietcombank',
                      prefixIcon: const Icon(Icons.note_alt_outlined, size: 20),
                      border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                    ),
                  ),
                  const SizedBox(height: 22),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      TextButton(
                        onPressed: _isLoading ? null : () => Navigator.pop(context),
                        child: const Text('Hủy bỏ', style: TextStyle(color: AppColors.textSecondary)),
                      ),
                      const SizedBox(width: 12),
                      ElevatedButton.icon(
                        onPressed: _isLoading ? null : _submit,
                        icon: _isLoading
                            ? const SizedBox(
                                width: 16,
                                height: 16,
                                child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                              )
                            : const Icon(Icons.check_circle_rounded, size: 18),
                        label: const Text('Xác nhận thanh toán'),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: AppColors.success,
                          foregroundColor: Colors.white,
                          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
