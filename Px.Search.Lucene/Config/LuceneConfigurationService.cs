namespace Px.Search.Lucene.Config
{
    public class LuceneConfigurationService : ILuceneConfigurationService
    {
        private readonly LuceneConfigurationOptions _configOptions;
        private readonly IPxHost _hostingEnvironment;

        public LuceneConfigurationService(IOptions<LuceneConfigurationOptions> configOptions, IPxHost hostingEnvironment)
        {
            _configOptions = configOptions.Value;
            _hostingEnvironment = hostingEnvironment;
        }
        public LuceneConfigurationOptions GetConfiguration()
        {
            return _configOptions;
        }

        /// <summary>
        /// Get path to the specified index directory 
        /// </summary>
        /// <returns>Physical path to Lucene index directory</returns>
        public string GetIndexDirectoryPath()
        {
            var luceneOptions = GetConfiguration();

            if (string.IsNullOrWhiteSpace(luceneOptions.IndexDirectory))
            {
                // If the IndexDirectory is not set, use a default path
                return "Database/_INDEX/";
            }

            string path = luceneOptions.IndexDirectory;

            string indexDirectory;
            if (Path.IsPathFullyQualified(path))
            {
                indexDirectory = path;
            }
            else
            {
                indexDirectory = Path.Combine(_hostingEnvironment.RootPath, path);
            }

            return indexDirectory;
        }

        public string[] GetSearchFields()
        {
            var luceneOptions = GetConfiguration();
            if (luceneOptions.SearchFields == null || luceneOptions.SearchFields.Length == 0)
            {
                return
                [
                  "docid",
                  "searchid",
                  "updated",
                  "matrix",
                  "title",
                  "description",
                  "sortcode",
                  "category",
                  "firstperiod",
                  "lastperiod",
                  "variables",
                  "period",
                  "values",
                  "codes",
                  "groupings",
                  "groupingcodes",
                  "valuesets",
                  "valuesetcodes",
                  "discontinued",
                  "tags"
                ];
            }
            return luceneOptions.SearchFields;
        }
    }

}
