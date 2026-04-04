using System.Threading.Tasks;

namespace EMarket.Events.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }

}
