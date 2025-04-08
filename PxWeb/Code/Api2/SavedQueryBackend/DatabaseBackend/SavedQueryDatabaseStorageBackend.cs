using Px.Abstractions.Interfaces;

namespace PxWeb.Code.Api2.SavedQueryBackend.DatabaseBackend
{
    public class SavedQueryDatabaseStorageBackend : ISavedQueryStorageBackend
    {
        private readonly ITablePathResolver _tablePathResolver;

        public SavedQueryDatabaseStorageBackend(ITablePathResolver tablePathResolver)
        {
            _tablePathResolver = tablePathResolver;
        }

        public string Load(string id)
        {
            throw new NotImplementedException();
        }

        public string Save(string savedQuery, string tableId, string language)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRunStatistics(string id)
        {
            throw new NotImplementedException();
        }
    }
}
