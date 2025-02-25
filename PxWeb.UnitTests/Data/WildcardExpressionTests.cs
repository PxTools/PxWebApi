using PxWeb.Code.Api2.DataSelection.SelectionExpressions;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class WildcardExpressionTests
    {
        [TestMethod]
        public void NotAWildcardmExpression_CanHandle_ReturnFalse()
        {
            // Arrange
            var expression = new WildcardExpression();

            // Act
            var canHandle = expression.CanHandle("TOP(10)");

            // Assert
            Assert.IsFalse(canHandle);
        }

        [TestMethod]
        public void OneWildcard_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();

            // Act
            var canHandle = expression.CanHandle("test*");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void TwoWildcard_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();

            // Act
            var canHandle = expression.CanHandle("*test*");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void ThreeWildcard_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();

            // Act
            var canHandle = expression.CanHandle("*tes*t*");

            // Assert
            Assert.IsTrue(canHandle);
        }


        [TestMethod]
        public void OneWildcard_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            // Act
            var canHandle = expression.Verfiy("test*", out problem);

            // Assert
            Assert.IsTrue(canHandle);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OneMiddleWildcard_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            // Act
            var canHandle = expression.Verfiy("te*st", out problem);

            // Assert
            Assert.IsFalse(canHandle);
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void OneMiddleWildcardStartsWith_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            // Act
            var canHandle = expression.Verfiy("test*", out problem);

            // Assert
            Assert.IsTrue(canHandle);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void OneMiddleWildcardEndsWith_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            // Act
            var canHandle = expression.Verfiy("*test", out problem);

            // Assert
            Assert.IsTrue(canHandle);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TwoWildcard_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            // Act
            var canHandle = expression.Verfiy("*test*", out problem);

            // Assert
            Assert.IsTrue(canHandle);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void TwoWildcard_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            // Act
            var canHandle = expression.Verfiy("*te*st", out problem);

            // Assert
            Assert.IsFalse(canHandle);
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void ThreeWildcard_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;
            // Act
            var canHandle = expression.Verfiy("*tes*t*", out problem);

            // Assert
            Assert.IsFalse(canHandle);
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void AddNonExistingValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "X*", out problem);

            // Assert
            Assert.AreEqual(0, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void AddAllValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "*", out problem);

            // Assert
            Assert.AreEqual(20, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void ValueAreReadyInlist_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 5, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_0_clsv_A");

            //Act
            expression.AddToSelection(variable, selection, "*", out problem);

            // Assert
            Assert.AreEqual(5, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void AddValuesStartingWith_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "Code_0*", out problem);

            // Assert
            Assert.AreEqual(1, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void AddValuesEndigWith_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new WildcardExpression();
            Problem? problem;
            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "*9_clsv_A", out problem);

            // Assert
            Assert.AreEqual(2, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

    }
}
