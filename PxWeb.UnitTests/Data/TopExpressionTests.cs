using PxWeb.Code.Api2.DataSelection.SelectionExpressions;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class TopExpressionTests
    {
        [TestMethod]
        public void NotATopExpression_CanHandle_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();

            // Act
            var canHandle = expression.CanHandle("BOTTOM(10)");

            // Assert
            Assert.IsFalse(canHandle);
        }

        [TestMethod]
        public void MixedCase_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();

            // Act
            var canHandle = expression.CanHandle("tOp(10)");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void MixedCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            // Assert
            Assert.IsTrue(expression.Verfiy("ToP(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void UpperCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("TOP(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void LowerCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("top(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void MissingParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("top()", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void ParameterIsNotNumeric_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("top(tio)", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void ParameterIsNotNumeric2_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("TOP(10tio)", out problem));
            Assert.IsNotNull(problem);
        }
        [TestMethod]
        public void TwoParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("TOP(10,5)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void SecondParamterIsNotNumeric_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("TOP(10,five)", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void SourroundingText_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("TOP(10,five),a", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void NegativeParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("TOP(-10)", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void AddNonExistingValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "TOP(10)", out problem);

            // Assert
            Assert.AreEqual(10, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OnlyFiveValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "TOP(10)", out problem);

            // Assert
            Assert.AreEqual(5, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OnlyFiveValuesWithOfset_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "TOP(10,2)", out problem);

            // Assert
            Assert.AreEqual(3, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OfsetLagrgerThenValueCount_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "TOP(10, 20)", out problem);

            // Assert
            Assert.AreEqual(0, selection.ValueCodes.Count); // No values should be added
            Assert.IsNull(problem);
        }


        [TestMethod]
        public void ValueAreReadyInlist_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_0_clsv_A");

            //Act
            expression.AddToSelection(variable, selection, "TOP(10)", out problem);

            // Assert
            Assert.AreEqual(5, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TimeValue_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 20, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            expression.AddToSelection(variable, selection, "TOP(1)", out problem);

            // Assert
            Assert.AreEqual(1, selection.ValueCodes.Count);
            Assert.AreEqual("2009", selection.ValueCodes[0]);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TimeValueWithOfset_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new TopExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 20, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            expression.AddToSelection(variable, selection, "Top(1, 5)", out problem);

            // Assert
            Assert.AreEqual(1, selection.ValueCodes.Count);
            Assert.AreEqual("2004", selection.ValueCodes[0]);
            Assert.IsNull(problem);
        }
    }
}
