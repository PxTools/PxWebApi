namespace PxWeb.UnitTests
{
    [TestClass]
    public class ProgramTests
    {
        // [TestMethod]
        // [DataRow("Development")]
        // [DataRow("Production")]
        // public void Environment_Configuration_ShouldLoadAppropriateConfig(string environment)
        // {
        //     // Arrange
        //     var args = new[] { $"--environment={environment}" };
        //     var builder = WebApplication.CreateBuilder(args);

        //     // Act
        //     var env = builder.Environment.EnvironmentName;

        //     // Assert
        //     Assert.IsNotNull(env);
        //     Assert.AreEqual(environment, env);
        // }

        // [TestMethod]
        // public void ConfigureLogging_DevelopmentEnvironment_DoesNotThrow()
        // {
        //     // Arrange
        //     var builder = WebApplication.CreateBuilder(["--environment=Development"]);
        //     var configureLogging = typeof(Program).GetMethod("ConfigureLogging", BindingFlags.NonPublic | BindingFlags.Static);
        //     Assert.IsNotNull(configureLogging, "Expected Program.ConfigureLogging to be available via reflection.");

        //     // Act
        //     configureLogging.Invoke(null, [builder]);

        //     // Assert
        //     Assert.IsNotNull(builder.Logging); // if no exception was thrown, logging configuration succeeded
        // }

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
