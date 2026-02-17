using System;

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

            try
            {
                // Act
                _luceneIndex.BeginWrite(language);
                _luceneIndex.EndWrite(language);
            }
            catch (Exception e)
            {
                // Assert
                Assert.Fail(e.Message);
            }


        }

        [TestMethod]
        public void BeginUpdate_ShouldCreateIndexWriterAndReader()
        {
            // Arrange
            var language = "en";

            try
            {
                // Act
                _luceneIndex.BeginUpdate(language);
                _luceneIndex.EndUpdate(language);
            }
            catch (Exception e)
            {
                // Assert
                Assert.Fail(e.Message);
            }
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
            tableInformation.Paths.Add(new Level[] { new Level("A", "Test", "A") });

            var meta = new PXMeta();
            meta.Matrix = "TAB001";
            meta.Variables.Add(ModelStore.CreateTimeVariable("", PlacementType.Stub, 2000, 2005));

            try
            {
                // Act
                _luceneIndex.BeginUpdate(language);
                _luceneIndex.UpdateEntry(tableInformation, meta);
                _luceneIndex.EndUpdate(language);
            }
            catch (Exception e)
            {
                // Assert
                Assert.Fail(e.Message);
            }
        }


        [TestMethod]
        public void NewLucenIndex_NoPath_ShouldThrowExcpetion()
        {
            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => { var index = new LuceneIndex(""); });
        }


        [TestMethod]
        public void UsageWithoutOpening_ShouldThrowExcpetion()
        {

            // Arrange

            var tableInformation = new TableInformation(
                    "TAB001",
                    "Population in the world",
                    "Population",
                    "2000",
                    "2005",
                    new string[] { "TIME" });
            tableInformation.Description = "Test";
            tableInformation.SortCode = "001";
            tableInformation.Paths.Add(new Level[] { new Level("A", "Test", "A") });

            var meta = new PXMeta();
            meta.Matrix = "TAB001";
            meta.Variables.Add(ModelStore.CreateTimeVariable("", PlacementType.Stub, 2000, 2005));


            //
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.AddEntry(tableInformation, meta));
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.RemoveEntry("removable"));

            //update with path
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.UpdateEntry(tableInformation, meta));

            //update withOut path
            tableInformation.Paths.Clear();
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.UpdateEntry(tableInformation, meta));


        }

        [TestMethod]
        public void Dispose_ShouldNotThrowExcpetion()
        {

            // Arrange
            try
            {
                LuceneIndex luceneIndex2 = new LuceneIndex("testIndex2Directory");
                luceneIndex2.Dispose();
                luceneIndex2.Dispose();
            }
            catch
            {
                Assert.Fail();
            }
        }




    }

}
