using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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
                await _next.Invoke(context);

                if (context.Response.StatusCode == 405)
                {

                    if (context.Response.Headers.Keys.Contains("Allow"))
                    {
                        context.Response.Headers["Allow"] += ", OPTIONS";
                        context.Response.Headers.Add("Access-Control-Allow-Methods", context.Response.Headers["Allow"]);
                    }

                    context.Response.StatusCode = 200;
                }

            }
            else
            {
                await _next.Invoke(context);
            }
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
