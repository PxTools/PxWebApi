using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PxWeb.Code.Api2
{
    public static class CorsServiceCollectionExtensions
    {
        /// <summary>
        /// Extension method to handle CORS configuration
        /// </summary>
        public static bool ConfigurePxCORS(this IServiceCollection services, WebApplicationBuilder builder)
        {
            bool corsEnbled = false;

            try
            {
                // Read configuration for CORS enabled
                var corsEnabled = builder.Configuration.GetSection("PxApiConfiguration:Cors:Enabled").Value ?? "false";
                bool.TryParse(corsEnabled.Trim(), out corsEnbled);
            }
            catch (System.Exception)
            {
                corsEnbled = false;
                Console.WriteLine("Could not read CORS Enabled configuration");
                return false;
            }

            if (corsEnbled)
            {
                string[] origins = { "" };

                try
                {
                    // Read configuration for CORS origins
                    var originsConfig = builder.Configuration.GetSection("PxApiConfiguration:Cors:Origins").Value ?? "";
                    origins = originsConfig.Split(',', System.StringSplitOptions.TrimEntries);
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Could not read CORS origins configuration");
                    return false;
                }

                bool allowAnyOrigin = false;

                if (origins[0] == "*" || origins[0] == "")
                {
                    allowAnyOrigin = true;
                }

                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                    policy =>
                    {
                        if (allowAnyOrigin)
                        {
                            policy.AllowAnyOrigin().AllowAnyHeader();
                        }
                        else
                        {
                            policy.WithOrigins(origins).AllowAnyHeader();
                        }
                    });
                });
            }

            return corsEnbled;
        }
    }
}
