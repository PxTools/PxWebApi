using Lucene.Net.Analysis.Miscellaneous;

namespace Px.Search.Lucene
{
    public class LuceneAnalyzer
    {
        internal static LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

        internal static Analyzer GetDefaultAnalyzer(string language)
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

        internal static Analyzer GetAnalyzer(string language)
        {
            var defaultAnalyzer = GetDefaultAnalyzer(language);
            var keywordAnalyzer = new CaseInsensitiveKeywordAnalyzer();

            return new PerFieldAnalyzerWrapper(defaultAnalyzer,
                new Dictionary<string, Analyzer>
                {
                    { SearchConstants.SEARCH_FIELD_TAGS, keywordAnalyzer },
                    { SearchConstants.SEARCH_FIELD_MATRIX, keywordAnalyzer },
                    { SearchConstants.SEARCH_FIELD_CODES, keywordAnalyzer },
                    { SearchConstants.SEARCH_FIELD_GROUPINGCODES, keywordAnalyzer },
                    { SearchConstants.SEARCH_FIELD_VALUESETCODES, keywordAnalyzer },
                    { SearchConstants.SEARCH_FIELD_LEVEL_CODE, keywordAnalyzer },
                    { SearchConstants.SEARCH_FIELD_META_ID, keywordAnalyzer },
                    { SearchConstants.SEARCH_FIELD_LEGACY_ID, keywordAnalyzer }
                });

        }

    }
}

