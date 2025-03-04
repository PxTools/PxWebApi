using PxWeb.Code.Api2.DataSelection.SelectionExpressions;

namespace PxWeb.UnitTests.Data
{
    [TestClass]
    public class QuestionmarkExpressionTests
    {
        [TestMethod]
        public void NotAQuestionmarkExpression_CanHandle_ReturnFalse()
        {
            // Arrange
            var expression = new QuestionmarkExpression();

            // Act
            var canHandle = expression.CanHandle("TOP(10)");

            // Assert
            Assert.IsFalse(canHandle);
        }

        [TestMethod]
        public void OneQuestionmark_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();

            // Act
            var canHandle = expression.CanHandle("?10");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void TwoQuestionmark_CanHandle_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();

            // Act
            var canHandle = expression.CanHandle("??10");

            // Assert
            Assert.IsTrue(canHandle);
        }

        [TestMethod]
        public void OneQuestionmark_Verify_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();
            Problem? problem;

            // Assert
            Assert.IsTrue(expression.Verfiy("?111", out problem));
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void NoQuestionmark_Verify_ReturnFalse()
        {
            // Arrange
            var expression = new QuestionmarkExpression();
            Problem? problem;

            // Assert
            Assert.IsFalse(expression.Verfiy("111", out problem));
            Assert.IsNotNull(problem);
        }

        [TestMethod]
        public void AddNoMatchingValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "?Test", out problem);

            // Assert
            Assert.AreEqual(0, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void AddPatternMatchingValues_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "Code_?_clsv_A", out problem);

            // Assert
            Assert.AreEqual(10, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void AddValueAlreadySelected_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.ValueCodes.Add("Code_0_clsv_A");

            //Act
            expression.AddToSelection(variable, selection, "Code_?_clsv_A", out problem);

            // Assert
            Assert.AreEqual(10, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void Add13CharacterLongCodes_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "?????????????", out problem);

            // Assert
            Assert.AreEqual(10, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }

        [TestMethod]
        public void Add14CharacterLongCodes_AddToSelection_ReturnTrue()
        {
            // Arrange
            var expression = new QuestionmarkExpression();
            Problem? problem;

            var variable = ModelStore.CreateClassificationVariable("A", PlacementType.Stub, 20, true);
            var selection = new VariableSelection();

            //Act
            expression.AddToSelection(variable, selection, "??????????????", out problem);

            // Assert
            Assert.AreEqual(10, selection.ValueCodes.Count);
            Assert.IsNull(problem);
        }
    }
}
