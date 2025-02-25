using PxWeb.Code.Api2.DataSelection.SelectionExpressions;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class BottomExpressionTests
    {
        [TestMethod]
        public void NotABottomExpression_CanHandle_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();

            // Act
            var canHandle = expression.CanHandle("TOP(10)");

            // Assert
            Assert.IsFalse(canHandle);
        }

        [TestMethod]
        public void MixedCase_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();

            // Act
            var canHandle = expression.CanHandle("boTTom(10)");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void MixedCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("boTTom(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void UpperCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("BOTTOM(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void LowerCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("bottom(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void MissingParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("bottom()", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void ParameterIsNotNumeric_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("bottom(tio)", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void ParameterIsNotNumeric2_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("bottom(10tio)", out problem));
            Assert.IsNotNull(problem);
        }
        [TestMethod]
        public void TwoParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("bottom(10,5)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void SecondParamterIsNotNumeric_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("bottom(10,five)", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void SourroundingText_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("bottom(10,five),a", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void NegativeParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("bottom(-10)", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void AddNonExistingValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "bottom(10)", out problem);

            // Assert
            Assert.AreEqual(10, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OnlyFiveValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "bottom(10)", out problem);

            // Assert
            Assert.AreEqual(5, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OnlyFiveValuesWithOfset_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "bottom(10,2)", out problem);

            // Assert
            Assert.AreEqual(3, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OfsetLagrgerThenValueCount_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "bottom(10, 20)", out problem);

            // Assert
            Assert.AreEqual(0, selection.ValueCodes.Count); // No values should be added
            Assert.IsNull(problem);
        }


        [TestMethod]
        public void ValueAreReadyInlist_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_0_clsv_A");


            //Act
            expression.AddToSelection(variable, selection, "bottom(10)", out problem);

            // Assert
            Assert.AreEqual(5, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TimeValue_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 20, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            expression.AddToSelection(variable, selection, "bottom(1)", out problem);

            // Assert
            Assert.AreEqual(1, selection.ValueCodes.Count);
            Assert.AreEqual("1990", selection.ValueCodes[0]);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TimeValueWithOfset_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new BottomExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 20, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            expression.AddToSelection(variable, selection, "bottom(1, 5)", out problem);

            // Assert
            Assert.AreEqual(1, selection.ValueCodes.Count);
            Assert.AreEqual("1995", selection.ValueCodes[0]);
            Assert.IsNull(problem);
        }
    }
}
