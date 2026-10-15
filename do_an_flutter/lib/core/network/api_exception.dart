import 'package:dio/dio.dart';
import '../config/app_config.dart';

enum ApiErrorType {
  unauthorized,       // 401: Chưa đăng nhập, token hết hạn, sai tài khoản/mật khẩu
  forbidden,          // 403: Không có quyền hạn truy cập chức năng này
  validation,         // 400: Thiếu thông tin, sai định dạng (ModelState validation)
  notFound,           // 404: Sai URL hoặc tài nguyên không tồn tại
  conflict,           // 409: Dữ liệu bị trùng lặp (trùng mã, email, username)
  serverCrash,        // 500: Lỗi code backend ASP.NET / Database SQL
  cannotConnect,      // Không kết nối được tới IP máy chủ / Backend chưa bật
  disconnected,       // Thiết bị mất kết nối mạng (Wifi/4G tắt)
  timeout,            // Quá thời gian chờ phản hồi
  sslError,           // Lỗi chứng chỉ bảo mật HTTPS
  formatError,        // Server trả về HTML trang lỗi thay vì JSON
  canceled,           // Request bị hủy
  unknown,            // Lỗi chưa xác định
}

class ApiException implements Exception {
  final String message;
  final int? statusCode;
  final ApiErrorType type;
  final String? technicalDetails;
  final Map<String, List<String>>? validationErrors;

  ApiException(
    this.message, {
    this.statusCode,
    this.type = ApiErrorType.unknown,
    this.technicalDetails,
    this.validationErrors,
  });

  /// Bóc tách chuỗi thông báo lỗi chi tiết từ nhiều cấu trúc JSON hoặc HTML của ASP.NET
  static String parseServerErrorMessage(dynamic data) {
    if (data == null) return '';

    // Trường hợp server trả về chuỗi trực tiếp (hoặc trang HTML)
    if (data is String) {
      final trimmed = data.trim();
      if (trimmed.startsWith('<!DOCTYPE') || trimmed.startsWith('<html')) {
        // Cố gắng bóc tách thẻ <title> hoặc <h1> trong HTML lỗi của IIS
        final titleMatch = RegExp(r'<title>(.*?)</title>', caseSensitive: false).firstMatch(trimmed);
        if (titleMatch != null) {
          return 'Máy chủ phản hồi trang web lỗi HTML: ${titleMatch.group(1)}';
        }
        return 'Máy chủ phản hồi trang lỗi HTML thay vì dữ liệu JSON.';
      }
      return trimmed;
    }

    if (data is Map<String, dynamic>) {
      // 1. Kiểm tra trường message thông thường
      final msg = data['message'] ?? data['Message'] ?? data['error'] ?? data['msg'] ?? data['title'];
      
      // 2. Kiểm tra validation errors từ ASP.NET Core (errors: { Field: ["error 1"] })
      if (data['errors'] is Map) {
        final errMap = data['errors'] as Map;
        final list = <String>[];
        errMap.forEach((k, v) {
          if (v is List && v.isNotEmpty) {
            list.add('$k: ${v.join(", ")}');
          } else if (v != null) {
            list.add('$k: $v');
          }
        });
        if (list.isNotEmpty) {
          final prefix = msg != null ? '$msg - ' : '';
          return '$prefix${list.join("; ")}';
        }
      }

      // 3. Kiểm tra ModelState từ ASP.NET Framework Web API (ModelState: { "model.Field": ["..."] })
      if (data['ModelState'] is Map) {
        final modelState = data['ModelState'] as Map;
        final list = <String>[];
        modelState.forEach((k, v) {
          final cleanKey = k.toString().replaceFirst(RegExp(r'^model\.'), '');
          if (v is List && v.isNotEmpty) {
            list.add('$cleanKey: ${v.join(", ")}');
          } else if (v != null) {
            list.add('$cleanKey: $v');
          }
        });
        if (list.isNotEmpty) {
          return list.join("; ");
        }
      }

      // 4. Kiểm tra ExceptionMessage / InnerException từ ASP.NET khi debug
      if (data['ExceptionMessage'] != null) {
        return 'Lỗi ngoại lệ Backend: ${data['ExceptionMessage']}';
      }

      if (msg != null) {
        return msg.toString();
      }
    }

    return '';
  }

  /// Trích xuất danh sách lỗi validation theo từng trường
  static Map<String, List<String>>? parseValidationFields(dynamic data) {
    if (data is! Map<String, dynamic>) return null;
    final result = <String, List<String>>{};

    if (data['errors'] is Map) {
      (data['errors'] as Map).forEach((k, v) {
        if (v is List) {
          result[k.toString()] = v.map((e) => e.toString()).toList();
        }
      });
    } else if (data['ModelState'] is Map) {
      (data['ModelState'] as Map).forEach((k, v) {
        final cleanKey = k.toString().replaceFirst(RegExp(r'^model\.'), '');
        if (v is List) {
          result[cleanKey] = v.map((e) => e.toString()).toList();
        }
      });
    }

    return result.isNotEmpty ? result : null;
  }

  factory ApiException.fromDioError(DioException error) {
    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        return ApiException(
          'Kết nối đến máy chủ quá thời gian chờ (Timeout). Kiểm tra xem mạng có chậm hoặc máy chủ có bị quá tải không.',
          statusCode: error.response?.statusCode,
          type: ApiErrorType.timeout,
          technicalDetails: error.message,
        );

      case DioExceptionType.connectionError:
        final errMsg = error.message ?? error.error?.toString() ?? '';
        if (errMsg.contains('SocketException') || errMsg.contains('Failed host lookup')) {
          return ApiException(
            'Thiết bị không có kết nối Internet hoặc không tìm thấy địa chỉ máy chủ (${AppConfig.emarketBaseUrl}). Vui lòng kiểm tra Wifi/4G.',
            statusCode: error.response?.statusCode,
            type: ApiErrorType.disconnected,
            technicalDetails: errMsg,
          );
        } else if (errMsg.contains('Connection refused')) {
          return ApiException(
            'Không thể kết nối đến máy chủ EMarket (${AppConfig.emarketBaseUrl}): Kết nối bị từ chối (Connection Refused). '
            'Vui lòng đảm bảo backend IIS/Visual Studio đang chạy và đúng cổng Port trong Cài đặt.',
            statusCode: error.response?.statusCode,
            type: ApiErrorType.cannotConnect,
            technicalDetails: errMsg,
          );
        }
        return ApiException(
          'Không thể kết nối đến máy chủ EMarket (${AppConfig.emarketBaseUrl}). '
          'Vui lòng kiểm tra địa chỉ IP máy chủ trong phần Cài đặt hệ thống.',
          statusCode: error.response?.statusCode,
          type: ApiErrorType.cannotConnect,
          technicalDetails: errMsg,
        );

      case DioExceptionType.badCertificate:
        return ApiException(
          'Lỗi chứng chỉ bảo mật HTTPS (SSL Handshake). Máy chủ sử dụng chứng chỉ tự ký hoặc chưa được tin cậy.',
          statusCode: error.response?.statusCode,
          type: ApiErrorType.sslError,
          technicalDetails: error.message,
        );

      case DioExceptionType.badResponse:
        final status = error.response?.statusCode;
        final data = error.response?.data;
        final serverMsg = parseServerErrorMessage(data);
        final valErrors = parseValidationFields(data);

        if (status == 400) {
          return ApiException(
            serverMsg.isNotEmpty 
                ? 'Dữ liệu không hợp lệ: $serverMsg' 
                : 'Yêu cầu không hợp lệ (400 Bad Request). Vui lòng kiểm tra các trường thông tin bắt buộc.',
            statusCode: 400,
            type: ApiErrorType.validation,
            technicalDetails: serverMsg,
            validationErrors: valErrors,
          );
        } else if (status == 401) {
          return ApiException(
            serverMsg.isNotEmpty
                ? serverMsg
                : 'Phiên đăng nhập đã hết hạn hoặc chưa được xác thực (401 Unauthorized). Vui lòng đăng nhập lại.',
            statusCode: 401,
            type: ApiErrorType.unauthorized,
            technicalDetails: serverMsg,
          );
        } else if (status == 403) {
          return ApiException(
            serverMsg.isNotEmpty
                ? serverMsg
                : 'Bạn không có quyền hạn (Permission) để thực hiện thao tác này (403 Forbidden).',
            statusCode: 403,
            type: ApiErrorType.forbidden,
            technicalDetails: serverMsg,
          );
        } else if (status == 404) {
          return ApiException(
            serverMsg.isNotEmpty
                ? serverMsg
                : 'Không tìm thấy dữ liệu hoặc đường dẫn API yêu cầu (404 Not Found: ${error.requestOptions.path}).',
            statusCode: 404,
            type: ApiErrorType.notFound,
            technicalDetails: serverMsg,
          );
        } else if (status == 409) {
          return ApiException(
            serverMsg.isNotEmpty
                ? serverMsg
                : 'Dữ liệu bị xung đột hoặc đã tồn tại trong hệ thống (409 Conflict).',
            statusCode: 409,
            type: ApiErrorType.conflict,
            technicalDetails: serverMsg,
          );
        } else if (status != null && status >= 500) {
          return ApiException(
            serverMsg.isNotEmpty
                ? 'Lỗi máy chủ nội bộ ($status): $serverMsg'
                : 'Máy chủ Backend EMarket gặp sự cố nội bộ ($status). Kiểm tra lại log backend hoặc cơ sở dữ liệu.',
            statusCode: status,
            type: ApiErrorType.serverCrash,
            technicalDetails: serverMsg,
          );
        }
        return ApiException(
          serverMsg.isNotEmpty ? serverMsg : 'Đã xảy ra lỗi khi trao đổi dữ liệu với máy chủ (Mã $status).',
          statusCode: status,
          type: ApiErrorType.unknown,
          technicalDetails: serverMsg,
        );

      case DioExceptionType.cancel:
        return ApiException(
          'Yêu cầu API đã bị hủy bỏ bởi ứng dụng.',
          type: ApiErrorType.canceled,
        );

      default:
        return ApiException(
          'Lỗi kết nối mạng: ${error.message ?? "Không xác định"}',
          statusCode: error.response?.statusCode,
          type: ApiErrorType.unknown,
          technicalDetails: error.error?.toString(),
        );
    }
  }

  @override
  String toString() => message;
}
