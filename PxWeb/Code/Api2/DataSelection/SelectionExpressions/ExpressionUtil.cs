using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.DataSelection.SelectionExpressions
{
    public static class ExpressionUtil
    {
        /// <summary>
        /// Extracts the count and offset from selection expressions like TOP(count), TOP(count,offset), BOTTOM(count), BOTTOM(count,offset)
        /// </summary>
        /// <param name="expression">The selection expression to extract count and offset from</param>
        /// <param name="count">Set to the count value if it could be extracted, else 0</param>
        /// <param name="offset">Set to the offset value if it could be extracted, else 0</param>
        /// <returns>True if values could be extracted, false if something went wrong</returns>
        public static bool GetCountAndOffset(string expression, out int count, out int offset)
        {
            count = 0;
            offset = 0;

            try
            {
                int firstParanteses = expression.IndexOf('(');

                if (firstParanteses == -1)
                {
                    return false;
                }

                string strNumbers = expression.Substring(firstParanteses + 1, expression.Length - (firstParanteses + 2)); // extract the numbers part of TOP(xxx) or TOP(xxx,yyy)
                string[] numbers = strNumbers.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

                if (!int.TryParse(numbers[0], out count))
                {
                    return false; // Something went wrong
                }

                if (numbers.Length == 2)
                {
                    if (!int.TryParse(numbers[1], out offset))
                    {
                        return false; // Something went wrong
                    }
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts the code from selection expressions like FROM(xxx) or TO(xxx)
        /// </summary>
        /// <param name="expression">The Range selection expression to extract the code from</param>
        /// <param name="code">The code</param>
        /// <returns>True if teh code could be extracted, false if something went wrong</returns>
        public static bool GetSingleCode(string expression, out string code)
        {
            code = "";

            try
            {
                int firstParanteses = expression.IndexOf('(');

                if (firstParanteses == -1)
                {
                    return false;
                }

                code = expression.Substring(firstParanteses + 1, expression.Length - (firstParanteses + 2)); // extract the code

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Find index of code in code array.
        /// First tries to find code as specified. If it is not found the method tries to find the code in a case insensitive way.
        /// </summary>
        /// <param name="codes">Array of codes</param>
        /// <param name="code">Code to find index for</param>
        /// <returns>Index of the specified code within the codes array. If not found -1 is returned.</returns>
        public static int GetCodeIndex(Variable variable, string code)
        {
            // Try to get the value using the code specified by the API user
            int index = variable.Values.FindIndex(x => x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase));

            return index;
        }


    }
}
