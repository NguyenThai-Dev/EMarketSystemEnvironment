import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../providers/pos_cart_provider.dart';
import '../../widgets/vnd_text.dart';

/// Bottom Sheet xem và chỉnh sửa giỏ hàng POS chi tiết
class PosCartSheet extends StatelessWidget {
  final VoidCallback onProceedToCheckout;
  final VoidCallback onOpenCustomerSelector;

  const PosCartSheet({
    super.key,
    required this.onProceedToCheckout,
    required this.onOpenCustomerSelector,
  });

  static Future<void> show(
    BuildContext context, {
    required VoidCallback onProceedToCheckout,
    required VoidCallback onOpenCustomerSelector,
  }) {
    return showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => PosCartSheet(
        onProceedToCheckout: onProceedToCheckout,
        onOpenCustomerSelector: onOpenCustomerSelector,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final cart = context.watch<PosCartProvider>();

    return Container(
      constraints: BoxConstraints(
        maxHeight: MediaQuery.of(context).size.height * 0.85,
      ),
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          // Drag handle
          Center(
            child: Container(
              margin: const EdgeInsets.only(top: 10, bottom: 8),
              width: 44,
              height: 4,
              decoration: BoxDecoration(
                color: Colors.grey.shade300,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
          ),

          // Header
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    const Icon(Icons.shopping_cart_outlined, color: AppColors.primaryBrand, size: 22),
                    const SizedBox(width: 8),
                    Text(
                      'Giỏ hàng (${cart.itemCount} sản phẩm)',
                      style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                    ),
                  ],
                ),
                if (!cart.isEmpty)
                  TextButton.icon(
                    style: TextButton.styleFrom(foregroundColor: AppColors.error),
                    icon: const Icon(Icons.delete_sweep_outlined, size: 18),
                    label: const Text('Xóa giỏ', style: TextStyle(fontSize: 13)),
                    onPressed: () {
                      cart.clearCart();
                      Navigator.pop(context);
                    },
                  ),
              ],
            ),
          ),
          const Divider(height: 1, color: AppColors.border),

          // Customer Selector Strip
          _buildCustomerStrip(context, cart),

          // Items list
          Expanded(
            child: cart.isEmpty
                ? const Center(
                    child: Text('Giỏ hàng trống', style: TextStyle(color: AppColors.textSecondary)),
                  )
                : ListView.separated(
                    padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                    itemCount: cart.items.length,
                    separatorBuilder: (_, __) => const Divider(height: 1, color: AppColors.border),
                    itemBuilder: (context, index) {
                      final item = cart.items[index];
                      final p = item.product;

                      return Padding(
                        padding: const EdgeInsets.symmetric(vertical: 8),
                        child: Row(
                          crossAxisAlignment: CrossAxisAlignment.center,
                          children: [
                            // Thumbnail
                            ClipRRect(
                              borderRadius: BorderRadius.circular(10),
                              child: Container(
                                width: 52,
                                height: 52,
                                color: AppColors.surfaceVariant,
                                child: p.fullImageUrl != null
                                    ? CachedNetworkImage(
                                        imageUrl: p.fullImageUrl!,
                                        fit: BoxFit.cover,
                                        memCacheWidth: 160,
                                        memCacheHeight: 160,
                                      )
                                    : const Icon(Icons.inventory_2_outlined, color: AppColors.textMuted),
                              ),
                            ),
                            const SizedBox(width: 12),

                            // Info
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    p.name,
                                    maxLines: 2,
                                    overflow: TextOverflow.ellipsis,
                                    style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13),
                                  ),
                                  const SizedBox(height: 4),
                                  VndText(
                                    item.price,
                                    style: const TextStyle(
                                      fontWeight: FontWeight.bold,
                                      fontSize: 14,
                                      color: AppColors.primaryBrand,
                                    ),
                                  ),
                                ],
                              ),
                            ),

                            // Quantity Stepper (Ergonomic touch targets >= 44x44)
                            Row(
                              children: [
                                _StepperButton(
                                  icon: Icons.remove,
                                  onPressed: () {
                                    HapticFeedback.lightImpact();
                                    cart.updateQuantity(p.productId, -1);
                                  },
                                ),
                                Container(
                                  constraints: const BoxConstraints(minWidth: 36),
                                  alignment: Alignment.center,
                                  child: Text(
                                    '${item.quantity}',
                                    style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                                  ),
                                ),
                                _StepperButton(
                                  icon: Icons.add,
                                  onPressed: () {
                                    HapticFeedback.lightImpact();
                                    cart.updateQuantity(p.productId, 1);
                                  },
                                ),
                              ],
                            ),
                          ],
                        ),
                      );
                    },
                  ),
          ),

          // Bottom Summary & Checkout Button
          Container(
            padding: const EdgeInsets.fromLTRB(20, 14, 20, 24),
            decoration: BoxDecoration(
              color: Colors.white,
              border: const Border(top: BorderSide(color: AppColors.border, width: 1)),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withValues(alpha: 0.04),
                  blurRadius: 10,
                  offset: const Offset(0, -4),
                ),
              ],
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                // Subtotal row
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text('Tổng tiền hàng:', style: TextStyle(color: AppColors.textSecondary, fontSize: 13)),
                    VndText(cart.subtotal, style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 14)),
                  ],
                ),
                if (cart.totalDiscount > 0) ...[
                  const SizedBox(height: 6),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Chiết khấu / Giảm giá:', style: TextStyle(color: AppColors.emerald, fontSize: 13)),
                      VndText(
                        cart.totalDiscount,
                        prefix: '-',
                        style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 14, color: AppColors.emerald),
                      ),
                    ],
                  ),
                ],
                const SizedBox(height: 8),

                // Final Total Row
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text('TỔNG THANH TOÁN:', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                    VndText(
                      cart.finalTotal,
                      style: const TextStyle(fontWeight: FontWeight.w800, fontSize: 20, color: AppColors.emerald),
                    ),
                  ],
                ),
                const SizedBox(height: 14),

                // Checkout CTA Button
                SizedBox(
                  width: double.infinity,
                  height: 50,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primaryBrand,
                      foregroundColor: Colors.white,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                      elevation: 0,
                    ),
                    onPressed: cart.isEmpty
                        ? null
                        : () {
                            Navigator.pop(context);
                            onProceedToCheckout();
                          },
                    child: const Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.payment_rounded, size: 20),
                        SizedBox(width: 8),
                        Text(
                          'TIẾN HÀNH THANH TOÁN',
                          style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold, letterSpacing: 0.3),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCustomerStrip(BuildContext context, PosCartProvider cart) {
    final customer = cart.selectedCustomer;

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      color: AppColors.background,
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            children: [
              Icon(
                customer != null ? Icons.person : Icons.person_outline,
                size: 18,
                color: customer != null ? AppColors.primaryBrand : AppColors.textSecondary,
              ),
              const SizedBox(width: 8),
              Text(
                customer != null ? customer.name : 'Khách vãng lai (Chưa tích điểm)',
                style: TextStyle(
                  fontSize: 12,
                  fontWeight: customer != null ? FontWeight.bold : FontWeight.normal,
                  color: customer != null ? AppColors.textPrimary : AppColors.textSecondary,
                ),
              ),
            ],
          ),
          TextButton(
            style: TextButton.styleFrom(
              visualDensity: VisualDensity.compact,
              padding: const EdgeInsets.symmetric(horizontal: 8),
            ),
            onPressed: () {
              Navigator.pop(context);
              onOpenCustomerSelector();
            },
            child: Text(
              customer != null ? 'Đổi khách' : 'Chọn khách',
              style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
            ),
          ),
        ],
      ),
    );
  }
}

class _StepperButton extends StatelessWidget {
  final IconData icon;
  final VoidCallback onPressed;

  const _StepperButton({required this.icon, required this.onPressed});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 36,
      height: 36,
      decoration: BoxDecoration(
        color: AppColors.surfaceVariant.withValues(alpha: 0.6),
        borderRadius: BorderRadius.circular(10),
      ),
      child: IconButton(
        icon: Icon(icon, size: 16, color: AppColors.textPrimary),
        padding: EdgeInsets.zero,
        constraints: const BoxConstraints(),
        onPressed: onPressed,
      ),
    );
  }
}
