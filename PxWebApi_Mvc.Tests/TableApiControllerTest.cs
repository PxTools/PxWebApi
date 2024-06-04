using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

using PxWeb;

namespace PxWebApi_Mvc.Tests
{
    [TestClass]
    public class TableApiControllerTest
    {
        private static readonly string expectedJsonDir = Util.GetFullPathToFile(@"PxWebApi_Mvc.Tests/ExpectedJson/");

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            //expectedJsonDir = Util.GetFullPathToFile(@"PxWebApi_Mvc.Tests/ExpectedJson/");
        }


        [TestMethod]
        public async Task ListAllTables()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/api/v2/tables?lang=en");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(expectedJsonDir, "ListAllTables.json"));

            AssertJson(rawExpected, rawActual);
        }



        [TestMethod]
        public async Task GetTableById_tab004()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/api/v2/tables/tab004?lang=en");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(expectedJsonDir, "TableById_tab004.json"));

            AssertJson(rawExpected, rawActual);

        }

        [TestMethod]
        public async Task GetMetadataById_tab004()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/api/v2/tables/tab004/metadata?lang=en");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

            string rawActual = await response.Content.ReadAsStringAsync();
            string rawExpected = File.ReadAllText(Path.Combine(expectedJsonDir, "MetadataById_tab004.json"));

            AssertJson(rawExpected, rawActual);

        }




        private void AssertJson(string inExpected, string inActual)
        {
            //RegexOptions options = RegexOptions.None;
            //Regex regex = new Regex("[ ]{2,}", options);

            //string expected = regex.Replace(inExpected.Replace(Environment.NewLine, ""), " ");
            //string actual = regex.Replace(inActual.Replace(Environment.NewLine, ""), " ");


            // prettyprints, but changes & to \u0026, so it has to be applied to expected as well
            string actual = System.Text.Json.Nodes.JsonNode.Parse(inActual).ToString();
            string expected = System.Text.Json.Nodes.JsonNode.Parse(inExpected).ToString();

            //updated causes problems. When expected and actual is made in different places and input is in localtime.
            int posOfUpdatedString = expected.IndexOf("2023-05-25T13:42:00");
            actual = actual.Substring(0, posOfUpdatedString) + "XXXX-XX-XXTXX" + actual.Substring(posOfUpdatedString + 13);
            expected = expected.Substring(0, posOfUpdatedString) + "XXXX-XX-XXTXX" + expected.Substring(posOfUpdatedString + 13);


            Assert.AreEqual(expected.Substring(0, 5), actual.Substring(0, 5), "Diff in first 5.");

            for (int i = 0; i < actual.Length; i += 25)
            {
                int lengthToCompare = Math.Min(50, actual.Length - i);
                Assert.AreEqual(expected.Substring(i, lengthToCompare), actual.Substring(i, lengthToCompare));
            }

        }
    }
}
