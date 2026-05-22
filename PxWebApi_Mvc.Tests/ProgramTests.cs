using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using PxWeb;

namespace PxWebApi_Mvc.Tests
{
    internal class PxWebApiFactory(string environment) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environment);
        }
    }

    [TestClass]
    public class ProgramTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataRow("Development")]
        [DataRow("Production")]
        public async Task RunMain(string environment)
        {
            await using var main = new PxWebApiFactory(environment);
            var client = main.CreateClient();
            var response = await client.GetAsync("/index.html", TestContext.CancellationToken);
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.Headers.TryGetValues("x-swagger-ui-version", out var values));
            Assert.AreEqual("5.29.2", values.Single());
        }
    }
}
