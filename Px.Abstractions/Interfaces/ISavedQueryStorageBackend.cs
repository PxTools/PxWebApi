namespace Px.Abstractions.Interfaces
{
    public interface ISavedQueryStorageBackend
    {
        string Save(string savedQuery, string tableId, string language);
        string Load(string id);
        bool UpdateRunStatistics(string id);
    }
}
