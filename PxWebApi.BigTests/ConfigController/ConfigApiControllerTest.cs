namespace PxWebApi.BigTests.ConfigController
{
    [TestClass]
    public class ConfigApiControllerTest
    {


        [TestMethod]
        [Description("Same input-file gives same output string.")]

        public void GetApiConfiguration()
        {
            string conf_dir_String = Util.GetFullPathToFile(@"PxWebApi.BigTests/ConfigController/");
            var builder = new ConfigurationBuilder()
            .SetBasePath(conf_dir_String)
            .AddJsonFile("test_appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            IOptions<PxApiConfigurationOptions> pxApiConfigurationOptions = Util.GetIOptions<PxApiConfigurationOptions>(configuration, "PxApiConfiguration");
            IPxApiConfigurationService pxApiConfigurationService = new PxApiConfigurationService(pxApiConfigurationOptions);

            IOptions<IpRateLimitOptions> optionsRate = Util.GetIOptions<IpRateLimitOptions>(configuration, "IpRateLimiting");

            var loggerMock = new Mock<ILogger<PxWeb.Controllers.Api2.ConfigurationApiController>>();

            PxWeb.Controllers.Api2.ConfigurationApiController cac =
                new PxWeb.Controllers.Api2.ConfigurationApiController(pxApiConfigurationService, optionsRate, loggerMock.Object);

            var result = cac.GetApiConfiguration();
            var aa = result.ToJSON(false);
            var bb = "{\"Value\":{\"apiVersion\":\"2.0\",\"languages\":[{\"id\":\"en\",\"label\":\"English\"},{\"id\":\"sv\",\"label\":\"Svenska\"}],\"defaultLanguage\":\"en\",\"maxDataCells\":10000,\"maxCallsPerTimeWindow\":30,\"timeWindow\":10,\"license\":\"https://creativecommons.org/share-your-work/public-domain/cc0/\",\"sourceReferences\":[{\"language\":\"en\",\"text\":\"Source: AAAAAAAAA Statistics Sweden\"},{\"language\":\"sv\",\"text\":\"Källa: AAAAAAAAAA SCB\"}],\"defaultMetadataFormat\":0,\"defaultDataFormat\":\"px\",\"dataFormats\":[\"xlsx\",\"xlsx_doublecolumn\",\"csv\",\"csv_tab\",\"csv_tabhead\",\"csv_comma\",\"csv_commahead\",\"csv_space\",\"csv_spacehead\",\"csv_semicolon\",\"csv_semicolonhead\",\"csv2\",\"csv3\",\"json\",\"json-stat\",\"json-stat2\",\"parquet\",\"html5_table\",\"relational_table\",\"px\"],\"features\":[{\"id\":\"CORS\",\"params\":[{\"key\":\"enabled\",\"value\":\"True\"}]}]},\"Formatters\":[],\"ContentTypes\":[]}";



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

    }
}
