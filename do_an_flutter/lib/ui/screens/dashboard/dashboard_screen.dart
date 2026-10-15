import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/formatters.dart';
import '../../../data/models/dashboard_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';
import '../../../providers/dashboard_provider.dart';
import '../../widgets/branch_selector_widget.dart';
import '../../widgets/flat_card.dart';
import '../../widgets/server_settings_dialog.dart';
import '../../widgets/shimmer_skeleton.dart';
import '../ai/ai_assistant_screen.dart';
import '../customers/customers_screen.dart';
import '../expenses/expenses_screen.dart';
import '../orders/orders_history_screen.dart';
import '../pos/pos_screen.dart';
import '../purchases/purchase_orders_screen.dart';
import '../quotations/quotations_screen.dart';
import '../suppliers/suppliers_screen.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  final _repository = DataRepository();
  bool _hideAmount = false;
  String _selectedPeriod = 'today'; // 'today', 'week', 'month'
  int? _lastBranchId;

  List<BranchPerformanceModel> _branchPerformances = [];
  List<ChartItemModel> _stockChartItems = [];
  bool _isLoadingCharts = true;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadAllData();
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
        if (mounted) _loadAllData();
      });
    }
  }

  void _loadAllData() {
    final auth = context.read<AuthProvider>();
    final branchId = auth.selectedBranch?.branchId;

    DateTime? fromDate;
    final toDate = DateTime.now();

    if (_selectedPeriod == 'today') {
      fromDate = DateTime(toDate.year, toDate.month, toDate.day);
    } else if (_selectedPeriod == 'week') {
      fromDate = toDate.subtract(const Duration(days: 7));
    } else if (_selectedPeriod == 'month') {
      fromDate = DateTime(toDate.year, toDate.month, 1);
    }

    context.read<DashboardProvider>().loadOverview(
      branchId: branchId,
      fromDate: fromDate,
      toDate: toDate,
      groupBy: _selectedPeriod == 'month' ? 'month' : 'day',
    );
    _loadChartData();
  }

  Future<void> _loadChartData() async {
    setState(() => _isLoadingCharts = true);
    final auth = context.read<AuthProvider>();
    final branchId = auth.selectedBranch?.branchId;

    DateTime? fromDate;
    final toDate = DateTime.now();

    if (_selectedPeriod == 'today') {
      fromDate = DateTime(toDate.year, toDate.month, toDate.day);
    } else if (_selectedPeriod == 'week') {
      fromDate = toDate.subtract(const Duration(days: 7));
    } else if (_selectedPeriod == 'month') {
      fromDate = DateTime(toDate.year, toDate.month, 1);
    }

    try {
      final results = await Future.wait([
        _repository.getBranchPerformance(branchId: branchId, fromDate: fromDate, toDate: toDate),
        _repository.getStockChart(branchId: branchId),
      ]);

      if (mounted) {
        setState(() {
          _branchPerformances = results[0] as List<BranchPerformanceModel>;
          _stockChartItems = results[1] as List<ChartItemModel>;
          _isLoadingCharts = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoadingCharts = false);
    }
  }

  Future<void> _refresh() async {
    _loadAllData();
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final dashboard = context.watch<DashboardProvider>();
    final overview = dashboard.overview;
    final isLoading = dashboard.isLoading;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Text(
                  'Xin chào, ${auth.currentUser?.fullName ?? "Quản lý"}',
                  style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, letterSpacing: -0.2),
                ),
                const SizedBox(width: 6),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                  decoration: BoxDecoration(
                    color: AppColors.emerald.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(6),
                  ),
                  child: const Row(
                    children: [
                      CircleAvatar(radius: 3, backgroundColor: AppColors.emerald),
                      SizedBox(width: 4),
                      Text('Online', style: TextStyle(color: AppColors.emerald, fontSize: 10, fontWeight: FontWeight.bold)),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 3),
            const BranchSelectorWidget(compact: true),
          ],
        ),
        actions: [
          IconButton(
            icon: Icon(
              _hideAmount ? Icons.visibility_off_outlined : Icons.visibility_outlined,
              color: AppColors.textSecondary,
            ),
            tooltip: _hideAmount ? 'Hiện số tiền' : 'Ẩn số tiền',
            onPressed: () => setState(() => _hideAmount = !_hideAmount),
          ),
          IconButton(
            icon: const Icon(Icons.dns_outlined, color: AppColors.textSecondary),
            tooltip: 'Cài đặt máy chủ',
            onPressed: () => ServerSettingsDialog.show(context),
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: _refresh,
        child: SingleChildScrollView(
          physics: const AlwaysScrollableScrollPhysics(),
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Bộ lọc thời gian nhanh
              Row(
                children: [
                  _buildPeriodChip('Hôm nay', 'today'),
                  const SizedBox(width: 8),
                  _buildPeriodChip('7 ngày qua', 'week'),
                  const SizedBox(width: 8),
                  _buildPeriodChip('Tháng này', 'month'),
                  const Spacer(),
                  if (isLoading)
                    const Row(
                      children: [
                        SizedBox(width: 12, height: 12, child: CircularProgressIndicator(strokeWidth: 2)),
                        SizedBox(width: 6),
                        Text('Đang nạp...', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                      ],
                    ),
                ],
              ),
              const SizedBox(height: 14),

              // Hero Revenue Card (có Shimmer Skeleton khi Loading)
              isLoading ? _buildHeroRevenueSkeleton() : _buildHeroRevenueCard(overview),
              const SizedBox(height: 16),

              // KPI Grid Cards (2 ô chính: Khách hàng & Cảnh báo tồn)
              isLoading ? _buildKpiGridSkeleton() : _buildKpiGridCards(overview),
              const SizedBox(height: 20),

              // Quick Action Hub (Trung tâm thao tác nhanh - 3 Giây)
              _buildQuickActionHub(),
              const SizedBox(height: 20),

              // Biểu đồ xu hướng bán hàng (Trend Chart)
              isLoading ? _buildChartSkeleton('Xu hướng doanh thu bán hàng') : _buildSalesTrendSection(overview),
              const SizedBox(height: 20),

              // Biểu đồ tròn cơ cấu danh mục tồn kho (Category / Stock Distribution)
              _isLoadingCharts ? _buildChartSkeleton('Cơ cấu Ngành hàng & Tồn kho') : _buildStockDistributionSection(),
              const SizedBox(height: 20),

              // Hiệu quả hoạt động chi nhánh (Branch Performance)
              _buildBranchPerformanceSection(),
              const SizedBox(height: 24),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildPeriodChip(String label, String value) {
    final isSelected = _selectedPeriod == value;
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
        setState(() => _selectedPeriod = value);
        _loadAllData();
      },
    );
  }

  Widget _buildHeroRevenueCard(DashboardOverview overview) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(22),
      decoration: BoxDecoration(
        gradient: AppColors.heroGradient,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            color: const Color(0xFF0F172A).withValues(alpha: 0.25),
            blurRadius: 20,
            offset: const Offset(0, 8),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Row(
                children: [
                  Icon(Icons.auto_graph_rounded, color: Colors.white70, size: 16),
                  SizedBox(width: 6),
                  Text(
                    'DOANH THU THỰC TẾ',
                    style: TextStyle(
                      color: Colors.white70,
                      fontSize: 12,
                      fontWeight: FontWeight.w700,
                      letterSpacing: 0.8,
                    ),
                  ),
                ],
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                decoration: BoxDecoration(
                  color: Colors.white.withValues(alpha: 0.15),
                  borderRadius: BorderRadius.circular(20),
                  border: Border.all(color: Colors.white.withValues(alpha: 0.2)),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.receipt_long, size: 13, color: Colors.white),
                    const SizedBox(width: 5),
                    Text(
                      '${overview.todayOrders} đơn hàng',
                      style: const TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.bold),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),
          Text(
            _hideAmount ? '****** ₫' : Formatters.formatVND(overview.todayRevenue),
            style: const TextStyle(
              color: Colors.white,
              fontSize: 32,
              fontWeight: FontWeight.w800,
              letterSpacing: -0.5,
            ),
          ),
          const SizedBox(height: 12),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            decoration: BoxDecoration(
              color: Colors.black.withValues(alpha: 0.2),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Icon(Icons.trending_up, size: 16, color: AppColors.emerald),
                const SizedBox(width: 6),
                Text(
                  'Lợi nhuận ước tính: ${_hideAmount ? "*** ₫" : Formatters.formatVND(overview.estimatedProfit)}',
                  style: const TextStyle(color: Colors.white, fontSize: 13, fontWeight: FontWeight.w600),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildHeroRevenueSkeleton() {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(22),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: AppColors.border),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              ShimmerSkeleton(width: 140, height: 16),
              ShimmerSkeleton(width: 90, height: 26, borderRadius: 20),
            ],
          ),
          SizedBox(height: 18),
          ShimmerSkeleton(width: 220, height: 36),
          SizedBox(height: 14),
          ShimmerSkeleton(width: 260, height: 24, borderRadius: 8),
        ],
      ),
    );
  }

  Widget _buildKpiGridCards(DashboardOverview overview) {
    return Row(
      children: [
        Expanded(
          child: _buildKpiCard(
            title: 'Sản phẩm kinh doanh',
            value: '${overview.totalProducts > 0 ? overview.totalProducts : overview.totalCustomers}',
            unit: 'mặt hàng',
            icon: Icons.inventory_2_outlined,
            iconColor: AppColors.primaryBrand,
          ),
        ),
        const SizedBox(width: 12),
        Expanded(
          child: _buildKpiCard(
            title: 'Cảnh báo sắp hết',
            value: '${overview.lowStockProducts}',
            unit: 'cần nhập',
            icon: Icons.warning_amber_rounded,
            iconColor: overview.lowStockProducts > 0 ? AppColors.warning : AppColors.emerald,
            isAlert: overview.lowStockProducts > 0,
          ),
        ),
      ],
    );
  }

  Widget _buildKpiGridSkeleton() {
    return const Row(
      children: [
        Expanded(
          child: ShimmerSkeleton(width: double.infinity, height: 96, borderRadius: 16),
        ),
        SizedBox(width: 12),
        Expanded(
          child: ShimmerSkeleton(width: double.infinity, height: 96, borderRadius: 16),
        ),
      ],
    );
  }

  Widget _buildChartSkeleton(String title) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title, style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold)),
          const SizedBox(height: 16),
          const ShimmerSkeleton(width: double.infinity, height: 140, borderRadius: 12),
        ],
      ),
    );
  }

  Widget _buildSalesTrendSection(DashboardOverview overview) {
    final trend = overview.trend;
    final hasRealTrend = trend != null && trend.sales.isNotEmpty;

    final List<FlSpot> spots;
    final List<String> labels;

    if (hasRealTrend) {
      spots = trend.sales.asMap().entries.map((e) {
        return FlSpot(e.key.toDouble(), e.value / 1000);
      }).toList();
      labels = trend.labels;
    } else {
      final revenue = overview.todayRevenue;
      spots = [
        FlSpot(0, (revenue * 0.05) / 1000),
        FlSpot(1, (revenue * 0.15) / 1000),
        FlSpot(2, (revenue * 0.25) / 1000),
        FlSpot(3, (revenue * 0.12) / 1000),
        FlSpot(4, (revenue * 0.18) / 1000),
        FlSpot(5, (revenue * 0.35) / 1000),
        FlSpot(6, (revenue * 0.40) / 1000),
        FlSpot(7, (revenue * 0.10) / 1000),
      ];
      labels = ['8h', '10h', '12h', '14h', '16h', '18h', '20h', '22h'];
    }

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                hasRealTrend ? 'Xu hướng doanh số thực tế' : 'Nhịp độ bán hàng trong ngày',
                style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: AppColors.primaryBrand.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  hasRealTrend ? '${trend.sales.length} điểm dữ liệu' : 'Mô phỏng 60fps',
                  style: const TextStyle(fontSize: 10, color: AppColors.primaryBrand, fontWeight: FontWeight.bold),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          SizedBox(
            height: 160,
            child: LineChart(
              LineChartData(
                gridData: const FlGridData(show: false),
                titlesData: FlTitlesData(
                  topTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                  rightTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                  leftTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                  bottomTitles: AxisTitles(
                    sideTitles: SideTitles(
                      showTitles: true,
                      reservedSize: 22,
                      interval: 1,
                      getTitlesWidget: (value, meta) {
                        final idx = value.toInt();
                        if (idx >= 0 && idx < labels.length) {
                          return Text(labels[idx], style: const TextStyle(fontSize: 10, color: AppColors.textMuted));
                        }
                        return const SizedBox.shrink();
                      },
                    ),
                  ),
                ),
                borderData: FlBorderData(show: false),
                lineBarsData: [
                  LineChartBarData(
                    spots: spots,
                    isCurved: true,
                    color: AppColors.primaryBrand,
                    barWidth: 3,
                    isStrokeCapRound: true,
                    dotData: const FlDotData(show: false),
                    belowBarData: BarAreaData(
                      show: true,
                      gradient: LinearGradient(
                        colors: [
                          AppColors.primaryBrand.withValues(alpha: 0.25),
                          AppColors.primaryBrand.withValues(alpha: 0.02),
                        ],
                        begin: Alignment.topCenter,
                        end: Alignment.bottomCenter,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStockDistributionSection() {
    final colors = [
      AppColors.primaryBrand,
      const Color(0xFF673AB7),
      AppColors.warning,
      AppColors.emerald,
      AppColors.accent,
    ];

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Cơ cấu Ngành hàng & Tồn kho',
            style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          _stockChartItems.isEmpty
              ? Container(
                  height: 110,
                  alignment: Alignment.center,
                  child: const Text('Chưa có dữ liệu cơ cấu ngành hàng', style: TextStyle(color: AppColors.textMuted, fontSize: 12)),
                )
              : Row(
                  children: [
                    SizedBox(
                      width: 120,
                      height: 120,
                      child: PieChart(
                        PieChartData(
                          sectionsSpace: 2,
                          centerSpaceRadius: 28,
                          sections: _stockChartItems.take(5).toList().asMap().entries.map((entry) {
                            final idx = entry.key;
                            final item = entry.value;
                            return PieChartSectionData(
                              color: colors[idx % colors.length],
                              value: item.value.toDouble(),
                              title: '${item.value}',
                              radius: 32,
                              titleStyle: const TextStyle(fontSize: 11, fontWeight: FontWeight.bold, color: Colors.white),
                            );
                          }).toList(),
                        ),
                      ),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: _stockChartItems.take(4).toList().asMap().entries.map((entry) {
                          final idx = entry.key;
                          final item = entry.value;
                          return Padding(
                            padding: const EdgeInsets.symmetric(vertical: 3),
                            child: Row(
                              children: [
                                Container(
                                  width: 10,
                                  height: 10,
                                  decoration: BoxDecoration(
                                    color: colors[idx % colors.length],
                                    shape: BoxShape.circle,
                                  ),
                                ),
                                const SizedBox(width: 6),
                                Expanded(
                                  child: Text(
                                    item.label,
                                    style: const TextStyle(fontSize: 11),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                                Text(
                                  '${item.value} món',
                                  style: const TextStyle(fontSize: 11, fontWeight: FontWeight.bold),
                                ),
                              ],
                            ),
                          );
                        }).toList(),
                      ),
                    ),
                  ],
                ),
        ],
      ),
    );
  }

  Widget _buildBranchPerformanceSection() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'Hiệu suất các chi nhánh',
                style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
              ),
              Text(
                '${_branchPerformances.length} cơ sở',
                style: const TextStyle(fontSize: 12, color: AppColors.textMuted),
              ),
            ],
          ),
          const SizedBox(height: 12),
          _isLoadingCharts
              ? const Center(child: Padding(padding: EdgeInsets.all(12), child: CircularProgressIndicator()))
              : _branchPerformances.isEmpty
                  ? const Padding(
                      padding: EdgeInsets.symmetric(vertical: 12),
                      child: Center(child: Text('Chưa có số liệu hiệu suất', style: TextStyle(color: AppColors.textMuted, fontSize: 12))),
                    )
                  : Column(
                      children: _branchPerformances.map((b) {
                        return Container(
                          margin: const EdgeInsets.symmetric(vertical: 4),
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: AppColors.background,
                            borderRadius: BorderRadius.circular(12),
                            border: Border.all(color: AppColors.border),
                          ),
                          child: Row(
                            children: [
                              CircleAvatar(
                                radius: 16,
                                backgroundColor: AppColors.primaryBrand.withValues(alpha: 0.1),
                                child: const Icon(Icons.store, size: 16, color: AppColors.primaryBrand),
                              ),
                              const SizedBox(width: 10),
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(b.name, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
                                    Text(
                                      'Doanh thu: ${Formatters.formatVND(b.salesAmount)}',
                                      style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                                    ),
                                  ],
                                ),
                              ),
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.end,
                                children: [
                                  Text(
                                    Formatters.formatVND(b.profit),
                                    style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 12, color: AppColors.emerald),
                                  ),
                                  const Text('Lợi nhuận', style: TextStyle(fontSize: 10, color: AppColors.textMuted)),
                                ],
                              ),
                            ],
                          ),
                        );
                      }).toList(),
                    ),
        ],
      ),
    );
  }

  Widget _buildKpiCard({
    required String title,
    required String value,
    required String unit,
    required IconData icon,
    required Color iconColor,
    bool isAlert = false,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: isAlert ? AppColors.warning.withValues(alpha: 0.5) : AppColors.border),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.02),
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
              Text(title, style: const TextStyle(fontSize: 12, color: AppColors.textSecondary, fontWeight: FontWeight.w500)),
              Container(
                padding: const EdgeInsets.all(6),
                decoration: BoxDecoration(
                  color: iconColor.withValues(alpha: 0.12),
                  shape: BoxShape.circle,
                ),
                child: Icon(icon, size: 16, color: iconColor),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            crossAxisAlignment: CrossAxisAlignment.baseline,
            textBaseline: TextBaseline.alphabetic,
            children: [
              Text(
                value,
                style: TextStyle(
                  fontSize: 22,
                  fontWeight: FontWeight.bold,
                  letterSpacing: -0.5,
                  color: isAlert ? AppColors.warning : AppColors.textPrimary,
                ),
              ),
              const SizedBox(width: 4),
              Text(unit, style: const TextStyle(fontSize: 11, color: AppColors.textMuted)),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildQuickActionHub() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Thao tác nhanh',
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                ),
                SizedBox(height: 2),
                Text(
                  'Nghiệp vụ siêu thị & vận hành tức thì',
                  style: TextStyle(fontSize: 11, color: AppColors.textMuted),
                ),
              ],
            ),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
              decoration: BoxDecoration(
                color: AppColors.primaryBrand.withValues(alpha: 0.08),
                borderRadius: BorderRadius.circular(12),
              ),
              child: const Row(
                children: [
                  Icon(Icons.bolt_rounded, size: 14, color: AppColors.primaryBrand),
                  SizedBox(width: 4),
                  Text('3 Giây', style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold, color: AppColors.primaryBrand)),
                ],
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: _buildHubPrimaryCard(
                title: 'Bán hàng mPOS',
                subtitle: 'Quét mã & Thu ngân',
                icon: Icons.point_of_sale_rounded,
                accentColor: AppColors.primaryBrand,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const PosScreen())),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: _buildHubPrimaryCard(
                title: 'Nhập hàng kho',
                subtitle: 'Đơn mua & Nhập NCC',
                icon: Icons.inventory_2_rounded,
                accentColor: const Color(0xFF8B5CF6),
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const PurchaseOrdersScreen())),
              ),
            ),
          ],
        ),
        const SizedBox(height: 10),
        Row(
          children: [
            Expanded(
              child: _buildHubPrimaryCard(
                title: 'Sổ chi phí',
                subtitle: 'Quản lý & Duyệt chi',
                icon: Icons.account_balance_wallet_rounded,
                accentColor: AppColors.warning,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const ExpensesScreen())),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: _buildHubPrimaryCard(
                title: 'Nhà cung cấp',
                subtitle: 'Đối tác & Sổ nợ NCC',
                icon: Icons.local_shipping_rounded,
                accentColor: AppColors.teal,
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const SuppliersScreen())),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          physics: const BouncingScrollPhysics(),
          child: Row(
            children: [
              _buildShortcutPill(
                icon: Icons.request_quote_outlined,
                label: 'Báo giá',
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const QuotationsScreen())),
              ),
              const SizedBox(width: 8),
              _buildShortcutPill(
                icon: Icons.groups_outlined,
                label: 'Khách hàng',
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const CustomersScreen())),
              ),
              const SizedBox(width: 8),
              _buildShortcutPill(
                icon: Icons.history_rounded,
                label: 'Lịch sử đơn',
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const OrdersHistoryScreen())),
              ),
              const SizedBox(width: 8),
              _buildShortcutPill(
                icon: Icons.smart_toy_outlined,
                label: 'Trợ lý AI',
                accentColor: const Color(0xFF7C3AED),
                onTap: () => Navigator.push(context, MaterialPageRoute(builder: (_) => const AIAssistantScreen())),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildHubPrimaryCard({
    required String title,
    required String subtitle,
    required IconData icon,
    required Color accentColor,
    required VoidCallback onTap,
  }) {
    return FlatCard(
      onTap: onTap,
      padding: const EdgeInsets.all(14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: accentColor.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(icon, color: accentColor, size: 20),
              ),
              Icon(Icons.arrow_forward_ios_rounded, size: 12, color: AppColors.textMuted.withValues(alpha: 0.5)),
            ],
          ),
          const SizedBox(height: 12),
          Text(
            title,
            style: const TextStyle(fontSize: 13, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
          ),
          const SizedBox(height: 2),
          Text(
            subtitle,
            style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
          ),
        ],
      ),
    );
  }

  Widget _buildShortcutPill({
    required IconData icon,
    required String label,
    required VoidCallback onTap,
    Color? accentColor,
  }) {
    final color = accentColor ?? AppColors.textSecondary;
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(10),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(color: AppColors.cardBorder),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 16, color: color),
            const SizedBox(width: 6),
            Text(
              label,
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w600,
                color: accentColor != null ? color : AppColors.textPrimary,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
