import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../data/models/inventory_model.dart';
import '../../../data/models/product_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/product_provider.dart';

class StocktakeItem {
  final ProductModel product;
  final int systemStock;
  int actualCount;

  StocktakeItem({
    required this.product,
    required this.systemStock,
    int? actualCount,
  }) : actualCount = actualCount ?? systemStock;

  int get difference => actualCount - systemStock;
}

class StocktakingScreen extends StatefulWidget {
  final WarehouseModel warehouse;

  const StocktakingScreen({
    super.key,
    required this.warehouse,
  });

  @override
  State<StocktakingScreen> createState() => _StocktakingScreenState();
}

class _StocktakingScreenState extends State<StocktakingScreen> {
  final DataRepository _repository = DataRepository();
  final MobileScannerController _scannerController = MobileScannerController(
    detectionSpeed: DetectionSpeed.normal,
  );

  final Map<int, StocktakeItem> _stocktakeMap = {};
  bool _isContinuousScan = true;
  bool _isTorchOn = false;
  bool _isSaving = false;
  DateTime _lastScanTime = DateTime.now();

  @override
  void initState() {
    super.initState();
    _initCatalog();
  }

  @override
  void dispose() {
    _scannerController.dispose();
    super.dispose();
  }

  void _initCatalog() {
    final prods = context.read<ProductProvider>().products;
    for (final p in prods) {
      _stocktakeMap[p.productId] = StocktakeItem(
        product: p,
        systemStock: p.stockQuantity,
      );
    }
  }

  void _handleBarcodeDetect(BarcodeCapture capture) {
    if (!_isContinuousScan) return;

    final now = DateTime.now();
    if (now.difference(_lastScanTime).inMilliseconds < 800) return; // Debounce scan
    _lastScanTime = now;

    final barcode = capture.barcodes.firstOrNull?.rawValue;
    if (barcode == null || barcode.trim().isEmpty) return;

    final cleanCode = barcode.trim().toLowerCase();
    StocktakeItem? foundItem;

    for (final item in _stocktakeMap.values) {
      if (item.product.sku.toLowerCase() == cleanCode) {
        foundItem = item;
        break;
      }
    }

    if (foundItem != null) {
      HapticFeedback.mediumImpact();
      setState(() {
        foundItem!.actualCount += 1;
      });

      ScaffoldMessenger.of(context).hideCurrentSnackBar();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Đã đếm: ${foundItem.product.name} (Thực tế: ${foundItem.actualCount})'),
          duration: const Duration(seconds: 1),
          backgroundColor: AppColors.primary,
        ),
      );
    } else {
      HapticFeedback.vibrate();
      ScaffoldMessenger.of(context).hideCurrentSnackBar();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Không tìm thấy sản phẩm có mã: $barcode'),
          duration: const Duration(seconds: 1),
          backgroundColor: AppColors.warning,
        ),
      );
    }
  }

  Future<void> _handleSaveAdjustments() async {
    final adjustedItems = _stocktakeMap.values.where((item) => item.difference != 0).toList();

    if (adjustedItems.isEmpty) {
      showDialog(
        context: context,
        builder: (ctx) => AlertDialog(
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
          title: const Text('Số liệu khớp hoàn toàn'),
          content: const Text('Không có mặt hàng nào bị lệch giữa số lượng thực tế và phần mềm.'),
          actions: [
            ElevatedButton(
              onPressed: () => Navigator.pop(ctx),
              child: const Text('Đóng'),
            ),
          ],
        ),
      );
      return;
    }

    final auth = context.read<AuthProvider>();
    final userId = auth.currentUser?.userId ?? 1;

    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Lưu điều chỉnh kho', style: TextStyle(fontWeight: FontWeight.bold)),
        content: Text('Phát hiện ${adjustedItems.length} mặt hàng có chênh lệch tồn kho. Bạn có muốn cập nhật vào hệ thống không?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Xác nhận lưu', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    setState(() => _isSaving = true);

    int successCount = 0;
    for (final item in adjustedItems) {
      try {
        final req = StockAdjustmentRequest(
          movementType: 'ADJUSTMENT',
          warehouseId: widget.warehouse.warehouseId,
          productId: item.product.productId,
          quantityChange: item.difference.toDouble(),
          reason: 'Kiểm kê kho siêu tốc Mobile',
          userId: userId,
        );
        final ok = await _repository.adjustStock(req);
        if (ok) successCount++;
      } catch (_) {}
    }

    if (!mounted) return;
    setState(() => _isSaving = false);

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Row(
          children: [
            Icon(Icons.check_circle, color: AppColors.success),
            SizedBox(width: 8),
            Text('Kiểm kê hoàn tất'),
          ],
        ),
        content: Text('Đã cập nhật điều chỉnh thành công $successCount / ${adjustedItems.length} mặt hàng vào kho "${widget.warehouse.name}".'),
        actions: [
          ElevatedButton(
            onPressed: () {
              Navigator.pop(ctx);
              Navigator.pop(context, true);
            },
            child: const Text('Xong'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final itemsList = _stocktakeMap.values.toList();
    final totalDiscrepancies = itemsList.where((i) => i.difference != 0).length;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Kiểm kê kho siêu tốc', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
            Text(
              'Kho: ${widget.warehouse.name}',
              style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
            ),
          ],
        ),
        actions: [
          IconButton(
            icon: Icon(_isTorchOn ? Icons.flash_on : Icons.flash_off),
            tooltip: 'Đèn Flash',
            onPressed: () {
              _scannerController.toggleTorch();
              setState(() => _isTorchOn = !_isTorchOn);
            },
          ),
          IconButton(
            icon: Icon(_isContinuousScan ? Icons.videocam : Icons.videocam_off),
            tooltip: _isContinuousScan ? 'Tắt quét liên tục' : 'Bật quét liên tục',
            onPressed: () => setState(() => _isContinuousScan = !_isContinuousScan),
          ),
        ],
      ),
      body: Column(
        children: [
          // Live Camera Barcode Scanner Viewport
          SizedBox(
            height: 180,
            child: Stack(
              children: [
                MobileScanner(
                  controller: _scannerController,
                  onDetect: _handleBarcodeDetect,
                ),
                Center(
                  child: Container(
                    width: 220,
                    height: 90,
                    decoration: BoxDecoration(
                      border: Border.all(color: AppColors.primary, width: 2),
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                ),
                Positioned(
                  bottom: 8,
                  left: 0,
                  right: 0,
                  child: Center(
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
                      decoration: BoxDecoration(
                        color: Colors.black.withValues(alpha: 0.6),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: const Text(
                        'Lia camera qua mã vạch để tăng số lượng',
                        style: TextStyle(color: Colors.white, fontSize: 11),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),

          // Discrepancy Status Banner
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
            color: totalDiscrepancies > 0 ? AppColors.warning.withValues(alpha: 0.15) : AppColors.surfaceVariant,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Tổng mặt hàng: ${itemsList.length}',
                  style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
                ),
                Text(
                  totalDiscrepancies > 0 ? 'Có $totalDiscrepancies món lệch số lượng' : 'Chưa có chênh lệch',
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.bold,
                    color: totalDiscrepancies > 0 ? Colors.orange.shade800 : AppColors.success,
                  ),
                ),
              ],
            ),
          ),

          // Count Table Header
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            color: Colors.grey.shade100,
            child: const Row(
              children: [
                Expanded(flex: 4, child: Text('Sản phẩm', style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold))),
                Expanded(flex: 2, child: Text('Hệ thống', textAlign: TextAlign.center, style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold))),
                Expanded(flex: 3, child: Text('Thực tế', textAlign: TextAlign.center, style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold))),
                Expanded(flex: 2, child: Text('Lệch', textAlign: TextAlign.right, style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold))),
              ],
            ),
          ),

          // Stocktaking Items List
          Expanded(
            child: ListView.separated(
              itemCount: itemsList.length,
              separatorBuilder: (_, _) => const Divider(height: 1, color: AppColors.border),
              itemBuilder: (context, index) {
                final item = itemsList[index];
                final diff = item.difference;

                Color diffColor = AppColors.textMuted;
                if (diff < 0) diffColor = AppColors.error;
                if (diff > 0) diffColor = AppColors.success;

                return Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                  child: Row(
                    children: [
                      // Product info
                      Expanded(
                        flex: 4,
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              item.product.name,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
                            ),
                            Text(
                              item.product.sku,
                              style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                            ),
                          ],
                        ),
                      ),

                      // System Stock
                      Expanded(
                        flex: 2,
                        child: Text(
                          '${item.systemStock}',
                          textAlign: TextAlign.center,
                          style: const TextStyle(fontSize: 13, color: AppColors.textSecondary),
                        ),
                      ),

                      // Actual Counter with quick buttons
                      Expanded(
                        flex: 3,
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            InkWell(
                              onTap: () {
                                if (item.actualCount > 0) {
                                  setState(() => item.actualCount--);
                                }
                              },
                              child: const Icon(Icons.remove_circle_outline, size: 20, color: AppColors.textMuted),
                            ),
                            Padding(
                              padding: const EdgeInsets.symmetric(horizontal: 8),
                              child: Text(
                                '${item.actualCount}',
                                style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
                              ),
                            ),
                            InkWell(
                              onTap: () => setState(() => item.actualCount++),
                              child: const Icon(Icons.add_circle_outline, size: 20, color: AppColors.primary),
                            ),
                          ],
                        ),
                      ),

                      // Difference
                      Expanded(
                        flex: 2,
                        child: Text(
                          diff > 0 ? '+$diff' : '$diff',
                          textAlign: TextAlign.right,
                          style: TextStyle(
                            fontSize: 13,
                            fontWeight: FontWeight.bold,
                            color: diffColor,
                          ),
                        ),
                      ),
                    ],
                  ),
                );
              },
            ),
          ),

          // Bottom Bar Action
          Container(
            padding: const EdgeInsets.all(16),
            decoration: const BoxDecoration(
              color: Colors.white,
              border: Border(top: BorderSide(color: AppColors.border)),
            ),
            child: SizedBox(
              width: double.infinity,
              height: 48,
              child: ElevatedButton.icon(
                icon: _isSaving
                    ? const SizedBox(width: 18, height: 18, child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white))
                    : const Icon(Icons.check_circle_outline),
                label: Text(
                  _isSaving ? 'Đang lưu điều chỉnh...' : 'Hoàn tất & Điều chỉnh tồn kho ($totalDiscrepancies lệch)',
                  style: const TextStyle(fontWeight: FontWeight.bold, color: Colors.white),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: totalDiscrepancies > 0 ? AppColors.primary : AppColors.success,
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                ),
                onPressed: _isSaving ? null : _handleSaveAdjustments,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
