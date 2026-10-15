using System.Net;
using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Newtonsoft.Json;

using PxWeb;
using PxWeb.Api2.Server.Models;
using PxWeb.Code.Api2;

using PxWebApi_Mvc.Tests.Wrappers;

namespace PxWebApi_Mvc.Tests
{
    [TestClass]
    public class SavedQueryApiControllerTests
    {
        public TestContext TestContext { get; set; }

        private const string SavedQuery = @"{
                            ""language"": ""en"",
                            ""tableId"": ""TAB001"",
                            ""outputFormat"": ""px"",
                            ""outputFormatParams"": [],
                            ""selection"": {
                                ""placement"": { ""heading"": [], ""stub"":[""ContentsCode""]},
                                ""selection"": [
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
        public async Task GetSavedQuery_WhenHavingDots_ShoudlReturnFileNotFound()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync(@"/savedqueries/..\tab002");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetSavedQuery_WhenOK_ShoudlReturnSaveQuery()
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
            var actualQuery = JsonConvert.DeserializeObject<SavedQueryResponse>(rawActual)?.SavedQuery;

            Assert.IsNotNull(actualQuery);
            Assert.IsNotNull(actualQuery.Id);
            Assert.AreEqual(query?.Id, actualQuery.Id);

            Assert.AreEqual("en", actualQuery.Language);
            Assert.AreEqual("*", actualQuery.Selection.Selection.FirstOrDefault(v => v.VariableCode == "REGION")?.ValueCodes[0]);

        }

        [TestMethod]
        public async Task GetSavedQueryData_WhenOK_ShoudlReturnOK()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var content = new StringContent(SavedQuery, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/savedqueries", content);

            var rawQuery = await response.Content.ReadAsStringAsync();
            var query = JsonConvert.DeserializeObject<SavedQuery>(rawQuery);

            // Act
            response = await client.GetAsync($"/savedqueries/{query?.Id}/data");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetSavedQueryData_WhenIdWithDots_ShoudlReturnBadRequest()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/savedqueries/..test/data");

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetSavedQueryData_WhenBadId_ShoudlReturnNotFound()
        {
            // Arrange
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            // Act
            var response = await client.GetAsync($"/savedqueries/no-id/data");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Saved_query_that_takes_too_long_to_execute_should_not_be_created()
        {
            await using var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IDataWorkflow>();
                        services.AddSingleton<IDataWorkflow, CancellationAwareDataWorkflow>();

                        services.PostConfigure<RequestTimeoutOptions>(options =>
                        {
                            options.Policies["GetTableDataTimeout"] = new RequestTimeoutPolicy
                            {
                                Timeout = TimeSpan.FromMilliseconds(1),
                                TimeoutStatusCode = StatusCodes.Status504GatewayTimeout
                            };
                        });
                    });
                });

            using var client = application.CreateClient();


            var content = new StringContent(SavedQuery, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/savedqueries", content, TestContext.CancellationToken);

            Assert.AreEqual(HttpStatusCode.GatewayTimeout, response.StatusCode);
        }
    }
}
