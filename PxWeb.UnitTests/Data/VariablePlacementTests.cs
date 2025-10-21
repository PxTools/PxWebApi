namespace PxWeb.UnitTests.Data
{

    [TestClass]
    public class VariablePlacementTests
    {
        [TestMethod]
        public void ShouldReturnNoPreferredPlacementIfPlacemntIsNull()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();
            variablesSelection.Placement = null;
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;
            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNull(placement);

        }

        [TestMethod]
        public void ShouldReturnNoPreferredPlacementIfPlacemntIsNull2()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();
            variablesSelection.Placement = new VariablePlacementType();
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;
            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNull(placement);

        }

        [TestMethod]
        public void ShouldReturnNoPreferredPlacementIfPlacemntIsEmpty()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Stub = new List<string>();
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNull(placement);

        }

        [TestMethod]
        public void ShouldReturnNoPreferredPlacementIfPlacemntVariableDoesNotExist()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Heading.Add("Age");
            variablesSelection.Placement.Stub = new List<string>();
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNotNull(problem);
            Assert.IsNull(placement);
        }

        [TestMethod]
        public void ShouldReturnNoPreferredPlacementIfPlacemntVariableIsDouplicated()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Heading.Add("PointOfTime");
            variablesSelection.Placement.Heading.Add("MEASURE");
            variablesSelection.Placement.Stub = new List<string>();
            variablesSelection.Placement.Stub.Add("PointOfTime");
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNotNull(problem);
            Assert.IsNull(placement);
        }

        [TestMethod]
        public void ShouldNotReturnPreferredPlacementWhenUsingTime()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Heading.Add("TIME");
            variablesSelection.Placement.Heading.Add("MEASURE");
            variablesSelection.Placement.Stub = new List<string>();
            variablesSelection.Placement.Stub.Add("PointOfTime");
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNotNull(problem);
            Assert.IsNull(placement);
        }

        [TestMethod]
        public void ShouldNotReturnPreferredPlacementWhenSpecifyingTooManyVariables()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Heading.Add("TIME");
            variablesSelection.Placement.Heading.Add("MEASURE");
            variablesSelection.Placement.Stub = new List<string>();
            variablesSelection.Placement.Stub.Add("Measure");
            variablesSelection.Placement.Stub.Add("X");
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNotNull(problem);
            Assert.IsNull(placement);
        }


        [TestMethod]
        public void ShouldReturnPreferredPlacementWhenUsingTime()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Heading.Add("TIME");
            variablesSelection.Placement.Heading.Add("MEASURE");
            variablesSelection.Placement.Stub = new List<string>();
            variablesSelection.Placement.Stub.Add("GENDER");
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(placement);
            Assert.HasCount(2, placement.Heading);
        }

        [TestMethod]
        public void ShouldReturnPreferredPlacementWhenOnlySpecifyingHeading()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Heading.Add("MEASURE");
            variablesSelection.Placement.Stub = new List<string>();
            Selection[]? selection = GetSelectionForAllVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(placement);
            Assert.HasCount(2, placement.Stub);
        }

        [TestMethod]
        public void ShouldReturnPreferredPlacementWhenOnlySpecifyingHeadingWithElimination()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Placement = new VariablePlacementType();
            variablesSelection.Placement.Heading = new List<string>();
            variablesSelection.Placement.Heading.Add("MEASURE");
            variablesSelection.Placement.Stub = new List<string>();
            Selection[]? selection = GetSelectionForMandantoryVariables();
            PXMeta meta = ModelStore.CreateModelA().Meta;

            Problem? problem;

            // Act
            var placement = placementHandler.GetPlacment(variablesSelection,
                                                         selection,
                                                         meta,
                                                         out problem);

            // Assert
            Assert.IsNull(problem);
            Assert.IsNotNull(placement);
            Assert.HasCount(1, placement.Stub);
        }

        private static Selection[] GetSelectionForAllVariables()
        {
            var selections = new List<Selection>();
            var selection = new Selection("PointOfTime");
            selection.ValueCodes.Add("2000");
            selection.ValueCodes.Add("2001");
            selection.ValueCodes.Add("2002");
            selections.Add(selection);
            selection = new Selection("Measure");
            selection.ValueCodes.Add("M1");
            selections.Add(selection);
            selection = new Selection("GENDER");
            selection.ValueCodes.Add("M");
            selections.Add(selection);
            return selections.ToArray();

        }


        private static Selection[] GetSelectionForMandantoryVariables()
        {
            var selections = new List<Selection>();
            var selection = new Selection("PointOfTime");
            selection.ValueCodes.Add("2000");
            selection.ValueCodes.Add("2001");
            selection.ValueCodes.Add("2002");
            selections.Add(selection);
            selection = new Selection("Measure");
            selection.ValueCodes.Add("M1");
            selections.Add(selection);
            selection = new Selection("GENDER");
            selections.Add(selection);
            return selections.ToArray();
        }


    }
}
