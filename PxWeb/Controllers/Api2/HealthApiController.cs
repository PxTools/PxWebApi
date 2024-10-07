using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PxWeb.Models.Api2;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class HealthApiController : ControllerBase
    {
        private readonly ILogger<HealthApiController> _logger;
        private readonly IApplicationState _applicationState;

        public HealthApiController(ILogger<HealthApiController> logger, IApplicationState applicationState)
        {
            _logger = logger;
            _applicationState = applicationState;
        }


        [HttpGet]
        [Route("/health/alive")]
        public IActionResult IsAlive()
        {
            try
            {
                string aliveBody = "yes";
                return new ObjectResult(aliveBody);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "IsAlive() caused an exception");
                return StatusCode(503, "I'm dead");
            }
        }

        [HttpGet]
        [Route("/health/ready")]
        public IActionResult Isready()
        {
            try
            {
                bool isReady = !_applicationState.InMaintanceMode;
                if (isReady)
                {
                    //TODO chech extenal dependencies

                    return new ObjectResult("I'm ready");
                }

                return StatusCode(503, "I'm not ready");
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Isready() caused an exception");
                return StatusCode(503, "I'm not ready");
            }
        }

    }
}
