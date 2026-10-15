import 'package:flutter/material.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/customer_model.dart';
import '../../../data/models/product_model.dart';
import '../../../data/models/quotation_model.dart';
import '../../../data/repositories/data_repository.dart';

class CreateQuotationDialog extends StatefulWidget {
  final int branchId;

  const CreateQuotationDialog({super.key, required this.branchId});

  @override
  State<CreateQuotationDialog> createState() => _CreateQuotationDialogState();
}

class _CreateQuotationDialogState extends State<CreateQuotationDialog> {
  final _repository = DataRepository();
  final _formKey = GlobalKey<FormState>();

  final _customerNameCtrl = TextEditingController();
  final _customerPhoneCtrl = TextEditingController();
  final _noteCtrl = TextEditingController();
  final _discountReasonCtrl = TextEditingController();

  List<CustomerModel> _allCustomers = [];
  CustomerModel? _selectedCustomer;
  List<ProductModel> _allProducts = [];

  final List<QuotationDetailModel> _items = [];
  final double _manualDiscount = 0;
  bool _isLoading = true;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _loadInitialData();
  }

  @override
  void dispose() {
    _customerNameCtrl.dispose();
    _customerPhoneCtrl.dispose();
    _noteCtrl.dispose();
    _discountReasonCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadInitialData() async {
    try {
      final res = await Future.wait([
        _repository.getCustomers(),
        _repository.getProducts(length: 100),
      ]);
      if (mounted) {
        setState(() {
          _allCustomers = res[0] as List<CustomerModel>;
          final productRes = res[1] as ProductPagedResponse;
          _allProducts = productRes.data;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  double get _subtotal {
    return _items.fold(0.0, (sum, item) => sum + item.totalPrice);
  }

  double get _finalTotal {
    final t = _subtotal - _manualDiscount;
    return t > 0 ? t : 0;
  }

  void _addItem(ProductModel product) {
    setState(() {
      final existingIndex = _items.indexWhere((e) => e.productId == product.productId);
      if (existingIndex >= 0) {
        final current = _items[existingIndex];
        final newQty = current.quantity + 1;
        _items[existingIndex] = QuotationDetailModel(
          productId: current.productId,
          productName: current.productName,
          productImage: current.productImage,
          unit: current.unit,
          quantity: newQty,
          unitPrice: current.unitPrice,
          discount: current.discount,
          totalPrice: (current.unitPrice * newQty) - current.discount,
        );
      } else {
        _items.add(QuotationDetailModel(
          productId: product.productId,
          productName: product.name,
          productImage: product.imageUrl,
          quantity: 1,
          unitPrice: product.price,
          discount: 0,
          totalPrice: product.price,
        ));
      }
    });
  }

  void _showProductPicker() {
    showModalBottomSheet(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (ctx) {
        return StatefulBuilder(
          builder: (context, setModalState) {
            return Container(
              padding: const EdgeInsets.all(16),
              child: Column(
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text(
                        'Chọn sản phẩm vào báo giá',
                        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                      IconButton(icon: const Icon(Icons.close), onPressed: () => Navigator.pop(ctx)),
                    ],
                  ),
                  const Divider(),
                  Expanded(
                    child: ListView.separated(
                      itemCount: _allProducts.length,
                      separatorBuilder: (_, _) => const Divider(height: 1),
                      itemBuilder: (context, index) {
                        final p = _allProducts[index];
                        return ListTile(
                          title: Text(p.name, style: const TextStyle(fontWeight: FontWeight.w600)),
                          subtitle: Text('Giá: ${Formatters.formatVND(p.price)}'),
                          trailing: ElevatedButton.icon(
                            style: ElevatedButton.styleFrom(
                              backgroundColor: AppColors.primary,
                              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                            ),
                            icon: const Icon(Icons.add, size: 16, color: Colors.white),
                            label: const Text('Thêm', style: TextStyle(color: Colors.white, fontSize: 12)),
                            onPressed: () {
                              _addItem(p);
                              Navigator.pop(ctx);
                            },
                          ),
                        );
                      },
                    ),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  Future<void> _save() async {
    if (_items.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng thêm ít nhất 1 sản phẩm vào báo giá')),
      );
      return;
    }

    setState(() => _isSaving = true);

    try {
      final code = 'BG-${DateTime.now().millisecondsSinceEpoch.toString().substring(7)}';
      final quotation = QuotationModel(
        quotationCode: code,
        branchId: widget.branchId,
        customerId: _selectedCustomer?.customerId,
        customerName: _selectedCustomer?.fullName ?? _customerNameCtrl.text.trim(),
        customerPhone: _selectedCustomer?.phone ?? _customerPhoneCtrl.text.trim(),
        issueDate: DateTime.now(),
        expiryDate: DateTime.now().add(const Duration(days: 7)),
        totalAmount: _subtotal,
        discountAmount: _manualDiscount,
        manualDiscount: _manualDiscount,
        discountReason: _discountReasonCtrl.text.trim(),
        finalAmount: _finalTotal,
        status: 'Draft',
        note: _noteCtrl.text.trim(),
        details: _items,
      );

      final id = await _repository.saveQuotation(quotation);
      if (!mounted) return;
      setState(() => _isSaving = false);

      if (id != null && id > 0) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Tạo báo giá #$code thành công!'), backgroundColor: AppColors.success),
        );
        Navigator.pop(context, true);
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Lưu báo giá thất bại.'), backgroundColor: AppColors.error),
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
        constraints: const BoxConstraints(maxWidth: 540, maxHeight: 680),
        child: _isLoading
            ? const Center(child: CircularProgressIndicator())
            : Padding(
                padding: const EdgeInsets.all(20),
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      // Header
                      Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.all(8),
                            decoration: BoxDecoration(
                              color: AppColors.primary.withValues(alpha: 0.1),
                              shape: BoxShape.circle,
                            ),
                            child: const Icon(Icons.request_quote_outlined, color: AppColors.primary, size: 24),
                          ),
                          const SizedBox(width: 12),
                          const Expanded(
                            child: Text(
                              'Lập bảng báo giá mới',
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

                      // Thông tin khách hàng
                      Row(
                        children: [
                          Expanded(
                            child: DropdownButtonFormField<CustomerModel?>(
                              initialValue: _selectedCustomer,
                              isExpanded: true,
                              decoration: InputDecoration(
                                labelText: 'Chọn khách hàng có sẵn',
                                prefixIcon: const Icon(Icons.person_search_outlined),
                                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                                border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                              ),
                              items: [
                                const DropdownMenuItem(value: null, child: Text('Nhập khách tự do...')),
                                ..._allCustomers.map((c) => DropdownMenuItem(
                                      value: c,
                                      child: Text('${c.fullName} - ${c.phone ?? ''} (${c.tierName})'),
                                    )),
                              ],
                              onChanged: (val) {
                                setState(() {
                                  _selectedCustomer = val;
                                  if (val != null) {
                                    _customerNameCtrl.text = val.fullName;
                                    _customerPhoneCtrl.text = val.phone ?? '';
                                  }
                                });
                              },
                            ),
                          ),
                        ],
                      ),
                      if (_selectedCustomer == null) ...[
                        const SizedBox(height: 10),
                        Row(
                          children: [
                            Expanded(
                              child: TextFormField(
                                controller: _customerNameCtrl,
                                decoration: InputDecoration(
                                  labelText: 'Tên người nhận báo giá *',
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                ),
                              ),
                            ),
                            const SizedBox(width: 10),
                            Expanded(
                              child: TextFormField(
                                controller: _customerPhoneCtrl,
                                keyboardType: TextInputType.phone,
                                decoration: InputDecoration(
                                  labelText: 'Số điện thoại',
                                  contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ],
                      const SizedBox(height: 14),

                      // Danh sách hàng hóa báo giá
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text('Sản phẩm (${_items.length})', style: const TextStyle(fontWeight: FontWeight.bold)),
                          TextButton.icon(
                            icon: const Icon(Icons.add_shopping_cart, size: 16),
                            label: const Text('Thêm món'),
                            onPressed: _showProductPicker,
                          ),
                        ],
                      ),

                      // Danh sách items
                      Expanded(
                        child: _items.isEmpty
                            ? Container(
                                decoration: BoxDecoration(
                                  color: AppColors.background,
                                  borderRadius: BorderRadius.circular(10),
                                  border: Border.all(color: AppColors.border),
                                ),
                                child: const Center(
                                  child: Text('Chưa có sản phẩm nào. Nhấn "+ Thêm món" ở trên.', style: TextStyle(color: AppColors.textMuted)),
                                ),
                              )
                            : ListView.separated(
                                itemCount: _items.length,
                                separatorBuilder: (_, _) => const Divider(height: 1),
                                itemBuilder: (context, index) {
                                  final item = _items[index];
                                  return ListTile(
                                    dense: true,
                                    title: Text(item.productName, style: const TextStyle(fontWeight: FontWeight.bold)),
                                    subtitle: Text('${Formatters.formatVND(item.unitPrice)} x ${item.quantity}'),
                                    trailing: Row(
                                      mainAxisSize: MainAxisSize.min,
                                      children: [
                                        Text(Formatters.formatVND(item.totalPrice), style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.primary)),
                                        IconButton(
                                          icon: const Icon(Icons.delete_outline, size: 18, color: AppColors.error),
                                          onPressed: () => setState(() => _items.removeAt(index)),
                                        ),
                                      ],
                                    ),
                                  );
                                },
                              ),
                      ),
                      const SizedBox(height: 12),

                      // Tổng tiền & Giảm giá
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: AppColors.primary.withValues(alpha: 0.05),
                          borderRadius: BorderRadius.circular(10),
                          border: Border.all(color: AppColors.primary.withValues(alpha: 0.2)),
                        ),
                        child: Column(
                          children: [
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                const Text('Tạm tính:'),
                                Text(Formatters.formatVND(_subtotal), style: const TextStyle(fontWeight: FontWeight.bold)),
                              ],
                            ),
                            const SizedBox(height: 6),
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                const Text('Tổng cộng báo giá:', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                                Text(
                                  Formatters.formatVND(_finalTotal),
                                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18, color: AppColors.primary),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 14),

                      // Nút Lưu
                      Row(
                        children: [
                          Expanded(
                            child: OutlinedButton(
                              onPressed: _isSaving ? null : () => Navigator.pop(context),
                              child: const Text('Hủy'),
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: ElevatedButton(
                              style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
                              onPressed: _isSaving ? null : _save,
                              child: _isSaving
                                  ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2))
                                  : const Text('Lưu Báo giá', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
      ),
    );
  }
}
