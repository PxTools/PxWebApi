using PxWeb.Code.Api2.DataSelection.SelectionExpressions;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class ToExpressionTests
    {
        [TestMethod]
        public void NotAFromExpression_CanHandle_ReturnFalse()
        {
            // Arrange
            var expression = new ToExpression();

            // Act
            var canHandle = expression.CanHandle("TOP(10)");

            // Assert
            Assert.IsFalse(canHandle);
        }

        [TestMethod]
        public void MixedCase_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();

            // Act
            var canHandle = expression.CanHandle("tO(10)");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void MixedCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("tO(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void UpperCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("TO(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void LowerCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("to(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void MissingParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("TO()", out problem));
            Assert.IsNotNull(problem);
        }


        [TestMethod]
        public void SourroundingText_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("TO(2010),a", out problem));
            Assert.IsNotNull(problem);
        }


        [TestMethod]
        public void AddNonExistingValue_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "TO(THIS-CODE-DOSE-NOT-EXIST)", out problem);

            // Assert
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void AddFiveValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 10, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "TO(Code_5_clsv_A)", out problem);

            // Assert
            Assert.HasCount(6, selection.ValueCodes);
            Assert.IsNull(problem);
        }


        [TestMethod]
        public void ValueAreReadyInlist_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 10, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_2_clsv_A");


            //Act
            expression.AddToSelection(variable, selection, "TO(Code_5_clsv_A)", out problem);

            // Assert
            Assert.HasCount(6, selection.ValueCodes);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TimeValue_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new ToExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 10, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            expression.AddToSelection(variable, selection, "TO(1995)", out problem);

            // Assert
            Assert.HasCount(6, selection.ValueCodes);
            Assert.IsNull(problem);
        }
    }
}
