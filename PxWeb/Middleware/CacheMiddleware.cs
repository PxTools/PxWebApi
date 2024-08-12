﻿using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using PxWeb.Code.Api2.Cache;

namespace PxWeb.Middleware
{
    public class CacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _cacheLock = "lock";
        readonly CacheMiddlewareConfigurationOptions _configuration;
        private readonly TimeSpan _cacheTime;

        public CacheMiddleware(RequestDelegate next, ICacheMiddlewareConfigurationService cacheMiddlewareConfigurationService)
        {
            _next = next;
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
                CachedResponse response = new CachedResponse(body, contentType, responseCode);
                return response;
            }
        }

        private string generateKey(HttpRequest request, string body)
        {
            // Get url
            string url = $"{request.Method}:{request.Scheme}://{request.Host.Value}{request.Path}{request.QueryString}";
            string key = $"{url}";
            if (request.Method == "POST" && body != "")
            {
                key += $":{body}";
            }
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

            httpContext.Response.ContentType = response.contentType;
            httpContext.Response.StatusCode = response.responseCode;

            await httpContext.Response.Body.WriteAsync(response.content);
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
