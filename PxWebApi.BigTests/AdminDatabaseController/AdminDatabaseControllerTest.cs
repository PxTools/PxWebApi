using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using PxWeb.Code.BackgroundWorker;

namespace PxWebApi.BigTests.AdminDatabaseController
{
    [TestClass]
    public class AdminDatabaseControllerTest
    {


        [TestMethod]
        [Description("Same input-file gives same output string.")]

        public async Task MakeMenu()
        {
            var afterCheckoutBeforeBuildMenu = DateTime.Now;


            var myState = new ControllerStateProvider();
            var myQueue = new BackgroundWorkerQueue();

            string conf_dir_String = Util.GetFullPathToFile(@"PxWebApi.BigTests/AdminDatabaseController/");
            var builder = new ConfigurationBuilder()
            .SetBasePath(conf_dir_String)
            .AddJsonFile("test_appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();


            Mock<IPxHost> hostingEnvironmentMock = new Mock<IPxHost>();
            var wwwrootPath = Util.GetFullPathToFile(@"PxWeb/wwwroot");
            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);


            // Create an instance of your test logger
            var testLogger = new TestLogger<PxWeb.Controllers.Api2.Admin.DatabaseController>();

            // var loggerMock = new Mock<ILogger<PxWeb.Controllers.Api2.Admin.DatabaseController>>();

            ///
            IOptions<PxApiConfigurationOptions> pxApiConfigurationOptions = Util.GetIOptions<PxApiConfigurationOptions>(configuration, "PxApiConfiguration");
            IPxApiConfigurationService pxApiConfigurationService = new PxApiConfigurationService(pxApiConfigurationOptions);

            //GetIOptions
            IOptions<PxFileConfigurationOptions> pxFileConfigurationOptions = Util.GetIOptions<PxFileConfigurationOptions>(configuration, "DataSource:PX");
            IPxFileConfigurationService pxFileConfigurationService = new PxFileConfigurationService(pxFileConfigurationOptions);


            var mockIPxCache = new Mock<IPxCache>();


            var loggerMockItemSelectorResolverPxFactory = new Mock<ILogger<ItemSelectorResolverPxFactory>>();
            IItemSelectionResolverFactory itemSelectorResolverPxFactory = new ItemSelectorResolverPxFactory(pxFileConfigurationService, hostingEnvironmentMock.Object, loggerMockItemSelectorResolverPxFactory.Object);
            IItemSelectionResolver itemSelectionResolver = new ItemSelectionResolverPxFile(mockIPxCache.Object, itemSelectorResolverPxFactory, pxApiConfigurationService);

            var loggerMockTablePathResolverPxFile = new Mock<ILogger<TablePathResolverPxFile>>();
            ITablePathResolver tablePathResolver = new TablePathResolverPxFile(mockIPxCache.Object, hostingEnvironmentMock.Object, pxApiConfigurationService, loggerMockTablePathResolverPxFile.Object);
            ICodelistMapper codelistMapper = new CodelistMapper();

            IDataSource iDataSource = new PxFileDataSource(pxFileConfigurationService, itemSelectionResolver, tablePathResolver,
                                                          hostingEnvironmentMock.Object, codelistMapper);
            ///

            PxWeb.Controllers.Api2.Admin.DatabaseController dac =
            new PxWeb.Controllers.Api2.Admin.DatabaseController(myState, myQueue, iDataSource, pxApiConfigurationOptions, testLogger, hostingEnvironmentMock.Object);

            Console.WriteLine("Hello");

            CancellationToken cancellationToken = new CancellationToken();
            await dac.createMenuXml(false, "filename", cancellationToken);

            foreach (var item in testLogger.LogMessages)
            {
                Console.WriteLine("Logged: " + item);
            }


            Console.WriteLine("Done logged.");




            var actualFilePath = Util.GetFullPathToFile(@"PxWeb/wwwroot/Database/Menu.xml");
            var expectedFilePath = Util.GetFullPathToFile(@"PxWebApi.BigTests/AdminDatabaseController/expectedMenu.xml");

            FileInfo fileInfo = new FileInfo(actualFilePath);
            DateTime menuXmllastUpdated = fileInfo.LastWriteTime;

            string FailString = "Failed menuXmllastUpdated > afterCheckoutBeforeBuildMenu.  menuXmllastUpdated = "
                 + menuXmllastUpdated.ToString() + ", afterCheckoutBeforeBuildMenu " + afterCheckoutBeforeBuildMenu.ToString();

            Assert.IsTrue(menuXmllastUpdated > afterCheckoutBeforeBuildMenu, FailString);




            var expected = RemoveElements(expectedFilePath).ToString();

            //Ensuring we dont compare empty things
            Assert.IsTrue(expected.Length > 10000, "Problems reading expectedMenu.xml or it no longer contains more than 10000 chars");



            var actual = RemoveElements(actualFilePath).ToString();


            Assert.AreEqual(expected.Substring(0, 5), actual.Substring(0, 5), "Diff in first 5.");

            //Comparing them in chunks or it is imposible to spot the diff 
            for (int i = 0; i < actual.Length; i += 25)
            {
                int lengthToCompare = Math.Min(50, actual.Length - i);
                Assert.AreEqual(expected.Substring(i, lengthToCompare), actual.Substring(i, lengthToCompare));
            }

            Assert.AreEqual(expected, actual);

        }



        public static XDocument RemoveElements(string fileName)
        {
            // Load the XML file into an XDocument
            XDocument doc = XDocument.Load(fileName);

            // Remove all <Published> elements
            doc.Descendants("Published").Remove();

            // Find and remove all <Attribute name="updated"> elements
            var attributesToRemove = doc.Descendants()
                .Where(x => x.Name == "Attribute" && ("updated" == (string?)x.Attribute("name") || "size" == (string?)x.Attribute("name")))
                .ToList();

            foreach (var attr in attributesToRemove)
            {
                attr.Remove();
            }

            return doc;
        }
    }

}

