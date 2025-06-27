using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using PxWeb.Middleware;

namespace PxWeb.UnitTests.Admin
{
    [TestClass]
    public class AdminProtectionIpWhitelistMiddlewareTests
    {

        [TestMethod]
        public async Task Invoke_IpIsNotWhitelisted_ReturnsUnauthorized()
        {
            // Arrange
            Mock<RequestDelegate> nextMock = new Mock<RequestDelegate>();

            Mock<HttpContext> _httpContextMock = new Mock<HttpContext>(); ;
            Mock<IAdminProtectionConfigurationService> adminProtectionConfigurationServiceMock = new Mock<IAdminProtectionConfigurationService>();
            adminProtectionConfigurationServiceMock.Setup(c => c.GetConfiguration()).Returns(new AdminProtectionConfigurationOptions
            {
                IpWhitelist = new List<string> { "172.17.0.9/16" }
            });

            Mock<ILogger<AdminProtectionIpWhitelistMiddleware>> loggerMock = new Mock<ILogger<AdminProtectionIpWhitelistMiddleware>>();


            AdminProtectionIpWhitelistMiddleware middleware = new AdminProtectionIpWhitelistMiddleware(nextMock.Object, adminProtectionConfigurationServiceMock.Object, loggerMock.Object);

            _httpContextMock.Setup(c => c.Connection.RemoteIpAddress).Returns(IPAddress.Parse("192.168.0.1"));
            var responseMock = new Mock<HttpResponse>();
            _httpContextMock.Setup(c => c.Response).Returns(responseMock.Object);

            // Act
            await middleware.Invoke(_httpContextMock.Object);

            // Assert
            responseMock.VerifySet(r => r.StatusCode = StatusCodes.Status401Unauthorized);
            nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
        }


        [TestMethod]
        public async Task Invoke_IpIsNotWhitelisted_CallsNextOnce()
        {
            // Arrange
            Mock<RequestDelegate> nextMock = new Mock<RequestDelegate>();

            Mock<HttpContext> _httpContextMock = new Mock<HttpContext>(); ;
            Mock<IAdminProtectionConfigurationService> adminProtectionConfigurationServiceMock = new Mock<IAdminProtectionConfigurationService>();
            adminProtectionConfigurationServiceMock.Setup(c => c.GetConfiguration()).Returns(new AdminProtectionConfigurationOptions
            {
                IpWhitelist = new List<string> { "192.168.0.1" }
            });


            Mock<ILogger<AdminProtectionIpWhitelistMiddleware>> loggerMock = new Mock<ILogger<AdminProtectionIpWhitelistMiddleware>>();

            AdminProtectionIpWhitelistMiddleware middleware = new AdminProtectionIpWhitelistMiddleware(nextMock.Object, adminProtectionConfigurationServiceMock.Object, loggerMock.Object);

            _httpContextMock.Setup(c => c.Connection.RemoteIpAddress).Returns(IPAddress.Parse("192.168.0.1"));
            var responseMock = new Mock<HttpResponse>();
            _httpContextMock.Setup(c => c.Response).Returns(responseMock.Object);

            // Act
            await middleware.Invoke(_httpContextMock.Object);

            // Assert
            //responseMock.VerifySet(r => r.StatusCode = StatusCodes.Status401Unauthorized);
            nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }

    }
}
