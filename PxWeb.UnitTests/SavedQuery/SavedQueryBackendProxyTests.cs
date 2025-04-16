using PxWeb.Code.Api2.SavedQueryBackend;

namespace PxWeb.UnitTests.SavedQuery
{

    [TestClass]
    public class SavedQueryBackendProxyTests
    {
        [TestMethod]
        public void LoadDefaultSelection_ReturnsASavedQuery_WhenIdExists()
        {
            // Arrange
            var backend = new Mock<ISavedQueryStorageBackend>();
            backend.Setup(x => x.LoadDefaultSelection(It.IsAny<string>()))
                .Returns(@"{""Id"":null,""Selection"":{""Selection"":[{""VariableCode"":""ContentsCode"",""CodeList"":null,""ValueCodes"":[""HNMGA""]},{""VariableCode"":""TIME"",""CodeList"":null,""ValueCodes"":[""2001""]},{""VariableCode"":""REGION"",""CodeList"":null,""ValueCodes"":[""*""]},{""VariableCode"":""SEX"",""CodeList"":null,""ValueCodes"":[""F"",""M""]},{""VariableCode"":""age"",""CodeList"":null,""ValueCodes"":[""Total""]}],""Placement"":{""Heading"":[""ContentsCode""],""Stub"":[]}},""Language"":""en"",""TableId"":""TAB001"",""OutputFormat"":1,""OutputFormatParams"":[]}");

            var savedQueryBackendProxy = new SavedQueryBackendProxy(backend.Object);
            // Act
            var result = savedQueryBackendProxy.LoadDefaultSelection("testId");
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoadDefaultSelection_ReturnsNull_WhenIdExists()
        {
            // Arrange
            var backend = new Mock<ISavedQueryStorageBackend>();
            backend.Setup(x => x.LoadDefaultSelection(It.IsAny<string>()))
                .Returns("");

            var savedQueryBackendProxy = new SavedQueryBackendProxy(backend.Object);
            // Act
            var result = savedQueryBackendProxy.LoadDefaultSelection("testId");
            // Assert
            Assert.IsNull(result);
        }
    }
}
