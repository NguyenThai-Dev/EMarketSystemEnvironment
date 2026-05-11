using EMarket.Helpers;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

[assembly: OwinStartup(typeof(EMarket.Startup))]

namespace EMarket
{
    public class AllowAllHangfireFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var clientID = System.Configuration.ConfigurationManager.AppSettings["ClientID"];
            var clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
            var secretKeyForJwt = System.Configuration.ConfigurationManager.AppSettings["SecretKeyForJwt"];

            // Cho phép ứng dụng sử dụng Cookie để lưu thông tin người dùng đăng nhập
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebCookieManager(),
                LoginPath = new PathString("/Admin/Login/Login"),
                ExpireTimeSpan = TimeSpan.FromHours(12),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = context =>
                    {
                        if (context.Request.Path.StartsWithSegments(new PathString("/api")))
                        {
                            context.Response.StatusCode = 401;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }
                    }
                }
            });

            // Cấu hình để sử dụng cookie tạm thời lưu thông tin về người dùng đăng nhập bằng bên thứ 3 (Google)
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Cấu hình Google Login
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = clientID,
                ClientSecret = clientSecret,
                CallbackPath = new PathString("/signin-google"),
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnApplyRedirect = context =>
                    {
                        var redirectUri = context.RedirectUri + "&prompt=select_account";
                        context.Response.Redirect(redirectUri);
                    }
                }
            });

            // =========================================================
            // CẤU HÌNH BỘ GIẢI MÃ JWT CHO WEB API
            // =========================================================
            var secretKey = secretKeyForJwt;
            var keyByteArray = Encoding.ASCII.GetBytes(secretKey);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = "eMarketServer",

                    ValidateAudience = true,
                    ValidAudience = "eMarketClient",

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyByteArray),

                    ValidateLifetime = true
                }
            });

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.ToString().ToLower();

                // Bỏ qua các file tĩnh và thư viện
                if (path.Contains("/signalr") || path.Contains("/swagger") || path.Contains("/bundles") || path.Contains("/content") || path.Contains("/assets") || path.EndsWith(".map"))
                {
                    await next();
                    return;
                }

                // 1. Kiểm tra QueryString
                if (context.Request.QueryString.HasValue)
                {
                    if (IsDangerous(context.Request.QueryString.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"[Security Block] Dangerous Query.");
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Security Violation: Dangerous Query");
                        return;
                    }
                }

                // 2. Kiểm tra Body cho POST/PUT 
                if (context.Request.Method == "POST" || context.Request.Method == "PUT")
                {
                    context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
                    using (var reader = new System.IO.StreamReader(context.Request.Body, System.Text.Encoding.UTF8, true, 1024, true))
                    {
                        var body = await reader.ReadToEndAsync();

                        if (IsDangerous(body))
                        {
                            System.Diagnostics.Debug.WriteLine($"[Security Block] Dangerous Body.");
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync("Security Violation: Dangerous Body");
                            return;
                        }
                    }
                    context.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
                }

                await next();
            });

            var options = new DashboardOptions
            {
                Authorization = new[] { new AllowAllHangfireFilter() }
            };
            app.UseHangfireDashboard("/hangfire", options);
            app.UseHangfireServer();

            var idProvider = new CustomUserIdProvider();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

            app.MapSignalR();
        }

        private bool IsDangerous(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // 1. Danh sách từ khóa nguy hiểm (XSS & SQLi phổ biến)
            string[] patterns = {
                "<script", "javascript:", "onerror=", "onload=",
                "drop table", "delete from", "xp_cmdshell"
            };

            var lowerInput = input.ToLowerInvariant();

            // 2. Check chuỗi gốc (Phòng trường hợp upload file chứa tên nguy hiểm)
            if (patterns.Any(p => lowerInput.Contains(p))) return true;

            // 3. Check chuỗi sau khi UrlDecode 
            try
            {
                var decoded = System.Web.HttpUtility.UrlDecode(lowerInput);
                if (patterns.Any(p => decoded.Contains(p))) return true;
            }
            catch
            {
                // Bỏ qua lỗi decode. Khi upload file ảnh, luồng binary decode đôi khi văng lỗi, 
                // ta cứ lờ đi vì ảnh thì làm gì có <script>
            }

            return false;
        }
    }
}