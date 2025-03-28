using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using PCAxis.Paxiom;
using PCAxis.Paxiom.Operations;

using Px.Abstractions.Interfaces;

using PxWeb.Api2.Server.Models;
using PxWeb.Code.Api2.DataSelection;
using PxWeb.Code.Api2.SavedQueryBackend;
using PxWeb.Code.Api2.Serialization;
using PxWeb.Helper.Api2;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class SavedQueryApiController : PxWeb.Api2.Server.Controllers.SavedQueriesApiController
    {
        private readonly PxApiConfigurationOptions _configOptions;
        private readonly ISelectionHandler _selectionHandler;
        private readonly IDataSource _dataSource;
        private readonly ISavedQueryBackendProxy _savedQueryBackendProxy;
        private readonly ISerializeManager _serializeManager;

        public SavedQueryApiController(IDataSource dataSource, ISelectionHandler selectionHandler, ISavedQueryBackendProxy savedQueryStorageBackend, ISerializeManager serializeManager, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _dataSource = dataSource;
            _selectionHandler = selectionHandler;
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

            // 1. Make sure that the SavedQuery is ok and that it results in a valid output.
            var builder = _dataSource.CreateBuilder(savedQuery.TableId, savedQuery.Language);

            if (builder == null)
            {
                return NotFound(ProblemUtility.NonExistentTable());
            }
            builder.BuildForSelection();

            Selection[]? selection = null;
            VariablePlacementType? placment = null;

            if (variablesSelection is not null)
            {
                if (!_selectionHandler.ExpandAndVerfiySelections(variablesSelection, builder, out problem))
                {
                    return BadRequest(problem);
                }

                selection = _selectionHandler.Convert(variablesSelection);

                if (problem is not null)
                {
                    return BadRequest(problem);
                }

                builder.BuildForPresentation(selection);

                var model = builder.Model;

                placment = savedQuery.Selection.Placement;

                if (placment is not null)
                {
                    var descriptions = new List<PivotDescription>();

                    descriptions.AddRange(placment.Heading.Select(h => new PivotDescription()
                    {
                        VariableName = model.Meta.Variables.First(v => v.Code.Equals(h, StringComparison.OrdinalIgnoreCase)).Name,
                        VariablePlacement = PlacementType.Heading
                    }));

                    descriptions.AddRange(placment.Stub.Select(h => new PivotDescription()
                    {
                        VariableName = model.Meta.Variables.First(v => v.Code == h).Name,
                        VariablePlacement = PlacementType.Stub
                    }));

                    var pivot = new PCAxis.Paxiom.Operations.Pivot();
                    model = pivot.Execute(model, descriptions.ToArray());
                }
            }
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

            // 3. Apply parameters to the SavedQuery
            if (outputFormat is not null)
            {
                savedQuery.OutputFormat = outputFormat.Value;
            }
            if (outputFormatParams is not null)
            {
                savedQuery.OutputFormatParams = outputFormatParams;
            }

            // 4. Run the SavedQuery
            var builder = _dataSource.CreateBuilder(savedQuery.TableId, savedQuery.Language);

            if (builder == null)
            {
                return NotFound(ProblemUtility.NonExistentTable());
            }
            builder.BuildForSelection();

            Selection[]? selection = null;
            VariablePlacementType? placment = null;

            var variablesSelection = savedQuery.Selection;


            if (!_selectionHandler.ExpandAndVerfiySelections(variablesSelection, builder, out problem))
            {
                return BadRequest(problem);
            }

            selection = _selectionHandler.Convert(variablesSelection);

            if (problem is not null)
            {
                return BadRequest(problem);
            }

            builder.BuildForPresentation(selection);

            var model = builder.Model;

            placment = savedQuery.Selection.Placement;

            if (placment is not null)
            {
                var descriptions = new List<PivotDescription>();

                descriptions.AddRange(placment.Heading.Select(h => new PivotDescription()
                {
                    VariableName = model.Meta.Variables.First(v => v.Code.Equals(h, StringComparison.OrdinalIgnoreCase)).Name,
                    VariablePlacement = PlacementType.Heading
                }));

                descriptions.AddRange(placment.Stub.Select(h => new PivotDescription()
                {
                    VariableName = model.Meta.Variables.First(v => v.Code == h).Name,
                    VariablePlacement = PlacementType.Stub
                }));

                var pivot = new PCAxis.Paxiom.Operations.Pivot();
                model = pivot.Execute(model, descriptions.ToArray());
            }

            // 5. Return the result

            bool paramError;
            string outputFormatStr;
            List<string> outputFormatParamsStr;

            (outputFormatStr, outputFormatParamsStr) = OutputParameterUtil.TranslateOutputParamters(outputFormat, _configOptions.DefaultOutputFormat, outputFormatParams, out paramError);

            var serializationInfo = _serializeManager.GetSerializer(outputFormatStr, model.Meta.CodePage, outputFormatParamsStr);

            Response.ContentType = serializationInfo.ContentType;
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{model.Meta.Matrix}{serializationInfo.Suffix}\"");
            serializationInfo.Serializer.Serialize(model, Response.Body);
            return Ok();
        }

    }
}
