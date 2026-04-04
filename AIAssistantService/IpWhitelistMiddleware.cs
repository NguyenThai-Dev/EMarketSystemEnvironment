using System.Net;

namespace AIAssistantService
{
    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedIps;

        public IpWhitelistMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;

            // Đọc chuỗi từ Config và cắt ra thành mảng
            var ips = configuration.GetValue<string>("AllowedIPs") ?? "127.0.0.1";
            _allowedIps = ips.Split(',').Select(ip => ip.Trim()).ToArray();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;

            bool isAllowed = false;
            foreach (var ipString in _allowedIps)
            {
                if (IPAddress.TryParse(ipString, out var address))
                {
                    if (address.Equals(remoteIp))
                    {
                        isAllowed = true;
                        break;
                    }
                }
            }

            if (!isAllowed)
            {
                context.Abort();
                return;
            }

            await _next(context);
        }
    }
}
