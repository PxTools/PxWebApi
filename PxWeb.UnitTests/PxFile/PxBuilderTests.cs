using System.Text;

using PxWeb.Code.PxFile;
using PxWeb.PxFile;
using PxWeb.UnitTests.Fixtures;

namespace PxWeb.UnitTests.PxFile
{

    [TestClass]
    public class PxBuilderTests
    {
        private sealed class TestablePxBuilder : PxBuilder
        {
            private readonly string _fixture;

            public TestablePxBuilder(string fixture)
            {
                _fixture = fixture;
                m_parser = new PxUtilsProxyParserTests.TestablePxUtilsProxyParser(_fixture);
            }

            protected override Stream GetStream()
            {
                var bytes = Encoding.UTF8.GetBytes(_fixture);
                return new MemoryStream(bytes, writable: false);
            }

            protected override GroupRegistryWrapper GetGroupRegistryProvider()
            {
                return _groupRegistryMock.Object;

            }
        }

        private static Mock<GroupRegistryWrapper> _groupRegistryMock = null!;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _groupRegistryMock = new Mock<GroupRegistryWrapper>();
            _groupRegistryMock.Setup(gr => gr.IsLoaded).Returns(true);

            _groupRegistryMock.Setup(gr => gr.GetDefaultGroupings(It.IsAny<string>()))
                .Returns(new List<GroupingInfo>());

        }

        [TestMethod]
        public void BuildForPresentation_WithEleminationByValue_ReturnsPxModelWithOneLessVariable()
        {
            var builder = new TestablePxBuilder(PxFileFixtures.EliminationPxFile);
            var model = builder.BuildForSelection();

            Assert.IsTrue(builder.Model.Meta.Variables.First(v => v.Code == "sex").Elimination);

            var selection = Selection.SelectAll(builder.Model.Meta);
            selection.First(v => v.VariableCode == "sex").ValueCodes.Clear();

            builder.BuildForPresentation(selection);

            Assert.HasCount(2, builder.Model.Meta.Variables);
            Assert.AreEqual(4456408, builder.Model.Data.ReadElement(0, 0));
        }

        [TestMethod]
        public void BuildForPresentation_WithEleminationBySum_ReturnsPxModelWithOneLessVariable()
        {
            var builder = new TestablePxBuilder(PxFileFixtures.EliminationPxFile);
            var model = builder.BuildForSelection();

            Assert.IsTrue(builder.Model.Meta.Variables.First(v => v.Code == "marital status").Elimination);

            var selection = Selection.SelectAll(builder.Model.Meta);
            selection.First(v => v.VariableCode == "marital status").ValueCodes.Clear();

            builder.BuildForPresentation(selection);

            Assert.HasCount(2, builder.Model.Meta.Variables);
            Assert.AreEqual(8816890, builder.Model.Data.ReadElement(0, 0));
        }
    }
}
