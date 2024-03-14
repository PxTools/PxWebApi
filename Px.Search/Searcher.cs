﻿namespace Px.Search
{
    /// <summary>
    /// Not sure if this adds value?   
    /// </summary>
    public class Searcher
    {
        private readonly ISearchBackend _backend;
        //private ILogger _logger;

        public Searcher(IDataSource dataSource, ISearchBackend backend)
        {
            //TODO remove unused param dataSource
            _backend = backend;
        }
        public SearchResultContainer Find(string? query, string language, int? pastdays, bool includediscontinued, int pageSize = 20, int pageNumber = 1)
        {
            var searcher = _backend.GetSearcher(language);

            return searcher.Find(query, pageSize, pageNumber, pastdays, includediscontinued);
        }
        public SearchResult FindTable(string tableId, string language)
        {
            var searcher = _backend.GetSearcher(language);

            return searcher.FindTable(tableId);
        }

    }
}
