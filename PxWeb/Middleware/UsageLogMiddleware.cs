using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using PxWeb.Code.Api2.Cache;

namespace PxWeb.Middleware
{
    public class UsageLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UsageLogMiddleware> _logger;

        public UsageLogMiddleware(RequestDelegate next, ILogger<UsageLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IPxCache cache)
        {
            await _next(context);
            if (context.Response.StatusCode == 200 &&
                context.Items.TryGetValue("PX_TableId", out var tableId) &&
                context.Items.TryGetValue("PX_Format", out var format) &&
                context.Items.TryGetValue("PX_Matrix_Size", out var size) &&
                tableId is not null &&
                format is not null &&
                size is not null)
            {
                _logger.LogUsage((string)tableId, (string)format, (int)size);
            }
        }
    }


    public static class UseUsageLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseUsageLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UsageLogMiddleware>();
        }
    }

    internal static partial class UsageLogMessages
    {
        [LoggerMessage(
            Message = "Fetch data for tableId={tableId}, format={format}, size={size}",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogUsage(
            this ILogger logger,
            string tableId,
            string format,
            int size);
    }
}
