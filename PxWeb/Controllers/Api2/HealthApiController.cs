using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PxWeb.Controllers.Api2
{
    [ApiController]
    public class HealthApiController : ControllerBase
    {
        private readonly ILogger<HealthApiController> _logger;
        private readonly string _alivePath;
        private readonly string _readyPath;

        public HealthApiController(IWebHostEnvironment env, ILogger<HealthApiController> logger)
        {
            _logger = logger;
            string root = env.ContentRootPath;
            string healthPath = Path.Combine(root, "wwwroot", "health");
            _alivePath = Path.Combine(healthPath, "alive", "alive.json");
            _readyPath = Path.Combine(healthPath, "ready", "yes.json");
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
                bool isReady = System.IO.File.Exists(_readyPath);
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
