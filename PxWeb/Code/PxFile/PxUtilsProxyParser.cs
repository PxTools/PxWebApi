using System.Collections.Specialized;
using System.IO;
using System.Text;
using PCAxis.Paxiom;
using Px.Utils.ModelBuilders;
using Px.Utils.Models.Metadata;
using Px.Utils.PxFile.Metadata;

using PxWeb.Code.PxFile;

namespace PxWeb.PxFile
{
    public class PxUtilsProxyParser : IPXModelParser
    {
        private readonly string _filePath;
        public PxUtilsProxyParser(string filePath)
        {
            _filePath = filePath;
        }

        public void ParseData(IPXModelParser.DataHandler handler, int preferredBufferSize)
        {
            throw new NotImplementedException();
        }

        public void ParseMeta(IPXModelParser.MetaHandler handler, string preferredLanguage)
        {
            using Stream fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            PxFileMetadataReader reader = new();
            Encoding encoding = reader.GetEncoding(fileStream);
            fileStream.Position = 0;
            IEnumerable<KeyValuePair<string, string>> entries = reader.ReadMetadata(fileStream, encoding);


            var entryBuilder = new MetaEntryBuilder();
            foreach (KeyValuePair<string, string> entry in entries)
            {
                var entryKey = entryBuilder.Parse(entry.Key);
                var values = ParseStringToList(entry.Value);
                var subkey = entryKey.SubKey == null ? "" : entryKey.SubKey.Trim('"');
                handler(entryKey.KeyWord, entryKey.Lang, subkey, values);
            }
        }

        /// <summary>
        /// Processes a string and produces a list of strings.
        /// If the string contains comma-separated items wrapped in quotes, it returns a list of those items.
        /// Otherwise, it returns a list with a single item.
        /// All quotation marks are removed from the result.
        /// </summary>
        /// <param name="input">The input string to process</param>
        /// <returns>A list of strings with quotation marks removed</returns>
        private static StringCollection ParseStringToList(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return [];
            }

            string trimmed = input.Trim();
            var result = new StringCollection();

            // Check if the string contains quoted values (starts and ends with quotes and has commas)
            if (trimmed.StartsWith('"') && trimmed.Contains(','))
            {
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
                        string item = trimmed.Substring(start, i - start).Trim().Trim('"');
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
            }
            else
            {
                // Single item - remove any surrounding quotes
                result.Add(trimmed.Trim('"'));
            }

            return result;
        }
    }

}
