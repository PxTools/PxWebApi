namespace PxWeb.Config.Api2
{
    public class CnmmConfigurationOptions
    {
        public const string DEFAULT_QUERY = "select 1";

        public string DatabaseID { get; set; } = string.Empty;

        public string HealthCheckQuery { get; set; } = DEFAULT_QUERY;
    }
}
