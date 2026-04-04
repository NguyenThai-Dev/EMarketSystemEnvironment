using System.Collections.Generic;
using System.Threading.Tasks;
using EMarket.Modules.SystemConfigModule.DTOs;

namespace EMarket.Modules.SystemConfigModule.Services.Interfaces
{
    public interface ISystemConfigService
    {
        Task<string> GetConfigValueAsync(string key);
        Task<List<SystemConfigDTO>> GetAllConfigsAsync();
        Task<bool> UpdateConfigAsync(string key, string value);

        Task<bool> IsEmailEnabledAsync();
        Task<bool> IsTelegramEnabledAsync();
        Task<string> GetTelegramTokenAsync();
        Task<string> GetTelegramChatIdAsync();
        Task<string> GetAppBaseUrl();
        Task<string> GetMailHost();
        Task<string> GetMailHostPass();
        Task<string> GetEmailDisplayNameAsync();
        Task<string> GetEMarketVAT();
        Task<string> GetEMarketBankID();
        Task<string> GetEMarketBankNum();
        Task<string> GetVIPDiscount();
        Task<string> GetMemberDiscount();
        Task<int> GetEMarketPointExchnageRate();
        Task<int> GetEMarketPointEarnedRate();
    }
}
