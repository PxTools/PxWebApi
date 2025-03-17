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

    }

}
