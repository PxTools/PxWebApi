using System.Linq;
using System.Text.RegularExpressions;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public class BottomExpression : ISelectionExpression
    {
        // BOTTOM(xxx), BOTTOM(xxx,yyy), bottom(xxx) and bottom(xxx,yyy)
        private static readonly string REGEX_BOTTOM = "^(BOTTOM\\([1-9]\\d*\\)|BOTTOM\\([1-9]\\d*,[0-9]\\d*\\))$";

        public void AddToSelection(Variable variable, VariableSelection selection, string expression)
        {
            int count;
            int offset = 0;

            if (!ExpressionUtil.GetCountAndOffset(expression, out count, out offset))
            {
                return; // Something went wrong
            }

            var codes = variable.Values.Select(value => value.Code).ToArray();

            if (variable.IsTime)
            {
                //Time should alway be sorted in ascending order
                var variableValues = variable.Values.Skip(offset).Take(count).Select(v => v.Code);
                SelectionUtil.AddValues(selection, variableValues);
            }
            else
            {
                var variableValues = variable.Values.Select(v => v.Code).Reverse().Skip(offset).Take(count).Reverse();
                SelectionUtil.AddValues(selection, variableValues);
            }
        }

        public bool CanHandle(string expression)
        {
            return expression.StartsWith("BOTTOM(", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Verfiy(string expression)
        {
            return Regex.IsMatch(expression, REGEX_BOTTOM, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
    }
}
