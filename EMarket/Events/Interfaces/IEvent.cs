using System.Threading.Tasks;

namespace EMarket.Events.Interfaces
{
    // Interface đánh dấu đây là một Event
    public interface IEvent { }

    // Interface cho người xử lý (Handler)
    public interface IEventHandler<T> where T : IEvent
    {
        Task HandleAsync(T domainEvent);
    }

    // Interface cho bộ điều phối (Dispatcher)
    public interface IEventDispatcher
    {
        Task DispatchAsync<T>(T domainEvent) where T : IEvent;
        Task ExecuteHandlerAsync<T>(T domainEvent) where T : IEvent;
    }
}
