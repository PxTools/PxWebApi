using PxWeb.Helper.Api2;

namespace PxWeb.UnitTests.Helpers
{
    [TestClass]
    public class OutputParameterUtilTests
    {
        readonly PxApiConfigurationOptions _configOptions = new PxApiConfigurationOptions();

        public OutputParameterUtilTests()
        {
            _configOptions.DefaultOutputFormat = "CSV";
            _configOptions.OutputFormats.Add("xlsx");
        }


        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithNullOutputFormat_ReturnsDefaultOutputFormat()
        {
            // Arrange
            OutputFormatType? outputFormat = null;
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual(_configOptions.DefaultOutputFormat, format);
            Assert.IsFalse(paramError);
            Assert.HasCount(0, formatParams);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithPXOutputFormat_ReturnsPx()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.PxEnum;
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("px", format);
            Assert.IsFalse(paramError);
            Assert.HasCount(0, formatParams);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithCsvOutputFormat_ReturnsCsv()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.CsvEnum;
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("csv", format);
            Assert.IsFalse(paramError);
            Assert.HasCount(0, formatParams);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithCsvOutputFormat_ReturnsDefaultOutputFormat()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.CsvEnum;
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("csv", format);
            Assert.IsFalse(paramError);
            Assert.HasCount(0, formatParams);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithPxAndOutputParmatersOutputFormat_ReturnsParameterError()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.PxEnum;
            List<OutputFormatParamType>? outputFormatParams = new List<OutputFormatParamType>();
            outputFormatParams.Add(OutputFormatParamType.IncludeTitleEnum);
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("px", format);
            Assert.IsTrue(paramError);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWith_Disabled_Parquet_ReturnsParameterError()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.ParquetEnum;
            List<OutputFormatParamType>? outputFormatParams = null;
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);
            // Assert

            Assert.IsTrue(paramError);
        }

        [TestMethod]
        public void TranslateOutputParamters_WhenCalledWithExcelAndOutputParmatersOutputFormat_ReturnsExcel()
        {
            // Arrange
            OutputFormatType? outputFormat = OutputFormatType.XlsxEnum;
            List<OutputFormatParamType>? outputFormatParams = new List<OutputFormatParamType>();
            outputFormatParams.Add(OutputFormatParamType.IncludeTitleEnum);
            bool paramError;
            // Act
            var (format, formatParams) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);
            // Assert
            Assert.AreEqual("xlsx", format);
            Assert.IsFalse(paramError);
            Assert.HasCount(1, formatParams);

        }
    }
}
