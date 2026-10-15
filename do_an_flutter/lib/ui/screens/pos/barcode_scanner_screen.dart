import 'package:flutter/material.dart';
import '../../widgets/barcode_scanner_wrapper.dart';

class BarcodeScannerScreen extends StatelessWidget {
  const BarcodeScannerScreen({super.key});

  static Future<String?> scan(BuildContext context) {
    return Navigator.push<String>(
      context,
      MaterialPageRoute(builder: (_) => const BarcodeScannerScreen()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.black,
      appBar: AppBar(
        backgroundColor: Colors.black,
        foregroundColor: Colors.white,
        title: const Text('Quét mã Barcode / QR', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
      ),
      body: BarcodeScannerWrapper(
        continuous: false,
        onBarcodeDetected: (code) {
          Navigator.pop(context, code);
        },
      ),
    );
  }
}

