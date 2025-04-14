namespace Px.Abstractions.Interfaces
{
    public interface ISavedQueryStorageBackend
    {
        string Save(string savedQuery, string tableId, string language);
        string Load(string id);
        string LoadDefaultSelection(string tableId);
        bool UpdateRunStatistics(string id);
    }
}
