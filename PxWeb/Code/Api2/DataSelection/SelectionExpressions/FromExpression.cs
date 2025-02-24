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

        public bool AddToSelection(Variable variable, VariableSelection selection, string expression, out Problem? problem)
        {
            string code = "";
            problem = null;

            if (!ExpressionUtil.GetSingleCode(expression, out code))
            {
                ProblemUtility.IllegalSelectionExpression();
                return false;
            }


            int index1 = ExpressionUtil.GetCodeIndex(variable, code);

            if (index1 == -1)
            {
                ProblemUtility.IllegalSelectionExpression();
                return false;
            }

            var variableValues = variable.Values.Skip(index1).Take(variable.Values.Count - index1).Select(v => v.Code);
            SelectionUtil.AddValues(selection, variableValues);
            return true;
        }

        public bool CanHandle(string expression)
        {
            return expression.StartsWith("FROM(", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Verfiy(string expression, out Problem? problem)
        {
            if (Regex.IsMatch(expression, REGEX_FROM, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
            {
                problem = null;
                return true;
            }

            problem = ProblemUtility.IllegalSelectionExpression();
            return false;
        }
    }
}
