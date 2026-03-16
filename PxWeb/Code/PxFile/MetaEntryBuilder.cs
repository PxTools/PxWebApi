namespace PxWeb.Code.PxFile
{
    /// <summary>
    /// Class for building <see cref="MetadataEntryKey"/> records from strings.
    /// </summary>
    public class MetaEntryBuilder
    {
        public struct EntryKeyParseResult(string keyWord, string? lang, string? subKey)
        {
            public string KeyWord { get; set; } = keyWord;
            public string? Lang { get; set; } = lang;
            public string? SubKey { get; set; } = subKey;
        }

        private const char _langParamStart = '[';
        private const char _langParamEnd = ']';

        private const char _spesifierParamStart = '(';
        private const char _spesifierParamEnd = ')';

        private const char _stringDelimeter = '"';
        private const char _listSeparator = ',';
        private readonly char[] _illegalKeyTokens =
            [
                _langParamEnd,
                _spesifierParamEnd,
                _stringDelimeter,
                _listSeparator,
                ' ', '\r', '\n', '\t'
            ];

        /// <summary>
        /// Parses a string into a <see cref="MetadataEntryKey"/> record.
        /// This method extracts the key name, language, and specifiers from the input string and constructs the record.
        /// It throws an exception if the input string is not in the correct format or contains illegal characters.
        /// </summary>
        /// <param name="key">The string to parse into a <see cref="MetadataEntryKey"/> record.</param>
        /// <returns>A <see cref="MetadataEntryKey"/> record constructed from the input string.</returns>
        public EntryKeyParseResult Parse(string key)
        {
            string name = ParseKeyName(ref key);
            string? lang = ParseLang(ref key);

            string? specifiers = ParseSpecifier(key);
            return new EntryKeyParseResult(name, lang, specifiers);
        }

        private string ParseKeyName(ref string remaining)
        {
            if (string.IsNullOrEmpty(remaining)) throw new ArgumentException("Input string is null or empty");
            char[] sectionStartTokens = [_langParamStart, _spesifierParamStart];
            string output = new(remaining);
            int index = remaining.IndexOfAny(sectionStartTokens);
            if (index < 0)
            {
                remaining = string.Empty;
            }
            else if (index > 0)
            {
                output = remaining[..index];
                remaining = remaining[index..];
            }
            else // 0 -> There is no legal key at the start of the string
            {
                throw new ArgumentException($"Can not parse key name from string {remaining}");
            }

            if (output.IndexOfAny(_illegalKeyTokens) >= 0) throw new ArgumentException($"Illegal key name {output}");

            return output.Trim();
        }

        private string? ParseLang(ref string remaining)
        {
            remaining = remaining.TrimStart();
            if (string.IsNullOrEmpty(remaining) || !remaining.StartsWith(_langParamStart))
            {
                return null;
            }
            else
            {
                int index = remaining.IndexOf(_langParamEnd);
                if (index > 1) // 1 because empty lang string is not valid
                {
                    string output = remaining[1..index].Trim();
                    remaining = remaining[(index + 1)..];
                    if (output.Contains(_langParamStart))
                    {
                        throw new ArgumentException($"Language identifier {output} contains invalid symbols");
                    }
                    return output;
                }
                else // <= 0, end symbol not found
                {
                    throw new ArgumentException($"Can not parse language identifier from string {remaining}");
                }
            }
        }

        private static string? ParseSpecifier(string remaining)
        {
            // input: ("foo", "bar") -> output: "foo", "bar"
            remaining = remaining.Trim();
            if (remaining.Length == 0) return null;
            else if (remaining.Length > 4 &&
                remaining[0] == _spesifierParamStart &&
                remaining[^1] == _spesifierParamEnd)
            {
                return remaining[1..^1].Trim();
            }
            else
            {
                throw new ArgumentException($"Unable to parse key specifier from input {remaining}");
            }
        }
    }
}
