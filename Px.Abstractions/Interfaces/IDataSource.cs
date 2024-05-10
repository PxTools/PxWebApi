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
    }
}
