namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class DefaultSelectionTest
    {

        [TestMethod]
        public void ShouldReturnSingleSelectionFromFallbackWith1500ValuesSelected()
        {
            // Arrange
            Bjarte3 algoritm = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithOnlyOneVariable(2000));

            // Act
            var selection = algoritm.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Selection.Count, 1);
            Assert.AreEqual(selection.Selection[0].ValueCodes.Count, 1500);
        }

        [TestMethod]
        public void ShouldReturnSingleSelectionFromFallbackWithOriginalNumberOfValuesSelected()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithOnlyOneVariable(50));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection);
            Assert.AreEqual(selection.Selection.Count, 1);
            Assert.AreEqual(selection.Selection[0].ValueCodes.Count, 50);
        }

        [TestMethod]
        public void ShouldHave2SelectionsByFallback()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith1MandantoryAnd1NoneMandantoryVariables(50, 150));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 2);
        }

        [TestMethod]
        public void MandantoryVariableShouldBeSelectedByFallback()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith1MandantoryAnd3NoneMandantoryVariables(50, 150));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 4);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var o1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var o2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

            Assert.IsNotNull(m1);
            Assert.AreEqual(m1.ValueCodes.Count, 11);
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
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith2MandantoryAnd2NoneMandantoryVariables(10, 10));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 4);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var m2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var o2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

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
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWith3MandantoryAnd1NoneMandantoryVariables(10, 10));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 4);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var m2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var m3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

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
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsAndTime(3, 30));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 2);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 3);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 13);
        }

        [TestMethod]
        public void CaseA2_ShouldReturnContentsInHeadAndTimeInStubMax13TimeValues()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsAndTime(30, 30));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 2);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 30);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 13);
        }

        [TestMethod]
        public void CaseB1_ShouldReturn13TimePeriodsAndClassificationVariable()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAnd1ClassificationVariable(1, 30, 3000));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 3);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);


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
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAnd1ClassificationVariable(10, 30, 3000));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 3);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);


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
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 3, 0));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 5);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 11);
            Assert.IsNotNull(cls2);
            Assert.AreEqual(cls2.ValueCodes.Count, 0);
            Assert.IsNotNull(cls3);
            Assert.AreEqual(cls3.ValueCodes.Count, 20);
        }

        [TestMethod]
        public void CaseC2_ShouldReturnFirstMandantoryAndLastClassificationVariable()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 2, 1));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 5);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 0);
            Assert.IsNotNull(cls2);
            Assert.AreEqual(cls2.ValueCodes.Count, 20);
            Assert.IsNotNull(cls3m);
            Assert.AreEqual(cls3m.ValueCodes.Count, 11);
        }

        [TestMethod]
        public void CaseC2_ShouldReturnThe2MandantoryVariables()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 1, 2));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 5);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 0);
            Assert.IsNotNull(cls2m);
            Assert.AreEqual(cls2m.ValueCodes.Count, 11);
            Assert.IsNotNull(cls3m);
            Assert.AreEqual(cls3m.ValueCodes.Count, 20);
        }

        [TestMethod]
        public void CaseC2_ShouldReturnTheFirstAndLastMandantoryVariables()
        {
            // Arrange
            Bjarte3 selectionHandler = new Bjarte3();
            var builderMock = GetPxModelBuilderMock(ModelStore.GetModelWithContentsTimeAndXClassificationVariable(10, 30, 20, 1, 3));

            // Act
            var selection = selectionHandler.GetDefaultSelection(builderMock.Object);

            // Assert
            Assert.IsNotNull(selection.Selection);
            Assert.AreEqual(selection.Selection.Count, 6);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);
            var cls4m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[5].Code);


            Assert.IsNotNull(contens);
            Assert.AreEqual(contens.ValueCodes.Count, 1);
            Assert.IsNotNull(time);
            Assert.AreEqual(time.ValueCodes.Count, 1);
            Assert.IsNotNull(cls1);
            Assert.AreEqual(cls1.ValueCodes.Count, 0);
            Assert.IsNotNull(cls2m);
            Assert.AreEqual(cls2m.ValueCodes.Count, 11);
            Assert.IsNotNull(cls3m);
            Assert.AreEqual(cls3m.ValueCodes.Count, 1);
            Assert.IsNotNull(cls4m);
            Assert.AreEqual(cls4m.ValueCodes.Count, 20);
        }


        private Mock<IPXModelBuilder> GetPxModelBuilderMock(PXModel model)
        {
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            return builderMock;
        }
    }
}
