using System.Threading.Tasks;

namespace EMarket.Events.Interfaces
{
    public interface ITelegramService
    {
        Task SendMessageAsync(string message);
    }
}
