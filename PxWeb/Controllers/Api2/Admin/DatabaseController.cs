using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Px.Abstractions.Interfaces;

using PxWeb.Code.Api2.DataSource.PxFile;
using PxWeb.Code.BackgroundWorker;

using PXWeb.Database;

using Swashbuckle.AspNetCore.Annotations;

[assembly: InternalsVisibleTo("PxWebApi.BigTests")]

namespace PxWeb.Controllers.Api2.Admin
{
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class DatabaseController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly PxApiConfigurationOptions _configOptions;
        private readonly ILogger<DatabaseController> _logger;
        private readonly IPxHost _hostingEnvironment;
        private readonly BackgroundWorkerQueue _backgroundWorkerQueue;
        private readonly IControllerState _responseState;

        public DatabaseController(IControllerStateProvider stateProvider, BackgroundWorkerQueue backgroundWorkerQueue, IDataSource dataSource, IOptions<PxApiConfigurationOptions> configOptions, ILogger<DatabaseController> logger, IPxHost hostingEnvironment)
        {
            _dataSource = dataSource;
            _configOptions = configOptions.Value;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _backgroundWorkerQueue = backgroundWorkerQueue;
            Type tempType = GetType();
            string id = (tempType.FullName != null) ? tempType.FullName : "GetType_has_no_FullName";
            _responseState = stateProvider.Load(id);
        }

        [HttpPut]
        [Route("/api/v2/admin/database")]
        [SwaggerOperation("Database")]
        [SwaggerResponse(statusCode: 202, description: "Accepted")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 405, description: "Method Not Allowed")]
        public IActionResult Database([FromQuery(Name = "langdependent")] bool? langDependent, [FromQuery(Name = "sortorder")] string? sortOrder)
        {
            if (_dataSource.GetType() != typeof(PxFileDataSource))
            {
                return StatusCode(405, "Only possible to generate database for PX-file databases");
            }

            _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
            {
                await createMenuXml(langDependent, sortOrder, token);
            });

            return new AcceptedResult();
        }


        internal async Task createMenuXml(bool? langDependent, string? sortOrder, CancellationToken token)
        {
            try
            {
                var customCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                customCulture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
                customCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
                customCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss";

                Thread.CurrentThread.CurrentCulture = customCulture;


                PXWeb.Database.DatabaseSpider spider;
                spider = new PXWeb.Database.DatabaseSpider();

                await Task.Run(() => spider.ActivateStateLogging(_responseState), token);

                spider.Handles.Add(new AliasFileHandler(_configOptions, _logger));
                spider.Handles.Add(new LinkFileHandler(_configOptions, _logger));
                spider.Handles.Add(new PxFileHandler());
                spider.Handles.Add(new MenuSortFileHandler(_configOptions, _logger));

                List<string> langs = new List<string>();
                foreach (Language lang in _configOptions.Languages)
                {
                    langs.Add(lang.Id);
                }

                string sorting = GetSorting(sortOrder);
                string databasePath = Path.Combine(_hostingEnvironment.RootPath, "Database");

                spider.Builders.Add(new MenuBuilder(_configOptions, _logger, _hostingEnvironment, langs.ToArray(), GetLangDependent(langDependent)) { SortOrder = GetSortOrder(sorting) });
                await Task.Run(() => spider.Search(databasePath), token);
            }
            catch (System.Exception ex)
            {
                _responseState.AddEvent(new Event("Error", ex.Message));
                _logger.LogError(ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/v2/admin/database")]
        [SwaggerOperation("Database")]
        [SwaggerResponse(statusCode: 200, description: "Success")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public IActionResult GetState()
        {
            return new JsonResult(_responseState.Data);
        }

        private static bool GetLangDependent(bool? langDependent)
        {
            if (langDependent == null)
            {
                return false;
            }
            else
            {
                return (bool)langDependent;
            }
        }

        private static string GetSorting(string? sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                return "matrix";
            }

            switch (sortOrder.ToLower())
            {
                case "matrix":
                    return "matrix";
                case "title":
                    return "title";
                case "filename":
                    return "filename";
                default:
                    return "matrix";
            }
        }

        private static Func<PCAxis.Paxiom.PXMeta, string, string> GetSortOrder(string sortOrder)
        {
            switch (sortOrder.ToLower())
            {
                case "matrix":
                    return (meta, path) => meta.Matrix;
                case "title":
                    return (meta, path) => !string.IsNullOrEmpty(meta.Description) ? meta.Description : meta.Title;
                case "filename":
                    return (meta, path) => System.IO.Path.GetFileNameWithoutExtension(path);
                default:
                    break;
            }
            return (meta, path) => path;
        }
    }
}
