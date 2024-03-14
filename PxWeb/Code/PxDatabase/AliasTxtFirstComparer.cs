namespace PxWeb.Code.PxDatabase
{
    public class AliasTxtFirstComparer : IComparer<string>
    {
        //Puts files ending with "alias.txt" first
        public int Compare(string? x, string? y)
        {
            // Check if x ends with "alias.txt" or "Alias.txt"

            bool xIsAlias = false;
            if (x != null)
            {
                xIsAlias = x.EndsWith("alias.txt", StringComparison.OrdinalIgnoreCase);
            }

            // Check if y ends with "alias.txt" or "Alias.txt"
            bool yIsAlias = false;
            if (y != null)
            {
                yIsAlias = y.EndsWith("alias.txt", StringComparison.OrdinalIgnoreCase);
            }

            // If both or neither are alias, do nothing
            if (xIsAlias == yIsAlias)
            {
                return 0;
            }

            // If only x is alias, it should come first
            if (xIsAlias)
            {
                return -1; // x comes before y
            }

            // If only y is alias, it should come first
            return 1; // x comes after y
        }
    }


}
