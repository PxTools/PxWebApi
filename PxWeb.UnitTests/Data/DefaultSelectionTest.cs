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

        [TestMethod]
        public void CaseA1_ShouldReturnContentsInHeadAndTimeInStub()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsAndTime(3, 30));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 2);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 3);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 30);
        }

        [TestMethod]
        public void CaseA2_ShouldReturnContentsInHeadAndTimeInStubMax13TimeValues()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsAndTime(30, 30));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 2);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 30);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 13);
        }

        [TestMethod]
        public void CaseB1_ShouldReturn13TimePeriodsAndClassificationVariable()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAnd1ClassificationVariable(1, 30, 3000));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 3);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 13);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 1500);
        }

        [TestMethod]
        public void CaseB2_ShouldReturn1TimePeriodsAndContentsClassificationVariable()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAnd1ClassificationVariable(10, 30, 3000));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 3);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 10);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 1500);
        }

        [TestMethod]
        public void CaseC1_ShouldReturnFirstAndLastClassificationVariable()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 3, 0));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 5);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 20);
            Assert.IsNotNull(cls2);
            Assert.AreEqual(cls2.ValueCodes.Count, 0);
            Assert.IsNotNull(cls3);
            Assert.AreEqual(cls3.ValueCodes.Count, 20);
        }

        [TestMethod]
        public void CaseC2_ShouldReturnFirstMandantoryAndLastClassificationVariable()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 2, 1));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 5);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 0);
            Assert.IsNotNull(cls2);
            Assert.AreEqual(cls2.ValueCodes.Count, 20);
            Assert.IsNotNull(cls3m);
            Assert.AreEqual(cls3m.ValueCodes.Count, 20);
        }

        [TestMethod]
        public void CaseC2_ShouldReturnThe2MandantoryVariables()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 1, 2));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 5);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2m = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 0);
            Assert.IsNotNull(cls2m);
            Assert.AreEqual(cls2m.ValueCodes.Count, 20);
            Assert.IsNotNull(cls3m);
            Assert.AreEqual(cls3m.ValueCodes.Count, 20);
        }

        [TestMethod]
        public void CaseC2_ShouldReturnTheFirstAndLastMandantoryVariables()
        {
            // Arrange
            SelectionHandler selectionHandler = GetSelectionHandler();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 1, 3));

            Problem? problem;
            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object, out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Length, 6);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2m = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);
            var cls4m = selection.FirstOrDefault(x => x.VariableCode == meta.Variables[5].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 0);
            Assert.IsNotNull(cls2m);
            Assert.AreEqual(cls2m.ValueCodes.Count, 20);
            Assert.IsNotNull(cls3m);
            Assert.AreEqual(cls3m.ValueCodes.Count, 1);
            Assert.IsNotNull(cls4m);
            Assert.AreEqual(cls4m.ValueCodes.Count, 20);
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
