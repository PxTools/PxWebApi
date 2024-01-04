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

        private readonly bool _useStandardAnalyzer;

        public LuceneBackend(ILuceneConfigurationService luceneConfigurationService)
        {
            _luceneConfigurationService = luceneConfigurationService;
            _path = _luceneConfigurationService.GetIndexDirectoryPath();
            _useStandardAnalyzer = _luceneConfigurationService.GetUseStandardAnalyzer();
        }

        public IIndex GetIndex()
        {
            return new LuceneIndex(_path, _useStandardAnalyzer);
        }

        public ISearcher GetSearcher(string language)
        {
            return new LuceneSearcher(_path, language, _useStandardAnalyzer);
        }
    }
}
