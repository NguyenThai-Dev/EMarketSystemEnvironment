using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using EMarket.Events.Class;
using EMarket.Events.Interfaces;

namespace EMarket.Events.Implementations
{
    public class ErrorLoggingInterceptor : IInterceptor
    {
        private readonly IEventDispatcher _dispatcher;

        public ErrorLoggingInterceptor(IEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget;
            bool isAsync = typeof(Task).IsAssignableFrom(method.ReturnType);

            if (!isAsync)
            {
                try
                {
                    invocation.Proceed();
                }
                catch (Exception ex)
                {
                    // Không await vì đây là method đồng bộ, nhưng vẫn đẩy vào Dispatcher chạy ngầm được
                    LogException(ex, invocation);
                    throw;
                }
            }
            else
            {
                invocation.Proceed();
                // Xử lý Task hoặc Task<T>
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
            }
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            try
            {
                await task; // Phải giữ nguyên context ở đây để không mất Scope của Service chính
            }
            catch (Exception ex)
            {
                LogException(ex, invocation);
                throw;
            }
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                LogException(ex, invocation);
                throw;
            }
        }

        private void LogException(Exception ex, IInvocation invocation)
        {
            if (!ex.Data.Contains("IsLogged"))
            {
                ex.Data["IsLogged"] = true;

                // BƯỚC QUAN TRỌNG: Trích xuất mọi dữ liệu cần thiết ra biến cục bộ
                // Đừng truyền 'invocation' vào trong luồng chạy ngầm của Dispatcher
                var logInfo = new AppLogEvent
                {
                    LogLevel = "ERROR",
                    Logger = invocation.TargetType.Name,
                    Message = $"[Service Error] tại {invocation.Method.Name}",
                    Exception = ex.ToString()
                };

                // Bắn event đi. Vì InMemoryEventDispatcher của bạn đã có Task.Run + BeginScope,
                // nên việc gọi DispatchAsync ở đây là cực kỳ an toàn.
                // Dùng gạch dưới '_' để báo hiệu Fire and Forget có chủ đích.
                _ = _dispatcher.DispatchAsync(logInfo);
            }
        }
    }
}