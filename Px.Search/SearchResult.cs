namespace Px.Search
{
    /// <summary>
    /// Is a TableInformation + Score as float 
    /// </summary>
    public class SearchResult : TableInformation
    {
        public SearchResult(string id, string label, string category, string firstPeriod, string lastPeriod, string[] variableNames, string source, string timeUnit, string subjectCode)
            : base(id, label, category, firstPeriod, lastPeriod, variableNames)
        {
            Source = source;
            TimeUnit = timeUnit;
            SubjectCode = subjectCode;
        }

        public float Score { get; set; }

    }
}
