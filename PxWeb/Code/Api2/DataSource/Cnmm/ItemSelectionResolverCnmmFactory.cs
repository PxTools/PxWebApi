using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

using PCAxis.Menu;
using PCAxis.Paxiom;
using PCAxis.Sql.DbConfig;

namespace PxWeb.Code.Api2.DataSource.Cnmm
{
    public class ItemSelectionResolverCnmmFactory : IItemSelectionResolverFactory
    {
        private readonly ICnmmConfigurationService _cnmmConfigurationService;
        private readonly IOptions<PxApiConfigurationOptions> _configOptions;

        public ItemSelectionResolverCnmmFactory(ICnmmConfigurationService cnmmConfigurationService, IOptions<PxApiConfigurationOptions> configOptions)
        {
            _cnmmConfigurationService = cnmmConfigurationService;
            _configOptions = configOptions;
        }

        public Dictionary<string, ItemSelection> GetMenuLookupFolders(string language)
        {
            var cnmmOptions = _cnmmConfigurationService.GetConfiguration();
            if (!SqlDbConfigsStatic.DataBases.ContainsKey(cnmmOptions.DatabaseID))
            {
                throw new PXException($"Database with id {cnmmOptions.DatabaseID} not found");
            }
            return SqlDbConfigsStatic.DataBases[cnmmOptions.DatabaseID].GetMenuLookupFolders(language, _configOptions) ?? new Dictionary<string, ItemSelection>();
        }

        public Dictionary<string, ItemSelection> GetMenuLookupTables(string language)
        {
            var cnmmOptions = _cnmmConfigurationService.GetConfiguration();
            if (!SqlDbConfigsStatic.DataBases.ContainsKey(cnmmOptions.DatabaseID))
            {
                throw new PXException($"Database with id {cnmmOptions.DatabaseID} not found");
            }
            return SqlDbConfigsStatic.DataBases[cnmmOptions.DatabaseID].GetMenuLookupTables(language, _configOptions) ?? new Dictionary<string, ItemSelection>();
        }
    }
}
