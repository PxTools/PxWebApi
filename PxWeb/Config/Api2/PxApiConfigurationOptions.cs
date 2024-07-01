using System.Reflection;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Config.Api2
{
    public class PxApiConfigurationOptions
    {
        private readonly string _apiVersion;

        public PxApiConfigurationOptions()
        {
            Assembly api2ServerAssembly = Assembly.Load("PxWeb.Api2.Server");
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(api2ServerAssembly.Location);
            _apiVersion = fileVersionInfo.ProductVersion ?? "2.0.0";
        }

        public string ApiVersion => _apiVersion;

        public List<Language> Languages { get; set; } = new List<Language>();
        public string DefaultLanguage { get; set; } = String.Empty;
        public int MaxDataCells { get; set; } = 1;
        public List<ApiFeature> Features { get; set; } = new List<ApiFeature>();
        public string License { get; set; } = String.Empty;
        public List<SourceReference>? SourceReferences { get; set; }
        public Cors? Cors { get; set; }
        public int CacheTime { get; set; } = 5;
        public int PageSize { get; set; }
        public string BaseURL { get; set; } = String.Empty;
        public string RoutePrefix { get; set; } = String.Empty;
        public List<string> OutputFormats { get; set; } = new List<string>();
        public string DefaultOutputFormat { get; set; } = String.Empty;
        public bool EnableSwaggerUI { get; set; } = false;

    }
}
