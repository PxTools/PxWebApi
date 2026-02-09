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



            _groupRegistryMock.Setup(gr => gr.GetDefaultGroupings(It.Is<string>(d => d == "Marital status")))
                .Returns(new List<GroupingInfo> { GroupingFixtures.MartialStatusInfo });
            _groupRegistryMock.Setup(gr => gr.GetDefaultGroupings(It.Is<string>(d => d == "Sex")))
                .Returns(new List<GroupingInfo> { GroupingFixtures.SexInfo });
            _groupRegistryMock.Setup(gr => gr.GetDefaultGroupings(It.Is<string>(d => d != "Marital status" && d != "Sex")))
                .Returns(new List<GroupingInfo>());

            _groupRegistryMock.Setup(gr => gr.GetGrouping(It.Is<GroupingInfo>(g => g.Name == "Marital status"))).Returns(GroupingFixtures.GroupMaritalStatus);
            _groupRegistryMock.Setup(gr => gr.GetGrouping(It.Is<GroupingInfo>(g => g.Name == "Sex"))).Returns(GroupingFixtures.GroupSex);

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

            Assert.HasCount(3, builder.Model.Meta.Variables);
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

            Assert.HasCount(3, builder.Model.Meta.Variables);
            Assert.AreEqual(8816890, builder.Model.Data.ReadElement(0, 0));
        }


        [TestMethod]
        public void BuildForPresentation_WithOneTimeValue_ShouldCleanTimeValue()
        {
            var builder = new TestablePxBuilder(PxFileFixtures.EliminationPxFile);
            var model = builder.BuildForSelection();

            var selections = Selection.SelectAll(builder.Model.Meta);
            var selection = selections.First(v => v.VariableCode == "period");
            selection.ValueCodes.Clear();
            selection.ValueCodes.Add("2001");

            builder.BuildForPresentation(selections);

            var timeVal = builder.Model.Meta.Variables.First(v => v.Code == "period").TimeValue;

            Assert.AreEqual(@"TLIST(A1),""2001""", timeVal);
        }

        [TestMethod]
        public void BuildForPresentation_WithGrouping_ReturnGroupedValues()
        {
            var builder = new TestablePxBuilder(PxFileFixtures.EliminationPxFile);
            var model = builder.BuildForSelection();


            var variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(1, variable.Groupings, "Initial groupings count mismatch for 'martial status'");

            builder.ApplyGrouping(variable.Code, variable.Groupings[0], GroupingIncludesType.AggregatedValues);

            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(3, variable.Values, "Grouped values count mismatch");

            var selections = Selection.SelectAll(builder.Model.Meta);
            builder.BuildForPresentation(selections);

            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(3, variable.Values, "Grouped values count mismatch");
            Assert.AreEqual("S", variable.Values[0].Code, "First grouped value code mismatch");
            Assert.AreEqual("L", variable.Values[1].Code, "Second grouped value code mismatch");
            Assert.AreEqual("T", variable.Values[2].Code, "Third grouped value code mismatch");
            Assert.AreEqual(2016801, builder.Model.Data.ReadElement(3, 0), "Large, men, 2001");
        }

        [TestMethod]
        public void BuildForPresentation_WithGroupingAndElimination_ReturnGroupedValues()
        {
            var builder = new TestablePxBuilder(PxFileFixtures.EliminationPxFile);
            var model = builder.BuildForSelection();

            var variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(1, variable.Groupings, "Initial groupings count mismatch for 'martial status'");

            builder.ApplyGrouping(variable.Code, variable.Groupings[0], GroupingIncludesType.AggregatedValues);

            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(3, variable.Values, "Grouped values count mismatch after applied grouping");

            var selections = Selection.SelectAll(builder.Model.Meta);
            selections.First(v => v.VariableCode == "sex").ValueCodes.Clear();

            builder.BuildForPresentation(selections);


            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(3, builder.Model.Meta.Variables, "Eliminated variable sex, variable count missmatch");
            Assert.HasCount(3, variable.Values, "Grouped values count mismatch");
            Assert.AreEqual("S", variable.Values[0].Code, "First grouped value code mismatch");
            Assert.AreEqual("L", variable.Values[1].Code, "Second grouped value code mismatch");
            Assert.AreEqual("T", variable.Values[2].Code, "Third grouped value code mismatch");
            Assert.AreEqual(4456408, builder.Model.Data.ReadElement(0, 0), "Small, Total(eliminated), 2001");
            Assert.AreEqual(4452720, builder.Model.Data.ReadElement(1, 0), "Large, Total(eliminated), 2001");
            Assert.AreEqual(8909128, builder.Model.Data.ReadElement(2, 0), "Total, Total(eliminated), 2001");

            Assert.AreEqual(4445279, builder.Model.Data.ReadElement(1, 1), "Large, Grouped(eliminated), 2002");
            Assert.AreEqual(4441440, builder.Model.Data.ReadElement(1, 2), "Large, Grouped(eliminated), 2003");
            Assert.AreEqual(4443053, builder.Model.Data.ReadElement(1, 3), "Large, Grouped(eliminated), 2004");
            Assert.AreEqual(4447637, builder.Model.Data.ReadElement(1, 4), "Large, Grouped(eliminated), 2005");
            Assert.AreEqual(4466261, builder.Model.Data.ReadElement(1, 5), "Large, Grouped(eliminated), 2006");
        }

        [TestMethod]
        public void BuildForPresentation_WithMultipleGroupings_ReturnGroupedValues()
        {
            var builder = new TestablePxBuilder(PxFileFixtures.EliminationPxFile);
            var model = builder.BuildForSelection();

            var variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(1, variable.Groupings, "Initial groupings count mismatch for 'martial status'");
            builder.ApplyGrouping(variable.Code, variable.Groupings[0], GroupingIncludesType.AggregatedValues);
            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(3, variable.Values, "Grouped values for 'marital status' count mismatch after applied grouping");

            variable = builder.Model.Meta.Variables.First(v => v.Code == "sex");
            Assert.HasCount(1, variable.Groupings, "Initial groupings count mismatch for 'sex'");
            builder.ApplyGrouping(variable.Code, variable.Groupings[0], GroupingIncludesType.AggregatedValues);
            variable = builder.Model.Meta.Variables.First(v => v.Code == "sex");
            Assert.HasCount(1, variable.Values, "Grouped values for 'sex' count mismatch after applied grouping");

            var selections = Selection.SelectAll(builder.Model.Meta);
            builder.BuildForPresentation(selections);

            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(3, variable.Values, "Grouped values count mismatch for 'marital status'");
            Assert.AreEqual("S", variable.Values[0].Code, "First grouped value code mismatch for 'marital status'");
            Assert.AreEqual("L", variable.Values[1].Code, "Second grouped value code mismatch for 'marital status'");
            Assert.AreEqual("T", variable.Values[2].Code, "Third grouped value code mismatch for 'marital status'");

            variable = builder.Model.Meta.Variables.First(v => v.Code == "sex");
            Assert.HasCount(1, variable.Values, "Grouped values count mismatch for 'sex'");
            Assert.AreEqual("T2", variable.Values[0].Code, "First grouped value code mismatch for 'sex'");

            Assert.AreEqual(4456408, builder.Model.Data.ReadElement(0, 0), "Small, Grouped Total, 2001");
            Assert.AreEqual(4452720, builder.Model.Data.ReadElement(1, 0), "Large, Grouped Total, 2001");
            Assert.AreEqual(8909128, builder.Model.Data.ReadElement(2, 0), "Total, Grouped Total, 2001");
            Assert.AreEqual(4445279, builder.Model.Data.ReadElement(1, 1), "Large, Grouped Total, 2002");
            Assert.AreEqual(4441440, builder.Model.Data.ReadElement(1, 2), "Large, Grouped Total, 2003");
            Assert.AreEqual(4443053, builder.Model.Data.ReadElement(1, 3), "Large, Grouped Total, 2004");
            Assert.AreEqual(4447637, builder.Model.Data.ReadElement(1, 4), "Large, Grouped Total, 2005");
            Assert.AreEqual(4466261, builder.Model.Data.ReadElement(1, 5), "Large, Grouped Total, 2006");
        }

        [TestMethod]
        public void BuildForPresentation_WithGroupingWithGroupingTypeAll_ReturnGroupedValuesAndOriginalValues()
        {
            var builder = new TestablePxBuilder(PxFileFixtures.EliminationPxFile);
            var model = builder.BuildForSelection();


            var variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(1, variable.Groupings, "Initial groupings count mismatch for 'martial status'");

            builder.ApplyGrouping(variable.Code, variable.Groupings[0], GroupingIncludesType.SingleValues);

            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(5, variable.Values, "Grouped values count mismatch");

            var selections = Selection.SelectAll(builder.Model.Meta);
            builder.BuildForPresentation(selections);

            variable = builder.Model.Meta.Variables.First(v => v.Code == "marital status");
            Assert.HasCount(5, variable.Values, "Grouped values count mismatch");
        }

    }
}
