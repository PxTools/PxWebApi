using System;

using Microsoft.Extensions.Options;

using PxWeb.Code.Api2.SavedQueryBackend.DatabaseBackend;

namespace PxWeb.UnitTests.SavedQuery
{

    [TestClass]
    public class SavedQueryDatabaseStorageBackendTests
    {


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
            Assert.ThrowsExactly<ArgumentException>(() => new SavedQueryDatabaseStorageBackend(mockDataSourceOptions.Object, mockStorageOptions.Object, resolver.Object));
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

        [TestMethod]
        public void Constructor_ShouldNotThrow_WhenDatabaseVendorIsSupported2()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions
            {
                DatabaseVendor = "Microsoft"
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

        [TestMethod]
        public void Load_ShouldReturnEmptyString_WhenInvalidId()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions
            {
                DatabaseVendor = "Microsoft"
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

            var backend = new SavedQueryDatabaseStorageBackend(mockDataSourceOptions.Object, mockStorageOptions.Object, resolver.Object);

            // Act

            var result = backend.Load("invalid_id");

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Load_ShouldThrowException_WhenNoConnectionStringSpecified()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions
            {
                DatabaseVendor = "Microsoft"
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

            var backend = new SavedQueryDatabaseStorageBackend(mockDataSourceOptions.Object, mockStorageOptions.Object, resolver.Object);

            // Act & assert 
            Assert.ThrowsExactly<InvalidOperationException>(() => backend.Load("0"));

        }

        [TestMethod]
        public void UpdatedStatistics_ShouldReturnFalse_WhenInvalidId()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions
            {
                DatabaseVendor = "Microsoft"
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

            var backend = new SavedQueryDatabaseStorageBackend(mockDataSourceOptions.Object, mockStorageOptions.Object, resolver.Object);

            // Act
            var result = backend.UpdateRunStatistics("invalid_id");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Save_ShouldReturnEmptyString_WhenNoConnectionStringSpecified()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions
            {
                DatabaseVendor = "Microsoft"
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
            resolver.Setup(r => r.Resolve(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<bool>.IsAny)).Returns(string.Empty);

            var backend = new SavedQueryDatabaseStorageBackend(mockDataSourceOptions.Object, mockStorageOptions.Object, resolver.Object);

            // Act
            var result = backend.Save("", "0", "sv");

            // Assert
            Assert.AreEqual(string.Empty, result);

        }

    }
}
