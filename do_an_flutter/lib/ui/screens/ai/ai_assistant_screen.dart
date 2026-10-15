import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../core/constants/app_colors.dart';
import '../../../data/models/ai_model.dart';
import '../../../data/repositories/data_repository.dart';
import '../../../providers/auth_provider.dart';
import '../../widgets/flat_card.dart';
import '../../widgets/status_badge.dart';

class AIAssistantScreen extends StatefulWidget {
  const AIAssistantScreen({super.key});

  @override
  State<AIAssistantScreen> createState() => _AIAssistantScreenState();
}

class _AIAssistantScreenState extends State<AIAssistantScreen> with SingleTickerProviderStateMixin {
  final _repository = DataRepository();
  late TabController _tabController;

  // Chat tab state
  final _inputCtrl = TextEditingController();
  final ScrollController _chatScrollCtrl = ScrollController();
  final List<Map<String, String>> _messages = [
    {
      'role': 'ai',
      'text': 'Xin chào! Tôi là Trợ lý AI EMarket. Tôi có thể hỗ trợ bạn tra cứu doanh số, kiểm tra tồn kho, gợi ý nhập hàng và dự báo kinh doanh. Bạn cần giúp gì hôm nay?',
    },
  ];
  bool _isSending = false;

  // Data tabs state
  List<AIReplenishmentModel> _replenishments = [];
  List<AIAnomalyModel> _anomalies = [];
  List<AIInsightModel> _insights = [];
  bool _isLoadingData = true;

  final List<String> _quickPrompts = [
    'Hôm nay cửa hàng bán được bao nhiêu rồi?',
    'Sản phẩm nào sắp hết hàng cần nhập?',
    'Dự báo kinh doanh tuần tới như thế nào?',
    'Mặt hàng nào đang bán chạy nhất?',
  ];

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _loadAiData();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _inputCtrl.dispose();
    _chatScrollCtrl.dispose();
    super.dispose();
  }

  Future<void> _loadAiData() async {
    setState(() => _isLoadingData = true);
    final auth = context.read<AuthProvider>();
    final branchId = auth.selectedBranch?.branchId ?? 1;

    try {
      final results = await Future.wait([
        _repository.getReplenishmentAdvice(branchId),
        _repository.getAnomalies(branchId),
        _repository.getProductInsights(branchId),
      ]);

      if (mounted) {
        setState(() {
          _replenishments = results[0] as List<AIReplenishmentModel>;
          _anomalies = results[1] as List<AIAnomalyModel>;
          _insights = results[2] as List<AIInsightModel>;
          _isLoadingData = false;
        });
      }
    } catch (_) {
      if (mounted) setState(() => _isLoadingData = false);
    }
  }

  Future<void> _sendMessage(String query) async {
    final text = query.trim();
    if (text.isEmpty || _isSending) return;

    _inputCtrl.clear();
    setState(() {
      _messages.add({'role': 'user', 'text': text});
      _isSending = true;
    });

    _scrollToBottom();

    final auth = context.read<AuthProvider>();
    final username = auth.currentUser?.username ?? 'admin';
    final response = await _repository.askAi(text, userName: username);
    if (!mounted) return;

    setState(() {
      _messages.add({'role': 'ai', 'text': response});
      _isSending = false;
    });

    _scrollToBottom();
  }

  void _scrollToBottom() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_chatScrollCtrl.hasClients) {
        _chatScrollCtrl.animateTo(
          _chatScrollCtrl.position.maxScrollExtent,
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeOut,
        );
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Row(
          children: [
            Icon(Icons.smart_toy_rounded, color: Color(0xFF673AB7)),
            SizedBox(width: 8),
            Text('Trợ lý AI & Voice Labs', style: TextStyle(fontWeight: FontWeight.bold)),
          ],
        ),
        bottom: TabBar(
          controller: _tabController,
          labelColor: const Color(0xFF673AB7),
          unselectedLabelColor: AppColors.textMuted,
          indicatorColor: const Color(0xFF673AB7),
          tabs: const [
            Tab(icon: Icon(Icons.chat_bubble_outline), text: 'Hỏi đáp AI'),
            Tab(icon: Icon(Icons.add_shopping_cart), text: 'Gợi ý Nhập'),
            Tab(icon: Icon(Icons.insights), text: 'Bất thường & Insights'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          _buildChatTab(),
          _buildReplenishmentTab(),
          _buildInsightsTab(),
        ],
      ),
    );
  }

  Widget _buildChatTab() {
    return Column(
      children: [
        // Gợi ý câu hỏi nhanh
        Container(
          height: 48,
          padding: const EdgeInsets.symmetric(vertical: 6),
          child: ListView.separated(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            scrollDirection: Axis.horizontal,
            itemCount: _quickPrompts.length,
            separatorBuilder: (_, _) => const SizedBox(width: 8),
            itemBuilder: (context, index) {
              final prompt = _quickPrompts[index];
              return ActionChip(
                label: Text(prompt, style: const TextStyle(fontSize: 12)),
                backgroundColor: const Color(0xFF673AB7).withValues(alpha: 0.08),
                side: BorderSide(color: const Color(0xFF673AB7).withValues(alpha: 0.2)),
                onPressed: () => _sendMessage(prompt),
              );
            },
          ),
        ),
        const Divider(height: 1),

        // Khung hiển thị tin nhắn
        Expanded(
          child: ListView.builder(
            controller: _chatScrollCtrl,
            padding: const EdgeInsets.all(16),
            itemCount: _messages.length,
            itemBuilder: (context, index) {
              final msg = _messages[index];
              final isUser = msg['role'] == 'user';

              return Align(
                alignment: isUser ? Alignment.centerRight : Alignment.centerLeft,
                child: Container(
                  margin: const EdgeInsets.only(bottom: 12),
                  padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                  constraints: BoxConstraints(maxWidth: MediaQuery.of(context).size.width * 0.8),
                  decoration: BoxDecoration(
                    color: isUser ? const Color(0xFF673AB7) : Colors.white,
                    borderRadius: BorderRadius.circular(16).copyWith(
                      bottomRight: isUser ? Radius.zero : const Radius.circular(16),
                      bottomLeft: !isUser ? Radius.zero : const Radius.circular(16),
                    ),
                    border: isUser ? null : Border.all(color: AppColors.border),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withValues(alpha: 0.03),
                        blurRadius: 4,
                        offset: const Offset(0, 2),
                      ),
                    ],
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (!isUser) ...[
                        const Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.auto_awesome, size: 14, color: Color(0xFF673AB7)),
                            SizedBox(width: 4),
                            Text('Trợ lý AI EMarket', style: TextStyle(fontSize: 11, fontWeight: FontWeight.bold, color: Color(0xFF673AB7))),
                          ],
                        ),
                        const SizedBox(height: 4),
                      ],
                      Text(
                        msg['text'] ?? '',
                        style: TextStyle(
                          color: isUser ? Colors.white : AppColors.textPrimary,
                          fontSize: 14,
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          ),
        ),

        if (_isSending)
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const SizedBox(width: 16, height: 16, child: CircularProgressIndicator(strokeWidth: 2, color: Color(0xFF673AB7))),
                const SizedBox(width: 8),
                Text('AI đang suy nghĩ và tính toán...', style: TextStyle(fontSize: 12, color: AppColors.textMuted)),
              ],
            ),
          ),

        // Thanh nhập tin nhắn
        Container(
          padding: const EdgeInsets.all(12),
          decoration: const BoxDecoration(
            color: Colors.white,
            border: Border(top: BorderSide(color: AppColors.border)),
          ),
          child: Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _inputCtrl,
                  decoration: InputDecoration(
                    hintText: 'Nhập câu hỏi tiếng Việt...',
                    contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                    border: OutlineInputBorder(borderRadius: BorderRadius.circular(24)),
                  ),
                  onSubmitted: _sendMessage,
                ),
              ),
              const SizedBox(width: 8),
              IconButton(
                icon: const Icon(Icons.send_rounded, color: Color(0xFF673AB7)),
                onPressed: () => _sendMessage(_inputCtrl.text),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildReplenishmentTab() {
    if (_isLoadingData) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_replenishments.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.inventory_2_outlined, size: 64, color: AppColors.textMuted.withValues(alpha: 0.5)),
            const SizedBox(height: 12),
            const Text('Tồn kho hiện tại rất dồi dào, chưa có khuyến nghị nhập hàng.', style: TextStyle(color: AppColors.textMuted)),
          ],
        ),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: _replenishments.length,
      separatorBuilder: (_, _) => const SizedBox(height: 10),
      itemBuilder: (context, index) {
        final r = _replenishments[index];
        final isUrgent = r.suggestedQty > 50 || r.confidenceLevel == 'HIGH';

        return FlatCard(
          padding: const EdgeInsets.all(14),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      r.productName,
                      style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                    ),
                  ),
                  StatusBadge(
                    label: isUrgent ? 'Cần nhập gấp' : 'Khuyến nghị',
                    color: isUrgent ? AppColors.error : AppColors.emerald,
                    icon: isUrgent ? Icons.priority_high_rounded : Icons.check_circle_outline_rounded,
                  ),
                ],
              ),
              const SizedBox(height: 10),
              Row(
                children: [
                  Text('Tồn kho: ${r.currentStock} ${r.unit ?? ""}', style: const TextStyle(fontSize: 12, color: AppColors.textMuted)),
                  const SizedBox(width: 12),
                  Text('Dự báo nhu cầu: ${r.expectedDemand}', style: const TextStyle(fontSize: 12, color: AppColors.textMuted)),
                  const Spacer(),
                  Text(
                    'Đề xuất nhập: +${r.suggestedQty}',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 14,
                      color: isUrgent ? AppColors.error : AppColors.primaryBrand,
                    ),
                  ),
                ],
              ),
              if (r.reason != null && r.reason!.isNotEmpty) ...[
                const SizedBox(height: 10),
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: const Color(0xFF673AB7).withValues(alpha: 0.06),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    children: [
                      const Icon(Icons.lightbulb_outline, size: 15, color: Color(0xFF673AB7)),
                      const SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          r.reason!,
                          style: const TextStyle(fontSize: 12, color: Color(0xFF673AB7)),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ],
          ),
        );
      },
    );
  }

  Widget _buildInsightsTab() {
    if (_isLoadingData) {
      return const Center(child: CircularProgressIndicator());
    }

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // Bất thường ngành hàng
        const Text('Bất thường Ngành hàng (Anomalies)', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15)),
        const SizedBox(height: 8),
        if (_anomalies.isEmpty)
          const Padding(
            padding: EdgeInsets.symmetric(vertical: 8),
            child: Text('Không phát hiện biến động bất thường nào.', style: TextStyle(color: AppColors.textMuted, fontSize: 13)),
          )
        else
          ..._anomalies.map((a) {
            final isSpike = a.anomalyType.toLowerCase() == 'spike';
            return FlatCard(
              margin: const EdgeInsets.only(bottom: 8),
              padding: const EdgeInsets.all(12),
              child: Row(
                children: [
                  Icon(
                    isSpike ? Icons.trending_up : Icons.trending_down,
                    color: isSpike ? AppColors.emerald : AppColors.error,
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(a.categoryName, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
                        Text('Thực tế: ${a.actualQty} vs Dự báo: ${a.forecastQty}', style: const TextStyle(fontSize: 12, color: AppColors.textMuted)),
                      ],
                    ),
                  ),
                  Text(
                    '${isSpike ? "+" : ""}${a.deviationPercent.toStringAsFixed(1)}%',
                    style: TextStyle(fontWeight: FontWeight.bold, color: isSpike ? AppColors.emerald : AppColors.error),
                  ),
                ],
              ),
            );
          }),

        const SizedBox(height: 16),
        // Product Insights
        const Text('Xếp hạng & Xu hướng Sản phẩm', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15)),
        const SizedBox(height: 8),
        if (_insights.isEmpty)
          const Padding(
            padding: EdgeInsets.symmetric(vertical: 8),
            child: Text('Chưa có báo cáo insight sản phẩm.', style: TextStyle(color: AppColors.textMuted, fontSize: 13)),
          )
        else
          ..._insights.map((ins) {
            final isStar = ins.insightLevel.toLowerCase() == 'star';
            final isWarning = ins.insightLevel.toLowerCase() == 'warning';
            final tagColor = isStar
                ? AppColors.warning
                : isWarning
                    ? AppColors.error
                    : AppColors.secondary;

            return FlatCard(
              margin: const EdgeInsets.only(bottom: 8),
              padding: const EdgeInsets.all(12),
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(ins.productName, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
                        Text('Đóng góp: ${ins.contributionPercent.toStringAsFixed(1)}% doanh thu', style: const TextStyle(fontSize: 11, color: AppColors.textMuted)),
                      ],
                    ),
                  ),
                  StatusBadge(
                    label: ins.insightLevel.toUpperCase(),
                    color: tagColor,
                  ),
                ],
              ),
            );
          }),
      ],
    );
  }
}
