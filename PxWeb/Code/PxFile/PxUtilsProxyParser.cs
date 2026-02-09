using System.IO;
using System.Text;

using PCAxis.Paxiom;

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
            using Stream fileStream = OpenFileStream();
            PxFileMetadataReader reader = new();
            Encoding encoding = reader.GetEncoding(fileStream);

            if (fileStream.CanSeek) fileStream.Position = 0;
            else throw new InvalidOperationException("The provided stream does not support seeking, which is required for reading metadata.");

            IEnumerable<KeyValuePair<string, string>> entries = reader.ReadMetadata(fileStream, encoding);

            var entryBuilder = new MetaEntryBuilder();
            foreach (KeyValuePair<string, string> entry in entries)
            {
                var entryKey = entryBuilder.Parse(entry.Key);
                var values = FileProcessingUtils.ParseStringToList(entry.Value);
                // The handler expects the subkey in  format: first", "second", "third, so we need to trim the quotes and keep the separator for the handler to work correctly.
                var subkey = entryKey.SubKey == null ? "" : entryKey.SubKey.Trim('"');
                handler(entryKey.KeyWord, entryKey.Lang, subkey, values);
            }
        }

        internal virtual Stream OpenFileStream()
        {
            return new FileStream(_filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
