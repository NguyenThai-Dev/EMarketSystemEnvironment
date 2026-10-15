import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/quotation_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';
import '../../widgets/flat_card.dart';
import '../../widgets/status_badge.dart';
import '../../widgets/vnd_text.dart';
import 'create_quotation_dialog.dart';

class QuotationsScreen extends StatefulWidget {
  const QuotationsScreen({super.key});

  @override
  State<QuotationsScreen> createState() => _QuotationsScreenState();
}

class _QuotationsScreenState extends State<QuotationsScreen> with SingleTickerProviderStateMixin {
  final _repository = DataRepository();
  late TabController _tabController;

  final List<String> _statuses = ['Tất cả', 'Draft', 'Sent', 'Accepted', 'Converted', 'Cancelled'];
  List<QuotationModel> _quotations = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: _statuses.length, vsync: this);
    _tabController.addListener(() {
      if (!_tabController.indexIsChanging) {
        _loadQuotations();
      }
    });
    _loadQuotations();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadQuotations() async {
    setState(() => _isLoading = true);
    final auth = context.read<AuthProvider>();
    final branchId = auth.selectedBranch?.branchId ?? 1;

    final selectedStatus = _statuses[_tabController.index];
    final statusFilter = selectedStatus == 'Tất cả' ? null : selectedStatus;

    try {
      final list = await _repository.getQuotations(
        branchId: branchId,
        status: statusFilter,
      );
      if (mounted) {
        setState(() {
          _quotations = list;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Color _getStatusColor(String status) {
    switch (status.toLowerCase()) {
      case 'draft':
        return AppColors.textMuted;
      case 'sent':
        return Colors.blue;
      case 'accepted':
        return AppColors.success;
      case 'converted':
        return const Color(0xFF673AB7);
      case 'cancelled':
        return AppColors.error;
      default:
        return AppColors.primary;
    }
  }

  String _getStatusLabel(String status) {
    switch (status.toLowerCase()) {
      case 'draft':
        return 'Bản nháp';
      case 'sent':
        return 'Đã gửi';
      case 'accepted':
        return 'Đã đồng ý';
      case 'converted':
        return 'Đã tạo đơn';
      case 'cancelled':
        return 'Đã hủy';
      default:
        return status;
    }
  }

  Future<void> _changeStatus(QuotationModel q, String newStatus) async {
    final ok = await _repository.changeQuotationStatus(q.quotationId, newStatus);
    if (!mounted) return;
    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Đã cập nhật trạng thái: ${_getStatusLabel(newStatus)}'), backgroundColor: AppColors.success),
      );
      _loadQuotations();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không thể đổi trạng thái.'), backgroundColor: AppColors.error),
      );
    }
  }

  Future<void> _convertToOrder(QuotationModel q) async {
    final auth = context.read<AuthProvider>();
    final userId = auth.currentUser?.userId ?? 1;

    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Chuyển đổi thành Đơn hàng'),
        content: Text('Tạo ngay một đơn bán hàng chính thức (Order) từ bảng báo giá #${q.quotationCode}?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Chuyển đổi ngay', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    final orderId = await _repository.convertQuotationToOrder(q.quotationId, userId: userId);
    if (!mounted) return;

    if (orderId != null && orderId > 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Chuyển đổi thành công! Mã đơn bán hàng mới: #$orderId'),
          backgroundColor: AppColors.success,
        ),
      );
      _loadQuotations();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Chuyển đổi thành đơn hàng thất bại.'), backgroundColor: AppColors.error),
      );
    }
  }

  void _showDetailModal(QuotationModel q) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (ctx) {
        return DraggableScrollableSheet(
          initialChildSize: 0.6,
          maxChildSize: 0.9,
          minChildSize: 0.4,
          expand: false,
          builder: (context, scrollCtrl) {
            return FutureBuilder<QuotationModel?>(
              future: _repository.getQuotationDetail(q.quotationId),
              builder: (context, snapshot) {
                final detail = snapshot.data ?? q;
                return Container(
                  padding: const EdgeInsets.all(20),
                  child: ListView(
                    controller: scrollCtrl,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(detail.quotationCode, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold)),
                              Text(
                                'Ngày lập: ${detail.issueDate.day}/${detail.issueDate.month}/${detail.issueDate.year}',
                                style: const TextStyle(fontSize: 12, color: AppColors.textMuted),
                              ),
                            ],
                          ),
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                            decoration: BoxDecoration(
                              color: _getStatusColor(detail.status).withValues(alpha: 0.15),
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: Text(
                              _getStatusLabel(detail.status),
                              style: TextStyle(
                                fontWeight: FontWeight.bold,
                                color: _getStatusColor(detail.status),
                              ),
                            ),
                          ),
                        ],
                      ),
                      const Divider(height: 24),
                      Text('Khách hàng: ${detail.customerName ?? 'Khách vãng lai'}', style: const TextStyle(fontWeight: FontWeight.bold)),
                      if (detail.customerPhone != null && detail.customerPhone!.isNotEmpty)
                        Text('Số điện thoại: ${detail.customerPhone}', style: const TextStyle(color: AppColors.textMuted)),
                      const SizedBox(height: 16),
                      const Text('Chi tiết hàng hóa:', style: TextStyle(fontWeight: FontWeight.bold)),
                      const SizedBox(height: 8),
                      ...detail.details.map((d) => Padding(
                            padding: const EdgeInsets.symmetric(vertical: 4),
                            child: Row(
                              children: [
                                Expanded(child: Text('${d.productName} (x${d.quantity})')),
                                VndText(d.totalPrice, style: const TextStyle(fontWeight: FontWeight.bold)),
                              ],
                            ),
                          )),
                      const Divider(height: 24),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text('Tổng thanh toán:', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                          VndText(
                            detail.finalAmount,
                            style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 18, color: AppColors.emerald),
                          ),
                        ],
                      ),
                      const SizedBox(height: 24),
                      if (detail.status.toLowerCase() != 'converted')
                        ElevatedButton.icon(
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            padding: const EdgeInsets.symmetric(vertical: 14),
                            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                          ),
                          icon: const Icon(Icons.shopping_bag_outlined, color: Colors.white),
                          label: const Text('Chuyển đổi thành Đơn hàng (POS)', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
                          onPressed: () {
                            Navigator.pop(ctx);
                            _convertToOrder(detail);
                          },
                        ),
                    ],
                  ),
                );
              },
            );
          },
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final branchId = auth.selectedBranch?.branchId ?? 1;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Báo giá (Quotations)', style: TextStyle(fontWeight: FontWeight.bold)),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadQuotations,
            tooltip: 'Làm mới',
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          isScrollable: true,
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.textMuted,
          indicatorColor: AppColors.primary,
          tabs: _statuses.map((s) => Tab(text: _getStatusLabel(s))).toList(),
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.primary,
        icon: const Icon(Icons.add, color: Colors.white),
        label: const Text('Lập báo giá', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
        onPressed: () async {
          final res = await showDialog<bool>(
            context: context,
            builder: (_) => CreateQuotationDialog(branchId: branchId),
          );
          if (res == true) _loadQuotations();
        },
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _quotations.isEmpty
              ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.request_quote_outlined, size: 64, color: AppColors.textMuted.withValues(alpha: 0.5)),
                      const SizedBox(height: 12),
                      const Text('Chưa có bảng báo giá nào', style: TextStyle(color: AppColors.textMuted)),
                    ],
                  ),
                )
              : RefreshIndicator(
                  onRefresh: _loadQuotations,
                  child: ListView.separated(
                    padding: const EdgeInsets.fromLTRB(16, 12, 16, 80),
                    itemCount: _quotations.length,
                    separatorBuilder: (_, _) => const SizedBox(height: 10),
                    itemBuilder: (context, index) {
                      final q = _quotations[index];

                      return FlatCard(
                        onTap: () => _showDetailModal(q),
                        padding: const EdgeInsets.all(14),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              children: [
                                Container(
                                  padding: const EdgeInsets.all(6),
                                  decoration: BoxDecoration(
                                    color: AppColors.primaryBrand.withValues(alpha: 0.08),
                                    borderRadius: BorderRadius.circular(8),
                                  ),
                                  child: const Icon(Icons.request_quote_rounded, size: 16, color: AppColors.primaryBrand),
                                ),
                                const SizedBox(width: 8),
                                Text(
                                  q.quotationCode,
                                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                                ),
                                const Spacer(),
                                StatusBadge.fromStatus(q.status),
                              ],
                            ),
                            const SizedBox(height: 10),
                            Row(
                              children: [
                                const Icon(Icons.person_outline, size: 14, color: AppColors.textMuted),
                                const SizedBox(width: 4),
                                Expanded(
                                  child: Text(
                                    q.customerName ?? 'Khách vãng lai',
                                    style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w500),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                                if (q.customerPhone != null && q.customerPhone!.isNotEmpty) ...[
                                  const SizedBox(width: 4),
                                  Text(q.customerPhone!, style: const TextStyle(fontSize: 11, color: AppColors.textMuted)),
                                ],
                              ],
                            ),
                            const SizedBox(height: 8),
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Row(
                                  children: [
                                    const Icon(Icons.calendar_today_outlined, size: 12, color: AppColors.textMuted),
                                    const SizedBox(width: 4),
                                    Text(
                                      Formatters.formatDateTime(q.issueDate),
                                      style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                                    ),
                                  ],
                                ),
                                VndText(
                                  q.finalAmount,
                                  style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppColors.emerald),
                                ),
                              ],
                            ),
                            const SizedBox(height: 8),
                            const Divider(height: 1, color: AppColors.border),
                            const SizedBox(height: 4),
                            Row(
                              mainAxisAlignment: MainAxisAlignment.end,
                              children: [
                                if (q.status.toLowerCase() != 'converted') ...[
                                  TextButton.icon(
                                    icon: const Icon(Icons.shopping_bag_outlined, size: 15, color: AppColors.primaryBrand),
                                    label: const Text('Tạo đơn POS', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: AppColors.primaryBrand)),
                                    onPressed: () => _convertToOrder(q),
                                  ),
                                  const SizedBox(width: 4),
                                ],
                                PopupMenuButton<String>(
                                  icon: const Icon(Icons.more_vert, size: 18, color: AppColors.textMuted),
                                  onSelected: (val) => _changeStatus(q, val),
                                  itemBuilder: (ctx) => [
                                    const PopupMenuItem(value: 'Sent', child: Text('Đánh dấu: Đã gửi')),
                                    const PopupMenuItem(value: 'Accepted', child: Text('Đánh dấu: Đã đồng ý')),
                                    const PopupMenuItem(value: 'Cancelled', child: Text('Đánh dấu: Hủy')),
                                  ],
                                ),
                              ],
                            ),
                          ],
                        ),
                      );
                    },
                  ),
                ),
    );
  }
}
