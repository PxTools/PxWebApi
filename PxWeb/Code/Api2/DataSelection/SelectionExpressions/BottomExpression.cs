﻿using System.Linq;
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

        public bool AddToSelection(Variable variable, VariableSelection selection, string expression, out Problem? problem)
        {
            int count;
            int offset = 0;
            problem = null;

            if (!ExpressionUtil.GetCountAndOffset(expression, out count, out offset))
            {
                problem = ProblemUtility.IllegalSelectionExpression();
                return false;
            }

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

            return true;
        }

        public bool CanHandle(string expression)
        {
            return expression.StartsWith("BOTTOM(", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Verfiy(string expression, out Problem? problem)
        {
            if (Regex.IsMatch(expression, REGEX_BOTTOM, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
            {
                problem = null;
                return true;
            }

            problem = ProblemUtility.IllegalSelectionExpression();
            return false;
        }
    }
}
