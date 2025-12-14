namespace Px.Search
{
    public class Level(string code, string text, string? sortCode)
    {
        public string Code { get; set; } = code;
        public string Text { get; set; } = text;
        public string? SortCode { get; set; } = sortCode;
    }
}
