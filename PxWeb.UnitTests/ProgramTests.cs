namespace PxWeb.UnitTests
{
    [TestClass]
    public class ProgramTests
    {
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
        public void ConfigureMiddleware_DevelopmentEnvironment_DoesNotThrow()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(["--environment=Development"]);
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGenNewtonsoftSupport();
            builder.Services.AddRouting();

            var app = builder.Build();

            var configureMiddleware = typeof(Program).GetMethod("ConfigureMiddleware", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(configureMiddleware, "Expected Program.ConfigureMiddleware to be available via reflection.");

            // Act
            configureMiddleware.Invoke(null, new object[] { app });

            // Assert
            Assert.IsNotNull(app); // if no exception was thrown, middleware configuration succeeded
        }

        [TestMethod]
        public void ConfigureMiddleware_ProductionEnvironment_DoesNotThrow()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(["--environment=Production"]);
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGenNewtonsoftSupport();
            builder.Services.AddRouting();
            builder.Services.AddAuthorization();
            builder.Services.AddMemoryCache();
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddSingleton<IAdminProtectionConfigurationService>(new Mock<IAdminProtectionConfigurationService>().Object);
            builder.Services.AddSingleton<ILogger<GlobalRoutePrefixMiddleware>>(new Mock<ILogger<GlobalRoutePrefixMiddleware>>().Object);

            var app = builder.Build();

            var configureMiddleware = typeof(Program).GetMethod("ConfigureMiddleware", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(configureMiddleware, "Expected Program.ConfigureMiddleware to be available via reflection.");

            // Act
            configureMiddleware.Invoke(null, new object[] { app });

            // Assert
            Assert.IsNotNull(app); // if no exception was thrown, middleware configuration succeeded
        }

        [TestMethod]
        public void ConfigureLogging_DevelopmentEnvironment_DoesNotThrow()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(["--environment=Development"]);
            var configureLogging = typeof(Program).GetMethod("ConfigureLogging", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(configureLogging, "Expected Program.ConfigureLogging to be available via reflection.");

            // Act
            configureLogging.Invoke(null, [builder]);

            // Assert
            Assert.IsNotNull(builder.Logging); // if no exception was thrown, logging configuration succeeded
        }

        [TestMethod]
        public void ConfigureLogging_ProductionEnvironment_DoesNotThrow()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder(["--environment=Production"]);
            var configureLogging = typeof(Program).GetMethod("ConfigureLogging", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(configureLogging, "Expected Program.ConfigureLogging to be available via reflection.");

            // Act
            configureLogging.Invoke(null, [builder]);

            // Assert
            Assert.IsNotNull(builder.Logging); // if no exception was thrown, logging configuration succeeded
        }

    }
}
