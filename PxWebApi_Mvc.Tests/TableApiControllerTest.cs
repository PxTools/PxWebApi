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

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "ListAllTables.json"));

            //updated causes problems. When expected and actual is made in different places and input is in localtime.
            Util.AssertJson(rawExpected, rawActual, ["updated"]);
        }



        [TestMethod]
        public async Task GetTableById_tab004()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/tables/tab004?lang=en");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "TableById_tab004.json"));

            Util.AssertJson(rawExpected, rawActual, ["updated"]);

        }

        [TestMethod]
        public async Task GetMetadataById_tab004_js2()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/tables/tab004/metadata?lang=en");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "MetadataById_tab004_js2.json"));

            Util.AssertJson(rawExpected, rawActual, ["updated"]);

        }

        [TestMethod]
        public async Task GetMetadataById_tab003_js2()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/tables/tab003/metadata?lang=en");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "MetadataById_tab003_js2.json"));

            Util.AssertJson(rawExpected, rawActual, ["updated", "nextUpdate"]);

        }

        [TestMethod]
        public async Task GetTableData_WhenDefault_ShoudlReturnOK()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            // Act
            var response = await client.GetAsync("/tables/tab003/data?lang=en");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);


        }

        [TestMethod]
        public async Task GetTableData_valueCodes_ShoudlReturnOK()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            // Act
            var response = await client.GetAsync("/tables/TAB004/data?valueCodes[ContentsCode]=Emission&valueCodes[TIME]=2017&valueCodes[GREENHOUSEGAS]=CO2&valueCodes[SECTOR]=7.0");

            string rawActual = await response.Content.ReadAsStringAsync();

            string rawExpected = File.ReadAllText(Path.Combine(Util.ExpectedJsonDir(), "tab4_fewcells.px"));

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual(rawActual, rawExpected);
        }

        [TestMethod]
        public async Task GetTableData_ShoudlReturnBadRequest_WhenValueCodes_withoutVaiable()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            // Act
            var response = await client.GetAsync("/tables/TAB004/data?valueCodes=Emission&valueCodes[TIME]=2017&valueCodes[GREENHOUSEGAS]=CO2&valueCodes[SECTOR]=7.0");
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetTableData_ShoudlReturnBadRequest_WhenTableIdDoesNotExist()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            // Act
            var response = await client.GetAsync("/tables/tabXYZ/data?lang=en");

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);


        }

    }
}
