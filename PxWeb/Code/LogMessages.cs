using Microsoft.Extensions.Logging;

namespace PxWeb.Code
{
    internal static partial class LogMessages
    {
        [LoggerMessage(
            Message = "Can not parse rate limitin section in config file. Using default values.",
            Level = LogLevel.Warning,
            SkipEnabledCheck = true)]
        internal static partial void LogInvalidRateLimitingConfiguration(
            this ILogger logger,
            Exception ex);


        [LoggerMessage(
            Message = "Processing a reqest in {methodName} caused and unhandle error.",
            Level = LogLevel.Warning,
            SkipEnabledCheck = true)]
        internal static partial void LogInternalErrorWhenProcessingRequest(
            this ILogger logger,
            string methodName,
            Exception ex);
    }

    internal static partial class LogMessages
    {

    }
}
