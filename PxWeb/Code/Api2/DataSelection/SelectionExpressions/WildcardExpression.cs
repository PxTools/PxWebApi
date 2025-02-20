using System.Linq;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public class WildcardExpression : ISelectionExpression
    {
        public void AddToSelection(Variable variable, VariableSelection selection, string expression)
        {
            if (expression.Equals("*"))
            {
                // Select all values
                var variableValues = variable.Values.Select(v => v.Code);
                SelectionUtil.AddValues(selection, variableValues);

            }
            else if (expression.StartsWith("*") && expression.EndsWith("*"))
            {
                // *xyz*
                var matchExpression = expression.Substring(1, expression.Length - 2);
                var variableValues = variable.Values.Where(v => v.Code.Contains(matchExpression, StringComparison.InvariantCultureIgnoreCase)).Select(v => v.Code);
                SelectionUtil.AddValues(selection, variableValues);
            }
            else if (expression.StartsWith("*"))
            {
                // *xyz
                var matchExpression = expression.Substring(1);
                var variableValues = variable.Values.Where(v => v.Code.EndsWith(matchExpression, StringComparison.InvariantCultureIgnoreCase)).Select(v => v.Code);
                SelectionUtil.AddValues(selection, variableValues);
            }
            else if (expression.EndsWith("*"))
            {
                // xyz*
                var matchExpression = expression.Substring(0, expression.Length - 1);
                var variableValues = variable.Values.Where(v => v.Code.StartsWith(matchExpression, StringComparison.InvariantCultureIgnoreCase)).Select(v => v.Code);
                SelectionUtil.AddValues(selection, variableValues);
            }
        }

        public bool CanHandle(string expression)
        {
            return expression.Contains('*');
        }

        /// <summary>
        /// Verifies that the expression * selection expression is valid
        /// </summary>
        /// <param name="expression">The expression selection expression to validate</param>
        /// <returns>True if the expression is valid, else false</returns>
        public bool Verfiy(string expression)
        {
            if (expression.Equals("*"))
            {
                return true;
            }

            int count = expression.Count(c => c == '*');

            if (count > 2)
            {
                // More than 2 * is not allowed
                return false;
            }

            if ((count == 1) && !(expression.StartsWith('*') || expression.EndsWith('*')))
            {
                // * must be in the beginning or end of the value
                return false;
            }

            if ((count == 2) && !(expression.StartsWith('*') && expression.EndsWith('*')))
            {
                // The * must be in the beginning and the end of the value
                return false;
            }

            return true;
        }
    }
}
