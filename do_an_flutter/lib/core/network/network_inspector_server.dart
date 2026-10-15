// ignore_for_file: avoid_print
import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:dio/dio.dart';
import 'api_exception.dart';

/// Đại diện cho một giao dịch API mạng kèm đầy đủ dữ liệu phân tích
class NetworkLogEntry {
  final String id;
  final DateTime timestamp;
  final String method;
  final String url;
  final String path;
  final Map<String, dynamic> requestHeaders;
  final Map<String, dynamic> queryParams;
  final dynamic requestBody;
  
  int? statusCode;
  String? statusMessage;
  int? durationMs;
  dynamic responseBody;
  Map<String, dynamic>? responseHeaders;
  
  bool isError;
  String errorCategory;
  String? errorSummary;
  String? errorDetails;
  String curlCommand;

  NetworkLogEntry({
    required this.id,
    required this.timestamp,
    required this.method,
    required this.url,
    required this.path,
    required this.requestHeaders,
    required this.queryParams,
    required this.requestBody,
    this.statusCode,
    this.statusMessage,
    this.durationMs,
    this.responseBody,
    this.responseHeaders,
    this.isError = false,
    this.errorCategory = 'NONE',
    this.errorSummary,
    this.errorDetails,
    required this.curlCommand,
  });

  Map<String, dynamic> toJson() => {
    'id': id,
    'timestamp': timestamp.toIso8601String(),
    'timeFormatted': '${timestamp.hour.toString().padLeft(2, '0')}:${timestamp.minute.toString().padLeft(2, '0')}:${timestamp.second.toString().padLeft(2, '0')}',
    'method': method,
    'url': url,
    'path': path,
    'requestHeaders': requestHeaders,
    'queryParams': queryParams,
    'requestBody': requestBody,
    'statusCode': statusCode,
    'statusMessage': statusMessage,
    'durationMs': durationMs,
    'responseBody': responseBody,
    'responseHeaders': responseHeaders,
    'isError': isError,
    'errorCategory': errorCategory,
    'errorSummary': errorSummary,
    'errorDetails': errorDetails,
    'curlCommand': curlCommand,
  };
}

/// Trình phục vụ Web Dashboard & WebSocket hứng log realtime
class NetworkInspectorServer {
  static final NetworkInspectorServer _instance = NetworkInspectorServer._internal();
  factory NetworkInspectorServer() => _instance;
  NetworkInspectorServer._internal();

  static int defaultPort = 8088;
  int _actualPort = 8088;
  int get actualPort => _actualPort;
  HttpServer? _server;
  final List<NetworkLogEntry> _logs = [];
  final List<WebSocket> _clients = [];
  static const int _maxLogs = 200;

  bool get isRunning => _server != null;
  List<NetworkLogEntry> get logs => List.unmodifiable(_logs);

  /// Khởi động server HTTP & WebSocket cục bộ
  Future<void> start() async {
    if (_server != null) return;
    
    // Thử mở cổng 8088, nếu bận thử tiếp 8089 hoặc 8090
    final candidatePorts = [8088, 8089, 8090, 8888];
    for (final p in candidatePorts) {
      try {
        _server = await HttpServer.bind(InternetAddress.anyIPv4, p);
        _actualPort = p;
        break;
      } catch (_) {
        // Cổng đang bận, thử cổng tiếp theo
      }
    }

    if (_server == null) {
      print('⚠️ [INSPECTOR] Không thể mở bất kỳ cổng nào trong danh sách: $candidatePorts');
      return;
    }

    print('\n======================================================================');
    print('🚀 [EMARKET NETWORK INSPECTOR] Bộ giám sát API & hứng lỗi Realtime');
    print('👉 Mở trình duyệt Web xem trực quan tại: http://localhost:$_actualPort');
    print('   (Hoặc: http://127.0.0.1:$_actualPort)');
    print('👉 Nếu dùng Android Emulator: Chạy lệnh \'adb reverse tcp:$_actualPort tcp:$_actualPort\'');
    print('======================================================================\n');

    _server!.listen((HttpRequest request) async {
      try {
        if (WebSocketTransformer.isUpgradeRequest(request)) {
          final socket = await WebSocketTransformer.upgrade(request);
          _handleWebSocket(socket);
        } else {
          await _handleHttpRequest(request);
        }
      } catch (e) {
        print('[INSPECTOR] Lỗi xử lý HTTP request: $e');
      }
    });
  }

  void _handleWebSocket(WebSocket socket) {
    _clients.add(socket);
    // Gửi toàn bộ logs hiện có ngay khi client kết nối
    final initialData = jsonEncode({
      'type': 'INIT',
      'logs': _logs.map((e) => e.toJson()).toList(),
    });
    socket.add(initialData);

    socket.listen(
      (data) {
        try {
          final msg = jsonDecode(data.toString());
          if (msg['action'] == 'CLEAR') {
            clearLogs();
          }
        } catch (_) {}
      },
      onDone: () => _clients.remove(socket),
      onError: (_) => _clients.remove(socket),
    );
  }

  Future<void> _handleHttpRequest(HttpRequest request) async {
    final response = request.response;
    response.headers.add('Access-Control-Allow-Origin', '*');
    response.headers.add('Access-Control-Allow-Methods', 'GET, POST, OPTIONS');
    response.headers.add('Access-Control-Allow-Headers', '*');

    if (request.method == 'OPTIONS') {
      response.statusCode = HttpStatus.ok;
      await response.close();
      return;
    }

    final path = request.uri.path;

    if (path == '/' || path == '/index.html') {
      response.headers.contentType = ContentType.html;
      response.write(_buildWebDashboardHtml());
      await response.close();
    } else if (path == '/api/logs') {
      response.headers.contentType = ContentType.json;
      response.write(jsonEncode(_logs.map((e) => e.toJson()).toList()));
      await response.close();
    } else if (path == '/api/clear') {
      clearLogs();
      response.headers.contentType = ContentType.json;
      response.write(jsonEncode({'success': true}));
      await response.close();
    } else {
      response.statusCode = HttpStatus.notFound;
      response.write('Not found');
      await response.close();
    }
  }

  /// Ghi nhận 1 request mới bắt đầu
  NetworkLogEntry logRequest(RequestOptions options) {
    final id = '${DateTime.now().millisecondsSinceEpoch}_${options.uri.hashCode}';
    final curl = _generateCurl(options);
    
    final entry = NetworkLogEntry(
      id: id,
      timestamp: DateTime.now(),
      method: options.method.toUpperCase(),
      url: options.uri.toString(),
      path: options.uri.path.isEmpty ? '/' : options.uri.path,
      requestHeaders: Map<String, dynamic>.from(options.headers),
      queryParams: Map<String, dynamic>.from(options.queryParameters),
      requestBody: _sanitizeBody(options.data),
      curlCommand: curl,
    );

    _logs.insert(0, entry);
    if (_logs.length > _maxLogs) {
      _logs.removeLast();
    }

    _broadcast({
      'type': 'REQUEST',
      'entry': entry.toJson(),
    });

    return entry;
  }

  /// Cập nhật kết quả khi request thành công hoặc có phản hồi
  void logResponse(Response response, int durationMs) {
    final id = response.requestOptions.extra['_inspector_id']?.toString();
    final entry = _findEntry(id, response.requestOptions.uri.toString());
    if (entry == null) return;

    entry.statusCode = response.statusCode;
    entry.statusMessage = response.statusMessage ?? _getStatusMessage(response.statusCode);
    entry.durationMs = durationMs;
    entry.responseHeaders = response.headers.map.map((k, v) => MapEntry(k, v.join(', ')));
    entry.responseBody = _sanitizeBody(response.data);

    final status = response.statusCode ?? 200;
    if (status >= 400) {
      entry.isError = true;
      _categorizeHttpError(entry, status, response.data);
    } else {
      entry.isError = false;
      entry.errorCategory = 'SUCCESS';
    }

    _broadcast({
      'type': 'RESPONSE',
      'entry': entry.toJson(),
    });

    _printConsoleSummary(entry);
  }

  /// Cập nhật kết quả khi request gặp lỗi ném ra ngoại lệ
  void logError(DioException error, int durationMs) {
    final id = error.requestOptions.extra['_inspector_id']?.toString();
    final entry = _findEntry(id, error.requestOptions.uri.toString());
    if (entry == null) return;

    entry.durationMs = durationMs;
    entry.isError = true;

    if (error.response != null) {
      entry.statusCode = error.response?.statusCode;
      entry.statusMessage = error.response?.statusMessage ?? _getStatusMessage(entry.statusCode);
      entry.responseHeaders = error.response?.headers.map.map((k, v) => MapEntry(k, v.join(', ')));
      entry.responseBody = _sanitizeBody(error.response?.data);
      _categorizeHttpError(entry, entry.statusCode ?? 0, error.response?.data);
    } else {
      entry.statusCode = 0;
      _categorizeDioException(entry, error);
    }

    _broadcast({
      'type': 'ERROR',
      'entry': entry.toJson(),
    });

    _printConsoleSummary(entry);
  }

  NetworkLogEntry? _findEntry(String? id, String url) {
    if (id != null) {
      try {
        return _logs.firstWhere((e) => e.id == id);
      } catch (_) {}
    }
    try {
      return _logs.firstWhere((e) => e.url == url);
    } catch (_) {
      return null;
    }
  }

  void clearLogs() {
    _logs.clear();
    _broadcast({'type': 'CLEAR'});
  }

  void _broadcast(Map<String, dynamic> data) {
    final jsonStr = jsonEncode(data);
    final deadClients = <WebSocket>[];
    for (final client in _clients) {
      try {
        client.add(jsonStr);
      } catch (_) {
        deadClients.add(client);
      }
    }
    _clients.removeWhere((c) => deadClients.contains(c));
  }

  /// Phân loại lỗi HTTP chuyên sâu (400, 401, 403, 404, 409, 500...)
  void _categorizeHttpError(NetworkLogEntry entry, int status, dynamic responseData) {
    final parsed = ApiException.parseServerErrorMessage(responseData);

    switch (status) {
      case 400:
        entry.errorCategory = 'VALIDATION_ERROR';
        entry.errorSummary = 'Dữ liệu gửi lên không hợp lệ hoặc thiếu thông tin (400 Bad Request)';
        entry.errorDetails = parsed.isNotEmpty ? parsed : 'Server ASP.NET từ chối do dữ liệu không khớp ModelState hoặc thiếu trường bắt buộc.';
        break;

      case 401:
        entry.errorCategory = 'AUTH_EXPIRED';
        entry.errorSummary = 'Chưa đăng nhập hoặc phiên làm việc đã hết hạn (401 Unauthorized)';
        entry.errorDetails = parsed.isNotEmpty ? parsed : 'Token JWT bị thiếu, không hợp lệ hoặc hết hạn. Vui lòng đăng nhập lại tài khoản.';
        break;

      case 403:
        entry.errorCategory = 'FORBIDDEN';
        entry.errorSummary = 'Bị từ chối truy cập do không đủ quyền hạn (403 Forbidden)';
        entry.errorDetails = parsed.isNotEmpty ? parsed : 'Tài khoản hiện tại không có quyền hạn (Permission/Role) để gọi API này.';
        break;

      case 404:
        entry.errorCategory = 'NOT_FOUND';
        entry.errorSummary = 'Không tìm thấy API hoặc tài nguyên trên máy chủ (404 Not Found)';
        entry.errorDetails = 'Đường dẫn [${entry.path}] không tồn tại trên backend EMarket. Kiểm tra lại routing trong Controller.';
        break;

      case 409:
        entry.errorCategory = 'CONFLICT';
        entry.errorSummary = 'Xung đột dữ liệu (409 Conflict)';
        entry.errorDetails = parsed.isNotEmpty ? parsed : 'Dữ liệu đã tồn tại (ví dụ trùng Username, Email hoặc Mã sản phẩm).';
        break;

      case 500:
      case 502:
      case 503:
        entry.errorCategory = 'SERVER_CRASH';
        entry.errorSummary = 'Máy chủ Backend gặp sự cố nội bộ ($status Server Error)';
        entry.errorDetails = parsed.isNotEmpty ? parsed : 'Code backend ASP.NET bị Exception (NullReference, lỗi truy vấn SQL Server, hoặc crash database).';
        break;

      default:
        entry.errorCategory = 'HTTP_ERROR';
        entry.errorSummary = 'Lỗi trao đổi dữ liệu với máy chủ (Mã $status)';
        entry.errorDetails = parsed.isNotEmpty ? parsed : 'HTTP status: $status';
        break;
    }
  }

  /// Phân loại lỗi ở tầng mạng & kết nối (Timeout, Mất mạng, Sai IP, SSL)
  void _categorizeDioException(NetworkLogEntry entry, DioException error) {
    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
        entry.errorCategory = 'TIMEOUT';
        entry.errorSummary = 'Quá thời gian chờ phản hồi (Timeout)';
        entry.errorDetails = 'Thời gian kết nối hoặc nhận dữ liệu vượt quá giới hạn cấu hình. Kiểm tra xem máy chủ có đang bị treo hay mạng quá chậm.';
        break;

      case DioExceptionType.connectionError:
        entry.errorCategory = 'CANNOT_CONNECT_SERVER';
        final errMsg = error.message ?? error.error?.toString() ?? '';
        if (errMsg.contains('SocketException') || errMsg.contains('Failed host lookup')) {
          entry.errorSummary = 'Không thể phân giải tên miền hoặc thiết bị mất kết nối mạng hoàn toàn';
          entry.errorDetails = 'Vui lòng kiểm tra Wifi/4G hoặc đảm bảo thiết bị có thể kết nối ra ngoài.';
        } else if (errMsg.contains('Connection refused')) {
          entry.errorSummary = 'Kết nối bị từ chối (Connection Refused)';
          entry.errorDetails = 'Máy chủ backend EMarket chưa được bật hoặc sai cổng port trong phần Cài đặt IP.';
        } else {
          entry.errorSummary = 'Không thể kết nối đến máy chủ backend';
          entry.errorDetails = 'Chi tiết: $errMsg\nGợi ý: Kiểm tra lại IP máy chủ trong ServerSettingsDialog (ví dụ: dùng IP Wifi thật của máy tính thay vì localhost nếu chạy điện thoại thật).';
        }
        break;

      case DioExceptionType.badCertificate:
        entry.errorCategory = 'SSL_ERROR';
        entry.errorSummary = 'Lỗi chứng chỉ bảo mật HTTPS (SSL Certificate)';
        entry.errorDetails = 'Chứng chỉ SSL tự ký không được tin cậy hoặc hết hạn. Đảm bảo cấu hình bypass certificate cho môi trường dev.';
        break;

      case DioExceptionType.cancel:
        entry.errorCategory = 'CANCEL';
        entry.errorSummary = 'Yêu cầu API đã bị hủy chủ động bởi ứng dụng';
        entry.errorDetails = 'CancelToken đã kích hoạt hủy bỏ request.';
        break;

      default:
        entry.errorCategory = 'NETWORK_ERROR';
        entry.errorSummary = 'Lỗi kết nối mạng không xác định';
        entry.errorDetails = error.message ?? error.toString();
        break;
    }
  }

  void _printConsoleSummary(NetworkLogEntry entry) {
    final ms = entry.durationMs != null ? '${entry.durationMs}ms' : '';
    if (entry.isError) {
      print('🔴 [DIO ERR ${entry.statusCode ?? 0}] ${entry.method} ${entry.path} ($ms) => [${entry.errorCategory}] ${entry.errorSummary}');
      if (entry.errorDetails != null && entry.errorDetails!.isNotEmpty) {
        print('   ↳ Chi tiết: ${entry.errorDetails}');
      }
    } else {
      print('🟢 [DIO OK ${entry.statusCode}] ${entry.method} ${entry.path} ($ms)');
    }
  }

  String _generateCurl(RequestOptions options) {
    final buffer = StringBuffer('curl -X ${options.method.toUpperCase()} "${options.uri}"');
    options.headers.forEach((key, value) {
      if (value != null) {
        buffer.write(' -H "$key: $value"');
      }
    });

    if (options.data != null) {
      if (options.data is FormData) {
        buffer.write(' [FormData Payload]');
      } else if (options.data is Map || options.data is List) {
        try {
          final jsonStr = jsonEncode(options.data).replaceAll('"', '\\"');
          buffer.write(' -d "$jsonStr"');
        } catch (_) {}
      } else {
        buffer.write(' -d "${options.data.toString().replaceAll('"', '\\"')}"');
      }
    }
    return buffer.toString();
  }

  dynamic _sanitizeBody(dynamic body) {
    if (body == null) return null;
    if (body is FormData) {
      final map = <String, dynamic>{};
      for (final field in body.fields) {
        map[field.key] = field.value;
      }
      for (final file in body.files) {
        map[file.key] = '[File: ${file.value.filename} (${file.value.length} bytes)]';
      }
      return map;
    }
    if (body is Map || body is List || body is num || body is bool || body is String) {
      return body;
    }
    try {
      return body.toString();
    } catch (_) {
      return '[Unparseable Data]';
    }
  }

  String _getStatusMessage(int? code) {
    switch (code) {
      case 200: return 'OK';
      case 201: return 'Created';
      case 204: return 'No Content';
      case 400: return 'Bad Request';
      case 401: return 'Unauthorized';
      case 403: return 'Forbidden';
      case 404: return 'Not Found';
      case 409: return 'Conflict';
      case 500: return 'Internal Server Error';
      case 502: return 'Bad Gateway';
      case 503: return 'Service Unavailable';
      default: return code != null ? 'HTTP $code' : 'Unknown';
    }
  }

  /// Giao diện Web Dashboard HTML/CSS/JS độc lập (Dark Mode siêu nét)
  String _buildWebDashboardHtml() {
    return '''<!DOCTYPE html>
<html lang="vi">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>EMarket API & Error Live Monitor</title>
  <style>
    :root {
      --bg-dark: #0f172a;
      --bg-card: #1e293b;
      --bg-hover: #334155;
      --border-color: #334155;
      --text-main: #f8fafc;
      --text-muted: #94a3b8;
      --primary: #6366f1;
      --success: #10b981;
      --warning: #f59e0b;
      --danger: #ef4444;
      --info: #0ea5e9;
      --method-get: #0ea5e9;
      --method-post: #10b981;
      --method-put: #f59e0b;
      --method-delete: #ef4444;
    }
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body {
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
      background: var(--bg-dark);
      color: var(--text-main);
      height: 100vh;
      display: flex;
      flex-direction: column;
      overflow: hidden;
    }
    header {
      background: var(--bg-card);
      border-bottom: 1px solid var(--border-color);
      padding: 12px 20px;
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 16px;
      z-index: 10;
    }
    .header-title {
      display: flex;
      align-items: center;
      gap: 12px;
      font-weight: 700;
      font-size: 1.15rem;
      color: #fff;
    }
    .status-pulse {
      width: 10px;
      height: 10px;
      border-radius: 50%;
      background: var(--success);
      box-shadow: 0 0 10px var(--success);
      animation: pulse 1.5s infinite;
    }
    @keyframes pulse {
      0% { transform: scale(0.95); opacity: 0.8; }
      50% { transform: scale(1.2); opacity: 1; }
      100% { transform: scale(0.95); opacity: 0.8; }
    }
    .header-controls {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    input[type="text"] {
      background: #0f172a;
      border: 1px solid var(--border-color);
      border-radius: 6px;
      padding: 7px 12px;
      color: #fff;
      font-size: 0.85rem;
      width: 260px;
      outline: none;
    }
    input[type="text"]:focus {
      border-color: var(--primary);
    }
    .btn {
      background: #334155;
      color: #fff;
      border: none;
      border-radius: 6px;
      padding: 7px 14px;
      font-size: 0.85rem;
      font-weight: 600;
      cursor: pointer;
      display: flex;
      align-items: center;
      gap: 6px;
      transition: all 0.15s;
    }
    .btn:hover { background: #475569; }
    .btn-danger { background: rgba(239, 68, 68, 0.2); color: #f87171; border: 1px solid rgba(239, 68, 68, 0.4); }
    .btn-danger:hover { background: rgba(239, 68, 68, 0.35); }
    .filter-btn {
      font-size: 0.78rem;
      padding: 5px 10px;
      border-radius: 20px;
      cursor: pointer;
      background: #1e293b;
      border: 1px solid var(--border-color);
      color: var(--text-muted);
    }
    .filter-btn.active {
      background: var(--primary);
      color: #fff;
      border-color: var(--primary);
    }

    /* Main Container 2 Columns */
    .main-container {
      display: flex;
      flex: 1;
      overflow: hidden;
    }
    .sidebar {
      width: 440px;
      border-right: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
      background: #0f172a;
    }
    .sidebar-filter-bar {
      padding: 10px 14px;
      border-bottom: 1px solid var(--border-color);
      display: flex;
      gap: 8px;
      background: #141e33;
    }
    .log-list {
      flex: 1;
      overflow-y: auto;
      list-style: none;
    }
    .log-item {
      padding: 12px 14px;
      border-bottom: 1px solid rgba(51, 65, 85, 0.4);
      cursor: pointer;
      display: flex;
      flex-direction: column;
      gap: 6px;
      transition: background 0.1s;
    }
    .log-item:hover { background: var(--bg-hover); }
    .log-item.active {
      background: #1e293b;
      border-left: 4px solid var(--primary);
    }
    .log-item-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    .badge {
      font-size: 0.72rem;
      font-weight: 700;
      padding: 2px 7px;
      border-radius: 4px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .badge-get { background: rgba(14, 165, 233, 0.2); color: var(--method-get); }
    .badge-post { background: rgba(16, 185, 129, 0.2); color: var(--method-post); }
    .badge-put { background: rgba(245, 158, 11, 0.2); color: var(--method-put); }
    .badge-delete { background: rgba(239, 68, 68, 0.2); color: var(--method-delete); }
    
    .status-badge {
      font-size: 0.72rem;
      font-weight: 700;
      padding: 2px 8px;
      border-radius: 4px;
    }
    .status-2xx { background: rgba(16, 185, 129, 0.2); color: var(--success); }
    .status-3xx { background: rgba(14, 165, 233, 0.2); color: var(--info); }
    .status-4xx { background: rgba(245, 158, 11, 0.2); color: var(--warning); }
    .status-5xx, .status-err { background: rgba(239, 68, 68, 0.25); color: var(--danger); font-weight: 800; }
    .status-pending { background: #334155; color: #cbd5e1; }

    .log-item-path {
      font-size: 0.88rem;
      font-weight: 600;
      color: #e2e8f0;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    .log-item-footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
      font-size: 0.75rem;
      color: var(--text-muted);
    }
    .log-item-err-tag {
      color: var(--danger);
      font-weight: 600;
      font-size: 0.74rem;
    }

    /* Detail View */
    .detail-view {
      flex: 1;
      display: flex;
      flex-direction: column;
      background: var(--bg-card);
      overflow-y: auto;
    }
    .empty-state {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      color: var(--text-muted);
      gap: 12px;
    }
    .empty-icon { font-size: 3rem; opacity: 0.4; }
    .detail-header {
      padding: 16px 20px;
      border-bottom: 1px solid var(--border-color);
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      background: #141e33;
    }
    .detail-url-title {
      font-size: 1.05rem;
      font-weight: 700;
      word-break: break-all;
      margin-bottom: 8px;
    }
    .detail-meta-tags {
      display: flex;
      gap: 10px;
      align-items: center;
      font-size: 0.8rem;
      color: var(--text-muted);
    }

    /* Error Alert Box */
    .error-alert-box {
      margin: 16px 20px 0 20px;
      padding: 14px 16px;
      background: rgba(239, 68, 68, 0.12);
      border: 1px solid rgba(239, 68, 68, 0.4);
      border-radius: 8px;
      color: #fca5a5;
    }
    .error-alert-title {
      font-weight: 700;
      font-size: 0.95rem;
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 6px;
      color: #ef4444;
    }
    .error-alert-desc {
      font-size: 0.85rem;
      line-height: 1.4;
      color: #fee2e2;
    }

    /* Tabs */
    .tab-bar {
      display: flex;
      border-bottom: 1px solid var(--border-color);
      padding: 0 20px;
      background: #1e293b;
      margin-top: 14px;
    }
    .tab-btn {
      padding: 10px 16px;
      background: none;
      border: none;
      color: var(--text-muted);
      font-size: 0.85rem;
      font-weight: 600;
      cursor: pointer;
      border-bottom: 2px solid transparent;
      transition: all 0.15s;
    }
    .tab-btn:hover { color: #fff; }
    .tab-btn.active {
      color: var(--primary);
      border-bottom: 2px solid var(--primary);
    }

    .tab-content {
      padding: 20px;
      flex: 1;
    }
    .code-box {
      background: #090d16;
      border: 1px solid var(--border-color);
      border-radius: 8px;
      padding: 14px;
      font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
      font-size: 0.82rem;
      line-height: 1.5;
      color: #cbd5e1;
      white-space: pre-wrap;
      word-break: break-all;
      max-height: 400px;
      overflow-y: auto;
    }
    .section-label {
      font-size: 0.82rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.6px;
      color: var(--text-muted);
      margin: 14px 0 6px 0;
    }
    .section-label:first-child { margin-top: 0; }
    .copy-btn {
      float: right;
      padding: 3px 8px;
      font-size: 0.72rem;
      background: #334155;
      color: #cbd5e1;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }
    .copy-btn:hover { background: #475569; color: #fff; }
  </style>
</head>
<body>

  <header>
    <div class="header-title">
      <div class="status-pulse"></div>
      <span>EMarket Network & Error Monitor</span>
      <span style="font-size: 0.75rem; background: #334155; padding: 2px 8px; border-radius: 12px; color: #94a3b8;">Port $_actualPort</span>
    </div>

    <div class="header-controls">
      <input type="text" id="searchInput" placeholder="Tìm theo đường dẫn, status hoặc từ khóa...">
      <button class="btn btn-danger" onclick="clearAllLogs()">Xóa logs</button>
    </div>
  </header>

  <div class="main-container">
    <!-- Cột trái: Danh sách log -->
    <div class="sidebar">
      <div class="sidebar-filter-bar">
        <button class="filter-btn active" data-filter="ALL" onclick="setFilter('ALL')">Tất cả (<span id="countAll">0</span>)</button>
        <button class="filter-btn" data-filter="ERROR" onclick="setFilter('ERROR')">Chỉ Lỗi (<span id="countError" style="color:#ef4444;">0</span>)</button>
        <button class="filter-btn" data-filter="2XX" onclick="setFilter('2XX')">2xx Thành công</button>
      </div>

      <ul class="log-list" id="logList">
        <!-- Rendered via JS -->
      </ul>
    </div>

    <!-- Cột phải: Chi tiết giao dịch đã chọn -->
    <div class="detail-view" id="detailView">
      <div class="empty-state">
        <div class="empty-icon">📡</div>
        <p>Chưa chọn yêu cầu nào. Hãy bấm vào một mục ở cột bên trái để phân tích.</p>
      </div>
    </div>
  </div>

  <script>
    let logs = [];
    let selectedId = null;
    let currentFilter = 'ALL';
    let currentTab = 'RESPONSE';
    let socket = null;

    function initWebSocket() {
      const loc = window.location;
      const wsUri = (loc.protocol === "https:" ? "wss:" : "ws:") + "//" + loc.host + "/ws";
      socket = new WebSocket(wsUri);

      socket.onmessage = function(event) {
        try {
          const msg = JSON.parse(event.data);
          if (msg.type === 'INIT') {
            logs = msg.logs || [];
            renderList();
          } else if (msg.type === 'REQUEST') {
            logs.unshift(msg.entry);
            renderList();
          } else if (msg.type === 'RESPONSE' || msg.type === 'ERROR') {
            const idx = logs.findIndex(e => e.id === msg.entry.id);
            if (idx >= 0) {
              logs[idx] = msg.entry;
            } else {
              logs.unshift(msg.entry);
            }
            renderList();
            if (selectedId === msg.entry.id) {
              renderDetail(msg.entry);
            }
          } else if (msg.type === 'CLEAR') {
            logs = [];
            selectedId = null;
            renderList();
            renderDetail(null);
          }
        } catch(e) {
          console.error(e);
        }
      };

      socket.onclose = function() {
        setTimeout(initWebSocket, 2000);
      };
    }

    function setFilter(filter) {
      currentFilter = filter;
      document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.filter === filter);
      });
      renderList();
    }

    document.getElementById('searchInput').addEventListener('input', () => {
      renderList();
    });

    function getFilteredLogs() {
      const q = document.getElementById('searchInput').value.trim().toLowerCase();
      return logs.filter(item => {
        if (currentFilter === 'ERROR' && !item.isError) return false;
        if (currentFilter === '2XX' && (item.statusCode < 200 || item.statusCode >= 300)) return false;

        if (q) {
          const matchUrl = item.url.toLowerCase().includes(q);
          const matchStatus = String(item.statusCode || '').includes(q);
          const matchErr = (item.errorSummary || '').toLowerCase().includes(q) || (item.errorDetails || '').toLowerCase().includes(q);
          return matchUrl || matchStatus || matchErr;
        }
        return true;
      });
    }

    function renderList() {
      const listEl = document.getElementById('logList');
      const filtered = getFilteredLogs();

      document.getElementById('countAll').innerText = logs.length;
      document.getElementById('countError').innerText = logs.filter(l => l.isError).length;

      listEl.innerHTML = '';
      if (filtered.length === 0) {
        listEl.innerHTML = '<li style="padding:30px; text-align:center; color:#64748b; font-size:0.85rem;">Không có bản ghi nào phù hợp</li>';
        return;
      }

      filtered.forEach(item => {
        const li = document.createElement('li');
        li.className = 'log-item' + (selectedId === item.id ? ' active' : '');
        li.onclick = () => selectItem(item.id);

        let statusClass = 'status-pending';
        let statusText = 'PENDING...';
        if (item.statusCode !== null && item.statusCode !== undefined) {
          if (item.statusCode === 0) {
            statusClass = 'status-err';
            statusText = 'NETWORK ERR';
          } else if (item.statusCode >= 200 && item.statusCode < 300) {
            statusClass = 'status-2xx';
            statusText = item.statusCode + ' ' + (item.statusMessage || 'OK');
          } else if (item.statusCode >= 400 && item.statusCode < 500) {
            statusClass = 'status-4xx';
            statusText = item.statusCode + ' ' + (item.statusMessage || 'BAD');
          } else if (item.statusCode >= 500) {
            statusClass = 'status-5xx';
            statusText = item.statusCode + ' ' + (item.statusMessage || 'CRASH');
          }
        }

        const methodBadgeClass = 'badge-' + item.method.toLowerCase();

        li.innerHTML = `
          <div class="log-item-header">
            <span class="badge \${methodBadgeClass}">\${item.method}</span>
            <span class="status-badge \${statusClass}">\${statusText}</span>
          </div>
          <div class="log-item-path" title="\${item.url}">\${item.path}</div>
          <div class="log-item-footer">
            <span>\${item.timeFormatted}</span>
            \${item.isError ? `<span class="log-item-err-tag">⚠️ \${item.errorCategory}</span>` : ''}
            <span>\${item.durationMs ? item.durationMs + ' ms' : ''}</span>
          </div>
        `;
        listEl.appendChild(li);
      });
    }

    function selectItem(id) {
      selectedId = id;
      renderList();
      const item = logs.find(l => l.id === id);
      renderDetail(item);
    }

    function renderDetail(item) {
      const container = document.getElementById('detailView');
      if (!item) {
        container.innerHTML = `
          <div class="empty-state">
            <div class="empty-icon">📡</div>
            <p>Chưa chọn yêu cầu nào.</p>
          </div>
        `;
        return;
      }

      const statusBadge = item.isError 
        ? `<span class="status-badge status-err">\${item.statusCode || 0} \${item.statusMessage || 'FAILED'}</span>`
        : `<span class="status-badge status-2xx">\${item.statusCode} \${item.statusMessage || 'OK'}</span>`;

      let errorAlert = '';
      if (item.isError) {
        errorAlert = `
          <div class="error-alert-box">
            <div class="error-alert-title">
              <span>⚠️ PHÂN TÍCH NGUYÊN NHÂN: [\${item.errorCategory}]</span>
            </div>
            <div class="error-alert-desc">
              <strong>\${item.errorSummary || 'Lỗi không xác định'}</strong><br/>
              <span style="font-size:0.8rem; opacity:0.9;">\${item.errorDetails || ''}</span>
            </div>
          </div>
        `;
      }

      container.innerHTML = `
        <div class="detail-header">
          <div style="flex:1;">
            <div class="detail-url-title">
              <span class="badge badge-\${item.method.toLowerCase()}">\${item.method}</span>
              \${item.url}
            </div>
            <div class="detail-meta-tags">
              \${statusBadge}
              <span>⏱ \${item.durationMs ? item.durationMs + ' ms' : 'Đang xử lý'}</span>
              <span>🕒 \${item.timeFormatted}</span>
            </div>
          </div>
          <button class="btn" onclick="copyText('\${item.curlCommand.replace(/'/g, "\\\\'")}')">📋 Copy cURL</button>
        </div>

        \${errorAlert}

        <div class="tab-bar">
          <button class="tab-btn \${currentTab === 'RESPONSE' ? 'active' : ''}" onclick="switchTab('RESPONSE')">Response Data</button>
          <button class="tab-btn \${currentTab === 'REQUEST' ? 'active' : ''}" onclick="switchTab('REQUEST')">Request Payload</button>
          <button class="tab-btn \${currentTab === 'HEADERS' ? 'active' : ''}" onclick="switchTab('HEADERS')">Headers & Params</button>
          <button class="tab-btn \${currentTab === 'CURL' ? 'active' : ''}" onclick="switchTab('CURL')">cURL</button>
        </div>

        <div class="tab-content" id="tabContentBox">
          \${renderTabContent(item)}
        </div>
      `;
    }

    function switchTab(tab) {
      currentTab = tab;
      const item = logs.find(l => l.id === selectedId);
      if (item) renderDetail(item);
    }

    function renderTabContent(item) {
      if (currentTab === 'RESPONSE') {
        const json = item.responseBody ? JSON.stringify(item.responseBody, null, 2) : 'Không có dữ liệu phản hồi (No Content)';
        return `
          <div class="section-label">Response Body <button class="copy-btn" onclick="copyText(document.getElementById('resBox').innerText)">Copy</button></div>
          <div class="code-box" id="resBox">\${escapeHtml(json)}</div>
        `;
      } else if (currentTab === 'REQUEST') {
        const body = item.requestBody ? JSON.stringify(item.requestBody, null, 2) : 'Trống (Không gửi kèm Body)';
        return `
          <div class="section-label">Request Body Payload <button class="copy-btn" onclick="copyText(document.getElementById('reqBox').innerText)">Copy</button></div>
          <div class="code-box" id="reqBox">\${escapeHtml(body)}</div>
        `;
      } else if (currentTab === 'HEADERS') {
        return `
          <div class="section-label">Request Headers</div>
          <div class="code-box">\${escapeHtml(JSON.stringify(item.requestHeaders, null, 2))}</div>
          <div class="section-label">Query Parameters</div>
          <div class="code-box">\${escapeHtml(JSON.stringify(item.queryParams, null, 2))}</div>
          <div class="section-label">Response Headers</div>
          <div class="code-box">\${escapeHtml(JSON.stringify(item.responseHeaders || {}, null, 2))}</div>
        `;
      } else if (currentTab === 'CURL') {
        return `
          <div class="section-label">Command Line (cURL)</div>
          <div class="code-box" id="curlBox">\${escapeHtml(item.curlCommand)}</div>
        `;
      }
    }

    function escapeHtml(str) {
      if (!str) return '';
      return String(str)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
    }

    function copyText(text) {
      navigator.clipboard.writeText(text).then(() => {
        alert('Đã copy thành công vào Clipboard!');
      });
    }

    function clearAllLogs() {
      if (confirm('Bạn có chắc chắn muốn xóa toàn bộ log hiện tại?')) {
        if (socket && socket.readyState === WebSocket.OPEN) {
          socket.send(JSON.stringify({ action: 'CLEAR' }));
        } else {
          fetch('/api/clear', { method: 'POST' });
        }
      }
    }

    initWebSocket();
  </script>
</body>
</html>
''';
  }
}
