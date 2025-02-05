using PxWeb.Code.Api2.ModelBinder;

namespace PxWeb.UnitTests.Helpers
{
    [TestClass]
    public class CommaSeparatedListToListConverterTests
    {
        [TestMethod]
        public void ToList_WhenCalledWithEmptyString_ReturnsEmptyList()
        {
            var result = CommaSeparatedListToListConverter.ToList<string>(string.Empty, x => x);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToList_WhenCalledWithOneValueString_ReturnsOneItemInList()
        {
            var result = CommaSeparatedListToListConverter.ToList<string>("First value", x => x);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [TestMethod]
        public void ToList_WhenCalledWithOneValueWithCommaString_ReturnsOneItemInList()
        {
            var result = CommaSeparatedListToListConverter.ToList<string>("[First, value]", x => x);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void ToList_WhenCalledWithTwoString_ReturnsOneItemInList()
        {
            var result = CommaSeparatedListToListConverter.ToList<string>("[First, value], Second value", x => x);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}
