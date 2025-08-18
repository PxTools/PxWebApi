using System;

using PxWeb.Converters;

namespace PxWeb.UnitTests.Helpers
{
    [TestClass]
    public class EnumConverterTests
    {
        [TestMethod]
        public void ConvertFromStringOutputFormatParam_IncludeTile()
        {
            var expected = OutputFormatParamType.IncludeTitleEnum;
            // Act
            var actual = EnumConverter.ToEnum<OutputFormatParamType>("IncludeTitle");
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertFromStringOutputFormatParam_CaseInsensitiveLowerCase()
        {
            var expected = OutputFormatParamType.IncludeTitleEnum;
            // Act
            var actual = EnumConverter.ToEnum<OutputFormatParamType>("includetitle");
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertFromStringOutputFormatParam_CaseInsensitiveUpperCase()
        {
            var expected = OutputFormatParamType.IncludeTitleEnum;
            // Act
            var actual = EnumConverter.ToEnum<OutputFormatParamType>("INCLUDETITLE");
            // Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void ConvertFromStringOutputFormatParam_InvalidValue_ThrowsException()
        {
            // Act
            Assert.ThrowsExactly<InvalidOperationException>(() => EnumConverter.ToEnum<OutputFormatParamType>("X"));

        }

        [TestMethod]
        public void ConvertToStringOutputFormatParam_CaseInsensitiveUpperCase()
        {
            var expected = "IncludeTitle";
            // Act
            var actual = EnumConverter.ToEnumString<OutputFormatParamType>(OutputFormatParamType.IncludeTitleEnum);
            // Assert
            Assert.AreEqual(expected, actual);
        }


    }
}
