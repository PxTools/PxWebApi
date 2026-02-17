using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using PCAxis.Paxiom;

using Px.Abstractions.Interfaces;
using Px.Search;

using PxWeb.Api2.Server.Models;
using PxWeb.Code;
using PxWeb.Code.Api2;
using PxWeb.Code.Api2.DataSelection;
using PxWeb.Code.Api2.ModelBinder;
using PxWeb.Code.Api2.SavedQueryBackend;
using PxWeb.Code.Api2.Serialization;
using PxWeb.Helper.Api2;
using PxWeb.Mappers;

namespace PxWeb.Controllers.Api2
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class TablesApiController : PxWeb.Api2.Server.Controllers.TablesApiController
    {
        private readonly IDataSource _dataSource;
        private readonly ILanguageHelper _languageHelper;
        private readonly IDatasetMapper _datasetMapper;
        private readonly ITablesResponseMapper _tablesResponseMapper;
        private readonly ITableResponseMapper _tableResponseMapper;

        private readonly ISearchBackend _backend;
        private readonly ISerializeManager _serializeManager;
        private readonly PxApiConfigurationOptions _configOptions;
        private readonly ISelectionHandler _selectionHandler;
        private readonly ISelectionResponseMapper _selectionResponseMapper;
        private readonly IDefaultSelectionAlgorithm _defaultSelectionAlgorithm;
        private readonly IDataWorkflow _dataWorkflow;
        private readonly ISavedQueryBackendProxy _savedQueryBackendProxy;
        private readonly ILogger<TablesApiController> _logger;

        public TablesApiController(IDataSource dataSource, ILanguageHelper languageHelper, IDatasetMapper datasetMapper, ISearchBackend backend, IOptions<PxApiConfigurationOptions> configOptions, ITablesResponseMapper tablesResponseMapper, ITableResponseMapper tableResponseMapper, ICodelistResponseMapper codelistResponseMapper, ISelectionResponseMapper selectionResponseMapper, ISerializeManager serializeManager, ISelectionHandler selectionHandler, IDefaultSelectionAlgorithm defaultSelectionAlgorithm, IDataWorkflow dataWorkflow, ISavedQueryBackendProxy savedQueryBackendProxy, ILogger<TablesApiController> logger)
        {
            _logger = logger;
            _dataSource = dataSource;
            _languageHelper = languageHelper;
            _datasetMapper = datasetMapper;
            _backend = backend;
            _configOptions = configOptions.Value;
            _tablesResponseMapper = tablesResponseMapper;
            _tableResponseMapper = tableResponseMapper;
            _serializeManager = serializeManager;
            _selectionHandler = selectionHandler;
            _selectionResponseMapper = selectionResponseMapper;
            _defaultSelectionAlgorithm = defaultSelectionAlgorithm;
            _dataWorkflow = dataWorkflow;
            _savedQueryBackendProxy = savedQueryBackendProxy;
        }

        public override IActionResult GetMetadataById([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang, [FromQuery(Name = "defaultSelection")] bool? defaultSelection, [FromQuery(Name = "savedQuery")] string? savedQueryId, [FromQuery(Name = "codelist")] Dictionary<string, string>? codelist)
        {
            lang = _languageHelper.HandleLanguage(lang);
            IPXModelBuilder? builder = _dataSource.CreateBuilder(id, lang);



            if (builder != null)
            {
                try
                {
                    builder.BuildForSelection();

                    if (savedQueryId is not null && savedQueryId != string.Empty)
                    {
                        //apply the saved query
                        var savedQuery = _savedQueryBackendProxy.Load(savedQueryId);
                        if (savedQuery is not null)
                        {
                            _logger.LogDataExctractionBySavedQuery(savedQuery.Id ?? "Unknown");
                            _selectionHandler.ExpandAndVerfiySelections(savedQuery.Selection, builder, out Problem? problem);
                        }
                        else
                        {
                            _logger.LogNoSavedQueryWithGivenId();
                            return NotFound(ProblemUtility.NonExistentSavedQuery());
                        }
                    }
                    else if (defaultSelection is not null && defaultSelection == true)
                    {
                        //apply the default selection
                        var savedQuery = _savedQueryBackendProxy.LoadDefaultSelection(id);
                        if (savedQuery is not null)
                        {
                            _logger.LogDataExctractionBySavedQuery(savedQuery.Id ?? "Unknown");
                            _selectionHandler.ExpandAndVerfiySelections(savedQuery.Selection, builder, out Problem? problem);
                        }
                        else
                        {
                            _logger.LogDataExctractionByAlgorithm();
                            _defaultSelectionAlgorithm.GetDefaultSelection(builder);
                        }
                    }
                    else if (codelist is not null && codelist.Keys.Count > 0) //Check that we have codelist specified
                    {
                        _logger.LogDataExctractionBySelection();

                        var selections = SelectionUtil.CreateVariablesSelectionFromCodelists(codelist);

                        if (!_selectionHandler.FixVariableRefsAndApplyCodelists(builder, selections, out Problem? problem))
                        {
                            _logger.LogParameterError();
                            return BadRequest(problem);
                        }
                    }

                    var model = builder.Model;

                    Searcher searcher = new Searcher(_dataSource, _backend);
                    SearchResult? searchResult = searcher.FindTable(id, lang);
                    if (searchResult != null)
                    {
                        model.Meta.Title = searchResult.Label;
                    }

                    Dataset ds = _datasetMapper.Map(model, id, lang);
                    return new ObjectResult(ds);

                }
                catch (Exception ex)
                {
                    _logger.LogInternalErrorWhenProcessingRequest("GetMetadataById", ex);
                    return NotFound(ProblemUtility.NonExistentTable());
                }
            }
            else
            {
                _logger.LogNoTableWithGivenId();
                return NotFound(ProblemUtility.NonExistentTable());
            }
        }

        public override IActionResult GetTableById([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang)
        {
            Searcher searcher = new Searcher(_dataSource, _backend);
            lang = _languageHelper.HandleLanguage(lang);

            if (!_dataSource.TableExists(id, lang))
            {
                _logger.LogNoTableWithGivenId();
                return NotFound(ProblemUtility.NonExistentTable());
            }

            SearchResult? searchResult = searcher.FindTable(id, lang);
            if (searchResult == null)
            {
                _logger.LogNoTableWithGivenIdInSearchIndex();
                return NotFound(ProblemUtility.NonExistentTable());
            }

            return Ok(_tableResponseMapper.Map(searchResult, lang));


        }



        public override IActionResult ListAllTables([FromQuery(Name = "lang")] string? lang, [FromQuery(Name = "query")] string? query, [FromQuery(Name = "pastDays")] int? pastDays, [FromQuery(Name = "includeDiscontinued")] bool? includeDiscontinued, [FromQuery(Name = "pageNumber")] int? pageNumber, [FromQuery(Name = "pageSize")] int? pageSize)
        {
            Searcher searcher = new Searcher(_dataSource, _backend);

            lang = _languageHelper.HandleLanguage(lang);

            if (pageNumber == null || pageNumber <= 0)
                pageNumber = 1;

            if (pageSize == null || pageSize <= 0)
                pageSize = _configOptions.PageSize;

            var searchResultContainer = searcher.Find(query, lang, pastDays, includeDiscontinued ?? false, pageSize.Value, pageNumber.Value);

            if (searchResultContainer.outOfRange == true)
            {
                _logger.LogPageOutOfRange(pageNumber.Value, pageSize.Value);
                return NotFound(ProblemUtility.OutOfRange());
            }

            return Ok(_tablesResponseMapper.Map(searchResultContainer, lang, query, pastDays));

        }

        public override IActionResult GetTableData(
            [FromRoute(Name = "id"), Required] string id,
            [FromQuery(Name = "lang")] string? lang,
            [FromQuery(Name = "valuecodes"), ModelBinder(typeof(QueryStringToDictionaryOfStrings))] Dictionary<string, List<string>>? valuecodes,
            [FromQuery(Name = "codelist")] Dictionary<string, string>? codelist,
            [FromQuery(Name = "outputFormat")] OutputFormatType? outputFormat,
            [FromQuery(Name = "outputFormatParams"), ModelBinder(typeof(OutputFormatParamsModelBinder))] List<OutputFormatParamType>? outputFormatParams,
            [FromQuery(Name = "heading"), ModelBinder(typeof(CommaSeparatedStringToListOfStrings))] List<string>? heading,
            [FromQuery(Name = "stub"), ModelBinder(typeof(CommaSeparatedStringToListOfStrings))] List<string>? stub)
        {
            VariablesSelection variablesSelection = MapDataParameters(valuecodes, codelist, heading, stub);
            return GetData(id, lang, variablesSelection, outputFormat, outputFormatParams is null ? new List<OutputFormatParamType>() : outputFormatParams);
        }

        public override IActionResult GetTableDataByPost(
            [FromRoute(Name = "id"), Required] string id,
            [FromQuery(Name = "lang")] string? lang,
            [FromQuery(Name = "outputFormat")] OutputFormatType? outputFormat,
            [FromQuery(Name = "outputFormatParams"), ModelBinder(typeof(OutputFormatParamsModelBinder))] List<OutputFormatParamType>? outputFormatParams,
            [FromBody] VariablesSelection? variablesSelection)
        {
            return GetData(id, lang, variablesSelection, outputFormat, outputFormatParams is null ? new List<OutputFormatParamType>() : outputFormatParams);
        }

        private IActionResult GetData(string id, string? lang, VariablesSelection? variablesSelection, OutputFormatType? outputFormat, List<OutputFormatParamType> outputFormatParams)
        {

            Problem? problem = null;

            lang = _languageHelper.HandleLanguage(lang);

            bool paramError;
            string outputFormatStr;
            List<string> outputFormatParamsStr;

            (outputFormatStr, outputFormatParamsStr) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions, outputFormatParams, out paramError);

            if (paramError)
            {
                _logger.LogUnsupportedOutputFormat(outputFormatStr);
                return BadRequest(ProblemUtility.UnsupportedOutputFormat());
            }

            PXModel? model;

            if (SelectionUtil.UseDefaultSelection(variablesSelection))
            {
                var savedQuery = _savedQueryBackendProxy.LoadDefaultSelection(id);
                if (savedQuery is not null)
                {
                    _logger.LogDataExctractionBySavedQuery(savedQuery.Id ?? "Unknown");
                    variablesSelection = savedQuery.Selection;
                    model = _dataWorkflow.Run(id, lang, variablesSelection, out problem);
                }
                else
                {
                    _logger.LogDataExctractionByAlgorithm();
                    model = _dataWorkflow.Run(id, lang, out problem);
                }
            }
            else
            {
                _logger.LogDataExctractionBySelection();
                model = _dataWorkflow.Run(id, lang, variablesSelection, out problem);
            }

            if (model is null)
            {
                _logger.LogCouldNotRunWorkflowSucessfully();
                return BadRequest(problem);
            }

            var serializationInfo = _serializeManager.GetSerializer(outputFormatStr, model.Meta.CodePage, outputFormatParamsStr);

            Response.ContentType = serializationInfo.ContentType;
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{model.Meta.Matrix}{serializationInfo.Suffix}\"");
            serializationInfo.Serializer.Serialize(model, Response.Body);

            HttpContext.AddLoggingContext(id, outputFormatStr, model.Data.MatrixSize);

            return Ok();
        }




        /// <summary>
        /// Map querystring parameters to VariablesSelection object
        /// </summary>
        /// <param name="valuecodes"></param>
        /// <param name="codelist"></param>
        /// <param name="outputvalues"></param>
        /// <param name="heading"></param>
        /// <param name="stub"></param> 
        /// <returns></returns>
        private VariablesSelection MapDataParameters(Dictionary<string, List<string>>? valuecodes, Dictionary<string, string>? codelist, List<string>? heading, List<string>? stub)
        {
            VariablesSelection selections = new VariablesSelection();
            if (valuecodes != null)
            {
                selections.Selection = new List<VariableSelection>();
                foreach (var variableCode in valuecodes.Keys)
                {
                    VariableSelection variableSelection = new VariableSelection();
                    variableSelection.VariableCode = variableCode;
                    variableSelection.ValueCodes = valuecodes[variableCode];
                    if (codelist != null && codelist.ContainsKey(variableCode))
                    {
                        variableSelection.Codelist = codelist[variableCode];
                    }
                    selections.Selection.Add(variableSelection);
                }
            }

            selections.Placement = new VariablePlacementType();
            selections.Placement.Heading = heading ?? new List<string>();
            selections.Placement.Stub = stub ?? new List<string>();

            return selections;
        }



        public override IActionResult GetDefaultSelection([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang)
        {
            lang = _languageHelper.HandleLanguage(lang);

            VariablesSelection selection;

            //Check if we have a saved query that should serv as default selection
            var savedQuery = _savedQueryBackendProxy.LoadDefaultSelection(id);
            if (savedQuery is not null)
            {
                _logger.LogDataExctractionBySavedQuery(savedQuery.Id ?? "Unknown");
                selection = savedQuery.Selection;

                var builder = _dataSource.CreateBuilder(id, lang);
                if (builder == null)
                {
                    _logger.LogNoTableWithGivenId();
                    return NotFound(ProblemUtility.NonExistentTable());
                }

                builder.BuildForSelection();

                if (!_selectionHandler.ExpandAndVerfiySelections(selection, builder, out Problem? problem))
                {
                    _logger.LogParameterError();
                    return BadRequest(problem);
                }
            }
            else //Fallback to the default selection algorithm
            {

                _logger.LogDataExctractionByAlgorithm();

                var builder = _dataSource.CreateBuilder(id, lang);
                if (builder == null)
                {
                    _logger.LogNoTableWithGivenId();
                    return NotFound(ProblemUtility.NonExistentTable());
                }

                builder.BuildForSelection();

                //No variable selection is provided, so we will return the default selection
                selection = _defaultSelectionAlgorithm.GetDefaultSelection(builder);
                if (!_selectionHandler.ExpandAndVerfiySelections(selection, builder, out Problem? problem))
                {
                    _logger.LogParameterError();
                    return BadRequest(problem);
                }

            }

            //Map selection to SelectionResponse
            SelectionResponse selectionResponse = _selectionResponseMapper.Map(selection, id, lang, false);
            return Ok(selectionResponse);
        }

    }

}
