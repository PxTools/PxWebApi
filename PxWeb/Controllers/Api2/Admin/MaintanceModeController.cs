using Microsoft.AspNetCore.Mvc;

using PxWeb.Models.Api2;

namespace PxWeb.Controllers.Api2.Admin
{
    [ApiController]
    public class MaintanceModeController : ControllerBase
    {

        private readonly IApplicationState _applicationState;

        public MaintanceModeController(IApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        [HttpPost]
        [Route("/admin/EnterMaintanceMode")]
        public IActionResult EnterMaintanceMode()
        {
            _applicationState.InMaintanceMode = true;
            return Ok();
        }

        [HttpPost]
        [Route("/admin/ExitMaintanceMode")]
        public IActionResult ExitMaintanceMode()
        {
            _applicationState.InMaintanceMode = false;
            return Ok();
        }
    }
}
