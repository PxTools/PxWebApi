﻿using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using PCAxis.Paxiom;

namespace PxWeb.Code.Api2.Serialization
{
    public class CsvSemicolonHeadDataSerializer : IDataSerializer
    {
        public void Serialize(PXModel model, HttpResponse response)
        {
            response.ContentType = "text/csv; charset=" + EncodingUtil.GetEncoding(model.Meta.CodePage).WebName;
            IPXModelStreamSerializer serializer = new CsvFileSerializer();
            ((CsvFileSerializer)serializer).Title = true;
            ((CsvFileSerializer)serializer).Delimiter = ';';
            serializer.Serialize(model, response.Body);
        }
    }
}
