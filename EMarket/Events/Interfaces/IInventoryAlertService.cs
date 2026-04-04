using System.Threading.Tasks;

namespace EMarket.Events.Interfaces
{
    public interface IInventoryAlertService
    {
        Task CheckAndSendAlertsAsync();

    }
}
