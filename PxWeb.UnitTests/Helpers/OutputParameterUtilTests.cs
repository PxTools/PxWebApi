using PxWeb.Helper.Api2;

namespace PxWeb.UnitTests.Helpers
{
    [TestClass]
    public class OutputParameterUtilTests
    {
        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithNullOutputFormat_ReturnsDefaultOutputFormat()
        {
            // Arrange
            OutputFormatType? outputFormat = null;
            string defaultOutputFormat = "CSV";
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, defaultOutputFormat, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual(defaultOutputFormat, format);
            Assert.IsFalse(paramError);
            Assert.AreEqual(0, formatParams.Count);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithPXOutputFormat_ReturnsPx()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.PxEnum;
            string defaultOutputFormat = "CSV";
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, defaultOutputFormat, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("px", format);
            Assert.IsFalse(paramError);
            Assert.AreEqual(0, formatParams.Count);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithCsvOutputFormat_ReturnsCsv()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.CsvEnum;
            string defaultOutputFormat = "CSV";
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, defaultOutputFormat, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("csv", format);
            Assert.IsFalse(paramError);
            Assert.AreEqual(0, formatParams.Count);
        }

        public void TranslateOutputParamters_WhenCalledWithCsvOutputFormat_ReturnsDefaultOutputFormat()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.CsvEnum;
            string defaultOutputFormat = "CSV";
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, defaultOutputFormat, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("csv", format);
            Assert.IsFalse(paramError);
            Assert.AreEqual(0, formatParams.Count);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithPxAndOutputParmatersOutputFormat_ReturnsParameterError()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.PxEnum;
            string defaultOutputFormat = "CSV";
            List<OutputFormatParamType>? outputFormatParams = new List<OutputFormatParamType>();
            outputFormatParams.Add(OutputFormatParamType.IncludeTitleEnum);
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, defaultOutputFormat, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("px", format);
            Assert.IsTrue(paramError);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithExcelAndOutputParmatersOutputFormat_ReturnsExcel()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.XlsxEnum;
            string defaultOutputFormat = "CSV";
            List<OutputFormatParamType>? outputFormatParams = new List<OutputFormatParamType>();
            outputFormatParams.Add(OutputFormatParamType.IncludeTitleEnum);
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, defaultOutputFormat, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("xlsx", format);
            Assert.IsFalse(paramError);
            Assert.AreEqual(1, formatParams.Count);

        }
    }
}
