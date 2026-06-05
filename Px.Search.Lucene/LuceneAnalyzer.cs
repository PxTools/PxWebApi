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


        public const string SEARCH_FIELD_DOCID = "docid";
        public const string SEARCH_FIELD_SEARCHID = "searchid";
        public const string SEARCH_FIELD_UPDATED = "updated";
        public const string SEARCH_FIELD_DISCONTINUED = "discontinued";
        public const string SEARCH_FIELD_TAGS = "tags";
        public const string SEARCH_FIELD_MATRIX = "matrix";
        public const string SEARCH_FIELD_TITLE = "title";
        public const string SEARCH_FIELD_DESCRIPTION = "description";
        public const string SEARCH_FIELD_SORTCODE = "sortcode";
        public const string SEARCH_FIELD_CATEGORY = "category";
        public const string SEARCH_FIELD_FIRSTPERIOD = "firstperiod";
        public const string SEARCH_FIELD_LASTPERIOD = "lastperiod";
        public const string SEARCH_FIELD_VARIABLES = "variables";
        public const string SEARCH_FIELD_PERIOD = "period";
        public const string SEARCH_FIELD_VALUES = "values";
        public const string SEARCH_FIELD_CODES = "codes";
        public const string SEARCH_FIELD_GROUPINGS = "groupings";
        public const string SEARCH_FIELD_GROUPINGCODES = "groupingcodes";
        public const string SEARCH_FIELD_VALUESETS = "valuesets";
        public const string SEARCH_FIELD_VALUESETCODES = "valuesetcodes";
        public const string SEARCH_FIELD_SYNONYMS = "synonyms";
        public const string SEARCH_FIELD_SOURCE = "source";
        public const string SEARCH_FIELD_TIME_UNIT = "timeunit";
        public const string SEARCH_FIELD_PATHS = "paths";
        public const string SEARCH_FIELD_LEVEL_CODE = "levelcode";
        public const string SEARCH_FIELD_LEVEL_NAME = "levelname";
        public const string SEARCH_FIELD_META_ID = "metaid";
        public const string SEARCH_FIELD_LEGACY_ID = "legacyid";
        public const string SEARCH_SUBJECT_CODE = "subjectcode";
        public const string SEARCH_AVAILABLE_LANGUAGES = "languages";

    }
    // TODO ? Should read langs from config and prepare a static analyzerByLanguage dictionary? 

    // depricated: Analyzer analyzer = new SnowballAnalyzer(LuceneVersion.LUCENE_48, "English");
}

