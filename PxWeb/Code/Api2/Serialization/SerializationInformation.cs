using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.Serialization
{
    public class SerializationInformation
    {
        public string ContentType { get; set; }
        public string Suffix { get; set; }
        public IPXModelStreamSerializer Serializer { get; set; }

        public SerializationInformation(string contentType, string suffix, IPXModelStreamSerializer serializer)
        {
            ContentType = contentType;
            Suffix = suffix;
            Serializer = serializer;
        }
    }
}
