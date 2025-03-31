using System.Text.Json;
using System.Text.RegularExpressions;

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

            var savedQuery = JsonSerializer.Deserialize<SavedQuery>(savedQueryString);

            if (savedQuery is not null)
            {
                savedQuery.Id = cleanId;
            }

            return savedQuery;
        }

        public string Save(SavedQuery savedQuery)
        {
            var savedQueryString = JsonSerializer.Serialize(savedQuery);
            return _backend.Save(savedQueryString);
        }

        public bool UpdateRunStatistics(string id)
        {
            var cleanId = SanitizeName(id);
            return _backend.UpdateRunStatistics(cleanId);
        }

        public static string SanitizeName(string id)
        {
            return Regex.Replace(id, @"[^a-zA-Z0-9-]", "", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
        }
    }
}
