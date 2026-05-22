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
        [TestMethod]
        [DataRow("Development")]
        [DataRow("Production")]
        public async Task RunMain(string environment)
        {
            await using var main = new PxWebApiFactory(environment);
            var client = main.CreateClient();
            var response = await client.GetAsync("/");
            Assert.IsTrue(response.IsSuccessStatusCode, $"Expected successful response from / endpoint, but got {(int)response.StatusCode} {response.ReasonPhrase}");

        }
    }
}
