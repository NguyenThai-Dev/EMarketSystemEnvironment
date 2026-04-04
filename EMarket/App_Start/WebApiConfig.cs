using System.Web.Http;

namespace EMarket
{
    /// <summary>
    /// Provides configuration for Web API routes.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers Web API configuration and services.
        /// </summary>
        /// <param name="config">The HTTP configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Kích hoạt [RoutePrefix] và [Route]
            config.MapHttpAttributeRoutes();
            var appJsonFormatter = config.Formatters.JsonFormatter;
            appJsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));

            // Xóa XML Formatter cho sạch máy
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}