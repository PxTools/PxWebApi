using System.Linq;
using System.Text.RegularExpressions;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public class QuestionmarkExpression : ISelectionExpression
    {
        public void AddToSelection(Variable variable, VariableSelection selection, string expression)
        {
            string regexPattern = string.Concat("^", Regex.Escape(expression).Replace("\\?", "."), "$");
            var variableValues = variable.Values.Where(v => Regex.IsMatch(v.Code, regexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100))).Select(v => v.Code);
            SelectionUtil.AddValues(selection, variableValues);
        }

        public bool CanHandle(string expression)
        {
            return expression.Contains('?');
        }

        public bool Verfiy(string expression)
        {
            return expression.Contains('?');
        }
    }
}
