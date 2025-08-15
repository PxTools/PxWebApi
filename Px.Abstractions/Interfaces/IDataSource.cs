namespace Px.Abstractions.Interfaces
{
    public interface IDataSource
    {
        /// <summary>
        /// Get Menu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="language"></param>
        /// <param name="selectionExists"></param>
        /// <returns></returns>
        Item? CreateMenu(string id, string language, out bool selectionExists);

        Item? LoadDatabaseStructure(string language);


        /// <summary>
        /// Gets a TableLink from the Menu. (This is only usen when Updating the search Index) 
        /// </summary>
        /// <param name="id">The "url"-id for the table</param>
        /// <param name="language"></param>
        /// <returns>The TableLink data or null if it does not exist</returns>
        TableLink? CreateMenuTableLink(string id, string language);

        /// <summary>
        /// Create builder
        /// </summary>
        /// <param name="id">Table id</param>
        /// <param name="language">Language</param>
        /// <returns></returns>
        IPXModelBuilder? CreateBuilder(string id, string language);

        /// <summary>
        /// Check if table exists
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        bool TableExists(string tableId, string language);


        /// <summary>
        /// Get a codelist by id and language
        /// </summary>
        /// <param name="id">id for the code list</param>
        /// <param name="language">codelist with texts for a specific language</param>
        /// <returns></returns>
        Codelist? GetCodelist(string id, string language);

        /// <summary> Intended for the (Lucene) indexing.
        /// Returns a list of tableid (url-type not datasource-type) for tables where published is in the intervall [from,to]
        /// For cnmm the tableid is the maintable.tableid column and time of publication is the content.published column.
        /// Does not restrict the output-list to things like PresCategory = public or table "is running", since
        /// we want to run it with internal DBs.
        /// </summary>
        /// <param name="from">Earliest. MinDate. Inclusive</param>
        /// <param name="to">Lastest. MaxDate. Inclusive</param>
        /// <returns>A list of (url-type) tableids (cnmm:maintable.tableid) which may be empty</returns>
        List<string> GetTablesPublishedBetween(DateTime from, DateTime to);
    }
}
