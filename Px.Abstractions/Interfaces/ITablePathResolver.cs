namespace Px.Abstractions.Interfaces
{
    public interface ITablePathResolver
    {
        string Resolve(string language, string id, out bool selectionExists);
    }
}
