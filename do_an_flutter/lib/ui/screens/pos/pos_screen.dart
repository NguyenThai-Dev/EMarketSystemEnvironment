import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/customer_model.dart';
import '../../../data/models/product_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/pos_cart_provider.dart';
import '../../../providers/product_provider.dart';
import '../../widgets/branch_selector_widget.dart';
import '../../widgets/flat_card.dart';
import '../../widgets/shimmer_skeleton.dart';
import '../../widgets/vnd_text.dart';
import '../orders/orders_history_screen.dart';
import 'barcode_scanner_screen.dart';
import 'pos_cart_sheet.dart';
import 'pos_payment_sheet.dart';

class PosScreen extends StatefulWidget {
  const PosScreen({super.key});

  @override
  State<PosScreen> createState() => _PosScreenState();
}

class _PosScreenState extends State<PosScreen> {
  final _barcodeInputController = TextEditingController();
  final _barcodeFocusNode = FocusNode();
  bool _isSearchingBarcode = false;
  int? _lastBranchId;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final auth = context.read<AuthProvider>();
      context.read<ProductProvider>().init(branchId: auth.selectedBranch?.branchId);
    });
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    final auth = context.watch<AuthProvider>();
    final currentBranchId = auth.selectedBranch?.branchId;
    if (_lastBranchId != currentBranchId) {
      _lastBranchId = currentBranchId;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) {
          context.read<ProductProvider>().init(branchId: currentBranchId);
        }
      });
    }
  }

  @override
  void dispose() {
    _barcodeInputController.dispose();
    _barcodeFocusNode.dispose();
    super.dispose();
  }

  /// Quét mã Barcode bằng Camera
  Future<void> _handleCameraScan() async {
    final barcode = await BarcodeScannerScreen.scan(context);
    if (barcode == null || barcode.isEmpty || !mounted) return;
    await _processBarcode(barcode);
  }

  /// Xử lý tra cứu Barcode & Từ khóa tên sản phẩm (từ Camera hoặc Nhập tay / Máy quét USB)
  Future<void> _processBarcode(String rawBarcode) async {
    final query = rawBarcode.trim();
    if (query.isEmpty) return;

    setState(() => _isSearchingBarcode = true);
    final productProv = context.read<ProductProvider>();
    final cart = context.read<PosCartProvider>();
    final auth = context.read<AuthProvider>();

    // 1. Kiểm tra nhanh trong RAM
    bool added = cart.addByBarcode(query, productProv.products);
    List<ProductModel> remoteMatches = [];

    // 2. Nếu chưa có trên RAM, gọi API backend
    if (!added) {
      try {
        remoteMatches = await DataRepository().searchProductsByBarcode(
          query,
          branchId: auth.selectedBranch?.branchId,
        );
        if (remoteMatches.length == 1) {
          cart.addToCart(remoteMatches.first);
          added = true;
        }
      } catch (_) {}
    }

    if (!mounted) return;
    setState(() => _isSearchingBarcode = false);

    if (remoteMatches.length > 1) {
      _showMultipleMatchesSheet(remoteMatches);
      return;
    }

    if (added) {
      HapticFeedback.lightImpact();
      _barcodeInputController.clear();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Row(
            children: [
              const Icon(Icons.check_circle_outline, color: Colors.white, size: 20),
              const SizedBox(width: 8),
              Expanded(child: Text('Đã thêm sản phẩm ($query)')),
            ],
          ),
          backgroundColor: AppColors.emerald,
          duration: const Duration(milliseconds: 900),
          behavior: SnackBarBehavior.floating,
        ),
      );
    } else {
      HapticFeedback.vibrate();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Row(
            children: [
              const Icon(Icons.warning_amber_rounded, color: Colors.white, size: 20),
              const SizedBox(width: 8),
              Expanded(child: Text('Không tìm thấy sản phẩm có mã hoặc tên: $query')),
            ],
          ),
          backgroundColor: AppColors.warning,
          duration: const Duration(seconds: 2),
          behavior: SnackBarBehavior.floating,
        ),
      );
    }
  }

  void _showMultipleMatchesSheet(List<ProductModel> matches) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) {
        final cart = context.read<PosCartProvider>();
        return Container(
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
          ),
          padding: const EdgeInsets.symmetric(vertical: 16),
          constraints: BoxConstraints(maxHeight: MediaQuery.of(context).size.height * 0.65),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Tìm thấy ${matches.length} sản phẩm phù hợp',
                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                    ),
                    IconButton(icon: const Icon(Icons.close), onPressed: () => Navigator.pop(ctx)),
                  ],
                ),
              ),
              const Divider(height: 1),
              Expanded(
                child: ListView.separated(
                  itemCount: matches.length,
                  separatorBuilder: (_, __) => const Divider(height: 1),
                  itemBuilder: (context, i) {
                    final p = matches[i];
                    return ListTile(
                      leading: ClipRRect(
                        borderRadius: BorderRadius.circular(8),
                        child: SizedBox(
                          width: 44,
                          height: 44,
                          child: p.fullImageUrl != null
                              ? CachedNetworkImage(
                                  imageUrl: p.fullImageUrl!,
                                  fit: BoxFit.cover,
                                  memCacheWidth: 160,
                                  memCacheHeight: 160,
                                )
                              : Container(color: AppColors.surfaceVariant, child: const Icon(Icons.inventory_2_outlined)),
                        ),
                      ),
                      title: Text(p.name, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
                      subtitle: Text(
                        '${Formatters.formatVND(p.price)} • Tồn: ${p.stockQuantity}',
                        style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
                      ),
                      trailing: ElevatedButton(
                        style: ElevatedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                          visualDensity: VisualDensity.compact,
                        ),
                        onPressed: () {
                          cart.addToCart(p);
                          Navigator.pop(ctx);
                          HapticFeedback.lightImpact();
                          _barcodeInputController.clear();
                        },
                        child: const Text('Chọn'),
                      ),
                    );
                  },
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  void _openCartSheet() {
    PosCartSheet.show(
      context,
      onProceedToCheckout: _openPaymentSheet,
      onOpenCustomerSelector: _openCustomerSelectorModal,
    );
  }

  void _openPaymentSheet() {
    PosPaymentSheet.show(context);
  }

  void _openCustomerSelectorModal() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => const _CustomerSelectorModal(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final cart = context.watch<PosCartProvider>();
    final productProv = context.watch<ProductProvider>();

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Thu ngân Siêu thị (POS)', style: TextStyle(fontSize: 17, fontWeight: FontWeight.bold)),
            Text(
              '${auth.selectedBranch?.branchName ?? "Chi nhánh"} • ${auth.currentUser?.fullName ?? "Admin"}',
              style: const TextStyle(fontSize: 11, color: AppColors.textSecondary),
            ),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.receipt_long_rounded, color: AppColors.primaryBrand),
            tooltip: 'Lịch sử hóa đơn',
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const OrdersHistoryScreen()),
              );
            },
          ),
          const Center(child: BranchSelectorWidget(compact: true)),
          const SizedBox(width: 4),
          if (!cart.isEmpty)
            IconButton(
              icon: const Icon(Icons.delete_sweep_outlined, color: AppColors.error),
              tooltip: 'Xóa toàn bộ giỏ hàng',
              onPressed: () => cart.clearCart(),
            ),
        ],
      ),
      body: Column(
        children: [
          // 1. Search Bar & Camera Scan Button
          _buildSearchAndScanBar(),

          // 2. Category Filter Pills
          _buildCategoryPills(productProv, auth.selectedBranch?.branchId),

          // 3. Product Catalog Grid
          Expanded(
            child: _buildProductGrid(productProv, cart),
          ),

          // 4. Floating Cart Dock
          _buildFloatingCartDock(cart),
        ],
      ),
    );
  }

  Widget _buildSearchAndScanBar() {
    return Container(
      padding: const EdgeInsets.fromLTRB(14, 10, 14, 10),
      color: Colors.white,
      child: Row(
        children: [
          // Search / Barcode Input
          Expanded(
            child: SizedBox(
              height: 44,
              child: TextField(
                controller: _barcodeInputController,
                focusNode: _barcodeFocusNode,
                textInputAction: TextInputAction.search,
                style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
                decoration: InputDecoration(
                  hintText: 'Tìm kiếm tên món, mã vạch...',
                  prefixIcon: const Icon(Icons.search, size: 20, color: AppColors.textSecondary),
                  contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  suffixIcon: _isSearchingBarcode
                      ? const SizedBox(
                          width: 16,
                          height: 16,
                          child: Center(child: CircularProgressIndicator(strokeWidth: 2)),
                        )
                      : (_barcodeInputController.text.isNotEmpty
                          ? IconButton(
                              icon: const Icon(Icons.clear, size: 16),
                              onPressed: () {
                                _barcodeInputController.clear();
                                context.read<ProductProvider>().search('', branchId: context.read<AuthProvider>().selectedBranch?.branchId);
                              },
                            )
                          : null),
                ),
                onSubmitted: (val) {
                  if (val.trim().isNotEmpty) {
                    _processBarcode(val);
                  }
                },
              ),
            ),
          ),
          const SizedBox(width: 10),

          // Quick Camera Barcode Scanner Button
          InkWell(
            onTap: _handleCameraScan,
            borderRadius: BorderRadius.circular(12),
            child: Container(
              height: 44,
              padding: const EdgeInsets.symmetric(horizontal: 14),
              decoration: BoxDecoration(
                color: AppColors.primaryBrand,
                borderRadius: BorderRadius.circular(12),
                boxShadow: AppColors.microShadow,
              ),
              child: const Row(
                children: [
                  Icon(Icons.qr_code_scanner_rounded, color: Colors.white, size: 20),
                  SizedBox(width: 6),
                  Text(
                    'Quét mã',
                    style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 13),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCategoryPills(ProductProvider productProv, int? branchId) {
    final categories = productProv.categories;
    final selectedCat = productProv.selectedCategory;

    return Container(
      height: 46,
      color: Colors.white,
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: ListView.separated(
        padding: const EdgeInsets.symmetric(horizontal: 14),
        scrollDirection: Axis.horizontal,
        itemCount: categories.length + 1,
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (context, index) {
          if (index == 0) {
            final isSelected = selectedCat == null;
            return ChoiceChip(
              label: const Text('Tất cả'),
              selected: isSelected,
              selectedColor: AppColors.primaryBrand,
              labelStyle: TextStyle(
                fontSize: 12,
                fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                color: isSelected ? Colors.white : AppColors.textPrimary,
              ),
              backgroundColor: AppColors.surfaceVariant.withValues(alpha: 0.5),
              onSelected: (_) => productProv.selectCategory(null, branchId: branchId),
            );
          }

          final cat = categories[index - 1];
          final isSelected = selectedCat?.categoryId == cat.categoryId;

          return ChoiceChip(
            label: Text(cat.categoryName),
            selected: isSelected,
            selectedColor: AppColors.primaryBrand,
            labelStyle: TextStyle(
              fontSize: 12,
              fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
              color: isSelected ? Colors.white : AppColors.textPrimary,
            ),
            backgroundColor: AppColors.surfaceVariant.withValues(alpha: 0.5),
            onSelected: (_) => productProv.selectCategory(cat, branchId: branchId),
          );
        },
      ),
    );
  }

  Widget _buildProductGrid(ProductProvider productProv, PosCartProvider cart) {
    if (productProv.isLoading) {
      return GridView.builder(
        padding: const EdgeInsets.all(12),
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: 2,
          childAspectRatio: 0.76,
          crossAxisSpacing: 10,
          mainAxisSpacing: 10,
        ),
        itemCount: 6,
        itemBuilder: (_, __) => const ShimmerSkeleton(width: double.infinity, height: 180, borderRadius: 14),
      );
    }

    final products = productProv.products;
    if (products.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.inventory_2_outlined, size: 48, color: AppColors.textMuted),
            const SizedBox(height: 12),
            const Text('Không có sản phẩm nào phù hợp', style: TextStyle(color: AppColors.textSecondary, fontSize: 14)),
            const SizedBox(height: 8),
            TextButton(
              onPressed: () => productProv.init(branchId: context.read<AuthProvider>().selectedBranch?.branchId),
              child: const Text('Tải lại danh sách'),
            ),
          ],
        ),
      );
    }

    return GridView.builder(
      padding: const EdgeInsets.all(12),
      addRepaintBoundaries: true,
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 2,
        childAspectRatio: 0.74,
        crossAxisSpacing: 10,
        mainAxisSpacing: 10,
      ),
      itemCount: products.length,
      itemBuilder: (context, index) {
        final p = products[index];
        final stock = p.stockQuantity;
        final isOutOfStock = stock <= 0;

        return FlatCard(
          padding: EdgeInsets.zero,
          onTap: () {
            HapticFeedback.lightImpact();
            cart.addToCart(p);
            ScaffoldMessenger.of(context).hideCurrentSnackBar();
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('Đã thêm "${p.name}" vào giỏ'),
                duration: const Duration(milliseconds: 700),
                behavior: SnackBarBehavior.floating,
              ),
            );
          },
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Product Image with Corner Stock Badge
              Expanded(
                child: Stack(
                  children: [
                    ClipRRect(
                      borderRadius: const BorderRadius.vertical(top: Radius.circular(14)),
                      child: Container(
                        width: double.infinity,
                        color: AppColors.surfaceVariant.withValues(alpha: 0.4),
                        child: p.fullImageUrl != null
                            ? CachedNetworkImage(
                                imageUrl: p.fullImageUrl!,
                                fit: BoxFit.cover,
                                memCacheWidth: 180,
                                memCacheHeight: 180,
                                errorWidget: (_, __, ___) => const Center(
                                  child: Icon(Icons.image_not_supported_outlined, color: AppColors.textMuted),
                                ),
                              )
                            : const Center(
                                child: Icon(Icons.inventory_2_outlined, color: AppColors.textMuted, size: 36),
                              ),
                      ),
                    ),

                    // Stock Quantity Badge
                    Positioned(
                      top: 6,
                      left: 6,
                      child: Container(
                        padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                        decoration: BoxDecoration(
                          color: isOutOfStock
                              ? AppColors.error
                              : (stock <= 5 ? AppColors.warning : AppColors.emerald),
                          borderRadius: BorderRadius.circular(6),
                        ),
                        child: Text(
                          isOutOfStock ? 'Hết hàng' : 'Tồn: $stock',
                          style: const TextStyle(color: Colors.white, fontSize: 10, fontWeight: FontWeight.bold),
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              // Product Info & Price
              Padding(
                padding: const EdgeInsets.all(10),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      p.name,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(fontSize: 12.5, fontWeight: FontWeight.w600, height: 1.2),
                    ),
                    const SizedBox(height: 6),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Expanded(
                          child: VndText(
                            p.price,
                            style: const TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.w800,
                              color: AppColors.primaryBrand,
                            ),
                          ),
                        ),
                        Container(
                          width: 28,
                          height: 28,
                          decoration: BoxDecoration(
                            color: AppColors.primaryBrand.withValues(alpha: 0.1),
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: const Icon(Icons.add, size: 18, color: AppColors.primaryBrand),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildFloatingCartDock(PosCartProvider cart) {
    if (cart.isEmpty) {
      return Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
        color: Colors.white,
        child: const SafeArea(
          top: false,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(Icons.touch_app_outlined, size: 16, color: AppColors.textMuted),
              SizedBox(width: 6),
              Text(
                'Chạm vào sản phẩm hoặc quét mã vạch để thêm vào giỏ',
                style: TextStyle(color: AppColors.textSecondary, fontSize: 12),
              ),
            ],
          ),
        ),
      );
    }

    return Container(
      padding: const EdgeInsets.fromLTRB(16, 10, 16, 12),
      decoration: BoxDecoration(
        color: AppColors.primary,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.15),
            blurRadius: 16,
            offset: const Offset(0, -4),
          ),
        ],
      ),
      child: SafeArea(
        top: false,
        child: Row(
          children: [
            // Left Cart Icon + Item count + Total Price
            Expanded(
              child: InkWell(
                onTap: _openCartSheet,
                child: Row(
                  children: [
                    Stack(
                      clipBehavior: Clip.none,
                      children: [
                        const Icon(Icons.shopping_bag_outlined, color: Colors.white, size: 26),
                        Positioned(
                          top: -4,
                          right: -6,
                          child: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 5, vertical: 1),
                            decoration: const BoxDecoration(
                              color: AppColors.error,
                              shape: BoxShape.circle,
                            ),
                            child: Text(
                              '${cart.itemCount}',
                              style: const TextStyle(color: Colors.white, fontSize: 10, fontWeight: FontWeight.bold),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(width: 14),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Text(
                            'Tổng giỏ hàng',
                            style: TextStyle(color: Colors.white70, fontSize: 11),
                          ),
                          VndText(
                            cart.finalTotal,
                            style: const TextStyle(
                              color: AppColors.emerald,
                              fontSize: 17,
                              fontWeight: FontWeight.w800,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),

            // Two Action Buttons
            OutlinedButton(
              style: OutlinedButton.styleFrom(
                foregroundColor: Colors.white,
                side: const BorderSide(color: Colors.white30),
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
              ),
              onPressed: _openCartSheet,
              child: const Text('Xem giỏ', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            ),
            const SizedBox(width: 8),
            ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.emerald,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                elevation: 0,
              ),
              onPressed: _openPaymentSheet,
              child: const Text('Thanh toán', style: TextStyle(fontSize: 13, fontWeight: FontWeight.bold)),
            ),
          ],
        ),
      ),
    );
  }
}

/// Modal chọn khách hàng hội viên tích điểm
class _CustomerSelectorModal extends StatefulWidget {
  const _CustomerSelectorModal();

  @override
  State<_CustomerSelectorModal> createState() => _CustomerSelectorModalState();
}

class _CustomerSelectorModalState extends State<_CustomerSelectorModal> {
  final _searchCtrl = TextEditingController();
  List<CustomerModel> _customers = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _fetchCustomers();
  }

  Future<void> _fetchCustomers([String? query]) async {
    setState(() => _isLoading = true);
    try {
      final list = await DataRepository().getCustomers(keyword: query);
      if (mounted) {
        setState(() {
          _customers = list;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final cart = context.read<PosCartProvider>();

    return Container(
      height: MediaQuery.of(context).size.height * 0.7,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      child: Column(
        children: [
          Container(
            padding: const EdgeInsets.all(16),
            decoration: const BoxDecoration(
              border: Border(bottom: BorderSide(color: AppColors.border)),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text('Chọn Khách hàng / Hội viên', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
                IconButton(icon: const Icon(Icons.close), onPressed: () => Navigator.pop(context)),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(12),
            child: TextField(
              controller: _searchCtrl,
              decoration: const InputDecoration(
                hintText: 'Tìm theo tên, SĐT khách hàng...',
                prefixIcon: Icon(Icons.search),
                contentPadding: EdgeInsets.symmetric(horizontal: 14, vertical: 10),
              ),
              onChanged: (val) => _fetchCustomers(val),
            ),
          ),
          ListTile(
            leading: const CircleAvatar(backgroundColor: AppColors.surfaceVariant, child: Icon(Icons.person_off_outlined)),
            title: const Text('Khách vãng lai (Không tích điểm)'),
            onTap: () {
              cart.selectCustomer(null);
              Navigator.pop(context);
            },
          ),
          const Divider(height: 1),
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : ListView.builder(
                    itemCount: _customers.length,
                    itemBuilder: (context, index) {
                      final c = _customers[index];
                      return ListTile(
                        leading: CircleAvatar(
                          backgroundColor: c.tierColor.withValues(alpha: 0.15),
                          child: Icon(Icons.stars, color: c.tierColor, size: 20),
                        ),
                        title: Text(c.fullName, style: const TextStyle(fontWeight: FontWeight.bold)),
                        subtitle: Text('${c.phone ?? "Không có SĐT"} • Điểm: ${c.pointBalance} • Hạng: ${c.tierName}'),
                        onTap: () {
                          cart.selectCustomer(c);
                          Navigator.pop(context);
                        },
                      );
                    },
                  ),
          ),
        ],
      ),
    );
  }
}
