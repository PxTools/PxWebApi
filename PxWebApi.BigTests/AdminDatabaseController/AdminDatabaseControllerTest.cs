using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using PxWeb.Code.BackgroundWorker;
using PxWeb.Controllers.Api2.Admin;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using System;
using System.Linq;
using System.Xml.Linq;

namespace PxWebApi.BigTests.AdminDatabaseController
{
    [TestClass]
    public class AdminDatabaseControllerTest
    {


        [TestMethod]
        [Description("Same input-file gives same output string.")]

        public async Task MakeMenu()
        {
            var myState = new ControllerStateProvider();
            var myQueue = new BackgroundWorkerQueue();

            string conf_dir_String = Util.GetFullPathToFile(@"PxWebApi.BigTests/AdminDatabaseController/");
            var builder = new ConfigurationBuilder()
            .SetBasePath(conf_dir_String)
            .AddJsonFile("test_appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();


            Mock<IPxHost> hostingEnvironmentMock = new Mock<IPxHost>();
            var wwwrootPath = Util.GetFullPathToFile(@"PxWeb\wwwroot");
            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);


            var loggerMock = new Mock<ILogger<PxWeb.Controllers.Api2.Admin.DatabaseController>>();


            ///
            IOptions<PxApiConfigurationOptions> pxApiConfigurationOptions = Util.GetIOptions<PxApiConfigurationOptions>(configuration, "PxApiConfiguration");
            IPxApiConfigurationService pxApiConfigurationService = new PxApiConfigurationService(pxApiConfigurationOptions);

           // Mock<ILogger<PxWeb.Controllers.Api2.TableApiController>> loggerMock = new Mock<ILogger<PxWeb.Controllers.Api2.TableApiController>>();

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
            new PxWeb.Controllers.Api2.Admin.DatabaseController(myState, myQueue, iDataSource, pxApiConfigurationOptions, loggerMock.Object, hostingEnvironmentMock.Object);
            
            CancellationToken cancellationToken = new CancellationToken();
            await dac.createMenuXml(false, "filename", cancellationToken);
     

            var expectedFilePath = Util.GetFullPathToFile(@"PxWebApi.BigTests/AdminDatabaseController/expectedMenu.xml");

            var expected = RemoveElements(expectedFilePath).ToString();

            //Ensuring we dont compare empty things
            Assert.IsTrue(expected.Length > 10000, "Problems reading expectedMenu.xml or it no longer contains more than 10000 chars");

            var actualFilePath = Util.GetFullPathToFile(@"PxWeb/wwwroot/Database/Menu.xml");

            var actual = RemoveElements(actualFilePath).ToString();


            Assert.AreEqual(expected.Substring(0, 5), actual.Substring(0, 5), "Diff in first 5.");

            //updated causes problems. When expected and actual is made in different places and input is in localtime.
            //int posOfUpdatedString = expected.IndexOf("2023-05-25T13:42:00");
            //actual = actual.Substring(0, posOfUpdatedString) + "XXXX-XX-XXTXX" + actual.Substring(posOfUpdatedString + 13);
            //expected = expected.Substring(0, posOfUpdatedString) + "XXXX-XX-XXTXX" + expected.Substring(posOfUpdatedString + 13);

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
                .Where(x => x.Name == "Attribute" && "updated" == (string?) x.Attribute("name") )
                .ToList();

            foreach (var attr in attributesToRemove)
            {
                attr.Remove();
            }

            return doc;
        }
    }




}

