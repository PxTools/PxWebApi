using System.Text.Json;

namespace PxWebApi_Mvc.Tests
{
    [TestClass]
    public class ProgramTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataRow("Development")]
        [DataRow("Production")]
        public async Task IndexHtml_ShouldReturnSuccessAndSwaggerUiVersionHeader(string environment)
        {
            // Arrange
            await using var app = new PxWebApiFactory(environment);
            var client = app.CreateClient();

            // Act
            var response = await client.GetAsync("/index.html", TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.Headers.TryGetValues("x-swagger-ui-version", out var values));
        }

        [TestMethod]
        [DataRow("Development")]
        [DataRow("Production")]
        public async Task SwaggerJson_ShouldContainConfiguredServerUrl(string environment)
        {
            // Arrange
            await using var app = new PxWebApiFactory(environment);
            var client = app.CreateClient();

            // Act
            var response = await client.GetAsync("/swagger/v2/swagger.json", TestContext.CancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(TestContext.CancellationToken);
            using var document = JsonDocument.Parse(json);
            var serverUrl = document.RootElement
                .GetProperty("servers")[0]
                .GetProperty("url")
                .GetString();

            // Assert
            Assert.AreEqual("/api/v2", serverUrl);
        }

        [TestMethod]
        [DataRow("Development")]
        public async Task HealthCheck_ShouldReturnSuccess(string environment)
        {
            // Arrange
            await using var app = new PxWebApiFactory(environment);
            var client = app.CreateClient();

            // Act
            var live = await client.GetAsync("/healthz/live", TestContext.CancellationToken);
            var ready = await client.GetAsync("/healthz/ready", TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(live.IsSuccessStatusCode);
            Assert.IsFalse(ready.IsSuccessStatusCode, "Did not expect CNMM to be ready");
        }
    }
}
