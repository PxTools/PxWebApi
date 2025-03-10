using Microsoft.Extensions.Logging;

using Moq;

using PCAxis.Menu;

using Px.Abstractions.Interfaces;


namespace Px.Search.Tests
{
    [TestClass]
    public sealed class IndexerTests
    {
        [TestMethod]
        public void IndexDatabase_OnlyOneLanguage_RunsOnces()
        {
            // Arrange
            var backend = new Mock<ISearchBackend>();
            var index = new Mock<IIndex>();
            var dataSource = new Mock<IDataSource>();
            var logger = new Mock<ILogger>();
            //var meta = new Mock<PXMeta>();
            //var tableLink = new TableLink("Population in the world", "Population", "AA", "POP", "01", "World population", LinkType.Table, TableStatus.AccessibleToAll, DateTime.Now, DateTime.Now, "2000", "2005", "001", PresCategory.Official);

            backend.Setup(b => b.GetIndex()).Returns(index.Object);

            var indexer = new Indexer(dataSource.Object, backend.Object, logger.Object);

            // Act
            indexer.IndexDatabase(new List<string> { "en" });

            // Assert
            index.Verify(b => b.BeginWrite(It.IsAny<string>()), Times.Exactly(1));
            index.Verify(b => b.EndWrite(It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void IndexDatabase_TwoOneLanguage_RunsTwice()
        {
            // Arrange
            var backend = new Mock<ISearchBackend>();
            var index = new Mock<IIndex>();
            var dataSource = new Mock<IDataSource>();
            var logger = new Mock<ILogger>();
            //var meta = new Mock<PXMeta>();
            //var tableLink = new TableLink("Population in the world", "Population", "AA", "POP", "01", "World population", LinkType.Table, TableStatus.AccessibleToAll, DateTime.Now, DateTime.Now, "2000", "2005", "001", PresCategory.Official);

            backend.Setup(b => b.GetIndex()).Returns(index.Object);

            var indexer = new Indexer(dataSource.Object, backend.Object, logger.Object);

            // Act
            indexer.IndexDatabase(new List<string> { "en", "sv" });

            // Assert
            index.Verify(b => b.BeginWrite(It.IsAny<string>()), Times.Exactly(2));
            index.Verify(b => b.EndWrite(It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void IndexDatabase_NoMenu_ReturnsNoException()
        {
            // Arrange
            var backend = new Mock<ISearchBackend>();
            var index = new Mock<IIndex>();
            var dataSource = new Mock<IDataSource>();
            var logger = new Mock<ILogger>();
            backend.Setup(b => b.GetIndex()).Returns(index.Object);
            dataSource.Setup(d => d.CreateMenu(It.IsAny<string>(), It.IsAny<string>(), out It.Ref<bool>.IsAny))
                .Returns((string id, string language, out bool selectionExists) =>
                {
                    selectionExists = false;
                    return (Item)null;
                });

            var indexer = new Indexer(dataSource.Object, backend.Object, logger.Object);
            // Act
            indexer.IndexDatabase(new List<string> { "en" });

            // Assert
            // No exception should be thrown

        }
    }
}
