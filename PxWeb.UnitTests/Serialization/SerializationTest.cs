using PCAxis.Serializers;

namespace PxWeb.UnitTests.Serialization
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void ShouldSerializeToPx()
        {
            PXModel pxModel = TestFactory.GetPxModel();

            var serializeManager = new SerializeManager();
            string outputFormat = "px";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", new List<string>());

            Assert.IsInstanceOfType(info.Serializer, typeof(PXFileSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("application/octet-stream", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToCsv()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToXlsx()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(Xlsx2Serializer));
            Assert.IsTrue(info.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToJosnStat2()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "json-stat2";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(JsonStat2Serializer));
            Assert.IsTrue(info.ContentType.Equals("application/json; charset=UTF-8", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToHtml()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(HtmlSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/html", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToParquet()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "Parquet";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(ParquetSerializer));
            Assert.IsTrue(info.ContentType.Equals("application/octet-stream", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToJsonPx()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "json-px";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(JsonSerializer));
            Assert.IsTrue(info.ContentType.Equals("application/json; charset=UTF-8", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToDefaultPx()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "this-serializer-does-not-exist";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(PXFileSerializer));
        }

    }
}
