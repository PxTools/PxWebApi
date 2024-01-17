using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.No;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Util;
using Px.Search.Lucene.Config;
using Lucene.Net.Analysis.Sv;

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
