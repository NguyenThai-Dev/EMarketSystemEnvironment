import 'package:flutter/material.dart';
import '../../core/config/app_config.dart';
import '../../core/constants/app_colors.dart';
import '../../core/network/api_client.dart';

class ServerSettingsDialog extends StatefulWidget {
  const ServerSettingsDialog({super.key});

  static Future<void> show(BuildContext context) {
    return showDialog(
      context: context,
      builder: (context) => const ServerSettingsDialog(),
    );
  }

  @override
  State<ServerSettingsDialog> createState() => _ServerSettingsDialogState();
}

class _ServerSettingsDialogState extends State<ServerSettingsDialog> {
  late TextEditingController _emarketController;
  late TextEditingController _aiController;

  @override
  void initState() {
    super.initState();
    _emarketController = TextEditingController(text: AppConfig.emarketBaseUrl);
    _aiController = TextEditingController(text: AppConfig.aiBaseUrl);
  }

  @override
  void dispose() {
    _emarketController.dispose();
    _aiController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    await AppConfig.updateEmarketBaseUrl(_emarketController.text);
    await AppConfig.updateAiBaseUrl(_aiController.text);
    ApiClient().refreshBaseUrls();
    if (mounted) {
      Navigator.pop(context);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Đã cập nhật địa chỉ máy chủ thành công!')),
      );
    }
  }

  Future<void> _reset() async {
    await AppConfig.resetToDefaults();
    ApiClient().refreshBaseUrls();
    setState(() {
      _emarketController.text = AppConfig.emarketBaseUrl;
      _aiController.text = AppConfig.aiBaseUrl;
    });
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      title: const Row(
        children: [
          Icon(Icons.dns_rounded, color: AppColors.primary),
          SizedBox(width: 8),
          Text('Cài đặt Máy chủ (Backend)', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
        ],
      ),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Thiết lập địa chỉ kết nối Backend ASP.NET và Dịch vụ AI:',
              style: TextStyle(fontSize: 13, color: AppColors.textSecondary),
            ),
            const SizedBox(height: 16),
            const Text('EMarket Base URL:', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 6),
            TextField(
              controller: _emarketController,
              decoration: const InputDecoration(
                hintText: 'http://10.0.2.2:54446/',
                prefixIcon: Icon(Icons.link, size: 20),
              ),
              style: const TextStyle(fontSize: 13),
            ),
            Wrap(
              spacing: 6,
              runSpacing: 6,
              children: [
                ActionChip(
                  label: const Text('Localhost (54446)', style: TextStyle(fontSize: 11)),
                  onPressed: () => setState(() => _emarketController.text = AppConfig.presetLocalhostHttp),
                ),
                ActionChip(
                  label: const Text('SSL (44339)', style: TextStyle(fontSize: 11)),
                  onPressed: () => setState(() => _emarketController.text = AppConfig.presetLocalhostHttps),
                ),
                ActionChip(
                  label: const Text('Emulator (10.0.2.2)', style: TextStyle(fontSize: 11)),
                  onPressed: () => setState(() => _emarketController.text = AppConfig.presetAndroidEmulator),
                ),
                ActionChip(
                  avatar: const Icon(Icons.cloud_outlined, size: 14),
                  label: const Text('Internet / Prod (Note)', style: TextStyle(fontSize: 11)),
                  onPressed: () => setState(() => _emarketController.text = AppConfig.presetProductionDemo),
                ),
              ],
            ),
            const SizedBox(height: 8),
            const Text(
              '• Môi trường dev ưu tiên localhost/10.0.2.2.\n• Khi deploy lên internet (VPS/Cloud/Domain thật), nhập link domain HTTPS của bạn vào đây.',
              style: TextStyle(fontSize: 11, color: AppColors.textMuted),
            ),
            const SizedBox(height: 14),
            const Text('AI Assistant Base URL:', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
            const SizedBox(height: 6),
            TextField(
              controller: _aiController,
              decoration: const InputDecoration(
                hintText: 'http://10.0.2.2:5000/',
                prefixIcon: Icon(Icons.smart_toy_outlined, size: 20),
              ),
              style: const TextStyle(fontSize: 13),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: _reset,
          child: const Text('Mặc định', style: TextStyle(color: AppColors.textSecondary)),
        ),
        ElevatedButton(
          onPressed: _save,
          style: ElevatedButton.styleFrom(
            backgroundColor: AppColors.primary,
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
          ),
          child: const Text('Lưu cài đặt', style: TextStyle(color: Colors.white)),
        ),
      ],
    );
  }
}
