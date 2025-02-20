using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Helper.Api2
{
    public class SelectionUtil
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

        public static VariablesSelection CreateEmptyVariablesSelection()
        {
            var selections = new VariablesSelection();
            selections.Selection = new List<VariableSelection>();
            selections.Placement = new VariablePlacementType() { Heading = new List<string>(), Stub = new List<string>() };
            return selections;
        }

        public static void AddValue(VariableSelection selection, string valueCode)
        {
            if (!selection.ValueCodes.Contains(valueCode))
            {
                selection.ValueCodes.Add(valueCode);
            }

        }

        public static void AddValues(VariableSelection selection, IEnumerable<string> valueCodes)
        {
            foreach (var valueCode in valueCodes)
            {
                if (!selection.ValueCodes.Contains(valueCode))
                {
                    selection.ValueCodes.Add(valueCode);
                }
            }
        }

    }


}
