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
            Assert.HasCount(1, selection.Selection);
            Assert.HasCount(1500, selection.Selection[0].ValueCodes);
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
            Assert.HasCount(1, selection.Selection);
            Assert.HasCount(50, selection.Selection[0].ValueCodes);
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
            Assert.HasCount(2, selection.Selection);
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

            Assert.HasCount(4, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var o1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var o2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

            Assert.IsNotNull(m1);
            Assert.HasCount(11, m1.ValueCodes);
            Assert.IsNotNull(o1);
            Assert.HasCount(0, o1.ValueCodes);
            Assert.IsNotNull(o2);
            Assert.HasCount(0, o2.ValueCodes);
            Assert.IsNotNull(o3);
            Assert.HasCount(150, o3.ValueCodes);
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
            Assert.HasCount(4, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var m2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var o2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

            Assert.IsNotNull(m1);
            Assert.HasCount(10, m1.ValueCodes);
            Assert.IsNotNull(m2);
            Assert.HasCount(10, m2.ValueCodes);
            Assert.IsNotNull(o2);
            Assert.HasCount(0, o2.ValueCodes);
            Assert.IsNotNull(o3);
            Assert.HasCount(0, o3.ValueCodes);
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
            Assert.HasCount(4, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var m1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var m2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var m3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var o3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);

            Assert.IsNotNull(m1);
            Assert.HasCount(10, m1.ValueCodes);
            Assert.IsNotNull(m2);
            Assert.HasCount(1, m2.ValueCodes);
            Assert.IsNotNull(m3);
            Assert.HasCount(10, m3.ValueCodes);
            Assert.IsNotNull(o3);
            Assert.HasCount(0, o3.ValueCodes);
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
            Assert.HasCount(2, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(3, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(13, time.ValueCodes);
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
            Assert.HasCount(2, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(30, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(13, time.ValueCodes);
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
            Assert.HasCount(3, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(1, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(13, time.ValueCodes);
            Assert.IsNotNull(cls1);
            Assert.HasCount(1500, cls1.ValueCodes);
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
            Assert.HasCount(3, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(10, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(1, time.ValueCodes);
            Assert.IsNotNull(cls1);
            Assert.HasCount(1500, cls1.ValueCodes);
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
            Assert.HasCount(5, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(1, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(1, time.ValueCodes);
            Assert.IsNotNull(cls1);
            Assert.HasCount(11, cls1.ValueCodes);
            Assert.IsNotNull(cls2);
            Assert.HasCount(0, cls2.ValueCodes);
            Assert.IsNotNull(cls3);
            Assert.HasCount(20, cls3.ValueCodes);
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
            Assert.HasCount(5, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(1, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(1, time.ValueCodes);
            Assert.IsNotNull(cls1);
            Assert.HasCount(0, cls1.ValueCodes);
            Assert.IsNotNull(cls2);
            Assert.HasCount(20, cls2.ValueCodes);
            Assert.IsNotNull(cls3m);
            Assert.HasCount(11, cls3m.ValueCodes);
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
            Assert.HasCount(5, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(1, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(1, time.ValueCodes);
            Assert.IsNotNull(cls1);
            Assert.HasCount(0, cls1.ValueCodes);
            Assert.IsNotNull(cls2m);
            Assert.HasCount(11, cls2m.ValueCodes);
            Assert.IsNotNull(cls3m);
            Assert.HasCount(20, cls3m.ValueCodes);
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
            Assert.HasCount(6, selection.Selection);

            var meta = builderMock.Object.Model.Meta;
            var contens = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[0].Code);
            var time = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[1].Code);
            var cls1 = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[2].Code);
            var cls2m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[3].Code);
            var cls3m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[4].Code);
            var cls4m = selection.Selection.FirstOrDefault(x => x.VariableCode == meta.Variables[5].Code);


            Assert.IsNotNull(contens);
            Assert.HasCount(1, contens.ValueCodes);
            Assert.IsNotNull(time);
            Assert.HasCount(1, time.ValueCodes);
            Assert.IsNotNull(cls1);
            Assert.HasCount(0, cls1.ValueCodes);
            Assert.IsNotNull(cls2m);
            Assert.HasCount(11, cls2m.ValueCodes);
            Assert.IsNotNull(cls3m);
            Assert.HasCount(1, cls3m.ValueCodes);
            Assert.IsNotNull(cls4m);
            Assert.HasCount(20, cls4m.ValueCodes);
        }


        private static Mock<IPXModelBuilder> GetPxModelBuilderMock(PXModel model)
        {
            var builderMock = new Mock<IPXModelBuilder>();
            builderMock.Setup(x => x.Model).Returns(model);
            return builderMock;
        }
    }
}
