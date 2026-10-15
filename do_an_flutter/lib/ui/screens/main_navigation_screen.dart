import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../core/constants/app_colors.dart';
import '../../data/models/user_model.dart';
import '../../providers/auth_provider.dart';
import 'dashboard/dashboard_screen.dart';
import 'inventory/warehouse_screen.dart';
import 'pos/pos_screen.dart';
import 'products/products_screen.dart';
import 'profile/profile_screen.dart';

class MainNavigationScreen extends StatefulWidget {
  const MainNavigationScreen({super.key});

  @override
  State<MainNavigationScreen> createState() => _MainNavigationScreenState();
}

class _NavigationItem {
  final Widget screen;
  final NavigationDestination destination;
  final String module;

  const _NavigationItem({
    required this.screen,
    required this.destination,
    required this.module,
  });
}

class _MainNavigationScreenState extends State<MainNavigationScreen> {
  int _currentIndex = 0;

  List<_NavigationItem> _getAvailableItems(UserModel? user) {
    if (user == null) {
      return [
        const _NavigationItem(
          screen: ProfileScreen(),
          destination: NavigationDestination(
            icon: Icon(Icons.person_outline),
            selectedIcon: Icon(Icons.person, color: AppColors.primaryBrand),
            label: 'Tài khoản',
          ),
          module: '',
        ),
      ];
    }

    final items = <_NavigationItem>[];

    // 1. Dashboard Module
    if (user.isAdmin || user.hasPermission('DashboardModule') || user.hasPermission('ReportModule')) {
      items.add(
        const _NavigationItem(
          screen: DashboardScreen(),
          destination: NavigationDestination(
            icon: Icon(Icons.dashboard_outlined),
            selectedIcon: Icon(Icons.dashboard_rounded, color: AppColors.primaryBrand),
            label: 'Tổng quan',
          ),
          module: 'DashboardModule',
        ),
      );
    }

    // 2. Sales / POS Module
    if (user.isAdmin || user.hasPermission('SalesModule')) {
      items.add(
        const _NavigationItem(
          screen: PosScreen(),
          destination: NavigationDestination(
            icon: Icon(Icons.point_of_sale_outlined),
            selectedIcon: Icon(Icons.point_of_sale_rounded, color: AppColors.primaryBrand),
            label: 'Bán hàng',
          ),
          module: 'SalesModule',
        ),
      );
    }

    // 3. Product Module
    if (user.isAdmin || user.hasPermission('ProductModule')) {
      items.add(
        const _NavigationItem(
          screen: ProductsScreen(),
          destination: NavigationDestination(
            icon: Icon(Icons.inventory_2_outlined),
            selectedIcon: Icon(Icons.inventory_2_rounded, color: AppColors.primaryBrand),
            label: 'Hàng hóa',
          ),
          module: 'ProductModule',
        ),
      );
    }

    // 4. Warehouse / Inventory Module
    if (user.isAdmin || user.hasPermission('InventoryModule')) {
      items.add(
        const _NavigationItem(
          screen: WarehouseScreen(),
          destination: NavigationDestination(
            icon: Icon(Icons.warehouse_outlined),
            selectedIcon: Icon(Icons.warehouse_rounded, color: AppColors.primaryBrand),
            label: 'Kho bãi',
          ),
          module: 'InventoryModule',
        ),
      );
    }

    // 5. Profile Module (luôn hiển thị cho mọi người dùng)
    items.add(
      const _NavigationItem(
        screen: ProfileScreen(),
        destination: NavigationDestination(
          icon: Icon(Icons.person_outline),
          selectedIcon: Icon(Icons.person, color: AppColors.primaryBrand),
          label: 'Tài khoản',
        ),
        module: '',
      ),
    );

    return items;
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final availableItems = _getAvailableItems(auth.currentUser);

    final safeIndex = _currentIndex >= availableItems.length ? 0 : _currentIndex;

    return Scaffold(
      body: IndexedStack(
        index: safeIndex,
        children: availableItems.map((e) => e.screen).toList(),
      ),
      bottomNavigationBar: Container(
        decoration: const BoxDecoration(
          border: Border(top: BorderSide(color: AppColors.border, width: 1)),
        ),
        child: NavigationBar(
          selectedIndex: safeIndex,
          backgroundColor: Colors.white,
          indicatorColor: AppColors.primaryBrand.withValues(alpha: 0.12),
          onDestinationSelected: (index) {
            setState(() => _currentIndex = index);
          },
          destinations: availableItems.map((e) => e.destination).toList(),
        ),
      ),
    );
  }
}
