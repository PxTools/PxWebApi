namespace PxWeb.Code.Api2.Serialization
{
    public class SerializeManager : ISerializeManager
    {
        public IDataSerializer GetSerializer(string outputFormat, List<string> outputFormatParams)
        {
            switch (outputFormat.ToLower())
            {
                case "xlsx":
                    return new XlsxDataSerializer(outputFormatParams);
                case "csv":
                    return new CsvDataSerializer(outputFormatParams);
                case "json-px":
                    return new PxJsonDataSerializer();
                case "json-stat2":
                    return new JsonStat2DataSerializer();
                case "parquet":
                    return new ParquetSerializer();
                case "html":
                    return new HtmlDataSerializer(outputFormatParams);
                case "px":
                    return new PxDataSerializer();
                default:
                    return new PxDataSerializer();
            }
        }
    }
}
