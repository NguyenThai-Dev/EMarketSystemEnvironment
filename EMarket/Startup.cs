using System;
using EMarket.Helpers;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;

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
            // Cho phép ứng dụng sử dụng Cookie để lưu thông tin người dùng đăng nhập
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Admin/Login/Login"),
                ExpireTimeSpan = TimeSpan.FromHours(12)
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
    }
}