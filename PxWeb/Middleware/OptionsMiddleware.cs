using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace PxWeb.Middleware
{
    public class OptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public OptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            return BeginInvoke(context);
        }

        private async Task BeginInvoke(HttpContext context)
        {
            if (context.Request.Method == "OPTIONS")
            {

                context.Response.OnStarting(() =>
                {
                    int responseStatusCode = context.Response.StatusCode;
                    if (responseStatusCode == (int)HttpStatusCode.MethodNotAllowed)
                    {
                        IHeaderDictionary headers = context.Response.Headers;
                        StringValues allowHeaderValue = string.Empty;
                        if (headers.TryGetValue("Allow", out allowHeaderValue))
                        {
                            context.Response.Headers.Remove("Allow");
                            context.Response.Headers.Append("Allow", allowHeaderValue + ", OPTIONS");
                        }
                        context.Response.StatusCode = 200;
                    }
                    return Task.FromResult(0);
                });

            }
            await _next.Invoke(context);
        }
    }

    public static class OptionsMiddlewareExtensions
    {
        public static IApplicationBuilder UseOptions(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OptionsMiddleware>();
        }
    }
}
