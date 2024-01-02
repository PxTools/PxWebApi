using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.No;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Util;
using Px.Search.Lucene.Config;

namespace Px.Search.Lucene
{
    public class LuceneBackend : ISearchBackend
    {

        private readonly ILuceneConfigurationService _luceneConfigurationService;

        internal static LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        private static bool UseStandardAnalyzer;

        public LuceneBackend(ILuceneConfigurationService luceneConfigurationService)
        {
            _luceneConfigurationService = luceneConfigurationService;
            _luceneConfigurationService.GetUseStandardAnalyzer();

            UseStandardAnalyzer = _luceneConfigurationService.GetUseStandardAnalyzer();
        }

        public IIndex GetIndex()
        {
            string path = _luceneConfigurationService.GetIndexDirectoryPath();
            return new LuceneIndex(path);
        }

        public ISearcher GetSearcher(string language)
        {
            string path = _luceneConfigurationService.GetIndexDirectoryPath();
            return new LuceneSearcher(path, language);
        }

        internal static Analyzer GetAnalyzer(string language)
        {
            // TODO ? Should read langs from config and prepare a static analyzerByLanguage dictionary? 
            // 
            if(UseStandardAnalyzer)
            {
                return new StandardAnalyzer(luceneVersion);
            }   
            else if (language.Equals("en"))
            {
                return new EnglishAnalyzer(luceneVersion);
            }
            else if (language.Equals("no"))
            {
                return new NorwegianAnalyzer(luceneVersion);
            }
            else
                return new StandardAnalyzer(luceneVersion);

            // depricated: Analyzer analyzer = new SnowballAnalyzer(LuceneVersion.LUCENE_48, "English");
        }

    }
}
