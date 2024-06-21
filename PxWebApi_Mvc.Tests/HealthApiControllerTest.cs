using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

using PxWeb;

namespace PxWebApi_Mvc.Tests
{
    [TestClass]
    public class HealthApiControllerTest
    {

        [TestMethod]
        public async Task IsAlive()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/health/alive");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }



        [TestMethod]
        public async Task IsReady()
        {
            await using var application = new WebApplicationFactory<Program>();
            using var client = application.CreateClient();
            using var client2 = application.CreateClient();

            int waitForMilliSeconds = 10000;
            //how soon is now

            var response = await client.GetAsync("/health/ready");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "First call to ready");

            var response2 = await client2.PostAsync("/admin/EnterMaintanceMode", null);
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode, "The call to EnterMaintanceMode");

            await Task.Delay(waitForMilliSeconds);
            response = await client.GetAsync("/health/ready");
            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode, "The call to ready after EnterMaintanceMode");

            await Task.Delay(waitForMilliSeconds);
            response2 = await client2.PostAsync("/admin/ExitMaintanceMode", null);
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode, "The call to ExitMaintanceMode");

            await Task.Delay(waitForMilliSeconds);
            response = await client.GetAsync("/health/ready");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The call to ready after ExitMaintanceMode");
        }

    }
}
