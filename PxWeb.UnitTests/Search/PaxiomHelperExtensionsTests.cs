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
            variable.SetTimeScaleX("tlist(A1)");

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
            variable.SetTimeScaleX("tlist(Q1)");

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
        public void GetTimeUnit_TimeScaleMonthly_ReturnsM()
        {

            // Arrange
            var variable = new TVariable();
            variable.SetTimeScaleX("tlist(M1)");

            PXMeta meta = new PXMeta();
            meta.Variables.Add(variable);
            var model = new PXModel();
            model.Meta = meta;

            // Act
            var timeUnit = model.Meta.GetTimeUnit();

            // Assert
            Assert.AreEqual("M", timeUnit);
        }

        [TestMethod]
        public void GetTimeUnit_TimeScaleWeekly_ReturnsW()
        {

            // Arrange
            var variable = new TVariable();
            variable.SetTimeScaleX("tlist(W1)");

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
            variable.SetTimeScaleX("tlist(H1)");

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
        public void SetTimeScaleX(string timeScale)
        {
            base.SetTimeScale(timeScale);
        }


    }
}
