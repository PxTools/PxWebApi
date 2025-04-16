namespace PxWeb.UnitTests.SavedQuery
{
    [TestClass]
    public class SavedQueryDatabaseStorageOptionsTests
    {

        [TestMethod]
        public void CheckDefaultConstructor_ShouldInitializeProperties()
        {
            // Arrange
            var options = new SavedQueryDatabaseStorageOptions();
            // Act & Assert
            Assert.AreEqual(string.Empty, options.ConnectionString);
            Assert.AreEqual("default", options.TargetDatabase);
            Assert.AreEqual("dbo", options.TableOwner);
            Assert.AreEqual("Microsoft", options.DatabaseVendor);
        }



    }
}
