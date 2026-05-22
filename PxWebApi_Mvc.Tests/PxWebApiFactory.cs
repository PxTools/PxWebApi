using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using PxWeb;

namespace PxWebApi_Mvc.Tests
{
    internal class PxWebApiFactory(string environment) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environment);
            builder.UseSetting("DataSource:DataSourceType", "CNMM");
            builder.UseSetting("PxApiConfiguration:BaseURL", "https://www.pxtools.net/");
            builder.UseSetting("PxApiConfiguration:RoutePrefix", "/api/v2");
        }
    }
}
