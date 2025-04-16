namespace PxWeb.Code.Api2.SavedQueryBackend.FileBackend
{
    public class SavedQueryStats
    {
        public DateTime Created { get; set; }
        public DateTime? LastUsed { get; set; }
        public int UsageCount { get; set; } = 0;
    }
}
