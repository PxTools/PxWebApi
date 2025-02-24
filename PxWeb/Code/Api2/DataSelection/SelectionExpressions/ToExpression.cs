using System.Linq;
using System.Text.RegularExpressions;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public class ToExpression : ISelectionExpression
    {
        // TO(xxx) and to(xxx)
        private static readonly string REGEX_TO = "^(TO\\(([^,]+)\\d*\\))$";

        public bool AddToSelection(Variable variable, VariableSelection selection, string expression, out Problem? problem)
        {
            string code;
            problem = null;
            if (!ExpressionUtil.GetSingleCode(expression, out code))
            {
                problem = ProblemUtility.IllegalSelectionExpression();
                return false;
            }


            int index1 = ExpressionUtil.GetCodeIndex(variable, code);

            if (index1 == -1)
            {
                problem = ProblemUtility.IllegalSelectionExpression();
                return false;
            }

            var variableValues = variable.Values.Take(index1 + 1).Select(v => v.Code);
            SelectionUtil.AddValues(selection, variableValues);

            return true;
        }

        public bool CanHandle(string expression)
        {
            return expression.StartsWith("TO(", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Verfiy(string expression, out Problem? problem)
        {
            if (Regex.IsMatch(expression, REGEX_TO, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
            {
                problem = null;
                return true;
            }
            problem = ProblemUtility.IllegalSelectionExpression();
            return false;
        }
    }
}
