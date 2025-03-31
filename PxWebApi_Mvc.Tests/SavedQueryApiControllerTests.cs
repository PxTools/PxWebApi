using System.Net;
using System.Text;

using Microsoft.AspNetCore.Mvc.Testing;

using Newtonsoft.Json;

using PxWeb;
using PxWeb.Api2.Server.Models;

namespace PxWebApi_Mvc.Tests
{
    [TestClass]
    public class SavedQueryApiControllerTests
    {
        private const string SavedQuery = @"{
                            ""language"": ""en"",
                            ""tableId"": ""TAB001"",
                            ""outputFormat"": ""px"",
                            ""outputFormatParams"": [],
                            ""selection"": {""selection"": [
                                {
                                    ""variableCode"": ""ContentsCode"",
                                    ""valueCodes"": [
                                        ""HNMGA""
                                    ]
                                },
                                {
                                    ""variableCode"": ""TIME"",
                                    ""valueCodes"": [
                                        ""2001""
                                    ]
                                },
                                {
                                    ""variableCode"": ""REGION"",
                                    ""valueCodes"": [
                                        ""*""
                                    ]
                                },
                                {
                                    ""variableCode"": ""SEX"",
                                    ""valueCodes"": [
                                        ""F"",
                                        ""M""
                                    ]
                                },
                                {
                                    ""variableCode"": ""age"",
                                    ""valueCodes"": [""Total""],

                                }
                            ]}
                        }";
        [TestMethod]
        public async Task CreateSavedQuery_WhenOK_ShoudlReturnSameQueryWithId()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var content = new StringContent(SavedQuery, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/savedqueries", content);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var rawActual = await response.Content.ReadAsStringAsync();
            var actualQuery = JsonConvert.DeserializeObject<SavedQuery>(rawActual);

            Assert.IsNotNull(actualQuery);
            Assert.IsNotNull(actualQuery.Id);

        }

        [TestMethod]
        public async Task CreateSavedQuery_WhenNotOK_ShoudlReturnBadRequest()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/savedqueries", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [TestMethod]
        public async Task GetSavedQuery_WhenNotOK_ShoudlReturnFileNotFound()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/savedqueries/no-id");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetSavedQuery_WhenOK_ShoudlReturnSameQuery()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var content = new StringContent(SavedQuery, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/savedqueries", content);

            var rawQuery = await response.Content.ReadAsStringAsync();
            var query = JsonConvert.DeserializeObject<SavedQuery>(rawQuery);

            // Act
            response = await client.GetAsync($"/savedqueries/{query?.Id}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var rawActual = await response.Content.ReadAsStringAsync();
            var actualQuery = JsonConvert.DeserializeObject<SavedQuery>(rawActual);

            Assert.IsNotNull(actualQuery);
            Assert.IsNotNull(actualQuery.Id);
            Assert.AreEqual(query?.Id, actualQuery.Id);

            Assert.AreEqual("en", actualQuery.Language);
            Assert.AreEqual("*", actualQuery.Selection.Selection.FirstOrDefault(v => v.VariableCode == "REGION")?.ValueCodes[0]);

        }
    }
}
