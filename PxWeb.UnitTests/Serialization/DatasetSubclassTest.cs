using PxWeb.Models.Api2;

namespace PxWeb.UnitTests.Serialization
{
    [TestClass]
    public class DatasetSubclassTest
    {

        [TestMethod]
        public void DatasetSubclass_AddAlternativeText_ShouldNotBeSetWhenNoAlternativeTextExists()
        {

            // Arrange
            var value = new DimensionValue();

#pragma warning disable CS8625 // Cannot convert null literal 
            // Act
            DatasetSubclass.AddAlternativeText(value, "valueCode", null);

#pragma warning restore CS8625

            // Assert
            Assert.IsNull(value.Extension);

        }

        [TestMethod]
        public void DatasetSubclass_AddAlternativeText_ShouldNotBeSetWhenNoAlternativeTextExistsEvenIfExtensionExists()
        {

            // Arrange
            var value = new DimensionValue();
            value.Extension = new ExtensionDimension();

#pragma warning disable CS8625 // Cannot convert null literal 
            // Act
            DatasetSubclass.AddAlternativeText(value, "valueCode", null);

#pragma warning restore CS8625

            // Assert
            Assert.IsNotNull(value.Extension);
            Assert.IsNull(value.Extension.AlternativeText);

        }

        [TestMethod]
        public void DatasetSubclass_AddAlternativeText_ShouldBeSetWhenAlternativeTextExists()
        {

            // Arrange
            var value = new DimensionValue();
            var altText = "This is alternative text";

            // Act
            DatasetSubclass.AddAlternativeText(value, "valueCode", altText);


            // Assert
            Assert.IsNotNull(value.Extension);
            Assert.AreEqual(altText, value.Extension.AlternativeText["valueCode"]);

        }
    }
}
