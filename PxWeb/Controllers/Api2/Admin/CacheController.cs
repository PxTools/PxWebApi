using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PxWeb.Code;
using PxWeb.Code.Api2.Cache;

namespace PxWeb.Controllers.Api2.Admin
{
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IPxCache _pxCache;
        private readonly ILogger<CacheController> _logger;


        public CacheController(IPxCache pxCache, ILogger<CacheController> logger)
        {
            _logger = logger;
            _pxCache = pxCache;
        }

        [HttpDelete]
        [Route("/admin/cache")]
        public IActionResult Clear()
        {
            _pxCache.Clear();
            _logger.LogCacheCleared();
            return Ok();
        }
    }
}
