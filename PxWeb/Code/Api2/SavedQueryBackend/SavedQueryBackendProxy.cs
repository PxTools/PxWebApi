using System.Text.Json;

using Px.Abstractions.Interfaces;

using PxWeb.Api2.Server.Models;

namespace PxWeb.Code.Api2.SavedQueryBackend
{
    public class SavedQueryBackendProxy : ISavedQueryBackendProxy
    {
        private readonly ISavedQueryStorageBackend _backend;
        public SavedQueryBackendProxy(ISavedQueryStorageBackend backend)
        {
            _backend = backend;
        }

        public SavedQuery? Load(string id)
        {
            var cleanId = SanitizeName(id);
            var savedQueryString = _backend.Load(cleanId);
            if (string.IsNullOrEmpty(savedQueryString))
            {
                return null;
            }
            return JsonSerializer.Deserialize<SavedQuery>(savedQueryString);
        }

        public string Save(SavedQuery savedQuery)
        {
            var savedQueryString = JsonSerializer.Serialize(savedQuery);
            return _backend.Save(savedQueryString);
        }

        public void UpdateRunStatistics(string id)
        {
            var cleanId = SanitizeName(id);
            _backend.UpdateRunStatistics(cleanId);
        }

        private static string SanitizeName(string id)
        {
            return id.Replace("/", "_").Replace("\\", "_").Replace(".", "_");
        }
    }
}
