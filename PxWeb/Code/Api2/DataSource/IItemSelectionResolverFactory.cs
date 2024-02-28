using PCAxis.Menu;

namespace PxWeb.Code.Api2.DataSource.Cnmm
{
    public interface IItemSelectionResolverFactory
    {
        Dictionary<string, ItemSelection> GetMenuLookup(string language);
    }
}
