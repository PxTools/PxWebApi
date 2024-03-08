namespace Px.Search
{
    /// <summary>
    /// SearchEngine
    /// </summary>
    public interface ISearchBackend
    {
        IIndex GetIndex();
        ISearcher GetSearcher(string language);
    }
}
