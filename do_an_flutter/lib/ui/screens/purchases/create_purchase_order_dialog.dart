import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/inventory_model.dart';
import '../../../data/models/product_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';

class CreatePurchaseOrderDialog extends StatefulWidget {
  final VoidCallback onCreated;

  const CreatePurchaseOrderDialog({
    super.key,
    required this.onCreated,
  });

  static Future<void> show(BuildContext context, {required VoidCallback onCreated}) {
    return showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => CreatePurchaseOrderDialog(onCreated: onCreated),
    );
  }

  @override
  State<CreatePurchaseOrderDialog> createState() => _CreatePurchaseOrderDialogState();
}

class _CreatePurchaseOrderDialogState extends State<CreatePurchaseOrderDialog> {
  final _repo = DataRepository();
  bool _isLoading = true;
  bool _isSubmitting = false;

  List<SupplierModel> _suppliers = [];
  List<WarehouseModel> _warehouses = [];
  List<ProductModel> _allProducts = [];

  SupplierModel? _selectedSupplier;
  WarehouseModel? _selectedWarehouse;
  final TextEditingController _notesController = TextEditingController();

  final List<_PurchaseItemDraft> _items = [];

  @override
  void initState() {
    super.initState();
    _loadInitialData();
  }

  @override
  void dispose() {
    _notesController.dispose();
    super.dispose();
  }

  Future<void> _loadInitialData() async {
    setState(() => _isLoading = true);
    final auth = context.read<AuthProvider>();
    final branchId = auth.effectiveBranchId;

    try {
      final results = await Future.wait([
        _repo.getSuppliers(),
        _repo.getWarehousesByBranch(branchId),
        _repo.getProducts(length: 100, branchId: branchId),
      ]);

      if (mounted) {
        setState(() {
          _suppliers = results[0] as List<SupplierModel>;
          _warehouses = results[1] as List<WarehouseModel>;
          final prodResp = results[2] as ProductPagedResponse;
          _allProducts = prodResp.data;

          if (_suppliers.isNotEmpty) _selectedSupplier = _suppliers.first;
          if (_warehouses.isNotEmpty) _selectedWarehouse = _warehouses.first;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  void _addItem() {
    if (_allProducts.isEmpty) return;
    final defaultProduct = _allProducts.first;
    setState(() {
      _items.add(_PurchaseItemDraft(
        product: defaultProduct,
        quantity: 1,
        unitPrice: (defaultProduct.costPrice > 0 ? defaultProduct.costPrice : defaultProduct.retailPrice * 0.7).toDouble(),
      ));
    });
  }

  void _removeItem(int index) {
    setState(() {
      _items.removeAt(index);
    });
  }

  double get _totalAmount {
    return _items.fold(0.0, (sum, item) => sum + (item.quantity * item.unitPrice));
  }

  Future<void> _submit() async {
    if (_selectedSupplier == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(backgroundColor: AppColors.error, content: Text('Vui lòng chọn Nhà cung cấp!')),
      );
      return;
    }
    if (_selectedWarehouse == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(backgroundColor: AppColors.error, content: Text('Vui lòng chọn Kho nhận hàng!')),
      );
      return;
    }
    if (_items.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(backgroundColor: AppColors.error, content: Text('Vui lòng thêm ít nhất 1 mặt hàng nhập!')),
      );
      return;
    }

    setState(() => _isSubmitting = true);
    final auth = context.read<AuthProvider>();
    final userId = auth.currentUser?.userId ?? 1;
    final branchId = auth.effectiveBranchId ?? _selectedWarehouse!.branchId;

    final details = _items.map((item) {
      return PurchaseOrderDetailModel(
        purchaseOrderDetailId: 0,
        purchaseOrderId: 0,
        productId: item.product.productId,
        productName: item.product.productName,
        categoryId: item.product.categoryId ?? 0,
        lotId: 0,
        quantity: item.quantity,
        unit: 'Đơn vị',
        unitPrice: item.unitPrice,
        totalPrice: item.quantity * item.unitPrice,
      );
    }).toList();

    final order = PurchaseOrderModel(
      purchaseOrderId: 0,
      supplierId: _selectedSupplier!.supplierId,
      warehouseId: _selectedWarehouse!.warehouseId,
      userId: userId,
      branchId: branchId,
      orderDate: DateTime.now(),
      status: 'COMPLETED',
      totalAmount: _totalAmount,
      notes: _notesController.text.trim().isEmpty ? null : _notesController.text.trim(),
      details: details,
    );

    final success = await _repo.createPurchase(order);

    if (mounted) {
      setState(() => _isSubmitting = false);
      if (success) {
        Navigator.pop(context);
        widget.onCreated();
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            backgroundColor: AppColors.success,
            content: Text('Tạo phiếu nhập hàng thành công! Tổng tiền: ${Formatters.formatVND(_totalAmount)}'),
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            backgroundColor: AppColors.error,
            content: Text('Lỗi khi tạo phiếu nhập hàng. Vui lòng kiểm tra lại!'),
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
        constraints: const BoxConstraints(maxWidth: 580, maxHeight: 720),
        child: Padding(
          padding: const EdgeInsets.all(22),
          child: _isLoading
              ? const Center(child: CircularProgressIndicator())
              : Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Header
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            color: AppColors.primary.withOpacity(0.12),
                            borderRadius: BorderRadius.circular(12),
                          ),
                          child: const Icon(Icons.inventory_2_rounded, color: AppColors.primary, size: 24),
                        ),
                        const SizedBox(width: 12),
                        const Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                'Tạo Phiếu Nhập Hàng (Inbound)',
                                style: TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                              ),
                              Text(
                                'Nhập hàng từ NCC & tăng tồn kho tự động',
                                style: TextStyle(fontSize: 12, color: AppColors.textSecondary),
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
                    const Divider(height: 24),

                    // Body
                    Expanded(
                      child: SingleChildScrollView(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          children: [
                            // Select Supplier & Warehouse
                            Row(
                              children: [
                                Expanded(
                                  child: DropdownButtonFormField<SupplierModel>(
                                    value: _selectedSupplier,
                                    decoration: InputDecoration(
                                      labelText: 'Nhà cung cấp *',
                                      prefixIcon: const Icon(Icons.storefront_rounded, size: 20),
                                      border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                                    ),
                                    items: _suppliers.map((s) {
                                      return DropdownMenuItem(
                                        value: s,
                                        child: Text(s.name, overflow: TextOverflow.ellipsis),
                                      );
                                    }).toList(),
                                    onChanged: (val) => setState(() => _selectedSupplier = val),
                                  ),
                                ),
                                const SizedBox(width: 12),
                                Expanded(
                                  child: DropdownButtonFormField<WarehouseModel>(
                                    value: _selectedWarehouse,
                                    decoration: InputDecoration(
                                      labelText: 'Kho nhập *',
                                      prefixIcon: const Icon(Icons.warehouse_rounded, size: 20),
                                      border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                                    ),
                                    items: _warehouses.map((w) {
                                      return DropdownMenuItem(
                                        value: w,
                                        child: Text(w.name, overflow: TextOverflow.ellipsis),
                                      );
                                    }).toList(),
                                    onChanged: (val) => setState(() => _selectedWarehouse = val),
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 16),

                            // Items Header
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Row(
                                  children: [
                                    const Text('Danh sách mặt hàng nhập', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
                                    const SizedBox(width: 6),
                                    Container(
                                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                                      decoration: BoxDecoration(
                                        color: AppColors.primary.withOpacity(0.12),
                                        borderRadius: BorderRadius.circular(10),
                                      ),
                                      child: Text(
                                        '${_items.length}',
                                        style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: AppColors.primary),
                                      ),
                                    ),
                                  ],
                                ),
                                TextButton.icon(
                                  onPressed: _addItem,
                                  icon: const Icon(Icons.add, size: 18),
                                  label: const Text('Thêm món'),
                                  style: TextButton.styleFrom(
                                    foregroundColor: AppColors.primary,
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 8),

                            // Items List
                            if (_items.isEmpty)
                              Container(
                                padding: const EdgeInsets.all(24),
                                decoration: BoxDecoration(
                                  color: AppColors.surface,
                                  borderRadius: BorderRadius.circular(12),
                                  border: Border.all(color: AppColors.border),
                                ),
                                child: Center(
                                  child: Column(
                                    children: [
                                      const Icon(Icons.add_shopping_cart_rounded, size: 36, color: AppColors.textMuted),
                                      const SizedBox(height: 8),
                                      const Text('Chưa có sản phẩm nào', style: TextStyle(fontSize: 13, color: AppColors.textSecondary)),
                                      TextButton(
                                        onPressed: _addItem,
                                        child: const Text('+ Thêm sản phẩm đầu tiên'),
                                      ),
                                    ],
                                  ),
                                ),
                              )
                            else
                              ListView.separated(
                                shrinkWrap: true,
                                physics: const NeverScrollableScrollPhysics(),
                                itemCount: _items.length,
                                separatorBuilder: (_, __) => const SizedBox(height: 8),
                                itemBuilder: (context, idx) {
                                  final item = _items[idx];
                                  return _buildItemRow(item, idx);
                                },
                              ),

                            const SizedBox(height: 16),
                            TextField(
                              controller: _notesController,
                              decoration: InputDecoration(
                                labelText: 'Ghi chú đơn nhập',
                                hintText: 'VD: Nhập lô hàng định kỳ theo hợp đồng...',
                                prefixIcon: const Icon(Icons.note_alt_outlined, size: 20),
                                border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),

                    const Divider(height: 20),

                    // Footer with Total Amount & Submit Button
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text('Tổng tiền nhập:', style: TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                            Text(
                              Formatters.formatVND(_totalAmount),
                              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.primary),
                            ),
                          ],
                        ),
                        Row(
                          children: [
                            TextButton(
                              onPressed: _isSubmitting ? null : () => Navigator.pop(context),
                              child: const Text('Hủy bỏ', style: TextStyle(color: AppColors.textSecondary)),
                            ),
                            const SizedBox(width: 10),
                            ElevatedButton.icon(
                              onPressed: _isSubmitting ? null : _submit,
                              icon: _isSubmitting
                                  ? const SizedBox(
                                      width: 16,
                                      height: 16,
                                      child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                                    )
                                  : const Icon(Icons.check_circle_rounded, size: 18),
                              label: const Text('Nhập hàng ngay'),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: AppColors.primary,
                                foregroundColor: Colors.white,
                                padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
                                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ],
                ),
        ),
      ),
    );
  }

  Widget _buildItemRow(_PurchaseItemDraft item, int index) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: DropdownButtonFormField<ProductModel>(
                  value: item.product,
                  isExpanded: true,
                  decoration: InputDecoration(
                    labelText: 'Sản phẩm',
                    contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                  items: _allProducts.map((p) {
                    return DropdownMenuItem(
                      value: p,
                      child: Text(p.productName, overflow: TextOverflow.ellipsis, style: const TextStyle(fontSize: 13)),
                    );
                  }).toList(),
                  onChanged: (val) {
                    if (val != null) {
                      setState(() {
                        item.product = val;
                        item.unitPrice = (val.costPrice > 0 ? val.costPrice : val.retailPrice * 0.7).toDouble();
                      });
                    }
                  },
                ),
              ),
              IconButton(
                icon: const Icon(Icons.delete_outline_rounded, color: AppColors.error, size: 20),
                onPressed: () => _removeItem(index),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              Expanded(
                flex: 2,
                child: TextFormField(
                  initialValue: item.quantity.toString(),
                  keyboardType: TextInputType.number,
                  decoration: InputDecoration(
                    labelText: 'Số lượng',
                    contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                  onChanged: (v) {
                    final qty = int.tryParse(v) ?? 1;
                    setState(() => item.quantity = qty > 0 ? qty : 1);
                  },
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                flex: 3,
                child: TextFormField(
                  initialValue: item.unitPrice.toStringAsFixed(0),
                  keyboardType: TextInputType.number,
                  decoration: InputDecoration(
                    labelText: 'Giá nhập (VNĐ)',
                    contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                  onChanged: (v) {
                    final price = double.tryParse(v) ?? 0;
                    setState(() => item.unitPrice = price >= 0 ? price : 0);
                  },
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                flex: 3,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    const Text('Thành tiền', style: TextStyle(fontSize: 10, color: AppColors.textSecondary)),
                    Text(
                      Formatters.formatVND(item.quantity * item.unitPrice),
                      style: const TextStyle(fontSize: 13, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _PurchaseItemDraft {
  ProductModel product;
  int quantity;
  double unitPrice;

  _PurchaseItemDraft({
    required this.product,
    required this.quantity,
    required this.unitPrice,
  });
}
