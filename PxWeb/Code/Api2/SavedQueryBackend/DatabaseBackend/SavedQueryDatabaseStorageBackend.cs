using System.Diagnostics;

using Microsoft.Extensions.Options;

using PCAxis.Sql.SavedQuery;

using Px.Abstractions.Interfaces;

namespace PxWeb.Code.Api2.SavedQueryBackend.DatabaseBackend
{
    public class SavedQueryDatabaseStorageBackend : ISavedQueryStorageBackend
    {
        private readonly ITablePathResolver _tablePathResolver;
        private readonly ISavedQueryDatabaseAccessor _savedQueryDatabaseAccessor;

        public SavedQueryDatabaseStorageBackend(IOptions<DataSourceOptions> datasource, IOptions<SavedQueryDatabaseStorageOptions> savedQueryBackendOptions, ITablePathResolver tablePathResolver)
        {
            _tablePathResolver = tablePathResolver;
            var dataSourceType = datasource.Value.DataSourceType.ToUpper();

            if (string.Equals(savedQueryBackendOptions.Value.DatabaseVendor, "Oracle", StringComparison.OrdinalIgnoreCase))
            {
                _savedQueryDatabaseAccessor = new OracleSavedQueryDataAccessor(savedQueryBackendOptions.Value.ConnectionString, savedQueryBackendOptions.Value.TableOwner, dataSourceType, savedQueryBackendOptions.Value.TargetDatabase);
            }
            else if (string.Equals(savedQueryBackendOptions.Value.DatabaseVendor, "Microsoft", StringComparison.OrdinalIgnoreCase))
            {
                _savedQueryDatabaseAccessor = new MsSqlSavedQueryDataAccessor(savedQueryBackendOptions.Value.ConnectionString, dataSourceType, savedQueryBackendOptions.Value.TargetDatabase);
            }
            else
            {
                throw new ArgumentException($"Database vendor '{savedQueryBackendOptions.Value.DatabaseVendor}' is not supported.");
            }
        }

        public string Load(string id)
        {
            if (int.TryParse(id, out int queryId))
            {
                return _savedQueryDatabaseAccessor.Load(queryId);
            }

            return string.Empty;
        }

        public string LoadDefaultSelection(string tableId)
        {
            return _savedQueryDatabaseAccessor.LoadDefaultSelection(tableId);
        }

        public string Save(string savedQuery, string tableId, string language)
        {
            bool exists;
            var mainTable = _tablePathResolver.Resolve(language, tableId, out exists);
            if (!exists)
            {
                mainTable = "?";  //better to throw something isnt it?
            }

            return _savedQueryDatabaseAccessor.Save(savedQuery, mainTable, null).ToString();
        }

        public bool UpdateRunStatistics(string id)
        {
            if (int.TryParse(id, out int queryId))
            {
                return _savedQueryDatabaseAccessor.MarkAsRunned(queryId);
            }
            else
            {
                // Log the error (not implemented in this example)
                Debug.WriteLine($"Invalid query ID: {id}");
                return false;
            }
        }
    }
}
