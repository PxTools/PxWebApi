using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Helper.Api2
{
    public static class SelectionUtil
    {
        /// <summary>
        /// Helper function that determis which variable should to the Stub or Heading
        /// </summary>
        /// <param name="one">first variable</param>
        /// <param name="two">second variable</param>
        /// <returns>variable that should go to the stub and the variable hat should go to the heading</returns>
        public static (Variable, Variable) StubOrHeading(Variable one, Variable two)
        {
            if (one.Values.Count > two.Values.Count)
            {
                return (one, two);
            }
            else
            {
                return (two, one);
            }
        }

        /// <summary>
        /// Get the codes for the first values in the variable
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="count">The number of value codes to fetch</param>
        /// <returns></returns>
        public static string[] GetCodes(Variable variable, int count)
        {
            var codes = variable.Values.Take(count).Select(value => value.Code).ToArray();

            return codes;
        }

        /// <summary>
        /// Get the codes for the last values in the variable
        /// Should only be called for time variables that are sorted in ascending order
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="count">The number of value codes to fetch</param>
        /// <returns></returns>
        public static string[] GetTimeCodes(Variable variable, int count)
        {
            var lstCodes = variable.Values.TakeLast(count).Select(value => value.Code).ToList();
            var codes = lstCodes.ToArray();

            return codes;
        }

        /// <summary>
        /// Create an empty VariablesSelection
        /// </summary>
        /// <returns>An empty variable selection with misc </returns>
        public static VariablesSelection CreateEmptyVariablesSelection()
        {
            var selections = new VariablesSelection();
            selections.Selection = new List<VariableSelection>();
            selections.Placement = new VariablePlacementType() { Heading = new List<string>(), Stub = new List<string>() };
            return selections;
        }

        /// <summary>
        /// Adds a value to a variable selection. Only adds the value if it is not already in the selection
        /// </summary>
        /// <param name="selection">The selection where to add the value</param>
        /// <param name="valueCode">Value codes to add</param>
        public static void AddValue(VariableSelection selection, string valueCode)
        {
            if (!selection.ValueCodes.Contains(valueCode))
            {
                selection.ValueCodes.Add(valueCode);
            }

        }

        /// <summary>
        /// Add values to a variable selection. Only adds values that are not already in the selection
        /// </summary>
        /// <param name="selection">The selection where to add the values</param>
        /// <param name="valueCodes">Value codes to add</param>
        public static void AddValues(VariableSelection selection, IEnumerable<string> valueCodes)
        {
            if (selection.ValueCodes is null)
            {
                selection.ValueCodes = new List<string>();
            }

            foreach (var valueCode in valueCodes)
            {
                if (!selection.ValueCodes.Contains(valueCode))
                {
                    selection.ValueCodes.Add(valueCode);
                }
            }
        }

        /// <summary>
        /// Check if a variable is mandatory
        /// </summary>
        /// <param name="model">The model containing the information about variables</param>
        /// <param name="variable">The variable to check if it is mandatory</param>
        /// <returns></returns>
        public static bool IsMandatory(PXModel model, VariableSelection variable)
        {
            bool mandatory = false;
            var mandatoryVariable = model.Meta.Variables.Where(x => x.Code.Equals(variable.VariableCode) && x.Elimination.Equals(false));

            if (mandatoryVariable.Any())
            {
                mandatory = true;
            }
            return mandatory;
        }

        /// <summary>
        /// Check if there is any selection in the VariablesSelection
        /// </summary>
        /// <param name="variablesSelection"></param>
        /// <returns></returns>
        public static bool UseDefaultSelection(VariablesSelection? variablesSelection)
        {
            return variablesSelection is null || !HasSelection(variablesSelection);
        }

        /// <summary>
        /// Check if there is any selection in the VariablesSelection
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        private static bool HasSelection(VariablesSelection selection)
        {

            if (selection.Selection is null)
            {
                return false;
            }

            if (selection.Selection.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
