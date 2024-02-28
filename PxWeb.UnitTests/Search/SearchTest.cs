namespace PxWeb.UnitTests.Search
{
    [TestClass]
    public class SearchTest
    {
        [TestMethod]
        public void ShouldReturnFirstTimeValue()
        {
            PXModel pxModel = TestFactory.GetPxModel();
            Assert.AreEqual("2018M01", pxModel.Meta.GetFirstTimeValue());
        }

        [TestMethod]
        public void ShouldNotReturnFirstTimeValue()
        {
            PXModel pxModel = TestFactory.GetPxModel();
            Assert.AreNotEqual("2018M12", pxModel.Meta.GetFirstTimeValue());
        }

        [TestMethod]
        public void ShouldReturnLastTimeValue()
        {
            PXModel pxModel = TestFactory.GetPxModel();
            Assert.AreEqual("2018M12", pxModel.Meta.GetLastTimeValue());
        }

        [TestMethod]
        public void ShouldNotReturnLastTimeValue()
        {
            PXModel pxModel = TestFactory.GetPxModel();
            Assert.AreNotEqual("2018M01", pxModel.Meta.GetLastTimeValue());
        }
    }
}
