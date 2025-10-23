using Microsoft.Extensions.Options;

namespace PxWeb.UnitTests.config
{
    [TestClass]
    public class LuceneConfigurationServiceTests
    {
        [TestMethod]
        public void GetIndexDirectory_ReturnsCorrectPathFromRelativePath()
        {
            // Arrange
            var hostPath = @"C:\inetpub\wwwwroot\api";
            var hostMock = new Mock<IPxHost>();
            hostMock.Setup(h => h.RootPath).Returns(hostPath);
            var luceneConfig = new LuceneConfigurationOptions
            {
                IndexDirectory = @"Indexes\LuceneIndex"
            };

            var optionsMock = new Mock<IOptions<LuceneConfigurationOptions>>();
            optionsMock.Setup(o => o.Value).Returns(luceneConfig);

            var service = new LuceneConfigurationService(optionsMock.Object, hostMock.Object);

            // Act
            var indexDirectoryPath = service.GetIndexDirectoryPath();

            // Assert
            Assert.StartsWith(@"C:\inetpub\wwwwroot\api", indexDirectoryPath);
        }

        [TestMethod]
        public void GetIndexDirectory_ReturnsCorrectPathFromFullyQualifiedPath()
        {
            // Arrange
            var hostPath = @"C:\inetpub\wwwwroot\api";
            var hostMock = new Mock<IPxHost>();
            hostMock.Setup(h => h.RootPath).Returns(hostPath);
            var luceneConfig = new LuceneConfigurationOptions
            {
                IndexDirectory = @"\\apishare\Indexes\LuceneIndex"
            };

            var optionsMock = new Mock<IOptions<LuceneConfigurationOptions>>();
            optionsMock.Setup(o => o.Value).Returns(luceneConfig);

            var service = new LuceneConfigurationService(optionsMock.Object, hostMock.Object);

            // Act
            var indexDirectoryPath = service.GetIndexDirectoryPath();

            // Assert
            Assert.AreEqual(@"\\apishare\Indexes\LuceneIndex", indexDirectoryPath);
        }
    }
}
