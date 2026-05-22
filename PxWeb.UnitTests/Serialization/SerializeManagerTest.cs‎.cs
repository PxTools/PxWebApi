using PCAxis.Serializers;

namespace PxWeb.UnitTests.Serialization
{
    [TestClass]
    public class SerializeManagerTest
    {
        [TestMethod]
        public void GetSerializerXlsx_Returns_Correct_ContentType_And_Suffix_For_Xlsx()
        {
            // Arrange
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            string codePage = "utf-8";
            List<string> outputFormatParams = new List<string>();
            // Act
            var result = serializeManager.GetSerializer(outputFormat, codePage, outputFormatParams);
            // Assert
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.AreEqual(".xlsx", result.Suffix);
            Assert.IsFalse(((Xlsx2Serializer)result.Serializer).ExcludeZerosAndMissingValues);
        }

        [TestMethod]
        public void GetSerializerXlsx_Returns_Correct_ContentType_And_Suffix_And_ExcludesZeros_For_Xlsx()
        {
            // Arrange
            var serializeManager = new SerializeManager();
            string outputFormat = "xlsx";
            string codePage = "utf-8";
            List<string> outputFormatParams = new List<string>();
            outputFormatParams.Add("ExcludeZerosAndMissingValues");
            // Act
            var result = serializeManager.GetSerializer(outputFormat, codePage, outputFormatParams);
            // Assert
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.AreEqual(".xlsx", result.Suffix);
            Assert.IsTrue(((Xlsx2Serializer)result.Serializer).ExcludeZerosAndMissingValues);
        }

        [TestMethod]
        public void GetSerializerHtml_Returns_Correct_ContentType_And_Suffix_For_Html()
        {
            // Arrange
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            string codePage = "utf-8";
            List<string> outputFormatParams = new List<string>();
            // Act
            var result = serializeManager.GetSerializer(outputFormat, codePage, outputFormatParams);
            // Assert
            Assert.AreEqual(".html", result.Suffix);
            Assert.IsFalse(((HtmlSerializer)result.Serializer).ExcludeZerosAndMissingValues);
        }

        [TestMethod]
        public void GetSerializerHtml_Returns_Correct_ContentType_And_Suffix_And_ExcludesZeros_For_Html()
        {
            // Arrange
            var serializeManager = new SerializeManager();
            string outputFormat = "html";
            string codePage = "utf-8";
            List<string> outputFormatParams = new List<string>();
            outputFormatParams.Add("ExcludeZerosAndMissingValues");
            // Act
            var result = serializeManager.GetSerializer(outputFormat, codePage, outputFormatParams);
            // Assert
            Assert.AreEqual(".html", result.Suffix);
            Assert.IsTrue(((HtmlSerializer)result.Serializer).ExcludeZerosAndMissingValues);
        }

        [TestMethod]
        public void GetSerializerCsv_Returns_Correct_ContentType_And_Suffix_For_Csv()
        {
            // Arrange
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            string codePage = "utf-8";
            List<string> outputFormatParams = new List<string>();
            // Act
            var result = serializeManager.GetSerializer(outputFormat, codePage, outputFormatParams);
            // Assert
            Assert.AreEqual(".csv", result.Suffix);
            Assert.IsFalse(((CsvSerializer)result.Serializer).ExcludeZerosAndMissingValues);
        }

        [TestMethod]
        public void GetSerializerCsv_Returns_Correct_ContentType_And_Suffix_And_ExcludesZeros_For_Csv()
        {
            // Arrange
            var serializeManager = new SerializeManager();
            string outputFormat = "csv";
            string codePage = "utf-8";
            List<string> outputFormatParams = new List<string>();
            outputFormatParams.Add("ExcludeZerosAndMissingValues");
            // Act
            var result = serializeManager.GetSerializer(outputFormat, codePage, outputFormatParams);
            // Assert
            Assert.AreEqual(".csv", result.Suffix);
            Assert.IsTrue(((CsvSerializer)result.Serializer).ExcludeZerosAndMissingValues);
        }
    }
}
