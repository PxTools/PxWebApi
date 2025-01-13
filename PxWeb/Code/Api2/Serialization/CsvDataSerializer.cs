using Microsoft.AspNetCore.Http;

using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.Serialization
{
    public class CsvDataSerializer : IDataSerializer
    {

        private readonly CsvSerializer _serializer;

        public CsvDataSerializer()
        {
            _serializer = new CsvSerializer();
        }

        public CsvDataSerializer(List<string> outputFormatParams)
        {
            _serializer = new CsvSerializer();

            foreach (var param in outputFormatParams)
            {

                if (param.Equals("IncludeTitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.IncludeTitle = true;
                }
                else if (param.Equals("UseCodes", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = CsvSerializer.LablePreference.Code;
                }
                else if (param.Equals("UseTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = CsvSerializer.LablePreference.Text;
                }
                else if (param.Equals("UseCodesAndTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = CsvSerializer.LablePreference.BothCodeAndText;
                }
                else if (param.Equals("SeparatorTab", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueDelimiter = CsvSerializer.Delimiters.Tab;
                }
                else if (param.Equals("SeparatorSpace", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueDelimiter = CsvSerializer.Delimiters.Space;

                }
                else if (param.Equals("SeparatorSemicolon", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueDelimiter = CsvSerializer.Delimiters.Semicolon;
                }
            }
        }

        public void Serialize(PXModel model, HttpResponse response)
        {
            response.ContentType = "text/csv; charset=" + EncodingUtil.GetEncoding(model.Meta.CodePage).WebName;
            _serializer.Serialize(model, response.Body);
        }
    }
}
