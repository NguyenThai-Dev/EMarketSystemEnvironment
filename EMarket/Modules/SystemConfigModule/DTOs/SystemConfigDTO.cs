namespace EMarket.Modules.SystemConfigModule.DTOs
{
    public class SystemConfigDTO
    {
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string Description { get; set; }
    }

    // ViewModel cho View
    public class SystemConfigViewModel
    {
        public bool EmailEnabled { get; set; }
        public bool TelegramEnabled { get; set; }
        public string TelegramToken { get; set; }
        public string TelegramChatId { get; set; }
        public string AppBaseUrl { get; set; }
        public string MailHost { get; set; }
        public string MailPassword { get; set; }
        public string MailDisplayName { get; set; }
        public string VAT { get; set; }
        public string BankID { get; set; }
        public string BankNum { get; set; }
        public string VipDiscount { get; set; }
        public string MemberDiscount { get; set; }
        public int PointExchangeRate { get; set; }
        public int PointEarnedRate { get; set; }
    }

    // DTO hứng dữ liệu từ JS gửi lên
    public class ConfigUpdateDTO
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}