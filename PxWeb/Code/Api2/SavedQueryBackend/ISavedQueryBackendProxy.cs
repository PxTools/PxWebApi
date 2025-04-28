using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2.SavedQueryBackend
{
    public interface ISavedQueryBackendProxy
    {
        string Save(SavedQuery savedQuery);
        SavedQuery? Load(string id);
        bool UpdateRunStatistics(string id);
        SavedQuery? LoadDefaultSelection(string tableId);
    }
}
