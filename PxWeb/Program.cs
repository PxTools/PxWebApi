using System.Linq;
using System.Text;

using AspNetCoreRateLimit;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using Px.Abstractions.Interfaces;

using PxWeb.Code;
using PxWeb.Code.Api2;
using PxWeb.Code.Api2.Cache;
using PxWeb.Code.Api2.DataSelection;
using PxWeb.Code.Api2.DataSource;
using PxWeb.Code.Api2.NewtonsoftConfiguration;
using PxWeb.Code.Api2.SavedQueryBackend;
using PxWeb.Code.Api2.Serialization;
using PxWeb.Code.BackgroundWorker;
using PxWeb.Filters.Api2;
using PxWeb.Helper.Api2;
using PxWeb.Mappers;
using PxWeb.Middleware;


namespace PxWeb
{
    public class Program
    {
        private const string AdminPath = "/admin";
        public static void Main(string[] args)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = WebApplication.CreateBuilder(args);

            ConfigureLogging(builder);

            // Paxiom settings
            var omit = builder.Configuration.GetSection("PxApiConfiguration:OmitContentsInTitle");
            if (omit != null && bool.TryParse(omit.Value, out bool omitContentsInTitle))
            {
                PCAxis.Paxiom.Settings.Metadata.OmitContentsVariableInTitle = omitContentsInTitle;
            }
            else
            {
                PCAxis.Paxiom.Settings.Metadata.OmitContentsVariableInTitle = true; // Default value
            }

            // Add services to the container.
            Console.WriteLine("Starting!");
            RegisterServices(builder, out var corsEnbled, out var pxApiConfiguration);
            var app = builder.Build();
            ConfigureMiddleware(app, pxApiConfiguration, corsEnbled);
            app.Run();
        }

        private static void ConfigureLogging(WebApplicationBuilder builder)
        {
            // Only use Log4Net provider
            builder.Logging.ClearProviders();
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddLog4Net("log4net.Development.config");
            }
            else
            {
                builder.Logging.AddLog4Net("log4net.config");
            }
        }

        private static void RegisterServices(WebApplicationBuilder builder, out bool corsEnabled, out PxApiConfigurationOptions pxApiConfiguration)
        {
            // needed to load configuration from appsettings.json
            builder.Services.AddOptions();

            var hBuilder = builder.Services.AddHealthChecks()
                .AddCheck<MaintenanceHealthCheck>("Maintenance", tags: ["ready"]);
            var datasourceType = builder.Configuration.GetSection("DataSource:DataSourceType").Value ?? "PX";
            if (datasourceType.Equals("CNMM", StringComparison.OrdinalIgnoreCase))
            {
                hBuilder.AddCheck<SqlDbConnectionHealthCheck>("Database", tags: ["ready"]);
            }

            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddSingleton<IPxCache>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<PxCache>>();
                var instance = new PxCache(logger);
                var clearTime = builder.Configuration.GetSection("PxApiConfiguration:CacheClearTime").Value;
                if (clearTime != null && DateTime.TryParse(clearTime, out DateTime time))
                {
                    DefaultCacheClearer.SetNextClearTime(time);
                    instance.SetCoherenceChecker(DefaultCacheClearer.CacheIsCoherent);
                }
                return instance;
            });
            builder.Services.AddSingleton<ILinkCreator, LinkCreator>();
            builder.Services.AddSingleton<ISelectionHandler, SelectionHandler>();
            builder.Services.AddSingleton<IPlacementHandler, PlacementHandler>();
            builder.Services.AddSingleton<IControllerStateProvider, ControllerStateProvider>();
            builder.Services.AddPxDataSource(builder);
            builder.Services.Configure<PxApiConfigurationOptions>(builder.Configuration.GetSection("PxApiConfiguration"));
            builder.Services.Configure<AdminProtectionConfigurationOptions>(builder.Configuration.GetSection("AdminProtection"));
            builder.Services.Configure<CacheMiddlewareConfigurationOptions>(builder.Configuration.GetSection("CacheMiddleware"));
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddSavedQuery(builder);
            builder.Services.AddTransient<IDataWorkflow, DataWorkflow>();
            builder.Services.AddTransient<ISavedQueryBackendProxy, SavedQueryBackendProxy>();
            builder.Services.AddTransient<IPxApiConfigurationService, PxApiConfigurationService>();
            builder.Services.AddTransient<IAdminProtectionConfigurationService, AdminProtectionConfigurationService>();
            builder.Services.AddTransient<ICacheMiddlewareConfigurationService, CacheMiddlewareConfigurationService>();
            builder.Services.AddTransient<ILanguageHelper, LanguageHelper>();
            builder.Services.AddTransient<IDatasetMapper, DatasetMapper>();
            builder.Services.AddTransient<ITablesResponseMapper, TablesResponseMapper>();
            builder.Services.AddTransient<ITableResponseMapper, TableResponseMapper>();
            builder.Services.AddTransient<IPxHost, PxWebHost>();
            builder.Services.AddTransient<ISerializeManager, SerializeManager>();
            builder.Services.AddTransient<ICodelistMapper, CodelistMapper>();
            builder.Services.AddTransient<ICodelistResponseMapper, CodelistResponseMapper>();
            builder.Services.AddTransient<ISelectionResponseMapper, SelectionResponseMapper>();
            builder.Services.AddTransient<ISavedQueryResponseMapper, SavedQueryResponseMapper>();
            builder.Services.AddTransient<IDefaultSelectionAlgorithm, Bjarte3>();
            builder.Services.AddHostedService<LongRunningService>();
            builder.Services.AddSingleton<BackgroundWorkerQueue>();
            builder.Services.AddPxSearchEngine(builder);
            var languages = builder.Configuration.GetSection("PxApiConfiguration:Languages")
                .AsEnumerable()
                .Where(p => p.Value != null && p.Key.ToLower().Contains("id"))
                .Select(p => p.Value ?? "")
                .ToList();
            builder.Services.AddControllers(x =>
                x.Filters.Add(new LangValidationFilter(languages))
                )
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new BaseFirstContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    });
                    opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    opts.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ"; // UTC
                });
            builder.Services.AddSwaggerGen(c =>
            {
                c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "PxWebApi",
                    Version = "v2"
                });
            });
            builder.Services.AddSwaggerGenNewtonsoftSupport();
            corsEnabled = builder.Services.ConfigurePxCORS(builder);
            pxApiConfiguration = new PxApiConfigurationOptions();
            builder.Configuration.Bind("PxApiConfiguration", pxApiConfiguration);
        }

        private static void ConfigureMiddleware(WebApplication app, PxApiConfigurationOptions pxApiConfiguration, bool corsEnabled)
        {
            app.UseMiddleware<GlobalRoutePrefixMiddleware>(pxApiConfiguration.RoutePrefix);
            app.UsePathBase(new PathString(pxApiConfiguration.RoutePrefix));
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    if (!(pxApiConfiguration.EnableAllEndpointsSwaggerUI || app.Environment.IsDevelopment()))
                    {
                        swaggerDoc.Paths = RemoveAdminEndpoints(swaggerDoc.Paths);
                    }
                    swaggerDoc.Servers = Program.GetOpenApiServers(pxApiConfiguration.BaseURL, pxApiConfiguration.RoutePrefix);
                });
            });
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = string.Empty;
                options.SwaggerEndpoint("swagger/v2/swagger.json", "PxWebApi 2.0");
            });
            if (corsEnabled)
            {
                app.UseCors();
                app.UseOptions();
            }
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.UseWhen(context => context.Request.Path.StartsWithSegments(pxApiConfiguration.RoutePrefix + AdminPath) || context.Request.Path.StartsWithSegments(AdminPath), appBuilder =>
                {
                    appBuilder.UseAdminProtectionIpWhitelist();
                    appBuilder.UseAdminProtectionKey();
                });
            }
            app.MapHealthChecks("/healthz/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });
            app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("ready")
            });
            app.MapControllers();
            if (!app.Environment.IsDevelopment())
            {
                app.UseIpRateLimiting();
            }
            app.UseWhen(context => !(context.Request.Path.StartsWithSegments(pxApiConfiguration.RoutePrefix + AdminPath) || context.Request.Path.StartsWithSegments(AdminPath) || context.Request.Path.StartsWithSegments(pxApiConfiguration.RoutePrefix + "/healthz") || context.Request.Path.StartsWithSegments("/healthz")), appBuilder =>
            {
                appBuilder.UseUsageLogMiddleware();
                appBuilder.UseCacheMiddleware();
            });
        }

        private static OpenApiPaths RemoveAdminEndpoints(OpenApiPaths paths)
        {
            var openApiPaths = new OpenApiPaths();
            foreach (var path in paths)
            {
                if (!path.Key.StartsWith(AdminPath))
                {
                    openApiPaths.Add(path.Key, path.Value);
                }
            }
            return openApiPaths;
        }

        private static List<OpenApiServer> GetOpenApiServers(string pxApiConfiguration_BaseURL, string pxApiConfiguration_RoutePrefix)
        {
            var part1 = "";
            if (!string.IsNullOrEmpty(pxApiConfiguration_BaseURL))
            {
                part1 = (new Uri(pxApiConfiguration_BaseURL)).PathAndQuery;
                if (string.IsNullOrEmpty(part1) || part1 == "/")
                {
                    part1 = "";
                }
            }
            return new List<OpenApiServer>
            {
                new() { Url = part1 + pxApiConfiguration_RoutePrefix }
            };
        }
    }
}
