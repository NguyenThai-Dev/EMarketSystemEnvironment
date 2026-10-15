import 'dart:convert';
import 'dart:io';
import 'package:flutter/foundation.dart';
import '../utils/formatters.dart';

enum PaperSize {
  mm58(32), // 58mm thermal paper, ~32 characters per line
  mm80(48); // 80mm thermal paper, ~48 characters per line

  final int maxChars;
  const PaperSize(this.maxChars);
}

class ReceiptItem {
  final String name;
  final int quantity;
  final double unitPrice;
  final double totalPrice;

  const ReceiptItem({
    required this.name,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
  });
}

class ReceiptData {
  final String storeName;
  final String storeAddress;
  final String storePhone;
  final String orderCode;
  final DateTime orderDate;
  final String cashierName;
  final String? customerName;
  final List<ReceiptItem> items;
  final double subtotal;
  final double discount;
  final double totalAmount;
  final String paymentMethod;
  final double? customerGiven;
  final double? changeAmount;
  final String? footerNote;

  const ReceiptData({
    required this.storeName,
    required this.storeAddress,
    required this.storePhone,
    required this.orderCode,
    required this.orderDate,
    required this.cashierName,
    this.customerName,
    required this.items,
    required this.subtotal,
    this.discount = 0.0,
    required this.totalAmount,
    this.paymentMethod = 'Tiền mặt',
    this.customerGiven,
    this.changeAmount,
    this.footerNote = 'Cảm ơn quý khách & Hẹn gặp lại!',
  });
}

/// Enterprise Thermal Printing Service (ESC/POS Standard)
/// Connects via raw TCP Socket (port 9100) to LAN/Wi-Fi POS printers or generates simulated receipt buffers.
class ThermalPrinterService {
  // ESC/POS Command Constants
  static const List<int> _cmdInit = [0x1B, 0x40];
  static const List<int> _cmdAlignLeft = [0x1B, 0x61, 0x00];
  static const List<int> _cmdAlignCenter = [0x1B, 0x61, 0x01];
  static const List<int> _cmdBoldOn = [0x1B, 0x45, 0x01];
  static const List<int> _cmdBoldOff = [0x1B, 0x45, 0x00];
  static const List<int> _cmdTextDoubleSize = [0x1D, 0x21, 0x11];
  static const List<int> _cmdTextNormal = [0x1D, 0x21, 0x00];
  static const List<int> _cmdFeedLines = [0x1B, 0x64, 0x03];
  static const List<int> _cmdCut = [0x1D, 0x56, 0x42, 0x00];

  /// Builds raw ESC/POS byte sequence
  static List<int> buildEscPosBytes(ReceiptData data, {PaperSize size = PaperSize.mm80}) {
    final bytes = <int>[];
    final cols = size.maxChars;

    void appendLine([String text = '']) {
      bytes.addAll(utf8.encode('$text\n'));
    }

    void appendDivider() {
      appendLine('-' * cols);
    }

    void appendDoubleDivider() {
      appendLine('=' * cols);
    }

    String makeTwoColumns(String left, String right) {
      final availableSpace = cols - right.length;
      if (left.length > availableSpace - 1) {
        left = left.substring(0, availableSpace - 1);
      }
      return left.padRight(cols - right.length) + right;
    }

    // 1. Initialize
    bytes.addAll(_cmdInit);

    // 2. Header (Centered, Double size for Store Name)
    bytes.addAll(_cmdAlignCenter);
    bytes.addAll(_cmdBoldOn);
    bytes.addAll(_cmdTextDoubleSize);
    appendLine(data.storeName.toUpperCase());
    bytes.addAll(_cmdTextNormal);
    bytes.addAll(_cmdBoldOff);

    appendLine(data.storeAddress);
    appendLine('Hotline: ${data.storePhone}');
    bytes.addAll(_cmdBoldOn);
    appendLine('HÓA ĐƠN THANH TOÁN');
    bytes.addAll(_cmdBoldOff);
    appendDoubleDivider();

    // 3. Metadata (Left aligned)
    bytes.addAll(_cmdAlignLeft);
    appendLine('Mã đơn : ${data.orderCode}');
    appendLine('Ngày   : ${Formatters.formatDateTime(data.orderDate)}');
    appendLine('Thu ngân: ${data.cashierName}');
    if (data.customerName != null && data.customerName!.isNotEmpty) {
      appendLine('Khách hàng: ${data.customerName}');
    }
    appendDivider();

    // 4. Items Table Header
    bytes.addAll(_cmdBoldOn);
    appendLine(makeTwoColumns('Tên hàng hóa', 'Thành tiền'));
    bytes.addAll(_cmdBoldOff);
    appendDivider();

    // 5. Line Items
    for (final item in data.items) {
      appendLine(item.name);
      final qtyPrice = '  ${item.quantity} x ${Formatters.formatNumber(item.unitPrice)}';
      final total = Formatters.formatNumber(item.totalPrice);
      appendLine(makeTwoColumns(qtyPrice, total));
    }
    appendDivider();

    // 6. Summary Totals
    appendLine(makeTwoColumns('Tổng tiền hàng:', Formatters.formatVND(data.subtotal)));
    if (data.discount > 0) {
      appendLine(makeTwoColumns('Chiết khấu:', '-${Formatters.formatVND(data.discount)}'));
    }

    bytes.addAll(_cmdBoldOn);
    appendLine(makeTwoColumns('TỔNG THANH TOÁN:', Formatters.formatVND(data.totalAmount)));
    bytes.addAll(_cmdBoldOff);

    appendLine(makeTwoColumns('Phương thức:', data.paymentMethod));
    if (data.customerGiven != null && data.customerGiven! > 0) {
      appendLine(makeTwoColumns('Tiền khách đưa:', Formatters.formatVND(data.customerGiven)));
      if (data.changeAmount != null) {
        appendLine(makeTwoColumns('Tiền trả lại:', Formatters.formatVND(data.changeAmount)));
      }
    }
    appendDoubleDivider();

    // 7. Footer
    bytes.addAll(_cmdAlignCenter);
    if (data.footerNote != null && data.footerNote!.isNotEmpty) {
      appendLine(data.footerNote!);
    }
    appendLine('Hóa đơn điện tử khởi tạo từ hệ thống EMarket');

    // 8. Feed & Paper Cut
    bytes.addAll(_cmdFeedLines);
    bytes.addAll(_cmdCut);

    return bytes;
  }

  /// Sends raw ESC/POS bytes to network POS printer over TCP (Port 9100)
  static Future<bool> printOverNetwork({
    required String printerIp,
    int port = 9100,
    required ReceiptData data,
    PaperSize size = PaperSize.mm80,
    Duration timeout = const Duration(seconds: 4),
  }) async {
    try {
      final bytes = buildEscPosBytes(data, size: size);
      final socket = await Socket.connect(printerIp, port, timeout: timeout);
      socket.add(bytes);
      await socket.flush();
      await socket.close();
      return true;
    } catch (e) {
      debugPrint('[ThermalPrinterService] Print error: $e');
      return false;
    }
  }

  /// Generates a monospaced ASCII text preview for UI display
  static String generateTextPreview(ReceiptData data, {PaperSize size = PaperSize.mm80}) {
    final cols = size.maxChars;
    final buffer = StringBuffer();

    void line([String text = '']) => buffer.writeln(text);
    void center(String text) {
      if (text.length >= cols) {
        line(text);
      } else {
        final pad = (cols - text.length) ~/ 2;
        line('${' ' * pad}$text');
      }
    }

    void divider([String char = '-']) => line(char * cols);

    String twoCol(String left, String right) {
      final available = cols - right.length;
      if (left.length > available - 1) {
        left = left.substring(0, available - 1);
      }
      return left.padRight(cols - right.length) + right;
    }

    center(data.storeName.toUpperCase());
    center(data.storeAddress);
    center('Hotline: ${data.storePhone}');
    divider('=');
    center('HÓA ĐƠN THANH TOÁN');
    divider('=');
    line('Mã đơn : ${data.orderCode}');
    line('Ngày   : ${Formatters.formatDateTime(data.orderDate)}');
    line('Thu ngân: ${data.cashierName}');
    if (data.customerName != null && data.customerName!.isNotEmpty) {
      line('Khách hàng: ${data.customerName}');
    }
    divider('-');
    line(twoCol('Tên hàng', 'Thành tiền'));
    divider('-');
    for (final item in data.items) {
      line(item.name);
      line(twoCol('  ${item.quantity} x ${Formatters.formatNumber(item.unitPrice)}', Formatters.formatNumber(item.totalPrice)));
    }
    divider('-');
    line(twoCol('Tổng tiền hàng:', Formatters.formatVND(data.subtotal)));
    if (data.discount > 0) {
      line(twoCol('Chiết khấu:', '-${Formatters.formatVND(data.discount)}'));
    }
    line(twoCol('TỔNG CỘNG:', Formatters.formatVND(data.totalAmount)));
    line(twoCol('Thanh toán:', data.paymentMethod));
    if (data.customerGiven != null && data.customerGiven! > 0) {
      line(twoCol('Tiền khách đưa:', Formatters.formatVND(data.customerGiven)));
      if (data.changeAmount != null) {
        line(twoCol('Tiền trả lại:', Formatters.formatVND(data.changeAmount)));
      }
    }
    divider('=');
    if (data.footerNote != null) center(data.footerNote!);
    center('Hệ thống bán lẻ EMarket');

    return buffer.toString();
  }
}
