

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
            if (folderAssembly.EndsWith("/") == false) folderAssembly = folderAssembly + "/";
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
            var dict = testFactory.GetMenuLookup();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            pcAxisFactory.Setup(x => x.GetMenuLookup(language)).Returns(dict);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory.Object, configMock.Object);

            bool selectionExists;

            var result = resolver.Resolve(language, "", out selectionExists);

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

            var resolver = new ItemSelectionResolverCnmm(memorymock.Object, pcAxisFactory, configMock.Object);
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

            var result = resolver.Resolve(language, "EN", out selectionExists);

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

            var resolver = new ItemSelectionResolverCnmm(memorymock.Object, pcAxisFactory, configMock.Object);
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
    }
}
