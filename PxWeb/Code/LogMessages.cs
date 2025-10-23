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
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogSavedQueryCreated(
            this ILogger logger,
            string queryId);

        [LoggerMessage(
            Message = "Possible injection attack on parmater {parameter}",
            Level = LogLevel.Warning,
            SkipEnabledCheck = true)]
        internal static partial void LogInjectionInParmater(
            this ILogger logger,
            string parameter);

        [LoggerMessage(
            Message = "Saved query with given id can not be found.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogNoSavedQueryWithGivenId(
            this ILogger logger);

        [LoggerMessage(
            Message = "Saved query with id {queryId} did not ran sucessfully.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogFaultySavedQuery(
            this ILogger logger,
            string queryId);

        [LoggerMessage(
            Message = "Output format {outputFormat} our one of its parameter is not supported.",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogUnsupportedOutputFormat(
            this ILogger logger,
            string outputFormat);

        [LoggerMessage(
            Message = "Fetching default selection by using saved query with id {savedQueryId}.",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogDataExctractionBySavedQuery(
            this ILogger logger,
            string savedQueryId);

        [LoggerMessage(
            Message = "Fetching default selection by using configured algorithm.",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogDataExctractionByAlgorithm(
            this ILogger logger);

        [LoggerMessage(
            Message = "Fetching using selection.",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogDataExctractionBySelection(
            this ILogger logger);

        [LoggerMessage(
            Message = "One or more parameters given by the user where incorrect. Results in bad request.",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogParameterError(
            this ILogger logger);

        [LoggerMessage(
            Message = "No table with given id was found.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogNoTableWithGivenId(
            this ILogger logger);

        [LoggerMessage(
            Message = "No table with given id was found in the search index.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogNoTableWithGivenIdInSearchIndex(
            this ILogger logger);

        [LoggerMessage(
            Message = "No codelist with given id  was found.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogNoCodelistWithGivenId(
            this ILogger logger);

        [LoggerMessage(
            Message = "Page selection is out of range. Page number {pageNumber}, page size {pageSize}",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogPageOutOfRange(
            this ILogger logger,
            int pageNumber,
            int pageSize);

        [LoggerMessage(
            Message = "Could not run workflow sucessfully",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogCouldNotRunWorkflowSucessfully(
            this ILogger logger);


        [LoggerMessage(
            Message = "Cache miss",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogCacheMiss(
            this ILogger logger);



        [LoggerMessage(
            Message = "Unauthorized call from Ip {ip}",
            Level = LogLevel.Warning,
            SkipEnabledCheck = false)]
        internal static partial void LogUnuthorizedCallFromIp(
            this ILogger logger,
            string ip);

        [LoggerMessage(
            Message = "Cached cleared",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogCacheCleared(
            this ILogger logger);


        [LoggerMessage(
            Message = "Menu2.xml file created",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogDatabaseGenerated(
            this ILogger logger);

        [LoggerMessage(
            Message = "Failed to generate Menu2.xml file",
            Level = LogLevel.Warning,
            SkipEnabledCheck = false)]
        internal static partial void LogFaildToGenerateDatabase(
            this ILogger logger,
            Exception ex);


        [LoggerMessage(
            Message = "Failed to generate serach index for database",
            Level = LogLevel.Warning,
            SkipEnabledCheck = false)]
        internal static partial void LogFaildToIndexDatabase(
            this ILogger logger,
            Exception ex);

        [LoggerMessage(
            Message = "Looked for tables published between {from} and {to}. Found {count} number of tables.",
            Level = LogLevel.Debug,
            SkipEnabledCheck = false)]
        internal static partial void LogUpdatedTableBetween(
            this ILogger logger,
            DateTime from,
            DateTime to,
            int count);

        [LoggerMessage(
            Message = "The list of tables to updated was empty, will not try to update index",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogNoTablesIndexWillNotUpdate(
           this ILogger logger);

        [LoggerMessage(
            Message = "No languages configured. Index will not be updated.",
            Level = LogLevel.Warning,
            SkipEnabledCheck = false)]
        internal static partial void LogNoLanguageIndexWillNotUpdate(
           this ILogger logger);
    }

    internal static partial class LogMessages
    {

    }
}
