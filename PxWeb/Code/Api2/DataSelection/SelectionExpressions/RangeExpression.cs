using System.Text.RegularExpressions;

using PCAxis.Paxiom;

using PxWeb.Api2.Server.Models;
using PxWeb.Helper.Api2;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public class RangeExpression : ISelectionExpression
    {
        // RANGE(xxx,yyy) and range(xxx,yyy)
        private static readonly string REGEX_RANGE = "^(RANGE\\(([^,]+)\\d*,([^,)]+)\\d*\\))$";

        public bool CanHandle(string expression)
        {
            return expression.StartsWith("RANGE(", System.StringComparison.InvariantCultureIgnoreCase);
        }
        public bool Verfiy(string expression)
        {
            return Regex.IsMatch(expression, REGEX_RANGE, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
        public void AddToSelection(Variable variable, VariableSelection selection, string expression)
        {
            string code1;
            string code2;

            if (!GetRangeCodes(expression, out code1, out code2))
            {
                return; //TODO: Something went wrong
            }

            int index1 = ExpressionUtil.GetCodeIndex(variable, code1);
            int index2 = ExpressionUtil.GetCodeIndex(variable, code2);
            int indexTemp;

            if (index1 == -1 || index2 == -1)
            {
                //TODO: Something went wrong no matching value
                return;
            }

            if (index1 > index2)
            {
                // Handle indexes in wrong order
                indexTemp = index1;
                index1 = index2;
                index2 = indexTemp;
            }


            for (int i = index1; i <= index2; i++)
            {
                SelectionUtil.AddValue(selection, variable.Values[i].Code);
            }

        }

        /// <summary>
        /// Extracts code1 and code2 from RANGE selection expressions like RANGE(xxx,yyy)
        /// </summary>
        /// <param name="expression">The Range selection expression to extract codes from</param>
        /// <param name="code1">The firts code</param>
        /// <param name="code2">The second code</param>
        /// <returns>True if the codes could be extracted, false if something went wrong</returns>
        private static bool GetRangeCodes(string expression, out string code1, out string code2)
        {
            code1 = "";
            code2 = "";

            try
            {
                int firstParanteses = expression.IndexOf('(');

                if (firstParanteses == -1)
                {
                    return false;
                }

                string strCodes = expression.Substring(firstParanteses + 1, expression.Length - (firstParanteses + 2)); // extract the codes
                string[] codes = strCodes.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

                if (codes.Length != 2)
                {
                    return false;
                }

                code1 = codes[0];
                code2 = codes[1];

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
