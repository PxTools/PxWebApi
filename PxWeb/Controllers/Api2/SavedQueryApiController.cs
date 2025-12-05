using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Px.Abstractions.Interfaces;

using PxWeb.Api2.Server.Models;
using PxWeb.Code;
using PxWeb.Code.Api2;
using PxWeb.Code.Api2.DataSelection;
using PxWeb.Code.Api2.SavedQueryBackend;
using PxWeb.Code.Api2.Serialization;
using PxWeb.Helper.Api2;
using PxWeb.Mappers;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class SavedQueryApiController : PxWeb.Api2.Server.Controllers.SavedQueriesApiController
    {
        private readonly ISelectionResponseMapper _selectionResponseMapper;
        private readonly IDataSource _dataSource;
        private readonly ILanguageHelper _languageHelper;
        private readonly PxApiConfigurationOptions _configOptions;
        private readonly ISavedQueryBackendProxy _savedQueryBackendProxy;
        private readonly ISerializeManager _serializeManager;
        private readonly IDataWorkflow _dataWorkflow;
        private readonly ILogger<SavedQueryApiController> _logger;
        private readonly ISelectionHandler _selectionHandler;
        private readonly SavedQueryResponseMapper _savedQueryResponseMapper;

        public SavedQueryApiController(IDataWorkflow dataWorkflow, ISavedQueryBackendProxy savedQueryStorageBackend, ISerializeManager serializeManager, IOptions<PxApiConfigurationOptions> configOptions, ILogger<SavedQueryApiController> logger, IDataSource dataSource, ILanguageHelper languageHelper, ISelectionResponseMapper selectionResponseMapper, ISelectionHandler selectionHandler, ISavedQueryResponseMapper savedQueryResponseMapper)
        {
            _dataWorkflow = dataWorkflow;
            _savedQueryBackendProxy = savedQueryStorageBackend;
            _serializeManager = serializeManager;
            _configOptions = configOptions.Value;
            _logger = logger;
            _dataSource = dataSource;
            _languageHelper = languageHelper;
            _selectionResponseMapper = selectionResponseMapper;
            _selectionHandler = selectionHandler;
            _savedQueryResponseMapper = (SavedQueryResponseMapper)savedQueryResponseMapper;
        }

        public override IActionResult CreateSaveQuery([FromBody] SavedQuery? savedQuery)
        {
            Problem? problem;

            if (savedQuery is null)
            {
                _logger.LogNoSavedQuerySpecified();
                return BadRequest(ProblemUtility.NoQuerySpecified());
            }

            // Create a copy of the selection to be able to expand it
            var variablesSelection = SelectionUtil.Copy(savedQuery.Selection);

            _dataWorkflow.Run(savedQuery.TableId, savedQuery.Language, variablesSelection, out problem);

            if (problem is not null)
            {
                return BadRequest(problem);
            }

            string id;
            try
            {
                // 2. Save the SavedQuery to the database/file.
                id = _savedQueryBackendProxy.Save(savedQuery);
                _logger.LogSavedQueryCreated(id);
            }
            catch (Exception e)
            {
                // If error return 400 Bad Request
                _logger.LogInternalErrorWhenProcessingRequest("CreateSaveQuery", e);
                return BadRequest(ProblemUtility.InternalErrorCreateSavedQuery());
            }

            // 3. Return the SavedQuery with the id set.
            savedQuery.Id = id;
            return Created(savedQuery.Id, savedQuery);

        }

        public override IActionResult GetSaveQuery([FromRoute(Name = "id"), Required] string id)
        {
            if (id.Contains("..") || id.Contains('/') || id.Contains('\\'))
            {
                _logger.LogInjectionInParmater("id");
                return BadRequest(ProblemUtility.NonExistentSavedQuery());
            }

            var savedQuery = _savedQueryBackendProxy.Load(id);
            if (savedQuery is null)
            {
                _logger.LogNoSavedQueryWithGivenId();
                return NotFound(ProblemUtility.NonExistentSavedQuery());
            }


            return Ok(_savedQueryResponseMapper.Map(savedQuery));
        }

        public override IActionResult RunSaveQuery([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang, [FromQuery(Name = "outputFormat")] OutputFormatType? outputFormat, [FromQuery(Name = "outputFormatParams")] List<OutputFormatParamType>? outputFormatParams)
        {
            Problem? problem;

            // 0. Validate the input
            if (id.Contains("..") || id.Contains('/') || id.Contains('\\'))
            {
                _logger.LogInjectionInParmater("id");
                return BadRequest(ProblemUtility.NonExistentSavedQuery());
            }

            // 1. Get the SavedQuery from the database/file.
            var savedQuery = _savedQueryBackendProxy.Load(id);
            if (savedQuery is null)
            {
                // 2. If the SavedQuery is not found return 404 Not Found
                _logger.LogNoSavedQueryWithGivenId();
                return NotFound(ProblemUtility.NonExistentSavedQuery());
            }

            // Override the language if specified
            var language = _languageHelper.HandleLanguage(lang);
            if (!string.Equals(savedQuery.Language, language, StringComparison.OrdinalIgnoreCase))
            {
                savedQuery.Language = language;
            }

            _savedQueryBackendProxy.UpdateRunStatistics(id);

            // 3. Override parameters to the SavedQuery
            bool paramError;
            string outputFormatStr;
            List<string> outputFormatParamsStr;

            (outputFormatStr, outputFormatParamsStr) = OutputParameterUtil.TranslateOutputParamters(outputFormat ?? savedQuery.OutputFormat, _configOptions, outputFormatParams ?? savedQuery.OutputFormatParams, out paramError);

            if (paramError)
            {
                _logger.LogUnsupportedOutputFormat(outputFormatStr);
                return BadRequest(ProblemUtility.UnsupportedOutputFormat());
            }

            // 4. Run the SavedQuery
            var model = _dataWorkflow.Run(savedQuery.TableId, savedQuery.Language, savedQuery.Selection, out problem);

            if (model is null)
            {
                _logger.LogFaultySavedQuery(savedQuery.Id ?? "Unknown");
                return BadRequest(problem);
            }

            // 5. Return the result
            var serializationInfo = _serializeManager.GetSerializer(outputFormatStr, model.Meta.CodePage, outputFormatParamsStr);
            Response.ContentType = serializationInfo.ContentType;
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{model.Meta.Matrix}{serializationInfo.Suffix}\"");
            serializationInfo.Serializer.Serialize(model, Response.Body);

            HttpContext.AddLoggingContext(id, outputFormatStr, model.Data.MatrixSize);

            return Ok();
        }

        public override IActionResult GetSavedQuerySelection([FromRoute(Name = "id")][Required] string id, [FromQuery(Name = "lang")] string? lang)
        {
            lang = _languageHelper.HandleLanguage(lang);

            VariablesSelection selection;

            var savedQuery = _savedQueryBackendProxy.Load(id);
            if (savedQuery is null)
            {
                _logger.LogNoSavedQueryWithGivenId();
                return NotFound(ProblemUtility.NonExistentSavedQuery());
            }

            selection = savedQuery.Selection;

            var builder = _dataSource.CreateBuilder(savedQuery.TableId, lang);
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

            //Map selection to SelectionResponse
            SelectionResponse selectionResponse = _selectionResponseMapper.Map(selection, id, lang, true);
            return Ok(selectionResponse);
        }

    }
}
