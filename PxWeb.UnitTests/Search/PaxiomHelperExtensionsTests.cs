namespace PxWeb.UnitTests.Search
{
    [TestClass]
    public class PaxiomHelperExtensionsTests
    {
        [TestMethod]
        public void GetTimeUnit_NoTime_ReturnsX()
        {
            // Arrange
            var pxModel = ModelStore.GetModelWithOnlyOneVariable(1);

            // Act
            var timeUnit = pxModel.Meta.GetTimeUnit();

            // Assert
            Assert.AreEqual("X", timeUnit);
        }

        [TestMethod]
        public void GetTimeUnit_TimeSacleNotSet_ReturnsX()
        {

            // Arrange
            var variable = ModelStore.CreateTimeVariable("", PlacementType.Stub, 10);
            PXMeta meta = new PXMeta();
            meta.Variables.Add(variable);
            var model = new PXModel();
            model.Meta = meta;

            // Act
            var timeUnit = model.Meta.GetTimeUnit();

            // Assert
            Assert.AreEqual("X", timeUnit);
        }
    }
}
