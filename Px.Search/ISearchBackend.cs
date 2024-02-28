namespace Px.Search
{
    public interface ISearchBackend
    {
        IIndex GetIndex();
        ISearcher GetSearcher(string language);
    }
}
