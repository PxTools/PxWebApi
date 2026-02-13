using System;

namespace PxWeb.UnitTests.Search
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
            var tableLink = new TableLink("Population in the world", "Population", "AA", "POP", "01", "World population", LinkType.Table, TableStatus.AccessibleToAll, DateTime.Now, DateTime.Now, "2000", "2005", "001", PresCategory.Official);

            dataSource.Setup(d => d.LoadDatabaseStructure(It.IsAny<string>())).Returns(tableLink);


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
            var tableLink = new TableLink("Population in the world", "Population", "AA", "POP", "01", "World population", LinkType.Table, TableStatus.AccessibleToAll, DateTime.Now, DateTime.Now, "2000", "2005", "001", PresCategory.Official);

            dataSource.Setup(d => d.LoadDatabaseStructure(It.IsAny<string>())).Returns(tableLink);

            backend.Setup(b => b.GetIndex()).Returns(index.Object);

            var indexer = new Indexer(dataSource.Object, backend.Object, logger.Object);

            // Act
            indexer.IndexDatabase(new List<string> { "en", "sv" });

            // Assert
            index.Verify(b => b.BeginWrite(It.IsAny<string>()), Times.Exactly(2));
            index.Verify(b => b.EndWrite(It.IsAny<string>()), Times.Exactly(2));
        }


        [TestMethod]
        public void UpdateDatabase_OneLanguage_RunsOnce()
        { // Arrange
            var backend = new Mock<ISearchBackend>();
            var index = new Mock<IIndex>();
            var dataSource = new Mock<IDataSource>();
            var logger = new Mock<ILogger>();
            var tableLink = new TableLink("Population in the world", "Population", "AA", "POP", "01", "World population", LinkType.Table, TableStatus.AccessibleToAll, DateTime.Now, DateTime.Now, "2000", "2005", "001", PresCategory.Official);

            dataSource.Setup(d => d.LoadDatabaseStructure(It.IsAny<string>())).Returns(tableLink);


            backend.Setup(b => b.GetIndex()).Returns(index.Object);

            var indexer = new Indexer(dataSource.Object, backend.Object, logger.Object);

            // Act
            indexer.IndexDatabase(new List<string> { "en" });
            indexer.UpdateTableEntries(new List<string> { "001" }, new List<string> { "en" }, true);

            // Assert
            index.Verify(b => b.BeginUpdate(It.IsAny<string>()), Times.Exactly(1));
            index.Verify(b => b.EndUpdate(It.IsAny<string>()), Times.Exactly(1));

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
                    return null as Item;
                });

            var indexer = new Indexer(dataSource.Object, backend.Object, logger.Object);
            // Act
            indexer.IndexDatabase(new List<string> { "en" });

            // Assert
            index.Verify(b => b.BeginWrite(It.IsAny<string>()), Times.Exactly(1));

        }

        [TestMethod]
        public void IndexDatabase_WithMenu_ShouldTraverseTheDatabase()
        {
            // Arrange
            var backend = new Mock<ISearchBackend>();
            var index = new Mock<IIndex>();
            var dataSource = new Mock<IDataSource>();
            var logger = new Mock<ILogger>();
            var tableLink = new TableLink("Population in the world", "Population", "AA", "POP", "01", "World population", LinkType.Table, TableStatus.AccessibleToAll, DateTime.Now, DateTime.Now, "2000", "2005", "001", PresCategory.Official);
            var rootItem = new PxMenuItem(null, "", "", "A", "", "root", "");
            var builder = new Mock<IPXModelBuilder>();
            var model = ModelStore.CreateModelA();
            rootItem.SubItems.Add(tableLink);
            backend.Setup(b => b.GetIndex()).Returns(index.Object);
            builder.Setup(b => b.Model).Returns(model);

            dataSource.Setup(d => d.LoadDatabaseStructure(It.IsAny<string>())).Returns(rootItem);

            dataSource.Setup(d => d.CreateMenu("", It.IsAny<string>(), out It.Ref<bool>.IsAny))
                .Returns((string id, string language, out bool selectionExists) =>
                {
                    selectionExists = true;
                    return (Item)rootItem;
                });
            dataSource.Setup(d => d.CreateMenu("root", It.IsAny<string>(), out It.Ref<bool>.IsAny))
                .Returns((string id, string language, out bool selectionExists) =>
                {
                    selectionExists = true;
                    return (Item)rootItem;
                });
            dataSource.Setup(d => d.CreateBuilder(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string id, string language) =>
                {
                    return builder.Object;
                });

            dataSource.Setup(d => d.GetTableLanguages()).Returns(new Dictionary<string, List<string>>());

            var indexer = new Indexer(dataSource.Object, backend.Object, logger.Object);
            // Act
            indexer.IndexDatabase(new List<string> { "en" });

            // Assert
            index.Verify(b => b.AddEntry(It.IsAny<TableInformation>(), It.IsAny<PXMeta>()), Times.Exactly(1));
        }
    }
}
