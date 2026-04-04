using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMarket.Filters;
using EMarket.Modules.SystemConfigModule.DTOs;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;

namespace EMarket.Areas.Admin.Controllers
{
    [EMarketAuthorize(RequireAdmin = true)]
    public class SystemConfigController : Controller
    {
        private readonly ISystemConfigService _configService;

        public SystemConfigController(ISystemConfigService configService)
        {
            _configService = configService;
        }

        public async Task<ActionResult> SystemManagement()
        {
            // Lấy toàn bộ config để fill vào View
            // Cách làm hay: Tạo ViewModel chứa các property tương ứng với Interface
            var model = new SystemConfigViewModel
            {
                EmailEnabled = await _configService.IsEmailEnabledAsync(),
                TelegramEnabled = await _configService.IsTelegramEnabledAsync(),
                TelegramToken = await _configService.GetTelegramTokenAsync(),
                TelegramChatId = await _configService.GetTelegramChatIdAsync(),
                AppBaseUrl = await _configService.GetAppBaseUrl(),
                MailHost = await _configService.GetMailHost(),
                MailPassword = await _configService.GetMailHostPass(), // Sẽ xử lý ẩn ở View
                MailDisplayName = await _configService.GetEmailDisplayNameAsync(),
                VAT = await _configService.GetEMarketVAT(),
                BankID = await _configService.GetEMarketBankID(),
                BankNum = await _configService.GetEMarketBankNum(),
                VipDiscount = await _configService.GetVIPDiscount(),
                MemberDiscount = await _configService.GetMemberDiscount(),
                PointExchangeRate = await _configService.GetEMarketPointExchnageRate(),
                PointEarnedRate = await _configService.GetEMarketPointEarnedRate()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateConfig(List<ConfigUpdateDTO> configs)
        {
            try
            {
                foreach (var item in configs)
                {
                    if (string.IsNullOrWhiteSpace(item.Value))
                        continue;

                    var updated = await _configService.UpdateConfigAsync(item.Key, item.Value);

                    if (!updated)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Cập nhật thất bại cho cấu hình: {item.Key}"
                        });
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Cập nhật cấu hình thành công!"
                });
            }
            catch (Exception ex)
            {
                // log ex
                return Json(new
                {
                    success = false,
                    message = "Lỗi hệ thống khi cập nhật cấu hình." + ex.Message
                });
            }
        }
    }
}
