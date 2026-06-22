using Lucene.Net.Analysis.Core;

namespace Px.Search.Lucene
{
    internal class CaseInsensitiveKeywordAnalyzer : Analyzer
    {
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            // Keeps the text as a single token (no word splitting)
            Tokenizer source = new KeywordTokenizer(reader);

            // Converts that single token to lowercase
            TokenStream filter = new LowerCaseFilter(LuceneVersion.LUCENE_48, source);

            return new TokenStreamComponents(source, filter);
        }
    }
}
