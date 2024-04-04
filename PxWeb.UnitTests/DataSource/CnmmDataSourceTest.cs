namespace PxWeb.UnitTests.DataSource
{
    [TestClass]
    public class CnmmDataSourceTest
    {
        [TestMethod]
        public void ResolveShouldResolveItemCollection()
        {
            string language = "sv";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();

            var testFactory = new TestFactory();
            var dict = testFactory.GetMenuLookupFolders();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            pcAxisFactory.Setup(x => x.GetMenuLookupFolders(language)).Returns(dict);

            var resolver = new ItemSelectionResolverCnmm(memorymock.Object, pcAxisFactory.Object, configMock.Object);

            bool selectionExists;

            var result = resolver.ResolveFolder(language, "AA0003", out selectionExists);

            Assert.IsNotNull(result);
            Assert.AreEqual("AA", result.Menu);
            Assert.IsTrue(selectionExists);
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

            var resolver = new ItemSelectionResolverCnmm(memorymock.Object, pcAxisFactory.Object, configMock.Object);

            bool selectionExists;

            var result = resolver.ResolveFolder(language, "", out selectionExists);

            Assert.AreEqual("START", result.Menu);
            Assert.AreEqual("START", result.Selection);
        }


        [Ignore]
        [TestMethod]
        public void ShouldReturnMenu()
        {
            //todo, mock database 
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<ICnmmConfigurationService>();
            var codelistMapperMock = new Mock<ICodelistMapper>();

            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();

            var testFactory = new TestFactory();
            var dict = testFactory.GetMenuLookupFolders();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            pcAxisFactory.Setup(x => x.GetMenuLookupFolders(language)).Returns(dict);

            var resolver = new ItemSelectionResolverCnmm(memorymock.Object, pcAxisFactory.Object, configMock.Object);
            var tablePathResolver = new TablePathResolverCnmm(configServiceMock.Object, resolver);

            var datasource = new CnmmDataSource(configServiceMock.Object, resolver, tablePathResolver, codelistMapperMock.Object);

            bool selectionExists;

            var result = datasource.CreateMenu("AA0003", language, out selectionExists);

            Assert.IsNotNull(result);
        }

        [Ignore]
        [TestMethod]
        public void TableExistsCNMMShouldReturnTrue()
        {
            //todo, mock database 
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<ICnmmConfigurationService>();
            var codelistMapperMock = new Mock<ICodelistMapper>();

            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();

            var testFactory = new TestFactory();
            var dict = testFactory.GetMenuLookupFolders();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            pcAxisFactory.Setup(x => x.GetMenuLookupTables(language)).Returns(dict);

            var resolver = new ItemSelectionResolverCnmm(memorymock.Object, pcAxisFactory.Object, configMock.Object);
            var tablePathResolver = new TablePathResolverCnmm(configServiceMock.Object, resolver);

            var datasource = new CnmmDataSource(configServiceMock.Object, resolver, tablePathResolver, codelistMapperMock.Object);

            var result = datasource.TableExists("Befolkning", language);

            Assert.IsTrue(result);
        }

        [Ignore]
        [TestMethod]
        public void TableExistsCNMMShouldReturnFalse()
        {
            //todo, mock database 
            string language = "en";
            var memorymock = new Mock<IPxCache>();
            var configMock = new Mock<IPxApiConfigurationService>();
            var configServiceMock = new Mock<ICnmmConfigurationService>();
            var codelistMapperMock = new Mock<ICodelistMapper>();

            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();

            var testFactory = new TestFactory();
            var dict = testFactory.GetMenuLookupTables();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            pcAxisFactory.Setup(x => x.GetMenuLookupTables(language)).Returns(dict);

            var resolver = new ItemSelectionResolverCnmm(memorymock.Object, pcAxisFactory.Object, configMock.Object);
            var tablePathResolver = new TablePathResolverCnmm(configServiceMock.Object, resolver);

            var datasource = new CnmmDataSource(configServiceMock.Object, resolver, tablePathResolver, codelistMapperMock.Object);

            var result = datasource.TableExists("select * from Befolkning", language);

            Assert.IsFalse(result);
        }

    }
}
