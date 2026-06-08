namespace PxWeb.UnitTests.DataSource
{
    [TestClass]
    public class PxFileBuilder2Test
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

        private PxFileDataSource GetDataSource()
        {
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
            hostingEnvironmentMock.Setup(m => m.RootPath).Returns(wwwrootPath);

            var resolver = new ItemSelectionResolverPxFile(memorymock.Object, pcAxisFactory, configMock.Object);
            var tablePathResolver = new TablePathResolverPxFile(memorymock.Object, hostingEnvironmentMock.Object, configMock.Object, loggerMock.Object);

            return new PxFileDataSource(configServiceMock.Object, resolver, tablePathResolver, hostingEnvironmentMock.Object, codelistMapperMock.Object);
        }

        [TestMethod]
        public void BuildForSelection_SingleLanguagePxFile_DoesNotThrow()
        {
            //arrange
            var dataSource = GetDataSource();
            var builder = dataSource.CreateBuilder("TAB003", "en");
            Assert.IsNotNull(builder);

            //act & assert
            builder.BuildForSelection(); // must not throw
        }

        [TestMethod]
        public void BuildForSelection_SingleLanguagePxFile_ReturnsTrue()
        {
            //arrange
            var dataSource = GetDataSource();
            var builder = dataSource.CreateBuilder("TAB003", "en");
            Assert.IsNotNull(builder);

            //act
            var result = builder.BuildForSelection();

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BuildForSelection_SingleLanguagePxFile_CreatesContentVariable()
        {
            //arrange
            var dataSource = GetDataSource();
            var builder = dataSource.CreateBuilder("TAB003", "en");
            Assert.IsNotNull(builder);

            //act
            builder.BuildForSelection();

            //assert
            Assert.IsNotNull(builder.Model.Meta.ContentVariable);
        }

        [TestMethod]
        public void BuildForSelection_SingleLanguagePxFile_ContentVariableIsMarkedAsContentVariable()
        {
            //arrange
            var dataSource = GetDataSource();
            var builder = dataSource.CreateBuilder("TAB003", "en");
            Assert.IsNotNull(builder);

            //act
            builder.BuildForSelection();

            //assert
            Assert.IsTrue(builder.Model.Meta.ContentVariable.IsContentVariable);
        }

        [TestMethod]
        public void BuildForSelection_SingleLanguagePxFile_ContentInfoNotNulledOnMeta()
        {
            //arrange
            var dataSource = GetDataSource();
            var builder = dataSource.CreateBuilder("TAB003", "en");
            Assert.IsNotNull(builder);

            //act
            builder.BuildForSelection();

            //assert
            Assert.IsNotNull(builder.Model.Meta.ContentInfo,
                "meta.ContentInfo must not be null after BuildForSelection — serializers such as Xlsx2Serializer depend on it.");
        }

        [TestMethod]
        public void BuildForSelection_SingleLanguagePxFile_ContentInfoCopiedToContentVariableValue()
        {
            //arrange
            var dataSource = GetDataSource();
            var builder = dataSource.CreateBuilder("TAB003", "en");
            Assert.IsNotNull(builder);

            //act
            builder.BuildForSelection();

            //assert
            var value = builder.Model.Meta.ContentVariable.Values[0];
            Assert.IsNotNull(value.ContentInfo,
                "ContentInfo should be copied from meta to the content variable value.");
        }
    }
}
