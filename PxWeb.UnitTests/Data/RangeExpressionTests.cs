using PxWeb.Code.Api2.DataSelection.SelectionExpressions;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class RangeExpressionTests
    {
        [TestMethod]
        public void NotARangeExpression_CanHandle_ReturnFalse()
        {
            // Arrange
            var expression = new RangeExpression();

            // Act
            var canHandle = expression.CanHandle("TOP(10)");

            // Assert
            Assert.IsFalse(canHandle);
        }

        [TestMethod]
        public void MixedCase_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();

            // Act
            var canHandle = expression.CanHandle("raNge(10)");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void MixedCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("raNge(10,12)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void UpperCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("RANGE(10,12)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void LowerCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("range(10,10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void MissingParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("RANGE()", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void OnlyOneParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("RANGE(param1)", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void TwoParameters_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("RANGE(10,5)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void SourroundingText_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("RANGE(10,12),a", out problem));
            Assert.IsNotNull(problem);
        }


        [TestMethod]
        public void ValueAreReadyInlist_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_0_clsv_A");

            //Act
            var ok = expression.AddToSelection(variable, selection, "RANGE(Code_0_clsv_A,Code_4_clsv_A)", out problem);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual(5, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void ValueAreReadyInlistReversed_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_0_clsv_A");

            //Act
            var ok = expression.AddToSelection(variable, selection, "RANGE(Code_4_clsv_A,Code_0_clsv_A)", out problem);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual(5, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void ValueAreReadyInlistAndValueCodeDoseNotExist_AddToSelection_ReturnFalse()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_0_clsv_A");

            //Act
            var ok = expression.AddToSelection(variable, selection, "RANGE(Code_10_clsv_A,Code_4_clsv_A)", out problem);

            // Assert
            Assert.IsFalse(ok);
            Assert.AreEqual(1, selection.ValueCodes.Count);
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void TimeValue_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 20, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            expression.AddToSelection(variable, selection, "RANGE(1995,2000)", out problem);

            // Assert
            Assert.AreEqual(6, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TimeValueDoseNotExist_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new RangeExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 20, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            var ok = expression.AddToSelection(variable, selection, "RANGE(1985,2000)", out problem);

            // Assert
            Assert.IsFalse(ok);
            Assert.AreEqual(0, selection.ValueCodes.Count);
            Assert.IsNotNull(problem);
        }

    }
}
