import 'package:flutter/material.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/order_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../pos/receipt_preview_dialog.dart';

class OrderDetailDialog extends StatefulWidget {
  final int orderId;
  final OrderModel? initialOrder;
  final bool canCancel;
  final VoidCallback? onOrderUpdated;

  const OrderDetailDialog({
    super.key,
    required this.orderId,
    this.initialOrder,
    this.canCancel = false,
    this.onOrderUpdated,
  });

  static void show(
    BuildContext context, {
    required int orderId,
    OrderModel? initialOrder,
    bool canCancel = false,
    VoidCallback? onOrderUpdated,
  }) {
    showDialog(
      context: context,
      builder: (_) => OrderDetailDialog(
        orderId: orderId,
        initialOrder: initialOrder,
        canCancel: canCancel,
        onOrderUpdated: onOrderUpdated,
      ),
    );
  }

  @override
  State<OrderDetailDialog> createState() => _OrderDetailDialogState();
}

class _OrderDetailDialogState extends State<OrderDetailDialog> {
  final DataRepository _repository = DataRepository();
  OrderModel? _order;
  bool _isLoading = true;
  bool _isCancelling = false;

  @override
  void initState() {
    super.initState();
    _order = widget.initialOrder;
    _fetchDetail();
  }

  Future<void> _fetchDetail() async {
    try {
      final detail = await _repository.getOrderDetail(widget.orderId);
      if (mounted) {
        setState(() {
          if (detail != null) _order = detail;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _handleCancelOrder() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Xác nhận hủy đơn hàng?', style: TextStyle(fontWeight: FontWeight.bold)),
        content: Text('Đơn hàng #${_order?.orderCode ?? widget.orderId} sẽ chuyển sang trạng thái "Đã hủy". Hành động này không thể hoàn tác.'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Bỏ qua')),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.error),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Xác nhận hủy đơn', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );

    if (confirmed != true) return;

    setState(() => _isCancelling = true);
    final success = await _repository.cancelOrder(widget.orderId);
    if (!mounted) return;
    setState(() => _isCancelling = false);

    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Đã hủy đơn hàng thành công!'),
          backgroundColor: AppColors.emerald,
        ),
      );
      widget.onOrderUpdated?.call();
      Navigator.pop(context);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Không thể hủy đơn hàng. Vui lòng kiểm tra lại quyền hạn.'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  void _handlePrintReceipt() {
    if (_order == null) return;
    final checkoutItems = _order!.details.map((d) => CheckoutItemRequest(
      productId: d.productId,
      quantity: d.quantity,
      price: d.unitPrice,
      discount: d.discount,
    )).toList();

    final productNames = {for (var d in _order!.details) d.productId: d.productName};

    ReceiptPreviewDialog.show(
      context,
      orderId: _order!.orderId,
      orderCode: _order!.orderCode,
      totalAmount: _order!.totalAmount,
      discount: _order!.details.fold(0.0, (sum, item) => sum + item.discount),
      paymentMethod: _order!.paymentMethod ?? 'Tiền mặt',
      cashierName: 'Thu ngân',
      branchName: 'Chi nhánh EMarket',
      items: checkoutItems,
      productNames: productNames,
    );
  }

  Color _getStatusColor(String status) {
    switch (status.toUpperCase()) {
      case 'COMPLETED':
      case 'PAID':
      case 'SUCCESS':
        return AppColors.emerald;
      case 'PENDING':
      case 'PROCESSING':
        return Colors.orange[800]!;
      case 'CANCELLED':
      case 'CANCELED':
        return AppColors.error;
      default:
        return AppColors.textSecondary;
    }
  }

  String _getStatusText(String status) {
    switch (status.toUpperCase()) {
      case 'COMPLETED':
      case 'PAID':
      case 'SUCCESS':
        return 'Hoàn thành';
      case 'PENDING':
      case 'PROCESSING':
        return 'Chờ xử lý';
      case 'CANCELLED':
      case 'CANCELED':
        return 'Đã hủy';
      default:
        return status;
    }
  }

  bool _canBeCancelled(OrderModel? order) {
    if (order == null) return false;
    final s = order.status.trim().toUpperCase();
    if (s == 'COMPLETED' ||
        s == 'PAID' ||
        s == 'SUCCESS' ||
        s == 'HOÀN THÀNH' ||
        s == 'CANCELLED' ||
        s == 'CANCELED' ||
        s == 'ĐÃ HỦY') {
      return false;
    }
    return s == 'PENDING' || s == 'WAITING' || s == 'PROCESSING' || s == 'CHỜ XỬ LÝ' || s == 'DRAFT';
  }

  @override
  Widget build(BuildContext context) {
    final order = _order;

    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      insetPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 24),
      child: ConstrainedBox(
        constraints: BoxConstraints(
          maxWidth: 500,
          maxHeight: MediaQuery.of(context).size.height * 0.85,
        ),
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Header
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          order?.orderCode ?? '#ORD-${widget.orderId}',
                          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, letterSpacing: -0.3),
                        ),
                        if (order?.orderDate != null)
                          Text(
                            Formatters.formatDateTime(order!.orderDate!),
                            style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
                          ),
                      ],
                    ),
                  ),
                  if (order != null)
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                      decoration: BoxDecoration(
                        color: _getStatusColor(order.status).withValues(alpha: 0.12),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Text(
                        _getStatusText(order.status),
                        style: TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.bold,
                          color: _getStatusColor(order.status),
                        ),
                      ),
                    ),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () => Navigator.pop(context),
                  ),
                ],
              ),
              const Divider(height: 20),

              // Customer & Payment Info
              if (order != null) ...[
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: AppColors.surfaceVariant,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Row(
                    children: [
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text('Khách hàng', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                            const SizedBox(height: 2),
                            Text(
                              order.customerName ?? 'Khách vãng lai',
                              style: const TextStyle(fontSize: 13, fontWeight: FontWeight.bold),
                            ),
                          ],
                        ),
                      ),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text('Thanh toán', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                            const SizedBox(height: 2),
                            Text(
                              order.paymentMethod ?? 'Tiền mặt',
                              style: const TextStyle(fontSize: 13, fontWeight: FontWeight.bold),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 14),
              ],

              const Text(
                'Chi tiết các món hàng:',
                style: TextStyle(fontSize: 13, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
              ),
              const SizedBox(height: 8),

              // Line Items
              Expanded(
                child: _isLoading && (order == null || order.details.isEmpty)
                    ? const Center(child: CircularProgressIndicator())
                    : (order == null || order.details.isEmpty)
                        ? const Center(child: Text('Không có chi tiết sản phẩm.'))
                        : ListView.separated(
                            itemCount: order.details.length,
                            separatorBuilder: (_, __) => const Divider(height: 1),
                            itemBuilder: (_, i) {
                              final item = order.details[i];
                              return Padding(
                                padding: const EdgeInsets.symmetric(vertical: 8),
                                child: Row(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Container(
                                      padding: const EdgeInsets.all(6),
                                      decoration: BoxDecoration(
                                        color: AppColors.primaryBrand.withValues(alpha: 0.08),
                                        borderRadius: BorderRadius.circular(8),
                                      ),
                                      child: const Icon(Icons.shopping_bag_outlined, size: 16, color: AppColors.primaryBrand),
                                    ),
                                    const SizedBox(width: 10),
                                    Expanded(
                                      child: Column(
                                        crossAxisAlignment: CrossAxisAlignment.start,
                                        children: [
                                          Text(
                                            item.productName.isNotEmpty ? item.productName : 'Sản phẩm #${item.productId}',
                                            style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13),
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
                                      Formatters.formatVND(item.totalPrice > 0 ? item.totalPrice : (item.quantity * item.unitPrice)),
                                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13, color: AppColors.emerald),
                                    ),
                                  ],
                                ),
                              );
                            },
                          ),
              ),

              const Divider(height: 20),

              // Total Row
              if (order != null) ...[
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text('Tổng thanh toán:', style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold)),
                    Text(
                      Formatters.formatVND(order.totalAmount),
                      style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.emerald),
                    ),
                  ],
                ),
                const SizedBox(height: 16),
              ],

              // Actions
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton.icon(
                      icon: const Icon(Icons.print_outlined, size: 18),
                      label: const Text('In lại Bill'),
                      style: OutlinedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 12),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                      ),
                      onPressed: _handlePrintReceipt,
                    ),
                  ),
                  if (widget.canCancel && _canBeCancelled(order)) ...[
                    const SizedBox(width: 10),
                    Expanded(
                      child: ElevatedButton.icon(
                        icon: _isCancelling
                            ? const SizedBox(width: 14, height: 14, child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white))
                            : const Icon(Icons.cancel_outlined, size: 18),
                        label: const Text('Hủy đơn'),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: AppColors.error,
                          padding: const EdgeInsets.symmetric(vertical: 12),
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                        ),
                        onPressed: _isCancelling ? null : _handleCancelOrder,
                      ),
                    ),
                  ],
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
