import '../../core/constants/api_endpoints.dart';
import '../../core/network/api_client.dart';
import '../models/ai_model.dart';

class AiApiService {
  final ApiClient _client = ApiClient();

  /// Chat với EMarket AIAssistantService (Cổng 5000/5001: Groq / Semantic Kernel)
  Future<String> askAi(String prompt, {String? userName}) async {
    try {
      final res = await _client.aiDio.post(
        ApiEndpoints.aiChat,
        data: {
          'Prompt': prompt,
          'UserName': userName ?? 'admin',
        },
      );
      if (res.data is Map<String, dynamic>) {
        return res.data['answer'] ?? res.data['reply'] ?? res.data['response'] ?? res.data['message'] ?? 'Đã nhận phản hồi.';
      }
      return res.data.toString();
    } catch (e) {
      return 'Không thể kết nối đến Trợ lý AI: $e';
    }
  }

  /// Lấy danh sách gợi ý nhập hàng thông minh (Replenishment Advice)
  Future<List<AIReplenishmentModel>> getReplenishmentAdvice(int branchId) async {
    try {
      final res = await _client.get('${ApiEndpoints.aiReplenishmentAdvice}/$branchId');
      if (res.data is List) {
        return (res.data as List).map((e) => AIReplenishmentModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Lấy danh sách bất thường ngành hàng do AI phát hiện (Spike / Dip)
  Future<List<AIAnomalyModel>> getAnomalies(int branchId) async {
    try {
      final res = await _client.get('${ApiEndpoints.aiAnomalies}/$branchId');
      if (res.data is List) {
        return (res.data as List).map((e) => AIAnomalyModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Lấy danh sách đánh giá insight sản phẩm (Star, Trending, Warning)
  Future<List<AIInsightModel>> getProductInsights(int branchId) async {
    try {
      final res = await _client.get('${ApiEndpoints.aiInsights}/$branchId');
      if (res.data is List) {
        return (res.data as List).map((e) => AIInsightModel.fromJson(e)).toList();
      }
    } catch (_) {}
    return [];
  }

  /// Kích hoạt chạy phân tích (mode: 'system' hoặc 'ai')
  Future<bool> runAnalysis({String mode = 'system', int branchId = 1}) async {
    try {
      final res = await _client.post(
        ApiEndpoints.aiRunAnalysis,
        data: {'Mode': mode, 'BranchId': branchId},
      );
      if (res.data is Map<String, dynamic>) {
        return res.data['success'] == true;
      }
      return res.statusCode == 200;
    } catch (_) {
      return false;
    }
  }
}
