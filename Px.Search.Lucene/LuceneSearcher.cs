

using System.Text.Json;

namespace Px.Search.Lucene
{
    public class LuceneSearcher : ISearcher
    {
        private readonly IndexSearcher _indexSearcher;
        private static readonly Operator _defaultOperator = Operator.AND;
        // Keyword fields must be queried as complete, normalized terms.
        private static readonly HashSet<string> _keywordFields = new()
        {
            SearchConstants.SEARCH_FIELD_TAGS,
            SearchConstants.SEARCH_FIELD_MATRIX,
            SearchConstants.SEARCH_FIELD_CODES,
            SearchConstants.SEARCH_FIELD_GROUPINGCODES,
            SearchConstants.SEARCH_FIELD_VALUESETCODES,
            SearchConstants.SEARCH_FIELD_LEVEL_CODE,
            SearchConstants.SEARCH_FIELD_META_ID,
            SearchConstants.SEARCH_FIELD_LEGACY_ID
        };
        private readonly Analyzer _analyzer;
        private readonly string[] _fieldNames;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="indexDirectory">Index directory</param>
        public LuceneSearcher(string indexDirectory, string language, string[] fieldNames)
        {
            if (string.IsNullOrWhiteSpace(indexDirectory))
            {
                throw new ArgumentNullException(indexDirectory, "Index directory not defined for Lucene");
            }

            FSDirectory fsDir = FSDirectory.Open(Path.Combine(indexDirectory, language));

            IndexReader reader = DirectoryReader.Open(fsDir);
            _indexSearcher = new IndexSearcher(reader);
            _analyzer = LuceneAnalyzer.GetAnalyzer(language);
            _fieldNames = fieldNames;
        }


        /// <summary>
        /// Search the right index depending on the language and give back a search result
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pastdays"></param>
        /// <param name="includediscontinued"></param>
        /// <returns></returns>
        public SearchResultContainer Find(string? query, int pageSize, int pageNumber, int? pastdays, bool includediscontinued = false)
        {
            var skipRecords = pageSize * (pageNumber - 1);
            var searchResultContainer = new SearchResultContainer();
            var searchResultList = new List<SearchResult>();
            BooleanFilter filter = new BooleanFilter();
            Query luceneQuery = BuildQuery(query);

            if (!string.IsNullOrEmpty(pastdays.ToString()))
            {
                var pastDay = DateTools.DateToString(DateTime.Now.AddDays(-Convert.ToDouble(pastdays)), DateResolution.HOUR);
                filter.Add(new FilterClause(FieldCacheRangeFilter.NewStringRange(SearchConstants.SEARCH_FIELD_UPDATED, lowerVal: pastDay, includeLower: true, upperVal: null, includeUpper: false), Occur.MUST));
            }

            if (!includediscontinued)
            {
                var areaFilter = new TermsFilter(new Term(SearchConstants.SEARCH_FIELD_DISCONTINUED, "true"));
                filter.Add(new FilterClause(areaFilter, Occur.MUST_NOT));
            }

            TopDocs topDocs;
            topDocs = filter.Any() ? _indexSearcher.Search(luceneQuery, filter, skipRecords + pageSize)
                : _indexSearcher.Search(luceneQuery, skipRecords + pageSize);

            ScoreDoc[] scoreDocs = topDocs.ScoreDocs;
            var pages = (double)topDocs.TotalHits / (double)pageSize;
            int totalpages;

            totalpages = Convert.ToInt32(Math.Ceiling(pages));

            if (totalpages < pageNumber && totalpages != 0)
            {
                searchResultContainer.outOfRange = true;
                return searchResultContainer;
            }

            for (int i = skipRecords; i < topDocs.TotalHits; i++)
            {
                if (i > (skipRecords + pageSize) - 1)
                {
                    break;
                }
                Document doc = _indexSearcher.Doc(scoreDocs[i].Doc);
                searchResultList.Add(GetSearchResult(doc));
            }

            searchResultContainer.pageNumber = pageNumber;
            searchResultContainer.pageSize = pageSize;
            searchResultContainer.totalPages = totalpages;
            searchResultContainer.totalElements = topDocs.TotalHits;
            searchResultContainer.searchResults = searchResultList;

            return searchResultContainer;

        }

        private Query BuildQuery(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new MatchAllDocsQuery();
            }

            // Text fields use language analysis; keyword fields need exact matching.
            string[] textFields = _fieldNames
                .Where(field => !_keywordFields.Contains(field))
                .ToArray();
            string[] keywordFields = _fieldNames
                .Where(field => _keywordFields.Contains(field))
                .ToArray();

            var queries = new BooleanQuery();

            if (textFields.Length > 0)
            {
                // Boost title matches while keeping the other text fields searchable.
                var boosts = textFields.ToDictionary(
                    field => field,
                    field => field == SearchConstants.SEARCH_FIELD_TITLE ? 3f : 1f);
                var textParser = new MultiFieldQueryParser(
                    LuceneAnalyzer.luceneVersion,
                    textFields,
                    _analyzer,
                    boosts)
                {
                    DefaultOperator = _defaultOperator
                };

                queries.Add(textParser.Parse(query), Occur.SHOULD);
            }

            string keywordValue = query.Trim().ToLowerInvariant();
            foreach (string field in keywordFields)
            {
                // Keep stopwords and punctuation meaningful for keyword fields.
                queries.Add(new TermQuery(new Term(field, keywordValue)), Occur.SHOULD);
            }

            return queries;
        }

        /// <summary>
        /// Search in index on table id
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public SearchResult? FindTable(string tableId)
        {

            string[] field = new[] { SearchConstants.SEARCH_FIELD_SEARCHID };
            LuceneVersion luceneVersion = LuceneAnalyzer.luceneVersion;
            Query luceneQuery;
            QueryParser queryParser = new MultiFieldQueryParser(luceneVersion,
                                                       field,
                                                       new StandardAnalyzer(luceneVersion));
            luceneQuery = queryParser.Parse(tableId);

            TopDocs topDocs = _indexSearcher.Search(luceneQuery, 1);
            if (topDocs.TotalHits == 0)
            {
                return null;
            }

            Document doc = _indexSearcher.Doc(topDocs.ScoreDocs[0].Doc);
            return GetSearchResult(doc);

        }

        /// <summary>
        /// Get SearchResultList
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static SearchResult GetSearchResult(Document doc)
        {
            DateTime updated;
            bool discontinued;

            var searchResult = new SearchResult(
                    doc.Get(SearchConstants.SEARCH_FIELD_DOCID),
                    doc.Get(SearchConstants.SEARCH_FIELD_TITLE),
                    doc.Get(SearchConstants.SEARCH_FIELD_CATEGORY),
                    doc.Get(SearchConstants.SEARCH_FIELD_FIRSTPERIOD),
                    doc.Get(SearchConstants.SEARCH_FIELD_LASTPERIOD),
                    doc.Get(SearchConstants.SEARCH_FIELD_VARIABLES).Split("|"),
                    doc.Get(SearchConstants.SEARCH_FIELD_SOURCE),
                    doc.Get(SearchConstants.SEARCH_FIELD_TIME_UNIT),
                    doc.Get(SearchConstants.SEARCH_SUBJECT_CODE)

                );
            searchResult.Description = doc.Get(SearchConstants.SEARCH_FIELD_DESCRIPTION);
            searchResult.SortCode = doc.Get(SearchConstants.SEARCH_FIELD_SORTCODE);
            if (DateTime.TryParse(doc.Get(SearchConstants.SEARCH_FIELD_UPDATED), out updated))
            {
                searchResult.Updated = updated;
            }
            if (bool.TryParse(doc.Get(SearchConstants.SEARCH_FIELD_DISCONTINUED), out discontinued))
            {
                searchResult.Discontinued = discontinued;
            }
            searchResult.Category = doc.Get(SearchConstants.SEARCH_FIELD_CATEGORY);
            searchResult.FirstPeriod = doc.Get(SearchConstants.SEARCH_FIELD_FIRSTPERIOD);
            searchResult.LastPeriod = doc.Get(SearchConstants.SEARCH_FIELD_LASTPERIOD);
            searchResult.Tags = doc.Get(SearchConstants.SEARCH_FIELD_TAGS).Split(" ");
            searchResult.Updated = String.IsNullOrEmpty(doc.Get(SearchConstants.SEARCH_FIELD_UPDATED)) ? null : DateTools.StringToDate(doc.Get(SearchConstants.SEARCH_FIELD_UPDATED));
            searchResult.Label = doc.Get(SearchConstants.SEARCH_FIELD_TITLE);

            searchResult.Paths = JsonSerializer.Deserialize<List<Level[]>>(doc.GetBinaryValue(SearchConstants.SEARCH_FIELD_PATHS).Bytes) ?? new List<Level[]>();
            searchResult.Languages = doc.Get(SearchConstants.SEARCH_AVAILABLE_LANGUAGES).Split("|", StringSplitOptions.RemoveEmptyEntries);

            return searchResult;
        }
    }
}
