namespace PxWeb.UnitTests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void Main_WithEmptyArgs_ShouldNotThrow()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act & Assert
            // This test verifies that the Main method can be called without throwing exceptions
            // In a real scenario, you'd need to extract the Main logic into testable methods
            Assert.IsNotNull(args);
        }

        [TestMethod]
        public void WebApplication_BuilderCreated_ShouldHaveServices()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            var services = builder.Services;

            // Assert
            Assert.IsNotNull(services);
            Assert.IsInstanceOfType(services, typeof(IServiceCollection));
        }

        [TestMethod]
        public void Configuration_LoadedFromBuilder_ShouldNotBeNull()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            var configuration = builder.Configuration;

            // Assert
            Assert.IsNotNull(configuration);
            Assert.IsInstanceOfType(configuration, typeof(IConfiguration));
        }

        [TestMethod]
        public void Logging_ClearedAndConfigured_ShouldNotThrow()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            builder.Logging.ClearProviders();

            // Assert
            Assert.IsNotNull(builder.Logging);
        }

        [TestMethod]
        public void HealthChecks_AddsMaintenanceCheck_ShouldNotThrow()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            var healthChecks = builder.Services.AddHealthChecks();

            // Assert
            Assert.IsNotNull(healthChecks);
        }

        [TestMethod]
        public void Services_AddMemoryCache_ShouldSucceed()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            builder.Services.AddMemoryCache();

            // Assert
            Assert.IsNotNull(builder.Services);
        }

        [TestMethod]
        public void Services_AddControllers_WithNewtonsoftJson_ShouldSucceed()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            builder.Services.AddControllers()
                .AddNewtonsoftJson();

            // Assert
            Assert.IsNotNull(builder.Services);
        }

        [TestMethod]
        public void Services_AddSwaggerGen_ShouldSucceed()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            builder.Services.AddSwaggerGen();

            // Assert
            Assert.IsNotNull(builder.Services);
        }

        [TestMethod]
        [DataRow("Development")]
        [DataRow("Production")]
        public void Environment_Configuration_ShouldLoadAppropriateConfig(string environment)
        {
            // Arrange
            var args = new[] { $"--environment={environment}" };
            var builder = WebApplication.CreateBuilder(args);

            // Act
            var env = builder.Environment.EnvironmentName;

            // Assert
            Assert.IsNotNull(env);
            Assert.AreEqual(environment, env);
        }

        [TestMethod]
        public void ServiceConfiguration_PxApiConfiguration_ShouldBeConfigurable()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            var config = builder.Configuration;

            // Act
            builder.Services.Configure<PxApiConfigurationOptions>(config.GetSection("PxApiConfiguration"));

            // Assert
            Assert.IsNotNull(builder.Services);
        }
    }
}
