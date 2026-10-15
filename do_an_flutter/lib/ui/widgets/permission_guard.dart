import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/auth_provider.dart';

/// Role-Based Access Control (RBAC) Widget Guard
class PermissionGuard extends StatelessWidget {
  final String? permission;
  final bool requireAdmin;
  final Widget child;
  final Widget fallback;

  const PermissionGuard({
    super.key,
    this.permission,
    this.requireAdmin = false,
    required this.child,
    this.fallback = const SizedBox.shrink(),
  });

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();
    final user = auth.currentUser;

    if (user == null) return fallback;

    if (requireAdmin && !user.isAdmin) {
      return fallback;
    }

    if (permission != null && !user.hasPermission(permission!)) {
      return fallback;
    }

    return child;
  }
}
