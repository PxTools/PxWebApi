namespace PxWeb.UnitTests.Search
{
    [TestClass]
    public class LuceneIndexTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private LuceneIndex _luceneIndex;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [TestInitialize]
        public void Setup()
        {
            _luceneIndex = new LuceneIndex("testIndexDirectory");
        }


        [TestMethod]
        public void BeginWrite_ShouldCreateIndexWriter()
        {
            // Arrange
            var language = "en";

            // Act
            _luceneIndex.BeginWrite(language);
            _luceneIndex.EndWrite(language);

            // Assert
            Assert.IsNotNull(_luceneIndex);
        }

        [TestMethod]
        public void BeginUpdate_ShouldCreateIndexWriterAndReader()
        {
            // Arrange
            var language = "en";

            // Act
            _luceneIndex.BeginUpdate(language);
            _luceneIndex.EndUpdate(language);
            // Assert
            Assert.IsNotNull(_luceneIndex);
        }

        [TestMethod]
        public void UpdatedEntry_ShouldCreateIndexWriterAndReader()
        {
            // Arrange
            var language = "en";
            var tableInformation = new TableInformation(
                    "TAB001",
                    "Population in the world",
                    "Population",
                    "2000",
                    "2005",
                    new string[] { "TIME" });
            tableInformation.Description = "Test";
            tableInformation.SortCode = "001";
            tableInformation.Paths.Add(new Level[] { new Level("A", "Test") });

            var meta = new PXMeta();
            meta.Matrix = "TAB001";
            meta.Variables.Add(ModelStore.CreateTimeVariable("", PlacementType.Stub, 2000, 2005));

            // Act
            _luceneIndex.BeginUpdate(language);
            _luceneIndex.UpdateEntry(tableInformation, meta);
            _luceneIndex.EndUpdate(language);
            // Assert
            Assert.IsNotNull(_luceneIndex);
        }

    }

}
