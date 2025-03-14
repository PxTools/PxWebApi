namespace Px.Search
{
    public class TableInformation
    {
        public TableInformation(string id, string label, string category, string firstPeriod, string lastPeriod, string[] variableNames, string source, string timeUnit)
        {
            Id = id;
            Label = label;
            Category = category;
            FirstPeriod = firstPeriod;
            LastPeriod = lastPeriod;
            VariableNames = variableNames;
            Tags = new string[] { }; // TODO: Implement later
            Paths = new List<Level[]>();
            Source = source;
            TimeUnit = timeUnit;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string? Description { get; set; }
        public string? SortCode { get; set; }
        public DateTime? Updated { get; set; }
        public bool? Discontinued { get; set; }
        public string Category { get; set; }
        public string FirstPeriod { get; set; }
        public string LastPeriod { get; set; }
        public string[] VariableNames { get; set; }
        public string[] Tags { get; set; }
        public List<Level[]> Paths { get; set; }
        public string Source { get; set; }
        public string TimeUnit { get; set; }
    }
}
