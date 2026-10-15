import 'package:flutter/material.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';

class CashReconciliationDialog extends StatefulWidget {
  final double expectedCash;

  const CashReconciliationDialog({super.key, this.expectedCash = 0});

  @override
  State<CashReconciliationDialog> createState() => _CashReconciliationDialogState();
}

class _CashReconciliationDialogState extends State<CashReconciliationDialog> {
  final Map<int, int> _counts = {
    500000: 0,
    200000: 0,
    100000: 0,
    50000: 0,
    20000: 0,
    10000: 0,
    5000: 0,
    2000: 0,
    1000: 0,
  };

  late double _expectedCash;
  final _expectedCtrl = TextEditingController();

  @override
  void initState() {
    super.initState();
    _expectedCash = widget.expectedCash;
    if (_expectedCash > 0) {
      _expectedCtrl.text = _expectedCash.toInt().toString();
    }
  }

  @override
  void dispose() {
    _expectedCtrl.dispose();
    super.dispose();
  }

  double get _actualTotal {
    double sum = 0;
    _counts.forEach((denom, count) {
      sum += denom * count;
    });
    return sum;
  }

  double get _discrepancy => _actualTotal - _expectedCash;

  void _updateCount(int denom, int delta) {
    setState(() {
      final current = _counts[denom] ?? 0;
      final next = current + delta;
      if (next >= 0) {
        _counts[denom] = next;
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final diff = _discrepancy;

    Color statusColor = AppColors.success;
    String statusText = 'Khớp 100% (Không chênh lệch)';
    IconData statusIcon = Icons.check_circle_outline;

    if (diff < 0) {
      statusColor = AppColors.error;
      statusText = 'Thiếu tiền trong két: ${Formatters.formatVND(diff)}';
      statusIcon = Icons.warning_amber_rounded;
    } else if (diff > 0) {
      statusColor = Colors.blue;
      statusText = 'Thừa tiền trong két: +${Formatters.formatVND(diff)}';
      statusIcon = Icons.info_outline;
    }

    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 520, maxHeight: 680),
        child: Padding(
          padding: const EdgeInsets.all(20),
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
                    child: const Icon(Icons.point_of_sale, color: AppColors.primary, size: 24),
                  ),
                  const SizedBox(width: 12),
                  const Expanded(
                    child: Text(
                      'Chốt két & Giao ca Thu ngân',
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

              // Doanh thu dự kiến trên phần mềm
              Row(
                children: [
                  const Text('Tiền theo phần mềm:', style: TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                  const SizedBox(width: 12),
                  Expanded(
                    child: SizedBox(
                      height: 38,
                      child: TextField(
                        controller: _expectedCtrl,
                        keyboardType: TextInputType.number,
                        decoration: InputDecoration(
                          hintText: 'Nhập doanh thu tiền mặt...',
                          contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
                          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
                        ),
                        onChanged: (val) {
                          setState(() {
                            _expectedCash = double.tryParse(val.replaceAll(RegExp(r'[^0-9]'), '')) ?? 0;
                          });
                        },
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),

              // Bảng đếm mệnh giá
              const Text('Kiểm đếm mệnh giá thực tế trong két:', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
              const SizedBox(height: 8),

              Expanded(
                child: ListView(
                  children: _counts.keys.map((denom) {
                    final count = _counts[denom] ?? 0;
                    final totalLine = denom * count;

                    return Container(
                      margin: const EdgeInsets.symmetric(vertical: 3),
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                      decoration: BoxDecoration(
                        color: count > 0 ? AppColors.primary.withValues(alpha: 0.05) : AppColors.background,
                        borderRadius: BorderRadius.circular(8),
                        border: Border.all(color: count > 0 ? AppColors.primary.withValues(alpha: 0.2) : AppColors.border),
                      ),
                      child: Row(
                        children: [
                          SizedBox(
                            width: 80,
                            child: Text(
                              Formatters.formatVND(denom.toDouble()),
                              style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 12),
                            ),
                          ),
                          const Spacer(),
                          IconButton(
                            icon: const Icon(Icons.remove_circle_outline, size: 20),
                            padding: EdgeInsets.zero,
                            constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
                            onPressed: () => _updateCount(denom, -1),
                          ),
                          Container(
                            constraints: const BoxConstraints(minWidth: 36),
                            alignment: Alignment.center,
                            child: Text(
                              '$count',
                              style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                            ),
                          ),
                          IconButton(
                            icon: const Icon(Icons.add_circle_outline, size: 20, color: AppColors.primary),
                            padding: EdgeInsets.zero,
                            constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
                            onPressed: () => _updateCount(denom, 1),
                          ),
                          const SizedBox(width: 8),
                          SizedBox(
                            width: 90,
                            child: Text(
                              Formatters.formatVND(totalLine.toDouble()),
                              textAlign: TextAlign.right,
                              style: TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 12,
                                color: totalLine > 0 ? AppColors.primary : AppColors.textMuted,
                              ),
                            ),
                          ),
                        ],
                      ),
                    );
                  }).toList(),
                ),
              ),

              const SizedBox(height: 12),

              // Tổng kết & Cảnh báo chênh lệch
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: statusColor.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: statusColor.withValues(alpha: 0.3)),
                ),
                child: Column(
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text('Thực tế trong két:', style: TextStyle(fontWeight: FontWeight.bold)),
                        Text(
                          Formatters.formatVND(_actualTotal),
                          style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16, color: statusColor),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Icon(statusIcon, size: 16, color: statusColor),
                        const SizedBox(width: 6),
                        Expanded(
                          child: Text(
                            statusText,
                            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 12, color: statusColor),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 14),

              // Nút hoàn tất giao ca
              ElevatedButton.icon(
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                ),
                icon: const Icon(Icons.check, color: Colors.white),
                label: const Text('Xác nhận chốt két ca trực', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
                onPressed: () {
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('Chốt két thành công! Tổng tiền thực tế: ${Formatters.formatVND(_actualTotal)}'),
                      backgroundColor: AppColors.success,
                    ),
                  );
                  Navigator.pop(context, true);
                },
              ),
            ],
          ),
        ),
      ),
    );
  }
}
