using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Helper.Api2
{
    public static class VariablesSelectionExtensions
    {
        public static void AddStubVariable(this VariablesSelection selections, Variable variable, Func<Variable, int, string[]> valuesFunction, int numberOfValues = 1500)
        {
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.VariableCode = variable.Code;
            selection.Codelist = GetCodeList(variable);
            selection.ValueCodes.AddRange(valuesFunction(variable, numberOfValues));
            selections.Selection.Add(selection);
            selections.Placement?.Stub.Add(variable.Code);
        }

        public static void AddHeadingVariable(this VariablesSelection selections, Variable variable, Func<Variable, int, string[]> valuesFunction, int numberOfValues = 13)
        {
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.VariableCode = variable.Code;
            selection.Codelist = GetCodeList(variable);
            selection.ValueCodes.AddRange(valuesFunction(variable, numberOfValues));
            selections.Selection.Add(selection);
            selections.Placement?.Heading.Add(variable.Code);
        }

        public static void AddVariableToHeading(this VariablesSelection selections, Variable variable, Func<Variable, int, string[]> valuesFunction)
        {
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.VariableCode = variable.Code;
            selection.Codelist = GetCodeList(variable);
            selection.ValueCodes.AddRange(valuesFunction(variable, 1));
            selections.Selection.Add(selection);
            selections.Placement?.Heading.Add(variable.Code);
        }

        public static void EliminateVariable(this VariablesSelection selections, Variable variable)
        {
            var selection = new VariableSelection();
            selection.ValueCodes = new List<string>();
            selection.VariableCode = variable.Code;
            selection.Codelist = GetCodeList(variable);
            selections.Selection.Add(selection);

        }

        private static string? GetCodeList(Variable? variable)
        {
            if (variable == null)
            {
                return null;
            }

            if (variable.CurrentGrouping != null)
            {
                return "agg_" + variable.CurrentGrouping.ID;
            }

            if (variable.CurrentValueSet != null)
            {
                return "vs_" + variable.CurrentValueSet.ID;
            }

            return null;

        }

    }
}
