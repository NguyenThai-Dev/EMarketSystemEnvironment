import 'package:dio/dio.dart';
import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../../core/network/api_exception.dart';
import '../../core/network/token_storage.dart';
import '../models/dashboard_model.dart';
import '../models/user_model.dart';

class AuthApiService {
  final ApiClient _client = ApiClient();

  /// Extract ASP.NET MVC CSRF Anti-Forgery Token from HTML
  String? _extractCsrfToken(String html) {
    final pattern1 = RegExp(r'name="__RequestVerificationToken"\s+type="hidden"\s+value="([^"]+)"');
    final match1 = pattern1.firstMatch(html);
    if (match1 != null) return match1.group(1);

    final pattern2 = RegExp(r'value="([^"]+)"\s+type="hidden"\s+name="__RequestVerificationToken"');
    final match2 = pattern2.firstMatch(html);
    if (match2 != null) return match2.group(1);

    return null;
  }

  /// Direct REST Web API + MVC Session Login Flow matching backend EMarket
  Future<LoginResponse> login(String username, String password) async {
    // 1. Try Direct REST Web API (UserAdminApiController: api/admin/user-management/auth/login)
    try {
      final directRes = await _client.post(
        ApiEndpoints.directLogin,
        data: {
          'EmailOrUsername': username.trim(),
          'Password': password.trim(),
        },
      );
      if (directRes.data is Map<String, dynamic>) {
        final loginRes = LoginResponse.fromJson(directRes.data);
        if (loginRes.success) {
          if (loginRes.token != null && loginRes.token!.isNotEmpty) {
            await TokenStorage.saveToken(loginRes.token!);
          }
          if (loginRes.user != null) {
            await TokenStorage.saveUser(loginRes.user!);
          } else {
            final user = await getCurrentUser();
            if (user != null) {
              await TokenStorage.saveUser(user);
            }
          }
          return loginRes;
        } else {
          return loginRes;
        }
      }
    } on ApiException catch (apiErr) {
      // Nếu là lỗi sai mật khẩu, tài khoản bị khóa, mất mạng hoặc sai IP, trả về lỗi chi tiết ngay lập tức
      if (apiErr.statusCode == 400 || apiErr.statusCode == 401 || apiErr.statusCode == 403) {
        return LoginResponse(success: false, message: apiErr.message);
      }
      if (apiErr.type == ApiErrorType.cannotConnect || 
          apiErr.type == ApiErrorType.disconnected || 
          apiErr.type == ApiErrorType.timeout) {
        return LoginResponse(success: false, message: apiErr.message);
      }
      // Chỉ thử fallback nếu 404 (route REST chưa tồn tại trên backend cũ)
    } catch (_) {
      // Fallback to MVC Session login
    }

    // 2. Fallback to MVC Form Login (Admin/Login/Login) with Anti-Forgery Token
    String? csrfToken;
    try {
      final pageRes = await _client.get(
        ApiEndpoints.loginPage,
        options: Options(responseType: ResponseType.plain),
      );
      if (pageRes.data != null) {
        csrfToken = _extractCsrfToken(pageRes.data.toString());
      }
    } catch (_) {
      // Continue even if CSRF extraction fails
    }

    final formData = FormData.fromMap({
      'EmailOrUsername': username.trim(),
      'Password': password.trim(),
      '__RequestVerificationToken': ?csrfToken,
    });

    final res = await _client.post(
      ApiEndpoints.loginPost,
      data: formData,
      options: Options(
        contentType: Headers.formUrlEncodedContentType,
      ),
    );

    if (res.data is Map<String, dynamic>) {
      final loginRes = LoginResponse.fromJson(res.data);
      if (loginRes.success) {
        if (loginRes.token != null && loginRes.token!.isNotEmpty) {
          await TokenStorage.saveToken(loginRes.token!);
        }
        if (loginRes.user != null) {
          await TokenStorage.saveUser(loginRes.user!);
        } else {
          // If response didn't return full user object, fetch current user info
          final user = await getCurrentUser();
          if (user != null) {
            await TokenStorage.saveUser(user);
          }
        }
      }
      return loginRes;
    }

    throw Exception('Dữ liệu phản hồi từ máy chủ không đúng định dạng JSON.');
  }

  /// Direct Google Login via EMarket Web API (POST api/admin/user-management/auth/google-login)
  Future<LoginResponse> loginWithGoogle({required String email, String? fullName, String? idToken}) async {
    try {
      final res = await _client.post(
        ApiEndpoints.googleLogin,
        data: {
          'Provider': 'Google',
          'Email': email.trim(),
          'FullName': fullName?.trim() ?? '',
          'IdToken': idToken ?? '',
        },
      );
      if (res.data is Map<String, dynamic>) {
        final loginRes = LoginResponse.fromJson(res.data);
        if (loginRes.success) {
          if (loginRes.token != null && loginRes.token!.isNotEmpty) {
            await TokenStorage.saveToken(loginRes.token!);
          }
          if (loginRes.user != null) {
            await TokenStorage.saveUser(loginRes.user!);
          } else {
            final user = await getCurrentUser();
            if (user != null) {
              await TokenStorage.saveUser(user);
            }
          }
          return loginRes;
        } else {
          return loginRes;
        }
      }
    } on DioException catch (dioErr) {
      if (dioErr.response?.data is Map<String, dynamic>) {
        final msg = dioErr.response?.data['message'] ?? dioErr.response?.data['Message'];
        if (msg != null) {
          return LoginResponse(success: false, message: msg.toString());
        }
      }
      return LoginResponse(success: false, message: dioErr.message ?? 'Lỗi kết nối máy chủ khi đăng nhập Google.');
    } catch (e) {
      return LoginResponse(success: false, message: e.toString().replaceFirst('Exception: ', ''));
    }
    return LoginResponse(success: false, message: 'Đăng nhập Google thất bại.');
  }

  /// Get Current User info from session/token
  Future<UserModel?> getCurrentUser() async {
    try {
      final res = await _client.get(ApiEndpoints.getCurrentUser);
      if (res.data is Map<String, dynamic>) {
        final data = res.data['data'] ?? res.data['Data'] ?? res.data;
        if (data is Map<String, dynamic>) {
          return UserModel.fromJson(data);
        }
      }
    } catch (_) {
      // Fail silently
    }
    return null;
  }

  /// Get Branches list
  Future<List<BranchModel>> getBranches() async {
    try {
      final res = await _client.get(ApiEndpoints.getAllBranches);
      if (res.data is List) {
        return (res.data as List).map((e) => BranchModel.fromJson(e)).toList();
      }
      if (res.data is Map<String, dynamic>) {
        final list = res.data['data'] ?? res.data['Data'] ?? res.data['branches'];
        if (list is List) {
          return list.map((e) => BranchModel.fromJson(e)).toList();
        }
      }
    } catch (_) {}
    return [];
  }

  /// Request OTP for Password Reset (POST api/admin/user-management/auth/request-otp)
  Future<bool> requestOtp(String email) async {
    try {
      final res = await _client.post(
        ApiEndpoints.requestOtp,
        data: {'Email': email.trim()},
      );
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return true;
    } catch (e) {
      throw Exception('Không thể gửi mã OTP: ${e.toString().replaceFirst("Exception: ", "")}');
    }
  }

  /// Reset Password with OTP (POST api/admin/user-management/auth/reset-password)
  Future<bool> resetPassword(String email, String otp, String newPassword) async {
    try {
      final res = await _client.post(
        ApiEndpoints.resetPassword,
        data: {
          'Email': email.trim(),
          'Otp': otp.trim(),
          'NewPassword': newPassword.trim(),
        },
      );
      if (res.data is Map<String, dynamic>) {
        final success = res.data['success'] ?? res.data['Success'] ?? false;
        if (!success) {
          throw Exception(res.data['message'] ?? res.data['Message'] ?? 'Mã OTP không chính xác hoặc đã hết hạn.');
        }
        return true;
      }
      return true;
    } catch (e) {
      throw Exception(e.toString().replaceFirst("Exception: ", ""));
    }
  }

  /// Sign out
  Future<void> signOut() async {
    try {
      await _client.post(ApiEndpoints.signOut);
    } catch (_) {}
    await TokenStorage.clear();
    await _client.cookieJar.deleteAll();
  }

  /// Get paged users
  Future<List<UserModel>> getUsers({String? keyword, int start = 0, int length = 50}) async {
    try {
      final queryParams = <String, dynamic>{
        'start': start,
        'length': length,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
      };
      final res = await _client.get(ApiEndpoints.pagedUsers, queryParameters: queryParams);
      dynamic raw = res.data;
      if (raw is Map<String, dynamic>) {
        raw = raw['data'] ?? raw['Data'] ?? raw['users'];
      }
      if (raw is List) {
        return raw.map((e) => UserModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Get list of system roles
  Future<List<RoleModel>> getRoles() async {
    try {
      final res = await _client.get(ApiEndpoints.userRoles);
      dynamic raw = res.data;
      if (raw is Map<String, dynamic>) {
        raw = raw['data'] ?? raw['Data'];
      }
      if (raw is List) {
        return raw.map((e) => RoleModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Create new user
  Future<bool> createUser(Map<String, dynamic> userData) async {
    try {
      final res = await _client.post(ApiEndpoints.createUser, data: userData);
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return res.statusCode == 200 || res.statusCode == 201;
    } catch (_) {
      return false;
    }
  }

  /// Update existing user
  Future<bool> updateUser(Map<String, dynamic> userData) async {
    try {
      final res = await _client.post(ApiEndpoints.updateUser, data: userData);
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return res.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  /// Delete user
  Future<bool> deleteUser(int userId) async {
    try {
      final res = await _client.delete('${ApiEndpoints.allUsers}/$userId');
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] ?? res.data['Success'] ?? true;
      }
      return res.statusCode == 200;
    } catch (_) {
      return false;
    }
  }

  /// Upload user avatar image file via multipart form
  Future<String?> uploadAvatar({required String filePath, String? fileName, int? userId}) async {
    try {
      final formData = FormData();
      formData.files.add(MapEntry(
        'file',
        await MultipartFile.fromFile(filePath, filename: fileName ?? 'avatar.jpg'),
      ));
      if (userId != null && userId > 0) {
        formData.fields.add(MapEntry('userId', userId.toString()));
      }

      final res = await _client.post(
        ApiEndpoints.uploadUserAvatar,
        data: formData,
      );

      if (res.data is Map<String, dynamic>) {
        return (res.data['url'] ?? res.data['Image'] ?? res.data['data'])?.toString();
      }
    } catch (e) {
      // Fallback
    }
    return null;
  }
}
