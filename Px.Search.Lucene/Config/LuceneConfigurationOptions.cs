namespace Px.Search.Lucene.Config
{
    public class LuceneConfigurationOptions
    {
        public string? IndexDirectory { get; set; }

        public string[] SearchFields { get; set; } = Array.Empty<string>();
    }
}
