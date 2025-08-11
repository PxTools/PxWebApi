using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Px.Abstractions.Interfaces;

namespace PxWeb.Code
{
    public class MaintenanceHealthCheck : IHealthCheck
    {

        private readonly string _maintenanceFilePath;

        public MaintenanceHealthCheck(IPxHost host)
        {
            _maintenanceFilePath = Path.Combine(host.RootPath, ".maintenance");
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (File.Exists(_maintenanceFilePath))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Maintenance mode is active (.maintenance file found)."));
            }

            return Task.FromResult(HealthCheckResult.Healthy("No maintenance file found."));
        }
    }
}
