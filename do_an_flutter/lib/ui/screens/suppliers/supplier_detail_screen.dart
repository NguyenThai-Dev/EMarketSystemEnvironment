import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/product_model.dart';
import '../../../data/repositories/data_repository.dart';
import 'add_edit_supplier_dialog.dart';
import 'pay_supplier_debt_dialog.dart';

class SupplierDetailScreen extends StatefulWidget {
  final SupplierModel supplier;

  const SupplierDetailScreen({
    super.key,
    required this.supplier,
  });

  @override
  State<SupplierDetailScreen> createState() => _SupplierDetailScreenState();
}

class _SupplierDetailScreenState extends State<SupplierDetailScreen> {
  late SupplierModel _currentSupplier;
  final _repo = DataRepository();
  bool _isLoading = true;
  List<SupplierDebtModel> _debts = [];
  SupplierDebtSummaryModel _summary = SupplierDebtSummaryModel();

  @override
  void initState() {
    super.initState();
    _currentSupplier = widget.supplier;
    _loadData();
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    try {
      final results = await Future.wait([
        _repo.getSupplierDebts(supplierId: _currentSupplier.supplierId),
        _repo.getSupplierDebtSummary(supplierId: _currentSupplier.supplierId),
      ]);
      if (mounted) {
        setState(() {
          _debts = results[0] as List<SupplierDebtModel>;
          _summary = results[1] as SupplierDebtSummaryModel;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _callPhone(String phone) async {
    final uri = Uri.parse('tel:$phone');
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri);
    }
  }

  Future<void> _sendEmail(String email) async {
    final uri = Uri.parse('mailto:$email');
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Text(
          _currentSupplier.name,
          style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold),
        ),
        actions: [
          IconButton(
            tooltip: 'Chỉnh sửa NCC',
            icon: const Icon(Icons.edit_outlined),
            onPressed: () {
              AddEditSupplierDialog.show(
                context,
                supplier: _currentSupplier,
                onSaved: () {
                  // Reload
                  _loadData();
                },
              );
            },
          ),
          IconButton(
            tooltip: 'Tải lại',
            icon: const Icon(Icons.refresh_rounded),
            onPressed: _loadData,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: _loadData,
              child: SingleChildScrollView(
                physics: const AlwaysScrollableScrollPhysics(),
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Supplier Info Card
                    _buildSupplierCard(),
                    const SizedBox(height: 16),

                    // Debt Summary KPIs
                    _buildDebtSummaryCards(),
                    const SizedBox(height: 20),

                    // Debt List Header
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Row(
                          children: [
                            const Icon(Icons.receipt_long_rounded, size: 20, color: AppColors.primary),
                            const SizedBox(width: 8),
                            const Text(
                              'Lịch sử Công nợ / Đơn nhập',
                              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                            ),
                            const SizedBox(width: 6),
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                              decoration: BoxDecoration(
                                color: AppColors.primary.withOpacity(0.12),
                                borderRadius: BorderRadius.circular(10),
                              ),
                              child: Text(
                                '${_debts.length}',
                                style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: AppColors.primary),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),

                    // Debt items
                    if (_debts.isEmpty)
                      Container(
                        padding: const EdgeInsets.symmetric(vertical: 40, horizontal: 16),
                        decoration: BoxDecoration(
                          color: AppColors.surface,
                          borderRadius: BorderRadius.circular(14),
                          border: Border.all(color: AppColors.border),
                        ),
                        child: const Column(
                          children: [
                            Icon(Icons.check_circle_outline_rounded, size: 48, color: AppColors.success),
                            SizedBox(height: 10),
                            Text(
                              'Không có ghi nhận công nợ nào!',
                              style: TextStyle(fontWeight: FontWeight.w600, color: AppColors.textPrimary),
                            ),
                            SizedBox(height: 4),
                            Text(
                              'Nhà cung cấp này hiện không có công nợ tồn đọng.',
                              style: TextStyle(fontSize: 12, color: AppColors.textSecondary),
                            ),
                          ],
                        ),
                      )
                    else
                      ListView.separated(
                        shrinkWrap: true,
                        physics: const NeverScrollableScrollPhysics(),
                        itemCount: _debts.length,
                        separatorBuilder: (_, __) => const SizedBox(height: 10),
                        itemBuilder: (context, index) {
                          final debt = _debts[index];
                          return _buildDebtCard(debt);
                        },
                      ),
                  ],
                ),
              ),
            ),
    );
  }

  Widget _buildSupplierCard() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
        boxShadow: [
          BoxInsets.cardShadow,
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: AppColors.primary.withOpacity(0.12),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Icon(Icons.business_rounded, color: AppColors.primary, size: 26),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      _currentSupplier.name,
                      style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                    ),
                    if (_currentSupplier.contactName != null && _currentSupplier.contactName!.isNotEmpty) ...[
                      const SizedBox(height: 4),
                      Row(
                        children: [
                          const Icon(Icons.person_outline_rounded, size: 14, color: AppColors.textSecondary),
                          const SizedBox(width: 4),
                          Text(
                            'Liên hệ: ${_currentSupplier.contactName}',
                            style: const TextStyle(fontSize: 13, color: AppColors.textSecondary),
                          ),
                        ],
                      ),
                    ],
                  ],
                ),
              ),
            ],
          ),
          const Divider(height: 24),
          if (_currentSupplier.phone != null && _currentSupplier.phone!.isNotEmpty) ...[
            InkWell(
              onTap: () => _callPhone(_currentSupplier.phone!),
              borderRadius: BorderRadius.circular(8),
              child: Padding(
                padding: const EdgeInsets.symmetric(vertical: 4),
                child: Row(
                  children: [
                    const Icon(Icons.phone_rounded, size: 16, color: AppColors.primary),
                    const SizedBox(width: 8),
                    Text(
                      _currentSupplier.phone!,
                      style: const TextStyle(fontSize: 13, color: AppColors.primary, fontWeight: FontWeight.w600),
                    ),
                    const Spacer(),
                    const Text('Gọi ngay', style: TextStyle(fontSize: 12, color: AppColors.primary)),
                  ],
                ),
              ),
            ),
          ],
          if (_currentSupplier.email != null && _currentSupplier.email!.isNotEmpty) ...[
            const SizedBox(height: 6),
            InkWell(
              onTap: () => _sendEmail(_currentSupplier.email!),
              borderRadius: BorderRadius.circular(8),
              child: Padding(
                padding: const EdgeInsets.symmetric(vertical: 4),
                child: Row(
                  children: [
                    const Icon(Icons.email_outlined, size: 16, color: AppColors.textSecondary),
                    const SizedBox(width: 8),
                    Text(
                      _currentSupplier.email!,
                      style: const TextStyle(fontSize: 13, color: AppColors.textSecondary),
                    ),
                  ],
                ),
              ),
            ),
          ],
          if (_currentSupplier.address != null && _currentSupplier.address!.isNotEmpty) ...[
            const SizedBox(height: 6),
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(Icons.location_on_outlined, size: 16, color: AppColors.textSecondary),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    _currentSupplier.address!,
                    style: const TextStyle(fontSize: 13, color: AppColors.textSecondary),
                  ),
                ),
              ],
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildDebtSummaryCards() {
    return Row(
      children: [
        Expanded(
          child: _buildMetricTile(
            title: 'Tổng tiền nợ',
            amount: _summary.totalDebt,
            color: AppColors.info,
            icon: Icons.receipt_outlined,
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: _buildMetricTile(
            title: 'Đã trả NCC',
            amount: _summary.totalPaid,
            color: AppColors.success,
            icon: Icons.check_circle_outlined,
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: _buildMetricTile(
            title: 'Còn phải trả',
            amount: _summary.totalRemaining,
            color: AppColors.error,
            icon: Icons.warning_amber_rounded,
          ),
        ),
      ],
    );
  }

  Widget _buildMetricTile({
    required String title,
    required double amount,
    required Color color,
    required IconData icon,
  }) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, size: 16, color: color),
              const SizedBox(width: 4),
              Expanded(
                child: Text(
                  title,
                  style: const TextStyle(fontSize: 11, color: AppColors.textSecondary),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
          const SizedBox(height: 6),
          Text(
            Formatters.formatVND(amount),
            style: TextStyle(fontSize: 13, fontWeight: FontWeight.bold, color: color),
            overflow: TextOverflow.ellipsis,
          ),
        ],
      ),
    );
  }

  Widget _buildDebtCard(SupplierDebtModel debt) {
    Color statusColor;
    String statusText;
    switch (debt.status.toUpperCase()) {
      case 'PAID':
        statusColor = AppColors.success;
        statusText = 'ĐÃ TRẢ HẾT';
        break;
      case 'PARTIAL':
        statusColor = AppColors.warning;
        statusText = 'TRẢ MỘT PHẦN';
        break;
      case 'OVERDUE':
        statusColor = AppColors.error;
        statusText = 'QUÁ HẠN';
        break;
      default:
        statusColor = AppColors.error;
        statusText = 'CHƯA TRẢ';
        break;
    }

    final hasDue = debt.remainingAmount > 0;

    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: hasDue ? AppColors.warning.withOpacity(0.3) : AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Phiếu nợ #${debt.debtId}',
                style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14, color: AppColors.textPrimary),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: statusColor.withOpacity(0.12),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  statusText,
                  style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold, color: statusColor),
                ),
              ),
            ],
          ),
          if (debt.createdDate != null) ...[
            const SizedBox(height: 4),
            Text(
              'Ngày tạo: ${Formatters.formatDateTime(debt.createdDate)}',
              style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
            ),
          ],
          const SizedBox(height: 10),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('Tổng nợ', style: TextStyle(fontSize: 11, color: AppColors.textSecondary)),
                  Text(Formatters.formatVND(debt.totalDebtAmount), style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                ],
              ),
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('Đã trả', style: TextStyle(fontSize: 11, color: AppColors.textSecondary)),
                  Text(Formatters.formatVND(debt.paidAmount), style: const TextStyle(fontSize: 13, color: AppColors.success, fontWeight: FontWeight.w600)),
                ],
              ),
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  const Text('Còn lại', style: TextStyle(fontSize: 11, color: AppColors.textSecondary)),
                  Text(
                    Formatters.formatVND(debt.remainingAmount),
                    style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold, color: hasDue ? AppColors.error : AppColors.textSecondary),
                  ),
                ],
              ),
            ],
          ),
          if (hasDue) ...[
            const SizedBox(height: 12),
            Align(
              alignment: Alignment.centerRight,
              child: ElevatedButton.icon(
                onPressed: () {
                  PaySupplierDebtDialog.show(
                    context,
                    debt: debt,
                    onPaid: _loadData,
                  );
                },
                icon: const Icon(Icons.payment_rounded, size: 16),
                label: const Text('Thanh toán'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.success,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                  textStyle: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class BoxInsets {
  static BoxShadow cardShadow = BoxShadow(
    color: Colors.black.withOpacity(0.04),
    blurRadius: 8,
    offset: const Offset(0, 2),
  );
}
