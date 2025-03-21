namespace PxWeb.UnitTests.Helpers
{
    [TestClass]
    public class TableResponseMapperTests
    {
        [TestMethod]
        public void Convert_WithA_ShouldReturnAnnual()
        {
            //Act
            var timeUnit = TableResponseMapper.Convert("A");

            //Assert
            Assert.AreEqual(TimeUnit.AnnualEnum, timeUnit);
        }

        [TestMethod]
        public void Convert_WithQ_ShouldReturnQuarterly()
        {
            //Act
            var timeUnit = TableResponseMapper.Convert("Q");

            //Assert
            Assert.AreEqual(TimeUnit.QuarterlyEnum, timeUnit);
        }

        [TestMethod]
        public void Convert_WithM_ShouldReturnMontly()
        {
            //Act
            var timeUnit = TableResponseMapper.Convert("M");

            //Assert
            Assert.AreEqual(TimeUnit.MonthlyEnum, timeUnit);
        }


        [TestMethod]
        public void Convert_WithW_ShouldReturnWeekly()
        {
            //Act
            var timeUnit = TableResponseMapper.Convert("W");

            //Assert
            Assert.AreEqual(TimeUnit.WeeklyEnum, timeUnit);
        }
    }
}
