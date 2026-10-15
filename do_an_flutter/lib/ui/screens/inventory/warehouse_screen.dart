import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../core/utils/image_helper.dart';
import '../../../data/models/inventory_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';
import '../../widgets/branch_selector_widget.dart';
import '../../widgets/flat_card.dart';
import '../../widgets/status_badge.dart';
import '../../widgets/vnd_text.dart';
import '../purchases/purchase_orders_screen.dart';
import 'stocktaking_screen.dart';

class WarehouseScreen extends StatefulWidget {
  const WarehouseScreen({super.key});

  @override
  State<WarehouseScreen> createState() => _WarehouseScreenState();
}

class _WarehouseScreenState extends State<WarehouseScreen> with SingleTickerProviderStateMixin {
  final DataRepository _repository = DataRepository();
  late TabController _tabController;
  int? _lastBranchId;

  List<WarehouseModel> _warehouses = [];
  WarehouseModel? _selectedWarehouse;

  List<InventoryItemModel> _inventoryItems = [];
  List<StockMovementModel> _movements = [];

  bool _isLoadingWarehouses = true;
  bool _isLoadingItems = false;
  bool _isLoadingMovements = false;

  static final WarehouseModel allWarehousesOption = WarehouseModel(
    warehouseId: 0,
    branchId: 0,
    name: 'Tất cả kho hàng',
  );

  String _selectedMovementType = 'ALL';
  final TextEditingController _movementSearchCtrl = TextEditingController();

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _loadWarehouses();
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    final auth = context.watch<AuthProvider>();
    final currentBranchId = auth.selectedBranch?.branchId;
    if (_lastBranchId != currentBranchId) {
      _lastBranchId = currentBranchId;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) _loadWarehouses();
      });
    }
  }

  @override
  void dispose() {
    _tabController.dispose();
    _movementSearchCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadWarehouses() async {
    setState(() => _isLoadingWarehouses = true);
    final auth = context.read<AuthProvider>();
    final branchId = (auth.selectedBranch != null && auth.selectedBranch!.branchId > 0)
        ? auth.selectedBranch!.branchId
        : null;

    try {
      final whs = await _repository.getWarehousesByBranch(branchId);
      if (!mounted) return;
      setState(() {
        _warehouses = [allWarehousesOption, ...whs];
        _isLoadingWarehouses = false;
        _selectedWarehouse = _warehouses.first;
        _loadWarehouseData();
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _isLoadingWarehouses = false);
    }
  }

  Future<void> _loadWarehouseData() async {
    final auth = context.read<AuthProvider>();
    final branchId = (auth.selectedBranch != null && auth.selectedBranch!.branchId > 0)
        ? auth.selectedBranch!.branchId
        : null;
    final whId = (_selectedWarehouse != null && _selectedWarehouse!.warehouseId > 0)
        ? _selectedWarehouse!.warehouseId
        : null;

    setState(() {
      _isLoadingItems = true;
      _isLoadingMovements = true;
    });

    try {
      final items = await _repository.getInventoryItems(warehouseId: whId, branchId: branchId);
      final movs = await _repository.getStockMovements(
        warehouseId: whId,
        branchId: branchId,
        type: _selectedMovementType == 'ALL' ? null : _selectedMovementType,
        keyword: _movementSearchCtrl.text.trim().isNotEmpty ? _movementSearchCtrl.text.trim() : null,
        toDate: DateTime.now(),
      );
      if (!mounted) return;
      setState(() {
        _inventoryItems = items;
        _movements = movs;
        _isLoadingItems = false;
        _isLoadingMovements = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _isLoadingItems = false;
        _isLoadingMovements = false;
      });
    }
  }

  void _openStocktaking() async {
    if (_selectedWarehouse == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Vui lòng chọn kho hàng trước khi kiểm kê.')),
      );
      return;
    }

    final res = await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => StocktakingScreen(warehouse: _selectedWarehouse!),
      ),
    );

    if (res == true && mounted) {
      _loadWarehouseData();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Quản lý Kho & Kiểm kê', style: TextStyle(fontSize: 17, fontWeight: FontWeight.bold)),
        actions: [
          IconButton(
            tooltip: 'Nhập hàng từ NCC (Inbound)',
            icon: const Icon(Icons.add_shopping_cart_rounded, color: AppColors.primary),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const PurchaseOrdersScreen()),
              ).then((_) {
                if (mounted) _loadWarehouseData();
              });
            },
          ),
          const Padding(
            padding: EdgeInsets.only(right: 12),
            child: Center(child: BranchSelectorWidget(compact: true)),
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          labelColor: AppColors.primary,
          unselectedLabelColor: AppColors.textSecondary,
          indicatorColor: AppColors.primary,
          indicatorWeight: 3,
          tabs: const [
            Tab(text: 'Tồn kho thực tế', icon: Icon(Icons.storage_outlined, size: 20)),
            Tab(text: 'Biến động kho', icon: Icon(Icons.history_outlined, size: 20)),
          ],
        ),
      ),
      body: Column(
        children: [
          // Warehouse Selector Header
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
            color: Colors.white,
            child: Row(
              children: [
                const Icon(Icons.warehouse_rounded, color: AppColors.primary, size: 22),
                const SizedBox(width: 10),
                Expanded(
                  child: _isLoadingWarehouses
                      ? const Text('Đang tải danh sách kho...', style: TextStyle(fontSize: 13, color: AppColors.textMuted))
                      : _warehouses.isEmpty
                          ? const Text('Chi nhánh chưa có kho riêng', style: TextStyle(fontSize: 13, color: AppColors.textMuted))
                          : DropdownButtonHideUnderline(
                              child: DropdownButton<WarehouseModel>(
                                isExpanded: true,
                                value: _selectedWarehouse,
                                items: _warehouses.map((wh) {
                                  return DropdownMenuItem<WarehouseModel>(
                                    value: wh,
                                    child: Text(
                                      wh.name,
                                      style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
                                    ),
                                  );
                                }).toList(),
                                onChanged: (val) {
                                  if (val != null) {
                                    setState(() => _selectedWarehouse = val);
                                    _loadWarehouseData();
                                  }
                                },
                              ),
                            ),
                ),
                IconButton(
                  icon: const Icon(Icons.refresh, size: 20),
                  tooltip: 'Tải lại',
                  onPressed: _loadWarehouseData,
                ),
              ],
            ),
          ),
          const Divider(height: 1, color: AppColors.border),

          // Tabs Views
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                _buildInventoryTab(),
                _buildMovementsTab(),
              ],
            ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
        icon: const Icon(Icons.qr_code_scanner, size: 20),
        label: const Text('Kiểm kê siêu tốc', style: TextStyle(fontWeight: FontWeight.bold)),
        onPressed: _openStocktaking,
      ),
    );
  }

  Widget _buildInventoryTab() {
    if (_isLoadingItems) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_inventoryItems.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.inventory_2_outlined, size: 48, color: AppColors.textMuted),
            const SizedBox(height: 12),
            const Text('Kho này chưa có dữ liệu tồn kho', style: TextStyle(color: AppColors.textSecondary)),
            const SizedBox(height: 8),
            ElevatedButton(
              onPressed: _loadWarehouseData,
              child: const Text('Làm mới'),
            ),
          ],
        ),
      );
    }

    final totalQty = _inventoryItems.fold<int>(0, (sum, i) => sum + i.quantity);
    final totalValuation = _inventoryItems.fold<double>(0.0, (sum, i) => sum + (i.quantity * (i.price ?? 0)));

    return Column(
      children: [
        // Inventory KPI Summary Strip
        Container(
          margin: const EdgeInsets.fromLTRB(16, 12, 16, 4),
          padding: const EdgeInsets.all(14),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: AppColors.border),
          ),
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text('Tổng mặt hàng', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                    const SizedBox(height: 2),
                    Text(
                      '${_inventoryItems.length}',
                      style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                    ),
                  ],
                ),
              ),
              Container(width: 1, height: 28, color: AppColors.border),
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.only(left: 12),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('Tổng lượng tồn', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                      const SizedBox(height: 2),
                      Text(
                        '$totalQty',
                        style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppColors.primaryBrand),
                      ),
                    ],
                  ),
                ),
              ),
              Container(width: 1, height: 28, color: AppColors.border),
              Expanded(
                flex: 2,
                child: Padding(
                  padding: const EdgeInsets.only(left: 12),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('Tổng giá trị tồn', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                      const SizedBox(height: 2),
                      VndText(
                        totalValuation,
                        style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppColors.emerald),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),

        // Items List
        Expanded(
          child: ListView.separated(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 80),
            itemCount: _inventoryItems.length,
            separatorBuilder: (_, _) => const SizedBox(height: 8),
            itemBuilder: (context, index) {
              final item = _inventoryItems[index];
              final itemValue = item.quantity * (item.price ?? 0);
              final hasImage = item.productImage != null &&
                  item.productImage!.trim().isNotEmpty &&
                  ImageHelper.getFullImageUrl(item.productImage) != null;

              return FlatCard(
                padding: const EdgeInsets.all(12),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    ClipRRect(
                      borderRadius: BorderRadius.circular(10),
                      child: Container(
                        width: 48,
                        height: 48,
                        color: item.quantity <= 0
                            ? AppColors.error.withValues(alpha: 0.08)
                            : item.quantity <= 5
                                ? AppColors.warning.withValues(alpha: 0.08)
                                : AppColors.primaryBrand.withValues(alpha: 0.08),
                        child: hasImage
                            ? CachedNetworkImage(
                                imageUrl: ImageHelper.getFullImageUrl(item.productImage)!,
                                memCacheWidth: 96,
                                memCacheHeight: 96,
                                fit: BoxFit.cover,
                                errorWidget: (_, __, ___) => Icon(
                                  Icons.inventory_2_rounded,
                                  color: item.quantity <= 0
                                      ? AppColors.error
                                      : item.quantity <= 5
                                          ? AppColors.warning
                                          : AppColors.primaryBrand,
                                  size: 22,
                                ),
                              )
                            : Icon(
                                Icons.inventory_2_rounded,
                                color: item.quantity <= 0
                                    ? AppColors.error
                                    : item.quantity <= 5
                                        ? AppColors.warning
                                        : AppColors.primaryBrand,
                                size: 22,
                              ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            item.productName,
                            style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                          ),
                          const SizedBox(height: 3),
                          Wrap(
                            spacing: 8,
                            runSpacing: 2,
                            crossAxisAlignment: WrapCrossAlignment.center,
                            children: [
                              if (item.barcode != null && item.barcode!.isNotEmpty)
                                Text(
                                  'SKU: ${item.barcode}',
                                  style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                                ),
                              if (item.batchCode != null && item.batchCode!.isNotEmpty)
                                Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 5, vertical: 1),
                                  decoration: BoxDecoration(
                                    color: AppColors.surfaceVariant,
                                    borderRadius: BorderRadius.circular(4),
                                  ),
                                  child: Text(
                                    'Lô: ${item.batchCode}',
                                    style: const TextStyle(fontSize: 10, color: AppColors.textMuted),
                                  ),
                                ),
                              if (item.price != null && item.price! > 0)
                                Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    VndText(
                                      item.price!,
                                      style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w500, color: AppColors.textSecondary),
                                    ),
                                    if (item.unit != null && item.unit!.isNotEmpty)
                                      Text(
                                        ' / ${item.unit}',
                                        style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                                      ),
                                  ],
                                ),
                            ],
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(width: 8),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Text(
                              '${item.quantity}',
                              style: TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                                color: item.quantity <= 0
                                    ? AppColors.error
                                    : item.quantity <= 5
                                        ? AppColors.warning
                                        : AppColors.textPrimary,
                              ),
                            ),
                            if (item.unit != null && item.unit!.isNotEmpty) ...[
                              const SizedBox(width: 2),
                              Text(
                                item.unit!,
                                style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                              ),
                            ],
                            const SizedBox(width: 6),
                            if (item.quantity <= 0)
                              const StatusBadge(label: 'Hết', color: AppColors.error)
                            else if (item.quantity <= 5)
                              const StatusBadge(label: 'Sắp hết', color: AppColors.warning)
                            else
                              const StatusBadge(label: 'Có sẵn', color: AppColors.emerald),
                          ],
                        ),
                        if (itemValue > 0) ...[
                          const SizedBox(height: 3),
                          VndText(
                            itemValue,
                            style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w600, color: AppColors.emerald),
                          ),
                        ],
                      ],
                    ),
                  ],
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildMovementsTab() {
    return Column(
      children: [
        // Filter bar & Search
        Container(
          padding: const EdgeInsets.fromLTRB(16, 10, 16, 8),
          color: Colors.white,
          child: Column(
            children: [
              SizedBox(
                height: 40,
                child: TextField(
                  controller: _movementSearchCtrl,
                  decoration: InputDecoration(
                    hintText: 'Tìm theo tên sản phẩm, mã vạch...',
                    prefixIcon: const Icon(Icons.search, size: 18, color: AppColors.textMuted),
                    suffixIcon: _movementSearchCtrl.text.isNotEmpty
                        ? IconButton(
                            icon: const Icon(Icons.clear, size: 16),
                            onPressed: () {
                              _movementSearchCtrl.clear();
                              _loadWarehouseData();
                            },
                          )
                        : null,
                    contentPadding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                  ),
                  onSubmitted: (_) => _loadWarehouseData(),
                ),
              ),
              const SizedBox(height: 8),
              SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                physics: const BouncingScrollPhysics(),
                child: Row(
                  children: [
                    _buildMovementTypeChip('Tất cả', 'ALL'),
                    const SizedBox(width: 6),
                    _buildMovementTypeChip('Nhập kho', 'Import'),
                    const SizedBox(width: 6),
                    _buildMovementTypeChip('Xuất bán', 'Sale'),
                    const SizedBox(width: 6),
                    _buildMovementTypeChip('Điều chỉnh', 'Adjustment'),
                  ],
                ),
              ),
            ],
          ),
        ),
        const Divider(height: 1, color: AppColors.border),

        // Audit Timeline List
        Expanded(
          child: _isLoadingMovements
              ? const Center(child: CircularProgressIndicator())
              : _movements.isEmpty
                  ? Center(
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Icon(Icons.history_rounded, size: 48, color: AppColors.textMuted),
                          const SizedBox(height: 12),
                          const Text('Chưa có lịch sử biến động kho khớp bộ lọc', style: TextStyle(color: AppColors.textSecondary)),
                          const SizedBox(height: 12),
                          ElevatedButton.icon(
                            icon: const Icon(Icons.refresh, size: 16),
                            label: const Text('Làm mới'),
                            onPressed: _loadWarehouseData,
                          ),
                        ],
                      ),
                    )
                  : ListView.separated(
                      padding: const EdgeInsets.fromLTRB(16, 12, 16, 80),
                      itemCount: _movements.length,
                      separatorBuilder: (_, _) => const SizedBox(height: 10),
                      itemBuilder: (context, index) {
                        final m = _movements[index];
                        final typeUpper = m.movementType.toUpperCase();
                        final isPositive = m.quantity > 0 || typeUpper == 'IMPORT' || typeUpper == 'IN';
                        final isAdjustment = typeUpper == 'ADJUSTMENT' || typeUpper == 'ADJUST';

                        return FlatCard(
                          padding: const EdgeInsets.all(14),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              // Audit header: Type Badge + Movement Date
                              Row(
                                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                children: [
                                  isAdjustment
                                      ? const StatusBadge(
                                          label: 'Cân bằng kiểm kê',
                                          color: AppColors.warning,
                                          icon: Icons.tune_rounded,
                                        )
                                      : isPositive
                                          ? const StatusBadge(
                                              label: 'Nhập kho',
                                              color: AppColors.emerald,
                                              icon: Icons.arrow_downward_rounded,
                                            )
                                          : const StatusBadge(
                                              label: 'Xuất kho / Bán',
                                              color: AppColors.error,
                                              icon: Icons.arrow_upward_rounded,
                                            ),
                                  Text(
                                    Formatters.formatDateTime(m.movementDate),
                                    style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 10),

                              // Item details + Delta Quantity
                              Row(
                                crossAxisAlignment: CrossAxisAlignment.center,
                                children: [
                                  ClipRRect(
                                    borderRadius: BorderRadius.circular(8),
                                    child: Container(
                                      width: 44,
                                      height: 44,
                                      color: AppColors.surfaceVariant,
                                      child: (m.productImage != null &&
                                              m.productImage!.trim().isNotEmpty &&
                                              ImageHelper.getFullImageUrl(m.productImage) != null)
                                          ? CachedNetworkImage(
                                              imageUrl: ImageHelper.getFullImageUrl(m.productImage)!,
                                              memCacheWidth: 88,
                                              memCacheHeight: 88,
                                              fit: BoxFit.cover,
                                              errorWidget: (_, __, ___) => Icon(
                                                isPositive ? Icons.arrow_downward_rounded : Icons.arrow_upward_rounded,
                                                size: 20,
                                                color: isPositive ? AppColors.emerald : AppColors.error,
                                              ),
                                            )
                                          : Icon(
                                              isPositive ? Icons.arrow_downward_rounded : Icons.arrow_upward_rounded,
                                              size: 20,
                                              color: isPositive ? AppColors.emerald : AppColors.error,
                                            ),
                                    ),
                                  ),
                                  const SizedBox(width: 10),
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          m.productName,
                                          style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                                          maxLines: 1,
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                        const SizedBox(height: 2),
                                        Text(
                                          m.warehouseName ?? 'Kho chính',
                                          style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                                        ),
                                      ],
                                    ),
                                  ),
                                  Text(
                                    isPositive ? '+${m.quantity}' : '${m.quantity}',
                                    style: TextStyle(
                                      fontSize: 18,
                                      fontWeight: FontWeight.bold,
                                      color: isPositive ? AppColors.emerald : AppColors.error,
                                    ),
                                  ),
                                ],
                              ),

                              // Footer: Performer & Reason
                              if ((m.userName != null && m.userName!.isNotEmpty) ||
                                  (m.reason != null && m.reason!.isNotEmpty)) ...[
                                const SizedBox(height: 8),
                                const Divider(height: 1, color: AppColors.border),
                                const SizedBox(height: 8),
                                Row(
                                  children: [
                                    if (m.userName != null && m.userName!.isNotEmpty) ...[
                                      const Icon(Icons.person_outline, size: 13, color: AppColors.textMuted),
                                      const SizedBox(width: 4),
                                      Text(
                                        m.userName!,
                                        style: const TextStyle(fontSize: 11, color: AppColors.textSecondary),
                                      ),
                                      const Spacer(),
                                    ],
                                    if (m.reason != null && m.reason!.isNotEmpty)
                                      Expanded(
                                        child: Text(
                                          'Lý do: ${m.reason}',
                                          style: const TextStyle(
                                            fontSize: 11,
                                            fontStyle: FontStyle.italic,
                                            color: AppColors.textMuted,
                                          ),
                                          textAlign: TextAlign.end,
                                          maxLines: 1,
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                      ),
                                  ],
                                ),
                              ],
                            ],
                          ),
                        );
                      },
                    ),
        ),
      ],
    );
  }

  Widget _buildMovementTypeChip(String label, String value) {
    final isSelected = _selectedMovementType == value;
    return ChoiceChip(
      label: Text(label),
      selected: isSelected,
      selectedColor: AppColors.primaryBrand.withValues(alpha: 0.15),
      labelStyle: TextStyle(
        fontSize: 12,
        fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
        color: isSelected ? AppColors.primaryBrand : AppColors.textSecondary,
      ),
      onSelected: (_) {
        setState(() => _selectedMovementType = value);
        _loadWarehouseData();
      },
    );
  }
}
