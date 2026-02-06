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
                throw new Exception("Index directory not configured for Lucene index");
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
    }
}
