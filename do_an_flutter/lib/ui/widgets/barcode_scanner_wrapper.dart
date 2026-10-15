import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import '../../core/constants/app_colors.dart';

/// Enterprise Barcode & QR Scanner Widget
/// Lifecycle-safe: Stops camera feed when app goes into background to save power and prevent overheating.
class BarcodeScannerWrapper extends StatefulWidget {
  final ValueChanged<String> onBarcodeDetected;
  final bool continuous;
  final Duration debounceDuration;
  final Widget? overlayHeader;

  const BarcodeScannerWrapper({
    super.key,
    required this.onBarcodeDetected,
    this.continuous = false,
    this.debounceDuration = const Duration(milliseconds: 900),
    this.overlayHeader,
  });

  @override
  State<BarcodeScannerWrapper> createState() => _BarcodeScannerWrapperState();
}

class _BarcodeScannerWrapperState extends State<BarcodeScannerWrapper> with WidgetsBindingObserver {
  late MobileScannerController _controller;
  bool _isTorchOn = false;
  bool _isProcessing = false;
  String? _lastCode;
  DateTime? _lastDetectedTime;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _controller = MobileScannerController(
      detectionSpeed: DetectionSpeed.noDuplicates,
      facing: CameraFacing.back,
      torchEnabled: false,
    );
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (!_controller.value.isInitialized) return;
    if (state == AppLifecycleState.inactive || state == AppLifecycleState.paused) {
      _controller.stop();
    } else if (state == AppLifecycleState.resumed) {
      _controller.start();
    }
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _controller.dispose();
    super.dispose();
  }

  void _handleDetect(BarcodeCapture capture) {
    if (_isProcessing) return;

    final now = DateTime.now();
    for (final barcode in capture.barcodes) {
      final code = barcode.rawValue?.trim();
      if (code != null && code.isNotEmpty) {
        // Debounce check
        if (_lastCode == code &&
            _lastDetectedTime != null &&
            now.difference(_lastDetectedTime!) < widget.debounceDuration) {
          continue;
        }

        _lastCode = code;
        _lastDetectedTime = now;
        HapticFeedback.mediumImpact();

        if (!widget.continuous) {
          _isProcessing = true;
          widget.onBarcodeDetected(code);
        } else {
          widget.onBarcodeDetected(code);
        }
        break;
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        // Camera View
        MobileScanner(
          controller: _controller,
          onDetect: _handleDetect,
          errorBuilder: (context, error) {
            return Container(
              color: Colors.black87,
              child: Center(
                child: Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.camera_alt_outlined, color: Colors.white54, size: 54),
                      const SizedBox(height: 16),
                      Text(
                        'Không thể khởi động camera: ${error.errorCode.name}',
                        textAlign: TextAlign.center,
                        style: const TextStyle(color: Colors.white70, fontSize: 14),
                      ),
                      const SizedBox(height: 12),
                      OutlinedButton.icon(
                        style: OutlinedButton.styleFrom(foregroundColor: Colors.white),
                        icon: const Icon(Icons.refresh, size: 16),
                        label: const Text('Thử lại'),
                        onPressed: () => _controller.start(),
                      ),
                    ],
                  ),
                ),
              ),
            );
          },
        ),

        // Darkened Scan Cutout Overlay
        CustomPaint(
          size: Size.infinite,
          painter: _ScannerOverlayPainter(),
        ),

        // Top Toolbar (Flash & Switch Camera)
        Positioned(
          top: 16,
          right: 16,
          child: Row(
            children: [
              IconButton.filled(
                style: IconButton.styleFrom(
                  backgroundColor: Colors.black54,
                  foregroundColor: _isTorchOn ? Colors.amber : Colors.white,
                ),
                icon: Icon(_isTorchOn ? Icons.flash_on : Icons.flash_off, size: 20),
                onPressed: () async {
                  await _controller.toggleTorch();
                  setState(() => _isTorchOn = !_isTorchOn);
                },
              ),
              const SizedBox(width: 8),
              IconButton.filled(
                style: IconButton.styleFrom(
                  backgroundColor: Colors.black54,
                  foregroundColor: Colors.white,
                ),
                icon: const Icon(Icons.cameraswitch_outlined, size: 20),
                onPressed: () => _controller.switchCamera(),
              ),
            ],
          ),
        ),

        // Optional Top Header
        if (widget.overlayHeader != null)
          Positioned(
            top: 20,
            left: 20,
            child: widget.overlayHeader!,
          ),

        // Bottom Instruction
        Positioned(
          bottom: 40,
          left: 20,
          right: 20,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
            decoration: BoxDecoration(
              color: Colors.black.withValues(alpha: 0.65),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(Icons.qr_code_scanner, color: AppColors.secondary, size: 18),
                const SizedBox(width: 8),
                Text(
                  widget.continuous
                      ? 'Chế độ quét liên tục: Hướng mã vào khung'
                      : 'Căn chỉnh mã Barcode / QR vào tâm khung quét',
                  style: const TextStyle(color: Colors.white, fontSize: 13, fontWeight: FontWeight.w500),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}

class _ScannerOverlayPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final double boxWidth = size.width * 0.75;
    final double boxHeight = boxWidth * 0.75;
    final Rect scanRect = Rect.fromCenter(
      center: Offset(size.width / 2, size.height / 2),
      width: boxWidth,
      height: boxHeight,
    );

    final backgroundPaint = Paint()
      ..color = Colors.black.withValues(alpha: 0.5)
      ..style = PaintingStyle.fill;

    // Draw dark overlay with cut-out hole
    final path = Path()
      ..addRect(Rect.fromLTWH(0, 0, size.width, size.height))
      ..addRRect(RRect.fromRectAndRadius(scanRect, const Radius.circular(16)))
      ..fillType = PathFillType.evenOdd;

    canvas.drawPath(path, backgroundPaint);

    // Draw focus corner borders
    final borderPaint = Paint()
      ..color = AppColors.primaryLight
      ..style = PaintingStyle.stroke
      ..strokeWidth = 3.5;

    const cornerLength = 24.0;
    // Top Left
    canvas.drawLine(scanRect.topLeft, scanRect.topLeft + const Offset(cornerLength, 0), borderPaint);
    canvas.drawLine(scanRect.topLeft, scanRect.topLeft + const Offset(0, cornerLength), borderPaint);

    // Top Right
    canvas.drawLine(scanRect.topRight, scanRect.topRight - const Offset(cornerLength, 0), borderPaint);
    canvas.drawLine(scanRect.topRight, scanRect.topRight + const Offset(0, cornerLength), borderPaint);

    // Bottom Left
    canvas.drawLine(scanRect.bottomLeft, scanRect.bottomLeft + const Offset(cornerLength, 0), borderPaint);
    canvas.drawLine(scanRect.bottomLeft, scanRect.bottomLeft - const Offset(0, cornerLength), borderPaint);

    // Bottom Right
    canvas.drawLine(scanRect.bottomRight, scanRect.bottomRight - const Offset(cornerLength, 0), borderPaint);
    canvas.drawLine(scanRect.bottomRight, scanRect.bottomRight - const Offset(0, cornerLength), borderPaint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
