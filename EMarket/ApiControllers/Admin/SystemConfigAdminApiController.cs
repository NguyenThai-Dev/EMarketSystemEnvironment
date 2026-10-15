using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using EMarket.Modules.SystemConfigModule.DTOs;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;

namespace EMarket.ApiControllers.Admin
{
    /// <summary>
    /// REST Web API for System Configurations (VAT, Bank, Points, Notifications).
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/admin/system-config")]
    public class SystemConfigAdminApiController : ApiController
    {
        private readonly ISystemConfigService _configService;

        public SystemConfigAdminApiController(ISystemConfigService configService)
        {
            _configService = configService;
        }

        /// <summary>
        /// Lấy toàn bộ cấu hình hệ thống (dạng object tổng hợp).
        /// </summary>
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetSystemConfig()
        {
            var model = new SystemConfigViewModel
            {
                EmailEnabled = await _configService.IsEmailEnabledAsync(),
                TelegramEnabled = await _configService.IsTelegramEnabledAsync(),
                TelegramToken = await _configService.GetTelegramTokenAsync(),
                TelegramChatId = await _configService.GetTelegramChatIdAsync(),
                AppBaseUrl = await _configService.GetAppBaseUrl(),
                MailHost = await _configService.GetMailHost(),
                MailPassword = await _configService.GetMailHostPass(),
                MailDisplayName = await _configService.GetEmailDisplayNameAsync(),
                VAT = await _configService.GetEMarketVAT(),
                BankID = await _configService.GetEMarketBankID(),
                BankNum = await _configService.GetEMarketBankNum(),
                VipDiscount = await _configService.GetVIPDiscount(),
                MemberDiscount = await _configService.GetMemberDiscount(),
                PointExchangeRate = await _configService.GetEMarketPointExchnageRate(),
                PointEarnedRate = await _configService.GetEMarketPointEarnedRate()
            };
            return Ok(model);
        }

        /// <summary>
        /// Lấy danh sách tất cả các cặp Key-Value cấu hình.
        /// </summary>
        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAllConfigs()
        {
            var configs = await _configService.GetAllConfigsAsync();
            return Ok(configs);
        }

        /// <summary>
        /// Lấy giá trị của một cấu hình theo Key.
        /// </summary>
        [HttpGet, Route("value/{key}")]
        public async Task<IHttpActionResult> GetConfigValue(string key)
        {
            var val = await _configService.GetConfigValueAsync(key);
            return Ok(new { key, value = val });
        }

        /// <summary>
        /// Cập nhật danh sách các cấu hình hệ thống.
        /// </summary>
        [HttpPost, Route("update")]
        public async Task<IHttpActionResult> UpdateConfig([FromBody] List<ConfigUpdateDTO> configs)
        {
            if (configs == null || configs.Count == 0)
                return BadRequest("Danh sách cấu hình cần cập nhật không hợp lệ.");

            foreach (var item in configs)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                    continue;

                var updated = await _configService.UpdateConfigAsync(item.Key, item.Value);
                if (!updated)
                {
                    return Ok(new { success = false, message = $"Cập nhật thất bại cho cấu hình: {item.Key}" });
                }
            }

            return Ok(new { success = true, message = "Cập nhật cấu hình thành công!" });
        }
    }
}
