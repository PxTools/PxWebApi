using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Px.Search;
using Px.Search.Lucene;
using Px.Search.Lucene.Config;
using System.IO;

namespace PxWeb.UnitTests.Search
{
    //Perhaps these tests test Lucene as much as local code :-)
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

            //seeking "C:\\repos\\github\\pxtools\\PxwebApi3\\PxWebApi\\PxWeb\\wwwroot" 
            string pathRunning = Directory.GetCurrentDirectory();
            int index = pathRunning.IndexOf("PxWeb.UnitTests");
            
            if (index == -1)
            {
                throw new System.Exception("Hmm, Directory.GetCurrentDirectory() does not contain string:PxWeb.UnitTests , so unable to find wwwroot path.");
            }
            string repoRoot = pathRunning.Substring(0, index);
            string wwwPath = Path.Combine(repoRoot,  "PxWeb", "wwwroot");

            MyHost myHost = new MyHost(wwwPath);



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
