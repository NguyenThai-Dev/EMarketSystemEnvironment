import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/expense_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';
import '../../../core/utils/image_helper.dart';
import '../../widgets/flat_card.dart';
import '../../widgets/status_badge.dart';
import '../../widgets/vnd_text.dart';
import 'add_expense_dialog.dart';
import 'cash_reconciliation_dialog.dart';

class ExpensesScreen extends StatefulWidget {
  const ExpensesScreen({super.key});

  @override
  State<ExpensesScreen> createState() => _ExpensesScreenState();
}

class _ExpensesScreenState extends State<ExpensesScreen> with SingleTickerProviderStateMixin {
  final _repository = DataRepository();
  late TabController _tabController;

  final List<String> _statuses = ['Tất cả', 'Pending', 'Approved', 'Rejected'];
  List<ExpenseModel> _expenses = [];
  ExpenseSummaryModel? _summary;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: _statuses.length, vsync: this);
    _tabController.addListener(() {
      if (!_tabController.indexIsChanging) {
        _loadData();
      }
    });
    _loadData();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    final auth = context.read<AuthProvider>();
    final branchId = auth.selectedBranch?.branchId ?? 1;

    final selected = _statuses[_tabController.index];
    final statusFilter = selected == 'Tất cả' ? null : selected;

    try {
      final results = await Future.wait([
        _repository.getExpenses(branchId: branchId, status: statusFilter),
        _repository.getExpenseSummary(branchId: branchId),
      ]);

      if (mounted) {
        setState(() {
          _expenses = results[0] as List<ExpenseModel>;
          _summary = results[1] as ExpenseSummaryModel?;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  String _getStatusLabel(String status) {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'Chờ duyệt';
      case 'approved':
        return 'Đã duyệt';
      case 'rejected':
        return 'Từ chối';
      default:
        return status;
    }
  }

  Future<void> _updateStatus(ExpenseModel expense, String newStatus) async {
    String? rejectReason;
    if (newStatus == 'Rejected') {
      final reasonCtrl = TextEditingController();
      final confirmed = await showDialog<bool>(
        context: context,
        builder: (ctx) => AlertDialog(
          title: const Text('Lý do từ chối phiếu chi'),
          content: TextField(
            controller: reasonCtrl,
            decoration: const InputDecoration(hintText: 'Nhập lý do...'),
          ),
          actions: [
            TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
            ElevatedButton(
              style: ElevatedButton.styleFrom(backgroundColor: AppColors.error),
              onPressed: () => Navigator.pop(ctx, true),
              child: const Text('Từ chối', style: TextStyle(color: Colors.white)),
            ),
          ],
        ),
      );
      if (confirmed != true) return;
      rejectReason = reasonCtrl.text.trim();
    }

    final ok = await _repository.updateExpenseStatus(
      expense.expenseId,
      newStatus,
      rejectReason: rejectReason,
      paymentMethod: expense.paymentMethod,
    );

    if (!mounted) return;
    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Đã cập nhật trạng thái phiếu: ${_getStatusLabel(newStatus)}'),
          backgroundColor: AppColors.success,
        ),
      );
      _loadData();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Cập nhật trạng thái thất bại.'), backgroundColor: AppColors.error),
      );
    }
  }

  Future<void> _deleteExpense(ExpenseModel expense) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Xác nhận xóa phiếu chi'),
        content: Text('Bạn có chắc chắn muốn xóa phiếu chi #${expense.expenseId} (${Formatters.formatVND(expense.amount)}) không?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.error),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Xóa', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    final ok = await _repository.deleteExpense(expense.expenseId);
    if (!mounted) return;
    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Xóa phiếu chi thành công!'), backgroundColor: AppColors.success),
      );
      _loadData();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không thể xóa phiếu chi.'), backgroundColor: AppColors.error),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Sổ quỹ Thu chi & Giao ca', style: TextStyle(fontWeight: FontWeight.bold)),
        actions: [
          IconButton(
            icon: const Icon(Icons.point_of_sale_outlined, color: AppColors.primary),
            tooltip: 'Chốt két giao ca',
            onPressed: () {
              showDialog(
                context: context,
                builder: (_) => const CashReconciliationDialog(),
              );
            },
          ),
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadData,
            tooltip: 'Làm mới',
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.textMuted,
          indicatorColor: AppColors.primary,
          tabs: _statuses.map((s) => Tab(text: _getStatusLabel(s))).toList(),
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.error,
        icon: const Icon(Icons.add, color: Colors.white),
        label: const Text('Tạo phiếu chi', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
        onPressed: () async {
          final res = await showDialog<bool>(
            context: context,
            builder: (_) => const AddExpenseDialog(),
          );
          if (res == true) _loadData();
        },
      ),
      body: Column(
        children: [
          // Thống kê nhanh quỹ chi
          if (_summary != null)
            Container(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 8),
              color: Colors.white,
              child: Row(
                children: [
                  Expanded(
                    child: _buildSummaryCard(
                      'Đã duyệt chi',
                      _summary!.totalApprovedAmount,
                      '${_summary!.approvedCount} phiếu',
                      AppColors.emerald,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: _buildSummaryCard(
                      'Chờ duyệt',
                      _summary!.totalPendingAmount,
                      '${_summary!.pendingCount} phiếu',
                      AppColors.warning,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: _buildSummaryCard(
                      'Bị từ chối',
                      _summary!.totalRejectedAmount,
                      '${_summary!.rejectedCount} phiếu',
                      AppColors.error,
                    ),
                  ),
                ],
              ),
            ),
          const Divider(height: 1, color: AppColors.border),

          // Danh sách phiếu chi
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _expenses.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(Icons.receipt_long_outlined, size: 64, color: AppColors.textMuted.withValues(alpha: 0.5)),
                            const SizedBox(height: 12),
                            const Text('Không có phiếu chi nào', style: TextStyle(color: AppColors.textMuted)),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: _loadData,
                        child: ListView.separated(
                          padding: const EdgeInsets.fromLTRB(16, 12, 16, 80),
                          itemCount: _expenses.length,
                          separatorBuilder: (_, _) => const SizedBox(height: 10),
                          itemBuilder: (context, index) {
                            final e = _expenses[index];
                            final isPending = e.status.toLowerCase() == 'pending';

                            return FlatCard(
                              padding: const EdgeInsets.all(14),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Row(
                                    children: [
                                      Container(
                                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                                        decoration: BoxDecoration(
                                          color: AppColors.primaryBrand.withValues(alpha: 0.08),
                                          borderRadius: BorderRadius.circular(8),
                                        ),
                                        child: Text(
                                          e.categoryName ?? 'Chi phí chung',
                                          style: const TextStyle(fontSize: 11, fontWeight: FontWeight.bold, color: AppColors.primaryBrand),
                                        ),
                                      ),
                                      const SizedBox(width: 8),
                                      Container(
                                        padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                                        decoration: BoxDecoration(
                                          color: AppColors.surfaceVariant,
                                          borderRadius: BorderRadius.circular(6),
                                        ),
                                        child: Text(
                                          e.paymentMethod == 'Cash' ? 'Tiền mặt' : 'Chuyển khoản',
                                          style: const TextStyle(fontSize: 10, color: AppColors.textSecondary),
                                        ),
                                      ),
                                      const Spacer(),
                                      StatusBadge.fromStatus(e.status),
                                    ],
                                  ),
                                  const SizedBox(height: 10),
                                  Row(
                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                    children: [
                                      VndText(
                                        e.amount,
                                        style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.error),
                                      ),
                                      Text(
                                        Formatters.formatDateTime(e.expenseDate),
                                        style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                                      ),
                                    ],
                                  ),
                                  if (e.note != null && e.note!.isNotEmpty) ...[
                                    const SizedBox(height: 6),
                                    Text('Ghi chú: ${e.note!}', style: const TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                                  ],
                                  const SizedBox(height: 4),
                                  Row(
                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                    children: [
                                      Text('Người lập: ${e.userName ?? "N/A"}', style: const TextStyle(fontSize: 11, color: AppColors.textMuted)),
                                      if (e.refImage != null && e.refImage!.trim().isNotEmpty)
                                        InkWell(
                                          onTap: () => _showAttachmentDialog(e.refImage!),
                                          borderRadius: BorderRadius.circular(6),
                                          child: Container(
                                            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                                            decoration: BoxDecoration(
                                              color: AppColors.secondary.withValues(alpha: 0.1),
                                              borderRadius: BorderRadius.circular(6),
                                            ),
                                            child: const Row(
                                              mainAxisSize: MainAxisSize.min,
                                              children: [
                                                Icon(Icons.attachment_rounded, size: 12, color: AppColors.secondary),
                                                SizedBox(width: 4),
                                                Text('Xem chứng từ', style: TextStyle(fontSize: 10, fontWeight: FontWeight.bold, color: AppColors.secondary)),
                                              ],
                                            ),
                                          ),
                                        ),
                                    ],
                                  ),

                                  if (e.rejectionReason != null && e.rejectionReason!.isNotEmpty) ...[
                                    const SizedBox(height: 6),
                                    Container(
                                      padding: const EdgeInsets.all(8),
                                      decoration: BoxDecoration(
                                        color: AppColors.error.withValues(alpha: 0.08),
                                        borderRadius: BorderRadius.circular(8),
                                      ),
                                      child: Text(
                                        'Lý do từ chối: ${e.rejectionReason}',
                                        style: const TextStyle(fontSize: 11, color: AppColors.error),
                                      ),
                                    ),
                                  ],

                                  // Phê duyệt nhanh nếu còn chờ duyệt
                                  if (isPending) ...[
                                    const Divider(height: 18, color: AppColors.border),
                                    Row(
                                      mainAxisAlignment: MainAxisAlignment.end,
                                      children: [
                                        OutlinedButton.icon(
                                          style: OutlinedButton.styleFrom(
                                            foregroundColor: AppColors.error,
                                            side: const BorderSide(color: AppColors.error),
                                            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                                            minimumSize: const Size(60, 32),
                                          ),
                                          icon: const Icon(Icons.close, size: 14),
                                          label: const Text('Từ chối', style: TextStyle(fontSize: 12)),
                                          onPressed: () => _updateStatus(e, 'Rejected'),
                                        ),
                                        const SizedBox(width: 8),
                                        ElevatedButton.icon(
                                          style: ElevatedButton.styleFrom(
                                            backgroundColor: AppColors.emerald,
                                            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
                                            minimumSize: const Size(60, 32),
                                          ),
                                          icon: const Icon(Icons.check, size: 14, color: Colors.white),
                                          label: const Text('Duyệt chi', style: TextStyle(fontSize: 12, color: Colors.white, fontWeight: FontWeight.bold)),
                                          onPressed: () => _updateStatus(e, 'Approved'),
                                        ),
                                      ],
                                    ),
                                  ] else ...[
                                    const Divider(height: 14, color: AppColors.border),
                                    Row(
                                      mainAxisAlignment: MainAxisAlignment.end,
                                      children: [
                                        IconButton(
                                          icon: const Icon(Icons.delete_outline, size: 18, color: AppColors.textMuted),
                                          tooltip: 'Xóa phiếu',
                                          onPressed: () => _deleteExpense(e),
                                          constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
                                          padding: EdgeInsets.zero,
                                        ),
                                      ],
                                    ),
                                  ],
                                ],
                              ),
                            );
                          },
                        ),
                      ),
          ),
        ],
      ),
    );
  }

  void _showAttachmentDialog(String imagePath) {
    final fullUrl = ImageHelper.getFullImageUrl(imagePath);
    if (fullUrl == null) return;

    showDialog(
      context: context,
      builder: (ctx) => Dialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.all(12),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text('Chứng từ phiếu chi', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14)),
                  IconButton(
                    icon: const Icon(Icons.close, size: 18),
                    onPressed: () => Navigator.pop(ctx),
                    padding: EdgeInsets.zero,
                    constraints: const BoxConstraints(),
                  ),
                ],
              ),
            ),
            ConstrainedBox(
              constraints: const BoxConstraints(maxHeight: 400),
              child: Image.network(
                fullUrl,
                fit: BoxFit.contain,
                errorBuilder: (_, __, ___) => const Padding(
                  padding: EdgeInsets.all(32),
                  child: Center(child: Text('Không thể tải hình ảnh chứng từ')),
                ),
              ),
            ),
            const SizedBox(height: 12),
          ],
        ),
      ),
    );
  }

  Widget _buildSummaryCard(String title, double amount, String count, Color color) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: color.withValues(alpha: 0.2)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          VndText(
            amount,
            style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: color),
          ),
          const SizedBox(height: 2),
          Text(title, style: const TextStyle(fontSize: 10, color: AppColors.textMuted)),
          Text(count, style: TextStyle(fontSize: 10, fontWeight: FontWeight.w600, color: color)),
        ],
      ),
    );
  }
}
