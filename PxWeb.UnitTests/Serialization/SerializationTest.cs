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
            var serializer = serializeManager.GetSerializer(outputFormat, new List<string>());

            Assert.AreEqual(serializer.GetType().Name, "PxDataSerializer");
        }

        [TestMethod, Ignore]
        public void ShouldSerializeToPxMime()
        {
            PXModel pxModel = TestFactory.GetMinimalModel();

            var serializeManager = new SerializeManager();
            string outputFormat = "px";
            var serializer = serializeManager.GetSerializer(outputFormat, new List<string>());

            var response = new Mock<HttpResponse>();
            response.Setup(x => x.StatusCode).Returns(1);
            HeaderDictionary headers = new HeaderDictionary();

            response.Setup(x => x.Headers).Returns(headers);

            response.Setup(x => x.Body).Returns(Stream.Null);

            string contentType = "";
            response.SetupProperty(x => x.ContentType, contentType);

            

            serializer.Serialize(pxModel, response.Object);

            Assert.IsTrue(contentType.StartsWith("application/octet-stream", System.StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public void ShouldSerializeToCsv()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var serializer = serializeManager.GetSerializer(outputFormat, []);

            Assert.AreEqual("CsvDataSerializer", serializer.GetType().Name);
        }

        [TestMethod]
        public void ShouldSerializeToXlsx()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            var serializer = serializeManager.GetSerializer(outputFormat, []);

            Assert.AreEqual("XlsxDataSerializer", serializer.GetType().Name);
        }

        [TestMethod]
        public void ShouldSerializeToJosnStat2()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "json-stat2";
            var serializer = serializeManager.GetSerializer(outputFormat, []);

            Assert.AreEqual("JsonStat2DataSerializer", serializer.GetType().Name);
        }

        [TestMethod]
        public void ShouldSerializeToHtml()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            var serializer = serializeManager.GetSerializer(outputFormat, []);

            Assert.AreEqual("HtmlDataSerializer", serializer.GetType().Name);
        }

        [TestMethod]
        public void ShouldSerializeToParquet()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "Parquet";
            var serializer = serializeManager.GetSerializer(outputFormat, []);

            Assert.AreEqual("ParquetSerializer", serializer.GetType().Name);
        }

        [TestMethod]
        public void ShouldSerializeToJsonPx()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "json-px";
            var serializer = serializeManager.GetSerializer(outputFormat, []);

            Assert.AreEqual("PxJsonDataSerializer", serializer.GetType().Name);
        }

        [TestMethod]
        public void ShouldSerializeToDefaultPx()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "this-serializer-does-not-exist";
            var serializer = serializeManager.GetSerializer(outputFormat, []);

            Assert.AreEqual("PxDataSerializer", serializer.GetType().Name);
        }

    }
}
