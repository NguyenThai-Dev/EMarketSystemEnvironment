using System;
using System.IO;
using System.Linq;
using System.Web.Http;
using EMarket;
using Swashbuckle.Application;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace EMarket
{
    /// <summary>
    /// Configures Swagger for the EMarket API.
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Registers the Swagger configuration.
        /// </summary>
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    // API version
                    c.SingleApiVersion("v1", "EMarket API");

                    // Bật XML comment nếu bạn dùng /// trong Controller và Model
                    var xmlPath = GetXmlCommentsPath();
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }

                    // Tránh lỗi trùng route (Web API hay bị)
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                    // Nếu có enum, mô tả bằng string (đẹp + dễ đọc)
                    c.DescribeAllEnumsAsStrings();
                })
                .EnableSwaggerUi(c =>
                {
                    // Bật Pretty UI
                    c.DocExpansion(DocExpansion.List);
                });
        }

        /// <summary>
        /// Gets the path to the XML comments file.
        /// </summary>
        /// <returns>The XML comments file path.</returns>
        private static string GetXmlCommentsPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "EMarket.XML");
        }
    }
}
