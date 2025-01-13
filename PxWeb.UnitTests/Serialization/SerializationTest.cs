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
            var serializer = serializeManager.GetSerializer(outputFormat, new List<string>());

            Assert.AreEqual(serializer.GetType().Name, "PxDataSerializer");
        }

        [TestMethod]
        public void ShouldSerializeToPxFomrat()
        {
            PXModel pxModel = TestFactory.GetPxModel();

            var serializeManager = new SerializeManager();
            string outputFormat = "px";
            var serializer = serializeManager.GetSerializer(outputFormat, new List<string>());

            var response = new Mock<HttpResponse>();
            response.Setup(x => x.StatusCode).Returns(1);

            Assert.AreEqual(serializer.GetType().Name, "PxDataSerializer");
        }


    }
}
