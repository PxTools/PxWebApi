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
        public void GetTimeUnit_TimeScaleNotSet_ReturnsX()
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

        [TestMethod]
        public void GetTimeUnit_TimeScaleAnnula_ReturnsA()
        {

            // Arrange
            var variable = new TVariable();
            variable.SetTimeScale("tlist(A1)");

            PXMeta meta = new PXMeta();
            meta.Variables.Add(variable);
            var model = new PXModel();
            model.Meta = meta;

            // Act
            var timeUnit = model.Meta.GetTimeUnit();

            // Assert
            Assert.AreEqual("A", timeUnit);
        }


        [TestMethod]
        public void GetTimeUnit_TimeScaleQuartely_ReturnsQ()
        {

            // Arrange
            var variable = new TVariable();
            variable.SetTimeScale("tlist(Q1)");

            PXMeta meta = new PXMeta();
            meta.Variables.Add(variable);
            var model = new PXModel();
            model.Meta = meta;

            // Act
            var timeUnit = model.Meta.GetTimeUnit();

            // Assert
            Assert.AreEqual("Q", timeUnit);
        }

        [TestMethod]
        public void GetTimeUnit_TimeScaleWeekly_ReturnsW()
        {

            // Arrange
            var variable = new TVariable();
            variable.SetTimeScale("tlist(W1)");

            PXMeta meta = new PXMeta();
            meta.Variables.Add(variable);
            var model = new PXModel();
            model.Meta = meta;

            // Act
            var timeUnit = model.Meta.GetTimeUnit();

            // Assert
            Assert.AreEqual("W", timeUnit);
        }

        [TestMethod]
        public void GetTimeUnit_TimeScaleHalfyear_ReturnsX()
        {

            // Arrange
            var variable = new TVariable();
            variable.SetTimeScale("tlist(H1)");

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

    public class TVariable : Variable
    {
        public TVariable()
        {
            base.IsTime = true;
            base.SetTimeScale("A");
        }
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public void SetTimeScale(string timeScale)
        {
            base.SetTimeScale(timeScale);
        }
        public bool IsTime { get { return true; } }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword


    }
}
