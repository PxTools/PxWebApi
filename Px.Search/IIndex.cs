namespace Px.Search
{
    /// <summary>
    /// SearchEngineWriter
    /// </summary>
    public interface IIndex : IDisposable
    {
        void BeginWrite(string language);

        void EndWrite(string language);

        void BeginUpdate(string language);

        void EndUpdate(string language);

        void AddEntry(TableInformation tbl, PXMeta meta);

        void UpdateEntry(TableInformation tbl, PXMeta meta);

        void RemoveEntry(string id);
    }
}
