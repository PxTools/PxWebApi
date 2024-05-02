using System;

using PxWeb.Helper.Api2;

namespace PxWebApi.BigTests.TableController
{
    [TestClass]
    public class TableApiControllerTest
    {
        private static readonly Mock<IPxHost> hostingEnvironmentMock = new Mock<IPxHost>();

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            var wwwrootPath = Util.GetFullPathToFile(@"PxWeb/wwwroot/");
            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

        }


        [TestMethod]
        [Description("Same input-file gives same output string.")]
        public void ListAllTables()
        {
            PxWeb.Controllers.Api2.TableApiController tac = GetController();

            var result = tac.ListAllTables(null, null, null, null, null, null);

            var aa = result.ToJSON(false);
            var bb = "{\"Value\":{\"language\":\"en\",\"tables\":[{\"updated\":\"2023-05-25T13:42:00\",\"firstPeriod\":\"1990\",\"lastPeriod\":\"2017\",\"category\":\"public\",\"variableNames\":[\"sector\",\"greenhouse gas\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB004\",\"label\":\"Total air emissions by sector, greenhouse gas, contents and year\",\"description\":\"\"},{\"updated\":\"2023-08-31T13:42:00\",\"firstPeriod\":\"2010\",\"lastPeriod\":\"2015\",\"category\":\"public\",\"variableNames\":[\"region\",\"land use\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB005?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB005/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB005/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB005\",\"label\":\"Land use in Sweden, hectares by region, land use, contents and year\",\"description\":\"\"},{\"updated\":\"2023-05-25T14:13:00\",\"firstPeriod\":\"2010\",\"lastPeriod\":\"2016\",\"category\":\"public\",\"variableNames\":[\"treatment category\",\"waste category\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB003?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB003/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB003/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB003\",\"label\":\"Disposal facilities by treatment category, waste category, contents and year\",\"description\":\"\"},{\"updated\":\"2023-05-25T13:42:00\",\"firstPeriod\":\"1981\",\"lastPeriod\":\"2001\",\"category\":\"public\",\"variableNames\":[\"region\",\"sex\",\"age\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB001?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB001/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB001/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB001\",\"label\":\"Population by region, sex, age, contents and year\",\"description\":\"\"},{\"updated\":\"2023-05-25T13:42:00\",\"firstPeriod\":\"1991\",\"lastPeriod\":\"2001\",\"category\":\"public\",\"variableNames\":[\"region\",\"sex\",\"age\",\"citizenship\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB002?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB002/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB002/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB002\",\"label\":\"Population by region, sex, age, citizenship, contents and year\",\"description\":\"\"}],\"page\":{\"pageNumber\":1,\"pageSize\":20,\"totalElements\":5,\"totalPages\":1,\"links\":[]},\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/?lang=en&pagesize=20&pageNumber=1\"}]},\"Formatters\":[],\"ContentTypes\":[],\"StatusCode\":200}";


            Assert.AreEqual(bb, aa);
        }

        [TestMethod]
        [Description("Same input-file gives same output string.")]
        public void GetTableById()
        {
            PxWeb.Controllers.Api2.TableApiController tac = GetController();

            var result = tac.GetTableById("TAB004", null);

            var aa = result.ToJSON(false);
            var bb = "{\"Value\":{\"language\":\"en\",\"updated\":\"2023-05-25T13:42:00\",\"firstPeriod\":\"1990\",\"lastPeriod\":\"2017\",\"category\":\"public\",\"variableNames\":[\"sector\",\"greenhouse gas\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB004\",\"label\":\"Total air emissions by sector, greenhouse gas, contents and year\",\"description\":\"\",\"sortCode\":\"TAB004\"},\"Formatters\":[],\"ContentTypes\":[],\"StatusCode\":200}";


            Assert.AreEqual(bb, aa);
        }



        [TestMethod]
        [Description("Same input-file gives same output string.")]
        public void GetMetadataById()
        {
            PxWeb.Controllers.Api2.TableApiController tac = GetController();

            var result = tac.GetMetadataById("TAB004", null);

            var actual = result.ToJSON(false);
            var expected = "{\"Value\":{\"language\":\"en\",\"id\":\"TAB004\",\"label\":\"Total air emissions by sector, greenhouse gas, contents and year\",\"aggregationAllowed\":false,\"officialStatistics\":false,\"subjectCode\":\"EN\",\"subjectLabel\":\"Environment\",\"source\":\"Statistics Sweden\",\"license\":\"https://creativecommons.org/share-your-work/public-domain/cc0/\",\"updated\":\"2023-05-25T13:42:00Z\",\"notes\":[{\"mandatory\":false,\"text\":\"Footnote text\"},{\"mandatory\":false,\"text\":\".. = Data not available\"}],\"contacts\":[{\"raw\":\"Test Testsson, SCB#Tel: 08-111 222 33#Fax: 08-222 333 44#E-mail: test.testsson@scb.se\"},{\"raw\":\"Test2 Testsson2, SCB#Tel: 08-333 444 55#Fax: 08-444 555 66#E-mail: test2.testsson2@scb.se\"}],\"variables\":[{\"elimination\":false,\"values\":[{\"code\":\"0.1\",\"label\":\"NATIONAL TOTAL (excluding LULUCF, excluding international transports)\"},{\"code\":\"0.2\",\"label\":\"NATIONAL TOTAL (excluding LULUCF, including international transports)\"},{\"code\":\"0.3\",\"label\":\"NATIONAL TOTAL (including LULUCF, excluding international transports)\"},{\"code\":\"0.4\",\"label\":\"NATIONAL TOTAL (including LULUCF, including international transports)\"},{\"code\":\"1.0\",\"label\":\"OFF-ROAD VEHICLES AND OTHER MACHINERY, TOTAL\"},{\"code\":\"10.0\",\"label\":\"LAND-USE, LAND-USE CHANGE AND FORESTRY (LULUCF), TOTAL\"},{\"code\":\"2.0\",\"label\":\"WASTE, TOTAL\"},{\"code\":\"3.0\",\"label\":\"ELECTRICITY AND DISTRICT HEATING, TOTAL\"},{\"code\":\"4.0\",\"label\":\"INDUSTRY, TOTAL\"},{\"code\":\"5.0\",\"label\":\"INTERNATIONAL TRANSPORT, TOTAL\"},{\"code\":\"6.0\",\"label\":\"AGRICULTURE, TOTAL\"},{\"code\":\"7.0\",\"label\":\"SOLVENT USE AND OTHER PRODUCT USE, TOTAL\"},{\"code\":\"8.0\",\"label\":\"DOMESTIC TRANSPORT, TOTAL\"},{\"code\":\"9.0\",\"label\":\"HEATING OF HOUSES AND PREMISES, TOTAL\"}],\"id\":\"SECTOR\",\"label\":\"sector\",\"type\":\"RegularVariable\",\"notes\":[{\"mandatory\":false,\"text\":\"Footnote text\"}]},{\"elimination\":false,\"values\":[{\"code\":\"CH4\",\"label\":\"Methane (CH4) (t)\"},{\"code\":\"CH4_CO2-ekv.\",\"label\":\"Methane (CH4) (kt CO2-eqv.)\"},{\"code\":\"CO2\",\"label\":\"Carbon Dioxide (CO2) (kt)\"},{\"code\":\"CO2-BIO\",\"label\":\"Biogenic carbon dioxide (CO2) from fuels (kt)\"},{\"code\":\"CO2-ekv.\",\"label\":\"Total Greenhouse Gases (kt CO2-eqv.)\"},{\"code\":\"HFC\",\"label\":\"Hydrofluorocarbons (HFCs) (kt CO2-eqv.)\"},{\"code\":\"N2O\",\"label\":\"Nitrous Oxide (N2O) (t)\"},{\"code\":\"N2O_CO2-ekv.\",\"label\":\"Nitrous Oxide (N2O) (kt CO2-eqv.)\"},{\"code\":\"PFC\",\"label\":\"Perfluorocarbons (PFCs) (kt CO2-eqv.)\"},{\"code\":\"SF6\",\"label\":\"Sulphur Hexafluoride (SF6) (kg)\"},{\"code\":\"SF6_CO2-ekv.\",\"label\":\"Sulphur Hexafluoride (SF6) (kt CO2-eqv.)\",\"notes\":[{\"mandatory\":false,\"text\":\"Footnote text\"}]}],\"id\":\"GREENHOUSEGAS\",\"label\":\"greenhouse gas\",\"type\":\"RegularVariable\"},{\"values\":[{\"adjustment\":\"None\",\"measuringType\":\"Stock\",\"preferedNumberOfDecimals\":0,\"priceType\":\"Undefined\",\"unit\":\"Device varies with the subject\",\"code\":\"Emission\",\"label\":\"Substance\"}],\"id\":\"ContentsCode\",\"label\":\"contents\",\"type\":\"ContentsVariable\"},{\"timeUnit\":\"Annual\",\"firstPeriod\":\"1990\",\"lastPeriod\":\"2017\",\"values\":[{\"code\":\"1990\",\"label\":\"1990\"},{\"code\":\"1991\",\"label\":\"1991\"},{\"code\":\"1992\",\"label\":\"1992\"},{\"code\":\"1993\",\"label\":\"1993\"},{\"code\":\"1994\",\"label\":\"1994\"},{\"code\":\"1995\",\"label\":\"1995\"},{\"code\":\"1996\",\"label\":\"1996\"},{\"code\":\"1997\",\"label\":\"1997\"},{\"code\":\"1998\",\"label\":\"1998\"},{\"code\":\"1999\",\"label\":\"1999\"},{\"code\":\"2000\",\"label\":\"2000\"},{\"code\":\"2001\",\"label\":\"2001\"},{\"code\":\"2002\",\"label\":\"2002\"},{\"code\":\"2003\",\"label\":\"2003\"},{\"code\":\"2004\",\"label\":\"2004\"},{\"code\":\"2005\",\"label\":\"2005\"},{\"code\":\"2006\",\"label\":\"2006\"},{\"code\":\"2007\",\"label\":\"2007\"},{\"code\":\"2008\",\"label\":\"2008\"},{\"code\":\"2009\",\"label\":\"2009\"},{\"code\":\"2010\",\"label\":\"2010\"},{\"code\":\"2011\",\"label\":\"2011\"},{\"code\":\"2012\",\"label\":\"2012\"},{\"code\":\"2013\",\"label\":\"2013\"},{\"code\":\"2014\",\"label\":\"2014\"},{\"code\":\"2015\",\"label\":\"2015\"},{\"code\":\"2016\",\"label\":\"2016\"},{\"code\":\"2017\",\"label\":\"2017\"}],\"id\":\"TIME\",\"label\":\"year\",\"type\":\"TimeVariable\"}],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/data?lang=en\"}]},\"Formatters\":[],\"ContentTypes\":[]}";



            Assert.AreEqual(expected.Substring(0, 5), actual.Substring(0, 5), "Diff in first 5.");
            Assert.AreEqual(expected.Length, actual.Length, "Not correct length.");


            //updated causes problems. When expected and actual is made in different places and input is in localtime.
            int posOfUpdatedString = expected.IndexOf("2023-05-25T13:42:00");
            actual = actual.Substring(0, posOfUpdatedString) + "XXXX-XX-XXTXX" + actual.Substring(posOfUpdatedString + 13);
            expected = expected.Substring(0, posOfUpdatedString) + "XXXX-XX-XXTXX" + expected.Substring(posOfUpdatedString + 13);

            for (int i = 0; i < actual.Length; i += 25)
            {
                int lengthToCompare = Math.Min(50, actual.Length - i);
                Assert.AreEqual(expected.Substring(i, lengthToCompare), actual.Substring(i, lengthToCompare));
            }

        }


        /*
        [TestMethod]
        [Description("Same input-file gives same output string.")]
        public void GetTableData()
        {
            PxWeb.Controllers.Api2.TableApiController tac = GetController();

            Mock<HttpResponse> HttpResponseMock = new Mock<HttpResponse>();
            Mock<HttpContext> HttpContextMock = new Mock<HttpContext>();
            HttpContextMock
                .Setup(m => m.Response)
                .Returns(HttpResponseMock.Object);

            Mock<ControllerContext> ControllerContextMock = new Mock<ControllerContext>();
            ControllerContextMock
                .Setup(m => m.HttpContext)
                .Returns(HttpContextMock.Object);

            tac.ControllerContext = ControllerContextMock.Object;
            //This stops with: 
            //Test method PxWebApi.BigTests.TableController.TableApiControllerTest.GetTableData threw exception:
            //System.NotSupportedException: Unsupported expression: m => m.HttpContext
            //Non - overridable members(here: ActionContext.get_HttpContext) may not be used in setup / verification expressions.

            // We mock what we do not understand :-) 


            var result = tac.GetTableData("TAB004", "en", new Dictionary<string, List<string>>(), new Dictionary<string, string>(), new Dictionary<string, CodeListOutputValuesType>(), null);

            var aa = result.ToJSON(false);
            var bb = "{\"Value\":{\"language\":\"en\",\"id\":\"TAB004\",\"label\":\"Total air emissions by sector, greenhouse gas, contents and year\",\"aggregationAllowed\":false,\"officalStatistics\":false,\"subjectCode\":\"EN\",\"subjectLabel\":\"Environment\",\"source\":\"Statistics Sweden\",\"licence\":\"https://creativecommons.org/share-your-work/public-domain/cc0/\",\"updated\":\"2023-05-25T13:42:00Z\",\"notes\":[{\"mandatory\":false,\"text\":\"Footnote text\"},{\"mandatory\":false,\"text\":\".. = Data not available\"}],\"contacts\":[{\"raw\":\"Test Testsson, SCB#Tel: 08-111 222 33#Fax: 08-222 333 44#E-mail: test.testsson@scb.se\"},{\"raw\":\"Test2 Testsson2, SCB#Tel: 08-333 444 55#Fax: 08-444 555 66#E-mail: test2.testsson2@scb.se\"}],\"variables\":[{\"elimination\":false,\"values\":[{\"code\":\"0.1\",\"label\":\"NATIONAL TOTAL (excluding LULUCF, excluding international transports)\"},{\"code\":\"0.2\",\"label\":\"NATIONAL TOTAL (excluding LULUCF, including international transports)\"},{\"code\":\"0.3\",\"label\":\"NATIONAL TOTAL (including LULUCF, excluding international transports)\"},{\"code\":\"0.4\",\"label\":\"NATIONAL TOTAL (including LULUCF, including international transports)\"},{\"code\":\"1.0\",\"label\":\"OFF-ROAD VEHICLES AND OTHER MACHINERY, TOTAL\"},{\"code\":\"10.0\",\"label\":\"LAND-USE, LAND-USE CHANGE AND FORESTRY (LULUCF), TOTAL\"},{\"code\":\"2.0\",\"label\":\"WASTE, TOTAL\"},{\"code\":\"3.0\",\"label\":\"ELECTRICITY AND DISTRICT HEATING, TOTAL\"},{\"code\":\"4.0\",\"label\":\"INDUSTRY, TOTAL\"},{\"code\":\"5.0\",\"label\":\"INTERNATIONAL TRANSPORT, TOTAL\"},{\"code\":\"6.0\",\"label\":\"AGRICULTURE, TOTAL\"},{\"code\":\"7.0\",\"label\":\"SOLVENT USE AND OTHER PRODUCT USE, TOTAL\"},{\"code\":\"8.0\",\"label\":\"DOMESTIC TRANSPORT, TOTAL\"},{\"code\":\"9.0\",\"label\":\"HEATING OF HOUSES AND PREMISES, TOTAL\"}],\"id\":\"SECTOR\",\"label\":\"sector\",\"type\":\"RegularVariable\",\"notes\":[{\"mandatory\":false,\"text\":\"Footnote text\"}]},{\"elimination\":false,\"values\":[{\"code\":\"CH4\",\"label\":\"Methane (CH4) (t)\"},{\"code\":\"CH4_CO2-ekv.\",\"label\":\"Methane (CH4) (kt CO2-eqv.)\"},{\"code\":\"CO2\",\"label\":\"Carbon Dioxide (CO2) (kt)\"},{\"code\":\"CO2-BIO\",\"label\":\"Biogenic carbon dioxide (CO2) from fuels (kt)\"},{\"code\":\"CO2-ekv.\",\"label\":\"Total Greenhouse Gases (kt CO2-eqv.)\"},{\"code\":\"HFC\",\"label\":\"Hydrofluorocarbons (HFCs) (kt CO2-eqv.)\"},{\"code\":\"N2O\",\"label\":\"Nitrous Oxide (N2O) (t)\"},{\"code\":\"N2O_CO2-ekv.\",\"label\":\"Nitrous Oxide (N2O) (kt CO2-eqv.)\"},{\"code\":\"PFC\",\"label\":\"Perfluorocarbons (PFCs) (kt CO2-eqv.)\"},{\"code\":\"SF6\",\"label\":\"Sulphur Hexafluoride (SF6) (kg)\"},{\"code\":\"SF6_CO2-ekv.\",\"label\":\"Sulphur Hexafluoride (SF6) (kt CO2-eqv.)\",\"notes\":[{\"mandatory\":false,\"text\":\"Footnote text\"}]}],\"id\":\"GREENHOUSEGAS\",\"label\":\"greenhouse gas\",\"type\":\"RegularVariable\"},{\"values\":[{\"adjustment\":\"None\",\"measuringType\":\"Stock\",\"preferedNumberOfDecimals\":0,\"priceType\":\"Undefined\",\"unit\":\"Device varies with the subject\",\"code\":\"Emission\",\"label\":\"Substance\"}],\"id\":\"ContentsCode\",\"label\":\"contents\",\"type\":\"ContentsVariable\"},{\"timeUnit\":\"Annual\",\"firstPeriod\":\"1990\",\"lastPeriod\":\"2017\",\"values\":[{\"code\":\"1990\",\"label\":\"1990\"},{\"code\":\"1991\",\"label\":\"1991\"},{\"code\":\"1992\",\"label\":\"1992\"},{\"code\":\"1993\",\"label\":\"1993\"},{\"code\":\"1994\",\"label\":\"1994\"},{\"code\":\"1995\",\"label\":\"1995\"},{\"code\":\"1996\",\"label\":\"1996\"},{\"code\":\"1997\",\"label\":\"1997\"},{\"code\":\"1998\",\"label\":\"1998\"},{\"code\":\"1999\",\"label\":\"1999\"},{\"code\":\"2000\",\"label\":\"2000\"},{\"code\":\"2001\",\"label\":\"2001\"},{\"code\":\"2002\",\"label\":\"2002\"},{\"code\":\"2003\",\"label\":\"2003\"},{\"code\":\"2004\",\"label\":\"2004\"},{\"code\":\"2005\",\"label\":\"2005\"},{\"code\":\"2006\",\"label\":\"2006\"},{\"code\":\"2007\",\"label\":\"2007\"},{\"code\":\"2008\",\"label\":\"2008\"},{\"code\":\"2009\",\"label\":\"2009\"},{\"code\":\"2010\",\"label\":\"2010\"},{\"code\":\"2011\",\"label\":\"2011\"},{\"code\":\"2012\",\"label\":\"2012\"},{\"code\":\"2013\",\"label\":\"2013\"},{\"code\":\"2014\",\"label\":\"2014\"},{\"code\":\"2015\",\"label\":\"2015\"},{\"code\":\"2016\",\"label\":\"2016\"},{\"code\":\"2017\",\"label\":\"2017\"}],\"id\":\"TIME\",\"label\":\"year\",\"type\":\"TimeVariable\"}],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/data?lang=en\"}]},\"Formatters\":[],\"ContentTypes\":[]}";


            Assert.AreEqual(bb, aa);
        }
        */


        private PxWeb.Controllers.Api2.TableApiController GetController()
        {
            IConfigurationRoot configuration = GetConfigFile();

            IOptions<PxApiConfigurationOptions> pxApiConfigurationOptions = Util.GetIOptions<PxApiConfigurationOptions>(configuration, "PxApiConfiguration");
            IPxApiConfigurationService pxApiConfigurationService = new PxApiConfigurationService(pxApiConfigurationOptions);

            Mock<ILogger<PxWeb.Controllers.Api2.TableApiController>> loggerMock = new Mock<ILogger<PxWeb.Controllers.Api2.TableApiController>>();

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

            ILanguageHelper languageHelper = new LanguageHelper(pxApiConfigurationService);

            ILinkCreator linkCreator = new LinkCreator(pxApiConfigurationOptions);
            ITableMetadataResponseMapper responseMapper = new TableMetadataResponseMapper(linkCreator, pxApiConfigurationOptions);

            IOptions<LuceneConfigurationOptions> luceneConfigurationOptions = Util.GetIOptions<LuceneConfigurationOptions>(configuration, "LuceneConfiguration");
            ILuceneConfigurationService luceneConfigurationService = new LuceneConfigurationService(luceneConfigurationOptions, hostingEnvironmentMock.Object);

            ISearchBackend backend = new LuceneBackend(luceneConfigurationService);

            ITablesResponseMapper tablesResponseMapper = new TablesResponseMapper(linkCreator, pxApiConfigurationOptions);
            ITableResponseMapper tableResponseMapper = new TableResponseMapper(linkCreator, pxApiConfigurationOptions);
            ICodelistResponseMapper codelistResponseMapper = new CodelistResponseMapper(linkCreator, pxApiConfigurationOptions);
            ISerializeManager serializeManager = new SerializeManager();
            ISelectionHandler selectionHandler = new SelectionHandler(pxApiConfigurationService);

            ISelectionResponseMapper selectionResponseMapper = new SelectionResponseMapper(linkCreator);

            return new PxWeb.Controllers.Api2.TableApiController(iDataSource, languageHelper, responseMapper,
                                                                    backend, pxApiConfigurationOptions, tablesResponseMapper, tableResponseMapper,
                                                                    codelistResponseMapper, selectionResponseMapper, serializeManager, selectionHandler);

        }



        private static IConfigurationRoot GetConfigFile()
        {
            string conf_dir_String = Util.GetFullPathToFile(@"PxWebApi.BigTests/TableController/");

            var builder = new ConfigurationBuilder()
            .SetBasePath(conf_dir_String)
            .AddJsonFile("test_appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }
    }
}
