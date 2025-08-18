using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using PCAxis.Sql.ApiUtils;

namespace PxWeb.Code
{
    public class SqlDbConnectionHealthCheck : IHealthCheck
    {

        private readonly string _query;

        public SqlDbConnectionHealthCheck(IOptions<CnmmConfigurationOptions> configOptions)
        {
            _query = configOptions.Value.HealthCheckQuery ?? CnmmConfigurationOptions.DEFAULT_QUERY;
        }


        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ApiUtilStatic.IsDbConnectionHealthy(_query))
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Db connection ok!"));
                }
                return Task.FromResult(HealthCheckResult.Unhealthy("Failed to query database!"));
            }
            catch
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Failed to query database!"));
            }

        }
    }
}
