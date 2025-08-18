using System.Threading;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using PxWeb.Code;

namespace PxWeb.UnitTests.Helpers
{

    [TestClass]
    public class MaintenanceHealthCheckTests
    {

        [TestMethod]
        public void When_NoMaintenanceFile_Then_HealthCheckIsHealthy()
        {
            var host = new Mock<IPxHost>();
            var tempPath = Path.GetTempPath();

            host.Setup(h => h.RootPath).Returns(tempPath);

            // Arrange
            var healthCheck = new MaintenanceHealthCheck(host.Object);

            // Act
            var context = new HealthCheckContext();
            var cancellationToken = CancellationToken.None;
            var result = healthCheck.CheckHealthAsync(context, cancellationToken).Result;
            // Assert
            Assert.AreEqual(result.Status, HealthStatus.Healthy);
        }


        [TestMethod]
        public void When_MaintenanceFile_Then_HealthCheckIsDegraded()
        {
            var host = new Mock<IPxHost>();
            var tempPath = Path.GetTempPath();
            var maintenanceFilePath = Path.Combine(tempPath, ".maintenance");

            if (!File.Exists(maintenanceFilePath))
            {
                File.Create(maintenanceFilePath).Close();
            }

            host.Setup(h => h.RootPath).Returns(tempPath);

            // Arrange
            var healthCheck = new MaintenanceHealthCheck(host.Object);

            // Act
            var context = new HealthCheckContext();
            var cancellationToken = CancellationToken.None;
            var result = healthCheck.CheckHealthAsync(context, cancellationToken).Result;
            // Assert
            Assert.AreEqual(result.Status, HealthStatus.Degraded);

            File.Delete(maintenanceFilePath);
        }

    }
}
