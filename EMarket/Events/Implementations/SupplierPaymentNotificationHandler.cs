using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;
using EMarket.Models;
using EMarket.Modules.InventoryModule.DTOs;
using EMarket.Modules.InventoryModule.Services.Interfaces;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;

namespace EMarket.Events.Implementations
{
    public class SupplierPaymentNotificationHandler :
    IEventHandler<SupplierPaymentCreatedEvent>,
    IEventHandler<SupplierPaymentDeletedEvent>,
        IEventHandler<SupplierDebtNearDueEvent>,
        IEventHandler<SupplierDebtOverdueEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ITelegramService _telegramService;
        private readonly ISystemConfigService _systemConfigService;
        private readonly ISupplierServiceDebtAndPaymentService _supplierDebtAndPaymentService;
        private readonly EMarket_DBEntities _db;

        public SupplierPaymentNotificationHandler(IEmailService emailService, EMarket_DBEntities db, ISupplierServiceDebtAndPaymentService supplierServiceDebtAndPaymentService, ITelegramService telegramService, ISystemConfigService systemConfigService)
        {
            _emailService = emailService;
            _supplierDebtAndPaymentService = supplierServiceDebtAndPaymentService;
            _db = db;
            _telegramService = telegramService;
            _systemConfigService = systemConfigService;
        }

        // 1. Xử lý khi TẠO thanh toán mới
        public async Task HandleAsync(SupplierPaymentCreatedEvent ev)
        {
            try
            {
                var info = await _supplierDebtAndPaymentService
                    .GetPaymentMailInfoAsync(ev.PaymentId);

                if (info == null || string.IsNullOrWhiteSpace(info.SupplierEmail))
                    return;
                var amountFormatted = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:N0}", info.Amount);
                var unpaidFormatted = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:N0}", info.UnpaidAmountAfterPayment);

                var subject =
                    $"[EMarket] Thông báo đã thanh toán công nợ – Phiếu chi #{info.PaymentId}";

                var paymentProofSection = string.IsNullOrWhiteSpace(info.PaymentProof)
                    ? "<p><i>Minh chứng thanh toán sẽ được bổ sung sau.</i></p>"
                    : $@"
            <p>
                <strong>Minh chứng thanh toán:</strong><br/>
                <a href='{info.PaymentProof}' target='_blank'>
                    Xem hình ảnh minh chứng
                </a>
            </p>
            <p>
                <img src='{info.PaymentProof}'
                     alt='Payment Proof'
                     style='max-width: 500px; border: 1px solid #ccc;' />
            </p>";

                var body = $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
    <div style='text-align: center; margin-bottom: 20px;'>
        <h2 style='color: #198754; margin: 0;'>EMarket - THÔNG BÁO THANH TOÁN</h2>
        <p style='font-size: 12px; color: #888;'>Hệ thống quản lý công nợ tự động</p>
    </div>

    <p>Kính gửi <strong>{info.SupplierName}</strong>,</p>

    <p>Chúng tôi xin trân trọng thông báo đã <strong>thực hiện thanh toán</strong> khoản công nợ với chi tiết như sau:</p>

    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 8px; margin-bottom: 20px;'>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px 0; border-bottom: 1px solid #eee;'><strong>Mã phiếu chi:</strong></td>
                <td style='padding: 8px 0; border-bottom: 1px solid #eee; text-align: right;'>#{info.PaymentId}</td>
            </tr>
            <tr>
                <td style='padding: 8px 0; border-bottom: 1px solid #eee;'><strong>Số tiền thanh toán:</strong></td>
                <td style='padding: 8px 0; border-bottom: 1px solid #eee; text-align: right; color: #198754; font-size: 18px; font-weight: bold;'>{amountFormatted} VNĐ</td>
            </tr>
            <tr>
                <td style='padding: 8px 0; border-bottom: 1px solid #eee;'><strong>Phương thức:</strong></td>
                <td style='padding: 8px 0; border-bottom: 1px solid #eee; text-align: right;'>{info.PaymentMethod}</td>
            </tr>
            <tr>
                <td style='padding: 8px 0;'><strong>Dư nợ còn lại:</strong></td>
                <td style='padding: 8px 0; text-align: right; font-weight: bold; color: #dc3545;'>{unpaidFormatted} VNĐ</td>
            </tr>
        </table>
    </div>

    {(!string.IsNullOrWhiteSpace(info.PaymentProof) ? $@"
    <div style='margin-top: 20px; border-top: 1px dashed #ccc; pt-15px;'>
        <p style='font-weight: bold; color: #555;'>Minh chứng giao dịch:</p>
        <div style='text-align: center;'>
            <img src='{info.PaymentProof}' style='max-width: 100%; border-radius: 5px; border: 1px solid #ddd;' alt='Payment Proof' />
            <p><a href='{info.PaymentProof}' style='display: inline-block; padding: 10px 20px; background-color: #198754; color: white; text-decoration: none; border-radius: 5px; margin-top: 10px;'>Tải ảnh minh chứng</a></p>
        </div>
    </div>" : "")}

    <p style='font-size: 13px; color: #666; font-style: italic; margin-top: 20px;'>
        * Đây là email tự động từ hệ thống kế toán EMarket. Vui lòng không phản hồi trực tiếp vào email này.
    </p>

    <div style='margin-top: 30px; border-top: 2px solid #198754; padding-top: 10px;'>
        <strong>Phòng Kế Toán – Hệ thống EMarket</strong><br/>
        Địa chỉ: Đại học Thủ Dầu Một, Thành phố Hồ Chí Minh<br/>
        Hotline: 0123456789
    </div>
</div>";

                await _emailService.SendAsync(
                    info.SupplierEmail,
                    subject,
                    body
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[SupplierPaymentCreatedEvent] Send mail failed: {ex.Message}"
                );
            }
        }


        // 2. Xử lý khi XÓA thanh toán
        public async Task HandleAsync(SupplierPaymentDeletedEvent ev)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ev.SupplierEmail))
                    return;
                var amountDel = ev.Amount.ToString("N0", new System.Globalization.CultureInfo("vi-VN"));
                var subject =
                    $"[EMarket] Thông báo hủy phiếu chi – #{ev.PaymentId}";

                var body = $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
    <div style='text-align: center; margin-bottom: 20px;'>
        <h2 style='color: #dc3545; margin: 0;'>THÔNG BÁO HỦY PHIẾU CHI</h2>
    </div>

    <p>Kính gửi <strong>{ev.SupplierName}</strong>,</p>

    <p>Chúng tôi xin thông báo <strong>phiếu chi sau đã bị hủy</strong> trên hệ thống để điều chỉnh lại dữ liệu:</p>

    <div style='background-color: #fff5f5; border: 1px solid #feb2b2; padding: 15px; border-radius: 8px;'>
        <p style='margin: 5px 0;'><strong>Mã phiếu chi:</strong> #{ev.PaymentId}</p>
        <p style='margin: 5px 0;'><strong>Số tiền đã hủy:</strong> <span style='color: #dc3545; font-weight: bold;'>{amountDel} VNĐ</span></p>
    </div>

    <p style='margin-top: 20px;'>Lý do hủy thường bao gồm: sai sót thông tin, điều chỉnh đối soát hoặc thay đổi phương thức thanh toán. Chúng tôi sẽ sớm cập nhật lại thông tin mới nhất cho Quý đối tác.</p>

    <p>Trân trọng,<br/><strong>Phòng Kế Toán – EMarket</strong></p>
</div>";

                await _emailService.SendAsync(
                    ev.SupplierEmail,
                    subject,
                    body
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[SupplierPaymentDeletedEvent] Send mail failed: {ex.Message}"
                );
            }
        }


        // --- 1. SỰ KIỆN SẮP ĐẾN HẠN (GỬI TỔNG HỢP) ---
        public async Task HandleAsync(SupplierDebtNearDueEvent ev)
        {
            try
            {
                // 1. Gọi Service lấy danh sách chi tiết các khoản nợ
                // Hàm GetInternalDebtDetailAsync bây giờ nhận vào List<int>
                var debtList = await _supplierDebtAndPaymentService.GetInternalDebtDetailAsync(ev.DebtIds);
                var domain = await _systemConfigService.GetAppBaseUrl();

                if (debtList == null || !debtList.Any()) return;

                // 2. Lấy thông tin người nhận từ bản ghi đầu tiên (vì tất cả dùng chung danh sách Role 5)
                var firstInfo = debtList.First();
                var subject = $"[EMarket - NHẮC VIỆC] Tổng hợp {debtList.Count} khoản nợ sắp đến hạn";
                var culture = new System.Globalization.CultureInfo("vi-VN");

                string tableRows = "";
                foreach (var item in debtList)
                {
                    string moneyColor = "#d92d20";

                    tableRows += $@"
                        <tr>
                            <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; color: #374151; font-weight: 600;'>#{item.PurchaseOrderId}</td>
                            <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; color: #4b5563;'>{item.SupplierName}</td>
                            <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; text-align: right; font-weight: 700; color: {moneyColor}; font-family: monospace; font-size: 14px;'>{item.UnpaidAmount?.ToString("N0", culture)}</td>
                            <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; text-align: center; color: #6b7280; font-size: 13px;'>{item.DueDate:dd/MM/yyyy}</td>
                        </tr>";
                }

                var body = $@"
<div style='background-color: #f3f4f6; padding: 40px 0; font-family: ""Segoe UI"", Helvetica, Arial, sans-serif;'>
    <table align='center' border='0' cellpadding='0' cellspacing='0' style='width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1); overflow: hidden;'>
        
        <tr>
            <td style='background-color: #f59e0b; padding: 20px 30px;'>
                <h1 style='color: #ffffff; font-size: 20px; margin: 0; font-weight: 600; letter-spacing: 0.5px;'>📅 NHẮC THANH TOÁN</h1>
            </td>
        </tr>

        <tr>
            <td style='padding: 30px;'>
                <p style='color: #374151; font-size: 15px; margin-top: 0;'>Chào <strong>{firstInfo.EmployeeName}</strong>,</p>
                <p style='color: #4b5563; line-height: 1.5;'>Dưới đây là tổng hợp các khoản công nợ cần được thanh toán trong <strong>3 ngày tới</strong>. Vui lòng kiểm tra và lập kế hoạch dòng tiền.</p>
                
                <table style='width: 100%; border-collapse: collapse; margin-top: 20px; font-size: 14px;'>
                    <thead>
                        <tr style='background-color: #f9fafb; text-align: left;'>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px;'>Mã đơn</th>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px;'>Nhà cung cấp</th>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px; text-align: right;'>Số tiền</th>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px; text-align: center;'>Hạn trả</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tableRows}
                    </tbody>
                </table>

                <div style='margin-top: 30px; text-align: center;'>
                    <a href='{domain}/Admin/Debt' style='background-color: #f59e0b; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block;'>Xem chi tiết và Duyệt chi</a>
                </div>
            </td>
        </tr>

        <tr>
            <td style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
                <p style='margin: 0; color: #9ca3af; font-size: 12px;'>© 2025 EMarket System. Đây là email tự động.</p>
            </td>
        </tr>
    </table>
</div>";

                await _emailService.SendAsync(firstInfo.RecipientEmail, subject, body);

                var teleMsg = await GenerateTelegramMessage("🔔 [EMARKET] TỔNG HỢP CÔNG NỢ SẮP ĐẾN HẠN", debtList);
                await _telegramService.SendMessageAsync(teleMsg);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NearDue Summary Error] {ex.Message}");
            }
        }

        public async Task HandleAsync(SupplierDebtOverdueEvent ev)
        {
            try
            {
                var debtList = await _supplierDebtAndPaymentService.GetInternalDebtDetailAsync(ev.DebtIds);

                var domain = await _systemConfigService.GetAppBaseUrl();

                if (debtList == null || !debtList.Any()) return;

                var firstInfo = debtList.First();
                var subject = $"[CẢNH BÁO] Phát hiện {debtList.Count} khoản nợ QUÁ HẠN thanh toán";
                var culture = new System.Globalization.CultureInfo("vi-VN");
                var today = DateTime.Now;

                string tableRows = "";
                foreach (var item in debtList)
                {
                    string moneyColor = "#b45309";

                    tableRows += $@"
    <tr>
        <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; color: #374151; font-weight: 600;'>#{item.PurchaseOrderId}</td>
        <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; color: #4b5563;'>{item.SupplierName}</td>
        <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; text-align: right; font-weight: 700; color: {moneyColor}; font-family: monospace; font-size: 14px;'>{item.UnpaidAmount?.ToString("N0", culture)}</td>
        <td style='padding: 12px 15px; border-bottom: 1px solid #e5e7eb; text-align: center; color: #6b7280; font-size: 13px;'>{item.DueDate:dd/MM/yyyy}</td>
    </tr>";
                }

                var totalAmount = debtList.Sum(x => x.UnpaidAmount ?? 0);

                // 3. Nội dung HTML Body
                // 4. Nội dung HTML Body (Sử dụng cấu trúc Table Wrapper để đảm bảo tính thống nhất)
                var body = $@"
<div style='background-color: #f3f4f6; padding: 40px 0; font-family: ""Segoe UI"", Helvetica, Arial, sans-serif;'>
    <table align='center' border='0' cellpadding='0' cellspacing='0' style='width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1); overflow: hidden;'>
        
        <tr>
            <td style='background-color: #b91c1c; padding: 20px 30px;'>
                <h1 style='color: #ffffff; font-size: 20px; margin: 0; font-weight: 600; letter-spacing: 0.5px;'> CẢNH BÁO: QUÁ HẠN THANH TOÁN</h1>
            </td>
        </tr>

        <tr>
            <td style='padding: 30px;'>
                <p style='color: #374151; font-size: 15px; margin-top: 0;'>Gửi bộ phận kế toán,</p>
                <div style='background-color: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin: 15px 0; color: #991b1b; font-size: 14px;'>
                    <strong>Hệ thống phát hiện:</strong> Có {debtList.Count} khoản nợ đã vượt quá thời hạn thanh toán. Cần xử lý ngay để tránh phạt vi phạm hợp đồng.
                </div>
                
                <table style='width: 100%; border-collapse: collapse; margin-top: 20px; font-size: 14px;'>
                    <thead>
                        <tr style='background-color: #f9fafb; text-align: left;'>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px;'>Mã đơn</th>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px;'>Đối tác</th>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px; text-align: right;'>Nợ quá hạn</th>
                            <th style='padding: 10px 15px; color: #6b7280; font-weight: 600; text-transform: uppercase; font-size: 11px; letter-spacing: 1px; text-align: center;'>Ngày đến hạn</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tableRows} 
                        <tr style='background-color: #fff1f2;'>
                            <td colspan='2' style='padding: 15px; border-top: 2px solid #e5e7eb; font-weight: 700; color: #374151;'>TỔNG CỘNG</td>
                            <td style='padding: 15px; border-top: 2px solid #e5e7eb; text-align: right; font-weight: 700; color: #dc2626; font-size: 16px;'>{totalAmount.ToString("N0", culture)}</td>
                            <td style='border-top: 2px solid #e5e7eb;'></td>
                        </tr>
                    </tbody>
                </table>

                <div style='margin-top: 30px; text-align: center;'>
                    <a href='{domain}/Admin/Debt' style='background-color: #dc2626; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block; box-shadow: 0 2px 4px rgba(220, 38, 38, 0.3);'>Xử lý ngay lập tức</a>
                </div>
            </td>
        </tr>

        <tr>
            <td style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
                <p style='margin: 0; color: #9ca3af; font-size: 12px;'>© 2025 EMarket System.</p>
            </td>
        </tr>
    </table>
</div>";
                await _emailService.SendAsync(firstInfo.RecipientEmail, subject, body);
                var teleMsgOverdue = await GenerateTelegramMessage(" [EMARKET] CẢNH BÁO CÔNG NỢ QUÁ HẠN", debtList);
                await _telegramService.SendMessageAsync(teleMsgOverdue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Overdue Summary Error] {ex.Message}");
            }
        }

        private async Task<string> GenerateTelegramMessage(string title, List<InternalDebtNotificationDTO> debtList)
        {
            var culture = new System.Globalization.CultureInfo("vi-VN");
            var totalAmount = debtList.Sum(x => x.UnpaidAmount ?? 0);

            // 1. Lấy và làm sạch domain
            var domain = await _systemConfigService.GetAppBaseUrl();
            var targetUrl = $"{domain}/Admin/Supplier/SupplierDebtAndPayment";

            // 2. Xây dựng nội dung
            var msg = $"<b>{title}</b>\n";
            msg += $"----------------------------\n";
            msg += $"Số lượng: <b>{debtList.Count} đơn hàng</b>\n";
            msg += $"Tổng tiền: <b>{totalAmount.ToString("N0", culture)} đ</b>\n\n";

            foreach (var item in debtList.Take(3))
            {
                msg += $"• #{item.PurchaseOrderId} - {item.SupplierName}: <b>{item.UnpaidAmount?.ToString("N0", culture)} đ</b>\n";
            }

            if (debtList.Count > 3) msg += $"... và {debtList.Count - 3} đơn khác.\n";

            msg += $"\n👉 <a href='{targetUrl}'>Mở trực tiếp trên Web</a>";


            return msg;
        }
    }
}