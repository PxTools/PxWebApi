namespace PXWeb.Database
{

    public class AliasItem
    {
        public AliasItem(string alias, string language)
        {
            Alias = alias;
            Language = language;
        }

        public string Alias { get; set; }
        public string Language { get; set; }
    }

}
