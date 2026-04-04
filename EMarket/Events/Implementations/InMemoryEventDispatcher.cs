using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;
using Hangfire;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace EMarket.Events.Implementations
{
    public class InMemoryEventDispatcher : IEventDispatcher
    {
        private readonly Container _container;

        public InMemoryEventDispatcher(Container container)
        {
            _container = container;
        }

        //public async Task DispatchAsync<T>(T domainEvent) where T : IEvent
        //{
        //    Debug.WriteLine($"[Event Dispatched] {typeof(T).Name}");
        //    // Lấy danh sách các handler
        //    var handlers = _container.GetAllInstances<IEventHandler<T>>().ToList();

        //    foreach (var handler in handlers)
        //    {
        //        var handlerType = handler.GetType();

        //        // Chạy ngầm trong một Scope mới hoàn toàn
        //        _ = Task.Run(async () =>
        //        {
        //            Debug.WriteLine($"[Event Handling] {handlerType.Name} handling {typeof(T).Name}");
        //            using (AsyncScopedLifestyle.BeginScope(_container))
        //            {
        //                try
        //                {
        //                    // Lấy instance mới trong Scope này để DbContext không bị Dispose
        //                    var scopedHandler = (IEventHandler<T>)_container.GetInstance(handlerType);
        //                    await scopedHandler.HandleAsync(domainEvent);
        //                }
        //                catch (System.Exception ex)
        //                {
        //                    System.Diagnostics.Debug.WriteLine($"[Event Error] {ex.Message}");
        //                }
        //            }
        //        });
        //    }

        //    await Task.CompletedTask;
        //}
        public async Task DispatchAsync<T>(T domainEvent) where T : IEvent
        {
            Debug.WriteLine($"[Event Dispatched] {typeof(T).Name}");

            if (domainEvent is AppLogEvent)
            {
                var handlers = _container.GetAllInstances<IEventHandler<T>>().ToList();
                foreach (var handler in handlers)
                {
                    var handlerType = handler.GetType();
                    _ = Task.Run(async () =>
                    {
                        using (AsyncScopedLifestyle.BeginScope(_container))
                        {
                            try
                            {
                                var scopedHandler = (IEventHandler<T>)_container.GetInstance(handlerType);
                                await scopedHandler.HandleAsync(domainEvent);
                            }
                            catch (System.Exception ex)
                            {
                                Debug.WriteLine($"[Log Event Error] {ex.Message}");
                            }
                        }
                    });
                }
            }
            else
            {
                Debug.WriteLine($"[Enqueue Event] {typeof(T).Name} to Hangfire");
                BackgroundJob.Enqueue<IEventDispatcher>(d => d.ExecuteHandlerAsync<T>(domainEvent));
            }

            await Task.CompletedTask;
        }

        public async Task ExecuteHandlerAsync<T>(T domainEvent) where T : IEvent
        {
            Debug.WriteLine($"[Hangfire Worker START] Handling event: {typeof(T).FullName}");

            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                try
                {
                    // Kiểm tra xem Container có giải quyết được IEventHandler cho T không
                    var registrations = _container.GetCurrentRegistrations()
                        .Where(r => r.ServiceType == typeof(IEventHandler<T>));

                    Debug.WriteLine($"[DI Check] Registrations found: {registrations.Count()}");

                    var handlers = _container.GetAllInstances<IEventHandler<T>>().ToList();
                    Debug.WriteLine($"[DI Check] Actual Handlers resolved: {handlers.Count}");

                    foreach (var handler in handlers)
                    {
                        Debug.WriteLine($"[Executing] {handler.GetType().Name}...");
                        await handler.HandleAsync(domainEvent);
                        Debug.WriteLine($"[Finished] {handler.GetType().Name}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[CRITICAL ERROR] ExecuteHandlerAsync: {ex.ToString()}");
                    throw; // Phải throw để Hangfire Dashboard hiện lỗi
                }
            }
        }
    }
}