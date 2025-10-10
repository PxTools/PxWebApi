using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

using PCAxis.Paxiom;

using Px.Utils.ModelBuilders;
using Px.Utils.Models.Metadata;
using Px.Utils.PxFile.Metadata;

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
            IEnumerable<KeyValuePair<string, string>> entries = reader.ReadMetadata(fileStream, encoding);

            fileStream.Position = 0;

            MetadataEntryKeyBuilder entryKeyBuilder = new();
            Dictionary<MetadataEntryKey, string> entries2 = [];
            foreach (KeyValuePair<string, string> entry in entries)
            {
                MetadataEntryKey entryKey = entryKeyBuilder.Parse(entry.Key);
                var values = ParseStringToList(entry.Value);
                handler(entryKey.KeyWord, entryKey.Language, $"{entryKey.FirstIdentifier}", values);
                entries2[entryKey] = entry.Value;
            }
        }

        private static StringCollection GetValues(string valuesString)
        {
            string[] values = valuesString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => s.Trim('"')).ToArray();
            StringCollection collection = new();
            collection.AddRange(values);
            return collection;
        }

        /// <summary>
        /// Processes a string and produces a list of strings.
        /// If the string contains comma-separated items wrapped in quotes, it returns a list of those items.
        /// Otherwise, it returns a list with a single item.
        /// All quotation marks are removed from the result.
        /// </summary>
        /// <param name="input">The input string to process</param>
        /// <returns>A list of strings with quotation marks removed</returns>
        public static StringCollection ParseStringToList(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new StringCollection();
            }
            string trimmed = input.Trim();
            // Check if the string is wrapped in quotes and contains commas
            if (trimmed.StartsWith('"') && trimmed.EndsWith('"') && trimmed.Contains(','))
            {
                // Remove outer quotes
                string content = trimmed[1..^1];
                // Split by comma and process each item
                var c = new StringCollection();
                c.AddRange(content.Split(',')
                    .Select(item => item.Trim().Trim('"'))
                    .Where(item => !string.IsNullOrEmpty(item))
                    .ToArray());
                return c;
            }
            else
            {
                // Single item - remove any surrounding quotes
                return new StringCollection { trimmed.Trim('"') };
            }
        }
    }

}
