using Microsoft.Extensions.Options;

using PxWeb.Code.Api2.SavedQueryBackend.FileBackend;

namespace PxWeb.UnitTests.SavedQuery
{
    [TestClass]
    public class SaveQueryFileStorgeBackendTests
    {
        [TestMethod]
        public void UpdatedRunStatistics_WhenNoFileAndNoQuery_ReturnsFalse()
        {
            // Arrange
            var settings = new SavedQueryFileStorageOptions() { Path = "SQ" };
            var options = new Mock<IOptions<SavedQueryFileStorageOptions>>();
            options.Setup(x => x.Value).Returns(settings);
            var host = new Mock<IPxHost>();
            host.Setup(h => h.RootPath).Returns(Path.GetTempPath);
            var backend = new SaveQueryFileStorgeBackend(options.Object, host.Object);

            // Act
            var result = backend.UpdateRunStatistics("no-id");

            // Assert
            Assert.IsFalse(result);
        }
    }
}
