using PxWeb.Helper.Api2;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class SelectionHandlerTests
    {
        [TestMethod]
        public void ExpandAndVerfiySelections_InvalidSelection_ReturnsProblem()
        {
            // Arrange
            var variablesSelection = new VariablesSelection();
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            // Act
            var result = handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void ExpandAndVerfiySelections_ReplaceTime_ReturnsTrue()
        {
            // Arrange
            var variablesSelection = CreateValidSelection();
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            // Act
            var result = handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("PointOfTime", variablesSelection.Selection[1].VariableCode);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void ExpandAndVerfiySelections_FixVaribleRefrence_ReturnsTrue()
        {
            // Arrange
            var variablesSelection = CreateValidSelection();
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            // Act
            var result = handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("MEASURE", variablesSelection.Selection[0].VariableCode);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void ExpandAndVerfiySelections_ValueRefrence_ReturnsTrue()
        {
            // Arrange
            var variablesSelection = CreateValidSelection();
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            // Act
            var result = handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("M1", variablesSelection.Selection[0].ValueCodes[0]);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void ExpandAndVerfiySelections_MissingMandatorySelection_ReturnsProblem()
        {
            // Arrange
            var variablesSelection = CreateMissingMandatorySelection();
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            // Act
            var result = handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(problem);
            Assert.AreEqual(ProblemUtility.MissingSelection().Title, problem.Title);
        }

        [TestMethod]
        public void ExpandAndVerfiySelections_InvalidValue_ReturnsProblem()
        {
            // Arrange
            var variablesSelection = CreateInvalidSelection();
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            // Act
            var result = handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(problem);
            Assert.AreEqual(ProblemUtility.NonExistentValue().Title, problem.Title);
        }

        [TestMethod]
        public void Convert_InvalidSelection_ReturnsValidSelection()
        {
            // Arrange
            var variablesSelection = CreateValidSelection();
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);

            // Act
            var result = handler.Convert(variablesSelection);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
        }

        [TestMethod]
        public void FixVariableRefsAndApplyCodelists_WhenNoExistingVaiableSpecified_ReturnFalseAndProblem()
        {
            // Arrange
            var codelist = new Dictionary<string, string>();
            codelist.Add("THIS_IS_A_INVALID_CODE", "m1");
            var variablesSelection = SelectionUtil.CreateVariablesSelectionFromCodelists(codelist);
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Act
            var result = handler.FixVariableRefsAndApplyCodelists(builderMock.Object, variablesSelection, out problem);
            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void FixVariableRefsAndApplyCodelists_WhenNoExistingCodeListSpecified_ReturnFalseAndProblem()
        {
            // Arrange
            var codelist = new Dictionary<string, string>();
            codelist.Add("measure", "THIS_VALUE_DOES_NOT_EXIST");
            var variablesSelection = SelectionUtil.CreateVariablesSelectionFromCodelists(codelist);
            var configMock = GetConfigMock();
            var handler = new SelectionHandler(configMock.Object);
            var model = ModelStore.CreateModelA();
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            handler.ExpandAndVerfiySelections(variablesSelection, builderMock.Object, out var problem);
            // Act
            var result = handler.FixVariableRefsAndApplyCodelists(builderMock.Object, variablesSelection, out problem);
            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(problem);
        }


        private static VariablesSelection CreateValidSelection()
        {
            var selection = new VariablesSelection();
            selection.Selection = new List<VariableSelection>();
            selection.Selection.Add(new VariableSelection() { VariableCode = "measure", ValueCodes = new List<string>() { "m1" } });
            selection.Selection.Add(new VariableSelection() { VariableCode = "TIME", ValueCodes = new List<string>() { "2019" } });

            return selection;
        }

        private static VariablesSelection CreateMissingMandatorySelection()
        {
            var selection = new VariablesSelection();
            selection.Selection = new List<VariableSelection>();
            selection.Selection.Add(new VariableSelection() { VariableCode = "measure", ValueCodes = new List<string>() });
            selection.Selection.Add(new VariableSelection() { VariableCode = "TIME", ValueCodes = new List<string>() { "2019" } });

            return selection;
        }

        private static VariablesSelection CreateInvalidSelection()
        {
            var selection = new VariablesSelection();
            selection.Selection = new List<VariableSelection>();
            selection.Selection.Add(new VariableSelection() { VariableCode = "measure", ValueCodes = new List<string>() { "THIS VALUE DOES NOT EXISIT" } });
            selection.Selection.Add(new VariableSelection() { VariableCode = "TIME", ValueCodes = new List<string>() { "2019" } });

            return selection;
        }

        private static Mock<IPxApiConfigurationService> GetConfigMock()
        {
            var configMock = new Mock<IPxApiConfigurationService>();
            var testFactory = new TestFactory();
            var config = testFactory.GetPxApiConfiguration();
            configMock.Setup(x => x.GetConfiguration()).Returns(config);

            return configMock;
        }
    }
}
