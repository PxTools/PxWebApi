
using Microsoft.Extensions.Options;

using PCAxis.Menu;
using PCAxis.Sql.Models;

using Px.Abstractions.Interfaces;

namespace PxWeb.Code.Api2.DataSource.Cnmm
{
    public class ItemSelectionResolverCnmmFactory : IItemSelectionResolverFactory
    {

        public ItemSelectionResolverCnmmFactory(ICnmmConfigurationService cnmmConfigurationService, IOptions<PxApiConfigurationOptions> configOptions)
        {

        }

        public Dictionary<string, ItemSelection> GetMenuLookupFolders(string language)
        {
            return Map(PCAxis.Sql.ApiUtils.ApiUtilStatic.GetMenuLookupFolders(language));
        }

        public Dictionary<string, ItemSelection> GetMenuLookupTables(string language)
        {
            return Map(PCAxis.Sql.ApiUtils.ApiUtilStatic.GetMenuLookupTables(language));
        }

        private static Dictionary<string, ItemSelection> Map(Dictionary<string, MenuSelectionItem> menuLookupDict)
        {
            Dictionary<string, ItemSelection> myOut = [];


            foreach (var entry in menuLookupDict)
            {
                myOut[entry.Key] = new ItemSelection(entry.Value.Menu, entry.Value.Selection);
            }
            return myOut;
        }
    }
}
