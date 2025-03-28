using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PxWeb.Api2.Server.Models;
using PxWeb.Code.Api2;
using PxWeb.Code.Api2.SavedQueryBackend;
using PxWeb.Code.Api2.Serialization;
using PxWeb.Helper.Api2;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class SavedQueryApiController : PxWeb.Api2.Server.Controllers.SavedQueriesApiController
    {
        private readonly PxApiConfigurationOptions _configOptions;
        private readonly ISavedQueryBackendProxy _savedQueryBackendProxy;
        private readonly ISerializeManager _serializeManager;
        private readonly IDataWorkflow _dataWorkflow;

        public SavedQueryApiController(IDataWorkflow dataWorkflow, ISavedQueryBackendProxy savedQueryStorageBackend, ISerializeManager serializeManager, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _dataWorkflow = dataWorkflow;
            _savedQueryBackendProxy = savedQueryStorageBackend;
            _serializeManager = serializeManager;
            _configOptions = configOptions.Value;
        }

        public override IActionResult CreateSaveQuery([FromBody] SavedQuery? savedQuery)
        {
            //TOOD: Implement
            Problem? problem;

            if (savedQuery is null)
            {
                //TODO Fix error message
                return BadRequest("The request body is empty.");
            }

            // Create a copy of the selection to be able to expand it
            var variablesSelection = SelectionUtil.Copy(savedQuery.Selection);

            _dataWorkflow.Run(savedQuery.TableId, savedQuery.Language, variablesSelection, out problem);

            string id;
            try
            {
                // 2. Save the SavedQuery to the database/file.
                id = _savedQueryBackendProxy.Save(savedQuery);
            }
            catch (Exception e)
            {
                // If error return 400 Bad Request
                //TODO: Fix error response 
                return BadRequest(e.Message);
            }

            // 3. Return the SavedQuery with the id set.
            savedQuery.Id = id;
            return Created(savedQuery.Id, savedQuery);

        }

        public override IActionResult GetSaveQuery([FromRoute(Name = "id"), Required] string id)
        {
            if (id.Contains("..") || id.Contains("/") || id.Contains("\\"))
            {
                // TODO: Fix error message
                return BadRequest("");
            }
            var savedQuery = _savedQueryBackendProxy.Load(id);
            if (savedQuery is null)
            {
                //TODO: Fix error message
                return NotFound();
            }
            return Ok(savedQuery);
        }

        public override IActionResult RunSaveQuery([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang, [FromQuery(Name = "outputFormat")] OutputFormatType? outputFormat, [FromQuery(Name = "outputFormatParams")] List<OutputFormatParamType>? outputFormatParams)
        {
            Problem? problem;

            // 0. Validate the input
            if (id.Contains("..") || id.Contains("/") || id.Contains("\\"))
            {
                // TODO: Fix error message
                return BadRequest("");
            }

            // 1. Get the SavedQuery from the database/file.
            var savedQuery = _savedQueryBackendProxy.Load(id);
            if (savedQuery is null)
            {
                // 2. If the SavedQuery is not found return 404 Not Found
                //TODO: Fix error message
                return NotFound();
            }

            // 3. Override parameters to the SavedQuery
            bool paramError;
            string outputFormatStr;
            List<string> outputFormatParamsStr;

            (outputFormatStr, outputFormatParamsStr) = OutputParameterUtil.TranslateOutputParamters(outputFormat ?? savedQuery.OutputFormat, _configOptions.DefaultOutputFormat, outputFormatParams ?? savedQuery.OutputFormatParams, out paramError);

            if (paramError)
            {
                return BadRequest(ProblemUtility.UnsupportedOutputFormat());
            }

            // 4. Run the SavedQuery
            var model = _dataWorkflow.Run(savedQuery.TableId, savedQuery.Language, savedQuery.Selection, out problem);

            if (model is null)
            {
                return BadRequest(problem);
            }

            // 5. Return the result
            var serializationInfo = _serializeManager.GetSerializer(outputFormatStr, model.Meta.CodePage, outputFormatParamsStr);
            Response.ContentType = serializationInfo.ContentType;
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{model.Meta.Matrix}{serializationInfo.Suffix}\"");
            serializationInfo.Serializer.Serialize(model, Response.Body);
            return Ok();
        }

    }
}
