using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Models;
using EMarket.Modules.SystemConfigModule.DTOs;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;

namespace EMarket.Modules.SystemConfigModule.Services.Implementations
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly EMarket_DBEntities _db;

        public SystemConfigService(EMarket_DBEntities db)
        {
            _db = db;
        }

        public async Task<string> GetConfigValueAsync(string key)
        {
            var config = await _db.SystemConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.config_key == key);
            return config?.config_value;
        }

        public async Task<List<SystemConfigDTO>> GetAllConfigsAsync()
        {
            return await _db.SystemConfigs
                .Select(x => new SystemConfigDTO
                {
                    ConfigKey = x.config_key,
                    ConfigValue = x.config_value,
                    Description = x.description
                }).ToListAsync();
        }

        public async Task<bool> UpdateConfigAsync(string key, string value)
        {
            var config = await _db.SystemConfigs.FirstOrDefaultAsync(x => x.config_key == key);
            if (config == null) return false;

            config.config_value = value;
            config.updated_at = DateTime.Now;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsEmailEnabledAsync()
        {
            var val = await GetConfigValueAsync("Notify_Email_Enabled");
            return val != null && val.ToLower() == "true";
        }

        public async Task<bool> IsTelegramEnabledAsync()
        {
            var val = await GetConfigValueAsync("Notify_Telegram_Enabled");
            return val != null && val.ToLower() == "true";
        }

        public async Task<string> GetTelegramTokenAsync()
        {
            return await GetConfigValueAsync("Telegram_Bot_Token") ?? "";
        }

        public async Task<string> GetTelegramChatIdAsync()
        {
            return await GetConfigValueAsync("Telegram_Group_Id") ?? "";
        }

        public async Task<string> GetAppBaseUrl()
        {
            return await GetConfigValueAsync("App_Base_Url") ?? "";
        }

        public async Task<string> GetMailHost()
        {
            return await GetConfigValueAsync("Email_Username") ?? "";
        }

        public async Task<string> GetMailHostPass()
        {
            return await GetConfigValueAsync("Email_AppPassword") ?? "";
        }

        public async Task<string> GetEmailDisplayNameAsync()
        {
            return await GetConfigValueAsync("Email_DisplayName") ?? "";
        }

        public async Task<string> GetEMarketVAT()
        {
            return await GetConfigValueAsync("EMarket_VAT") ?? "";
        }

        public async Task<string> GetEMarketBankID()
        {
            return await GetConfigValueAsync("EMarket_Bank_ID") ?? "";
        }

        public async Task<string> GetEMarketBankNum()
        {
            return await GetConfigValueAsync("EMarket_Bank_Num") ?? "";
        }

        public async Task<string> GetVIPDiscount()
        {
            return await GetConfigValueAsync("VIP_Discount") ?? "";
        }

        public async Task<string> GetMemberDiscount()
        {
            return await GetConfigValueAsync("Member_Discount") ?? "";
        }

        public async Task<int> GetEMarketPointExchnageRate()
        {
            var val = await GetConfigValueAsync("EMarket_Point_Exchange_Rate");
            int rate;
            if (int.TryParse(val, out rate))
            {
                return rate;
            }
            return 0;
        }

        public async Task<int> GetEMarketPointEarnedRate()
        {
            var val = await GetConfigValueAsync("EMarket_Point_Earned_Rate");
            int rate;
            if (int.TryParse(val, out rate))
            {
                return rate;
            }
            return 0;
        }
    }
}