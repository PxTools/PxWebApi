using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Px.Abstractions.Interfaces;
using Px.Search;

using PxWeb.Code;
using PxWeb.Code.Api2.Cache;
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
        private readonly IPxCache _pxCache;

        public SearchindexController(BackgroundWorkerQueue backgroundWorkerQueue, IControllerStateProvider stateProvider, IDataSource dataSource, ISearchBackend backend, IPxApiConfigurationService pxApiConfigurationService, ILogger<SearchindexController> logger, IPxCache pxCache)
        {
            _dataSource = dataSource;
            _backend = backend;
            _pxApiConfigurationService = pxApiConfigurationService;
            _logger = logger;
            _backgroundWorkerQueue = backgroundWorkerQueue;
            _pxCache = pxCache;
            System.Type tempType = GetType();
            string id = (tempType.FullName != null) ? tempType.FullName : "GetType_has_no_FullName";
            _responseState = stateProvider.Load(id);
        }

        /// <summary>
        /// Index the whole database in all languages
        /// </summary>
        /// <param name="pastHours">Incrementally index tables published in pastHours.</param>
        /// <param name="updateBreadcrumbInfo">This is ignored when pastHours is null.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/admin/searchindex")]
        [SwaggerOperation("IndexDatabase")]
        [SwaggerResponse(statusCode: 202, description: "Accepted")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public IActionResult IndexDatabase(int? pastHours, bool updateBreadcrumbInfo = false)
        {
            _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
            {
                if (pastHours is not null)
                {
                    try
                    {
                        DateTime to = DateTime.Now;
                        DateTime from = to - TimeSpan.FromHours(pastHours.Value);
                        List<string> tableList = _dataSource.GetTablesPublishedBetween(from, to);

                        string message = $"Looked for tables published between {from:yyyy-MM-dd HH:mm:ss} and {to:yyyy-MM-dd HH:mm:ss}. Found {tableList.Count()}";

                        _responseState.AddEvent(new Event("Information", message));
                        _logger.LogUpdatedTableBetween(from, to, tableList.Count);
                        if (tableList.Count > 0)
                        {
                            await UpdateFromTableList(tableList, updateBreadcrumbInfo, token);
                            _pxCache.Clear();
                            _logger.LogCacheCleared();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        _responseState.AddEvent(new Event("Error", ex.Message));
                        _logger.LogFaildToIndexDatabase(ex);
                    }
                }
                else
                {
                    try
                    {
                        List<string> languages = GetLangaugesFromConfig();
                        if (languages.Count == 0)
                        {
                            return;
                        }

                        Indexer indexer = new Indexer(_dataSource, _backend, _logger);
                        await Task.Run(() => indexer.IndexDatabase(languages), token);
                        _pxCache.Clear();
                        _logger.LogCacheCleared();
                    }
                    catch (System.Exception ex)
                    {
                        _responseState.AddEvent(new Event("Error", ex.Message));
                        _logger.LogFaildToIndexDatabase(ex);
                    }
                }
            });
            return new AcceptedResult();
        }

        /// <summary>
        /// Update index for the specified tables
        /// </summary>
        /// <param name="tables">List of tableid</param>
        /// <param name="updateBreadcrumbInfo">Set this to true if you want to traverse the db to find the paths.</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("/admin/searchindex")]
        [SwaggerOperation("IndexDatabase")]
        [SwaggerResponse(statusCode: 202, description: "Accepted")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public IActionResult IndexDatabase([FromBody, Required] string[] tables, bool updateBreadcrumbInfo = false)
        {
            _backgroundWorkerQueue.QueueBackgroundWorkItem(async token =>
            {
                try
                {
                    List<string> tableList = tables
                        .Select(table => Regex.Replace(table.Trim(), @"[^0-9a-zA-Z]+", "", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                        .ToList();

                    await UpdateFromTableList(tableList, updateBreadcrumbInfo, token);
                    _pxCache.Clear();
                    _logger.LogCacheCleared();
                }
                catch (System.Exception ex)
                {
                    _responseState.AddEvent(new Event("Error", ex.Message));
                    _logger.LogFaildToIndexDatabase(ex);
                }


            });
            return new AcceptedResult();
        }

        private async Task UpdateFromTableList(List<string> tableList, bool updateBreadcrumbInfo, CancellationToken token)
        {

            if (tableList.Count == 0 || (tableList.Count == 1 && string.IsNullOrEmpty(tableList[0])))
            {
                string message = "Incoming list with table id's to be updated is empty. Index will not be updated.";
                _responseState.AddEvent(new Event("Information", message));
                _logger.LogNoTablesIndexWillNotUpdate();
                return;
            }


            List<string> languages = GetLangaugesFromConfig();
            if (languages.Count == 0)
            {
                return;
            }

            Indexer indexer = new Indexer(_dataSource, _backend, _logger);
            await Task.Run(() => indexer.UpdateTableEntries(tableList, languages, updateBreadcrumbInfo), token);
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

        private List<string> GetLangaugesFromConfig()
        {
            List<string> languages = new List<string>();
            var config = _pxApiConfigurationService.GetConfiguration();

            if (config.Languages.Count == 0)
            {
                string message = "No languages configured. Index will not be updated.";
                _responseState.AddEvent(new Event("Error", message));
                _logger.LogNoLanguageIndexWillNotUpdate();
                return languages;
            }

            foreach (var lang in config.Languages)
            {
                languages.Add(lang.Id);
            }
            return languages;
        }
    }
}
