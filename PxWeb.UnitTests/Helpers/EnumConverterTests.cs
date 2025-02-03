using System;

using PxWeb.Converters;

namespace PxWeb.UnitTests.Helpers
{
    [TestClass]
    public class EnumConverterTests
    {
        [TestMethod]
        public void OutputFormatParam_IncludeTile()
        {
            var expected = OutputFormatParamType.IncludeTitleEnum;
            // Act
            var actual = EnumConverter.ToEnum<OutputFormatParamType>("IncludeTitle");
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OutputFormatParam_CaseInsensitiveLowerCase()
        {
            var expected = OutputFormatParamType.IncludeTitleEnum;
            // Act
            var actual = EnumConverter.ToEnum<OutputFormatParamType>("includetitle");
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OutputFormatParam_CaseInsensitiveUpperCase()
        {
            var expected = OutputFormatParamType.IncludeTitleEnum;
            // Act
            var actual = EnumConverter.ToEnum<OutputFormatParamType>("INCLUDETITLE");
            // Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OutputFormatParam_InvalidValue_ThrowsException()
        {
            // Act
            EnumConverter.ToEnum<OutputFormatParamType>("X");
        }


    }
}
