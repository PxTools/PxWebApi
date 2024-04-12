namespace Px.Abstractions.Interfaces
{
    public interface IItemSelectionResolver
    {
        ItemSelection ResolveFolder(string language, string selection, out bool selectionExists);
        ItemSelection ResolveTable(string language, string selection, out bool selectionExists);
    }
}
