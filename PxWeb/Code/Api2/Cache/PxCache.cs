using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace PxWeb.Code.Api2.Cache
{
    /// <summary>
    /// Implementation class for the API cache
    /// </summary>
    public class PxCache : IPxCache
    {
        /// <summary>
        /// Delegate that waits for something after the cache has been cleaned
        /// </summary>
        public delegate bool CacheReenabler();
        private Func<bool>? _coherenceChecker;

        private readonly ILogger<PxCache> _logger;
        private readonly string _cacheLock = "lock";
        private readonly MemoryCache _cache;
        private bool _enableCache;
        private readonly TimeSpan _cacheTime;

        public PxCache(ILogger<PxCache> logger, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _logger = logger;
            _cache = new MemoryCache(new MemoryCacheOptions());
            _enableCache = true;
            _cacheTime = TimeSpan.FromSeconds(configOptions.Value.CacheTime);
        }

        public PxCache(ILogger<PxCache> logger)
        {
            _logger = logger;
            _cache = new MemoryCache(new MemoryCacheOptions());
            _enableCache = true;
            _cacheTime = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Fetches a cached object from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Get<T>(object key)
        {
            if (!IsEnabled())
            {
                return default;
            }

            //Check if the cache controller has set a coherence checker
            if (_coherenceChecker != null)
            {
                //Check that the cache is coherent
                if (!_coherenceChecker())
                {
                    //Clear the cache if it is not coherent
                    Clear();
                }
            }

            var value = _cache.Get(key);
            if (value is null)
            {
                return default;
            }
            return (T)value;
        }

        /// <summary>
        /// Stores a object in the cache
        /// </summary>
        /// <param name="data"></param>
        public void Set(object key, object value)
        {
            Set(key, value, _cacheTime);
        }

        /// <summary>
        /// Stores a object in the cache for a specified time
        /// </summary>
        /// <param name="data"></param>
        public void Set(object key, object value, TimeSpan lifetime)
        {
            if (_cache.Get(key) is null)
            {
                lock (_cacheLock)
                {
                    if (_cache.Get(key) is null)
                    {
                        _cache.Set(key, value, lifetime);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        private void ClearCache()
        {
            lock (_cacheLock)
            {
                _cache.Compact(1.0);
            }
        }

        public bool IsEnabled()
        {
            return _enableCache;
        }

        public void Clear()
        {
            _logger.LogInformation("Cache cleared started");
            ClearCache();
            _logger.LogInformation("Cache cleared finished");
        }

        public void Disable()
        {
            _logger.LogInformation("Cache disabled");
            _enableCache = false;
        }

        public void Enable()
        {
            _logger.LogInformation("Cache enabled");
            _enableCache = true;
        }

        public void SetCoherenceChecker(Func<bool> coherenceChecker)
        {
            _coherenceChecker = coherenceChecker;
        }
    }
}
