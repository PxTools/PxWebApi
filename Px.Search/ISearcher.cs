namespace Px.Search
{
    /// <summary>
    /// ISearcher
    /// </summary>
    public interface ISearcher
    {
        SearchResultContainer Find(string? query, int pageSize, int pageNumber, int? pastdays, bool includediscontinued = false);

        /// <summary>
        /// Gets zero or one table from search index
        /// </summary>
        /// <param name="tableId">Id of table from url</param>
        /// <returns>The entry for the table or null if it cant be found</returns>
        SearchResult? FindTable(string tableId);
    }
}
