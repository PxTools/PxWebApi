using System;

using PxWeb.Code.PxFile;

namespace PxWeb.UnitTests.PxFile
{
    [TestClass]
    public class MetaEntryBuilderTests
    {
        private readonly MetaEntryBuilder _builder = new();

        [TestMethod]
        [DataRow("CHARSET", "CHARSET", null, null)]
        [DataRow("SUBJECT-AREA[sv]", "SUBJECT-AREA", "sv", null)]
        [DataRow("VALUES(\"Vuosi\")", "VALUES", null, "\"Vuosi\"")]
        [DataRow("VALUES[sv](\"År\")", "VALUES", "sv", "\"År\"")]
        [DataRow("PRECISION(\"Tiedot\",\"Kivihiilen kulutus, vuosimuutos (%)\")", "PRECISION", null, "\"Tiedot\",\"Kivihiilen kulutus, vuosimuutos (%)\"")]
        [DataRow("VALUENOTE[en](\"Year\",\"2023*\")", "VALUENOTE", "en", "\"Year\",\"2023*\"")]
        [DataRow("CELLNOTE[en](\"2023*\", \"Additional info\", \"*\")", "CELLNOTE", "en", "\"2023*\", \"Additional info\", \"*\"")]
        public void Parse_ValidKeys_ReturnsExpectedParts(string input, string expectedKeyword, string? expectedLang, string? expectedSubKey)
        {
            var result = _builder.Parse(input);

            Assert.AreEqual(expectedKeyword, result.KeyWord);
            Assert.AreEqual(expectedLang, result.Lang);
            Assert.AreEqual(expectedSubKey, result.SubKey);
        }

        [TestMethod]
        public void Parse_TrimsWhitespaceAroundKeyParts()
        {
            var result = _builder.Parse("SUBJECT-AREA[sv]  ");

            Assert.AreEqual("SUBJECT-AREA", result.KeyWord);
            Assert.AreEqual("sv", result.Lang);
            Assert.IsNull(result.SubKey);
        }

        [TestMethod]
        [DataRow("SUBJECT-AREA[]")]
        [DataRow("SUBJECT-AREA[")]
        [DataRow("SUBJECT-AREA[sv")]
        [DataRow("SUBJECT-AREA[sv]]")]
        public void Parse_InvalidLanguage_ThrowsArgumentException(string input)
        {
            Assert.ThrowsExactly<ArgumentException>(() => _builder.Parse(input));
        }

        [TestMethod]
        [DataRow("VALUES(")]
        [DataRow("VALUES)")]
        [DataRow("VALUES(\"Vuosi\"")]
        public void Parse_InvalidSpecifier_ThrowsArgumentException(string input)
        {
            Assert.ThrowsExactly<ArgumentException>(() => _builder.Parse(input));
        }

        [TestMethod]
        [DataRow("SUBJECT AREA")]
        [DataRow("SUBJECT-AREA\"")]
        [DataRow("SUBJECT-AREA,")]
        [DataRow("SUBJECT-AREA\n")]
        public void Parse_IllegalKeyNameTokens_ThrowsArgumentException(string input)
        {
            Assert.ThrowsExactly<ArgumentException>(() => _builder.Parse(input));
        }

        [TestMethod]
        public void Parse_EmptyString_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => _builder.Parse(string.Empty));
        }

        [TestMethod]
        public void Parse_NullString_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => _builder.Parse(null!));
        }
    }
}
