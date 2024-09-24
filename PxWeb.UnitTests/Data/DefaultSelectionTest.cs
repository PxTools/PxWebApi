namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class DefaultSelectionTest
    {

        [TestMethod]
        public void ShouldReturnSingleSelectionFromFallbackWith1500ValuesSelected()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithOnlyOneVariable(2000));
            Problem? problem;

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 1);
            Assert.AreEqual(selection[0].ValueCodes.Count, 1500);
        }

        [TestMethod]
        public void ShouldReturnSingleSelectionFromFallbackWithOriginalNumberOfValuesSelected()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithOnlyOneVariable(50));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 1);
            Assert.AreEqual(selection[0].ValueCodes.Count, 50);
        }

        [TestMethod]
        public void ShouldHave2SelectionsByFallback()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith1MandantoryAnd1NoneMandantoryVariables(50, 150));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 2);
        }

        [TestMethod]
        public void MandantoryVariableShouldBeSelectedByFallback()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith1MandantoryAnd3NoneMandantoryVariables(50, 150));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 4);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var o1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var o2 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

            Assert.IsNotNull(m1);
            Assert.AreEqual(m1.ValueCodes.Count, 30);
            Assert.IsNotNull(o1);
            Assert.AreEqual(o1.ValueCodes.Count, 0);
            Assert.IsNotNull(o2);
            Assert.AreEqual(o2.ValueCodes.Count, 0);
            Assert.IsNotNull(o3);
            Assert.AreEqual(o3.ValueCodes.Count, 150);
        }

        [TestMethod]
        public void OnlyMandantoryVariableShouldBeSelectedByFallback()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith2MandantoryAnd2NoneMandantoryVariables(10, 10));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 4);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var m2 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var o2 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

            Assert.IsNotNull(m1);
            Assert.AreEqual(m1.ValueCodes.Count, 10);
            Assert.IsNotNull(m2);
            Assert.AreEqual(m2.ValueCodes.Count, 10);
            Assert.IsNotNull(o2);
            Assert.AreEqual(o2.ValueCodes.Count, 0);
            Assert.IsNotNull(o3);
            Assert.AreEqual(o3.ValueCodes.Count, 0);
        }

        [TestMethod]
        public void OnlyMandantoryVariableShouldBeSelectedThirdVariableShouldOnlyHaveOneValueSelectedByFallback()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith3MandantoryAnd1NoneMandantoryVariables(10, 10));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 4);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var m2 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var m3 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

            Assert.IsNotNull(m1);
            Assert.AreEqual(m1.ValueCodes.Count, 10);
            Assert.IsNotNull(m2);
            Assert.AreEqual(m2.ValueCodes.Count, 1);
            Assert.IsNotNull(m3);
            Assert.AreEqual(m3.ValueCodes.Count, 10);
            Assert.IsNotNull(o3);
            Assert.AreEqual(o3.ValueCodes.Count, 0);
        }


        private SelectionHandler GetSelectionHandler()
        {
            var configOptionsMock = new Mock<PxApiConfigurationOptions>();
            configOptionsMock.SetupGet(x => x.MaxDataCells).Returns(100000);

            var configServiceMock = new Mock<IPxApiConfigurationService>();
            configServiceMock.Setup(x => x.GetConfiguration()).Returns(configOptionsMock.Object);
            return new SelectionHandler(configServiceMock.Object);
        }

        private Mock<IPXModelBuilder> GetPxModelBuilderMock(PXModel model)
        {
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            return builderMock;
        }
    }
}
