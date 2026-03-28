using System.Collections.Specialized;

namespace PxWeb.Code.PxFile
{
    public static class FileProcessingUtils
    {
        /// <summary>
        /// Processes a string and produces a list of strings.
        /// If the string contains comma-separated items wrapped in quotes, it returns a list of those items.
        /// Otherwise, it returns a list with a single item.
        /// All quotation marks are removed from the result.
        /// </summary>
        /// <param name="input">The input string to process</param>
        /// <returns>A list of strings with quotation marks removed</returns>
        internal static StringCollection ParseStringToList(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return [];
            }

            string trimmed = input.Trim();
            var result = new StringCollection();
            var items = new List<string>();
            bool inQuotes = false;
            int start = 0;

            for (int i = 0; i < trimmed.Length; i++)
            {
                char c = trimmed[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    // Found a comma outside of quotes - this is a separator
                    string item = trimmed[start..i].Trim().Trim('"');
                    if (!string.IsNullOrEmpty(item))
                    {
                        items.Add(item);
                    }
                    start = i + 1;
                }
            }

            string lastItem = trimmed[start..].Trim().Trim('"');
            if (!string.IsNullOrEmpty(lastItem))
            {
                items.Add(lastItem);
            }

            result.AddRange([.. items]);

            return result;
        }
    }
}
