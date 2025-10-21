using PxWeb.Code.Api2.DataSelection.SelectionExpressions;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class FromExpressionTests
    {
        [TestMethod]
        public void NotAFromExpression_CanHandle_ReturnFalse()
        {
            // Arrange
            var expression = new FromExpression();

            // Act
            var canHandle = expression.CanHandle("TOP(10)");

            // Assert
            Assert.IsFalse(canHandle);
        }

        [TestMethod]
        public void MixedCase_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();

            // Act
            var canHandle = expression.CanHandle("fRom(10)");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void MixedCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("fROm(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void UpperCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("FROM(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void LowerCases_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("from(10)", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void MissingParameter_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("FROM()", out problem));
            Assert.IsNotNull(problem);
        }


        [TestMethod]
        public void SourroundingText_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("FROM(2010),a", out problem));
            Assert.IsNotNull(problem);
        }


        [TestMethod]
        public void AddNonExistingValue_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "FROM(THIS-CODE-DOSE-NOT-EXIST)", out problem);

            // Assert
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void AddFiveValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 10, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "FROM(Code_5_clsv_A)", out problem);

            // Assert
            Assert.HasCount(5, selection.ValueCodes);
            Assert.IsNull(problem);
        }


        [TestMethod]
        public void ValueAreReadyInlist_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 10, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_6_clsv_A");


            //Act
            expression.AddToSelection(variable, selection, "FROM(Code_5_clsv_A)", out problem);

            // Assert
            Assert.HasCount(5, selection.ValueCodes);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TimeValue_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new FromExpression();
            Problem? problem;
            var variable = ModelStore.CreateTimeVariable("Tid", PlacementType.Stub, 10, 1990);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();

            //Act
            expression.AddToSelection(variable, selection, "FROM(1995)", out problem);

            // Assert
            Assert.HasCount(5, selection.ValueCodes);
            Assert.IsNull(problem);
        }
    }
}
