using PCAxis.Paxiom;
using PCAxis.Serializers;

namespace PxWeb.Code.Api2.Serialization
{
    public class SerializeManager : ISerializeManager
    {
        public SerializationInformation GetSerializer(string outputFormat, string codePage, List<string> outputFormatParams)
        {
            string contentType;
            string suffix;
            IPXModelStreamSerializer serializer;


            switch (outputFormat.ToLower())
            {
                case "xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    suffix = ".xlsx";
                    serializer = CreateXlsxSerializer(outputFormatParams);
                    break;
                case "csv":
                    contentType = "text/csv; charset=" + EncodingUtil.GetEncoding(codePage).WebName;
                    suffix = ".csv";
                    serializer = CreateCsvSerializer(outputFormatParams);
                    break;
                case "json-px":
                    contentType = "application/json; charset=UTF-8";
                    suffix = ".json";
                    serializer = new JsonSerializer();
                    break;
                case "json-stat2":
                    contentType = "application/json; charset=UTF-8";
                    suffix = ".json";
                    serializer = new JsonStat2Serializer();
                    break;
                case "parquet":
                    contentType = "application/octet-stream";
                    suffix = ".parquet";
                    serializer = new ParquetSerializer();
                    break;
                case "html":
                    contentType = "text/html; charset=" + System.Text.Encoding.GetEncoding(codePage).WebName;
                    suffix = ".html";
                    serializer = CreateHtmlSerializer(outputFormatParams);
                    break;
                case "px":
                    contentType = "application/octet-stream; charset=" + EncodingUtil.GetEncoding(codePage).WebName;
                    suffix = ".px";
                    serializer = new PXFileSerializer();
                    break;
                default:
                    contentType = "application/octet-stream; charset=" + EncodingUtil.GetEncoding(codePage).WebName;
                    suffix = ".px";
                    serializer = new PXFileSerializer();
                    break;
            }

            return new SerializationInformation(contentType, suffix, serializer);
        }

        private CsvSerializer CreateCsvSerializer(List<string> outputFormatParams)
        {
            var serializer = new CsvSerializer();

            foreach (var param in outputFormatParams)
            {

                if (param.Equals("IncludeTitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.IncludeTitle = true;
                }
                else if (param.Equals("UseCodes", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = CsvSerializer.LablePreference.Code;
                }
                else if (param.Equals("UseTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = CsvSerializer.LablePreference.Text;
                }
                else if (param.Equals("UseCodesAndTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = CsvSerializer.LablePreference.BothCodeAndText;
                }
                else if (param.Equals("SeparatorTab", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueDelimiter = CsvSerializer.Delimiters.Tab;
                }
                else if (param.Equals("SeparatorSpace", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueDelimiter = CsvSerializer.Delimiters.Space;

                }
                else if (param.Equals("SeparatorSemicolon", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueDelimiter = CsvSerializer.Delimiters.Semicolon;
                }
            }

            return serializer;
        }

        private Xlsx2Serializer CreateXlsxSerializer(List<string> outputFormatParams)
        {
            var serializer = new Xlsx2Serializer();

            foreach (var param in outputFormatParams)
            {
                if (param.Equals("IncludeTitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.IncludeTitle = true;
                }
                else if (param.Equals("UseCodes", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = Xlsx2Serializer.LablePreference.Code;
                }
                else if (param.Equals("UseTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = Xlsx2Serializer.LablePreference.Text;
                }
                else if (param.Equals("UseCodesAndTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = Xlsx2Serializer.LablePreference.BothCodeAndText;
                }
            }

            return serializer;
        }

        private HtmlSerializer CreateHtmlSerializer(List<string> outputFormatParams)
        {
            var serializer = new HtmlSerializer();

            foreach (var param in outputFormatParams)
            {
                if (param.Equals("IncludeTitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.IncludeTitle = true;
                }
                else if (param.Equals("UseCodes", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = HtmlSerializer.LablePreference.Code;
                }
                else if (param.Equals("UseTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = HtmlSerializer.LablePreference.Text;
                }
                else if (param.Equals("UseCodesAndTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    serializer.ValueLablesDisplay = HtmlSerializer.LablePreference.BothCodeAndText;
                }
            }

            return serializer;
        }
    }
}
