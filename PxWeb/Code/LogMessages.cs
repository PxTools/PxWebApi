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

        [LoggerMessage(
            Message = "No saved query specified",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogNoSavedQuerySpecified(
            this ILogger logger);

        [LoggerMessage(
            Message = "Saved query created with id {queryId}.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogSavedQueryCreated(
            this ILogger logger,
            string queryId);

        [LoggerMessage(
            Message = "Possible injection attack on parmater {parameter} with value {value}",
            Level = LogLevel.Warning,
            SkipEnabledCheck = true)]
        internal static partial void LogInjectionInParmater(
            this ILogger logger,
            string parameter,
            string value);

        [LoggerMessage(
            Message = "Saved query with id {queryId} can not be found.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogNoSavedQueryWithGivenId(
            this ILogger logger,
            string queryId);

        [LoggerMessage(
            Message = "Saved query with id {queryId} did not ran sucessfully.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogFaultySavedQuery(
            this ILogger logger,
            string queryId);

        [LoggerMessage(
            Message = "Output format {outputFormat} our one of its parameter is not supported.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogUnsupportedOutputFormat(
            this ILogger logger,
            string outputFormat);

    }

    internal static partial class LogMessages
    {

    }
}
