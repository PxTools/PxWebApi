using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

using PxWeb;

namespace PxWebApi_Mvc.Tests
{
    [TestClass]
    public class ConfigApiControllerTest
    {

        [TestMethod]
        public async Task GetApiConfiguration()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/config");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "GetApiConfiguration.json"));

            // "apiVersion" points is api-spec version, so it changes often. Will become more stable    
            Util.AssertJson(rawExpected, rawActual, ["apiVersion"]);
        }

    }
}
