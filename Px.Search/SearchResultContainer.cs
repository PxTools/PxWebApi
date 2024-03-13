namespace Px.Search
{

    /// <summary>
    /// Holdes a list of SearchResult and pagination info
    /// </summary>
    public class SearchResultContainer
    {
        public IEnumerable<SearchResult> searchResults = new List<SearchResult>();
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalElements { get; set; }
        public int totalPages { get; set; }
        public bool outOfRange { get; set; }





    }
}
