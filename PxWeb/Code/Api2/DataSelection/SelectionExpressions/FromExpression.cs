using System.Linq;
using System.Text.RegularExpressions;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public class FromExpression : ISelectionExpression
    {
        // FROM(xxx) and from(xxx)
        private static readonly string REGEX_FROM = "^(FROM\\(([^,]+)\\d*\\))$";

        public void AddToSelection(Variable variable, VariableSelection selection, string expression)
        {
            string code = "";

            if (!ExpressionUtil.GetSingleCode(expression, out code))
            {
                return; // Something went wrong
            }


            int index1 = ExpressionUtil.GetCodeIndex(variable, code);

            if (index1 == -1)
            {
                return; //TODO: Something went wrong no matching value
            }

            var variableValues = variable.Values.Skip(index1).Take(variable.Values.Count - index1).Select(v => v.Code);
            SelectionUtil.AddValues(selection, variableValues);
        }

        public bool CanHandle(string expression)
        {
            return expression.StartsWith("FROM(", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Verfiy(string expression)
        {
            return Regex.IsMatch(expression, REGEX_FROM, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
    }
}
