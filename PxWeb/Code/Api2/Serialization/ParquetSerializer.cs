using Microsoft.AspNetCore.Http;

using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.Serialization
{
    public class ParquetSerializer : IDataSerializer
    {
        public void Serialize(PXModel model, HttpResponse response)
        {
            var matrix = model.Meta.Matrix ?? "data";

            response.ContentType = "application/octet-stream";
            response.Headers.Append("Content-Disposition", $"attachment; filename=\"{matrix}.parquet\"");
            IPXModelStreamSerializer serializer = new PCAxis.Serializers.ParquetSerializer();
            serializer.Serialize(model, response.Body);
        }
    }
}
