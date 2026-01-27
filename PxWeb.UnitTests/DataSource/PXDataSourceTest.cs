

using System;

using Microsoft.Extensions.Options;

namespace PxWeb.UnitTests.DataSource
{
    [TestClass]
    public class PXDataSourceTest
    {

        internal static string GetFullPathToFile(string pathRelativeUnitTestingFile)
        {
            string folderProjectLevel = GetPathToPxWebProject();
            string final = System.IO.Path.Combine(folderProjectLevel, pathRelativeUnitTestingFile);
            return final;
        }

        private static string GetPathToPxWebProject()
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string directoryName = System.IO.Path.GetDirectoryName(pathAssembly) ?? throw new System.Exception("GetDirectoryName(pathAssembly) is null for:" + pathAssembly);
            string folderAssembly = directoryName.Replace("\\", "/");
            if (!folderAssembly.EndsWith('/')) folderAssembly = folderAssembly + "/";
            string folderProjectLevel = System.IO.Path.GetFullPath(folderAssembly + "../../../../");
            return folderProjectLevel;
        }

        [TestMethod]
        public void ResolveEmtySelectionItemShouldReturnStart()
        {
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();

            var testFactory = new TestFactory();
            var dict = testFactory.GetMenuLookupFolders();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            pcAxisFactory.Setup(x => x.GetMenuLookupFolders(language)).Returns(dict);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory.Object, configMock.Object);

            bool selectionExists;

            var result = resolver.ResolveFolder(language, "", out selectionExists);

            Assert.AreEqual("START", result.Menu);
            Assert.AreEqual("START", result.Selection);
        }


        [TestMethod]
        public void ShouldReturnMenu()
        {

            //arrange
            var testFactory = new TestFactory();
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<IPxFileConfigurationService>();
            var hostingEnvironmentMock = new Mock<IPxHost>();
            var loggerMock = new Mock<ILogger<TablePathResolverPxFile>>();
            var codelistMapperMock = new Mock<ICodelistMapper>();

            var itemLoggerMock = new Mock<ILogger<ItemSelectorResolverPxFactory>>();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            var pcAxisFactory = new ItemSelectorResolverPxFactory(configServiceMock.Object, hostingEnvironmentMock.Object, itemLoggerMock.Object);

            var wwwrootPath = GetFullPathToFile(@"PxWeb/wwwroot/");

            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory, configMock.Object);
            var tablePathResolver = new TablePathResolverPxFile(memorymock.Object, hostingEnvironmentMock.Object, configMock.Object, loggerMock.Object);
            var datasource = new PxFileDataSource(configServiceMock.Object, resolver, tablePathResolver, hostingEnvironmentMock.Object, codelistMapperMock.Object);
            bool selectionExists;

            //act
            var result = datasource.CreateMenu("", language, out selectionExists);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ResolveShouldResolveItemCollection()
        {
            var testFactory = new TestFactory();
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<IPxFileConfigurationService>();
            var hostingEnvironmentMock = new Mock<IPxHost>();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            var itemLoggerMock = new Mock<ILogger<ItemSelectorResolverPxFactory>>();
            var pcAxisFactory = new ItemSelectorResolverPxFactory(configServiceMock.Object, hostingEnvironmentMock.Object, itemLoggerMock.Object);

            var wwwrootPath = GetFullPathToFile(@"PxWeb/wwwroot/");

            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory, configMock.Object);

            bool selectionExists;

            var result = resolver.ResolveFolder(language, "EN", out selectionExists);

            Assert.IsNotNull(result);
            Assert.AreEqual("Database", result.Menu);
            Assert.IsTrue(selectionExists);
        }

        [TestMethod]
        public void ShouldResolveTablePath()
        {
            string language = "en";
            var resolver = GetTablePathResolver();

            bool selectionExists;

            var result = resolver.Resolve(language, "TAB003", out selectionExists);

            Assert.IsNotNull(result);
            Assert.IsTrue(selectionExists);
        }

        [TestMethod]
        public void TableExistPxFileShouldReturnTrue()
        {

            //arrange
            var testFactory = new TestFactory();
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<IPxFileConfigurationService>();
            var hostingEnvironmentMock = new Mock<IPxHost>();
            var loggerMock = new Mock<ILogger<TablePathResolverPxFile>>();
            var codelistMapperMock = new Mock<ICodelistMapper>();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            var itemLoggerMock = new Mock<ILogger<ItemSelectorResolverPxFactory>>();
            var pcAxisFactory = new ItemSelectorResolverPxFactory(configServiceMock.Object, hostingEnvironmentMock.Object, itemLoggerMock.Object);

            var wwwrootPath = GetFullPathToFile(@"PxWeb/wwwroot/");

            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory, configMock.Object);
            var tablePathResolver = new TablePathResolverPxFile(memorymock.Object, hostingEnvironmentMock.Object, configMock.Object, loggerMock.Object);
            var datasource = new PxFileDataSource(configServiceMock.Object, resolver, tablePathResolver, hostingEnvironmentMock.Object, codelistMapperMock.Object);

            //act
            var result = datasource.TableExists("tAB003", language);

            //assert
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void TableExistPXFileShouldNotReturnTrue()
        {

            //arrange
            var testFactory = new TestFactory();
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<IPxFileConfigurationService>();
            var hostingEnvironmentMock = new Mock<IPxHost>();
            var loggerMock = new Mock<ILogger<TablePathResolverPxFile>>();
            var codelistMapperMock = new Mock<ICodelistMapper>();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            var itemLoggerMock = new Mock<ILogger<ItemSelectorResolverPxFactory>>();
            var pcAxisFactory = new ItemSelectorResolverPxFactory(configServiceMock.Object, hostingEnvironmentMock.Object, itemLoggerMock.Object);

            var wwwrootPath = GetFullPathToFile(@"PxWeb/wwwroot/");

            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory, configMock.Object);
            var tablePathResolver = new TablePathResolverPxFile(memorymock.Object, hostingEnvironmentMock.Object, configMock.Object, loggerMock.Object);
            var datasource = new PxFileDataSource(configServiceMock.Object, resolver, tablePathResolver, hostingEnvironmentMock.Object, codelistMapperMock.Object);

            //act
            var result = datasource.TableExists("select * from BE0101F1", language);

            //assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void ShouldNotResolveTablePath()
        {
            string language = "en";
            var resolver = GetTablePathResolver();

            bool selectionExists;

            var result = resolver.Resolve(language, "officialstatistics2.px", out selectionExists);

            Assert.AreEqual("", result);
            Assert.IsFalse(selectionExists);
        }

        [TestMethod]
        public void GetTablesPublishedBetween_WhenCalled_ReturnNonNull()
        {
            string pathRunning = Directory.GetCurrentDirectory();
            int index = pathRunning.IndexOf("PxWeb.UnitTests");

            if (index == -1)
            {
                throw new System.Exception("Hmm, Directory.GetCurrentDirectory() does not contain string:PxWeb.UnitTests , so unable to find wwwroot path.");
            }
            string repoRoot = pathRunning.Substring(0, index);
            string wwwPath = Path.Combine(repoRoot, "PxWeb", "wwwroot");

            var hostingEnvironmentMock = new Mock<IPxHost>();
            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwPath);

            var dataSource = new PxFileDataSource(
                new Mock<IPxFileConfigurationService>().Object,
                new Mock<IItemSelectionResolver>().Object,
                new Mock<ITablePathResolver>().Object,
                hostingEnvironmentMock.Object,
                new Mock<ICodelistMapper>().Object);

            var updataedTables = dataSource.GetTablesPublishedBetween(new DateTime(2023, 8, 1), new DateTime(2023, 9, 1));

            Assert.IsNotNull(updataedTables);

        }

        private TablePathResolverPxFile GetTablePathResolver()
        {
            var testFactory = new TestFactory();
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<IPxFileConfigurationService>();
            var hostingEnvironmentMock = new Mock<IPxHost>();
            var loggerMock = new Mock<ILogger<TablePathResolverPxFile>>();


            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            var itemLoggerMock = new Mock<ILogger<ItemSelectorResolverPxFactory>>();
            var pcAxisFactory = new ItemSelectorResolverPxFactory(configServiceMock.Object, hostingEnvironmentMock.Object, itemLoggerMock.Object);

            /* This actually works, and fails the test since/when there was no logging made.
            itemLoggerMock.Verify( x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                    Times.AtLeastOnce);
            */

            var wwwrootPath = GetFullPathToFile(@"PxWeb/wwwroot/");

            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

            var resolver = new TablePathResolverPxFile(memorymock.Object, hostingEnvironmentMock.Object, configMock.Object, loggerMock.Object);

            return resolver;
        }

        [TestMethod]
        public void ShouldReturnMappingOfTableLanguages()
        {

            //arrange
            var testFactory = new TestFactory();
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<IPxFileConfigurationService>();
            var hostingEnvironmentMock = new Mock<IPxHost>();
            var loggerMock = new Mock<ILogger<TablePathResolverPxFile>>();
            var codelistMapperMock = new Mock<ICodelistMapper>();

            var itemLoggerMock = new Mock<ILogger<ItemSelectorResolverPxFactory>>();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            var pcAxisFactory = new ItemSelectorResolverPxFactory(configServiceMock.Object, hostingEnvironmentMock.Object, itemLoggerMock.Object);

            var wwwrootPath = GetFullPathToFile(@"PxWeb/wwwroot/");

            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory, configMock.Object);
            var tablePathResolver = new TablePathResolverPxFile(memorymock.Object, hostingEnvironmentMock.Object, configMock.Object, loggerMock.Object);
            var datasource = new PxFileDataSource(configServiceMock.Object, resolver, tablePathResolver, hostingEnvironmentMock.Object, codelistMapperMock.Object);


            //act
            var result = datasource.GetTableLanguages();

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoadDatabaseStructure_ShouldNotHaveDatabaseInId()
        {
            //arrange
            var testFactory = new TestFactory();
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<IPxFileConfigurationService>();
            var hostingEnvironmentMock = new Mock<IPxHost>();
            var loggerMock = new Mock<ILogger<TablePathResolverPxFile>>();
            var codelistMapperMock = new Mock<ICodelistMapper>();
            var cnmmConfigMock = new Mock<IOptions<CnmmConfigurationOptions>>();

            var itemLoggerMock = new Mock<ILogger<ItemSelectorResolverPxFactory>>();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            var pcAxisFactory = new ItemSelectorResolverPxFactory(configServiceMock.Object, hostingEnvironmentMock.Object, itemLoggerMock.Object);

            var wwwrootPath = GetFullPathToFile(@"PxWeb/wwwroot/");

            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory, configMock.Object);
            var tablePathResolver = new TablePathResolverPxFile(memorymock.Object, hostingEnvironmentMock.Object, configMock.Object, loggerMock.Object);
            var datasource = new PxFileDataSource(configServiceMock.Object, resolver, tablePathResolver, hostingEnvironmentMock.Object, codelistMapperMock.Object);

            //act
            var menu = datasource.LoadDatabaseStructure("en");

            //assert
            Assert.IsNotNull(menu);
            Assert.IsFalse(((PxMenuItem)menu).SubItems.Any(t => t.ID.Selection.Contains("Database/")));
        }

    }
}
