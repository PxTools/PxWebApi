using Microsoft.AspNetCore.Http;

using PCAxis.Paxiom;
using PCAxis.Serializers;

namespace PxWeb.Code.Api2.Serialization
{
    public class HtmlDataSerializer : IDataSerializer
    {

        private readonly HtmlSerializer _serializer;

        public HtmlDataSerializer()
        {
            _serializer = new HtmlSerializer();
        }

        public HtmlDataSerializer(List<string> outputFormatParams)
        {
            _serializer = new HtmlSerializer();

            foreach (var param in outputFormatParams)
            {

                if (param.Equals("IncludeTitle", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.IncludeTitle = true;
                }
                else if (param.Equals("UseCodes", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = HtmlSerializer.LablePreference.Code;
                }
                else if (param.Equals("UseTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = HtmlSerializer.LablePreference.Text;
                }
                else if (param.Equals("UseCodesAndTexts", StringComparison.InvariantCultureIgnoreCase))
                {
                    _serializer.ValueLablesDisplay = HtmlSerializer.LablePreference.BothCodeAndText;
                }   
            }
        }

        public void Serialize(PXModel model, HttpResponse response)
        {
            response.ContentType = "text/html; charset=" + System.Text.Encoding.GetEncoding(model.Meta.CodePage).WebName;
            _serializer.Serialize(model, response.Body);
        }
    }
}
