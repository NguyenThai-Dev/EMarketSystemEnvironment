import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/product_model.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/product_provider.dart';
import '../../widgets/branch_selector_widget.dart';
import '../../widgets/permission_guard.dart';
import 'add_edit_product_dialog.dart';
import 'product_detail_screen.dart';

class ProductsScreen extends StatefulWidget {
  const ProductsScreen({super.key});

  @override
  State<ProductsScreen> createState() => _ProductsScreenState();
}

class _ProductsScreenState extends State<ProductsScreen> {
  final _searchController = TextEditingController();
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
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final productProv = context.watch<ProductProvider>();

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Danh mục Sản phẩm', style: TextStyle(fontSize: 17, fontWeight: FontWeight.bold)),
        actions: [
          const Center(child: BranchSelectorWidget(compact: true)),
          IconButton(
            icon: const Icon(Icons.qr_code_scanner_rounded, color: AppColors.primary),
            tooltip: 'Quét Barcode tìm hàng',
            onPressed: () {
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Kích hoạt camera quét Barcode tìm kiếm sản phẩm...')),
              );
            },
          ),
        ],
      ),
      body: Column(
        children: [
          // Search Field
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
            color: Colors.white,
            child: TextField(
              controller: _searchController,
              decoration: InputDecoration(
                hintText: 'Tìm kiếm theo tên sản phẩm, mã vạch...',
                prefixIcon: const Icon(Icons.search, size: 20, color: AppColors.textMuted),
                suffixIcon: _searchController.text.isNotEmpty
                    ? IconButton(
                        icon: const Icon(Icons.clear, size: 18),
                        onPressed: () {
                          _searchController.clear();
                          productProv.search('', branchId: auth.selectedBranch?.branchId);
                        },
                      )
                    : null,
                contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
              ),
              onChanged: (val) {
                productProv.search(val, branchId: auth.selectedBranch?.branchId);
              },
            ),
          ),

          // Categories Horizontal List
          if (productProv.categories.isNotEmpty)
            Container(
              height: 46,
              color: Colors.white,
              child: ListView.separated(
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
                scrollDirection: Axis.horizontal,
                itemCount: productProv.categories.length + 1,
                separatorBuilder: (context, i) => const SizedBox(width: 8),
                itemBuilder: (context, index) {
                  if (index == 0) {
                    final isSelected = productProv.selectedCategory == null;
                    return FilterChip(
                      label: const Text('Tất cả'),
                      selected: isSelected,
                      selectedColor: AppColors.primary.withValues(alpha: 0.12),
                      checkmarkColor: AppColors.primary,
                      labelStyle: TextStyle(
                        fontSize: 12,
                        fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                        color: isSelected ? AppColors.primary : AppColors.textSecondary,
                      ),
                      onSelected: (_) => productProv.selectCategory(null, branchId: auth.selectedBranch?.branchId),
                    );
                  }
                  final cat = productProv.categories[index - 1];
                  final isSelected = productProv.selectedCategory?.categoryId == cat.categoryId;
                  return FilterChip(
                    label: Text(cat.categoryName),
                    selected: isSelected,
                    selectedColor: AppColors.primary.withValues(alpha: 0.12),
                    checkmarkColor: AppColors.primary,
                    labelStyle: TextStyle(
                      fontSize: 12,
                      fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                      color: isSelected ? AppColors.primary : AppColors.textSecondary,
                    ),
                    onSelected: (_) => productProv.selectCategory(cat, branchId: auth.selectedBranch?.branchId),
                  );
                },
              ),
            ),

          const Divider(height: 1, color: AppColors.border),

          // Product List
          Expanded(
            child: RefreshIndicator(
              onRefresh: () => productProv.loadProducts(branchId: auth.selectedBranch?.branchId),
              child: productProv.isLoading
                  ? const Center(child: CircularProgressIndicator())
                  : productProv.products.isEmpty
                      ? Center(
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Icon(Icons.inventory_2_outlined, size: 56, color: AppColors.textMuted.withValues(alpha: 0.5)),
                              const SizedBox(height: 12),
                              const Text('Chưa có sản phẩm nào phù hợp', style: TextStyle(color: AppColors.textSecondary)),
                            ],
                          ),
                        )
                      : ListView.separated(
                          padding: const EdgeInsets.all(16),
                          itemCount: productProv.products.length,
                          separatorBuilder: (context, i) => const SizedBox(height: 12),
                          itemBuilder: (context, index) {
                            final product = productProv.products[index];
                            return _buildProductCard(product);
                          },
                        ),
            ),
          ),
        ],
      ),
      floatingActionButton: PermissionGuard(
        child: FloatingActionButton.extended(
          backgroundColor: AppColors.primary,
          foregroundColor: Colors.white,
          icon: const Icon(Icons.add, size: 20),
          label: const Text('Thêm sản phẩm', style: TextStyle(fontWeight: FontWeight.bold)),
          onPressed: () async {
            final auth = context.read<AuthProvider>();
            final prodProv = context.read<ProductProvider>();
            final res = await AddEditProductDialog.show(context);
            if (res == true && mounted) {
              prodProv.init(branchId: auth.selectedBranch?.branchId);
            }
          },
        ),
      ),
    );
  }

  Widget _buildProductCard(ProductModel product) {
    return Material(
      color: Colors.white,
      borderRadius: BorderRadius.circular(14),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: () async {
          final auth = context.read<AuthProvider>();
          final prodProv = context.read<ProductProvider>();
          final res = await Navigator.push(
            context,
            MaterialPageRoute(
              builder: (_) => ProductDetailScreen(product: product),
            ),
          );
          if (res == true && mounted) {
            prodProv.init(branchId: auth.selectedBranch?.branchId);
          }
        },
        child: Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: AppColors.border),
          ),
          child: Row(
        children: [
          // Product Thumbnail
          ClipRRect(
            borderRadius: BorderRadius.circular(10),
            child: SizedBox(
              width: 64,
              height: 64,
              child: product.fullImageUrl != null
                  ? CachedNetworkImage(
                      imageUrl: product.fullImageUrl!,
                      fit: BoxFit.cover,
                      placeholder: (context, url) => Container(color: AppColors.surfaceVariant),
                      errorWidget: (context, url, err) => _buildFallbackThumbnail(),
                    )
                  : _buildFallbackThumbnail(),
            ),
          ),
          const SizedBox(width: 12),

          // Product Details
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  product.name,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600, color: AppColors.textPrimary),
                ),
                const SizedBox(height: 4),
                if (product.sku.isNotEmpty)
                  Text(
                    'Mã: ${product.sku}',
                    style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                  ),
                const SizedBox(height: 6),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      Formatters.formatVND(product.price),
                      style: const TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: AppColors.primaryLight,
                      ),
                    ),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: product.stockQuantity > 5
                            ? AppColors.success.withValues(alpha: 0.1)
                            : AppColors.warning.withValues(alpha: 0.12),
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        'Tồn: ${product.stockQuantity}',
                        style: TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.w600,
                          color: product.stockQuantity > 5 ? AppColors.success : AppColors.warning,
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    ),
  ),
);
  }

  Widget _buildFallbackThumbnail() {
    return Container(
      color: AppColors.surfaceVariant,
      child: const Icon(Icons.image_not_supported_outlined, color: AppColors.textMuted, size: 24),
    );
  }
}
