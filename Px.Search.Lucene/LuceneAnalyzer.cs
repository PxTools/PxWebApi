using Lucene.Net.Analysis.En;
using Lucene.Net.Analysis.Sv;
using Lucene.Net.Analysis.No;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Util;
using Lucene.Net.Analysis.Fi;
using Lucene.Net.Analysis.El;
using Lucene.Net.Analysis.Eu;
using Lucene.Net.Analysis.Es;
using Lucene.Net.Analysis.Fr;
using Lucene.Net.Analysis.Ar;
using Lucene.Net.Analysis.It;
using Lucene.Net.Analysis.Lv;
using Lucene.Net.Analysis.Id;
using Lucene.Net.Analysis.Hy;
using Lucene.Net.Analysis.De;
using Lucene.Net.Analysis.Da;

namespace Px.Search.Lucene
{
    public class LuceneAnalyzer
    {
        internal static LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        internal static Analyzer GetAnalyzer(string language)
        {
            switch (language)
            {
                case "en":
                    return new EnglishAnalyzer(luceneVersion);
                case "sv":
                    return new SwedishAnalyzer(luceneVersion);
                case "no":
                    return new NorwegianAnalyzer(luceneVersion);
                case "fi":
                    return new FinnishAnalyzer(luceneVersion);
                case "el":
                    return new GreekAnalyzer(luceneVersion);
                case "eu":
                    return new BasqueAnalyzer(luceneVersion);
                case "es":
                    return new SpanishAnalyzer(luceneVersion);
                case "fr":
                    return new FrenchAnalyzer(luceneVersion);
                case "ar":
                    return new ArabicAnalyzer(luceneVersion);
                case "it":
                    return new ItalianAnalyzer(luceneVersion);
                case "lv":
                    return new LatvianAnalyzer(luceneVersion);
                case "in":
                    return new IndonesianAnalyzer(luceneVersion);
                case "hy":
                    return new ArmenianAnalyzer(luceneVersion);
                case "de":
                    return new GermanAnalyzer(luceneVersion);
                case "da":
                    return new DanishAnalyzer(luceneVersion);
                default:
                    return new StandardAnalyzer(luceneVersion);
            }
            
        }

    }
    // TODO ? Should read langs from config and prepare a static analyzerByLanguage dictionary? 
    
    // depricated: Analyzer analyzer = new SnowballAnalyzer(LuceneVersion.LUCENE_48, "English");
}

