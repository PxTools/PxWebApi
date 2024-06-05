using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

using PxWeb;

namespace PxWebApi_Mvc.Tests
{
    [TestClass]
    public class TableApiControllerTest
    {

        [TestMethod]
        public async Task ListAllTables()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/tables?lang=en");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "ListAllTables.json"));

            Util.AssertJson(rawExpected, rawActual);
        }



        [TestMethod]
        public async Task GetTableById_tab004()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/tables/tab004?lang=en");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "TableById_tab004.json"));

            Util.AssertJson(rawExpected, rawActual);

        }

        [TestMethod]
        public async Task GetMetadataById_tab004()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/tables/tab004/metadata?lang=en");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "MetadataById_tab004.json"));

            Util.AssertJson(rawExpected, rawActual);

        }

        [TestMethod]
        public async Task GetMetadataById_tab004_js2()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/tables/tab004/metadata?lang=en&outputFormat=json-stat2");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "MetadataById_tab004_js2.json"));

            Util.AssertJson(rawExpected, rawActual);

        }

    }
}
