using Microsoft.AspNetCore.Http;

namespace PxWeb.Code
{
    internal static class HttpContextLoggingExtensions
    {
        public static void AddLoggingContext(this HttpContext context, string? tableId, string? format, int? matrixSize)
        {
            context.Items["PX_TableId"] = tableId;
            context.Items["PX_Format"] = format;
            context.Items["PX_Matrix_Size"] = matrixSize;
        }
    }
}
