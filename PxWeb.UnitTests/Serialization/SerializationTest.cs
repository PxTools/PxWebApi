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
        public void ShouldSerializeToCsvWithTitle()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["IncludeTitle"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.IsTrue(csv?.IncludeTitle);
        }

        [TestMethod]
        public void ShouldSerializeToCsvWithCodes()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseCodes"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.AreEqual(CsvSerializer.LablePreference.Code, csv?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToCsvWithTexts()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseTexts"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.AreEqual(CsvSerializer.LablePreference.Text, csv?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToCsvWithBothCodesAndText()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseCodesAndTexts"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.AreEqual(CsvSerializer.LablePreference.BothCodeAndText, csv?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToCsvDefaultSeparatorShouldBeComma()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", []);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.AreEqual(CsvSerializer.Delimiters.Comma, csv?.ValueDelimiter);
        }

        [TestMethod]
        public void ShouldSerializeToCsvSeparatorTab()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["SeparatorTab"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.AreEqual(CsvSerializer.Delimiters.Tab, csv?.ValueDelimiter);
        }

        [TestMethod]
        public void ShouldSerializeToCsvSeparatorSpace()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["SeparatorSpace"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.AreEqual(CsvSerializer.Delimiters.Space, csv?.ValueDelimiter);
        }

        [TestMethod]
        public void ShouldSerializeToCsvSeparatorSemicolon()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["SeparatorSemicolon"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(CsvSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/csv", System.StringComparison.InvariantCultureIgnoreCase));
            CsvSerializer? csv = info.Serializer as CsvSerializer;
            Assert.AreEqual(CsvSerializer.Delimiters.Semicolon, csv?.ValueDelimiter);
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
        public void ShouldSerializeToXlsxWithTitle()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["IncludeTitle"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(Xlsx2Serializer));
            Assert.IsTrue(info.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.StringComparison.InvariantCultureIgnoreCase));
            Xlsx2Serializer? csv = info.Serializer as Xlsx2Serializer;
            Assert.IsTrue(csv?.IncludeTitle);
        }

        [TestMethod]
        public void ShouldSerializeToXlsxWithCodes()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseCodes"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(Xlsx2Serializer));
            Assert.IsTrue(info.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.StringComparison.InvariantCultureIgnoreCase));
            Xlsx2Serializer? xlsx = info.Serializer as Xlsx2Serializer;
            Assert.AreEqual(Xlsx2Serializer.LablePreference.Code, xlsx?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToXlsxWithTexts()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseTexts"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(Xlsx2Serializer));
            Assert.IsTrue(info.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.StringComparison.InvariantCultureIgnoreCase));
            Xlsx2Serializer? xlsx = info.Serializer as Xlsx2Serializer;
            Assert.AreEqual(Xlsx2Serializer.LablePreference.Text, xlsx?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToXlsxWithBothCodeAndTexts()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseCodesAndTexts"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(Xlsx2Serializer));
            Assert.IsTrue(info.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", System.StringComparison.InvariantCultureIgnoreCase));
            Xlsx2Serializer? xlsx = info.Serializer as Xlsx2Serializer;
            Assert.AreEqual(Xlsx2Serializer.LablePreference.BothCodeAndText, xlsx?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToJsonStat2()
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
        public void ShouldSerializeToHtmlWithTitle()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["IncludeTitle"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(HtmlSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/html", System.StringComparison.InvariantCultureIgnoreCase));

            HtmlSerializer? html = info.Serializer as HtmlSerializer;
            Assert.IsTrue(html?.IncludeTitle);
        }

        [TestMethod]
        public void ShouldSerializeToHtmlWithCodes()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseCodes"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(HtmlSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/html", System.StringComparison.InvariantCultureIgnoreCase));

            HtmlSerializer? html = info.Serializer as HtmlSerializer;
            Assert.AreEqual(HtmlSerializer.LablePreference.Code, html?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToHtmlWithTexts()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseTexts"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(HtmlSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/html", System.StringComparison.InvariantCultureIgnoreCase));

            HtmlSerializer? html = info.Serializer as HtmlSerializer;
            Assert.AreEqual(HtmlSerializer.LablePreference.Text, html?.ValueLablesDisplay);
        }

        [TestMethod]
        public void ShouldSerializeToHtmlWithBothCodesAndTexts()
        {
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            var info = serializeManager.GetSerializer(outputFormat, "ISO-8859-1", ["UseCodesAndTexts"]);

            Assert.IsInstanceOfType(info.Serializer, typeof(HtmlSerializer));
            Assert.IsTrue(info.ContentType.StartsWith("text/html", System.StringComparison.InvariantCultureIgnoreCase));

            HtmlSerializer? html = info.Serializer as HtmlSerializer;
            Assert.AreEqual(HtmlSerializer.LablePreference.BothCodeAndText, html?.ValueLablesDisplay);
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
