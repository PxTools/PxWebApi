using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class SavedQueryApiController : PxWeb.Api2.Server.Controllers.SavedQueriesApiController
    {

        public SavedQueryApiController()
        {

        }

        public override IActionResult CreateSaveQuery([FromBody] SavedQuery? savedQuery)
        {
            //TOOD: Implement
            // 1. Make sure that the SavedQuery is ok and that it results in a valid output.
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
