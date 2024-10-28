using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            variablesSelection.Palcement = null;
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
            variablesSelection.Palcement = new VariablePlacementType();
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
            
            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Stub = new List<string>();
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

            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Heading.Add("Age");
            variablesSelection.Palcement.Stub = new List<string>();
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

            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Heading.Add("PointOfTime");
            variablesSelection.Palcement.Heading.Add("MEASURE");
            variablesSelection.Palcement.Stub = new List<string>();
            variablesSelection.Palcement.Stub.Add("PointOfTime");
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

            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Heading.Add("TIME");
            variablesSelection.Palcement.Heading.Add("MEASURE");
            variablesSelection.Palcement.Stub = new List<string>();
            variablesSelection.Palcement.Stub.Add("PointOfTime");
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

            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Heading.Add("TIME");
            variablesSelection.Palcement.Heading.Add("MEASURE");
            variablesSelection.Palcement.Stub = new List<string>();
            variablesSelection.Palcement.Stub.Add("Measure");
            variablesSelection.Palcement.Stub.Add("X");
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

            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Heading.Add("TIME");
            variablesSelection.Palcement.Heading.Add("MEASURE");
            variablesSelection.Palcement.Stub = new List<string>();
            variablesSelection.Palcement.Stub.Add("GENDER");
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
            Assert.AreEqual(placement.Heading.Count, 2);
        }

        [TestMethod]
        public void ShouldReturnPreferredPlacementWhenOnlySpecifyingHeading()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Heading.Add("MEASURE");
            variablesSelection.Palcement.Stub = new List<string>();
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
            Assert.AreEqual(placement.Stub.Count, 2);
        }

        [TestMethod]
        public void ShouldReturnPreferredPlacementWhenOnlySpecifyingHeadingWithElimination()
        {
            // Arrange
            IPlacementHandler placementHandler = new PlacementHandler();
            VariablesSelection variablesSelection = new VariablesSelection();

            variablesSelection.Palcement = new VariablePlacementType();
            variablesSelection.Palcement.Heading = new List<string>();
            variablesSelection.Palcement.Heading.Add("MEASURE");
            variablesSelection.Palcement.Stub = new List<string>();
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
            Assert.AreEqual(placement.Stub.Count, 1);
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
