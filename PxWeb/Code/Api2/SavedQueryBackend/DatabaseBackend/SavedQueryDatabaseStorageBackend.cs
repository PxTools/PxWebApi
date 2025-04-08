using Microsoft.Extensions.Options;

using Px.Abstractions.Interfaces;

namespace PxWeb.Code.Api2.SavedQueryBackend.DatabaseBackend
{
    public class SavedQueryDatabaseStorageBackend : ISavedQueryStorageBackend
    {
        private readonly ITablePathResolver _tablePathResolver;
        private readonly string _dataSourceType;

        public SavedQueryDatabaseStorageBackend(IOptions<DataSourceOptions> datasource, IOptions<SavedQueryDatabaseStorageOptions> savedQueryBackendOptions, ITablePathResolver tablePathResolver)
        {
            _tablePathResolver = tablePathResolver;
            _dataSourceType = datasource.Value.DataSourceType.ToUpper();

            if (string.Equals(savedQueryBackendOptions.Value.DatabaseVendor, "Oracle", StringComparison.OrdinalIgnoreCase))
            {

            }
            else if (string.Equals(savedQueryBackendOptions.Value.DatabaseVendor, "Microsoft", StringComparison.OrdinalIgnoreCase))
            {

            }
            else
            {
                throw new ArgumentException($"Database vendor '{savedQueryBackendOptions.Value.DatabaseVendor}' is not supported.");
            }
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
