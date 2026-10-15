class RoleModel {
  final int roleId;
  final String roleName;

  RoleModel({required this.roleId, required this.roleName});

  factory RoleModel.fromJson(dynamic json) {
    if (json is String) {
      return RoleModel(roleId: 0, roleName: json);
    }
    if (json is Map<String, dynamic>) {
      return RoleModel(
        roleId: (json['roleId'] ?? json['RoleId'] ?? json['id'] as num?)?.toInt() ?? 0,
        roleName: (json['roleName'] ?? json['RoleName'] ?? json['name'] ?? json['Name'] ?? '').toString(),
      );
    }
    return RoleModel(roleId: 0, roleName: json?.toString() ?? '');
  }

  Map<String, dynamic> toJson() => {
    'roleId': roleId,
    'roleName': roleName,
  };
}

class PermissionModel {
  final int permissionId;
  final String permissionName;
  final String? module;

  PermissionModel({
    required this.permissionId,
    required this.permissionName,
    this.module,
  });

  factory PermissionModel.fromJson(dynamic json) {
    if (json is String) {
      return PermissionModel(permissionId: 0, permissionName: json);
    }
    if (json is Map<String, dynamic>) {
      return PermissionModel(
        permissionId: (json['permissionId'] ?? json['PermissionId'] ?? json['id'] as num?)?.toInt() ?? 0,
        permissionName: (json['permissionName'] ?? json['PermissionName'] ?? json['name'] ?? json['Name'] ?? '').toString(),
        module: json['module'] ?? json['Module'],
      );
    }
    return PermissionModel(permissionId: 0, permissionName: json?.toString() ?? '');
  }

  Map<String, dynamic> toJson() => {
    'permissionId': permissionId,
    'permissionName': permissionName,
    if (module != null) 'module': module,
  };
}

class UserModel {
  final int userId;
  final String username;
  final String fullName;
  final String? email;
  final String? phone;
  final String? image;
  final String? status;
  final int? branchId;
  final int? supplierId;
  final int roleId;
  final List<RoleModel> roles;
  final List<PermissionModel> permissions;
  final bool isAdmin;
  final bool isSupplier;

  UserModel({
    required this.userId,
    required this.username,
    required this.fullName,
    this.email,
    this.phone,
    this.image,
    this.status,
    this.branchId,
    this.supplierId,
    this.roleId = 0,
    this.roles = const [],
    this.permissions = const [],
    this.isAdmin = false,
    this.isSupplier = false,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) {
    List<RoleModel> parseRoles() {
      final list = json['roles'] ?? json['Roles'];
      if (list is List) {
        return list.map((e) => RoleModel.fromJson(e)).toList();
      }
      return [];
    }

    List<PermissionModel> parsePermissions() {
      final list = json['permissions'] ?? json['Permissions'];
      if (list is List) {
        return list.map((e) => PermissionModel.fromJson(e)).toList();
      }
      return [];
    }

    final rawIsAdmin = json['isAdmin'] ?? json['IsAdmin'] ?? false;
    final rawIsSupplier = json['isSupplier'] ?? json['IsSupplier'] ?? false;

    return UserModel(
      userId: (json['userId'] ?? json['UserId'] ?? json['id'] ?? json['Id'] as num?)?.toInt() ?? 0,
      username: (json['username'] ?? json['Username'] ?? json['userName'] ?? json['UserName'] ?? '').toString(),
      fullName: (json['fullName'] ?? json['FullName'] ?? json['name'] ?? json['Name'] ?? json['username'] ?? '').toString(),
      email: json['email'] ?? json['Email'],
      phone: json['phone'] ?? json['Phone'],
      image: json['image'] ?? json['Image'] ?? json['avatar'] ?? json['Avatar'] ?? json['userImg'],
      status: json['status'] ?? json['Status'],
      branchId: (json['branchId'] ?? json['BranchId'] as num?)?.toInt(),
      supplierId: (json['supplierId'] ?? json['SupplierId'] as num?)?.toInt(),
      roleId: (json['roleId'] ?? json['RoleId'] as num?)?.toInt() ?? 0,
      roles: parseRoles(),
      permissions: parsePermissions(),
      isAdmin: rawIsAdmin == true || rawIsAdmin.toString().toLowerCase() == 'true',
      isSupplier: rawIsSupplier == true || rawIsSupplier.toString().toLowerCase() == 'true',
    );
  }

  Map<String, dynamic> toJson() => {
    'userId': userId,
    'username': username,
    'fullName': fullName,
    'email': email,
    'phone': phone,
    'image': image,
    'status': status,
    'branchId': branchId,
    'supplierId': supplierId,
    'roleId': roleId,
    'roles': roles.map((e) => e.toJson()).toList(),
    'permissions': permissions.map((e) => e.toJson()).toList(),
    'isAdmin': isAdmin,
    'isSupplier': isSupplier,
  };

  String get primaryRoleName {
    if (isAdmin) return 'Quản Trị Viên (System Admin)';
    if (roles.isNotEmpty) return roles.first.roleName;
    return 'Nhân viên';
  }

  /// Check if user has permission, module access, or is Admin
  bool hasPermission(String permissionOrModule) {
    if (isAdmin) return true;
    final key = permissionOrModule.trim().toLowerCase();
    // 1. Check module
    if (permissions.any((p) => p.module != null && p.module!.toLowerCase() == key)) {
      return true;
    }
    // 2. Check permission name
    if (permissions.any((p) => p.permissionName.toLowerCase() == key)) {
      return true;
    }
    // 3. Check role name contains
    if (roles.any((r) => r.roleName.toLowerCase().contains(key))) {
      return true;
    }
    return false;
  }
}

class LoginResponse {
  final bool success;
  final String? message;
  final String? token;
  final UserModel? user;

  LoginResponse({
    required this.success,
    this.message,
    this.token,
    this.user,
  });

  factory LoginResponse.fromJson(Map<String, dynamic> json) {
    UserModel? parsedUser;
    final userData = json['user'] ?? json['User'] ?? json['currentUser'] ?? json['data'] ?? json['Data'];
    if (userData != null && userData is Map<String, dynamic>) {
      parsedUser = UserModel.fromJson(userData);
    }

    final isOk = json['success'] ?? json['Success'] ?? (parsedUser != null);
    final msg = json['message'] ?? json['Message'] ?? json['msg'];
    final tok = json['token'] ?? json['Token'] ?? json['accessToken'];

    return LoginResponse(
      success: isOk is bool ? isOk : (isOk.toString().toLowerCase() == 'true'),
      message: msg?.toString(),
      token: tok?.toString(),
      user: parsedUser,
    );
  }
}
