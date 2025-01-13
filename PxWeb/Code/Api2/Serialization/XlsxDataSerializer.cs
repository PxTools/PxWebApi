using Microsoft.AspNetCore.Http;

using PCAxis.Paxiom;
using PCAxis.Serializers;

namespace PxWeb.Code.Api2.Serialization
{
    public class XlsxDataSerializer : IDataSerializer
    {
        private readonly Xlsx2Serializer _serializer;

        public XlsxDataSerializer()
        {
            _serializer = new Xlsx2Serializer();
        }

        public XlsxDataSerializer(List<string> outputFormatParams)
        {
            _serializer = new Xlsx2Serializer();

            foreach (var param in outputFormatParams)
            {

                if (param.Equals("IncludeTitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.IncludeTitle = true;
                }
                else if (param.Equals("UseCodes", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = Xlsx2Serializer.LablePreference.Code;
                }
                else if (param.Equals("UseTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = Xlsx2Serializer.LablePreference.Text;
                }
                else if (param.Equals("UseCodesAndTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = Xlsx2Serializer.LablePreference.BothCodeAndText;
                }
            }
        }

        public void Serialize(PXModel model, HttpResponse response)
        {
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            IPXModelStreamSerializer serializer = new XlsxSerializer();
            serializer.Serialize(model, response.Body);
        }
    }
}
