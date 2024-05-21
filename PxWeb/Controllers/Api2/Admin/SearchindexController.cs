using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Px.Abstractions.Interfaces;
using Px.Search;

using PxWeb.Code.BackgroundWorker;

using Swashbuckle.AspNetCore.Annotations;

namespace PxWeb.Controllers.Api2.Admin
{
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class SearchindexController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly ISearchBackend _backend;
        private readonly IPxApiConfigurationService _pxApiConfigurationService;
        private readonly ILogger<SearchindexController> _logger;
        private readonly BackgroundWorkerQueue _backgroundWorkerQueue;
        private readonly IControllerState _responseState;

        public SearchindexController(BackgroundWorkerQueue backgroundWorkerQueue, IControllerStateProvider stateProvider, IDataSource dataSource, ISearchBackend backend, IPxApiConfigurationService pxApiConfigurationService, ILogger<SearchindexController> logger)
        {
            _dataSource = dataSource;
            _backend = backend;
            _pxApiConfigurationService = pxApiConfigurationService;
            _logger = logger;
            _backgroundWorkerQueue = backgroundWorkerQueue;
            System.Type tempType = GetType();
            string id = (tempType.FullName != null) ? tempType.FullName : "GetType_has_no_FullName";
            _responseState = stateProvider.Load(id);
        }

        /// <summary>
        /// Index the whole database in all languages
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/admin/searchindex")]
        [SwaggerOperation("IndexDatabase")]
        [SwaggerResponse(statusCode: 202, description: "Accepted")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public IActionResult IndexDatabase()
        {
            _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    List<string> languages = new List<string>();

                    var config = _pxApiConfigurationService.GetConfiguration();

                    if (config.Languages.Count == 0)
                    {
                        _logger.LogError("No languages configured for PxApi");
                        return;
                    }
                    foreach (var lang in config.Languages)
                    {
                        languages.Add(lang.Id);
                    }

                    Indexer indexer = new Indexer(_dataSource, _backend, _logger);
                    await Task.Run(() => indexer.IndexDatabase(languages), token);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Error when building serach index");
                }
            });
            return new AcceptedResult();
        }

        /// <summary>
        /// Update index for the specified tables
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/admin/searchindex")]
        [SwaggerOperation("IndexDatabase")]
        [SwaggerResponse(statusCode: 202, description: "Accepted")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public IActionResult IndexDatabase([FromBody, Required] string[] tables)
        {
            _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    List<string> languages = new List<string>();
                    List<string> tableList = tables
                        .Select(table => Regex.Replace(table.Trim(), @"[^0-9a-zA-Z]+", ""))
                        .ToList();

                    if (tableList.Count == 0)
                    {
                        string message = "No languages configured for PxApi";
                        _logger.LogError(message);
                        _responseState.AddEvent(new Event("Error", message));
                        return;
                    }

                    var config = _pxApiConfigurationService.GetConfiguration();

                    if (config.Languages.Count == 0)
                    {
                        string message = "No languages configured for PxApi";
                        _logger.LogError(message);
                        _responseState.AddEvent(new Event("Error", message));
                        return;
                    }

                    foreach (var lang in config.Languages)
                    {
                        languages.Add(lang.Id);
                    }

                    Indexer indexer = new Indexer(_dataSource, _backend, _logger);
                    await Task.Run(() => indexer.UpdateTableEntries(tableList, languages), token);
                }
                catch (System.Exception ex)
                {
                    _responseState.AddEvent(new Event("Error", ex.Message));
                    _logger.LogError(ex.Message);
                }
            });
            return new AcceptedResult();
        }

        [HttpGet]
        [Route("/admin/searchindex")]
        [SwaggerOperation("IndexDatabase")]
        [SwaggerResponse(statusCode: 200, description: "Success")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public IActionResult GetState()
        {
            return new JsonResult(_responseState.Data);
        }
    }
}
