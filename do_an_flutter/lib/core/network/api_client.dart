import 'dart:io';
import 'package:cookie_jar/cookie_jar.dart';
import 'package:dio/dio.dart';
import 'package:dio/io.dart';
import 'package:dio_cookie_manager/dio_cookie_manager.dart';
import 'package:flutter/foundation.dart';
import '../config/app_config.dart';
import 'api_exception.dart';
import 'network_inspector_server.dart';
import 'token_storage.dart';

class ApiClient {
  static final ApiClient _instance = ApiClient._internal();
  factory ApiClient() => _instance;

  late Dio _dio;
  late Dio _aiDio;
  final CookieJar _cookieJar = CookieJar();

  Dio get dio => _dio;
  Dio get aiDio => _aiDio;
  CookieJar get cookieJar => _cookieJar;

  ApiClient._internal() {
    _initEmarketDio();
    _initAiDio();
  }

  /// Tạo interceptor chuẩn kết nối với NetworkInspectorServer & Token Auth
  Interceptor _createInspectorInterceptor({required String label}) {
    return InterceptorsWrapper(
      onRequest: (options, handler) async {
        options.extra['_start_time'] = DateTime.now().millisecondsSinceEpoch;

        // Tự động đính kèm Token JWT nếu đã đăng nhập
        final token = await TokenStorage.getToken();
        if (token != null && token.isNotEmpty) {
          options.headers['Authorization'] = 'Bearer $token';
        }

        // Ghi nhận vào Network Inspector Server (Web Dashboard)
        try {
          final entry = NetworkInspectorServer().logRequest(options);
          options.extra['_inspector_id'] = entry.id;
        } catch (_) {}

        return handler.next(options);
      },
      onResponse: (response, handler) {
        final startTime = response.requestOptions.extra['_start_time'] as int?;
        final durationMs = startTime != null
            ? DateTime.now().millisecondsSinceEpoch - startTime
            : 0;

        // Cập nhật kết quả vào Web Dashboard & Console
        try {
          NetworkInspectorServer().logResponse(response, durationMs);
        } catch (_) {}

        return handler.next(response);
      },
      onError: (DioException error, handler) {
        final startTime = error.requestOptions.extra['_start_time'] as int?;
        final durationMs = startTime != null
            ? DateTime.now().millisecondsSinceEpoch - startTime
            : 0;

        // Cập nhật lỗi chi tiết vào Web Dashboard & Console
        try {
          NetworkInspectorServer().logError(error, durationMs);
        } catch (_) {}

        return handler.next(error);
      },
    );
  }

  void _initEmarketDio() {
    _dio = Dio(
      BaseOptions(
        baseUrl: AppConfig.emarketBaseUrl,
        connectTimeout: const Duration(seconds: 20),
        receiveTimeout: const Duration(seconds: 45),
        headers: {
          'Accept': 'application/json',
          'User-Agent': 'EMarketFlutterClient',
        },
        followRedirects: true,
        maxRedirects: 5,
        // Chuẩn HTTP: 2xx là thành công, 4xx và 5xx là lỗi cần xử lý
        validateStatus: (status) => status != null && status >= 200 && status < 300,
      ),
    );

    // Cookie Manager cho ASP.NET OWIN Session & Anti-Forgery cookies
    _dio.interceptors.add(CookieManager(_cookieJar));

    // Bộ thanh tra mạng & Hứng lỗi Realtime
    _dio.interceptors.add(_createInspectorInterceptor(label: 'EMARKET'));

    // Bypass SSL certificate errors cho môi trường phát triển cục bộ (Localhost HTTPS)
    if (!kIsWeb) {
      _dio.httpClientAdapter = IOHttpClientAdapter(
        createHttpClient: () {
          final client = HttpClient();
          client.badCertificateCallback = (X509Certificate cert, String host, int port) => true;
          return client;
        },
      );
    }
  }

  void _initAiDio() {
    _aiDio = Dio(
      BaseOptions(
        baseUrl: AppConfig.aiBaseUrl,
        connectTimeout: const Duration(seconds: 25),
        receiveTimeout: const Duration(seconds: 45),
        headers: {
          'Accept': 'application/json',
          'User-Agent': 'EMarketFlutterClient',
        },
        followRedirects: true,
        maxRedirects: 5,
        validateStatus: (status) => status != null && status >= 200 && status < 300,
      ),
    );

    // Bypass SSL certificate errors cho AI API
    if (!kIsWeb) {
      _aiDio.httpClientAdapter = IOHttpClientAdapter(
        createHttpClient: () {
          final client = HttpClient();
          client.badCertificateCallback = (X509Certificate cert, String host, int port) => true;
          return client;
        },
      );
    }

    _aiDio.interceptors.add(_createInspectorInterceptor(label: 'AI_BACKEND'));
  }

  /// Nạp lại Base URLs khi người dùng thay đổi IP trong phần Cài đặt
  void refreshBaseUrls() {
    _dio.options.baseUrl = AppConfig.emarketBaseUrl;
    _aiDio.options.baseUrl = AppConfig.aiBaseUrl;
  }

  // Generic Request Handlers với cơ chế ném ngoại lệ phân tích chi tiết
  Future<Response> get(
    String path, {
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final res = await _dio.get(path, queryParameters: queryParameters, options: options);
      _ensureSuccessResponse(res);
      return res;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<Response> post(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final res = await _dio.post(path, data: data, queryParameters: queryParameters, options: options);
      _ensureSuccessResponse(res);
      return res;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<Response> put(
    String path, {
    dynamic data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final res = await _dio.put(path, data: data, queryParameters: queryParameters, options: options);
      _ensureSuccessResponse(res);
      return res;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<Response> delete(
    String path, {
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) async {
    try {
      final res = await _dio.delete(path, queryParameters: queryParameters, options: options);
      _ensureSuccessResponse(res);
      return res;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  /// Bảo đảm nếu response trả về status >= 400 (do custom validateStatus) thì vẫn ném lỗi rõ ràng
  void _ensureSuccessResponse(Response res) {
    if (res.statusCode != null && res.statusCode! >= 400) {
      throw ApiException.fromDioError(
        DioException(
          requestOptions: res.requestOptions,
          response: res,
          type: DioExceptionType.badResponse,
        ),
      );
    }
  }
}
