namespace Px.Search
{
    /// <summary>
    /// SearchEngineReader
    /// </summary>
    public interface ISearcher
    {
        SearchResultContainer Find(string? query, int pageSize, int pageNumber, int? pastdays, bool includediscontinued = false);
        SearchResult FindTable(string tableId);
    }
}
