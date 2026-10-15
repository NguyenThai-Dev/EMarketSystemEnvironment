import 'package:flutter/material.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/inventory_model.dart';
import '../../../data/repositories/data_repository.dart';

class PurchaseDetailDialog extends StatefulWidget {
  final int purchaseOrderId;
  final VoidCallback onUpdated;

  const PurchaseDetailDialog({
    super.key,
    required this.purchaseOrderId,
    required this.onUpdated,
  });

  static Future<void> show(BuildContext context, {required int purchaseOrderId, required VoidCallback onUpdated}) {
    return showDialog(
      context: context,
      builder: (context) => PurchaseDetailDialog(
        purchaseOrderId: purchaseOrderId,
        onUpdated: onUpdated,
      ),
    );
  }

  @override
  State<PurchaseDetailDialog> createState() => _PurchaseDetailDialogState();
}

class _PurchaseDetailDialogState extends State<PurchaseDetailDialog> {
  final _repo = DataRepository();
  bool _isLoading = true;
  PurchaseOrderModel? _order;

  @override
  void initState() {
    super.initState();
    _loadDetail();
  }

  Future<void> _loadDetail() async {
    setState(() => _isLoading = true);
    final data = await _repo.getPurchaseDetail(widget.purchaseOrderId);
    if (mounted) {
      setState(() {
        _order = data;
        _isLoading = false;
      });
    }
  }

  Future<void> _deleteOrder() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Row(
          children: [
            Icon(Icons.warning_amber_rounded, color: AppColors.error),
            SizedBox(width: 8),
            Text('Hủy phiếu nhập hàng', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
          ],
        ),
        content: Text(
          'Bạn có chắc muốn xóa/hủy phiếu nhập #${widget.purchaseOrderId}?\nThao tác này sẽ hủy ghi nhận lô hàng tương ứng.',
          style: const TextStyle(fontSize: 13),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Bỏ qua', style: TextStyle(color: AppColors.textSecondary)),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.error, foregroundColor: Colors.white),
            child: const Text('Xác nhận xóa'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      final success = await _repo.deletePurchase(widget.purchaseOrderId);
      if (mounted) {
        if (success) {
          Navigator.pop(context);
          widget.onUpdated();
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(backgroundColor: AppColors.success, content: Text('Đã hủy phiếu nhập hàng thành công!')),
          );
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(backgroundColor: AppColors.error, content: Text('Không thể xóa phiếu nhập hàng!')),
          );
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 520, maxHeight: 680),
        child: Padding(
          padding: const EdgeInsets.all(22),
          child: _isLoading
              ? const Center(child: CircularProgressIndicator())
              : _order == null
                  ? const Center(child: Text('Không tìm thấy thông tin đơn nhập hàng'))
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
                              child: const Icon(Icons.receipt_long_rounded, color: AppColors.primary, size: 24),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Phiếu nhập hàng #${_order!.purchaseOrderId}',
                                    style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                                  ),
                                  if (_order!.orderDate != null)
                                    Text(
                                      Formatters.formatDateTime(_order!.orderDate),
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
                        const Divider(height: 24),

                        // Body
                        Expanded(
                          child: SingleChildScrollView(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                // Info Box
                                Container(
                                  padding: const EdgeInsets.all(14),
                                  decoration: BoxDecoration(
                                    color: AppColors.surface,
                                    borderRadius: BorderRadius.circular(12),
                                    border: Border.all(color: AppColors.border),
                                  ),
                                  child: Column(
                                    children: [
                                      _buildInfoRow('Nhà cung cấp:', _order!.supplierName ?? 'NCC #${_order!.supplierId}'),
                                      const SizedBox(height: 6),
                                      _buildInfoRow('Kho nhập hàng:', _order!.warehouseName ?? 'Kho #${_order!.warehouseId}'),
                                      if (_order!.branchName != null) ...[
                                        const SizedBox(height: 6),
                                        _buildInfoRow('Chi nhánh:', _order!.branchName!),
                                      ],
                                      if (_order!.userName != null) ...[
                                        const SizedBox(height: 6),
                                        _buildInfoRow('Nhân viên lập:', _order!.userName!),
                                      ],
                                      if (_order!.notes != null && _order!.notes!.isNotEmpty) ...[
                                        const SizedBox(height: 6),
                                        _buildInfoRow('Ghi chú:', _order!.notes!),
                                      ],
                                    ],
                                  ),
                                ),
                                const SizedBox(height: 16),

                                // Items List Header
                                Row(
                                  children: [
                                    const Text('Chi tiết mặt hàng', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
                                    const SizedBox(width: 6),
                                    Container(
                                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                                      decoration: BoxDecoration(
                                        color: AppColors.primary.withOpacity(0.12),
                                        borderRadius: BorderRadius.circular(10),
                                      ),
                                      child: Text(
                                        '${_order!.details.length}',
                                        style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: AppColors.primary),
                                      ),
                                    ),
                                  ],
                                ),
                                const SizedBox(height: 10),

                                // Items
                                ListView.separated(
                                  shrinkWrap: true,
                                  physics: const NeverScrollableScrollPhysics(),
                                  itemCount: _order!.details.length,
                                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                                  itemBuilder: (context, idx) {
                                    final item = _order!.details[idx];
                                    return Container(
                                      padding: const EdgeInsets.all(12),
                                      decoration: BoxDecoration(
                                        color: AppColors.surface,
                                        borderRadius: BorderRadius.circular(10),
                                        border: Border.all(color: AppColors.border),
                                      ),
                                      child: Row(
                                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                        children: [
                                          Expanded(
                                            child: Column(
                                              crossAxisAlignment: CrossAxisAlignment.start,
                                              children: [
                                                Text(
                                                  item.productName,
                                                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                                                ),
                                                const SizedBox(height: 2),
                                                Text(
                                                  '${item.quantity} x ${Formatters.formatVND(item.unitPrice)}',
                                                  style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
                                                ),
                                              ],
                                            ),
                                          ),
                                          Text(
                                            Formatters.formatVND(item.totalPrice),
                                            style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14, color: AppColors.primary),
                                          ),
                                        ],
                                      ),
                                    );
                                  },
                                ),
                              ],
                            ),
                          ),
                        ),

                        const Divider(height: 20),

                        // Summary and Action
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                const Text('Tổng giá trị đơn:', style: TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                                Text(
                                  Formatters.formatVND(_order!.totalAmount),
                                  style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.primary),
                                ),
                              ],
                            ),
                            Row(
                              children: [
                                TextButton.icon(
                                  onPressed: _deleteOrder,
                                  icon: const Icon(Icons.delete_outline, color: AppColors.error, size: 18),
                                  label: const Text('Xóa phiếu', style: TextStyle(color: AppColors.error)),
                                ),
                                const SizedBox(width: 8),
                                ElevatedButton(
                                  onPressed: () => Navigator.pop(context),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: AppColors.primary,
                                    foregroundColor: Colors.white,
                                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                                  ),
                                  child: const Text('Đóng'),
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

  Widget _buildInfoRow(String label, String value) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 110,
          child: Text(label, style: const TextStyle(fontSize: 12, color: AppColors.textSecondary)),
        ),
        Expanded(
          child: Text(value, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600, color: AppColors.textPrimary)),
        ),
      ],
    );
  }
}
