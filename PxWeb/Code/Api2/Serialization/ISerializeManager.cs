namespace PxWeb.Code.Api2.Serialization
{
    public interface ISerializeManager
    {
        SerializationInformation GetSerializer(string outputFormat, string codePage, List<string> outputFormatParams);
    }
}
