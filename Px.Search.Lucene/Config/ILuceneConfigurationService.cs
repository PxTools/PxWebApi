namespace Px.Search.Lucene.Config
{
    public interface ILuceneConfigurationService
    {
        LuceneConfigurationOptions GetConfiguration();
        string GetIndexDirectoryPath();

        /// <summary>
        /// Should Lucene use the StandardAnalyzer, or the language dependent ones. Defaults to false.
        /// </summary>
        bool GetUseStandardAnalyzer();
    }
}
