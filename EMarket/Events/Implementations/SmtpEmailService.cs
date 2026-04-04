using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EMarket.Events.Interfaces;
using EMarket.Modules.SystemConfigModule.Services.Interfaces;


namespace EMarket.Modules.InventoryModule.Services.Implementations
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _enableSsl;
        private readonly ISystemConfigService _systemConfigService;

        public SmtpEmailService(ISystemConfigService systemConfigService)
        {
            _host = ConfigurationManager.AppSettings["SmtpHost"];
            _port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            _username = ConfigurationManager.AppSettings["SmtpUsername"];
            _password = ConfigurationManager.AppSettings["SmtpPassword"];
            _fromEmail = ConfigurationManager.AppSettings["SmtpFromEmail"];
            _fromName = ConfigurationManager.AppSettings["SmtpFromName"];
            _enableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"]);
            _systemConfigService = systemConfigService;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            Debug.WriteLine("SmtpEmailService SendAsync called with toEmail: " + toEmail + ", subject: " + subject);
            if (!await _systemConfigService.IsEmailEnabledAsync()) return;

            var dbEmail = await _systemConfigService.GetMailHost();
            var dbPassword = await _systemConfigService.GetMailHostPass();
            var dbDisplayName = await _systemConfigService.GetEmailDisplayNameAsync();

            string finalEmail = !string.IsNullOrEmpty(dbEmail) ? dbEmail : _fromEmail;
            string finalPassword = !string.IsNullOrEmpty(dbPassword) ? dbPassword : _password;
            string finalName = !string.IsNullOrEmpty(dbDisplayName) ? dbDisplayName : _fromName;

            using (var client = new SmtpClient(_host, _port))
            {
                client.Credentials = new NetworkCredential(finalEmail, finalPassword);
                client.EnableSsl = _enableSsl;

                var mail = new MailMessage
                {
                    From = new MailAddress(finalEmail, finalName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                try
                {
                    Debug.WriteLine("Đang gửi Mail đến: " + toEmail);
                    await client.SendMailAsync(mail);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi gửi Mail: " + ex.Message);
                    throw;
                }
            }
        }
    }
}