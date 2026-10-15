import '../config/app_config.dart';

class ImageHelper {
  static String? getFullImageUrl(String? relativePath) {
    if (relativePath == null || relativePath.trim().isEmpty) {
      return null;
    }
    String path = relativePath.trim();
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }
    if (path.startsWith('~')) {
      path = path.substring(1);
    }
    if (path.startsWith('/')) {
      path = path.substring(1);
    }

    String baseUrl = AppConfig.emarketBaseUrl;
    if (!baseUrl.endsWith('/')) {
      baseUrl += '/';
    }
    return '$baseUrl$path';
  }
}
