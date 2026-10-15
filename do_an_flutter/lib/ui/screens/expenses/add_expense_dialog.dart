import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../data/models/expense_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';

class AddExpenseDialog extends StatefulWidget {
  const AddExpenseDialog({super.key});

  @override
  State<AddExpenseDialog> createState() => _AddExpenseDialogState();
}

class _AddExpenseDialogState extends State<AddExpenseDialog> {
  final _formKey = GlobalKey<FormState>();
  final _repository = DataRepository();

  final _amountCtrl = TextEditingController();
  final _noteCtrl = TextEditingController();

  List<ExpenseCategoryModel> _categories = [];
  ExpenseCategoryModel? _selectedCategory;
  String _paymentMethod = 'Cash';
  bool _isLoading = true;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _loadCategories();
  }

  @override
  void dispose() {
    _amountCtrl.dispose();
    _noteCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadCategories() async {
    try {
      final cats = await _repository.getExpenseCategories();
      if (mounted) {
        setState(() {
          _categories = cats;
          if (cats.isNotEmpty) _selectedCategory = cats.first;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedCategory == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn loại chi phí')),
      );
      return;
    }

    final amount = double.tryParse(_amountCtrl.text.replaceAll(RegExp(r'[^0-9]'), '')) ?? 0;
    if (amount <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng nhập số tiền chi hợp lệ')),
      );
      return;
    }

    setState(() => _isSaving = true);
    final auth = context.read<AuthProvider>();
    final branchId = auth.selectedBranch?.branchId ?? 1;
    final userId = auth.currentUser?.userId ?? 1;

    try {
      final req = CreateExpenseRequest(
        branchId: branchId,
        categoryId: _selectedCategory!.categoryId,
        userId: userId,
        amount: amount,
        note: _noteCtrl.text.trim().isEmpty ? null : _noteCtrl.text.trim(),
        paymentMethod: _paymentMethod,
      );

      final ok = await _repository.createExpense(req);
      if (!mounted) return;
      setState(() => _isSaving = false);

      if (ok) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Tạo phiếu chi thành công!'), backgroundColor: AppColors.success),
        );
        Navigator.pop(context, true);
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Tạo phiếu chi thất bại.'), backgroundColor: AppColors.error),
        );
      }
    } catch (_) {
      if (mounted) setState(() => _isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 460),
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: _isLoading
              ? const SizedBox(height: 200, child: Center(child: CircularProgressIndicator()))
              : Form(
                  key: _formKey,
                  child: SingleChildScrollView(
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        Row(
                          children: [
                            Container(
                              padding: const EdgeInsets.all(8),
                              decoration: BoxDecoration(
                                color: AppColors.error.withValues(alpha: 0.1),
                                shape: BoxShape.circle,
                              ),
                              child: const Icon(Icons.receipt_long, color: AppColors.error, size: 24),
                            ),
                            const SizedBox(width: 12),
                            const Expanded(
                              child: Text(
                                'Tạo phiếu chi (5 giây)',
                                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                              ),
                            ),
                            IconButton(
                              icon: const Icon(Icons.close, color: AppColors.textMuted),
                              onPressed: () => Navigator.pop(context),
                            ),
                          ],
                        ),
                        const Divider(height: 20),

                        // Số tiền
                        TextFormField(
                          controller: _amountCtrl,
                          keyboardType: TextInputType.number,
                          style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: AppColors.error),
                          decoration: InputDecoration(
                            labelText: 'Số tiền chi (VNĐ) *',
                            prefixIcon: const Icon(Icons.attach_money, color: AppColors.error),
                            border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                          ),
                          validator: (v) {
                            if (v == null || v.trim().isEmpty) return 'Vui lòng nhập số tiền chi';
                            return null;
                          },
                        ),
                        const SizedBox(height: 14),

                        // Danh mục chi phí
                        DropdownButtonFormField<ExpenseCategoryModel>(
                          initialValue: _selectedCategory,
                          decoration: InputDecoration(
                            labelText: 'Loại chi phí *',
                            prefixIcon: const Icon(Icons.category_outlined),
                            border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                          ),
                          items: _categories.map((c) => DropdownMenuItem(value: c, child: Text(c.name))).toList(),
                          onChanged: (val) {
                            if (val != null) setState(() => _selectedCategory = val);
                          },
                        ),
                        const SizedBox(height: 14),

                        // Hình thức thanh toán
                        Row(
                          children: [
                            const Text('Hình thức:', style: TextStyle(fontWeight: FontWeight.w600)),
                            const SizedBox(width: 12),
                            ChoiceChip(
                              label: const Text('Tiền mặt'),
                              selected: _paymentMethod == 'Cash',
                              onSelected: (_) => setState(() => _paymentMethod = 'Cash'),
                            ),
                            const SizedBox(width: 8),
                            ChoiceChip(
                              label: const Text('Chuyển khoản'),
                              selected: _paymentMethod == 'Transfer',
                              onSelected: (_) => setState(() => _paymentMethod = 'Transfer'),
                            ),
                          ],
                        ),
                        const SizedBox(height: 14),

                        // Ghi chú / Lý do
                        TextFormField(
                          controller: _noteCtrl,
                          maxLines: 2,
                          decoration: InputDecoration(
                            labelText: 'Lý do chi / Ghi chú',
                            prefixIcon: const Icon(Icons.notes_outlined),
                            border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                          ),
                        ),
                        const SizedBox(height: 24),

                        // Nút bấm
                        Row(
                          children: [
                            Expanded(
                              child: OutlinedButton(
                                onPressed: _isSaving ? null : () => Navigator.pop(context),
                                style: OutlinedButton.styleFrom(padding: const EdgeInsets.symmetric(vertical: 14)),
                                child: const Text('Hủy'),
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: ElevatedButton(
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: AppColors.error,
                                  padding: const EdgeInsets.symmetric(vertical: 14),
                                ),
                                onPressed: _isSaving ? null : _save,
                                child: _isSaving
                                    ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2))
                                    : const Text('Lưu phiếu chi', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
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
