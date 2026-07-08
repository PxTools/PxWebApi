using System;
using System.Text;
using System.Text.Json;

namespace PxWeb.UnitTests.Search
{
    [TestClass]
    public class LuceneIndexTests
    {
        private string _indexRoot = string.Empty;
        private LuceneIndex _luceneIndex = null!;

        [TestInitialize]
        public void Setup()
        {
            _indexRoot = Path.Combine(Path.GetTempPath(), "PxWeb_LuceneIndexTests", Guid.NewGuid().ToString("N"));
            _luceneIndex = new LuceneIndex(_indexRoot);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _luceneIndex.Dispose();

            if (Directory.Exists(_indexRoot))
            {
                Directory.Delete(_indexRoot, true);
            }
        }

        [TestMethod]
        public void Constructor_EmptyPath_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new LuceneIndex(""));
        }

        [TestMethod]
        public void BeginWrite_ShouldCreateIndexWriter()
        {
            try
            {
                _luceneIndex.BeginWrite("en");
                _luceneIndex.EndWrite("en");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void BeginUpdate_ShouldCreateIndexWriterAndReader()
        {
            try
            {
                _luceneIndex.BeginUpdate("en");
                _luceneIndex.EndUpdate("en");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void UpdatedEntry_ShouldCreateIndexWriterAndReader()
        {
            var tableInformation = CreateValidTableInformation("TAB001");
            var meta = CreateValidMeta("TAB001");

            try
            {
                _luceneIndex.BeginUpdate("en");
                _luceneIndex.UpdateEntry(tableInformation, meta);
                _luceneIndex.EndUpdate("en");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void NewLucenIndex_NoPath_ShouldThrowExcpetion()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new LuceneIndex(""));
        }

        [TestMethod]
        public void UsageWithoutOpening_ShouldThrowExcpetion()
        {
            var tableInformation = CreateValidTableInformation("TAB001", includePaths: true);
            var meta = CreateValidMeta("TAB001");

            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.AddEntry(tableInformation, meta));
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.RemoveEntry("removable"));
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.UpdateEntry(tableInformation, meta));

            tableInformation.Paths.Clear();
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.UpdateEntry(tableInformation, meta));
        }

        [TestMethod]
        public void Dispose_ShouldNotThrowExcpetion()
        {
            var tempIndex = Path.Combine(Path.GetTempPath(), "PxWeb_LuceneIndexTests", Guid.NewGuid().ToString("N"));

            try
            {
                var luceneIndex2 = new LuceneIndex(tempIndex);
                luceneIndex2.Dispose();
                luceneIndex2.Dispose();
            }
            catch
            {
                Assert.Fail();
            }
            finally
            {
                if (Directory.Exists(tempIndex))
                {
                    Directory.Delete(tempIndex, true);
                }
            }
        }

        [TestMethod]
        public void BeginWrite_EmptyLanguage_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _luceneIndex.BeginWrite(""));
        }

        [TestMethod]
        public void BeginUpdate_EmptyLanguage_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _luceneIndex.BeginUpdate(""));
        }

        [TestMethod]
        public void BeginWrite_WhenIndexAlreadyLocked_ThrowsIOException()
        {
            _luceneIndex.BeginWrite("en");

            Assert.ThrowsExactly<IOException>(() => _luceneIndex.BeginWrite("en"));
        }

        [TestMethod]
        public void EndWrite_And_EndUpdate_WhenNotStarted_DoNotThrow()
        {
            try
            {
                _luceneIndex.EndWrite("en");
                _luceneIndex.EndUpdate("en");
            }
            catch (Exception)
            {
                Assert.Fail("EndWrite and EndUpdate should not throw when not started.");
            }
        }

        [TestMethod]
        public void AddEntry_WhenWriterIsMissing_ThrowsInvalidOperationException()
        {
            var table = CreateValidTableInformation("TAB001");
            var meta = CreateValidMeta("TAB001");

            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.AddEntry(table, meta));
        }

        [TestMethod]
        public void RemoveEntry_WhenWriterIsMissing_ThrowsInvalidOperationException()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.RemoveEntry("TAB001"));
        }

        [TestMethod]
        public void FindDocument_WhenSearcherIsMissing_ThrowsInvalidOperationException()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.FindDocument("TAB001"));
        }

        [TestMethod]
        public void UpdateEntry_WhenWriterMissing_AndPathsExist_ThrowsInvalidOperationException()
        {
            var table = CreateValidTableInformation("TAB001", includePaths: true);
            var meta = CreateValidMeta("TAB001");

            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.UpdateEntry(table, meta));
        }

        [TestMethod]
        public void UpdateEntry_WhenWriterAndSearcherMissing_AndPathsAreEmpty_ThrowsInvalidOperationException()
        {
            var table = CreateValidTableInformation("TAB001", includePaths: false);
            var meta = CreateValidMeta("TAB001");

            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.UpdateEntry(table, meta));
        }

        [TestMethod]
        public void AddAndFindDocument_ReturnsStoredDocument_AndMissingReturnsNull()
        {
            var table = CreateValidTableInformation("TAB001");
            table.Updated = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            table.Discontinued = true;

            var meta = CreateValidMeta("TAB001", includeOptionalMetaFields: true);

            _luceneIndex.BeginWrite("en");
            _luceneIndex.AddEntry(table, meta);
            _luceneIndex.EndWrite("en");

            _luceneIndex.BeginUpdate("en");
            var found = _luceneIndex.FindDocument("tab001");
            var notFound = _luceneIndex.FindDocument("does-not-exist");
            _luceneIndex.EndUpdate("en");

            Assert.IsNotNull(found);
            Assert.AreEqual("TAB001", found.Get(SearchConstants.SEARCH_FIELD_DOCID));
            Assert.AreEqual("Population in the world", found.Get(SearchConstants.SEARCH_FIELD_TITLE));
            Assert.AreEqual("True", found.Get(SearchConstants.SEARCH_FIELD_DISCONTINUED));
            Assert.IsFalse(string.IsNullOrWhiteSpace(found.Get(SearchConstants.SEARCH_FIELD_UPDATED)));
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void AddEntry_WhenRequiredDocumentDataMissing_ThrowsInvalidOperationException()
        {
            var table = CreateValidTableInformation("TAB001");
            table.Label = string.Empty;

            var meta = CreateValidMeta("TAB001");

            _luceneIndex.BeginWrite("en");

            Assert.ThrowsExactly<InvalidOperationException>(() => _luceneIndex.AddEntry(table, meta));
        }

        [TestMethod]
        public void UpdateEntry_WithEmptyPaths_RestoresStoredPathsFromExistingDocument()
        {
            var original = CreateValidTableInformation("TAB001", includePaths: true);
            var meta = CreateValidMeta("TAB001");

            _luceneIndex.BeginWrite("en");
            _luceneIndex.AddEntry(original, meta);
            _luceneIndex.EndWrite("en");

            var updated = CreateValidTableInformation("TAB001", includePaths: false);
            updated.Label = "Updated label";

            _luceneIndex.BeginUpdate("en");
            _luceneIndex.UpdateEntry(updated, meta);
            _luceneIndex.EndUpdate("en");

            _luceneIndex.BeginUpdate("en");
            var found = _luceneIndex.FindDocument("tab001");
            _luceneIndex.EndUpdate("en");

            Assert.IsNotNull(found);
            var bytes = found.GetBinaryValue(SearchConstants.SEARCH_FIELD_PATHS);
            Assert.IsNotNull(bytes);

            var json = Encoding.UTF8.GetString(bytes.Bytes, bytes.Offset, bytes.Length);
            var restoredPaths = JsonSerializer.Deserialize<List<Level[]>>(json);

            Assert.IsNotNull(restoredPaths);
            Assert.HasCount(1, restoredPaths);
            Assert.AreEqual("A", restoredPaths[0][0].Code);
        }

        [TestMethod]
        public void UpdateEntry_WithEmptyPaths_AndNoPreviousDocument_KeepsEmptyPaths()
        {
            var table = CreateValidTableInformation("TAB001", includePaths: false);
            var meta = CreateValidMeta("TAB001");

            // Create an initial (empty) index so BeginUpdate can create an IndexSearcher.
            _luceneIndex.BeginWrite("en");
            _luceneIndex.EndWrite("en");

            _luceneIndex.BeginUpdate("en");
            _luceneIndex.UpdateEntry(table, meta);
            _luceneIndex.EndUpdate("en");

            _luceneIndex.BeginUpdate("en");
            var found = _luceneIndex.FindDocument("tab001");
            _luceneIndex.EndUpdate("en");

            Assert.IsNotNull(found);

            var bytes = found.GetBinaryValue(SearchConstants.SEARCH_FIELD_PATHS);
            Assert.IsNotNull(bytes);

            var json = Encoding.UTF8.GetString(bytes.Bytes, bytes.Offset, bytes.Length);
            var paths = JsonSerializer.Deserialize<List<Level[]>>(json);

            Assert.IsNotNull(paths);
            Assert.IsEmpty(paths);
        }

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes_WithActiveWriter()
        {
            try
            {

                _luceneIndex.BeginWrite("en");

                _luceneIndex.Dispose();
                _luceneIndex.Dispose();
            }
            catch (Exception)
            {
                Assert.Fail("Calling Dispose multiple times should not throw exception");
            }
        }

        [TestMethod]
        public void GetAllTags_ReturnsConcatenatedString_AndHandlesEmptyArray()
        {
            var withTags = LuceneIndex.GetAllTags(new[] { "tag1", "tag2" });
            var emptyTags = LuceneIndex.GetAllTags(Array.Empty<string>());

            Assert.AreEqual("tag1 tag2 ", withTags);
            Assert.AreEqual(string.Empty, emptyTags);
        }

        private static TableInformation CreateValidTableInformation(string id, bool includePaths = true)
        {
            var table = new TableInformation(
                id,
                "Population in the world",
                "Population",
                "2000",
                "2005",
                new[] { "TIME", "SEX" })
            {
                Description = "Test description",
                SortCode = "001",
                Source = "SCB",
                TimeUnit = "A",
                SubjectCode = "SUBJ",
                Languages = new[] { "en", "sv" }
            };

            if (includePaths)
            {
                table.Paths.Add(new[] { new Level("A", "Level A", "A") });
            }

            return table;
        }

        private static PXMeta CreateValidMeta(string matrix, bool includeOptionalMetaFields = false)
        {
            var meta = new PXMeta
            {
                Matrix = matrix
            };

            var timeVariable = ModelStore.CreateTimeVariable("TIME", PlacementType.Stub, 2000, 2005);
            var nonTimeVariable = ModelStore.CreateClassificationVariable("sex", PlacementType.Stub, 2);

            if (includeOptionalMetaFields)
            {
                meta.MetaId = "META-ROOT";
                meta.MainTable = "MAIN-TABLE";
                meta.Synonyms = "population inhabitants";
                nonTimeVariable.MetaId = "META-VARIABLE";
            }

            meta.Variables.Add(timeVariable);
            meta.Variables.Add(nonTimeVariable);

            return meta;
        }
    }
}
