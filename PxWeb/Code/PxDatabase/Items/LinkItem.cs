namespace PXWeb.Database
{
    public class LinkItem
    {
        public LinkItem(string text, string location, string language)
        {
            Text = text;
            Location = location;
            Language = language;
        }

        public string Text { get; set; }
        public string Location { get; set; }
        public string Language { get; set; }
    }
}
