using System.Collections.Specialized;

using PxWeb.Code.PxFile;

namespace PxWeb.UnitTests.PxFile
{
    [TestClass]
    public class FileProcessingUtilsTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("\t\r\n")]
        public void ParseStringToList_NullOrWhitespace_ReturnsEmptyCollection(string? input)
        {
            var result = FileProcessingUtils.ParseStringToList(input!);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        [DataRow("  abc  ", "abc")]
        [DataRow("  \"abc\"  ", "abc")]
        [DataRow("\"a\"b\"c\"", "a\"b\"c")]
        [DataRow("\"a,b,c\"", "a,b,c")]
        [DataRow("1", "1")]
        [DataRow("0", "0")]
        [DataRow("1.5", "1.5")]
        [DataRow("-7.2", "-7.2")]
        [DataRow("YES", "YES")]
        [DataRow("NO", "NO")]
        [DataRow("\"ANSI\"", "ANSI")]
        [DataRow("\"utf-8\"", "utf-8")]
        [DataRow("\"20190103 09:00\"", "20190103 09:00")]
        [DataRow("\"1000 t, MWh, ktoe, TJ, %\"", "1000 t, MWh, ktoe, TJ, %")]
        [DataRow("\"Consumption of hard coal (1,000 ton)\"", "Consumption of hard coal (1,000 ton)")]
        public void ParseStringToList_ScalarInputs_ReturnSingleItemWithExpectedContent(string input, string expected)
        {
            var result = FileProcessingUtils.ParseStringToList(input);

            Assert.IsInstanceOfType(result, typeof(StringCollection));
            Assert.HasCount(1, result);
            Assert.AreEqual(expected, result[0]);
        }

        [TestMethod]
        [DataRow("\"a\",\"b\",\"c\"", 3, "a", "b", "c")]
        [DataRow("  \"a\" ,  \"b\"  ,\"c\"   ", 3, "a", "b", "c")]
        [DataRow("\"a,a\",\"b\" ,\"c,c,c\"", 3, "a,a", "b", "c,c,c")]
        [DataRow("\"fi\",\"sv\",\"en\"", 3, "fi", "sv", "en")]
        [DataRow("\"1970\",\"1971\"", 2, "1970", "1971", null)]
        public void ParseStringToList_QuotedCommaSeparatedItems_ParsesIntoExpectedItems(
            string input,
            int expectedCount,
            string expected0,
            string expected1,
            string? expected2)
        {
            var result = FileProcessingUtils.ParseStringToList(input);

            Assert.HasCount(expectedCount, result);
            Assert.AreEqual(expected0, result[0]);
            Assert.AreEqual(expected1, result[1]);

            if (expectedCount > 2)
            {
                Assert.IsNotNull(expected2);
                Assert.AreEqual(expected2, result[2]);
            }
        }

        [TestMethod]
        [DataRow("\"a\",\"\",\"b\"", 2, "a", "b")]
        [DataRow("\"a\",\"   \"", 2, "a", "   ")]
        public void ParseStringToList_QuotedItems_HandlesEmptyOrWhitespaceItems(string input, int expectedCount, string expected0, string? expected1)
        {
            var result = FileProcessingUtils.ParseStringToList(input);

            Assert.HasCount(expectedCount, result);
            Assert.AreEqual(expected0, result[0]);

            if (expectedCount > 1)
            {
                Assert.IsNotNull(expected1);
                Assert.AreEqual(expected1, result[1]);
            }
        }

        [TestMethod]
        public void ParseStringToList_MixedUnquotedAndQuotedCommaSeparatedItems_ParsesIntoExpectedItems()
        {
            var result = FileProcessingUtils.ParseStringToList("TLIST(Q1),\"20221\",\"20222\",\"20223\"");

            Assert.HasCount(4, result);
            Assert.AreEqual("TLIST(Q1)", result[0]);
            Assert.AreEqual("20221", result[1]);
            Assert.AreEqual("20222", result[2]);
            Assert.AreEqual("20223", result[3]);
        }
    }
}
