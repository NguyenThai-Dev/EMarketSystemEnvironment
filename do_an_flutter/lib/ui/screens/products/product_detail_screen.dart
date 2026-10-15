import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/product_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../widgets/permission_guard.dart';
import 'add_edit_product_dialog.dart';

class ProductDetailScreen extends StatefulWidget {
  final ProductModel product;

  const ProductDetailScreen({
    super.key,
    required this.product,
  });

  @override
  State<ProductDetailScreen> createState() => _ProductDetailScreenState();
}

class _ProductDetailScreenState extends State<ProductDetailScreen> with SingleTickerProviderStateMixin {
  final DataRepository _repository = DataRepository();
  late TabController _tabController;

  List<ProductLotModel> _lots = [];
  bool _isLoadingLots = true;
  String? _lotsError;

  late ProductModel _currentProduct;

  @override
  void initState() {
    super.initState();
    _currentProduct = widget.product;
    _tabController = TabController(length: 2, vsync: this);
    _loadLots();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadLots() async {
    setState(() {
      _isLoadingLots = true;
      _lotsError = null;
    });

    try {
      final lots = await _repository.getProductLots(_currentProduct.productId);
      // Sort FEFO: Earliest expiry date first
      lots.sort((a, b) => a.expiryDate.compareTo(b.expiryDate));
      if (!mounted) return;
      setState(() {
        _lots = lots;
        _isLoadingLots = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoadingLots = false;
        _lotsError = 'Không thể tải danh sách lô hàng.';
      });
    }
  }

  Future<void> _handleDelete() async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Xác nhận xóa', style: TextStyle(fontWeight: FontWeight.bold)),
        content: Text('Bạn có chắc chắn muốn xóa sản phẩm "${_currentProduct.name}" không?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Hủy'),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.error),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Xóa', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    try {
      final success = await _repository.deleteProduct(_currentProduct.productId);
      if (!mounted) return;
      if (success) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Đã xóa sản phẩm thành công.'), backgroundColor: AppColors.success),
        );
        Navigator.pop(context, true);
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Không thể xóa sản phẩm.'), backgroundColor: AppColors.error),
        );
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Lỗi: $e'), backgroundColor: AppColors.error),
      );
    }
  }

  Future<void> _handleEdit() async {
    final updated = await AddEditProductDialog.show(context, product: _currentProduct);
    if (!mounted) return;
    if (updated == true) {
      // Reload product info if possible
      Navigator.pop(context, true);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Text(_currentProduct.name, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 17)),
        backgroundColor: Colors.white,
        elevation: 0,
        actions: [
          PermissionGuard(
            child: IconButton(
              icon: const Icon(Icons.edit_outlined, color: AppColors.primary),
              tooltip: 'Chỉnh sửa sản phẩm',
              onPressed: _handleEdit,
            ),
          ),
          PermissionGuard(
            requireAdmin: true,
            child: IconButton(
              icon: const Icon(Icons.delete_outline, color: AppColors.error),
              tooltip: 'Xóa sản phẩm',
              onPressed: _handleDelete,
            ),
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.textSecondary,
          indicatorColor: AppColors.primary,
          indicatorWeight: 3,
          tabs: const [
            Tab(text: 'Thông tin chung', icon: Icon(Icons.info_outline, size: 20)),
            Tab(text: 'Theo dõi Lô hàng (FEFO)', icon: Icon(Icons.inventory_2_outlined, size: 20)),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          _buildGeneralTab(),
          _buildLotsTab(),
        ],
      ),
    );
  }

  Widget _buildGeneralTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Product Image & Price Card
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColors.border),
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: Container(
                    width: 100,
                    height: 100,
                    color: AppColors.surfaceVariant,
                    child: _currentProduct.fullImageUrl != null
                        ? CachedNetworkImage(
                            imageUrl: _currentProduct.fullImageUrl!,
                            fit: BoxFit.cover,
                            errorWidget: (_, _, _) => const Icon(Icons.inventory_2_outlined, size: 40, color: AppColors.textMuted),
                          )
                        : const Icon(Icons.inventory_2_outlined, size: 40, color: AppColors.textMuted),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        _currentProduct.name,
                        style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                      ),
                      const SizedBox(height: 6),
                      Text(
                        'Mã SKU / Barcode: ${_currentProduct.sku}',
                        style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        Formatters.formatVND(_currentProduct.price),
                        style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.primary),
                      ),
                      if (_currentProduct.unit != null && _currentProduct.unit!.isNotEmpty)
                        Text(
                          'Đơn vị tính: ${_currentProduct.unit}',
                          style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                        ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),

          // Inventory & Stock Status Card
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Row(
                  children: [
                    Icon(Icons.warehouse_outlined, color: AppColors.primary, size: 20),
                    SizedBox(width: 8),
                    Text('Tình trạng tồn kho', style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold)),
                  ],
                ),
                const SizedBox(height: 12),
                Row(
                  children: [
                    Expanded(
                      child: _buildStockStatBox(
                        title: 'Tồn thực tế',
                        value: '${_currentProduct.stockQuantity}',
                        color: _currentProduct.stockQuantity > 0 ? AppColors.success : AppColors.error,
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: _buildStockStatBox(
                        title: 'Giá vốn ước tính',
                        value: Formatters.formatVND(_currentProduct.costPrice),
                        color: AppColors.textPrimary,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),

          // Attributes Details Card
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text('Thông số phân loại', style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold)),
                const SizedBox(height: 12),
                _buildInfoRow('Danh mục hàng hóa', _currentProduct.categoryName ?? 'Chưa phân loại'),
                _buildInfoRow('Nhà cung cấp', _currentProduct.supplierName ?? 'Chưa cập nhật'),
                _buildInfoRow('Mô tả sản phẩm', _currentProduct.description ?? 'Không có mô tả chi tiết'),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildLotsTab() {
    if (_isLoadingLots) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_lotsError != null) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.error_outline, size: 48, color: AppColors.error),
            const SizedBox(height: 8),
            Text(_lotsError!, style: const TextStyle(color: AppColors.textSecondary)),
            const SizedBox(height: 12),
            ElevatedButton(onPressed: _loadLots, child: const Text('Thử lại')),
          ],
        ),
      );
    }

    if (_lots.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: AppColors.surfaceVariant,
                shape: BoxShape.circle,
              ),
              child: const Icon(Icons.calendar_today_outlined, size: 48, color: AppColors.textMuted),
            ),
            const SizedBox(height: 16),
            const Text(
              'Chưa có dữ liệu lô hàng',
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
            ),
            const SizedBox(height: 6),
            const Text(
              'Sản phẩm này chưa được gán mã lô và hạn sử dụng trong CSDL.',
              style: TextStyle(fontSize: 12, color: AppColors.textSecondary),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: _lots.length,
      separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final lot = _lots[index];
        return _buildLotCard(lot);
      },
    );
  }

  Widget _buildLotCard(ProductLotModel lot) {
    Color badgeColor;
    String badgeText;
    IconData badgeIcon;

    if (lot.isExpired) {
      badgeColor = AppColors.error;
      badgeText = 'ĐÃ HẾT HẠN';
      badgeIcon = Icons.cancel;
    } else if (lot.isNearExpired) {
      badgeColor = Colors.orange.shade700;
      badgeText = 'CẬN DATE (${lot.daysRemaining} ngày)';
      badgeIcon = Icons.warning_amber_rounded;
    } else {
      badgeColor = AppColors.success;
      badgeText = 'CÒN HẠN (${lot.daysRemaining} ngày)';
      badgeIcon = Icons.check_circle_outline;
    }

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(
          color: lot.isNearExpired || lot.isExpired ? badgeColor.withValues(alpha: 0.5) : AppColors.border,
          width: lot.isNearExpired || lot.isExpired ? 1.5 : 1,
        ),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.03),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Row(
                children: [
                  const Icon(Icons.qr_code_scanner, size: 18, color: AppColors.primary),
                  const SizedBox(width: 8),
                  Text(
                    'Lô: ${lot.batchCode}',
                    style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                  ),
                ],
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                decoration: BoxDecoration(
                  color: badgeColor.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(badgeIcon, size: 12, color: badgeColor),
                    const SizedBox(width: 4),
                    Text(
                      badgeText,
                      style: TextStyle(color: badgeColor, fontSize: 11, fontWeight: FontWeight.bold),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          const Divider(height: 1, color: AppColors.border),
          const SizedBox(height: 10),

          Row(
            children: [
              Expanded(
                child: _buildLotDateCol(
                  title: 'Ngày sản xuất',
                  date: lot.manufacturingDate != null ? Formatters.formatDate(lot.manufacturingDate!) : 'N/A',
                ),
              ),
              Expanded(
                child: _buildLotDateCol(
                  title: 'Hạn sử dụng (FEFO)',
                  date: Formatters.formatDate(lot.expiryDate),
                  isBold: true,
                ),
              ),
            ],
          ),

          if (lot.costPrice != null && lot.costPrice! > 0) ...[
            const SizedBox(height: 8),
            Text(
              'Giá vốn lô: ${Formatters.formatVND(lot.costPrice!)}',
              style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildLotDateCol({required String title, required String date, bool isBold = false}) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(title, style: const TextStyle(fontSize: 11, color: AppColors.textMuted)),
        const SizedBox(height: 2),
        Text(
          date,
          style: TextStyle(
            fontSize: 13,
            fontWeight: isBold ? FontWeight.bold : FontWeight.w500,
            color: AppColors.textPrimary,
          ),
        ),
      ],
    );
  }

  Widget _buildStockStatBox({required String title, required String value, required Color color}) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 12),
      decoration: BoxDecoration(
        color: AppColors.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title, style: const TextStyle(fontSize: 11, color: AppColors.textSecondary)),
          const SizedBox(height: 4),
          Text(
            value,
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: color),
          ),
        ],
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: const TextStyle(fontSize: 11, color: AppColors.textMuted)),
          const SizedBox(height: 2),
          Text(value, style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w500, color: AppColors.textPrimary)),
        ],
      ),
    );
  }
}
