using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using PCAxis.Paxiom;
using PCAxis.Paxiom.Operations;

using Px.Abstractions.Interfaces;

using PxWeb.Api2.Server.Models;
using PxWeb.Code.Api2.DataSelection;
using PxWeb.Helper.Api2;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class SavedQueryApiController : PxWeb.Api2.Server.Controllers.SavedQueriesApiController
    {

        private readonly ISelectionHandler _selectionHandler;
        private readonly IDataSource _dataSource;

        public SavedQueryApiController(IDataSource dataSource, ISelectionHandler selectionHandler)
        {
            _dataSource = dataSource;
            _selectionHandler = selectionHandler;
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
            var variablesSelection = savedQuery.Selection;

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

            // 2. Save the SavedQuery to the database/file.
            // 3. Return the SavedQuery with the id set.
            // If error return 400 Bad Request

            throw new NotImplementedException();
        }

        public override IActionResult GetSaveQuery([FromRoute(Name = "id"), Required] string id)
        {
            //TODO: Implement
            // 1. Get the SavedQuery from the database/file.
            // 2. If the SavedQuery is not found return 404 Not Found
            // 3. Return the SavedQuery

            throw new NotImplementedException();
        }

        public override IActionResult RunSaveQuery([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "lang")] string? lang, [FromQuery(Name = "outputFormat")] OutputFormatType? outputFormat, [FromQuery(Name = "outputFormatParams")] List<OutputFormatParamType>? outputFormatParams)
        {
            //TODO: Implement
            // 1. Get the SavedQuery from the database/file.
            // 2. If the SavedQuery is not found return 404 Not Found
            // 3. Apply parameters to the SavedQuery
            // 4. Run the SavedQuery
            // 5. Return the result
            throw new NotImplementedException();
        }

    }
}
