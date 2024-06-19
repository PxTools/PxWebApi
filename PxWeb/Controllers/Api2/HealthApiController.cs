using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PxWeb.Models.Api2;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class HealthApiController : ControllerBase
    {
        private readonly ILogger<HealthApiController> _logger;
        private readonly string _alivePath;
        private readonly IApplicationState _applicationState;

        public HealthApiController(IWebHostEnvironment env, ILogger<HealthApiController> logger, IApplicationState applicationState)
        {
            _logger = logger;
            string root = env.ContentRootPath;
            string healthPath = Path.Combine(root, "wwwroot", "Health");
            _alivePath = Path.Combine(healthPath, "Alive", "alive.json");
            _applicationState = applicationState;
        }


        [HttpGet]
        [Route("/health/alive")]
        public IActionResult IsAlive()
        {
            try
            {
                string aliveBody = System.IO.File.ReadAllText(_alivePath);
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
                bool isReady = !_applicationState.MarkedForShutdown;
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
