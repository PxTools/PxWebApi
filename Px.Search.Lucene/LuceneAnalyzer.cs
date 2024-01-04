using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.Sv;
using Lucene.Net.Analysis.No;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace Px.Search.Lucene
{
    public class LuceneAnalyzer
    {
        internal static LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        internal static Analyzer GetAnalyzer(string language, bool useStandardAnalyzer)
        {
            // TODO ? Should read langs from config and prepare a static analyzerByLanguage dictionary? 
            // 
            if(useStandardAnalyzer)
            {
                return new StandardAnalyzer(luceneVersion);
            }   
            else if (language.Equals("en"))
            {
                return new EnglishAnalyzer(luceneVersion);
            }
            else if (language.Equals("sv"))
            {
                return new SwedishAnalyzer(luceneVersion);
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
