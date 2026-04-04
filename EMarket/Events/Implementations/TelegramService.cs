using System.Diagnostics;
using System.Threading.Tasks;
using EMarket.Events.Interfaces;
using EMarket.Models;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;

namespace EMarket.Events.Implementations
{
    public class TelegramService : ITelegramService
    {
        private readonly EMarket_DBEntities _db;
        private readonly ISystemConfigService _systemConfigService;

        public TelegramService(EMarket_DBEntities db, ISystemConfigService systemConfigService)
        {
            _db = db;
            _systemConfigService = systemConfigService;
        }

        public async Task SendMessageAsync(string message)
        {
            Debug.WriteLine("TelegramService SendMessageAsync called with message: " + message);
            var sendMessageEnabled = await _systemConfigService.IsTelegramEnabledAsync();
            if (!sendMessageEnabled)
            {
                return;
            }

            var token = await _systemConfigService.GetTelegramTokenAsync();
            var chatId = await _systemConfigService.GetTelegramChatIdAsync();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(chatId))
            {
                return;
            }

            using (var client = new System.Net.Http.HttpClient())
            {
                var url = $"https://api.telegram.org/bot{token}/sendMessage";
                var payload = new
                {
                    chat_id = chatId,
                    text = message,
                    parse_mode = "HTML"
                };

                var content = new System.Net.Http.StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                await client.PostAsync(url, content);
            }
        }
    }
}