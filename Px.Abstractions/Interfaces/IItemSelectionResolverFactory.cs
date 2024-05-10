namespace Px.Abstractions.Interfaces
{
    public interface IItemSelectionResolverFactory
    {
        Dictionary<string, ItemSelection> GetMenuLookupTables(string language);

        Dictionary<string, ItemSelection> GetMenuLookupFolders(string language);
    }
}
