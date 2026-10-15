import 'dart:async';
import 'package:flutter/material.dart';
import 'package:qr_flutter/qr_flutter.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/order_model.dart';
import '../../../data/repositories/data_repository.dart';

class VietQrDialog extends StatefulWidget {
  final double amount;
  final String orderCode;

  const VietQrDialog({
    super.key,
    required this.amount,
    required this.orderCode,
  });

  static Future<bool?> show(
    BuildContext context, {
    required double amount,
    required String orderCode,
  }) {
    return showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (context) => VietQrDialog(
        amount: amount,
        orderCode: orderCode,
      ),
    );
  }

  @override
  State<VietQrDialog> createState() => _VietQrDialogState();
}

class _VietQrDialogState extends State<VietQrDialog> {
  final DataRepository _repository = DataRepository();
  bool _isLoading = true;
  String? _errorMessage;
  String? _qrCodeData;
  int? _payOsOrderCode;
  Timer? _statusPollingTimer;
  bool _isPaid = false;

  @override
  void initState() {
    super.initState();
    _initializePayOsQr();
  }

  @override
  void dispose() {
    _statusPollingTimer?.cancel();
    super.dispose();
  }

  Future<void> _initializePayOsQr() async {
    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      final req = CreateQrRequest(
        amount: widget.amount.round(),
        description: 'Chuyen khoan POS',
      );

      final res = await _repository.createPayOsLink(req);
      if (!mounted) return;

      if (res.success && res.qrCode != null && res.qrCode!.isNotEmpty) {
        setState(() {
          _qrCodeData = res.qrCode;
          _payOsOrderCode = res.orderCode;
          _isLoading = false;
        });

        // Start polling payment status matching OrderController.cs background check
        if (_payOsOrderCode != null && _payOsOrderCode! > 0) {
          _startPollingStatus(_payOsOrderCode!);
        }
      } else {
        setState(() {
          _isLoading = false;
          _errorMessage = res.message ?? 'Máy chủ PayOS không thể khởi tạo mã QR.';
          // Fallback to standard VietQR payload with orderCode if PayOS credentials are empty
          _qrCodeData = 'EMARKET_PAY_${widget.amount.round()}_${widget.orderCode}';
        });
      }
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _errorMessage = 'Lỗi kết nối cổng thanh toán: ${e.toString().replaceFirst("Exception: ", "")}';
        _qrCodeData = 'EMARKET_PAY_${widget.amount.round()}_${widget.orderCode}';
      });
    }
  }

  void _startPollingStatus(int orderCode) {
    int checkCount = 0;
    _statusPollingTimer = Timer.periodic(const Duration(seconds: 3), (timer) async {
      checkCount++;
      if (checkCount > 60 || _isPaid) {
        timer.cancel();
        return;
      }

      try {
        final statusRes = await _repository.checkPayOsStatus(orderCode);
        if (!mounted) return;

        if (statusRes.success && statusRes.isPaid) {
          timer.cancel();
          setState(() {
            _isPaid = true;
          });
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Xác nhận: Đã nhận tiền thành công từ PayOS!'),
              backgroundColor: AppColors.success,
            ),
          );
          await Future.delayed(const Duration(milliseconds: 600));
          if (mounted) Navigator.pop(context, true);
        }
      } catch (_) {
        // Silently continue polling
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      titlePadding: const EdgeInsets.fromLTRB(20, 20, 20, 0),
      contentPadding: const EdgeInsets.fromLTRB(20, 16, 20, 0),
      actionsPadding: const EdgeInsets.all(16),
      title: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          const Row(
            children: [
              Icon(Icons.qr_code_2_rounded, color: AppColors.primary),
              SizedBox(width: 8),
              Text('Thanh toán PayOS / VietQR', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
            ],
          ),
          IconButton(
            icon: const Icon(Icons.close, size: 20),
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(),
            onPressed: () => Navigator.pop(context, false),
          ),
        ],
      ),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Amount Banner
            Container(
              width: double.infinity,
              padding: const EdgeInsets.symmetric(vertical: 12),
              decoration: BoxDecoration(
                color: AppColors.primary.withValues(alpha: 0.08),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                children: [
                  const Text('Tổng tiền cần thanh toán', style: TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                  const SizedBox(height: 4),
                  Text(
                    Formatters.formatVND(widget.amount),
                    style: const TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                      color: AppColors.primaryLight,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),

            // QR Box / Loading
            if (_isLoading)
              Container(
                height: 220,
                alignment: Alignment.center,
                child: const Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    CircularProgressIndicator(strokeWidth: 2.5),
                    SizedBox(height: 12),
                    Text('Đang tạo mã PayOS từ máy chủ...', style: TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                  ],
                ),
              )
            else ...[
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(16),
                  border: Border.all(color: AppColors.border),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.04),
                      blurRadius: 10,
                    ),
                  ],
                ),
                child: QrImageView(
                  data: _qrCodeData ?? widget.orderCode,
                  version: QrVersions.auto,
                  size: 200.0,
                  eyeStyle: const QrEyeStyle(
                    eyeShape: QrEyeShape.square,
                    color: AppColors.primary,
                  ),
                  dataModuleStyle: const QrDataModuleStyle(
                    dataModuleShape: QrDataModuleShape.square,
                    color: AppColors.textPrimary,
                  ),
                ),
              ),
              const SizedBox(height: 12),

              if (_payOsOrderCode != null)
                Text(
                  'Mã giao dịch PayOS: #$_payOsOrderCode',
                  style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: AppColors.textPrimary),
                ),

              if (_errorMessage != null)
                Padding(
                  padding: const EdgeInsets.only(top: 8),
                  child: Text(
                    _errorMessage!,
                    style: const TextStyle(fontSize: 11, color: AppColors.error),
                    textAlign: TextAlign.center,
                  ),
                ),

              const SizedBox(height: 6),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  SizedBox(
                    width: 12,
                    height: 12,
                    child: CircularProgressIndicator(
                      strokeWidth: 1.5,
                      color: _isPaid ? AppColors.success : AppColors.primary,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Text(
                    _isPaid
                        ? 'Đã nhận thanh toán thành công!'
                        : 'Đang tự động lắng nghe thanh toán...',
                    style: TextStyle(
                      fontSize: 11,
                      fontStyle: FontStyle.italic,
                      color: _isPaid ? AppColors.success : AppColors.textMuted,
                      fontWeight: _isPaid ? FontWeight.bold : FontWeight.normal,
                    ),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
      actions: [
        SizedBox(
          width: double.infinity,
          height: 44,
          child: OutlinedButton.icon(
            icon: const Icon(Icons.close_rounded, size: 18),
            label: const Text('HỦY THANH TOÁN', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.error,
              side: const BorderSide(color: AppColors.error),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
            ),
            onPressed: () {
              _statusPollingTimer?.cancel();
              Navigator.pop(context, false);
            },
          ),
        ),
      ],
    );
  }
}
