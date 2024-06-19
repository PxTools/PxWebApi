using Microsoft.AspNetCore.Mvc;

using PxWeb.Models.Api2;

namespace PxWeb.Controllers.Api2.Admin
{
    [ApiController]
    public class MarkForShutdownController : ControllerBase
    {

        private readonly IApplicationState _applicationState;

        public MarkForShutdownController(IApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        [HttpPost]
        [Route("/admin/MarkForShutdown")]
        public IActionResult MarkForShutdown()
        {
            _applicationState.MarkedForShutdown = true;
            return Ok();
        }

        [HttpPost]
        [Route("/admin/MarkForShutdownUndo")]
        public IActionResult MarkForShutdownUndo()
        {
            _applicationState.MarkedForShutdown = false;
            return Ok();
        }
    }
}
