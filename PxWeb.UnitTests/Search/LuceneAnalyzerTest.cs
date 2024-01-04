using Microsoft.VisualStudio.TestTools.UnitTesting;
using Px.Search;
using Px.Search.Lucene;
using Px.Search.Lucene.Config;

namespace PxWeb.UnitTests.Search
{
    [Ignore("These tests require that you index your databases first. And perhaps they test Lucene as much as local code :-)")]
    [TestClass]
    public class LuceneAnalyzerTest
    {
        [TestMethod]
        public void UsingStandardAnalyzerOnPlural()
        {
            TestSearcher(true, "sv", "regionar", 0);
        }

        [TestMethod]
        public void UsingStandardAnalyzerOnSingular()
        {
            TestSearcher(true, "sv", "region", 3);
        }

        [TestMethod]
        public void UsingSwedishAnalyzerOnPlural()
        {
            TestSearcher(false, "sv", "regionar", 3);
        }

        [TestMethod]
        public void UsingSwedishAnalyzerOnSingular()
        {
            TestSearcher(false, "sv", "region", 3);
        }


        private void TestSearcher(bool useStandardAnalyzer, string language, string searchFor, int expectedCount)
        {
            LuceneConfigurationOptions luceneConfigurationOptions = new LuceneConfigurationOptions();
            luceneConfigurationOptions.UseStandardAnalyzer = useStandardAnalyzer;
            luceneConfigurationOptions.IndexDirectory = "Database";

            MyHost myHost = new MyHost("C:\\repos\\github\\pxtools\\PxwebApi2\\PxWebApi\\PxWeb\\wwwroot");


            var myOptions = Microsoft.Extensions.Options.Options.Create(luceneConfigurationOptions);

            ILuceneConfigurationService conf = new LuceneConfigurationService(myOptions, myHost);

            ISearchBackend myBackend = new LuceneBackend(conf);

            var searcher = myBackend.GetSearcher(language);

            // Act
            var res = searcher.Find(searchFor, 1, 1, null);

            // Assert
            Assert.AreEqual(expectedCount, res.totalElements);
        }

    }
}
