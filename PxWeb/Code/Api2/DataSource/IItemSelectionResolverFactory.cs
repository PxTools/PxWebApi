using PCAxis.Menu;

namespace PxWeb.Code.Api2.DataSource.Cnmm
{
    public interface IItemSelectionResolverFactory
    {
        Dictionary<string, ItemSelection> GetMenuLookupTables(string language);

        Dictionary<string, ItemSelection> GetMenuLookupFolders(string language);
    }
}
