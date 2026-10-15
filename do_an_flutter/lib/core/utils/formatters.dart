import 'package:intl/intl.dart';

class Formatters {
  static final _currencyFormat = NumberFormat.currency(locale: 'vi_VN', symbol: '₫', decimalDigits: 0);
  static final _numberFormat = NumberFormat('#,###', 'vi_VN');
  static final _dateFormat = DateFormat('dd/MM/yyyy');
  static final _dateTimeFormat = DateFormat('HH:mm dd/MM/yyyy');

  static String formatVND(num? value) {
    if (value == null) return '0 ₫';
    return _currencyFormat.format(value).trim();
  }

  /// VND Safe-Zone formatter (Prevents RenderFlex overflow on tight mobile screens)
  static String formatVndSafe(num? value, {bool compact = false}) {
    if (value == null) return '0 ₫';
    final abs = value.abs();
    if (compact) {
      if (abs >= 1000000000) {
        final billions = value / 1000000000;
        return '${billions.toStringAsFixed(2).replaceAll(RegExp(r'\.?0+$'), '')} Tỷ ₫';
      } else if (abs >= 10000000) {
        final millions = value / 1000000;
        return '${millions.toStringAsFixed(1).replaceAll(RegExp(r'\.?0+$'), '')} Tr ₫';
      }
    }
    return _currencyFormat.format(value).trim();
  }

  static String formatNumber(num? value) {
    if (value == null) return '0';
    return _numberFormat.format(value);
  }

  static String formatDate(DateTime? date) {
    if (date == null) return '';
    return _dateFormat.format(date);
  }

  static String formatDateTime(DateTime? dateTime) {
    if (dateTime == null) return '';
    return _dateTimeFormat.format(dateTime);
  }
}
