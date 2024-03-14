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

        public Dictionary<string, ItemSelection> GetMenuLookup(string language)
        {
            var cnmmOptions = _cnmmConfigurationService.GetConfiguration();
            if (!SqlDbConfigsStatic.DataBases.ContainsKey(cnmmOptions.DatabaseID))
            {
                throw new PXException($"Database with id {cnmmOptions.DatabaseID} not found");
            }
            return SqlDbConfigsStatic.DataBases[cnmmOptions.DatabaseID].GetMenuLookup(language, _configOptions) ?? new Dictionary<string, ItemSelection>();
        }
    }
}
