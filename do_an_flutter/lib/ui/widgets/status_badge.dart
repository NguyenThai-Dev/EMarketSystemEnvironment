import 'package:flutter/material.dart';
import '../../core/constants/app_colors.dart';

enum StatusBadgeType {
  success,
  warning,
  error,
  info,
  neutral,
}

class StatusBadge extends StatelessWidget {
  final String text;
  final StatusBadgeType type;
  final Color? color;
  final IconData? icon;
  final double fontSize;
  final EdgeInsetsGeometry padding;

  const StatusBadge({
    super.key,
    String? text,
    String? label,
    this.type = StatusBadgeType.neutral,
    this.color,
    this.icon,
    this.fontSize = 11,
    this.padding = const EdgeInsets.symmetric(horizontal: 8, vertical: 3.5),
  }) : text = text ?? label ?? '';

  /// Factory helper that parses common status strings to appropriate StatusBadgeType
  factory StatusBadge.fromStatus(String? status, {double fontSize = 11}) {
    final s = (status ?? '').trim().toUpperCase();
    if (s == 'COMPLETED' || s == 'PAID' || s == 'SUCCESS' || s == 'HOÀN THÀNH' || s == 'APPROVED' || s == 'ĐÃ DUYỆT') {
      return StatusBadge(
        text: status ?? 'Hoàn thành',
        type: StatusBadgeType.success,
        icon: Icons.check_circle_outline_rounded,
        fontSize: fontSize,
      );
    }
    if (s == 'PENDING' || s == 'WAITING' || s == 'PROCESSING' || s == 'CHỜ XỬ LÝ' || s == 'CHỜ DUYỆT') {
      return StatusBadge(
        text: status ?? 'Chờ xử lý',
        type: StatusBadgeType.warning,
        icon: Icons.access_time_rounded,
        fontSize: fontSize,
      );
    }
    if (s == 'CANCELLED' || s == 'CANCELED' || s == 'REJECTED' || s == 'ĐÃ HỦY' || s == 'TỪ CHỐI') {
      return StatusBadge(
        text: status ?? 'Đã hủy',
        type: StatusBadgeType.error,
        icon: Icons.cancel_outlined,
        fontSize: fontSize,
      );
    }
    if (s == 'IN' || s == 'NHẬP' || s == 'NHẬP KHO') {
      return StatusBadge(
        text: status ?? 'Nhập kho',
        type: StatusBadgeType.success,
        icon: Icons.arrow_downward_rounded,
        fontSize: fontSize,
      );
    }
    if (s == 'OUT' || s == 'XUẤT' || s == 'XUẤT BÁN') {
      return StatusBadge(
        text: status ?? 'Xuất kho',
        type: StatusBadgeType.info,
        icon: Icons.arrow_upward_rounded,
        fontSize: fontSize,
      );
    }
    if (s == 'ADJUST' || s == 'KIỂM KÊ') {
      return StatusBadge(
        text: status ?? 'Kiểm kê',
        type: StatusBadgeType.warning,
        icon: Icons.tune_rounded,
        fontSize: fontSize,
      );
    }
    return StatusBadge(
      text: status ?? 'Khác',
      type: StatusBadgeType.neutral,
      fontSize: fontSize,
    );
  }

  @override
  Widget build(BuildContext context) {
    Color bg;
    Color fg;

    if (color != null) {
      bg = color!.withValues(alpha: 0.12);
      fg = color!;
    } else {
      switch (type) {
        case StatusBadgeType.success:
          bg = AppColors.emerald.withValues(alpha: 0.12);
          fg = const Color(0xFF047857); // Deep Emerald text
          break;
        case StatusBadgeType.warning:
          bg = AppColors.warning.withValues(alpha: 0.15);
          fg = const Color(0xFFB45309); // Deep Amber text
          break;
        case StatusBadgeType.error:
          bg = AppColors.error.withValues(alpha: 0.12);
          fg = const Color(0xFFB91C1C); // Deep Red text
          break;
        case StatusBadgeType.info:
          bg = AppColors.info.withValues(alpha: 0.12);
          fg = const Color(0xFF0369A1); // Deep Sky text
          break;
        case StatusBadgeType.neutral:
          bg = AppColors.surfaceVariant;
          fg = AppColors.textSecondary;
          break;
      }
    }

    return Container(
      padding: padding,
      decoration: BoxDecoration(
        color: bg,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (icon != null) ...[
            Icon(icon, size: fontSize + 2, color: fg),
            const SizedBox(width: 4),
          ],
          Text(
            text,
            style: TextStyle(
              fontSize: fontSize,
              fontWeight: FontWeight.w600,
              color: fg,
            ),
          ),
        ],
      ),
    );
  }
}
