using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Px.Abstractions.Interfaces;
using Px.Search.Lucene.Config;
using PxWeb.Code.Api2.Cache;
using PxWeb.Config.Api2;
using PxWeb.Helper.Api2;
using PxWeb.Mappers;

namespace PxWebApi.BigTests.TableController
{
    [TestClass]
    public class TableApiControllerTest
    {
      
        [TestMethod]
        [Description("Same input-file gives same output string.")]

        public void GetTable()
        {
            string conf_dir_String = Util.GetFullPathToFile(@"PxWebApi.BigTests/TableController/");

            var builder = new ConfigurationBuilder()
            .SetBasePath(conf_dir_String)
            .AddJsonFile("test_appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            // Use the configuration
            //builder.Services.Configure<PxApiConfigurationOptions>(builder.Configuration.GetSection("PxApiConfiguration"));

            //PxApiConfigurationOptions pxApiConfigurationOptions = new PxApiConfigurationOptions();
            //configuration.GetSection("PxApiConfiguration").Bind(pxApiConfigurationOptions);
            //IOptions<PxApiConfigurationOptions> options = Options.Create(pxApiConfigurationOptions);

            IOptions<PxApiConfigurationOptions> pxApiConfigurationOptions = GetIOptions<PxApiConfigurationOptions>(configuration, "PxApiConfiguration");
            IPxApiConfigurationService pxApiConfigurationService = new PxApiConfigurationService(pxApiConfigurationOptions);



            //builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
           // IpRateLimitOptions ipRateLimitOptions = new IpRateLimitOptions();
           // configuration.GetSection("IpRateLimiting").Bind(ipRateLimitOptions);
          //  IOptions<IpRateLimitOptions> optionsRate = Options.Create(ipRateLimitOptions);

            var loggerMock = new Mock<ILogger<PxWeb.Controllers.Api2.TableApiController>>();

           


            //GetIOptions
            IOptions<PxFileConfigurationOptions> pxFileConfigurationOptions = GetIOptions<PxFileConfigurationOptions>(configuration, "DataSource:PX");
            IPxFileConfigurationService pxFileConfigurationService = new PxFileConfigurationService(pxFileConfigurationOptions);


            var mockIPxCache = new Mock<IPxCache>();
            var hostingEnvironmentMock = new Mock<IPxHost>();
            var wwwrootPath = Util.GetFullPathToFile(@"PxWeb/wwwroot/");
            hostingEnvironmentMock
                .Setup(m => m.RootPath)
                .Returns(wwwrootPath);

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

           
            IOptions<LuceneConfigurationOptions> luceneConfigurationOptions = GetIOptions<LuceneConfigurationOptions>(configuration, "LuceneConfiguration");
            ILuceneConfigurationService luceneConfigurationService = new LuceneConfigurationService(luceneConfigurationOptions, hostingEnvironmentMock.Object);

            ISearchBackend backend = new LuceneBackend(luceneConfigurationService);

            ITablesResponseMapper tablesResponseMapper = new TablesResponseMapper(linkCreator, pxApiConfigurationOptions);
            ITableResponseMapper tableResponseMapper = new TableResponseMapper(linkCreator, pxApiConfigurationOptions);
            ICodelistResponseMapper codelistResponseMapper = new CodelistResponseMapper(linkCreator, pxApiConfigurationOptions);
            ISerializeManager serializeManager = new SerializeManager();
            ISelectionHandler selectionHandler = new SelectionHandler(pxApiConfigurationService);

            PxWeb.Controllers.Api2.TableApiController tac = new PxWeb.Controllers.Api2.TableApiController(iDataSource, languageHelper, responseMapper,
                                                                    backend, pxApiConfigurationOptions, tablesResponseMapper, tableResponseMapper,
                                                                    codelistResponseMapper, serializeManager, selectionHandler);


            var result = tac.ListAllTables(null,null,null,null,null,null);
            var aa = result.ToJSON(false);
            var bb = "{\"Value\":{\"language\":\"en\",\"tables\":[{\"updated\":\"2023-05-25T13:42:00\",\"firstPeriod\":\"1990\",\"lastPeriod\":\"2017\",\"category\":\"public\",\"variableNames\":[\"sector\",\"greenhouse gas\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB004/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB004\",\"label\":\"Total air emissions by sector, greenhouse gas, contents and year\",\"description\":\"\"},{\"updated\":\"2023-08-31T13:42:00\",\"firstPeriod\":\"2010\",\"lastPeriod\":\"2015\",\"category\":\"public\",\"variableNames\":[\"region\",\"land use\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB005?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB005/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB005/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB005\",\"label\":\"Land use in Sweden, hectares by region, land use, contents and year\",\"description\":\"\"},{\"updated\":\"2023-05-25T14:13:00\",\"firstPeriod\":\"2010\",\"lastPeriod\":\"2016\",\"category\":\"public\",\"variableNames\":[\"treatment category\",\"waste category\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB003?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB003/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB003/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB003\",\"label\":\"Disposal facilities by treatment category, waste category, contents and year\",\"description\":\"\"},{\"updated\":\"2023-05-25T13:42:00\",\"firstPeriod\":\"1981\",\"lastPeriod\":\"2001\",\"category\":\"public\",\"variableNames\":[\"region\",\"sex\",\"age\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB001?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB001/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB001/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB001\",\"label\":\"Population by region, sex, age, contents and year\",\"description\":\"\"},{\"updated\":\"2023-05-25T13:42:00\",\"firstPeriod\":\"1991\",\"lastPeriod\":\"2001\",\"category\":\"public\",\"variableNames\":[\"region\",\"sex\",\"age\",\"citizenship\",\"contents\",\"year\"],\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB002?lang=en\"},{\"rel\":\"metadata\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB002/metadata?lang=en\"},{\"rel\":\"data\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/TAB002/data?lang=en\"}],\"type\":\"Table\",\"id\":\"TAB002\",\"label\":\"Population by region, sex, age, citizenship, contents and year\",\"description\":\"\"}],\"page\":{\"pageNumber\":1,\"pageSize\":20,\"totalElements\":5,\"totalPages\":1,\"links\":[]},\"links\":[{\"rel\":\"self\",\"hreflang\":\"en\",\"href\":\"https://www.pxapi.com/api/v2/tables/?lang=en&pagesize=20&pageNumber=1\"}]},\"Formatters\":[],\"ContentTypes\":[],\"StatusCode\":200}";


            Assert.AreEqual(bb, aa);
            /*
            loggerMock.Verify( x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<System.Exception>(),
                    (Func<It.IsAnyType, System.Exception?, string>)It.IsAny<object>()),
                    Times.AtLeastOnce);
            */

        }







        //From AI :-)
        private static IOptions<TOptions> GetIOptions<TOptions>(IConfigurationRoot configuration, string section)
             where TOptions : class, new() // This ensures TOptions is a class and has a parameterless constructor
        {
            TOptions options = new TOptions();
            configuration.GetSection(section).Bind(options);
            IOptions<TOptions> myOptions = Options.Create(options);
            return myOptions;
        }
    }
}
