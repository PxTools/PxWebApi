using System.IO;
using System.IO.Hashing;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using PxWeb.Code;
using PxWeb.Code.Api2.Cache;

namespace PxWeb.Middleware
{
    public class CacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly object _cacheLock = new();
        readonly CacheMiddlewareConfigurationOptions _configuration;
        private readonly TimeSpan _cacheTime;
        private readonly ILogger<CacheMiddleware> _logger;

        public CacheMiddleware(RequestDelegate next, ICacheMiddlewareConfigurationService cacheMiddlewareConfigurationService, ILogger<CacheMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _configuration = cacheMiddlewareConfigurationService.GetConfiguration();
            _cacheTime = TimeSpan.FromSeconds(_configuration.CacheTime);
        }
        private async Task<CachedResponse> readResponse(HttpContext httpContext)
        {
            using (var ms = new MemoryStream())
            {
                Stream originalStream = httpContext.Response.Body;
                httpContext.Response.Body = ms;
                await _next(httpContext);

                ms.Seek(0, SeekOrigin.Begin);
                byte[] body = ms.ToArray();
                ms.Seek(0, SeekOrigin.Begin);

                httpContext.Response.Body = originalStream;

                string? contentType = httpContext.Response.ContentType;
                int responseCode = httpContext.Response.StatusCode;


                string contentDisposition = httpContext.Response.Headers.ContentDisposition.ToString();
                CachedResponse response = new CachedResponse(body, contentType, responseCode, contentDisposition);
                return response;
            }
        }

        private string generateKey(HttpRequest request, string body)
        {
            var hasher = new XxHash128();
            hasher.Append(Encoding.UTF8.GetBytes(request.Method));
            hasher.Append(Encoding.UTF8.GetBytes(request.Path));
            hasher.Append(Encoding.UTF8.GetBytes(request.QueryString.Value ?? ""));

            if (request.Method == "POST" && body != "")
            {
                hasher.Append(Encoding.UTF8.GetBytes(body));
            }


            Span<byte> hashBytes = stackalloc byte[16]; // 128 bitar = 16 byte
            hasher.GetCurrentHash(hashBytes);

            // Konvertera till hex-sträng
            string key = Convert.ToHexString(hashBytes);

            return key;

        }

        public async Task Invoke(HttpContext httpContext, IPxCache cache)
        {
            HttpRequest request = httpContext.Request;

            request.EnableBuffering(_configuration.BufferThreshold);

            string body = await new StreamReader(request.Body).ReadToEndAsync();

            request.Body.Seek(0, SeekOrigin.Begin);

            string key = generateKey(request, body);

            CachedResponse response;
            CachedResponse? cached = cache.Get<CachedResponse>(key);
            if (cached is null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogCacheMiss();
                }

                response = readResponse(httpContext).Result;

                lock (_cacheLock)
                {
                    CachedResponse? freshCached = cache.Get<CachedResponse>(key);
                    if (freshCached is null)
                    {
                        cache.Set(key, response, _cacheTime);
                    }
                    else
                    {
                        response = freshCached;
                    }
                }
            }
            else
            {
                response = cached;
            }

            httpContext.Response.ContentType = response.ContentType;
            httpContext.Response.StatusCode = response.ResponseCode;
            if (!string.IsNullOrEmpty(response.ContentDisposition))
            {
                httpContext.Response.Headers.Append("Content-Disposition", response.ContentDisposition);
            }

            await httpContext.Response.Body.WriteAsync(response.Content);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCacheMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheMiddleware>();
        }
    }
}
