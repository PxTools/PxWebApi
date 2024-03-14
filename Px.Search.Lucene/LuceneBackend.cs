namespace Px.Search.Lucene
{
    public class LuceneBackend : ISearchBackend
    {

        private readonly ILuceneConfigurationService _luceneConfigurationService;

        private readonly string _path;

        public LuceneBackend(ILuceneConfigurationService luceneConfigurationService)
        {
            _luceneConfigurationService = luceneConfigurationService;
            _path = _luceneConfigurationService.GetIndexDirectoryPath();
        }

        public IIndex GetIndex()
        {
            return new LuceneIndex(_path);
        }

        public ISearcher GetSearcher(string language)
        {
            return new LuceneSearcher(_path, language);
        }
    }
}
