using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;
using EMarket.Modules.ProductModule.Services.Interfaces;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;
using EMarket.Modules.UserModule.Services.Interfaces;

namespace EMarket.Events.Implementations
{
    public class InventoryAlertService : IInventoryAlertService
    {
        private readonly IProductService _productService;
        private readonly IEmailService _emailService;
        private readonly ITelegramService _telegramService;
        private readonly IUserService _userService;
        private readonly ISystemConfigService _systemConfigService;

        public InventoryAlertService(IProductService productService, IEmailService emailService, ITelegramService telegramService, IUserService userService, ISystemConfigService systemConfigService)
        {
            _productService = productService;
            _emailService = emailService;
            _telegramService = telegramService;
            _userService = userService;
            _systemConfigService = systemConfigService;
        }

        public async Task CheckAndSendAlertsAsync()
        {
            var lowStockList = await _productService.ReadLowStockAlertsAsync();
            if (lowStockList == null || !lowStockList.Any()) return;

            var urgentList = lowStockList.Where(x => x.CurrentStock <= (x.MinStock * 0.5)).ToList();

            // Gửi Telegram
            if (urgentList.Any())
            {
                try
                {
                    // Chỉ những ông cực thiếu mới làm phiền Telegram
                    var teleMsg = await GenerateTelegramMessage("🚨 [EMARKET] CẢNH BÁO: HÀNG SẮP CẠN KIỆT", urgentList);
                    await _telegramService.SendMessageAsync(teleMsg);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Lỗi gửi Telegram tồn kho thấp: " + ex.Message);
                }
            }

            // Gửi Email
            try
            {
                await SendLowStockEmailAsync(lowStockList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi gửi Email tồn kho thấp: " + ex.Message);
            }
        }

        private async Task SendLowStockEmailAsync(List<LowStockAlertDTO> list)
        {
            var emails = await _userService.GetWarehouseManagerEmailsAsync();
            var subject = $"[EMarket] Báo cáo tồn kho hụt ngưỡng - {DateTime.Now:dd/MM/yyyy}";
            var domain = await _systemConfigService.GetAppBaseUrl();
            var targetUrl = $"{domain}/Admin/Supplier/SupplierDebtAndPayment";

            // Build rows cho table sản phẩm
            var rows = "";
            foreach (var item in list)
            {
                rows += $@"
                <tr>
                    <td style='padding: 10px; border-bottom: 1px solid #eee;'>{item.ProductName}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee;'>{item.WarehouseName}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: center; color: #dc3545; font-weight: bold;'>{item.CurrentStock}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-align: center;'>{item.MinStock}</td>
                </tr>";
            }

            var body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <h2 style='color: #dc3545; margin: 0;'>CẢNH BÁO TỒN KHO</h2>
                <p style='font-size: 12px; color: #888;'>Phát hiện {list.Count} sản phẩm dưới mức tối thiểu</p>
            </div>
            <p>Chào Ban Quản Lý,</p>
            <p>Hệ thống ghi nhận một số mặt hàng đã chạm ngưỡng cần nhập hàng thêm:</p>
            <table style='width: 100%; border-collapse: collapse; margin-top: 10px;'>
                <thead style='background-color: #f8f9fa;'>
                    <tr>
                        <th style='padding: 10px; text-align: left;'>Sản phẩm</th>
                        <th style='padding: 10px; text-align: left;'>Kho</th>
                        <th style='padding: 10px;'>Hiện tại</th>
                        <th style='padding: 10px;'>Mức Min</th>
                    </tr>
                </thead>
                <tbody>{rows}</tbody>
            </table>
            <div style='margin-top: 25px; text-align: center;'>
                <a href='{targetUrl}' style='background-color: #0d6efd; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Đến Dashboard Kho</a>
            </div>
        </div>";

            foreach (var email in emails)
            {
                await _emailService.SendAsync(email, subject, body);
            }
        }

        private async Task<string> GenerateTelegramMessage(string title, List<LowStockAlertDTO> list)
        {
            var domain = await _systemConfigService.GetAppBaseUrl();
            var targetUrl = $"{domain}/Admin/Supplier/SupplierDebtAndPayment";

            if (list == null || list.Count == 0)
                return $"<b>{title}</b>\n\n Không có sản phẩm nào dưới mức tồn tối thiểu.";

            var msg = $"<b>{title}</b>\n";
            msg += $"━━━━━━━━━━━━━━━━━━━━━━\n";
            msg += $"🚨 <b>Cảnh báo thiếu hàng</b>\n";
            msg += $" Tổng sản phẩm: <b>{list.Count}</b>\n\n";

            foreach (var item in list.Take(5)) // chỉ hiển thị 5 case nặng nhất
            {
                var percent = item.MinStock == 0
                    ? 0
                    : (int)Math.Round(item.CurrentStock * 100.0 / item.MinStock);

                msg += $" <b>{item.ProductName}</b>\n";
                msg += $"   • Tồn kho: <code>{item.CurrentStock}</code> / <code>{item.MinStock}</code> ({percent}%)\n";
                msg += $"   • Kho: <b>{item.WarehouseName}</b>\n";
                msg += $"   • Chi nhánh: <b>{item.BranchName}</b>\n\n";
            }

            if (list.Count > 5)
            {
                msg += $"<i>… và {list.Count - 5} sản phẩm khác đang thiếu hàng.</i>\n";
            }

            msg += $"\n👉 <b>Khuyến nghị:</b> Kiểm tra và tạo phiếu nhập sớm để tránh gián đoạn bán hàng.";
            msg += $"\n👉 <a href='{targetUrl}'>Mở trực tiếp trên Web</a>";

            return msg;
        }

    }
}