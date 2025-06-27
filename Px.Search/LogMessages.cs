namespace Px.Search
{
    internal static partial class LogMessages
    {
        [LoggerMessage(
            Message = "Start creating search index for language {language}",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogIndexingStarted(
            this ILogger logger,
            string language);

        [LoggerMessage(
            Message = "End search index for language {language}. Total of {count} tables indexed.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogIndexingEnded(
            this ILogger logger,
            string language,
            int count);

        [LoggerMessage(
            Message = "Could not get root level for database",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogNoRootLevel(
            this ILogger logger);

        [LoggerMessage(
            Message = "Could not create breadcrumb for table {tableId} and language {language}",
            Level = LogLevel.Warning,
            SkipEnabledCheck = false)]
        internal static partial void LogCouldNotCreateBreadcrumb(
            this ILogger logger,
            string tableId,
            string language,
            Exception ex);


        [LoggerMessage(
            Message = "Could not CreateMenu for id {level} for language {language}",
            Level = LogLevel.Warning,
            SkipEnabledCheck = false)]
        internal static partial void LogCouldNotCreateMenu(
            this ILogger logger,
            string level,
            string language,
            Exception ex);


        [LoggerMessage(
            Message = "Could not get database level with id {level} for language {language}",
            Level = LogLevel.Warning,
            SkipEnabledCheck = false)]
        internal static partial void LogLevelIsNull(
            this ILogger logger,
            string level,
            string language);


        [LoggerMessage(
            Message = "Have currently index {count} number of tables.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogProgression(
            this ILogger logger,
            int count);


        [LoggerMessage(
            Message = "Table {tableId} is have aready been index.",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogTableAlreadyIndex(
            this ILogger logger,
            string tableId);

        [LoggerMessage(
            Message = "Could not load table {tableId} for language {language}",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogCouldNotBuildTable(
            this ILogger logger,
            string tableId,
            string language,
            Exception ex);


        [LoggerMessage(
            Message = "Could not create builder for table {tableId} for language {language}",
            Level = LogLevel.Information,
            SkipEnabledCheck = false)]
        internal static partial void LogCouldNotCreateBuilder(
            this ILogger logger,
            string tableId,
            string language);

    }
}
