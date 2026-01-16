using Microsoft.Extensions.Options;

namespace PxWeb.UnitTests.DataSource
{

    [TestClass]
    public class ItemSelectionResolverCnmmTest
    {
        [TestMethod]
        public void WhenResolvingExistingSelection_ThenSelectionExistsIsTrue()
        {
            // Arrange
            string language = "sv";

            var folders = new Dictionary<string, ItemSelection>
            {
                { "AM", new ItemSelection { Menu = "START", Selection = "AM" } },
                { "BE", new ItemSelection { Menu = "START", Selection = "BE" } },
                { "AM0101", new ItemSelection { Menu = "AM", Selection = "AM0101" } },
                { "BE", new ItemSelection { Menu = "START", Selection = "BE0101" } }
            };

            var cacheMock = new Mock<IPxCache>();
            cacheMock.Setup(x => x.Get<Dictionary<string, ItemSelection>?>(It.IsAny<object>())).Returns((Dictionary<string, ItemSelection>?)null);



            var configMock = new Mock<IPxApiConfigurationService>();
            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();
            var cnmmConfigMock = new Mock<IOptions<CnmmConfigurationOptions>>();
            cnmmConfigMock.Setup(x => x.Value).Returns(new CnmmConfigurationOptions());
            var testFactory = new TestFactory();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);
            pcAxisFactory.Setup(x => x.GetMenuLookupFolders(language)).Returns(folders);
            var resolver = new ItemSelectionResolverCnmm(cacheMock.Object, pcAxisFactory.Object, configMock.Object, cnmmConfigMock.Object);
            bool selectionExists;
            // Act
            var result = resolver.ResolveFolder(language, "AM", out selectionExists);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("START", result.Menu);
            Assert.IsTrue(selectionExists);
        }

        [TestMethod]
        public void WhenResolvingDoesNotExistSelection_ThenSelectionExistsIsFalse()
        {
            // Arrange
            string language = "sv";

            var folders = new Dictionary<string, ItemSelection>
            {
                { "AM", new ItemSelection { Menu = "START", Selection = "AM" } },
                { "BE", new ItemSelection { Menu = "START", Selection = "BE" } },
                { "AM0101", new ItemSelection { Menu = "AM", Selection = "AM0101" } },
                { "BE0101", new ItemSelection { Menu = "BE", Selection = "BE0101" } }
            };

            var cacheMock = new Mock<IPxCache>();
            cacheMock.Setup(x => x.Get<Dictionary<string, ItemSelection>?>(It.IsAny<object>())).Returns((Dictionary<string, ItemSelection>?)null);



            var configMock = new Mock<IPxApiConfigurationService>();
            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();
            var cnmmConfigMock = new Mock<IOptions<CnmmConfigurationOptions>>();
            cnmmConfigMock.Setup(x => x.Value).Returns(new CnmmConfigurationOptions());
            var testFactory = new TestFactory();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);
            pcAxisFactory.Setup(x => x.GetMenuLookupFolders(language)).Returns(folders);
            var resolver = new ItemSelectionResolverCnmm(cacheMock.Object, pcAxisFactory.Object, configMock.Object, cnmmConfigMock.Object);
            bool selectionExists;
            // Act
            var result = resolver.ResolveFolder(language, "FM", out selectionExists);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(selectionExists);
        }

        [TestMethod]
        public void WhenRootedResolvingDoesNotExistSelection_ThenSelectionExistsIsFalse()
        {
            // Arrange
            string language = "sv";

            var folders = new Dictionary<string, ItemSelection>
            {
                { "AM", new ItemSelection { Menu = "START", Selection = "AM" } },
                { "BE", new ItemSelection { Menu = "START", Selection = "BE" } },
                { "AM0101", new ItemSelection { Menu = "AM", Selection = "AM0101" } },
                { "BE0101", new ItemSelection { Menu = "BE", Selection = "BE0101" } }
            };

            var cacheMock = new Mock<IPxCache>();
            cacheMock.Setup(x => x.Get<Dictionary<string, ItemSelection>?>(It.IsAny<object>())).Returns((Dictionary<string, ItemSelection>?)null);



            var configMock = new Mock<IPxApiConfigurationService>();
            var pcAxisFactory = new Mock<IItemSelectionResolverFactory>();
            var cnmmConfigMock = new Mock<IOptions<CnmmConfigurationOptions>>();
            cnmmConfigMock.Setup(x => x.Value).Returns(new CnmmConfigurationOptions() { RootNode = "BE" });
            var testFactory = new TestFactory();

            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);
            pcAxisFactory.Setup(x => x.GetMenuLookupFolders(language)).Returns(folders);
            var resolver = new ItemSelectionResolverCnmm(cacheMock.Object, pcAxisFactory.Object, configMock.Object, cnmmConfigMock.Object);
            bool selectionExists;
            // Act
            var result = resolver.ResolveFolder(language, "AM", out selectionExists);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(selectionExists);
        }
    }
}
