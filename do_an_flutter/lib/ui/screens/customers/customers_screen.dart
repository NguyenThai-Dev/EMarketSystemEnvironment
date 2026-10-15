import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:cached_network_image/cached_network_image.dart';
import '../../../core/constants/app_colors.dart';
import '../../../core/utils/image_helper.dart';
import '../../../data/models/customer_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../widgets/flat_card.dart';
import '../../widgets/status_badge.dart';
import 'add_edit_customer_dialog.dart';

class CustomersScreen extends StatefulWidget {
  const CustomersScreen({super.key});

  @override
  State<CustomersScreen> createState() => _CustomersScreenState();
}

class _CustomersScreenState extends State<CustomersScreen> {
  final _repository = DataRepository();
  final _searchCtrl = TextEditingController();

  List<CustomerModel> _customers = [];
  CustomerStatsModel? _stats;
  bool _isLoading = true;
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  @override
  void dispose() {
    _searchCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadData() async {
    setState(() => _isLoading = true);
    try {
      final results = await Future.wait([
        _repository.getCustomers(keyword: _searchQuery.isEmpty ? null : _searchQuery),
        _repository.getCustomerStats(),
      ]);

      if (mounted) {
        setState(() {
          _customers = results[0] as List<CustomerModel>;
          _stats = results[1] as CustomerStatsModel?;
          _isLoading = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _makePhoneCall(String? phone) async {
    if (phone == null || phone.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Khách hàng chưa cập nhật số điện thoại.')),
      );
      return;
    }
    final uri = Uri.parse('tel:${phone.replaceAll(' ', '')}');
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri);
    } else {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Không thể thực hiện cuộc gọi đến $phone')),
      );
    }
  }

  Future<void> _sendSms(String? phone) async {
    if (phone == null || phone.trim().isEmpty) return;
    final uri = Uri.parse('sms:${phone.replaceAll(' ', '')}');
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri);
    }
  }

  Future<void> _showLoyaltyHistory(CustomerModel customer) async {
    showDialog(
      context: context,
      builder: (ctx) => FutureBuilder<List<LoyaltyProgramModel>>(
        future: _repository.getLoyaltyHistory(customerId: customer.customerId),
        builder: (context, snapshot) {
          final history = snapshot.data ?? [];
          return AlertDialog(
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
            title: Row(
              children: [
                Icon(Icons.stars_rounded, color: customer.tierColor),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    'Điểm tích lũy: ${customer.fullName}',
                    style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                  ),
                ),
              ],
            ),
            content: SizedBox(
              width: 360,
              height: 320,
              child: snapshot.connectionState == ConnectionState.waiting
                  ? const Center(child: CircularProgressIndicator())
                  : history.isEmpty
                      ? Center(
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Icon(Icons.history, size: 48, color: AppColors.textMuted.withValues(alpha: 0.5)),
                              const SizedBox(height: 8),
                              const Text('Chưa có lịch sử giao dịch tích điểm', style: TextStyle(color: AppColors.textMuted)),
                            ],
                          ),
                        )
                      : ListView.separated(
                          itemCount: history.length,
                          separatorBuilder: (_, _) => const Divider(height: 1),
                          itemBuilder: (context, index) {
                            final item = history[index];
                            final isEarned = item.pointsEarned > 0;
                            return ListTile(
                              dense: true,
                              contentPadding: EdgeInsets.zero,
                              leading: CircleAvatar(
                                radius: 16,
                                backgroundColor: (isEarned ? AppColors.success : AppColors.error).withValues(alpha: 0.15),
                                child: Icon(
                                  isEarned ? Icons.add : Icons.remove,
                                  size: 16,
                                  color: isEarned ? AppColors.success : AppColors.error,
                                ),
                              ),
                              title: Text(
                                isEarned ? '+${item.pointsEarned} điểm' : '-${item.pointsRedeemed} điểm',
                                style: TextStyle(
                                  fontWeight: FontWeight.bold,
                                  color: isEarned ? AppColors.success : AppColors.error,
                                ),
                              ),
                              subtitle: Text(
                                'Mã đơn: #${item.orderId ?? 'N/A'} • ${item.createdAt.day}/${item.createdAt.month}/${item.createdAt.year}',
                                style: const TextStyle(fontSize: 11, color: AppColors.textMuted),
                              ),
                            );
                          },
                        ),
            ),
            actions: [
              TextButton(onPressed: () => Navigator.pop(ctx), child: const Text('Đóng')),
            ],
          );
        },
      ),
    );
  }

  Future<void> _deleteCustomer(CustomerModel customer) async {
    final confirm = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Xác nhận xóa khách hàng'),
        content: Text('Bạn có chắc chắn muốn xóa hồ sơ khách hàng "${customer.fullName}" không?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.error),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Xóa', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );

    if (confirm != true) return;

    final ok = await _repository.deleteCustomer(customer.customerId);
    if (!mounted) return;
    if (ok) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Xóa khách hàng thành công!'), backgroundColor: AppColors.success),
      );
      _loadData();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không thể xóa khách hàng.'), backgroundColor: AppColors.error),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Quản lý Khách hàng & Hội viên', style: TextStyle(fontWeight: FontWeight.bold)),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadData,
            tooltip: 'Làm mới',
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppColors.primary,
        icon: const Icon(Icons.person_add, color: Colors.white),
        label: const Text('Thêm khách', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
        onPressed: () async {
          final res = await showDialog<bool>(
            context: context,
            builder: (_) => const AddEditCustomerDialog(),
          );
          if (res == true) _loadData();
        },
      ),
      body: Column(
        children: [
          // Thống kê nhanh CRM
          if (_stats != null)
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
              color: Colors.white,
              child: Row(
                children: [
                  Expanded(
                    child: _buildStatItem('Tổng khách', '${_stats!.totalCustomers}', Icons.people, AppColors.primary),
                  ),
                  Container(width: 1, height: 32, color: AppColors.border),
                  Expanded(
                    child: _buildStatItem('Hội viên VIP', '${_stats!.vipCustomers}', Icons.workspace_premium, const Color(0xFFF59E0B)),
                  ),
                  Container(width: 1, height: 32, color: AppColors.border),
                  Expanded(
                    child: _buildStatItem('Mới (30 ngày)', '${_stats!.newCustomersSinceDate}', Icons.person_add_alt_1, AppColors.success),
                  ),
                ],
              ),
            ),

          // Thanh tìm kiếm
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 8),
            child: TextField(
              controller: _searchCtrl,
              decoration: InputDecoration(
                hintText: 'Tìm theo tên hoặc số điện thoại...',
                prefixIcon: const Icon(Icons.search, color: AppColors.textMuted),
                suffixIcon: _searchQuery.isNotEmpty
                    ? IconButton(
                        icon: const Icon(Icons.clear, color: AppColors.textMuted),
                        onPressed: () {
                          _searchCtrl.clear();
                          setState(() => _searchQuery = '');
                          _loadData();
                        },
                      )
                    : null,
                filled: true,
                fillColor: Colors.white,
                contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(color: AppColors.border),
                ),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(color: AppColors.border),
                ),
              ),
              onSubmitted: (val) {
                setState(() => _searchQuery = val);
                _loadData();
              },
            ),
          ),

          // Danh sách khách hàng
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _customers.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(Icons.person_off_outlined, size: 64, color: AppColors.textMuted.withValues(alpha: 0.5)),
                            const SizedBox(height: 12),
                            const Text('Không tìm thấy khách hàng nào', style: TextStyle(color: AppColors.textMuted)),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: _loadData,
                        child: ListView.separated(
                          padding: const EdgeInsets.fromLTRB(16, 8, 16, 80),
                          itemCount: _customers.length,
                          separatorBuilder: (_, _) => const SizedBox(height: 10),
                          itemBuilder: (context, index) {
                            final c = _customers[index];
                            return _buildCustomerCard(c);
                          },
                        ),
                      ),
          ),
        ],
      ),
    );
  }

  Widget _buildStatItem(String label, String value, IconData icon, Color color) {
    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, size: 16, color: color),
            const SizedBox(width: 4),
            Text(value, style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: color)),
          ],
        ),
        const SizedBox(height: 2),
        Text(label, style: const TextStyle(fontSize: 11, color: AppColors.textMuted)),
      ],
    );
  }

  Widget _buildCustomerCard(CustomerModel customer) {
    final initials = customer.fullName.isNotEmpty
        ? customer.fullName.trim().split(' ').map((e) => e.isNotEmpty ? e[0] : '').take(2).join()
        : 'KH';
    final hasAvatar = customer.userImg != null &&
        customer.userImg!.trim().isNotEmpty &&
        ImageHelper.getFullImageUrl(customer.userImg) != null;

    return FlatCard(
      padding: const EdgeInsets.all(14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Avatar
              ClipRRect(
                borderRadius: BorderRadius.circular(20),
                child: SizedBox(
                  width: 44,
                  height: 44,
                  child: hasAvatar
                      ? CachedNetworkImage(
                          imageUrl: ImageHelper.getFullImageUrl(customer.userImg)!,
                          memCacheWidth: 88,
                          memCacheHeight: 88,
                          fit: BoxFit.cover,
                          errorWidget: (_, __, ___) => _buildInitialsAvatar(customer, initials),
                        )
                      : _buildInitialsAvatar(customer, initials),
                ),
              ),
              const SizedBox(width: 12),

              // Main Info
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            customer.fullName,
                            style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                        StatusBadge(
                          label: customer.tierName,
                          color: customer.tierColor,
                          icon: Icons.workspace_premium_rounded,
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    if (customer.phone != null && customer.phone!.isNotEmpty)
                      Row(
                        children: [
                          const Icon(Icons.phone_outlined, size: 13, color: AppColors.textMuted),
                          const SizedBox(width: 4),
                          Text(customer.phone!, style: const TextStyle(fontSize: 12, color: AppColors.textSecondary)),
                        ],
                      ),
                    if (customer.email != null && customer.email!.isNotEmpty)
                      Padding(
                        padding: const EdgeInsets.only(top: 2),
                        child: Row(
                          children: [
                            const Icon(Icons.email_outlined, size: 13, color: AppColors.textMuted),
                            const SizedBox(width: 4),
                            Expanded(
                              child: Text(
                                customer.email!,
                                style: const TextStyle(fontSize: 12, color: AppColors.textMuted),
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis,
                              ),
                            ),
                          ],
                        ),
                      ),
                  ],
                ),
              ),
            ],
          ),

          const SizedBox(height: 10),
          const Divider(height: 1, color: AppColors.border),
          const SizedBox(height: 8),

          // Point balance & Actions
          Row(
            children: [
              // Điểm khả dụng
              InkWell(
                onTap: () => _showLoyaltyHistory(customer),
                borderRadius: BorderRadius.circular(8),
                child: Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: AppColors.primaryBrand.withValues(alpha: 0.08),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Icon(Icons.stars_rounded, size: 16, color: AppColors.primaryBrand),
                      const SizedBox(width: 4),
                      Text(
                        '${customer.pointBalance} điểm',
                        style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold, color: AppColors.primaryBrand),
                      ),
                      const Icon(Icons.chevron_right, size: 14, color: AppColors.primaryBrand),
                    ],
                  ),
                ),
              ),

              const Spacer(),

              // Nút Gọi điện thoại 1 chạm
              if (customer.phone != null && customer.phone!.isNotEmpty) ...[
                IconButton(
                  icon: const Icon(Icons.call, size: 18, color: AppColors.emerald),
                  tooltip: 'Gọi điện',
                  onPressed: () => _makePhoneCall(customer.phone),
                  constraints: const BoxConstraints(minWidth: 34, minHeight: 34),
                  padding: EdgeInsets.zero,
                ),
                IconButton(
                  icon: const Icon(Icons.chat_bubble_outline, size: 18, color: AppColors.secondary),
                  tooltip: 'Nhắn tin SMS',
                  onPressed: () => _sendSms(customer.phone),
                  constraints: const BoxConstraints(minWidth: 34, minHeight: 34),
                  padding: EdgeInsets.zero,
                ),
              ],

              // Sửa thông tin
              IconButton(
                icon: const Icon(Icons.edit_outlined, size: 18, color: AppColors.textSecondary),
                tooltip: 'Sửa thông tin',
                onPressed: () async {
                  final res = await showDialog<bool>(
                    context: context,
                    builder: (_) => AddEditCustomerDialog(customer: customer),
                  );
                  if (res == true) _loadData();
                },
                constraints: const BoxConstraints(minWidth: 34, minHeight: 34),
                padding: EdgeInsets.zero,
              ),

              // Xóa
              IconButton(
                icon: const Icon(Icons.delete_outline, size: 18, color: AppColors.error),
                tooltip: 'Xóa khách hàng',
                onPressed: () => _deleteCustomer(customer),
                constraints: const BoxConstraints(minWidth: 34, minHeight: 34),
                padding: EdgeInsets.zero,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildInitialsAvatar(CustomerModel customer, String initials) {
    return Container(
      color: customer.tierColor.withValues(alpha: 0.15),
      alignment: Alignment.center,
      child: Text(
        initials.toUpperCase(),
        style: TextStyle(
          color: customer.tierColor,
          fontWeight: FontWeight.bold,
          fontSize: 14,
        ),
      ),
    );
  }
}
