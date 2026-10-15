import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../core/constants/app_colors.dart';
import '../../providers/auth_provider.dart';

class BranchSelectorWidget extends StatelessWidget {
  final bool compact;

  const BranchSelectorWidget({super.key, this.compact = false});

  void _showBranchPicker(BuildContext context, AuthProvider auth) {
    if (!auth.canSwitchBranch) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Row(
            children: [
              const Icon(Icons.lock_outline, color: Colors.white, size: 18),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Tài khoản của bạn được phân công cố định tại "${auth.selectedBranch?.branchName ?? "chi nhánh"}".',
                  style: const TextStyle(fontSize: 13),
                ),
              ),
            ],
          ),
          backgroundColor: AppColors.textSecondary,
          duration: const Duration(seconds: 2),
          behavior: SnackBarBehavior.floating,
        ),
      );
      return;
    }

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) {
        final currentId = auth.selectedBranch?.branchId ?? 0;
        final branches = auth.branches;

        return Container(
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
          ),
          padding: const EdgeInsets.only(top: 12, bottom: 24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              // Drag handle
              Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: AppColors.border,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              const SizedBox(height: 16),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                child: Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: AppColors.primaryBrand.withValues(alpha: 0.1),
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: const Icon(Icons.storefront_rounded, color: AppColors.primaryBrand, size: 22),
                    ),
                    const SizedBox(width: 12),
                    const Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Chọn chi nhánh làm việc',
                            style: TextStyle(fontSize: 17, fontWeight: FontWeight.bold, letterSpacing: -0.3),
                          ),
                          SizedBox(height: 2),
                          Text(
                            'Dữ liệu kho, doanh thu và bán hàng sẽ cập nhật theo chi nhánh',
                            style: TextStyle(fontSize: 12, color: AppColors.textSecondary),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 12),
              const Divider(height: 1),
              ConstrainedBox(
                constraints: BoxConstraints(maxHeight: MediaQuery.of(context).size.height * 0.5),
                child: ListView.separated(
                  shrinkWrap: true,
                  itemCount: branches.length,
                  separatorBuilder: (context, i) => const Divider(height: 1, indent: 64),
                  itemBuilder: (ctx, index) {
                    final b = branches[index];
                    final isSelected = b.branchId == currentId;
                    final isAll = b.branchId == 0;

                    return ListTile(
                      contentPadding: const EdgeInsets.symmetric(horizontal: 20, vertical: 4),
                      leading: Container(
                        padding: const EdgeInsets.all(8),
                        decoration: BoxDecoration(
                          color: isSelected
                              ? AppColors.primaryBrand.withValues(alpha: 0.15)
                              : (isAll ? Colors.amber.withValues(alpha: 0.1) : AppColors.surfaceVariant),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Icon(
                          isAll ? Icons.hub_outlined : Icons.store_mall_directory_outlined,
                          color: isSelected
                              ? AppColors.primaryBrand
                              : (isAll ? Colors.amber[800] : AppColors.textSecondary),
                          size: 20,
                        ),
                      ),
                      title: Text(
                        b.branchName,
                        style: TextStyle(
                          fontSize: 14,
                          fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
                          color: isSelected ? AppColors.primaryBrand : AppColors.textPrimary,
                        ),
                      ),
                      subtitle: (b.address != null && b.address!.isNotEmpty)
                          ? Text(
                              b.address!,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: const TextStyle(fontSize: 12, color: AppColors.textSecondary),
                            )
                          : null,
                      trailing: isSelected
                          ? const Icon(Icons.check_circle_rounded, color: AppColors.primaryBrand, size: 22)
                          : null,
                      onTap: () {
                        auth.selectBranch(b);
                        Navigator.pop(ctx);
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Row(
                              children: [
                                const Icon(Icons.swap_horiz_rounded, color: Colors.white, size: 18),
                                const SizedBox(width: 8),
                                Expanded(child: Text('Đã chuyển sang chi nhánh: ${b.branchName}')),
                              ],
                            ),
                            backgroundColor: AppColors.emerald,
                            duration: const Duration(seconds: 1),
                            behavior: SnackBarBehavior.floating,
                          ),
                        );
                      },
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

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final selectedBranch = auth.selectedBranch;
    final canSwitch = auth.canSwitchBranch;
    final branchName = selectedBranch?.branchName ?? 'Chưa chọn chi nhánh';

    if (compact) {
      return InkWell(
        onTap: () => _showBranchPicker(context, auth),
        borderRadius: BorderRadius.circular(20),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
          decoration: BoxDecoration(
            color: canSwitch ? AppColors.primaryBrand.withValues(alpha: 0.08) : AppColors.surfaceVariant,
            borderRadius: BorderRadius.circular(20),
            border: Border.all(
              color: canSwitch ? AppColors.primaryBrand.withValues(alpha: 0.25) : AppColors.border,
            ),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                canSwitch ? Icons.storefront_rounded : Icons.store_mall_directory_outlined,
                size: 14,
                color: canSwitch ? AppColors.primaryBrand : AppColors.textSecondary,
              ),
              const SizedBox(width: 6),
              ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 160),
                child: Text(
                  branchName,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                    color: canSwitch ? AppColors.primaryBrand : AppColors.textPrimary,
                  ),
                ),
              ),
              const SizedBox(width: 4),
              Icon(
                canSwitch ? Icons.arrow_drop_down_rounded : Icons.lock_outline_rounded,
                size: 16,
                color: canSwitch ? AppColors.primaryBrand : AppColors.textMuted,
              ),
            ],
          ),
        ),
      );
    }

    return InkWell(
      onTap: () => _showBranchPicker(context, auth),
      borderRadius: BorderRadius.circular(12),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: canSwitch ? AppColors.primaryBrand.withValues(alpha: 0.3) : AppColors.border,
          ),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.03),
              blurRadius: 4,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              padding: const EdgeInsets.all(6),
              decoration: BoxDecoration(
                color: canSwitch ? AppColors.primaryBrand.withValues(alpha: 0.1) : AppColors.surfaceVariant,
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(
                Icons.storefront_rounded,
                size: 16,
                color: canSwitch ? AppColors.primaryBrand : AppColors.textSecondary,
              ),
            ),
            const SizedBox(width: 8),
            Flexible(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        canSwitch ? 'CHI NHÁNH LÀM VIỆC' : 'CHI NHÁNH PHÂN CÔNG',
                        style: const TextStyle(fontSize: 9, fontWeight: FontWeight.bold, color: AppColors.textMuted, letterSpacing: 0.5),
                      ),
                      if (!canSwitch) ...[
                        const SizedBox(width: 4),
                        const Icon(Icons.lock_outline, size: 10, color: AppColors.textMuted),
                      ],
                    ],
                  ),
                  const SizedBox(height: 2),
                  Text(
                    branchName,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: TextStyle(
                      fontSize: 13,
                      fontWeight: FontWeight.bold,
                      color: canSwitch ? AppColors.primaryBrand : AppColors.textPrimary,
                    ),
                  ),
                ],
              ),
            ),
            if (canSwitch) ...[
              const SizedBox(width: 6),
              const Icon(Icons.keyboard_arrow_down_rounded, size: 18, color: AppColors.primaryBrand),
            ],
          ],
        ),
      ),
    );
  }
}
