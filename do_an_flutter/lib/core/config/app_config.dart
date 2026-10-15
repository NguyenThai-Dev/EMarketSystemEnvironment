import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

class AppConfig {
  static const String _keyEmarketBaseUrl = 'emarket_base_url';
  static const String _keyAiBaseUrl = 'ai_base_url';

  // Default URLs:
  // - Localhost HTTP: http://localhost:54446/
  // - Localhost HTTPS: https://localhost:44339/
  // - Android Emulator: http://10.0.2.2:54446/
  // - Production Note: Khi triển khai ứng dụng EMarket lên cloud/VPS (ví dụ IIS Server, Azure, AWS, Cloudflare Tunnel),
  //   chỉ cần thay đổi Base URL sang domain production (ví dụ: https://emarket.yourdomain.com/) thông qua ServerSettingsDialog
  //   hoặc biến môi trường mà không cần sửa code.
  static const String presetLocalhostHttp = 'http://localhost:54446/';
  static const String presetLocalhostHttps = 'https://localhost:44339/';
  static const String presetAndroidEmulator = 'http://10.0.2.2:54446/';
  static const String presetProductionDemo = 'https://api.emarket.vn/'; // Dự phòng khi deploy internet

  static String get defaultEmarketBaseUrl {
    if (kIsWeb) return presetLocalhostHttp;
    if (Platform.isAndroid) return presetAndroidEmulator;
    return presetLocalhostHttp;
  }

  static String get defaultAiBaseUrl {
    if (kIsWeb) return 'http://localhost:5000/';
    if (Platform.isAndroid) return 'http://10.0.2.2:5000/';
    return 'https://localhost:5001/';
  }

  static String _emarketBaseUrl = defaultEmarketBaseUrl;
  static String _aiBaseUrl = defaultAiBaseUrl;

  static String get emarketBaseUrl => _emarketBaseUrl;
  static String get aiBaseUrl => _aiBaseUrl;

  static Future<void> init() async {
    final prefs = await SharedPreferences.getInstance();
    _emarketBaseUrl = prefs.getString(_keyEmarketBaseUrl) ?? defaultEmarketBaseUrl;
    _aiBaseUrl = prefs.getString(_keyAiBaseUrl) ?? defaultAiBaseUrl;
  }

  static Future<void> updateEmarketBaseUrl(String newUrl) async {
    String url = newUrl.trim();
    if (!url.endsWith('/')) url += '/';
    _emarketBaseUrl = url;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_keyEmarketBaseUrl, url);
  }

  static Future<void> updateAiBaseUrl(String newUrl) async {
    String url = newUrl.trim();
    if (!url.endsWith('/')) url += '/';
    _aiBaseUrl = url;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_keyAiBaseUrl, url);
  }

  static Future<void> resetToDefaults() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_keyEmarketBaseUrl);
    await prefs.remove(_keyAiBaseUrl);
    _emarketBaseUrl = defaultEmarketBaseUrl;
    _aiBaseUrl = defaultAiBaseUrl;
  }
}
