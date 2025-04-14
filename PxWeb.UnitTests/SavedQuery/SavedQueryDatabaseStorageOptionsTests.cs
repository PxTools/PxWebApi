using System;

using Microsoft.Extensions.Options;

using PxWeb.Code.Api2.SavedQueryBackend.DatabaseBackend;

namespace PxWeb.UnitTests.SavedQuery
{
    [TestClass]
    public class SavedQueryDatabaseStorageOptionsTests
    {

        [TestMethod]
        public void CheckDefaultConstructor_ShouldInitializeProperties()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions();
            // Act & Assert
            Assert.AreEqual(string.Empty, options.ConnectionString);
            Assert.AreEqual("default", options.TargetDatabase);
            Assert.AreEqual("dbo", options.TableOwner);
            Assert.AreEqual("Microsoft", options.DatabaseVendor);
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentException_WhenDatabaseVendorIsNotSupported()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions
            {
                DatabaseVendor = "UnsupportedVendor"
            };

            var mockStorageOptions = new Mock<IOptions<SavedQueryDatabaseStorageOptions>>();
            mockStorageOptions.Setup(o => o.Value).Returns(options);


            DataSourceOptions dataSourceOptions = new DataSourceOptions
            {
                DataSourceType = "Database"
            };

            var mockDataSourceOptions = new Mock<IOptions<DataSourceOptions>>();
            mockDataSourceOptions.Setup(o => o.Value).Returns(dataSourceOptions);

            var resolver = new Mock<ITablePathResolver>();


            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => new SavedQueryDatabaseStorageBackend(mockDataSourceOptions.Object, mockStorageOptions.Object, resolver.Object));
        }



        [TestMethod]
        public void Constructor_ShouldNotThrow_WhenDatabaseVendorIsSupported()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions
            {
                DatabaseVendor = "Oracle"
            };

            var mockStorageOptions = new Mock<IOptions<SavedQueryDatabaseStorageOptions>>();
            mockStorageOptions.Setup(o => o.Value).Returns(options);


            DataSourceOptions dataSourceOptions = new DataSourceOptions
            {
                DataSourceType = "Database"
            };

            var mockDataSourceOptions = new Mock<IOptions<DataSourceOptions>>();
            mockDataSourceOptions.Setup(o => o.Value).Returns(dataSourceOptions);

            var resolver = new Mock<ITablePathResolver>();

            // Act & Assert
            var backend = new SavedQueryDatabaseStorageBackend(mockDataSourceOptions.Object, mockStorageOptions.Object, resolver.Object);
            Assert.IsNotNull(backend);
        }
    }
}
