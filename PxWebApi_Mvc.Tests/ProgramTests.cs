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
    }
}
