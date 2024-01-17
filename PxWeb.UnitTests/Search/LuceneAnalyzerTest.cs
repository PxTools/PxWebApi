using Microsoft.VisualStudio.TestTools.UnitTesting;
using Px.Search;
using Px.Search.Lucene;
using Px.Search.Lucene.Config;

namespace PxWeb.UnitTests.Search
{
    //Perhaps these tests test Lucene as much as local code :-)")]
    [TestClass]
    public class LuceneAnalyzerTest
    {
        [TestMethod]
        public void UsingSwedishAnalyzerOnPlural()
        {
            TestSearcher("sv", "regionar", 3);
        }

        [TestMethod]
        public void UsingSwedishAnalyzerOnSingular()
        {
            TestSearcher("sv", "region", 3);
        }


        private void TestSearcher(string language, string searchFor, int expectedCount)
        {
            LuceneConfigurationOptions luceneConfigurationOptions = new LuceneConfigurationOptions();
            luceneConfigurationOptions.IndexDirectory = "Database";

            MyHost myHost = new MyHost("C:\\repos\\github\\pxtools\\PxwebApi3\\PxWebApi\\PxWeb\\wwwroot");


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
